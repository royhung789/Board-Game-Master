using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// adds extension methods to Unity UI types to make handling them easier
public static class UIExtensions
{
    /*** STATIC VARIABLES USED FOR EXTENSIONS ***/
    // keeps track of the chosen item in each scroll view
    private static readonly Dictionary<ScrollRect, UnityEngine.Object> chosenItems =
        new Dictionary<ScrollRect, UnityEngine.Object>();
    // tracks action to take when chosen item changes
    private static readonly Dictionary<ScrollRect, Func<ScrollRect, UnityAction>> whenChosenChanges =
        new Dictionary<ScrollRect, Func<ScrollRect, UnityAction>>();




    /*** EXTENSION METHODS ***/
    /// <summary>
    /// remove children objects of the type specified except for the ones in the list provided
    /// </summary>
    /// <param name="scrView">Scroll view this method extends</param>
    /// <param name="uncleared">Objects that will not be cleared from the scroll view</param>
    /// <typeparam name="T">type of objects to be destroyed</typeparam>
    public static void Clear<T>(this ScrollRect scrView, List<T> uncleared)
        where T : MonoBehaviour // equivalent syntax in java is: <T extends ...>
    {
        RectTransform content = scrView.content;
        if (content == null) // guard against null 
        {
            return;
        }

        foreach (T element in content.GetComponentsInChildren<T>()) 
        { 
            if (!uncleared.Contains(element)) 
            {
                UnityEngine.Object.Destroy(element.gameObject);
            }
        }
    }



    // above method overloaded to work with one exception without generating a list
    public static void Clear<T>(this ScrollRect scrView, T uncleared)
        where T : MonoBehaviour // equivalent syntax in java is: <T extends ...>
    {
        foreach (T element in scrView.GetComponentsInChildren<T>())
        {
            if (!uncleared.Equals(element))
            {
                UnityEngine.Object.Destroy(element.gameObject);
            }
        }
    }



    /// <summary>
    /// Applies the action to every component of type specified in the scroll view's content
    /// </summary>
    /// <param name="scrView">Scroll view this method extends</param>
    /// <param name="act">Action to be performed on each T</param>
    /// <typeparam name="T">type of objects to be acted on</typeparam>
    public static void ForEach<T>(this ScrollRect scrView, Action<T> act)
        where T : UnityEngine.Object
    {
        RectTransform content = scrView.content;
        if (content == null) // guard against null i.e. no content
        {
            return;
        }

        foreach (T elem in content.GetComponentsInChildren<T>()) 
        {
            act(elem);
        }
    }



    // attempts to get the chosen item and casts it to the specified type
    //   returns true and assign value to chosen if successful
    //   otherwise returns false
    //   Note: operation considered unsuccessful if item chosen is null
    public static bool GetChosenItem<T>(this ScrollRect scrView, out T chosen)
        where T : UnityEngine.Object
    {
        try 
        {
            chosen = chosenItems[scrView] as T; // try to retrive and cast
            return (chosen != null); // guard against null
        } 
        catch 
        {
            chosen = null;
            return false; // return false if failed
        }
    }



    // makes the scroll view highlights the chosen item the colour specified
    //   unhighlights all other items of the same type, as well as extra ones provided
    public static void HighlightOnlyChosen<T>(this ScrollRect scrView, 
                                           List<T> toUnhighlight,
                                           Color unhighlightColour,
                                           Color highlightColour)
        where T : UnityEngine.EventSystems.UIBehaviour
    {
        // unhighlights non chosen
        scrView.ForEach<T>
            (
                (obj) => obj.GetComponent<Image>().color = unhighlightColour
            );
        toUnhighlight.ForEach((obj) => obj.GetComponent<Image>().color = unhighlightColour);

        // highlights chosen
        if (scrView.GetChosenItem(out T chosen) &&
            chosen != null)
        {
            chosen.GetComponent<Image>().color = highlightColour;
        }
    }



    // above overloaded with unhighlightColour set to white
    public static void HighlightOnlyChosen<T>(this ScrollRect scrView,
                                           List<T> toUnhighlight,
                                           Color highlightColour)
        where T : UnityEngine.EventSystems.UIBehaviour
    {
        scrView.HighlightOnlyChosen(toUnhighlight, Color.white, highlightColour);
    }



    // clears and then populates scroll view with button
    //   based on template provided, is set to Chosen item when clicked,
    //   applies act(buttonMade, index) afterwards
    public static void RepopulatePieceButtons(this ScrollRect scrView, 
                                              Button template,
                                              UnityAction<Button, byte> act)
    {
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();


        // clears all old visible button on scroll view (except template)
        scrView.Clear(template);

        // populates the scroll view with buttons labeled with piece names
        for (byte index = 0; index<gameHandler.pieces.Count; index++)
        {
            // index of the associated piece 
            //  index should not be used directly in delegate, as it *will* change
            //  after this iteration of the loop ends
            // index and indexAssocPiece are kind of like up'value's in Lua
            byte indexAssocPiece = index;
            PieceInfo pce = gameHandler.pieces[index];

            // creates a button tagged with the piece name and attach it to scrollView
            Utility.CreateButton(template, scrView.content,
            pce.pieceName,
            (btn) => delegate
            {
                scrView.SetChosenItem(btn);
                // TODO TEMP DEBUG
                // index of piece selected
                Debug.Log("INDEX OF PIECE SELECTED: " + indexAssocPiece);
                act(btn, indexAssocPiece);
        });
}
    }



    // changes the colour of the image component of the UI object
    public static void SetColour<T>(this T UIElem, Color color)
        where T : UnityEngine.EventSystems.UIBehaviour
    {
        UIElem.GetComponent<Image>().color = color;
    }



    // sets chosen item to button clicked if it is in the list provided
    public static void SetChoosableButtons(this ScrollRect scrView, List<Button> buttons) 
    { 
        foreach (Button b in buttons) 
        {
            b.onClick.AddListener(delegate { scrView.SetChosenItem(b); });
        }
    }



    /// <summary>
    /// Sets the scroll view's chosen item and calls the scroll view's  
    /// when chosen changes method (if it has one)
    /// </summary>
    /// <param name="scrView">The scroll view this method extends</param>
    /// <param name="obj">The chosen item to set</param>
    public static void SetChosenItem(this ScrollRect scrView, UnityEngine.Object obj) 
    {
        // assigns chosen item
        chosenItems[scrView] = obj;

        // check for action on change
        if (whenChosenChanges.ContainsKey(scrView)) 
        {
            Func<ScrollRect, UnityAction> act = whenChosenChanges[scrView];
            if (act != null) // guard against null
            {
                act(scrView)(); // calls action returned from applying act on scrView
            }
        }
    }



    /// <summary>
    /// Specifies what to do when the scroll view's chosen item changes
    /// </summary>
    /// <param name="scrView">the scrollview extended to have this method</param>
    /// <param name="act">
    /// calls with <paramref name="scrView"/>, and calls resulting action
    /// whenever the <paramref name="scrView"/>'s chosen item changes
    /// </param>
    public static void WhenChosenChanges
        (
            this ScrollRect scrView,
            System.Func<ScrollRect, UnityAction> act
        ) 
    {
        whenChosenChanges[scrView] = act;
    }
}
