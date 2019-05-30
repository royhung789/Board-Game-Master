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
    



    /*** METHODS ***/

    // spawns the cube above this slot, if there should be one, 
    //  coloured according to pieceVisualRepresentation
    public void Spawn() 
    {
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
                new Color((float) rgbData.red, 
                          (float) rgbData.green, 
                          (float) rgbData.blue);

            // add cube to list of objects to destroy after session
            Utility.objsToDelete.Add(cubeMade);

        }
        // otherwise dont do anything
    }

}
