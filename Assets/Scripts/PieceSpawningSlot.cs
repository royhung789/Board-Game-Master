using UnityEngine;

// class which controls behaviour of PieceSpawningSlot game objects
//   if pieces in the game are of size/resolution n by n 
//   then each square of the board will be made up of n by n of these slots
// 
// used to spawn parts of a game piece, a square-worth of them spawns 
//   an entire piece
public class PieceSpawningSlot : PieceSlot
{
    /*** INSTANCE VARIABLES ***/
    // cubes to be spawned above these in play mode, which make up pieces
    public GameObject pieceCubePlayMode;

    // custom game currently being played, featuring this spawning slot
    public Game game;

    // co-ordinates of the square of the board this smaller slot is in
    public byte boardRow;
    public byte boardCol;




    /*** INSTANCE METHODS ***/
    // If in process of making board, place selected piece on board
    public void OnMouseDown()
    {
        // TODO
        // TEMP debug messages
        Debug.Log("ROW POS. OF SQUARE CLICKED: " + boardRow);
        Debug.Log("COLUMN POS. OF SQUARE CLICKED: " + boardCol);

        ButtonHandler bh = Camera.main.GetComponent<ButtonHandler>();
        if (bh.currentProgramState == ButtonHandler.ProgramState.MakingBoard) 
        {
            // recover information about piece chosen
            byte indexPieceChosen = bh.boardCreationPanel.pieceSelected;

            // TODO
            // TEMP debug messages
            Debug.Log("PIECE TO BE PLACED: " + indexPieceChosen);

            // reassigns piece at that slot 
            game.info.boardAtStart.boardStateRepresentation[boardRow, boardCol] = 
                indexPieceChosen;

            // respawns the piece on the large board square clicked
            game.spawningsSlots[boardRow, boardCol].ForEach(
                (slot) =>
                {
                    slot.Spawn();
                });
        }
    }



    // spawns the cube above this slot, if there should be one, 
    //  coloured according to pieceVisualRepresentation
    public void Spawn() 
    {
        // destroys old cube above this slot to clear room for new cube
        // checks that it exists
        if (pieceCube != null && pieceCube.gameObject != null)
        {
            Destroy(pieceCube.gameObject); //destroys
        }

        // get information about piece above this slot, 
        //  and whether there's even one or not
        PosInfo posInfo;
        byte pieceHere = 
            game.boardState.boardStateRepresentation[boardRow, boardCol];
        if (pieceHere == PosInfo.noPiece) 
        {
            posInfo = new PosInfo.Nothing(); 
        } 
        else
        {
            PieceInfo pieceInfo = game.info.pieces[pieceHere];
            posInfo = pieceInfo.visualRepresentation[rowPos, colPos];
        }


        if (posInfo is PosInfo.RGBData)
        {
            // spawns piece if there is one to be spawned

            // extract (cast) information about colour of piece
            PosInfo.RGBData rgbData =
                (posInfo as PosInfo.RGBData);

            // scale and position cube based on plane's scale and position
            pieceCubePlayMode.transform.localScale =
                this.transform.localScale * relScale;
            Vector3 posToSpawn = this.transform.position + spawnOffset;
            posToSpawn.y += pieceCubePlayMode.transform.localScale.y / 2;


            // creates the piece cube and instantiates its variables
            GameObject cubeMade
                = Instantiate(pieceCubePlayMode, posToSpawn, Quaternion.identity);
            PieceCubePlayMode cubeMadeScript =
                cubeMade.GetComponent<PieceCubePlayMode>();
            cubeMadeScript.rowPos = this.rowPos;
            cubeMadeScript.colPos = this.colPos;

            // change the colour of the cube to the appropriate one

            cubeMade.GetComponent<Renderer>().material.color =
                new Color(rgbData.red / 255f,
                          rgbData.green / 255f,
                          rgbData.blue / 255f);

            // associates new cube with spawning slot
            pieceCube = cubeMadeScript;

            // add cube to list of objects to destroy after session
            Utility.objsToDelete.Add(cubeMade);

        }
        // otherwise, don't do anything
    }

}
