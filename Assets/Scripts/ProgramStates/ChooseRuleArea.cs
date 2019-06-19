using System;
using UnityEngine;
using UnityEngine.UI;

// typealias, (size, player who can use rule, player going after rule) : Tuple<byte, byte, byte>
using RuleSetupData = System.Tuple<byte, byte, byte>;


// Items associated with specifying size of area affected by rule and player's turn
internal sealed class ChooseRuleArea : Process<ChooseRuleArea>, 
    IAssociatedState<GameCreationHandler, RuleSetupData>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button startMakingRuleButton;
    [SerializeField] internal InputField areaSizeInput;
    [SerializeField] internal InputField whichPlayerUseInput;
    [SerializeField] internal InputField whoseTurnAfterInput;






    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.ChooseRuleArea;
    }



    public void OnEnterState(IAssociatedStateLeave<GameCreationHandler> previousState, 
                             GameCreationHandler gameHandler)
    {
        SetupUIs();
    }



    public RuleSetupData OnLeaveState(IAssociatedStateEnter<RuleSetupData> nextState)
    {
        if (byte.TryParse(areaSizeInput.text, out byte areaSize) && 
            byte.TryParse(whichPlayerUseInput.text, out byte playerUsing) &&
            byte.TryParse(whoseTurnAfterInput.text, out byte playerAfter)) 
        {
            // TODO 
            // Add check for 0 < size <= min(#rows, #cols), playerTurn in range

            return Tuple.Create(areaSize, playerUsing, playerAfter);
        }
        else 
        {
            // TODO
            throw new System.NotImplementedException("ADD ASKING AGAIN");
        }
    }



    // clears old data in input fields
    private void SetupUIs() 
    {
        areaSizeInput.text = "";
        whichPlayerUseInput.text = "";
        whoseTurnAfterInput.text = "";
    }
}
