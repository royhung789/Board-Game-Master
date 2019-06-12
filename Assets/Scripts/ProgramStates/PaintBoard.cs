using UnityEngine;
using UnityEngine.UI;

// Items associated with paiting a board
internal sealed class PaintBoard : Process<PaintBoard>, IAssociatedState<Object, Object>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;
    //TODO
    




    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.PaintBoard;
    }



    public void OnEnterState(IAssociatedStateLeave<Object> previousState, Object args)
    {
        throw new System.NotImplementedException();
    }



    public Object OnLeaveState(IAssociatedStateEnter<Object> nextState)
    {
        throw new System.NotImplementedException();
    }
}
