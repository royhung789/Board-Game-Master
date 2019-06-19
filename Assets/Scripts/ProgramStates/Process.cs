using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class denoting a "process" which may take place throughout the program
public abstract class Process<P> : MonoBehaviour where P : Process<P>
{
    /*** STATIC METHODS ***/
    internal static P GetProcess()
    {
        return Camera.main.GetComponent<P>();
    }
}

