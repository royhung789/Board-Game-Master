using UnityEngine;
using UnityEngine.UI;

// Items associated with making a win condition
internal sealed class MakeWinCond : Process<MakeWinCond>, IAssociatedState<Object, Object>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;
    //TODO

        



    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.MakeWinCond;
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
