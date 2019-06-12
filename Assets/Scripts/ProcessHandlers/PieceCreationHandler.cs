using UnityEngine;


// script which controls the behaviour of the piece creation process 
public class PieceCreationHandler : ProcessHandler<PieceCreationHandler>
{
    /*** STATIC VARIABLES ***/
    // colour of "board" for putting piece cubes on
    private static readonly PosInfo.RGBData creationSquareColour =
        new PosInfo.RGBData(255, 255, 200);





    /*** INSTANCE VARIABLES ***/
    // size of build slots
    [SerializeField] internal float buildSlotScale = 1f;

    // representation of piece being created
    internal byte pieceResolution;
    internal PosInfo[,] pieceBeingMadeRep;





    /*** INSTANCE PROPERTIES ***/
    // size of build slots
    internal float BuildSlotSize { get => buildSlotScale * 10; }

    // virtual board used in the creation of this piece 
    internal VirtualBoard<PieceBuildingSlot> VirtualBoardUsed { get; set; }






    /*** INSTANCE METHODS ***/
    // creates and returns piece that was being created
    internal PieceInfo FinalizePiece(string pceName) 
    {
        return new PieceInfo(pceName, pieceBeingMadeRep);
    }


    // Clears information about old piece while setting up for new piece creation.
    //   returns the representation board of how the piece is made up of smaller cubes
    internal BoardInfo StartNewPiece(byte pceRes) 
    {
        pieceResolution = pceRes;
        pieceBeingMadeRep = PosInfo.NothingMatrix(pceRes, pceRes);

        return BoardInfo.DefaultBoard(pceRes, pceRes, BuildSlotSize, 0.1f, creationSquareColour);
    }

}