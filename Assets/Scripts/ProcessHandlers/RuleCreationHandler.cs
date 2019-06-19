using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// script which controls the process of rule creation
public class RuleCreationHandler : ProcessHandler<RuleCreationHandler>
{
    /*** INNER CLASSES ***/
    // for denoting which board states the rule may be applied
    internal class PieceSelection 
    { 
        internal class AllPieces : PieceSelection { }
        internal class Unaffected : PieceSelection { }
        internal class NoPiece : PieceSelection { }
        internal class RemovePiece : PieceSelection { }
        internal class OnePiece : PieceSelection 
        { 
            internal byte Value { get; }
            internal OnePiece(byte v) { Value = v; }
        }
    }





    /*** STATIC VARIABLES ***/
    // colour of the affected holographic squares
    internal static Color affectedSquareColour =
        new Color(20 / 255f, 200 / 255f, 20 / 255f, 0.8f);
        

    // default alpha value of a single holographic piece
    internal static byte defaultSinglePieceAlpha = 255 / 2;

    // default colour of square with no piece on it
    internal static Color noPieceColour =
        new Color(20 / 255f, 200 / 255f, 20 / 255f, 0.8f);

    // default colour of the piece button currently selected
    internal static Color selectedPieceColour =
        new Color(36 / 255f, 185 / 255f, 46 / 255f, 1);

    // colour of the set trigger piece button when in process of selecting one
    internal static Color setTriggerTrueColour =
        new Color(44 / 255f, 44 / 255f, 200 / 255f, 1);

    // colour of the set trigger piece button when not selecting one
    internal static Color setTriggerFalseColour = Color.white;

    // colour of the square the trigger piece is on
    internal static Color triggerPieceColour =
        new Color(20 / 255f, 20 / 255f, 200/ 255f, 0.8f);

    // colour of the unaffected holographic squares
    internal static Color unaffectedSquareColour =
        new Color(220 / 255f, 220 / 255f, 220 / 255f, 0.8f);
        





    /*** INSTANCE VARIABLES ***/
    // rule can only be activated on this player's turn (represented with a byte)
    private byte usableOn;

    // the player playing the next turn
    private byte nextPlayer;

    // the 'relative changes' of the area affected
    private RuleInfo.SquareChange[,] relChangesBeingMade;

    // visual representation of area before application of rule
    private List<PosInfo[,]>[,] areaBeforeRep;

    // visual representation of area after application of rule
    //   note that the lists should all be singletons, as of the first VERSION
    private List<PosInfo[,]>[,] areaAfterRep;

    // information on the piece currently selected to be placed 
    //  (or if 'no piece' selected)
    private PieceSelection pieceSelected;

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

    internal PieceSelection PieceSelected { set => pieceSelected = value; }

    internal bool MakingTriggerRule
    {
        get => makingTriggerRule;
        set 
        {
            makingTriggerRule = value;

            MakeRule mkRule = MakeRule.GetProcess();
            if (value) 
            {
                mkRule.setTriggerPieceButton.gameObject.SetActive(true);
            }
            else 
            {
                mkRule.setTriggerPieceButton.gameObject.SetActive(false);
            }
        }
    }


    internal bool SelectingTriggerPiece
    {
        get => selectingTriggerPiece;
        set => selectingTriggerPiece = value;
    }


