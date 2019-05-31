using System;
using System.Collections.Generic;

// class representing a 'rule' of a custom game
//   e.g. Move Piece in Checkers for moving a piece diagonally
public class RuleInfo
{
    /*** INSTANCE VARIABLES ***/
    // the name of this rule 
    public string name;

    // for using this rule to affect the game 
    //   takes current state of game,
    //   returns all possible game states reachable from "playing" this rule
    //   If the rule cannot be applied, the list is empty
    public Func<Game, List<Game>> apply;
}
