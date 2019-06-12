using UnityEngine;

// class representing information about the state of a board
[System.Serializable]
public class BoardInfo
{
    /*** INSTANCE VARIABLES ***/
    private PosInfo[,] boardShapeRepresentation;
    private byte[,] boardStateRepresentation;





    /*** INSTANCE PROPERTIES ***/
    // board sizes in number of squares (which can have full pieces on top)
    public byte NumOfRows { get; }
    public byte NumOfCols { get; }
    public float SquareSize { get; }// size of the board square 

    // relative size of gaps between large squares, 
    //   e.g. 1 means gaps are as large as the squares
    public float GapSize { get; }
     

    // array representating the shape and colouring of the board
    internal PosInfo[,] BoardShapeRepresentation { get => boardShapeRepresentation; }


    // array representing which type of piece is where on the board
    // NOTE: array should always be of size numOfRows x numOfCold
    internal byte[,] BoardStateRepresentation 
    { 
        get => boardStateRepresentation; 
        set => boardStateRepresentation = value;
    }


    // dimensions of the board in Unity's units
    public float Height
    {
        get
        {
            return NumOfRows * SquareSize + // space taken by squares
                   (NumOfRows - 1) * GapSize * SquareSize; // taken by gaps 
        }
    }
    public float Width
    {
        get
        {
            return NumOfCols * SquareSize + // space taken by squares
                   (NumOfCols - 1) * GapSize * SquareSize; // taken by gaps
        }
    }

    // vector bottom left corner of the board (projected onto plane y=0)
    public Vector3 BottomLeft 
    { 
        get 
        {
            return new Vector3(-Width/2, 0, -Height/2);
        }
    }



    /*** CONSTRUCTORS ***/
    // hidden default constructor
    private BoardInfo() { }

    // generates board of numRows x numCols size without any pieces
    //  with large squares of size specified, and gaps of size specified
    private BoardInfo(byte numRows, byte numCols, 
                      float sqSize, float gapSize) 
    {
        // TODO
        // TEMP. debug messages
        UnityEngine.Debug.Log("ATTEMPTING TO MAKE BOARD WITH " + numRows +" ROWS");
        UnityEngine.Debug.Log("ATTEMPTING TO MAKE BOARD WITH " + numCols + " COLUMNS");


        NumOfRows = numRows;
        NumOfCols = numCols;
        GapSize = gapSize;
        SquareSize = sqSize;
        boardShapeRepresentation = PosInfo.NothingMatrix(numRows,numCols); //TODO
        boardStateRepresentation = new byte[numRows, numCols];
    }



    /*** STATIC METHODS ***/
    // creates a numRows x numCols board with specified colour,
    //  with no pieces on the board
    internal static BoardInfo DefaultBoard(byte numRows, byte numCols, float sqSize,
                                         float gapSize, PosInfo.RGBData colour) 
    {
        // instantiates variables
        BoardInfo board = new BoardInfo(numRows, numCols, sqSize, gapSize);

        // sets all squares to be filled with that colour and no pieces
        for (byte r = 0; r < numRows; r++) 
        {
            for (byte c = 0; c < numCols; c++) 
            {
                board.BoardShapeRepresentation[r,c] = colour;
                board.BoardStateRepresentation[r,c] = PieceInfo.noPiece;
            }
        }


        return board;
    }




    /*** INSTANCE METHODS ***/
    // CAREFUL! Returns by-value copy BUT with same pointer for ShapeRepresentation
    //  i.e. result.boardShapeRepresentation == this.boardShapeRepresentation still
    public BoardInfo GetCopy() 
    {
        // assigns all variables 
        BoardInfo result = new BoardInfo(NumOfRows, NumOfCols, SquareSize, GapSize);
        result.boardShapeRepresentation = BoardShapeRepresentation; //same ref

        // copies state representation by value
        result.boardStateRepresentation = 
            this.boardStateRepresentation.Clone() as byte[,];

        return result;
    }

    // returns true and assign piece at position r,c to 'piece'
    //  OR return false if unsuccessful, e.g. index out of bound
    //  Note that even if indexing succeeds, this may still return false,
    //   due to the lack of a board square at that index
    public bool TryGetPiece(byte r, byte c, out byte piece) 
    { 
        try 
        {
            PosInfo pieceRep = boardShapeRepresentation[r, c];
            piece = boardStateRepresentation[r, c];

            // guards against case where there is no board square there
            if (pieceRep is PosInfo.Nothing) 
            {
                return false;
            }

            return true;
        }  
        catch 
        {
            piece = 0; // piece must be assigned, unfortunately
            return false;
        }
    }
}
