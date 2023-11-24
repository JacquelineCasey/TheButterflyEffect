/* DialogueManager.cs - A script that loads, validates, and provides access to dialogue
 * data.
 * Author(s): Jacqueline
 * 
 * This is an autoload class (https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html)
 * so all other nodes have easy access to it. See Autoload in project settings.
 *
 * The plan is to provide a dialogue markup language so that anyone writing dialogue
 * can encode the various complexities of dialogue right in the text file itself.
 * In addition to providing the dialogue lines itself, the files can also specify,
 * for example, what portrait to show, any effects to show on the dialogue itself,
 * if different dialogue should be shown based on some effect, whether to emit an
 * event based on a dialogue choice being picked, etc.
 *
 * The "ButterflyDialogueLanguage" (or ButtDiaL) will be parsed and processed here,
 * when the game loads. Any immediately detectable issues will be reported in the console right
 * when the files are parsed, so that we don't need to get to the dialogue in the game
 * in order to be confident that a tweak didn't break the dialogue.
 *
 * Then, during the game, various characters or other events can make calls to this
 * class to trigger "execution" of the dialogue.
 */


using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DialogueManager : Node {
	private string NL = System.Environment.NewLine;  // Newline for the system.
	private Dictionary<String, Dialogue> dialogues;

	// Which commands are allowed, and how many arguments do they have.
	private readonly List<(String, int)> COMMAND_SIGNATURES = new() {
		("EVENT", 1),  // Emit an event (the first argument).
		// ("SET", 2),  // Set a variable (the first argument) to a value (a string, the second argument).
	};

	public override void _Ready() {
		dialogues = new();

		GD.Print("[INFO] DialogueManager is checking dialogue files.");

		List<String> files = new(); 
		GetFilesRecursive("res://Dialogue", files);

		foreach (String path in files) {
			using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			String text = file.GetAsText();

			String dialogue_key = path.AsSpan().Slice(15, path.Length - 19).ToString();
			
			GD.Print($"[INFO] Found: {dialogue_key}");
			dialogues.Add(dialogue_key, ParseDialogue(text, dialogue_key));
		}

		DumpDialogue("test");  // TODO: Remove this test.
	}

	private void GetFilesRecursive(String dir_path, List<String> files) {
		foreach (String file in DirAccess.GetFilesAt(dir_path)) {
			files.Add(dir_path + '/' + file);
		}

		foreach (String dir in DirAccess.GetDirectoriesAt(dir_path)) {
			GetFilesRecursive(dir_path + '/' + dir, files);
		}
	}

	private Dialogue ParseDialogue(String text, String key) {
		// STEP 1: Remove comments, whitespace around lines, and empty lines.

		// LINQ is pretty cool.
		String[] lines = text.Split(System.Environment.NewLine.ToCharArray());
		lines = lines
			.Select(line => {
				line = line.Trim();
				int cut_point = line.Find("#");
				if (cut_point == -1) {
					return line;
				}

				return line[..cut_point].Trim();
			})
			.Where(line => line.Length > 0)
			.ToArray();


		// Step 2: Look at all the lines and build dialgogue units.

		DialogueUnit curr_unit = null;
        Dialogue dialogue = new() { units = new() };

        foreach (String line in lines) {
			if (line[0] == '[') {
				if (curr_unit != null) {
					dialogue.units.Add(curr_unit);
				}

				curr_unit = new();

				int cut_point = line.Find(']');
				if (cut_point == -1) {
					GD.PrintErr($"[ERROR] Error while parsing Dialogue file {key}.");
					GD.PrintErr("Could not find ']' to match '['");
					GD.PrintErr($"Problem Line: \"{line}\"");
					return null;
				}

				ParseTag(curr_unit, line[..(cut_point+1)], key);

				curr_unit.text = line[(cut_point+1)..].Trim();
			}
			else if (line[0] == '!') {
				if (curr_unit != null) {
					dialogue.units.Add(curr_unit);
				}

				String command = line[1..].Trim();
				ValidateCommand(command, key);

				dialogue.units.Add(new() {
					isCommand = true,
					text = command
                });

				curr_unit = null;
            }
			else {
				if (curr_unit == null) {
					GD.PrintErr($"[ERROR] Error while parsing Dialogue file {key}.");
					GD.PrintErr("Line is not part of Blurb or command");
					GD.PrintErr($"Problem Line: \"{line}\"");
					return null;
				}

				curr_unit.text += NL + line;
			}
		}

		if (curr_unit != null) {
			dialogue.units.Add(curr_unit);
		}

		// Step 3: Some of the dialogue units have PREV for their speaker and expressions.
		// We fill them out here.

		string speaker = null;
		string expression = null;

		foreach (var unit in dialogue.units) {
			if (unit.speaker != null) {
				if (unit.speaker == "PREV") {
					unit.speaker = speaker;
				}
				else {
					speaker = unit.speaker;
				}
			}
			if (unit.expression != null) {
				if (unit.expression == "PREV") {
					unit.expression = expression;
				}
				else {
					expression = unit.expression;
				}
			}
		}

		return dialogue;
	}

    private static void ParseTag(DialogueUnit curr_unit, string tag, string key) {
		string original_tag = tag;
		tag = tag[1..(tag.Length-1)].Trim();

		if (tag == "_") {
			return;  // No speaker, no expression.
		}
		else if (tag == "") {
			curr_unit.speaker = "PREV";
			curr_unit.expression = "PREV";
		}
		else {
			int bar_count = tag.Count("|");

			if (bar_count == 0) {
				GD.PrintErr($"[ERROR] Error while parsing Dialogue file {key}.");
				GD.PrintErr("Expected Seperator bar '|' in Tag.");
				GD.PrintErr($"Problem Tag: \"{original_tag}\"");
				return;
			}
			else if (bar_count > 1) {
				GD.PrintErr($"[ERROR] Error while parsing Dialogue file {key}.");
				GD.PrintErr("Expected only 1 seperator bar '|' in Tag.");
				GD.PrintErr($"Problem Tag: \"{original_tag}\"");
				return;
			}

			int bar_idx = tag.Find("|");
			string left = tag[0..bar_idx].Trim();
			string right = tag[(bar_idx + 1)..].Trim();

			if (left == "") {
				GD.PrintErr($"[ERROR] Error while parsing Dialogue file {key}.");
				GD.PrintErr("Expected character name to left of '|' in Tag.");
				GD.PrintErr($"Problem Tag: \"{original_tag}\"");
				return;
			}
			if (right == "") {
				GD.PrintErr($"[ERROR] Error while parsing Dialogue file {key}.");
				GD.PrintErr("Expected expression name to right of '|' in Tag.");
				GD.PrintErr($"Problem Tag: \"{original_tag}\"");
				return;
			}

			curr_unit.speaker = left;
			curr_unit.expression = right;
		}
    }

	private void ValidateCommand(string command, string key) {
        var words = command.Trim().Split(" ")
			.Where(line => line.Length > 0)
			.ToArray();

		int arg_count = words.Length - 1;
		string function = words[0];

		bool found = false;
		foreach (var (fn, argc) in COMMAND_SIGNATURES) {
			if (function != fn) {
				continue;
			}	

			if (arg_count != argc) {
				GD.PrintErr($"[ERROR] Error while parsing Dialogue file {key}.");
				GD.PrintErr($"Command has wrong number of arguments. Expected {argc}, found {arg_count}.");
				GD.PrintErr($"Problem Command: \"{command}\"");
				return;
			}

			found = true;
		}

		if (!found) {
			GD.PrintErr($"[ERROR] Error while parsing Dialogue file {key}.");
			GD.PrintErr($"Command not found.");
			GD.PrintErr($"Problem Command: \"{command}\"");
			return;
		}
    }

	private void DumpDialogue(string key) {
		Dialogue dialogue = dialogues[key];

		GD.Print($"Dialogue {key}:");

		foreach (DialogueUnit unit in dialogue.units) {
			if (unit.isCommand) {
				GD.Print($"  - Command: \"{unit.text}\"");
			}
			else {
				GD.Print($"  - Blurb: \"{unit.text}\"");
				if (unit.speaker != null) {
					GD.Print($"      + Speaker: \"{unit.speaker}\"");
				}
				if (unit.expression != null) {
					GD.Print($"      + Expression: \"{unit.expression}\"");
				}
			}
		}
	}

    private class DialogueUnit {
		public bool isCommand = false;
		public String speaker;
		public String expression;
		public String text;  // Either the text to write or the command to run. (! removed)
	}

	private class Dialogue {
		public List<DialogueUnit> units;
	}
}
