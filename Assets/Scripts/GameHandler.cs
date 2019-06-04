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

        // retrievs game info, calculate where to start tiling
        GameInfo gmInf = game.info;
        BoardInfo startBoard = gmInf.boardAtStart;
        Vector3 start = new Vector3(-startBoard.width / 2, 10, -startBoard.height / 2);

        ButtonHandler bh = 
            this.gameObject.GetComponent<ButtonHandler>();

        float spawnSlotSize = bh.boardSquareSize / gmInf.pieceResolution;

        // tiles and assigns appropriate variables to piece spawning slots
        Utility.TileAct(start, bh.pieceSpawningSlot, spawnSlotSize,
            gmInf.numOfRows, gmInf.numOfCols, gmInf.pieceResolution,
            gmInf.boardAtStart.sizeOfGap,
            (slot, boardR, boardC, pieceR, pieceC) =>
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
