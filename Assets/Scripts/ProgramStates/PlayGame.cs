using UnityEngine;
using UnityEngine.UI;

// Items displayed while playing a custom game
internal sealed class PlayGame : Process<PlayGame>, IAssociatedState<Game, Object>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button moveButtonTemplate;
    [SerializeField] internal ScrollRect movesScrView;
    [SerializeField] internal Text curPlayerText;





    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.PlayGame;
    }



    public void OnEnterState(IAssociatedStateLeave<Game> previousState, Game game)
    {
        // TODO
        GamePlayHandler gameHandler = GamePlayHandler.GetHandler();

        gameHandler.StartGame(game);
    }



    public Object OnLeaveState(IAssociatedStateEnter<Object> nextState)
    {
        // TODO
        throw new System.NotImplementedException();
    }
}
