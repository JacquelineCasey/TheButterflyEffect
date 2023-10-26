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
	private float target_rotation;

	[Export]
	private float track_speed;  // What percent (0-100) towards destination are you moved each 1/60th of a second.

	/* Used for test behavior */
	private double time_since_target_move = 0;
	private Window window;

	public PhysicalCard(Card logical_card) {
		GD.Print(logical_card.CardName());
		this.logical_card = logical_card;
	}

	public override void _Ready() {
		name_label = GetNode<RichTextLabel>("./Sprite/Name");
		description_label = GetNode<RichTextLabel>("./Sprite/Description");
		/* base_card assumed to be set */

		window = GetTree().Root;

		name_label.Text = $"[center]{logical_card.CardName()}[/center]";
		description_label.Text = $"[center]{logical_card.Description()}[/center]";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		/* Temporary behavior */
		time_since_target_move += delta;

		if (time_since_target_move >= 5.0) {
			/* Next step: Move positioning logic to Battle.cs, have it spawn and
			 * move cards onto the HandCurve */

			time_since_target_move -= 5.0;

			target_point = new(
				GD.Randf() * window.ContentScaleSize.X,
				GD.Randf() * window.ContentScaleSize.Y
			);

			target_rotation = (float) GD.RandRange(-45.0, 45.0);

			GD.Print(target_point);
			GD.Print(target_rotation);
		}

		float distance_scale = (float) Mathf.Pow(1 - track_speed / 100, delta * 60);
		Position = distance_scale * (Position - target_point) + target_point;

		RotationDegrees = distance_scale * (RotationDegrees - target_rotation) + target_rotation;
	}
}
