using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// type alias, (# of players, # of rows, # of cols, piece resolution, relative gap size)
using DimensionsData = System.Tuple<byte, byte, byte, byte, float>;
using System;


// Items displayed on the MakeGame canvas 
internal sealed class MakeGame : Process<MakeGame>, IAssociatedStateEnter<DimensionsData>,
    IAssociatedStateEnter<BoardInfo>, IAssociatedStateEnter<PieceInfo>,
    IAssociatedStateEnter<RuleInfo>, IAssociatedStateEnter<WinCondInfo>,
    IAssociatedStateLeave<GameCreationHandler>, IAssociatedStateLeave<GameInfo>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button makePieceButton;
    [SerializeField] internal Button makeBoardButton;
    [SerializeField] internal Button makeRuleButton;
    [SerializeField] internal Button makeWinCondButton;
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

        SetupUIs();

        // uses data to create new game board and list of pieces
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();
        gameHandler.StartNewGame(data);
        
    }


    // Make Piece -> Make Game
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



    // Make Rule -> Make Game
    public void OnEnterState(IAssociatedStateLeave<RuleInfo> previousState, RuleInfo ruleMade)
    {
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();


        if (!gameHandler.rules.ContainsKey(ruleMade.usableOn)) // guard against no key 
        {
            gameHandler.rules.Add(ruleMade.usableOn, new Dictionary<byte, List<RuleInfo>>());
        }
        Dictionary<byte, List<RuleInfo>> theirMoves = gameHandler.rules[ruleMade.usableOn];


        if (!theirMoves.ContainsKey(ruleMade.triggerPiece)) 
        {
            theirMoves.Add(ruleMade.triggerPiece, new List<RuleInfo>());
        }
        List<RuleInfo> triggeredMoves = theirMoves[ruleMade.triggerPiece];


        triggeredMoves.Add(ruleMade);
    }



    public void OnEnterState(IAssociatedStateLeave<WinCondInfo> previousState, 
                             WinCondInfo winCondMade)
    {
        // add win condition
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();
        gameHandler.winConditions.Add(winCondMade);
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



    // called upon entered from ChooseDim canvas, as of the first version
    private void SetupUIs() 
    {
        // clears name input field
        nameInput.text = "";
    }





}
