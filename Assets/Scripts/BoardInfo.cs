// class representing information about the state of a board

[System.Serializable]
public class BoardInfo
{
    /*** INSTANCE VARIABLES ***/
    // board sizes in number of squares (which can have full pieces on top)
    public byte numOfRows;
    public byte numOfCols;

    // dimensions of the board in Unity's units
    public float height; // based on number of rows
    public float width; // based on number of columns

    // relative size of gaps between large squares, 
    //   e.g. 1 means gaps are as large as the squares
    public float sizeOfGap;
     

    // array representating the shape and colouring of the board
    public PosInfo[,] boardShapeRepresentation;


    // array representing which type of piece is where on the board
    // NOTE: array should always be of size numOfRows x numOfCold
    public byte[,] boardStateRepresentation;



    /*** CONSTRUCTORS ***/
    // generates board of numRows x numCols size without any pieces
    //  with large squares of size specified, and gaps of size specified
    private BoardInfo(byte numRows, byte numCols, 
                      float squareSizeAsScale, float gapSize) 
    {
        // TODO
        // TEMP. debug messages
        UnityEngine.Debug.Log("ATTEMPTING TO MAKE BOARD WITH " + numRows +" ROWS");
        UnityEngine.Debug.Log("ATTEMPTING TO MAKE BOARD WITH " + numCols + " COLUMNS");


        this.numOfRows = numRows;
        this.numOfCols = numCols;
        this.sizeOfGap = gapSize;
        this.boardShapeRepresentation = PosInfo.NothingMatrix(numRows,numCols);
        this.boardStateRepresentation = new byte[numRows, numCols];

        // calculate and assign dimensions,
        // NOTE: reminder, planes start off 10 units by 10 units
        float squareSize = squareSizeAsScale * 10;
        this.height = numOfRows * squareSize + // space taken by squares
            (numOfRows - 1) * sizeOfGap * squareSize; // taken by gaps

        this.width = numOfCols * squareSize + // space taken by squares
            (numOfCols - 1) * sizeOfGap * squareSize;
    }



    /*** STATIC METHODS ***/
    // creates a numRows x numCols board with specified colour,
    //  with no pieces on the board
    public static BoardInfo DefaultBoard(byte numRows, byte numCols, float sqSize,
                                         float gapSize, PosInfo.RGBData colour) 
    {
        // instantiates variables
        BoardInfo board = new BoardInfo(numRows, numCols, sqSize, gapSize);

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




    /*** INSTANCE METHODS ***/
    // returns true and assign piece at position r,c to 'piece'
    //  OR return false if unsuccessful, e.g. index out of bound
    public bool TryGetPiece(byte r, byte c, out byte piece) 
    { 
        try 
        {
            piece = this.boardStateRepresentation[r, c];
            return true;
        }  
        catch 
        {
            piece = 0; // piece must be assigned, unfortunately
            return false;
        }
    }
}
