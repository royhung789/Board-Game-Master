using UnityEngine;



// script which controls the behaviour of the board customization panel 
//  in the board creation process 
public class BoardCreationPanel : MonoBehaviour
{
    /*** STATIC VARIABLES ***/
    // default colour of the piece button currently selected
    public static Color selectedPieceColour =
        new Color(36 / 255f, 185/255f, 46/255f, 1);



    /*** INSTANCE VARIABLES ***/
    // information on board being created
    public BoardInfo boardInfo;

    // information on the piece currently selected to be placed 
    //  (or if 'no piece' selected)
    public byte pieceSelected;
}
