using UnityEngine;
using UnityEngine.UI;

// Items displayed on the Intro canvas
internal sealed class Intro : Process<Intro>, IAssociatedState<GameInfo, Object>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button playGameButton;
    [SerializeField] internal Button makeGameButton;






    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.Intro;
    }



    public void OnEnterState(IAssociatedStateLeave<GameInfo> previousState, GameInfo args)
    {
        // TODO
        throw new System.NotImplementedException();
    }



    public Object OnLeaveState(IAssociatedStateEnter<Object> nextState)
    {
        // TODO
        return null;
    }
}
