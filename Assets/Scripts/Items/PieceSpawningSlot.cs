using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// class which controls behaviour of PieceSpawningSlot game objects
//   if pieces in the game are of size/resolution n by n 
//   then each square of the board will be made up of n by n of these slots
// 
// used to spawn parts of a game piece, a square-worth of them spawns 
//   an entire piece
public class PieceSpawningSlot : PieceSlot
{
    /*** INSTANCE VARIABLES ***/
    // holo board this piece slot is in (if it is)
    private VirtualHoloBoard holoBoard;





    /*** CONSTRUCTORS ***/
    internal PieceSpawningSlot(byte pieceR, byte pieceC, byte boardR, byte boardC) 
        : base(pieceR, pieceC, boardR, boardC) // this calls the parent constructor
    {

    }



    /*** INSTANCE METHODS ***/
    // returns the holo board associated with this piece spawning slot
    internal VirtualHoloBoard GetHoloBoard() 
    {
        return holoBoard;
    } 

    // called from virtual board this is in (if it is) when the associated square changes
    internal override void OnUpdate() 
    {
        Spawn();
    }



    // called from holo board this is in (if it is) when the associated square changes
    internal void OnUpdateHolo()
    {
        VirtualHoloBoard holoBrd = GetHoloBoard();
        PosInfo[,] visRep = 
            PosInfo.Overlay(holoBrd.boardRepresentation[boardRow, boardCol], 
                            holoBrd.hologramResolution);

        Spawn(visRep, out PieceCubePlayMode cubeMade); 
    }



    // Decides what to do when clicked based on current game state
    // If in process of making board, place selected piece on board
    // While playing a game, checks and applies rules which trigger 
    //   when the piece at this location is clicked
    private void OnMouseDown()
    {
        // TODO
        // TEMP debug messages
        Debug.Log("ROW POS. OF SQUARE CLICKED: " + boardRow);
        Debug.Log("COLUMN POS. OF SQUARE CLICKED: " + boardCol);

        // update boards this slot is in
        VirtualBoard<PieceSpawningSlot> vboard = GetVirtualBoard<PieceSpawningSlot>();
        if (vboard != null)
        {
            vboard.OnSquareClicked(boardRow, boardCol);
        }

        VirtualHoloBoard hboard = GetHoloBoard();
        if (hboard != null)
        {
            hboard.OnSquareClicked(boardRow, boardCol);
        }


        /*
        else if (bh.currentProgramState == SetupHandler.ProgramState.Playing) 
        {
            // recover piece above this spawning slot
            byte pieceAbove = 
                game.boardState.boardStateRepresentation[boardRow, boardCol];

            // sees all rules which can be triggered upon pressing this piece
            //   (or the board if set to no piece)
            List<RuleInfo> rulesTriggerable = game.info.rules[pieceAbove];

            // clear all previously displayed button on the rule selection scroll view
            bh.selectRuleScrView.Clear<Button>(bh.selectRuleButtonTemplate);

            foreach (RuleInfo rule in rulesTriggerable) 
            {
                // looks at possible next states
                List<Game> possibleStates = rule.Apply(game, boardRow, boardCol);

                // if the rule is not applicable (no future states), 
                //   don't make a button for it!
                if (possibleStates.Count == 0) 
                {
                    continue;
                }

                // otherwise make a button which applies the rule when clicked
                Button button =
                    Utility.CreateButton
                    (
                        bh.selectRuleButtonTemplate,
                        bh.selectRuleScrView.content, rule.name,
                        (btn) => delegate
                        {
                            //TODO TEMP.
                            // for now, choose first state
                            gh.gameBeingPlayed = possibleStates[0];

                            // clears scroll view for future use
                            bh.selectRuleScrView.Clear(bh.selectRuleScrView);
                        }
                    );
            }
        } // playing

        else if (bh.currentProgramState == SetupHandler.ProgramState.MakingRelRule) 
        { 
            //
        } // making relative/on-click rule
        */

    }



