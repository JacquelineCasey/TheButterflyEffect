/* Battle.cs - A node that represents an ongoing conversation battle.
 * Author(s): Jacqueline
 * 
 * This class has a lot of repsonsibility, handling both the logic and the display
 * of the conversation battle. To make this more managable, it is intended to be
 * the *only* bridge between those realms - other classes should be either pure
 * logic or pure display (i.e. Card vs PhysicalCard). 
 */

using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


public partial class Battle : Node2D {
	/* Equivalent to Java's ArrayList, Rust & C++'s vector, Python's List.
	 * I call these "stretchy arrays," because they stretch out (roughly double
	 * in size) when run out of space. */
	private List<Card> draw_pile;

	private List<Card> hand = new();

	/* This is a hashtable behind the scences */
	private Dictionary<Card, PhysicalCard> hand_card_nodes = new();

	private PackedScene card_scene = GD.Load<PackedScene>("res://Scenes/PhysicalCard.tscn");

	private Curve2D hand_curve;

	private Node2D draw_pile_node;

	private HashSet<Card> cards_attempting_focus = new();
	private Card focused_card; // Often null

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		hand_curve = GetNode<Path2D>("./HandCurve").Curve;
		draw_pile_node = GetNode<Node2D>("./DrawPile");

		/* TODO: Get cards from RunInfo.cs instead. */
		draw_pile = new() {
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Besmirch.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Besmirch.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Besmirch.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Besmirch.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Besmirch.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Gaslight.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Gaslight.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Gaslight.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Gaslight.tres")),
            new(GD.Load<BaseCard>("res://Resources/BaseCards/Gaslight.tres")),
        };

		// A kinda shitty shuffle btw. O(n log n) instead of O(n). Technically only
		// 2^32 possible shuffles due to GD seed. Good enough for small lists, and
		// a non crypotgraphic applications.
		draw_pile = draw_pile.OrderBy((_) => GD.Randi()).ToList();

		foreach (Card card in draw_pile) {
			GD.Print(card.CardName());
		}

		/* Draw 5 cards? */
	}

	/* Moves a card from the draw pile to the hand. This spawns a Physical Card on
	 * the screen, and should update the position of cards in hand. */
	public void DrawCard() {
		if (draw_pile.Count == 0) {
			/* Shuffle discard pile into draw pile */
		}
		if (draw_pile.Count == 0) {
			return;
		}

		Card pulled_card = draw_pile[0];
		draw_pile.RemoveAt(0);

		PhysicalCard pulled_card_node = card_scene.Instantiate<PhysicalCard>();
		pulled_card_node.Init(pulled_card, draw_pile_node.Position);
		AddChild(pulled_card_node);

		hand.Add(pulled_card);
		hand_card_nodes[pulled_card] = pulled_card_node;

		pulled_card_node.TryFocusStart += OnCardFocus;
		pulled_card_node.FocusEnd += OnCardUnfocus;

		AdjustHandCardLocations();
	}

	public void OnCardFocus(Card card) {
		if (cards_attempting_focus.Count == 0) {
			focused_card = card;
			hand_card_nodes[card].Focus();
		}

		cards_attempting_focus.Add(card);
	}

	public void OnCardUnfocus(Card card) {
		if (card == focused_card) {
			focused_card = null;
		}

		cards_attempting_focus.Remove(card);
		AdjustHandCardLocations();

		while (cards_attempting_focus.Count != 0) {
			var next = cards_attempting_focus.First();
			if (hand_card_nodes[next].MouseHovering()) {
				focused_card = next;
				hand_card_nodes[next].Focus();
				return;
			}

			cards_attempting_focus.Remove(next);
		}
	}

	/* Tells cards in the hand where to go. */
	public void AdjustHandCardLocations() {
		Debug.Assert(hand.Count == hand_card_nodes.Count);

		float central_idx = (float) (hand.Count - 1) / 2;

		// 10 cards at most is expected, but if we go beyond this then we can cope.
		float spacing_amount = hand.Count <= 10 ? 0.1f : 1.0f / hand.Count;

		for (int i = 0; i < hand.Count; i++) {
			float curve_percent = (i - central_idx) * spacing_amount + 0.5f;

			Transform2D sample_transform = hand_curve.SampleBakedWithRotation(curve_percent * hand_curve.GetBakedLength());

			hand_card_nodes[hand[i]].SetTargetPoint(sample_transform.Origin);
			
			hand_card_nodes[hand[i]].SetTargetRotation(new Vector2(0, 1).AngleTo(sample_transform.X));

			/* Rightmost cards should render in front. */
			hand_card_nodes[hand[i]].ZIndex = i; // I think this is relative to parent?	
		}
	}

	// TEST
	private double timer = 1.0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		/* DEMO */
		
		timer -= delta;
		if (timer < 0) {
			timer += 0.125;

			DrawCard();
		}

		/* END DEMO */ 

		// Might have some silly code here to do intersections with all the things
		// on the screen that the mouse could hover over. But I might want a more
		// general system for that. Cards can be given a collision body that is
		// willing to detect inputs from the mouse, but issues might arise in the
		// case where one ends up hovering over both.
		//
		// Actually my tests in slay the spire indicate that maybe its not a big
		// deal to pick the card below the other one, if they don't overlap that
		// much and we definitily pick one while making the others reachable.
		//
		// Unrelated, but maybe hand location stuff could be sent to a dedicated
		// script on the Path2D?
	}
}
