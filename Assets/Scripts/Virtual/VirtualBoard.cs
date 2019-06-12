using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// A class for handling boards tiled by spawning slots
/// </summary>
public class VirtualBoard<Slot> where Slot : PieceSlot
{
    // TODO remove unnecessary variables and use handlers (later)
    /*** INSTANCE VARIABLES ***/
    /// <summary>
    /// The PieceSlots used to tile this board <para />
    /// Each list corresponds to slots used to tile each board square
    /// </summary>
    private readonly List<Slot>[,] slots;

    /// <summary>
    /// PieceSlot game object used as instantiation template for making
    /// more slots.
    /// </summary>
    private readonly Slot slotTemplate; 

    /// <summary>
    /// provides information about the board 
    /// </summary>
    internal readonly BoardInfo info;
    internal readonly List<PieceInfo> pieces; // pieces that may be placed on this
    private byte slotsPerSide; // normally this would correspond to pieceResolution

    // other objects on the board (e.g. piece cubes)
    internal List<Object> otherObjsOnBoard; 

    /// <summary>
    /// Handler which is called whenever a board square is clicked.
    /// The positions (index) of the square clicked is passed in, as well,
    /// starting from (r=0, c=0) on the bottom left and increasing up and rightwards
    /// </summary>
    private readonly UnityAction<VirtualBoard<Slot>, byte, byte> onClickHandler;





    /*** INSTANCE PROPERTIES ***/
    internal BoardInfo Info { get; }





    /*** CONSTRUCTOR ***/
    internal VirtualBoard(BoardInfo startBoard, List<PieceInfo> pces, byte subsq, 
                          Slot template, UnityAction<VirtualBoard<Slot>, byte, byte> handler) 
    {
        slots = new List<Slot>[startBoard.NumOfRows, startBoard.NumOfCols];
        slots.FillWith((i, j) => new List<Slot>());

        info = startBoard;
        pieces = pces;
        slotsPerSide = subsq;
        slotTemplate = template;
        otherObjsOnBoard = new List<Object>();
        onClickHandler = handler;   
    }





    /*** INSTANCE METHODS ***/
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
        Vector3 start = info.BottomLeft + centrePos;
        float slotSize = info.SquareSize / slotsPerSide;
        float slotScale = slotSize / 10f;

        // tiles and assigns appropriate variables to piece spawning slots
        Utility.TileAct(start, slotTemplate.gameObject, slotScale,
            info.NumOfRows, info.NumOfCols, slotsPerSide,
            info.GapSize,
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