    // when switching value, replace sub-board displayed
    internal bool SettingBoardAfter 
    { 
        get => settingBoardAfter;
        set 
        {
            settingBoardAfter = value;

            MakeRule mkRule = MakeRule.GetProcess();
            if (value) 
            {
                mkRule.allPiecesButton.gameObject.SetActive(false);
                mkRule.setTriggerPieceButton.gameObject.SetActive(false);
                HoloBoardBefore.DestroyBoard();
                HoloBoardAfter.SpawnBoard(SpatialConfigs.commonBoardOrigin);
            }  
            else 
            {
                mkRule.allPiecesButton.gameObject.SetActive(true);
                mkRule.setTriggerPieceButton.gameObject.SetActive(true);
                HoloBoardAfter.DestroyBoard();
                HoloBoardBefore.SpawnBoard(SpatialConfigs.commonBoardOrigin);
            }
        }
    }




    
    /*** INSTANCE METHDOS ***/
    // adds a piece to the square onto the board before state
    private void AddPieceBefore(PieceSelection selected, byte r, byte c) 
    {
        RuleInfo.SquareChange change = relChangesBeingMade[r, c];
        // decides what to do based on what's selected and old value
        switch (selected) 
        {
            case PieceSelection.Unaffected un:
                relChangesBeingMade[r, c] = new RuleInfo.SquareChange.Unaffected();
                break;

            case PieceSelection.RemovePiece removePce:
                List<byte> from = change.PieceChangedFromOrEmpty();
                if (from.Count > 0) // guard against empty
                {
                    from.RemoveAt(from.Count - 1); // remove latest
                    // update value
                    relChangesBeingMade[r, c] =
                        new RuleInfo.SquareChange
                                    .Changed(from, change.PieceChangedToOrNoPiece()); 
                }
                break;

            case PieceSelection.NoPiece noPce:
                relChangesBeingMade[r, c] = 
                    new RuleInfo.SquareChange
                                .Changed(new List<byte>(), 
                                         change.PieceChangedToOrNoPiece());
                break;

            case PieceSelection.AllPieces allPces:
                GameCreationHandler gh = GameCreationHandler.GetHandler();
                List<byte> ls = new List<byte>();
                for (byte i = 0; i < gh.pieces.Count; i++) 
                {
                    ls.Add(i);
                }
                                        
                relChangesBeingMade[r, c] = 
                    new RuleInfo.SquareChange
                                .Changed(ls, change.PieceChangedToOrNoPiece());
                break;

            case PieceSelection.OnePiece pce:
                List<byte> oldLs = change.PieceChangedFromOrEmpty();
                if (oldLs.Contains(pce.Value)) // toggle if it was already there
                {
                    oldLs.Remove(pce.Value);
                }
                else // otherwise just add
                {
                    oldLs.Add(pce.Value);
                    relChangesBeingMade[r, c] =
                        new RuleInfo
                                .SquareChange
                                .Changed(oldLs, change.PieceChangedToOrNoPiece());
                }
                break;

            default:
                throw new System.ArgumentException("Unaccounted for PieceSelection type");
        }
    }



    // finalizes rule and creation process and returns rule created
    internal RuleInfo FinalizeRule(string ruleName) 
    {
        // improve application speed by using Unaffected where application
        relChangesBeingMade.NoteUnaffected();

        RuleInfo ruleMade = new RuleInfo(ruleName, nextPlayer, relChangesBeingMade,
                                         triggerPiece, triggerRow, triggerCol, usableOn);

        return ruleMade;
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
        pieceSelected = new PieceSelection.Unaffected();
        selectingTriggerPiece = false;
        settingBoardAfter = false;
        makingTriggerRule = true;

        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();
        BoardCreationHandler boardHandler = BoardCreationHandler.GetHandler();

        // prepares states before and after - both starting off empty
        areaBeforeRep = new List<PosInfo[,]>[areaSize, areaSize];
        areaAfterRep = new List<PosInfo[,]>[areaSize, areaSize];

        areaBeforeRep.FillWith((i, j) => new List<PosInfo[,]>());
        areaAfterRep.FillWith((i, j) => new List<PosInfo[,]>());


        // creates virtual holo boards corresponding to states before & after
        HoloBoardBefore = new VirtualHoloBoard
            (
                unaffectedSquareColour,
                areaBeforeRep,
                areaSize,
                gameHandler.pieceResolution,
                boardHandler.BoardSquareSize,
                gameHandler.GapSize,
                (hboard, r, c) =>
                {
                    if (selectingTriggerPiece) // sets piece click triggered
                    {
                        // attempts to set trigger piece
                        switch (relChangesBeingMade[r, c]) 
                        {
                            case RuleInfo.SquareChange.Unaffected un:
                                // TODO say no
                                break;

                            case RuleInfo.SquareChange.Changed changed:
                                List<byte> from = changed.pieceChangedFrom;
                                if (from.Count == 1) 
                                {
                                    triggerPiece = from[0];
                                    triggerRow = r;
                                    triggerCol = c;

                                    HoloBoardBefore.boardColours[r, c] =
                                        triggerPieceColour;
                                }
                                else if (from.Count == 0) // when board clicked
                                {
                                    triggerPiece = PieceInfo.noPiece;
                                    triggerRow = r;
                                    triggerCol = c;
                                }
                                else 
                                {
                                    // TODO add check and complain
                                    throw new System.NotSupportedException("TODO");
                                }
                                break;

                            default:
                                throw new System.ArgumentException(
                                    "Unaccounted for SquareChange type");
                        }

                        // toggles selectingTrigger value back
                        selectingTriggerPiece = false;

                        // clears button colour
                        MakeRule.GetProcess().setTriggerPieceButton
                                             .GetComponent<Image>()
                                             .color = Color.white;
                    }
                    else
                    {
                        // places/removes piece on click 
                        AddPieceBefore(pieceSelected, r, c);
                        UpdateBeforeRep(r, c);
                    }
                }
            );
                
        HoloBoardAfter = new VirtualHoloBoard
            (
                unaffectedSquareColour,
                areaAfterRep,
                areaSize,
                gameHandler.pieceResolution,
                boardHandler.BoardSquareSize,
                gameHandler.GapSize,
                (hboard, r, c) =>
                {
                    // sets/removes piece on click
                    SetPieceAfter(pieceSelected, r, c);
                    UpdateAfterRep(r, c);
                }
            );


        // starts by spawning the holo board before
        HoloBoardBefore.SpawnBoard(SpatialConfigs.commonBoardOrigin);
    }



