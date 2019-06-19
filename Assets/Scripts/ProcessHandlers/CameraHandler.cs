using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//manages position and movement of camera
public class CameraHandler : Process<CameraHandler>    
{
    [SerializeField] private int minHeight = 100;
    [SerializeField] private int maxHeight = 1000;
    
    /*** INSTANCE METHODS ***/
    internal void moveCamera(float height)
    {
        Camera.main.transform.position = new Vector3(100, 200, 300);
    }

}
