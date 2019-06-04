using System;
using System.Collections.Generic;

// class representing a 'rule' of a custom game
//   e.g. Move Piece in Checkers for moving a piece diagonally
[System.Serializable]
public class RuleInfo
{
    /*** INNER CLASSES ***/
    // whether a piece in a certain square is changed by the rule or not
    [System.Serializable]
    public class SquareChange 
    { 
        private SquareChange() { } // hides default constructor

        [System.Serializable]
        public class Unaffected : SquareChange { }
        [System.Serializable]
        public class Changed : SquareChange 
        {
            public List<byte> pieceChangedFrom;

            // The piece to be replaced (or to replace the old piece)
            // NOTE: This program currently only supports one piece here
            public byte pieceChangedTo;
        }
    }

    /*** INSTANCE VARIABLES ***/
    // the name of this rule 
    public string name;

    // the 'relative changes' of the area affected
    public SquareChange[,] relChanges;

    // for using this rule to affect the game 
    //   takes current state of game,
    //   returns all possible game states reachable from "playing" this rule
    //   If the rule cannot be applied, the list is empty
    public Func<Game, List<Game>> apply;
}
