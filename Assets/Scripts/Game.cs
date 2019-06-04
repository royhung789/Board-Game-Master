using System.Collections.Generic;


// class representing a custom game and its behaviours 
//  (that is being played or modified)
public class Game
{
    /*** INSTANCE VARIABLES ***/

    // information about (setting up) the game
    public GameInfo info;

    // the current state of the board 
    public BoardInfo boardState;

    // the spawning slots used to tile the board of the game and spawn pieces
    //   this is matrix of lists of them. 
    //   a List at a certain position in the matrix corresponds to the 
    //   slots used to spawn a piece at the same position in the board
    public List<PieceSpawningSlot>[,] spawningsSlots;




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
    public Game(BoardInfo brdStrt, List<PieceInfo> pcs) 
    {
        info = new GameInfo(brdStrt, pcs);
        boardState  = brdStrt;


        spawningsSlots =
            new List<PieceSpawningSlot>[boardState.numOfRows, boardState.numOfCols];
        // starts off with empty list at each position in spawningSlots matrix
        //  each list has max size pieceRes^2, as there are that many slots in 
        //  a square on the board
        for (byte r = 0; r < boardState.numOfRows; r++) 
        { 
            for (byte c = 0; c < boardState.numOfCols; c++) 
            {
                spawningsSlots[r, c] = new List<PieceSpawningSlot>(
                    info.pieceResolution * info.pieceResolution);
            }
        }
    }
}
