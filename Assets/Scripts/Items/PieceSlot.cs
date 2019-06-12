using UnityEngine;
using UnityEngine.Events;

public abstract class PieceSlot : MonoBehaviour
{
    /*** STATIC VARIABLE ***/
    // fixed variables determining size and position of cube spawned
    protected static readonly Vector3 spawnOffset = new Vector3(0, 0, 0);
    // default cube size: 1x1x1, default plane size:10x10
    protected const float relScale = 10f;



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
    private VirtualBoard<PieceBuildingSlot> virtualBoardBuild;
    private VirtualBoard<PieceSpawningSlot> virtualBoardSpawn;





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
    internal VirtualBoard<Slot> GetVirtualBoard<Slot>() where Slot : PieceSlot 
    { 
        if (typeof(Slot) == typeof(PieceBuildingSlot)) 
        {
            return virtualBoardBuild as VirtualBoard<Slot>;
        } 
        else if (typeof(Slot) == typeof(PieceSpawningSlot)) 
        {
            return virtualBoardSpawn as VirtualBoard<Slot>;
        } 
        else // guard against mismatch 
        {
            Debug.Log("Attempted to recover non-build, non-spawn virtual board");
            throw new System.Exception("Invalid Type Argument");
        }
    }


    // assign this slot to a virtual board which manages it
    internal void SetVirtualBoard<Slot>(VirtualBoard<Slot> vBoard) where Slot : PieceSlot 
    { 
        switch (vBoard) 
        {
            case VirtualBoard<PieceBuildingSlot> buildBoard when this.GetType() == typeof(PieceBuildingSlot):
                virtualBoardBuild = buildBoard;
                break;
            case VirtualBoard<PieceSpawningSlot> spawnBoard when this.GetType() == typeof(PieceSpawningSlot):
                virtualBoardSpawn = spawnBoard;
                break;
            default:
                Debug.Log("Mismatched between piece-slot assigned and virtual board");
                break;
        }
    }



    // method to be called upon creation of PieceSlot game object
    internal abstract void OnUpdate();
}
