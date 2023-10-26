/* PhysicalCard.cs - A Class representing a card.
 * Author(s): Jacqueline
 * 
 * Not to be confused with the Node PhysicalCard, which is a card being displayed
 * to the user. Also not the be confused with BaseCard, which is a resource that
 * represents a card that can be obtained in the game. 
 *
 * Card will point to a base card, which is the kind of card that this is an instance
 * of. A Physical card will reference a Card, so that the physical card only cares
 * about the card on the screen, and this class can care only about the logic and
 * state of the current card. BaseCard should never change during the running of
 * the game. Card can change, if the card is upgraded or otherwise changed somehow.
 * PhysicalCard looks at Card to see these changes, and changes itself to handle
 * user input. If Card needs to notify any of its Physical Card instances (ideally
 * there is only one...), it should use Signals.
 *
 * RefCounted means that we do not need to worry about freeing this manually. RefCounted 
 * is a good candidate for classes that are important to game logic, but are not
 * actually nodes in the scene tree. */

using Godot;
using System;


public partial class Card : RefCounted {
	private BaseCard base_card;

    public Card(BaseCard base_card) {
        this.base_card = base_card;
    }

    public string CardName() { 
        return base_card.CardName;
    }

    public string Description() {
        return base_card.Description;
    }
}
