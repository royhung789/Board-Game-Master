using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// A class for handling boards tiled by spawning slots
/// </summary>
public class VirtualBoard<Slot> where Slot : PieceSlot<Slot>
{
    /*** STATIC VARIABLES ***/
    /// <summary>
    /// PieceSlot game object used as instantiation template for making
    /// more slots.
    /// </summary>
    private readonly Slot slotTemplate = PieceSlot<Slot>.template;





    // TODO remove unnecessary variables and use handlers (later)
    /*** INSTANCE VARIABLES ***/
    /// <summary>
    /// The PieceSlots used to tile this board <para />
    /// Each list corresponds to slots used to tile each board square
    /// </summary>
    private readonly List<Slot>[,] slots;

    // size of gaps between squares
    private readonly float gapSize;

    // size of a square
    private readonly float squareSize;

    // normally this would correspond to pieceResolution
    private readonly byte slotsPerSide;

    // matrix of RGBA colours of board squares
    internal IProvider2D<int, PosInfo> boardColours;

    // matrix of visual representations of what's on a board square
    internal IProvider2D<int, PosInfo[,]> boardRepresentation;

    // other objects on the board (e.g. piece cubes)
    internal List<Object> otherObjsOnBoard; 

    /// <summary>
    /// Handler which is called whenever a board square is clicked.
    /// The positions (index) of the square clicked is passed in, as well,
    /// starting from (r=0, c=0) on the bottom left and increasing up and rightwards
    /// </summary>
    private readonly UnityAction<VirtualBoard<Slot>, byte, byte> onClickHandler;





    /*** CONSTRUCTOR ***/
    private VirtualBoard(IProvider2D<int, PosInfo[,]> linked, 
                         IProvider2D<int, PosInfo> colours,
                         int length0, int length1, byte subsize, 
                         float sizeSq, float sizeGap,
                         UnityAction<VirtualBoard<Slot>, byte, byte> handler)
    {
        slots = new List<Slot>[length0, length1];
        slots.FillWith((i, j) => new List<Slot>());

        boardRepresentation = linked;
        boardColours = colours;

        slotsPerSide = subsize;
        squareSize = sizeSq;
        gapSize = sizeGap;

        otherObjsOnBoard = new List<Object>();
        onClickHandler = handler;
    }



    internal VirtualBoard(IProvider2D<int, PosInfo[,]> linked,
                          IProvider2D<int, PosInfo> colours,
                          byte subsize, float sizeSq, float sizeGap,
                          UnityAction<VirtualBoard<Slot>, byte, byte> handler)
        : this
            (
                linked, colours, linked.GetLength(0), linked.GetLength(1),
                subsize, sizeSq, sizeGap, handler
            )
    { }



    internal VirtualBoard(byte[,] startBoard, IProvider2D<int, PosInfo> colours,
                          byte subsize, float sizeSq, float sizeGap,
                          UnityAction<VirtualBoard<Slot>, byte, byte> handler)
        : this
            (
                null, colours, startBoard.GetLength(0), startBoard.GetLength(1),
                subsize, sizeSq, sizeGap, handler
            )
    {
        // links board rep. to visual rep. based on current state of program
        if (ProgramData.currentState == ProgramData.State.PlayGame) 
        {
            GamePlayHandler playHandler = GamePlayHandler.GetHandler();
            boardRepresentation = playHandler.gameBeingPlayed
                                             .Info.LinkVisRepTo(startBoard);
        }
        else 
        {
            GameCreationHandler createHandler = GameCreationHandler.GetHandler();
            boardRepresentation = createHandler.LinkVisRepTo(startBoard);
        }
    }



    internal VirtualBoard(byte[,] startBoard, PosInfo[,] colours,
                      byte subsize, float sizeSq, float sizeGap,
                      UnityAction<VirtualBoard<Slot>, byte, byte> handler)
    : this
        (
            null, new Linked2D<PosInfo, PosInfo>(colours, Utility.Identity),
            startBoard.GetLength(0), startBoard.GetLength(1),
            subsize, sizeSq, sizeGap, handler
        )
    {
        // links board rep. to visual rep. based on current state of program
        if (ProgramData.currentState == ProgramData.State.PlayGame)
        {
            GamePlayHandler playHandler = GamePlayHandler.GetHandler();
            boardRepresentation = playHandler.gameBeingPlayed
                                             .Info.LinkVisRepTo(startBoard);
        }
        else
        {
            GameCreationHandler createHandler = GameCreationHandler.GetHandler();
            boardRepresentation = createHandler.LinkVisRepTo(startBoard);
        }
    }



    internal VirtualBoard(PosInfo[,][,] visReps, PosInfo[,] colours,
                          byte subsize, float sizeSq, float sizeGap,
                          UnityAction<VirtualBoard<Slot>, byte, byte> handler)
        : this
            (
                new Linked2D<PosInfo[,], PosInfo[,]>(visReps, Utility.Identity),
                new Linked2D<PosInfo, PosInfo>(colours, Utility.Identity),
                visReps.GetLength(0), visReps.GetLength(1),
                subsize, sizeSq, sizeGap, handler
            )
    { }



