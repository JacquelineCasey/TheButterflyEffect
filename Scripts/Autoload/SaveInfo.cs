/* RunInfo.cs - A script that manages data relating to the current run.
 * Author(s): Jacqueline
 * 
 * This is an autoload class (https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html)
 * so all other nodes have easy access to it. See Autoload in project settings.
 *
 * This class represents data pertaining to the loaded save. If some game event
 * should impact the save (not just the current run), then it should talk to this
 * object. This object could also manage actually performing saving and loading.
 * However, that will require info about the run, so maybe that behavior doesn't
 * belong here?
 */

using Godot;
using System;
using System.Numerics;

public partial class SaveInfo : Node {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		/* Try an autosave every minute? */
	}
}
