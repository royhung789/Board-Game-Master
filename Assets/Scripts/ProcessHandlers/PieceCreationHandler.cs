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
    [SerializeField] internal float buildSlotGapScale = 0.1f;

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
    internal void StartNewPiece() 
    {
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();
        byte pceRes = gameHandler.pieceResolution;
        pieceResolution = pceRes;
        pieceBeingMadeRep = PosInfo.NothingMatrix(pceRes, pceRes);

        VirtualBoard<PieceBuildingSlot> vBoard = new VirtualBoard<PieceBuildingSlot>
            (
                // TODO this is adhoc solution: vboard only used for tiling, really
                new Linked2D<PosInfo, PosInfo[,]>(pieceBeingMadeRep, (_) => pieceBeingMadeRep),
                new Linked2D<PosInfo, PosInfo>(pieceBeingMadeRep, (_) => creationSquareColour),
                1,
                BuildSlotSize,
                buildSlotGapScale,
                (brd, r, c) =>
                {
                    PosInfo curColour = pieceBeingMadeRep[r, c];
                    switch (curColour)
                    {
                        case PosInfo.RGBData colour:
                            pieceBeingMadeRep[r, c] = new PosInfo.Nothing();
                            break;
                        case PosInfo.Nothing none:
                            // TODO add ability to choose colours!
                            pieceBeingMadeRep[r, c] =
                                new PosInfo.RGBData(0, 0, 0);
                            break;
                    }
                }
            );

        VirtualBoardUsed = vBoard;
        vBoard.SpawnBoard(SpatialConfigs.commonBoardOrigin);
    }

}