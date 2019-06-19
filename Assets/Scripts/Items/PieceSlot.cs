using UnityEngine;
using UnityEngine.Events;

public abstract class PieceSlot<Slot> : MonoBehaviour where Slot : PieceSlot<Slot>
{
    /*** STATIC VARIABLE ***/
    // fixed variables determining size and position of cube spawned
    protected static readonly Vector3 spawnOffset = new Vector3(0, 0, 0);
    // default cube size: 1x1x1, default plane size:10x10
    protected const float relScale = 10f;

    // prefabs corresponding to Slot type
    internal static Slot template;


     
    /***  INSTANCE VARIABLES ***/
    // piece cube spawned above this slot 
    internal PieceCube pieceCube;

    // co-ordinates corresponding to vis.rep. indexes this object 
    //  is associated with, i.e. position inside a board's square
    internal byte pieceRow;
    internal byte pieceCol;

    // co-ordinates of the square of the board this smaller slot is in
    internal byte boardRow;
    internal byte boardCol;

    // virtual board this piece spawn slot is in (if it is)
    //   only one of these two will be used for a certain slot type
    private VirtualBoard<Slot> virtualBoard;





    /*** CONSTRUCTORS ***/
    internal PieceSlot() { }
    internal PieceSlot(byte pr, byte pc, byte br, byte bc) 
    {
        pieceRow = pr;
        pieceCol = pc;
        boardRow = br;
        boardCol = bc;
    }






    /*** INSTANCE METHODS ***/
    // recovers virtual board this piece slot is associated with
    internal VirtualBoard<Slot> GetVirtualBoard() 
    {
        return virtualBoard;
    }


    // assign this slot to a virtual board which manages it
    internal void SetVirtualBoard(VirtualBoard<Slot> vBoard)
    {
        virtualBoard = vBoard;
    }



    // method to be called upon creation of PieceSlot game object
    internal abstract void OnUpdate();





}
