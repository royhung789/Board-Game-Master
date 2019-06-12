using UnityEngine;
using System.Collections.Generic;

// script which controls the process of rule creation
public class RuleCreationHandler : ProcessHandler<RuleCreationHandler>
{
    /*** STATIC VARIABLES ***/
    // default colour of the piece button currently selected
    internal static Color selectedPieceColour =
        new Color(36 / 255f, 185 / 255f, 46 / 255f, 1);

    // colour of the set trigger piece button when in process of selecting one
    internal static Color setTriggerTrueColour =
        new Color(44 / 255f, 44 / 255f, 44 / 255f, 1);

    // colour of the set trigger piece button when not selecting one
    internal static Color setTriggerFalseColour = Color.white;





    /*** INSTANCE VARIABLES ***/
    // rule can only be activated on this player's turn (represented with a byte)
    private byte usableOn;

    // the player playing the next turn
    private byte nextPlayer;

    // the 'relative changes' of the area affected
    private RuleInfo.SquareChange[,] relChangesBeingMade;

    // visual representation of area before application of rule
    private List<PosInfo[,]>[,] areaBefore;

    // visual representation of area after application of rule
    //   note that the lists should all be singletons, as of the first VERSION
    private List<PosInfo[,]>[,] areaAfter;

    // information on the piece currently selected to be placed 
    //  (or if 'no piece' selected)
    private byte pieceSelected;

    // true iff. this is a rule which triggers on piece/board click
    //   otherwise, it is a rule activated only from the panel
    private bool makingTriggerRule;

    // true iff. selecting a piece to be the 'trigger' piece
    //   (piece which 'plays' the rule when clicked)
    private bool selectingTriggerPiece;

    // true iff. making the state of the area after application of the rule
    //   false iff. giving the state before application
    private bool settingBoardAfter;

    // index and location of the trigger piece in relChange
    //  the trigger piece is the one which activates the rule when clicked
    //  PieceInfo.noPiece for a rule that activates upon clicking an empty square
    //  PieceInfo.noSquare for panel rules (activated from scroll view during play)
    internal byte triggerPiece;
    internal byte triggerRow;
    internal byte triggerCol;






    /*** INSTANCE PROPERTIES ***/
    // virtual boards used to show area affected before and after application of rule
    internal VirtualHoloBoard HoloBoardBefore { get; set; }
    internal VirtualHoloBoard HoloBoardAfter { get; set; }

    internal byte PieceSelected { set => pieceSelected = value; }

    internal bool MakingTriggerRule
    {
        get => makingTriggerRule;
        set
        {
            makingTriggerRule = value;

            if (value)
            {
                // TODO
            }
            else
            {
                // TODO
            }
        }
    }


    internal bool SelectingTriggerPiece
    {
        get => selectingTriggerPiece;
        set
        {
            selectingTriggerPiece = value;

            if (value)
            {
                // TODO
            }
            else
            {
                // TODO
            }
        }
    }


    // when switching value, replace sub-board displayed
    internal bool SettingBoardAfter 
    { 
        get => settingBoardAfter;
        set 
        {
            settingBoardAfter = value;

            if (value) 
            { 
                // TODO
            }  
            else 
            { 
                // TODO
            }
        }
    }





    /*** INSTANCE METHDOS ***/
    // finalizes rule and creation process and returns rule created
    internal RuleInfo FinalizeRule(string ruleName) 
    {
        // TODO
        throw new System.NotImplementedException();
    }

    // remove all values used for making another rule 
    // NOTE: values assigned to null, 
    //       the method does not actually destroy old spawning slots used
    internal void StartNewRule(byte areaSize, byte playerUsingRule, byte playerAfter) 
    {
        // assigns variables according to data provided
        usableOn = playerUsingRule;
        nextPlayer = playerAfter;
        relChangesBeingMade = RuleInfo.SquareChange.GetDefaultAreaAffected(areaSize);

        // resets other variables
        pieceSelected = 0;
        selectingTriggerPiece = false;
        settingBoardAfter = false;
        makingTriggerRule = true;
    }
}
