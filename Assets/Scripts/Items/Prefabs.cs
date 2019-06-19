using UnityEngine;

// collection of objects used throughout the course of the program,
//   objects to be declared in the inspector
public class Prefabs : MonoBehaviour
{
    /*** INSTANCE VARIABLES ***/
    /** Unity Objects **/
    [SerializeField] internal PieceBuildingSlot pieceBuildingSlot;
    [SerializeField] internal PieceSpawningSlot pieceSpawningSlot;
    [SerializeField] internal PieceCubeBuildMode pieceCubeBuildMode;
    [SerializeField] internal PieceCubePlayMode pieceCubePlayMode;





    /*** STATIC METHODS ***/
    internal static Prefabs GetPrefabs() 
    { 
        return Camera.main.GetComponent<Prefabs>(); 
    }






}
