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





    /*** INSTANCE METHODS ***/
    /// <summary>
    /// Return the piece building/spawning slot specified in the type param.
    /// Usage is not ideal, but useful
    /// </summary>
    /// <returns>The slot.</returns>
    /// <typeparam name="Slot">Which type of slot to return</typeparam>
    internal Slot GetSlot<Slot>() where Slot : PieceSlot
    { 
        if (typeof(Slot) == typeof(PieceSpawningSlot)) 
        {
            return pieceSpawningSlot as Slot;
        }
        else if (typeof(Slot) == typeof(PieceBuildingSlot)) 
        {
            return pieceBuildingSlot as Slot;
        }
        else //guard against other cases 
        {
            Debug.Log("Attempted to get a non-building, non-spawning slot.");
            return null;
        }
    }
}
