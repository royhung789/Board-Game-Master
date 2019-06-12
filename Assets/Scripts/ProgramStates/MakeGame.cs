using UnityEngine;
using UnityEngine.UI;

// type alias, (# of players, # of rows, # of cols, piece resolution, relative gap size)
using DimensionsData = System.Tuple<byte, byte, byte, byte, float>;
using System;


// Items displayed on the MakeGame canvas 
internal sealed class MakeGame : Process<MakeGame>, IAssociatedStateEnter<DimensionsData>,
    IAssociatedStateEnter<BoardInfo>, IAssociatedStateEnter<PieceInfo>,
    IAssociatedStateLeave<GameCreationHandler>, IAssociatedStateLeave<GameInfo>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button makePieceButton;
    [SerializeField] internal Button makeBoardButton;
    [SerializeField] internal Button makeRuleButton;
    [SerializeField] internal Button setWinCondButton;
    [SerializeField] internal Button doneButton;
    [SerializeField] internal InputField nameInput;





    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.MakeGame;
    }



    public void OnEnterState(IAssociatedStateLeave<BoardInfo> prevState, BoardInfo boardMade) 
    {
        // TODO
        if (!(prevState is MakeBoard)) // guard against wrong state
        {
            throw new Exception("Unexpected previous state - mismatch with arguments passed");
        }

        // sets board received to be board at the start of the game
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();
        gameHandler.boardAtStart = boardMade;
    }


    public void OnEnterState(IAssociatedStateLeave<DimensionsData> prevState, DimensionsData data)
    {
        // TODO
        if (!(prevState is ChooseBoardDim)) // guard against wrong state
        {
            throw new Exception("Unexpected previous state - mismatch with arguments passed");
        }

        // uses data to create new game board and list of pieces
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();
        gameHandler.StartNewGame(data);
        
    }


    // entered after end of piece creation
    public void OnEnterState(IAssociatedStateLeave<PieceInfo> previousState, PieceInfo pieceMade)
    {
        // TODO
        if (!(previousState is MakePiece)) // guard against wrong state
        {
            throw new Exception("Unexpected previous state - mismatch with arguments passed");
        }

        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();
        // adds piece to list of pieces
        gameHandler.pieces.Add(pieceMade);
    }


    public GameCreationHandler OnLeaveState(IAssociatedStateEnter<GameCreationHandler> nextState)
    {
        // TODO
        return GameCreationHandler.GetHandler();
    }



    // Make Game -> Intro
    // finalize game, stores it in the games folder, and passes it back to Intro
    public GameInfo OnLeaveState(IAssociatedStateEnter<GameInfo> nextState)
    {
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();
        // TODO ADD CHECKS ON NAME
        GameInfo gameMade = gameHandler.FinalizeGame(nameInput.text);

        return gameMade;
    }
}
