using UnityEngine;

// class which stores configurations for the (relative) size and position of objects
public static class SpatialConfigs // TODO init at start ?
{
    /*** STATIC VARIABLES ***/
    // location to move camera to at the start of each process
    internal static Vector3 commonCameraPosition = new Vector3(0f, 100f, 0f);
    internal static Quaternion commonCameraOrientation = Quaternion.Euler(90f, 0f, 0f);

    // y-distance gap of board and camera 
    internal static float defaultBoardDistBelowCamera = 90f;

    /* value assigned in board creation handler currently used
    // default size of board square in units
    internal static float boardSquareSize = 4f;
    // */




    /*** STATIC PROPERTIES ***/
    // default location for spawning boards based on other variables specified
    // this is the center of the board
    internal static Vector3 commonBoardOrigin 
    {
        get 
        {
            return commonCameraPosition - 
                        new Vector3(0f, defaultBoardDistBelowCamera, 0f);
        }
    }

    // height of the board from the y = 0 plane
    internal static float HeightOfBoard 
    {
        get => commonCameraPosition.y - defaultBoardDistBelowCamera;
    }

}