    // sets the piece afterwards according to what's selected
    private void SetPieceAfter(PieceSelection selected, byte r, byte c) 
    {
        RuleInfo.SquareChange change = relChangesBeingMade[r, c];
        // decides on what to do based on what's selected and old value
        switch (selected) 
        {
            case PieceSelection.Unaffected un:
                relChangesBeingMade[r, c] = new RuleInfo.SquareChange.Unaffected();
                break;

            case PieceSelection.RemovePiece removePce:
                relChangesBeingMade[r, c] =
                    new RuleInfo.SquareChange
                                .Changed(change.PieceChangedFromOrEmpty(), PieceInfo.noPiece);
                break;

            case PieceSelection.NoPiece noPce:
                relChangesBeingMade[r, c] =
                    new RuleInfo.SquareChange
                                .Changed(change.PieceChangedFromOrEmpty(), PieceInfo.noPiece);
                break;

            case PieceSelection.AllPieces allPces:
                throw new System.InvalidOperationException(
                                    "All Pieces selected in area-after state");

            case PieceSelection.OnePiece pce:
                byte changeTo;
                // toggle if piece selected is already there
                if (pce.Value == change.PieceChangedToOrNoPiece()) 
                {
                    changeTo = PieceInfo.noPiece;
                }
                else // otherwise, place piece there
                {
                    changeTo = pce.Value;
                }
                relChangesBeingMade[r, c] =
                    new RuleInfo.SquareChange
                                .Changed(change.PieceChangedFromOrEmpty(), changeTo);
                break;
        }
    }



    // updates before representation at position specified according to
    //   relChangesBeingMade 
    private void UpdateBeforeRep(int r, int c) 
    {
        // update visual representations
        GameCreationHandler gh = GameCreationHandler.GetHandler();

        switch (relChangesBeingMade[r, c]) 
        {
            case RuleInfo.SquareChange.Unaffected un:
                areaBeforeRep[r, c] = new List<PosInfo[,]>();
                areaAfterRep[r, c] = new List<PosInfo[,]>();
                break;

            case RuleInfo.SquareChange.Changed changed:
                areaBeforeRep[r, c] =
                    changed.pieceChangedFrom
                           .ConvertAll((p) => gh.pieces[p].visualRepresentation);
                break;

            default:
                throw new System.ArgumentException("Unaccounted for SquareChange type");
        }

        // update colour 
        Color colour;
        switch (relChangesBeingMade[r, c]) 
        {
            case RuleInfo.SquareChange.Unaffected un:
                colour = unaffectedSquareColour;
                break;
            case RuleInfo.SquareChange.Changed changed:
                if (changed.pieceChangedFrom.Count == 0) // for no piece
                {
                    colour = noPieceColour;
                }  
                else 
                {
                    colour = affectedSquareColour;
                }
                break;
            default:
                throw new System.ArgumentException("Unaccounted for Square Change type");
        }

        HoloBoardBefore.boardColours[r, c] = colour;
    }



    // updates after representation at position specified according to
    //   relChangesBeingMade
    private void UpdateAfterRep(int r, int c) 
    {
        // update visual representation
        GameCreationHandler gh = GameCreationHandler.GetHandler();

        switch (relChangesBeingMade[r, c]) 
        {
            case RuleInfo.SquareChange.Unaffected un:
                areaAfterRep[r, c] = new List<PosInfo[,]>();
                areaBeforeRep[r, c] = new List<PosInfo[,]>();
                break;

            case RuleInfo.SquareChange.Changed changed:
                if (changed.pieceChangedTo == PieceInfo.noPiece)
                {
                    areaAfterRep[r, c] = new List<PosInfo[,]>();
                }
                else
                {
                    areaAfterRep[r, c] = new List<PosInfo[,]>
                    {
                        gh.pieces[changed.pieceChangedTo].visualRepresentation
                    };
                }
                break;

            default:
                throw new System.ArgumentException("Unaccounted for SquareChange type");
        }

        // update colour
        Color colour;
        switch (relChangesBeingMade[r, c]) 
        {
            case RuleInfo.SquareChange.Unaffected un:
                colour = unaffectedSquareColour;
                break;

            case RuleInfo.SquareChange.Changed changed:
                if (changed.pieceChangedTo == PieceInfo.noPiece) 
                {
                    colour = noPieceColour;
                }
                else 
                {
                    colour = affectedSquareColour;
                }
                break;

            default:
                throw new System.ArgumentException("Unaccounted for Square Change type");
        }

        HoloBoardAfter.boardColours[r, c] = colour;
    }
}
