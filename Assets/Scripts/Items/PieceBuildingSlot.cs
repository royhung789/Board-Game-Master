using UnityEngine;

// Behaviour for slots used for making custom game pieces
internal class PieceBuildingSlot : PieceSlot<PieceBuildingSlot>
{
    /*** INSTANCE VARIABLES ***/
    // denotes whether there is a piece cube currently above this slot
    //   "incorrectly" starts off true so it is toggled to false upon first OnUpdate call
    [SerializeField] private bool hasPieceAbove = true;





    /*** INSTANCE METHODS ***/
    internal override void OnUpdate() 
    { 
        // clears piece above it if there was one
        if (pieceCube != null && pieceCube.gameObject != null) 
        {
            Destroy(pieceCube.gameObject);
        }

        // sets piece above if there was none 
        if (!hasPieceAbove) 
        {
            // TODO add colours based on repreesntation in PieceCreationHandler
            PieceCubeBuildMode pieceCubeBuildMode = Prefabs.GetPrefabs().pieceCubeBuildMode;

            // scale and position cube based on plane's scale and position
            pieceCubeBuildMode.transform.localScale =
                transform.localScale * relScale;
            Vector3 posToSpawn = transform.position + spawnOffset;
            posToSpawn.y += pieceCubeBuildMode.transform.localScale.y / 2;

            // creates the piece cube and instantiates its variables
            GameObject cubeMade
                = Instantiate(pieceCubeBuildMode.gameObject, posToSpawn, Quaternion.identity);
            PieceCubeBuildMode cubeMadeScript =
                cubeMade.GetComponent<PieceCubeBuildMode>();
            cubeMadeScript.rowPos = boardRow; // the 'board' is the square this 
            cubeMadeScript.colPos = boardCol; //   piece is being created on

            // assign piece cube to slot, and note that it is on the board
            pieceCube = cubeMadeScript;
            GetVirtualBoard().otherObjsOnBoard.Add(cubeMade);
        }

        // toggles indicator variable
        hasPieceAbove = !hasPieceAbove;
    }



    // spawns cube and update visual rep. array when clicked
    private void OnMouseDown()
    {
        VirtualBoard<PieceBuildingSlot> vboard = GetVirtualBoard();
        vboard.OnSquareClicked(boardRow, boardCol);
    }

    

}
