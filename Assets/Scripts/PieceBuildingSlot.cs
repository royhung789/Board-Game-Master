using UnityEngine;

// Behaviour for slots used for making custom game pieces
public class PieceBuildingSlot : PieceSlot
{

    /*** INSTANCE VARIABLES ***/
    // link to panel, which has array representation of piece being built
    public PieceCreationPanel associatedPanel;

    // cube which makes up pieces, in build mode 
    public GameObject pieceCubeBuildMode;

    // unique id representing this PieceBuildingSlot
    public int slotId;


    /*** METHODS ***/
    // spawns cube and update visual rep. array when clicked
    private void OnMouseDown()
    {
        // debug message
        Debug.Log("Plane " + slotId + " was clicked!");

        // scale and position cube based on plane's scale and position
        pieceCubeBuildMode.transform.localScale = 
            this.transform.localScale * relScale;
        Vector3 posToSpawn = this.transform.position + spawnOffset;
        posToSpawn.y += pieceCubeBuildMode.transform.localScale.y / 2;


        // creates the piece cube and instantiates its variables
        GameObject cubeMade 
            = Instantiate(pieceCubeBuildMode, posToSpawn, Quaternion.identity);
        PieceCubeBuildMode cubeMadeScript = 
            cubeMade.GetComponent<PieceCubeBuildMode>();
        cubeMadeScript.rowPos = this.rowPos;
        cubeMadeScript.colPos = this.colPos;
        cubeMadeScript.associatedPanel
            = this.associatedPanel;

        // will delete cube after creation of piece
        Utility.objsToDelete.Add(cubeMade);

        // adds information about the piece to the rep. array
        //  TEMP: stores colour info as (0 0 0) for now
        this.associatedPanel.pieceInfo.visualRepresentation[rowPos, colPos]
            = new PosInfo.RGBData(0, 0, 0);
    }


}
