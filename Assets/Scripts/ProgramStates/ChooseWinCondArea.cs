using System;
using UnityEngine;
using UnityEngine.UI;

// type alias, (size of winning structure in squares) : Tuple<byte>
using WinCondSetupData = System.Tuple<byte, byte>;


// class for setting up information about win conditions
internal sealed class ChooseWinCondArea : Process<ChooseWinCondArea>,
    IAssociatedState<GameCreationHandler, WinCondSetupData>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button startButton;
    [SerializeField] internal InputField sizeInput;
    [SerializeField] internal InputField winnerInput;





    /*** INSTANCE METHODS ***/
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.ChooseWinCondArea;
    }
    


    public Canvas GetCanvas()
    {
        return canvas;
    }



    public void OnEnterState(IAssociatedStateLeave<GameCreationHandler> previousState, GameCreationHandler gh)
    {
        SetupUIs();
    }



    public WinCondSetupData OnLeaveState(IAssociatedStateEnter<WinCondSetupData> nextState)
    {
        // TODO add checks
        bool success = Byte.TryParse(sizeInput.text, out byte size);
        success &= Byte.TryParse(winnerInput.text, out byte winner);
        // success = succes & check(size, winner)

        if (success) 
        {
            return Tuple.Create(size, winner);
        }
        else
        {
            // TODO
            throw new NotImplementedException("TO ADD: ASKS AGAIN");
        }
    }



    private void SetupUIs() 
    {
        // clears old input
        sizeInput.text = "";
    } 
}
