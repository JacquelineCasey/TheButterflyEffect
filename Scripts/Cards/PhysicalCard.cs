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
using System.Diagnostics;

public partial class PhysicalCard : Area2D {
	private RichTextLabel name_label; 
	private RichTextLabel description_label; 
	private CollisionShape2D collision_shape;

	// Represents the card in game logic. PhysicalCard just represents the card
	// as an entity on the screen. Card itself is used for logic.
	private Card logical_card;

	/* The card moves to it's target point and rotation, which must be set from 
	 * a controlloing class. */
	private Vector2 target_point;
	private float target_rotation; // radians

	private bool focused = false;
	private int no_check_unfocus_frames = 0;

	[Export]
	private float track_speed;  // What percent (0-100) towards destination are you moved each 1/60th of a second.


	/* Init is better than a constructor here, since a constructor would not create
	 * the whole scene! */
	public void Init(Card logical_card, Vector2 start_position) {
		this.logical_card = logical_card;
		this.Position = start_position;
	}

	public override void _Ready() {
		name_label = GetNode<RichTextLabel>("./Sprite/Name");
		description_label = GetNode<RichTextLabel>("./Sprite/Description");
		collision_shape = GetNode<CollisionShape2D>("./CollisionShape2D");
		
		/* logical_card assumed to be set */
		Debug.Assert(logical_card != null);

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
		if (!focused) {
			float distance_scale = (float) Mathf.Pow(1 - track_speed / 100, delta * 60);
			Position = distance_scale * (Position - target_point) + target_point;

			Rotation = distance_scale * (Rotation - target_rotation) + target_rotation;
		}
		else {
			if (no_check_unfocus_frames == 0) {
				if (!MouseHovering()) {
					EmitSignal(SignalName.FocusEnd, logical_card);
					focused = false;
					Scale = new(1, 1);
				}
			}
			else {
				no_check_unfocus_frames -= 1;
			}
 		}
	}

	[Signal]
	public delegate void TryFocusStartEventHandler(Card card);

	[Signal]
	public delegate void FocusEndEventHandler(Card card);

	/* Connected Signal */
	public void OnMouseEntered() {
		EmitSignal(SignalName.TryFocusStart, logical_card);
	}

	/* Methods for when Focus is actually granted or removed */
	public void Focus() {
		focused = true;
		no_check_unfocus_frames = 2;
		Scale = new(1.3f, 1.3f);
		Rotation = 0;
		Position = new(Position.X, 1080 - 140 * 1.3f);
		ZIndex = 20;
	}

	/* Performs a collision check to determine if the mouse is hovering.
	 * Very hacky, but using the signals for this wasn't really working probably
	 * because I was rescaling the card. Try not to call this a lot. */
	public bool MouseHovering() {
		var shape = collision_shape.Shape;
		CircleShape2D mouseShape = new() { Radius = 1 };

		return shape.Collide(Transform, mouseShape, new(0, GetGlobalMousePosition()));		
	}
}
