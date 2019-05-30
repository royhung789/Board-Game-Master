using UnityEngine;

// class representing the information needed to play a custom game 
[System.Serializable]
public class GameInfo
{
    /*** INSTANCE VARIABLES ***/
    // state of the board at the start of the game
    public BoardInfo boardAtStart;

    // board sizes in number of squares (which can have full pieces on top)
    public byte numOfRows;
    public byte numOfCols;

    // pieces of the game, with indexes used as a sort of identifier
    // NOTE: the max number of types of pieces is capped by the type of  
    //        arrays representing board states
    //        As of the first version, this is char[][], and capped at 
    //        255 pieces (not 256 due to possibility of no piece)
    public PieceInfo[] pieces;

    // the "resolution in cubes" of the piece 
    //  would be 'n' for pieces made on an n x n grid
    public byte pieceResolution;

    // number of pieces declared so far;
    // NOTE: This should be used instead of pieces.length
    public byte numOfPieces; 

    // the rules of the game
    // TODO






    /*** CONSTRUCTORS ***/
    public GameInfo(BoardInfo brdStrt, PieceInfo[] pcs)
    {
        this.boardAtStart = brdStrt;
        this.pieces = pcs;
        this.numOfRows = brdStrt.numOfRows;
        this.numOfCols = brdStrt.numOfCols;
    }



    /*** STATIC METHODS ***/
    // randomly place pieces on the empty slots on the old board
    public BoardInfo RandomPiecePlacements(BoardInfo oldBoard) 
    {
        System.Random ranGen = new System.Random();

        for (byte r = 0; r < oldBoard.numOfRows; r++) 
        { 
            for (byte c = 0; c < oldBoard.numOfCols; c++) 
            {
                byte ranPiece =  (byte)ranGen.Next(numOfPieces + 1);
                if (ranPiece == numOfPieces)
                {
                    ranPiece = PosInfo.noPiece;
                }

                oldBoard.boardStateRepresentation[r, c] =
                    ranPiece;
            }
        }

        return oldBoard;
    }
}
