/* Script.cs - a sample script
 * Author(s): Jacqueline
 * 
 * A sample script file. Prints some info to the console on object load and on
 * a few subsequent frames. */

using Godot; // Gives us things in the Godot namespace. For example, `GD`.
using System; // Not used, Godot generates it anyway since its use is probably common or something.

// We could probably put related classes in namespaces, but Godot seems to think
// that is optional

/* This names a class that inherits from Godot's all important `Node` type.
 * 
 * `partial` here means another file can have an Icon class and they are the same
 *  class. Godot uses this to add some methods to your class automatically, but I
 * think this is mostly used internally by the engine, so probably ignore it.
 * https://godotengine.org/article/whats-new-in-csharp-for-godot-4-0/#engine-interop-with-source-generators */
public partial class Sample : Node {
	[Export] // This *attribute* tells Godot to show the next field in the editor!
	private int ticks; 
	// Even private variables can be marked [Export]. Private means other classes 
	// can't access these variables, but Godot is probably cheating (for good reason) 
	// using reflection or those aformentioned added methods.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Lots of nice methods in `GD`. Anything that is global in GDScript is
		// in here.

		GD.Print("[_Ready] Hello World");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		/* Any method marked `override` is coming from a parent class (here, Node).
		 * override is *required* for that, if you don't put it you create a new method.
		 *
		 * Godot knows about this method, which is how it calls it when the node
		 * is created. */

		if (ticks > 0) {
			// C# supports have string interpolation, so chuck variables (or expressions)
			// right into strings, so long as you start with `$`.
			GD.Print($"[_Process] Tick {ticks}");
			ticks--;
		}
	}
}
