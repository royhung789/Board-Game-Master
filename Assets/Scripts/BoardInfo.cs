using UnityEngine;



// class representing information about the state of a board
[System.Serializable]
public class BoardInfo
{
    /*** INSTANCE VARIABLES ***/
    // board sizes in number of squares (which can have full pieces on top)
    public byte numOfRows;
    public byte numOfCols;
     

    // array representating the shape and colouring of the board
    public PosInfo[,] boardShapeRepresentation;


    // array representing which type of piece is where on the board
    // NOTE: array should always be of size numOfRows x numOfCold
    public byte[,] boardStateRepresentation;



    /*** CONSTRUCTORS ***/
    // generates board of numRows x numCols size without any squares
    private BoardInfo(byte numRows, byte numCols) 
    {
        this.numOfRows = numRows;
        this.numOfCols = numCols;
        this.boardShapeRepresentation = PosInfo.NothingMatrix(numRows,numCols);
        this.boardStateRepresentation = new byte[numRows, numCols];
    }



    /*** STATIC METHODS ***/

    // creates a numRows x numCols board with specified colour,
    //  with no pieces on the board
    public static BoardInfo DefaultBoard(byte numRows, byte numCols, PosInfo.RGBData colour) 
    {
        // instantiates variables
        BoardInfo board = new BoardInfo(numRows, numCols);
        board.numOfRows = numRows;
        board.numOfCols = numCols;

        // sets all squares to be filled with that colour and no pieces
        for (byte r = 0; r < numRows; r++) 
        {
            for (byte c = 0; c < numCols; c++) 
            {
                board.boardShapeRepresentation[r,c] = colour;
                board.boardStateRepresentation[r,c] = PosInfo.noPiece;
            }
        }


        return board;
    }
}
