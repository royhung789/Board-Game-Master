using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;



// collection of useful variables and methods useful throughout the program
public class Utility : MonoBehaviour
{
    /*** STATIC VARIABLES ***/
    // all the game objects to delete after a (creation/play) session
    public static List<GameObject> objsToDelete = new List<GameObject>();



    /*** STATIC METHODS ***/
    // clones button from template with specified text and put it in location 
    //  activates action, with button made passed in as parameter, on click
    public static Button CreateButton(Button template, Component location,
                                      string text, Func<Button, UnityAction> action)
    {
        // creates button, set text, activates it
        Button button = Instantiate(template);
        button.GetComponentInChildren<Text>().text = text;
        button.gameObject.SetActive(true);

        // places button where specified by setting it as a children of location
        button.transform.SetParent(location.transform, false);

        // apply action whenever clicked
        button.onClick.AddListener(action(button));
        return button;
    }

    // overloads CreateButton to be able to take an action with no parameters
    public static Button CreateButton(Button template, Component location,
                                      string text, UnityAction action)
    {
        return CreateButton(template, location, text, (_) => action);
    }



    // clears and recreates games folder, REMOVES ALL SAVED GAMES!!!
    public static void DeleteAllSavedGames()
    {
        // recreates directory if it is not there
        Directory.CreateDirectory(ProgramData.gamesFolderPath);
        // removes all files inside
        foreach (string path in Directory.GetFiles(ProgramData.gamesFolderPath))
        {
            File.Delete(path);
        }
    }


    // delete objects queued for deletion (all objects in objsToDelete)
    //  and clear the list for re-use
    public static void DeleteQueuedObjects()
    {
        // delete each object
        foreach (GameObject obj in objsToDelete)
        {
            Destroy(obj);
        }
        objsToDelete.Clear(); // empty list for reuse
    }


    // checks that string entered is alphanumeric, 
    //  and is not the empty string ("")
    //  does NOT check for name crashes
    public static bool EnsureProperName(string str)
    {
        //TODO
        return true;
    }



    // this makes planes
    public static void Tile(Vector3 start, GameObject plane, float width, byte sqsXDir, byte sqsZDir,
                            byte sizeSmall)
    {
        TileAct(start, plane, width, sqsXDir, sqsZDir, sizeSmall, 0.5f,
            (_, _x, _z, _xSm, _zSm) => { });
    }


    // Utility function used to tile boards, while performing extraAct for each square made,
    //  passing in the object instantiated and the four loop variables
    //                                              (denoting which small/large squares it's in)
    // NOTE: width is the local scale of the smaller squares
    //  gap between large squares = 
    public static void TileAct(Vector3 start, GameObject plane, float width, byte sqsZDir, byte sqsXDir, 
                            byte sizeSmall, float relSpcBtwnSqs, Action<GameObject, byte, byte, byte, byte> extraAct)
    {
        //A, B, C variables method -- here for reminding/remembering what each variable does
        // int a ~ sqsZDir;     //length in z direction
        // int b ~ sqsXDir;     //length in x direction
        // int c ~ sizeSmall;     //small square dimensions (c by c)

        float widthPlane = width * 10; // plane is 10 x 10 default
        float spaceSmall = widthPlane;  //distance to move to start the next square
        float spaceLarge = widthPlane * sizeSmall * (1 + relSpcBtwnSqs);

        // c = xSmall & zSmall loop, b = x loop, a = z loop
        for (byte x = 0; x < sqsXDir; x++)
        {
            for (byte z = 0; z < sqsZDir; z++)
            {
                for (byte xSmall = 0; xSmall < sizeSmall; xSmall++)
                {
                    for (byte zSmall = 0; zSmall < sizeSmall; zSmall++)
                    {
                        plane.transform.localScale = new Vector3(width, width, width);   //size

                        //For position, move only first and last coordinates
                        float posX = start.x + spaceLarge * x + spaceSmall * xSmall;
                        float posZ = start.z + spaceLarge * z + spaceSmall * zSmall;

                        //initial position is 'start'
                        GameObject objMade = 
                            Instantiate(plane, new Vector3(posX, start.y, posZ), Quaternion.identity);

                        // do this method with arguments of object instantiated,
                        //  position of large square in board, 
                        //  position of small slot in large square
                        extraAct(objMade, z, x, zSmall, xSmall);



                    }

                }
            }
        }


    }
}
