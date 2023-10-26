/* Card.cs - A Node representing a card.
 * Author(s): Jacqueline
 * 
 * See also the resource, BaseCard, which defines the base state of each card.
 * This class represents a single instance of a card (with any modifications / other
 * info), whereas BaseCard is the platonic form of the card.
 *
 * I'm not set on this architecture tbh. I might seperate further the logical card
 * (perhaps a Reference) from the card node that exists on the screen to be dragged
 * around (more of a UI thing really). */

using Godot;
using System;

public partial class Card : Node {
	private RichTextLabel name_label; 
	private RichTextLabel description_label; 

	[Export]
	private BaseCard base_card;

	public override void _Ready() {
		name_label = GetNode<RichTextLabel>("./Sprite/Name");
		description_label = GetNode<RichTextLabel>("./Sprite/Description");
		/* base_card assumed to be set */

		name_label.Text = $"[center]{base_card.CardName}[/center]";
		description_label.Text = $"[center]{base_card.Description}[/center]";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
