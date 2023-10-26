/* Script.cs - A resource representing a card.
 * Author(s): Jacqueline
 * 
 * This class inherits Resource, not Node. This means that it is attached to .tres
 * files to make them card resources. Ideally, this will be attached to one file
 * per card we add to the game, and all of the properties of each card can be defined
 * just in the inspector!
 *
 * Represents a card available in the game. This is called BaseCard because actual
 * cards (Nodes? or maybe References?) will use this to know what card they are, but
 * actual cards are free to be temporarily buffed, upgraded, etc. 
 *
 * More on resources here: https://docs.godotengine.org/en/stable/tutorials/scripting/resources.html */

using Godot;
using System;


public partial class BaseCard : Resource {
    /* Docs imply that you can serialize lots of things in resources, including
     * lists of things and maybe subresources! We might use this to add a module
     * system - i.e. attack cards get a damage module. */

    
    /* C# Properties with get and set. */

    [Export(PropertyHint.MultilineText)] public string Description { get; private set; }
        
    [Export] public string CardName { get; private set; }

    /* Not all cards will have damage, so maybe we should add an effect or module
     * system. */
    [Export] public int Damage { get; private set; } 


    /* Docs seem to think you need both a constructor and this special no argument
     * constructor. The `this(...)` is a way of invoking another constructor. */
    public BaseCard() : this("", "", 0) {}

    public BaseCard(string description, string card_name, int damage) {
        Description = description;
        CardName = card_name;
        Damage = damage;
    }
}
