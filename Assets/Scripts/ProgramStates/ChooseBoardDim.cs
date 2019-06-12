using UnityEngine;
using UnityEngine.UI;
// type alias, (# of players, # of rows, # of cols, piece resolution, relative gap size)
using DimensionsData = System.Tuple<byte, byte, byte, byte, float>;

// Items for entering the dimensions (of board) specification process
internal sealed class ChooseBoardDim : Process<ChooseBoardDim>, 
    IAssociatedState<UnityEngine.Object, DimensionsData>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;
    
    [SerializeField] internal Button useDimsButton;
    [SerializeField] internal InputField numPlayersInput;
    [SerializeField] internal InputField numRowsInput;
    [SerializeField] internal InputField numColsInput;
    [SerializeField] internal InputField pceResInput;
    [SerializeField] internal Slider gapSlider; // moved from MakeBoard 






    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.ChooseBoardDim;
    }


    public void OnEnterState(IAssociatedStateLeave<UnityEngine.Object> prev, UnityEngine.Object args) 
    {
        SetupUIs();
    }



    /// <summary>
    /// pass user input to Make Game process
    /// </summary>
    /// <returns>user input information</returns>
    public DimensionsData OnLeaveState(IAssociatedStateEnter<DimensionsData> _) 
    {
        // NOTE: variables can be declared right as they are used in C#
        //   so byte b; f(b); ~ f(byte b);    That's pretty neat
        //   also, 'out' just means the variable is passed/returned by reference

        // parses user input, move on to next process if inputs are valid
        if (byte.TryParse(numPlayersInput.text, out byte numPlayers) &&
            byte.TryParse(numRowsInput.text, out byte numRows) &&
            byte.TryParse(numColsInput.text, out byte numCols) &&
            byte.TryParse(pceResInput.text, out byte pceRes))
        {
            float gap = gapSlider.normalizedValue;
            // TODO check input is valid
            return System.Tuple.Create(numPlayers, numRows, numCols, pceRes, gap);
        }
        else 
        {
            // TODO Add checks -> keep going until success
            return null;
        }
    }



    // clears old data in input fields
    private void SetupUIs()
    {
        numPlayersInput.text = "";
        numRowsInput.text = "";
        numColsInput.text = "";
        pceResInput.text = "";
    }
}