    // assigns a holo board to this piece spawning slot
    internal void SetHoloBoard(VirtualHoloBoard holoBrd) 
    {
        holoBoard = holoBrd;
    }



    // spawns the cube above this slot, if there should be one, 
    //  coloured according to visRep (visual representation) provided
    //  returns true iff. a (new) piece cube is spawned
    internal bool Spawn(PosInfo[,] visRep, out PieceCubePlayMode made) 
    {
        // TODO check on visRep dimenssions/size ?

        PieceCubePlayMode pieceCubePlayMode = Prefabs.GetPrefabs().pieceCubePlayMode;

        // destroys old cube above this slot to clear room for new cube
        // checks that it exists
        if (pieceCube != null && pieceCube.gameObject != null)
        {
            Destroy(pieceCube.gameObject); 
        }

        // get information about colour of cube above this slot, 
        //  and whether there's even one or not
        PosInfo posInfo = visRep[pieceRow, pieceCol];

        // spawns cube above if it exists accord to PosInfo specified
        switch (posInfo) 
        {
            case PosInfo.RGBData colourData: // retrives data and spawns cube
                // gets alpha
                float alpha;
                switch (colourData) 
                {
                    case PosInfo.RGBWithAlpha withAlpha:
                        alpha = withAlpha.alpha / 255f;
                        break;
                    default:
                        alpha = 1f;
                        break;
                }

                // scale and position cube based on plane's scale and position
                pieceCubePlayMode.transform.localScale =
                    transform.localScale * relScale;
                Vector3 posToSpawn = transform.position + spawnOffset;
                posToSpawn.y += pieceCubePlayMode.transform.localScale.y / 2;

                // creates the piece cube and instantiates its variables
                GameObject cubeMade
                    = Instantiate(pieceCubePlayMode.gameObject, posToSpawn, Quaternion.identity);
                PieceCubePlayMode cubeMadeScript =
                    cubeMade.GetComponent<PieceCubePlayMode>();
                cubeMadeScript.rowPos = pieceRow;
                cubeMadeScript.colPos = pieceCol;

                // change the colour of the cube to the appropriate one
                cubeMade.GetComponent<Renderer>().material.color =
                    new Color(colourData.red / 255f,
                              colourData.green / 255f,
                              colourData.blue / 255f,
                              alpha);

                // associates new cube with spawning slot
                pieceCube = cubeMadeScript;

                // returns true and assigns piece cube made
                made = cubeMadeScript;
                return true;
            case PosInfo.Nothing nothing: // no need to do anything much
                made = null;
                return false;
            default:
                throw new System.ArgumentException("PosInfo is of form unaccounted for");
        }
    }



    // spawns the cube above this slot, if there should be one, 
    //  coloured according to pieceVisualRepresentation of piece on virtual board
    internal void Spawn() 
    {
        VirtualBoard<PieceSpawningSlot> vBoard = GetVirtualBoard<PieceSpawningSlot>();

        // get information about piece above this slot, 
        //  and whether there's even one or not
        byte pieceHere = vBoard.info.BoardStateRepresentation[boardRow, boardCol];

        // only destroy cube above if there's no piece to spawn
        if (pieceHere == PieceInfo.noPiece) 
        {
            // destroys old cube above this slot to clear room for new cube
            // checks that it exists
            if (pieceCube != null && pieceCube.gameObject != null)
            {
                Destroy(pieceCube.gameObject); 
            }
        } 
        else
        {
            // see piece cube above
            PieceInfo pieceInfo = vBoard.pieces[pieceHere];
            PosInfo[,] posInfos = pieceInfo.visualRepresentation;

            // spawns it 
            if (Spawn(posInfos, out PieceCubePlayMode cubeMadeScr)) 
            {
                // note that this object should be destroyed later
                vBoard.otherObjsOnBoard.Add(cubeMadeScr.gameObject);
            }
        }
    }

}
