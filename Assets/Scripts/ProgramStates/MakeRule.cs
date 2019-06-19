using System;
using UnityEngine;
using UnityEngine.UI;

// typealias, (size, player who can use rule, player going after rule) : Tuple<byte, byte, byte>
using RuleSetupData = System.Tuple<byte, byte, byte>;


// items for the rule creation process
internal sealed class MakeRule : Process<MakeRule>, 
    IAssociatedState<RuleSetupData, RuleInfo>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button allPiecesButton;
    [SerializeField] internal Button doneButton;
    [SerializeField] internal Button noPieceButton;
    [SerializeField] internal Button removePieceButton;
    [SerializeField] internal Button unaffectedButton;
    [SerializeField] internal Button pieceButtonTemplate;
    [SerializeField] internal Button setTriggerPieceButton;
    [SerializeField] internal Button toggleBeforeAfterButton;
    [SerializeField] internal Button togglePanelTriggerButton;
    [SerializeField] internal InputField nameInput;
    [SerializeField] internal ScrollRect selectPieceScrView;




    /*** START ****/
    // Adds handlers to UI elements
    private void Start()
    {
        RuleCreationHandler ruleHandler = RuleCreationHandler.GetHandler();

        // 
        selectPieceScrView.SetChoosableButtons(new System.Collections.Generic.List<Button> 
            { 
                allPiecesButton,
                noPieceButton,
                removePieceButton,
                unaffectedButton,
                setTriggerPieceButton,
            });

        // makes scrollview highlight chosen item (only)
        selectPieceScrView.WhenChosenChanges
            ((scrView) =>
                delegate
                {
                    // unhighlights other buttons
                    scrView.ForEach<Button>
                    ((btn) =>
                    {
                        btn.GetComponent<Image>().color = Color.white;
                    }
                    );

                    removePieceButton.GetComponent<Image>().color = Color.white;
                    setTriggerPieceButton.GetComponent<Image>().color = Color.white;
                    allPiecesButton.GetComponent<Image>().color = Color.white;
                    noPieceButton.GetComponent<Image>().color = Color.white;
                    unaffectedButton.GetComponent<Image>().color = Color.white;

                    // highlights chosen
                    if (scrView.GetChosenItem<Button>(out Button chosen) &&
                        chosen != null)
                    {
                        switch (chosen) 
                        {
                            case Button btn when btn == setTriggerPieceButton:
                                if (ruleHandler.SelectingTriggerPiece)
                                {
                                    setTriggerPieceButton.GetComponent<Image>().color =
                                        RuleCreationHandler.setTriggerTrueColour;
                                } 
                                else 
                                {
                                    setTriggerPieceButton.GetComponent<Image>().color =
                                        RuleCreationHandler.setTriggerFalseColour;
                                }
                                break;
                            case Button otherBtn:
                                chosen.GetComponent<Image>().color =
                                RuleCreationHandler.selectedPieceColour;
                                ruleHandler.SelectingTriggerPiece = false;
                                break;
                        }
                    }
                }
            );

        // places all the pieces on square clicked
        allPiecesButton.onClick.AddListener
            (delegate
            {
                ruleHandler.PieceSelected =
                                    new RuleCreationHandler.PieceSelection
                                                           .AllPieces();

                // highlights button
                selectPieceScrView.SetChosenItem(allPiecesButton);

            });

        // only applicable if there is NOT a piece there
        noPieceButton.onClick.AddListener
            (delegate
            {
                ruleHandler.PieceSelected =
                    new RuleCreationHandler.PieceSelection
                                           .NoPiece();

                selectPieceScrView.SetChosenItem(noPieceButton);
            });

        // change piece selected when clicked
        removePieceButton.onClick.AddListener
            (delegate
            {
                ruleHandler.PieceSelected =
                    new RuleCreationHandler.PieceSelection
                                           .RemovePiece();

                // highlights button
                selectPieceScrView.SetChosenItem(removePieceButton);
            });

        // notifies handler and change button colour
        setTriggerPieceButton.onClick.AddListener
            (
                delegate
                {
                    // toggle state
                    ruleHandler.SelectingTriggerPiece =
                        !ruleHandler.SelectingTriggerPiece;

                    // unhighlights other button 
                    selectPieceScrView.SetChosenItem(setTriggerPieceButton);
                }
            );

        // switches between specifying the before and the after state of the area
        toggleBeforeAfterButton.onClick.AddListener
            (
                delegate
                {
                    // toggle between setting before/setting after
                    ruleHandler.SettingBoardAfter =
                        !ruleHandler.SettingBoardAfter;

                    if (ruleHandler.SettingBoardAfter) // if setting after state
                    {
                        toggleBeforeAfterButton.GetComponentInChildren<Text>().text =
                            "SET AREA BEFORE";
                    }
                    else // if setting before state
                    {
                        toggleBeforeAfterButton.GetComponentInChildren<Text>().text =
                            "SET AREA AFTER";
                    }
                }
            );

        // switches between panel rules and trigger rules
        togglePanelTriggerButton.onClick.AddListener
            (
                delegate
                {
                    // toggle 
                    ruleHandler.MakingTriggerRule =
                        !ruleHandler.MakingTriggerRule;

                    if (ruleHandler.MakingTriggerRule)
                    {
                        togglePanelTriggerButton.GetComponentInChildren<Text>().text =
                            "CHANGE TO PANEL RULE";
                    }
                    else
                    {
                        togglePanelTriggerButton.GetComponentInChildren<Text>().text =
                            "CHANGE TO TRIGGER RULE";
                    }
                }
            );

        // changes square selected to become unaffected
        unaffectedButton.onClick.AddListener
            (delegate
            {
                ruleHandler.PieceSelected =
                    new RuleCreationHandler.PieceSelection
                                           .Unaffected();

                selectPieceScrView.SetChosenItem(unaffectedButton);
            });
    }





    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.MakeRule;
    }
     


    public void OnEnterState(IAssociatedStateLeave<RuleSetupData> previousState, RuleSetupData setupData)
    {
        // TODO
        RuleCreationHandler ruleHandler = RuleCreationHandler.GetHandler();

        // unpacks data
        (byte areaSize, byte playerUsing, byte nextPlayer) = setupData;

        SetupUIs();
        ruleHandler.StartNewRule(areaSize, playerUsing, nextPlayer);
    }



    public RuleInfo OnLeaveState(IAssociatedStateEnter<RuleInfo> nextState)
    {
        RuleCreationHandler ruleHandler = RuleCreationHandler.GetHandler();

        // destroys board displayed
        if (ruleHandler.SettingBoardAfter) 
        {
            ruleHandler.HoloBoardAfter.DestroyBoard();
        }
        else 
        {
            ruleHandler.HoloBoardBefore.DestroyBoard();
        }

        // TODO add checks
        RuleInfo ruleMade = ruleHandler.FinalizeRule(nameInput.text);
        return ruleMade;
    }



    private void SetupUIs() 
    {
        RuleCreationHandler ruleHandler = RuleCreationHandler.GetHandler();

        // clear old name input
        nameInput.text = "";

        // configure UI during the starting state
        allPiecesButton.gameObject.SetActive(true);
        setTriggerPieceButton.gameObject.SetActive(true);
        toggleBeforeAfterButton.GetComponentInChildren<Text>().text =
            "SET AREA AFTER";
        togglePanelTriggerButton.GetComponentInChildren<Text>().text =
            "CHANGE TO PANEL RULE";

        selectPieceScrView.RepopulatePieceButtons
            (
                pieceButtonTemplate, 
                (btn, index) =>
                     ruleHandler.PieceSelected =
                                    new RuleCreationHandler
                                            .PieceSelection
                                            .OnePiece(index)
            );
    }
}
