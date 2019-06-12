using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// HoloBoard for 'Hologram Board'
// Similar to Virtual Board, but provides ability to make non-opaque pieces,
//   and to "overlay" multiple pieces on top of each other on the same square
// Uses piece spawning slots
public class VirtualHoloBoard
{
    /*** INSTANCE VARIABLES ***/
    /// <summary>
    /// Handler which is called whenever a board square is clicked.
    /// The positions (index) of the square clicked is passed in, as well,
    /// starting from (r=0, c=0) on the bottom left and increasing up and rightwards
    /// </summary>
    private readonly UnityAction<VirtualHoloBoard, byte, byte> onClickHandler;

    /// <summary>
    /// The PieceSlots used to tile this board <para />
    /// Each list corresponds to slots used to tile each board square
    /// </summary>
    private readonly List<PieceSpawningSlot>[,] slots;

    // matrix of visual representations of what's on a board square
    internal readonly List<PosInfo[,]>[,] boardRepresentation;

    // the resolution of a "hologram" on a square, usually = piece resolution
    internal readonly byte hologramResolution;

    // size of gap between squares
    internal readonly float gapSize;

    // other objects on the board (e.g. piece cubes)
    internal List<Object> otherObjsOnBoard;

    // size of a square
    internal readonly float squareSize;





    /*** INSTANCE PROPERTIES ***/
    // returns the length of area affected in board squares
    private float AreaSize
    {
        get => boardRepresentation.GetLength(0);
    }





    /*** CONSTRUCTORS ***/
    internal VirtualHoloBoard(byte sizeBoard, byte sizeHologram, float sizeSq, float sizeGap,
                              UnityAction<VirtualHoloBoard, byte, byte> handler) 
    {
        slots = new List<PieceSpawningSlot>[sizeBoard,sizeBoard];
        slots.FillWith((i, j) => new List<PieceSpawningSlot>());

        boardRepresentation = new List<PosInfo[,]>[sizeBoard, sizeBoard];
        hologramResolution = sizeHologram;
        gapSize = sizeGap;
        otherObjsOnBoard = new List<Object>();
        onClickHandler = handler;
        squareSize = sizeSq;
    }





    /*** INSTANCE METHODS ***/
    // puts the hologram on the square specified
    internal void AddHologram(byte r, byte c, PosInfo[,] hologram) 
    {
        // TODO add check to ensure hologram is correct size
        boardRepresentation[r, c].Add(hologram);
    }



    // mark the specified item as being on the board
    internal void MarkAsOnBoard(Object obj) 
    {
        otherObjsOnBoard.Add(obj);
    }



    // upon clicked, passes holoboard and position to handler, and refresh square
    internal void OnSquareClicked(byte rowPos, byte colPos)
    {
        onClickHandler(this, rowPos, colPos);
        RefreshSquare(rowPos, colPos);
    }



    // calls OnUpdateHolo method of slots in square specified
    internal void RefreshSquare(byte row, byte col) 
    {
        slots[row, col].ForEach((s) => s.OnUpdateHolo());
    }



    // spawns the "holographic board" specified
    internal void SpawnBoard(Vector3 centrePos) 
    {
        // TODO
        float sideLength = AreaSize * squareSize + (AreaSize - 1) * squareSize * gapSize;
        Vector3 bottomLeft = new Vector3(-sideLength / 2f, 0f, -sideLength / 2f);

        Vector3 start = bottomLeft + centrePos;

        // tiles and assigns appropriate variables to piece spawning slots
        PieceSpawningSlot spawnSlot = Prefabs.GetPrefabs().pieceSpawningSlot;
        byte areaSide = (byte) boardRepresentation.GetLength(0);
        float slotSize = (squareSize / 10) / hologramResolution;

        Utility.TileAct(start, spawnSlot.gameObject, slotSize,
            areaSide, areaSide, hologramResolution,
            gapSize,
            (slot, boardR, boardC, pieceR, pieceC) =>
            {
                // assigns variables
                PieceSpawningSlot slotScr =
                    slot.GetComponent<PieceSpawningSlot>();
                slotScr.pieceRow = pieceR;
                slotScr.pieceCol = pieceC;
                slotScr.boardRow = boardR;
                slotScr.boardCol = boardC;
                slotScr.SetHoloBoard(this);



                // notes that this spawning slot is currently used
                slots[boardR, boardC].Add(slotScr);

                // spawns cube at the corresponding position relative to the piece
                slotScr.OnUpdateHolo();
            });
    }



    // destroys the board, and all objects on top of it
    internal void DestroyBoard() 
    {
        // destroys slots
        foreach (List<PieceSpawningSlot> sq in slots)
        {
            sq.ForEach((obj) => Object.Destroy(obj.gameObject));
        }

        // destroys other objects
        foreach (Object obj in otherObjsOnBoard)
        {
            if (obj != null) // guard against objects already destroyed
            {
                Object.Destroy(obj);
            }
        }
    }
}
