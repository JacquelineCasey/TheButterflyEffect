/* InputManager.cs - A script that does some input related things
 * Author(s): Jacqueline
 * 
 * Currently just a test / demo for some input methods. I want to test the Input
 * Map system, and see if this class can respond. In the future, we may have this
 * object let the player manipulate the input mappings (but that is a ways a way).
 *
 * Other objects could reasonably ask this object for input related information
 * but they should defer to using Godot's systems directly if possible.
 *
 * References:
 * - https://docs.godotengine.org/en/stable/classes/class_inputmap.html#class-inputmap
 * - https://docs.godotengine.org/en/stable/tutorials/inputs/inputevent.html#doc-inputevent
 * - https://docs.godotengine.org/en/stable/tutorials/inputs/input_examples.html
 */

using Godot;
using System;

public partial class InputManager : Node {
	// Another magic method called whenever Godot things input should reach this
	// node. Note: A node can optionally consume an event, so other nodes won't get it.
	// This means this method is not gauranteed to work for InputManager. This is
	// more of a demo.
	public override void _Input(InputEvent @event) {
		// Don't overthing the @ sign. `event` is a C# keyword, so people put an
		// @ sign to allow it to be an identifier.

		if (@event.IsActionPressed("walk_left")) {
			GD.Print("Left action detected.");
		}
		if (@event.IsActionPressed("walk_right")) {
			GD.Print("Right action detected.");
		}
		else if (@event.IsActionPressed("walk_up")) {
			GD.Print("Up action detected");
		}
		else if (@event.IsActionPressed("walk_down")) {
			GD.Print("Down action detected.");
		}

	}

	// If you are here I assume you may want more info on Input. There are two
	// main ways to get it. You can get it *right when it occurs* via the _Input
	// virtual method (see above). You can also continuously pull `Input` (from
	// namespace Godot) to see the current state of, say, a key.
	//
	// In both cases, you can use raw input, or you can use the InputMap stuff as
	// seen above. That is a bit nicer, since we can give names to each of the keys,
	// set multiple inputs to an action (e.g. allowing nice controller support),
	// and eventually allow to player to map inputs themselves.
	//
	// Configure the input map in Project Settings.
}