    internal VirtualBoard(byte[,] startBoard, PosInfo defaultColour,
                      byte subsize, float sizeSq, float sizeGap,
                      UnityAction<VirtualBoard<Slot>, byte, byte> handler)
    : this
        (
            null, new Linked2D<byte, PosInfo>(startBoard, (_) => defaultColour), 
            startBoard.GetLength(0), startBoard.GetLength(1),
            subsize, sizeSq, sizeGap, handler
        )
    {
        // links board rep. to visual rep. based on current state of program
        if (ProgramData.currentState == ProgramData.State.PlayGame)
        {
            GamePlayHandler playHandler = GamePlayHandler.GetHandler();
            boardRepresentation = playHandler.gameBeingPlayed
                                             .Info.LinkVisRepTo(startBoard);
        }
        else
        {
            GameCreationHandler createHandler = GameCreationHandler.GetHandler();
            boardRepresentation = createHandler.LinkVisRepTo(startBoard);
        }
    }



    internal VirtualBoard(PosInfo[,][,] visReps, PosInfo defaultColour, 
                          byte subsize, float sizeSq, float sizeGap,
                          UnityAction<VirtualBoard<Slot>, byte, byte> handler)
        : this
            (
                new Linked2D<PosInfo[,], PosInfo[,]>(visReps, Utility.Identity),
                new Linked2D<PosInfo[,], PosInfo>(visReps, (_) => defaultColour),
                visReps.GetLength(0), visReps.GetLength(1),
                subsize, sizeSq, sizeGap, handler
            )
    { }




    /*** INSTANCE METHODS ***/
    // refresh every single square on the board
    internal void RefreshBoard() 
    { 
        for (byte r = 0; r < boardRepresentation.GetLength(0); r++) 
        { 
            for (byte c = 0; c < boardRepresentation.GetLength(1); c++) 
            {
                RefreshSquare(r, c);
            }
        }
    }



    /// <summary>
    /// Calls the OnCreate method of the slots tiling the square at the 
    /// position specified
    /// </summary>
    /// <param name="row">row the square is in</param>
    /// <param name="col">column the square is in</param>
    internal void RefreshSquare(byte row, byte col)
    {
        slots[row, col].ForEach((s) => s.OnUpdate());
    }



    /// <summary>
    /// Spawns the board (slots) into the game world according to the 
    /// board info specified during construction.
    /// </summary>
    /// <param name="centrePos">position of the centre of the board</param>
    internal void SpawnBoard(Vector3 centrePos) 
    {
        // calculates where to start tiling
        int rows = boardRepresentation.GetLength(0);
        int cols = boardRepresentation.GetLength(1);
        float height = rows * squareSize + (rows - 1) * squareSize * gapSize;
        float width = cols * squareSize + (cols - 1) * squareSize * gapSize;
        Vector3 bottomLeft = new Vector3(-width / 2, 0, -height / 2);
        // correction due to spawning at centre, not bottom left
        float subSlotSize = squareSize / slotsPerSide;
        bottomLeft.x += subSlotSize / 2;
        bottomLeft.z += subSlotSize / 2; 

        Vector3 start = bottomLeft + centrePos;
        float slotSize = squareSize / slotsPerSide;
        float slotScale = slotSize / 10f;

        // tiles and assigns appropriate variables to piece spawning slots
        Utility.TileAct(start, slotTemplate.gameObject, slotScale,
            (byte)rows, (byte)cols, slotsPerSide,
            gapSize,
            (slot, boardR, boardC, pieceR, pieceC) =>
            {
                // assigns variables
                Slot slotScr =
                    slot.GetComponent<Slot>();
                slotScr.pieceRow = pieceR;
                slotScr.pieceCol = pieceC;
                slotScr.boardRow = boardR;
                slotScr.boardCol = boardC;
                slotScr.SetVirtualBoard(this); 

                

                // notes that this spawning slot is currently used
                slots[boardR, boardC].Add(slotScr);

                // spawns cube at the corresponding position relative to the piece
                slotScr.OnUpdate();
            });
    }


    /// <summary>
    /// Calls handler when a board square is clicked with position of square
    /// and passes in this VirtualBoard. <para />
    /// Also notifies slot that it has been clicked and updates it
    /// </summary>
    internal void OnSquareClicked(byte rowPos, byte colPos) 
    {
        onClickHandler(this, rowPos, colPos);
        RefreshSquare(rowPos, colPos);
    }


    /// <summary>
    /// Destroy all of the piece slots used in this board,
    ///  as well as any other objects placed on it
    /// </summary>
    internal void DestroyBoard() 
    { 
        foreach (List<Slot> sq in slots) 
        {
            sq.ForEach((obj) => Object.Destroy(obj.gameObject));
        }

        foreach (Object obj in otherObjsOnBoard) 
        { 
            if (obj != null) // guard against objects already destroyed
            {
                Object.Destroy(obj);
            }
        }
    }
}
