using System;
using System.Collections.Generic;
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
        where T : UnityEngine.Object // equivalent syntax in java is: <T extends Object>
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
                UnityEngine.Object.Destroy(element);
            }
        }
    }



    // above method overloaded to work with one exception without generating a list
    public static void Clear<T>(this ScrollRect scrView, T uncleared)
        where T : UnityEngine.Object // equivalent syntax in java is: <T extends Object>
    {
        foreach (T element in scrView.GetComponentsInChildren<T>())
        {
            if (!uncleared.Equals(element))
            {
                UnityEngine.Object.Destroy(element);
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
