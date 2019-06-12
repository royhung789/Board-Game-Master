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
        // some setup is done in TransitionHandler, which has access
        //   to most (if not all) UI elements
        TransitionHandler th = Camera.main.GetComponent<TransitionHandler>();
        th.AddListenersToButtons();

        // creates games folder if it does not exist yet
        Directory.CreateDirectory(ProgramData.gamesFolderPath);
    }






    /*** INSTANCE METHODS ***/

    /* TODO TO DELETE
    // DELETE EVERY GAME STORED INSIDE OF THE GAMES FOLDER
    //  THAT IS WHERE ALL GAMES ARE LOCATED WHEN CREATED WITH THIS PROGRAM!
    public void DeleteAllGames() 
    {
        if (numTimesDeleteAllGamesClickedSinceDeletion > 0) 
        {
            // deletes all games, and resets the content of the scroll view
            Utility.DeleteAllSavedGames();

            // hide warning text
            areYouSureText.text = "";

            // reset num of times it has been clicked since last deletion
            numTimesDeleteAllGamesClickedSinceDeletion = 0;

            // change warning text back
            deleteAllGamesButton.GetComponentInChildren<Text>().text = 
                "DELETE ALL GAMES";

            // updates game state
            currentProgramState = ProgramData.Intro;

            // switches canvas back to main/intro canvas 
            //  (there's no game to choose to play anymore, why stay there?)
            chooseGameCanvas.gameObject.SetActive(false);
            introCanvas.gameObject.SetActive(true);
        } 
        else 
        {
            // asks, if has not asked already
            areYouSureText.text = "ARE YOU SURE?";
            deleteAllGamesButton.GetComponentInChildren<Text>().text = "YES!!!";

            numTimesDeleteAllGamesClickedSinceDeletion++; //increment count
        }
    }


    // appends information about game just created to a file which 
    //  stores information about all playable games
    public void DoneGame() 
    {
        // using .gam extension to stand for 'game' since there
        //  are no obvious conventions... 


        // Check if file (game with same name) already exists
        // check that name is proper (alphanum, non-empty)
        // TODO
        string gameName = enterGameNameInputField.text;
        enterGameNameInputField.text = ""; //reset text in input field

        // name file according to game, prepare to put in games folder
        // spaces are replaced with underscrolls for developpers' convenience
        //  shall be replaced back with spaces when displayed
        string gamePath = gamesFolderPath +
            "/" + (gameName.Replace(' ', '_')) + ".gam";



        // TODO
        // TEMP 
        // creates a file called TEMP_(id#) inside the games folder to 
        //  store all information about created game
        FileStream gameFile = File.Open(gamePath, FileMode.Create);

        // TEMP
        // serializes game data to file 
        //  temporarily tests serializing a '0'
        BinaryFormatter binFmt = new BinaryFormatter();
        binFmt.Serialize(gameFile, gameBeingMade.info);


        // closes the file
        gameFile.Close();

        // updates state
        currentProgramState = ProgramData.Intro;

        // switches back to main screen
        makeGameCanvas.gameObject.SetActive(false);
        introCanvas.gameObject.SetActive(true);

    }
    */  
}
