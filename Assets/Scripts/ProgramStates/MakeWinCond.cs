using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// type alias, (size of winning structure in squares) : Tuple<byte>
using WinCondSetupData = System.Tuple<byte, byte>;

// Items associated with making a win condition
internal sealed class MakeWinCond : Process<MakeWinCond>, 
    IAssociatedState<WinCondSetupData, WinCondInfo>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button anythingButton;
    [SerializeField] internal Button doneButton;
    [SerializeField] internal Button pieceButtonTemplate;
    [SerializeField] internal Button noPieceButton;
    [SerializeField] internal Button removePieceButton;
    [SerializeField] internal InputField nameInput;
    [SerializeField] internal ScrollRect selectPieceScrView;





    /*** START ***/
    private void Start()
    {
        // sets up chosen button highlighting
        selectPieceScrView.WhenChosenChanges
            ((scrView) => 
                delegate
                {
                    scrView.HighlightOnlyChosen<Button>
                    (
                        new List<Button> { removePieceButton },
                        WinCondCreationHandler.selectedPieceColour
                    );
                }
            );

        // highlights and set piece selected for buttons outside scrollview
        removePieceButton.onClick.AddListener
            (delegate 
            {
                selectPieceScrView.SetChosenItem(removePieceButton);
                WinCondCreationHandler winHandler = WinCondCreationHandler.GetHandler();
                winHandler.pieceSelected = PieceInfo.noPiece;
            });

        // highlights and say square click is not checked 
        anythingButton.onClick.AddListener
            (delegate
            {
                selectPieceScrView.SetChosenItem(anythingButton);
                WinCondCreationHandler winHandler = WinCondCreationHandler.GetHandler();
                winHandler.pieceSelected = PieceInfo.noSquare;
            });
    }





    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.MakeWinCond;
    }



    public void OnEnterState(IAssociatedStateLeave<WinCondSetupData> previousState, 
                             WinCondSetupData data)
    {
        SetupUIs();

        // unpacks data
        (byte size, byte winner) = data;

        // starts creation process
        WinCondCreationHandler winCondHandler = WinCondCreationHandler.GetHandler();
        winCondHandler.StartNewWinCond(size, winner);
    }



    public WinCondInfo OnLeaveState(IAssociatedStateEnter<WinCondInfo> nextState)
    {
        // parses name
        // TODO add check
        string nm = nameInput.text;

        WinCondCreationHandler winCondHandler = WinCondCreationHandler.GetHandler();
        WinCondInfo winCondMade = winCondHandler.FinalizeWinCond(nm);

        return winCondMade;
    }



    private void SetupUIs() 
    {
        // clear old name
        nameInput.text = "";

        // repopulates scroll view with piece buttons
        WinCondCreationHandler winHandler = WinCondCreationHandler.GetHandler();
        selectPieceScrView.Clear(pieceButtonTemplate);
        selectPieceScrView.RepopulatePieceButtons
            (
                pieceButtonTemplate,
                (btn, index) => winHandler.pieceSelected = index
            );
    }
}
