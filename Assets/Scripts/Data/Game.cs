using System.Collections.Generic;


// class representing a custom game and its behaviours 
//  (that is being played or modified)
public class Game
{
    /*** INSTANCE VARIABLES ***/
    // the current player during this turn 
    internal byte currentPlayer;

    // the current state of the board 
    internal BoardInfo boardState;





    /*** INSTANCE PROPERTIES ***/
    // information about (setting up) the game
    public GameInfo Info { get; }




    /*** CONSTRUCTORS ***/
    // instantiates with given game info, board state, and current player
    internal Game(GameInfo gInfo, BoardInfo bInfo, byte curPlayer) 
    {
        Info = gInfo;
        boardState = bInfo;
        currentPlayer = curPlayer;
    }



    // instantiates with given game info in the board state at start of game
    //  the syntax here is just C# constructor chaining
    internal Game(GameInfo gInfo) : 
        this(gInfo, gInfo.boardAtStart, gInfo.startingPlayer) { }





    /*** INSTANCE METHODS ***/
    // TODO TEMP: turn this into full-blown evaluation function for AI?
    // true iff. this and the otherGame has same board state and current player
    //   note that the colouring of the boards are ignored
    internal bool SameStateAs(Game otherGame)
    {
        // ensure board states are the same
        // first, ensure sizes are the same
        if ((boardState.NumOfRows != otherGame.boardState.NumOfRows) ||
            (boardState.NumOfCols != otherGame.boardState.NumOfCols))
        {
            return false; // otherwise, they clearly are not the same
        }

        // check board states equality, square/piece-wise 
        for (byte r = 0; r < boardState.NumOfRows; r++) 
        { 
            for (byte c = 0; c < boardState.NumOfCols; c++) 
            { 
                if (boardState.BoardStateRepresentation[r, c] != 
                    otherGame.boardState.BoardStateRepresentation[r, c]) 
                {
                    return false; // false if a single piece/square mismatched
                }
            }
        }


        // if boards are same, true iff. current players are the same, also
        return (currentPlayer == otherGame.currentPlayer);
    }
}
