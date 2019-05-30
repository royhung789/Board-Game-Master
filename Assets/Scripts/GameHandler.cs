using UnityEngine;


// This script manages custom games in play mode, 
//  deals with what happens when user interacts with pieces and boards 
public class GameHandler : MonoBehaviour
{
    /*** INSTANCE VARIABLES ***/
    // the game currently being played 
    public Game game;


    /*** INSTANCE METHODS ***/
    public void Play() 
    {
        // TODO

        // TEMP
        Vector3 tempstart = new Vector3(-40, 10, -40);
        GameInfo gmInf = game.info;

        ButtonHandler bh = 
            this.gameObject.GetComponent<ButtonHandler>();

        // tiles and assigns appropriate variables to piece spawning slots
        Utility.TileAct(tempstart, bh.pieceSpawningSlot, bh.spawnSlotSize,
            gmInf.numOfRows, gmInf.numOfCols, gmInf.pieceResolution,
            bh.gapBetweenSlider.value,
            (slot, boardC, boardR, pieceC, pieceR) =>
            {
                // assigns variables
                PieceSpawningSlot spawnSlotScr =
                    slot.GetComponent<PieceSpawningSlot>();
                spawnSlotScr.game = game;
                spawnSlotScr.rowPos = pieceR;
                spawnSlotScr.colPos = pieceC;
                spawnSlotScr.boardRow = boardR;
                spawnSlotScr.boardCol = boardC;
                spawnSlotScr.Spawn();

                // adds object to list of item to destroy after creation process
                Utility.objsToDelete.Add(slot);
            });
    }


}
