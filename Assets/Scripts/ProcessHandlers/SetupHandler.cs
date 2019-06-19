using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;

// TODO
// REFACTOR code into smaller chunks

// Class that handles what happens when the program starts
public class SetupHandler : ProcessHandler<SetupHandler>
{
    /*** AWAKE - CALLED BEFORE START ***/
    private void Awake()
    {
        // Application.persistentDataPath links to a folder which contains
        //  information about the gamess
        //  It can only be accessed in the Start or Awake method for MonoBehaviours
        ProgramData.gamesFolderPath = Application.persistentDataPath + "/games";
    }





    /*** ACTIVATES AT START OF PROGRAM ***/
    // Start is called before the first frame update
    // assigns handler to every single button
    private void Start()
    {
        SetupStaticGenerics();

        // some setup is done in TransitionHandler, which has access
        //   to most (if not all) UI elements
        TransitionHandler th = Camera.main.GetComponent<TransitionHandler>();
        th.AddListenersToButtons();

        // creates games folder if it does not exist yet
        Directory.CreateDirectory(ProgramData.gamesFolderPath);
    }






    /*** INSTANCE METHODS ***/
    // sets up static generic variables
    private void SetupStaticGenerics() 
    {
        Prefabs prefabs = Prefabs.GetPrefabs();
        PieceSlot<PieceBuildingSlot>.template = prefabs.pieceBuildingSlot;
        PieceSlot<PieceSpawningSlot>.template = prefabs.pieceSpawningSlot;
    }
}
