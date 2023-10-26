/* RunInfo.cs - A script that manages data relating to the current run.
 * Author(s): Jacqueline
 * 
 * This is an autoload class (https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html)
 * so all other nodes have easy access to it. See Autoload in project settings.
 *
 * This class represents data pertaining to the current run. What is the state of
 * the wider world? What cards are in the player's deck? How much health do they
 * have? How much time do they have left? I think the answers to these questions
 * can all be stored here.
 *
 * Admittedly, it might be smarter to split some of those capabilities into other
 * classes. Hard to say right now if that will end up being worth it.
 */

using Godot;
using System;

public partial class RunInfo : Node {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

	}
}
