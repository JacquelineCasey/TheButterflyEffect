/* PhysicalCard.cs - A Node representing a card.
 * Author(s): Jacqueline
 * 
 * See also the resource, BaseCard, which defines the base state of each card.
 * This class represents a physical card on the screen that the player can look
 * at and potentially play.
 *
 * I'm not set on this architecture tbh. I might seperate further the logical card
 * (perhaps a Reference) from the card node that exists on the screen to be dragged
 * around (more of a UI thing really). */

using Godot;
using System;

public partial class PhysicalCard : Node2D {
	private RichTextLabel name_label; 
	private RichTextLabel description_label; 

	// Represents the card in game logic. PhysicalCard just represents the card
	// as an entity on the screen. Card itself is used for logic.
	private Card logical_card;

	/* The card moves to it's target point and rotation, which must be set from 
	 * a controlloing class. */
	private Vector2 target_point;
	private float target_rotation; // radians

	[Export]
	private float track_speed;  // What percent (0-100) towards destination are you moved each 1/60th of a second.


	/* Init is better than a constructor here, since a constructor would not create
	 * the whole scene! */
	public void Init(Card logical_card) {
		this.logical_card = logical_card;
	}

	public override void _Ready() {
		name_label = GetNode<RichTextLabel>("./Sprite/Name");
		description_label = GetNode<RichTextLabel>("./Sprite/Description");
		/* base_card assumed to be set */

		// window = GetTree().Root;

		name_label.Text = $"[center]{logical_card.CardName()}[/center]";
		description_label.Text = $"[center]{logical_card.Description()}[/center]";
	}

	public void SetTargetPoint(Vector2 point) {
		target_point = point;
	}

	public void SetTargetRotation(float radians) {
		target_rotation = radians;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		float distance_scale = (float) Mathf.Pow(1 - track_speed / 100, delta * 60);
		Position = distance_scale * (Position - target_point) + target_point;

		Rotation = distance_scale * (Rotation - target_rotation) + target_rotation;
	}
}
