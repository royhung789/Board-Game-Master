using UnityEngine;


// class representing a custom game (that is being played) and its behaviours
public class Game
{
    /*** INSTANCE VARIABLES ***/

    // information about (setting up) the game
    public GameInfo info;

    // the current state of the board 
    public BoardInfo boardState;




    /*** CONSTRUCTORS ***/
    // instantiates with given game info and board state 
    public Game(GameInfo gInf, BoardInfo bInf) 
    {
        info = gInf;
        boardState = bInf;
    }

    // instantiates with given game info in the board state at start of game
    //  the syntax here is just C# constructor chaining
    public Game(GameInfo gInf) : this(gInf, gInf.boardAtStart) { }

    // Starts a game with board in brdStrt state, using pcs as pieces
    public Game(BoardInfo brdStrt, PieceInfo[] pcs) 
    {
        info = new GameInfo(brdStrt, pcs);
        boardState  = brdStrt;
    }
}
