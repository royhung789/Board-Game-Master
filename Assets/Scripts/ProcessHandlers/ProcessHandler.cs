using UnityEngine;

// class which represents handlers controlling what happens during a process
public abstract class ProcessHandler<H> : MonoBehaviour where H : ProcessHandler<H>
{
    /*** STATIC METHODS ***/
    internal static H GetHandler()
    {
        return Camera.main.GetComponent<H>();
    }
}
