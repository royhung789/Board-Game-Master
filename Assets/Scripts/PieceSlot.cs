using UnityEngine;

public class PieceSlot : MonoBehaviour
{
    // fixed variables determining size and position of cube spawned
    protected static Vector3 spawnOffset = new Vector3(0, 0, 0);
    // default cube size: 1x1x1, default plane size:10x10
    protected static int relScale = 10;



    /***  INSTANCE VARIABLES ***/
    // the piece cube above this spawning slot 
    public PieceCubePlayMode pieceCube;

    // co-ordinates corresponding to vis.rep. indexes this object 
    //  is associated with, i.e. position inside a board's square
    public byte rowPos;
    public byte colPos;
}
