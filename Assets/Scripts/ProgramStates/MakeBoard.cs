using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Items for the board creation process
internal sealed class MakeBoard : Process<MakeBoard>, IAssociatedState<GameCreationHandler, BoardInfo>
{
    /*** INSTANCE VARIABLES ***/
    [SerializeField] internal Canvas canvas;

    [SerializeField] internal Button doneButton;
    [SerializeField] internal Button pieceButtonTemplate;
    [SerializeField] internal Button removePieceButton;
    [SerializeField] internal ScrollRect selectPieceScrView;
    [SerializeField] internal Slider zoomSlider;






    /*** START ***/
    // Add handlers to scroll view and remove piece button
    private void Start()
    {
        BoardCreationHandler bh = BoardCreationHandler.GetHandler();
        // on click, change button
        removePieceButton.onClick.AddListener(
            delegate
            {
                // sets piece selected at start to no piece
                bh.PieceSelected = PieceInfo.noPiece;

                // changes background of all buttons in the scrollview to white
                selectPieceScrView.ForEach<Button>
                    (
                        (b) => b.GetComponent<Image>().color = Color.white
                    );

                // change colour of 'no piece' button 
                selectPieceScrView.SetChosenItem(removePieceButton);
            });

        // highlights button clicked on scroll view while resetting all others
        selectPieceScrView.WhenChosenChanges
            ((scrView) => delegate
            {
                // selects all non-highlighted buttons background to white
                scrView.ForEach<Button>(
                    (b) => b.GetComponent<Image>().color = Color.white);

                // including remove piece button
                removePieceButton.GetComponent<Image>().color =
                    Color.white;

                // highlights chosen button
                if (scrView.GetChosenItem<Button>(out Button chosen) &&
                    chosen != null)
                {
                    chosen.GetComponent<Image>().color =
                        BoardCreationHandler.selectedPieceColour;
                }
            }
            );
    }





    /*** INSTANCE METHODS ***/
    public Canvas GetCanvas() { return canvas; }
    public ProgramData.State GetAssociatedState()
    {
        return ProgramData.State.MakeBoard;
    }



    /// <summary>
    /// Activates at the start of the make board process - sets editable board 
    /// up and populates selection scroll view with clickable buttons, among 
    /// other things.
    /// </summary>
    /// <param name="gamedata">tuple containing (# of rows, # of columns,  
    /// piece resolution, relative size of gap between squares) </param>
    public void OnEnterState(IAssociatedStateLeave<GameCreationHandler> _, GameCreationHandler gamedata)
    {
        // get link to board handler
        BoardCreationHandler bh = BoardCreationHandler.GetHandler();

        // set up UIs on this canvas
        SetupUIs(bh);

        // unpacks information
        byte numRows = gamedata.numOfRows;
        byte numCols = gamedata.numOfCols;
        byte pceRes = gamedata.pieceResolution;
        float gapSize = gamedata.gapSize;
        List<PieceInfo> pieces = gamedata.pieces;

        // clear old states and starts new board
        bh.StartNewBoard(numRows, numCols, gapSize);
    }



    /// <summary>
    /// Destroys slots used to create the board and pass board information 
    /// back to the Make Game process
    /// </summary>
    /// <returns>The board created</returns>
    /// <param name="_">unused - next state (MakeGame)</param>
    /// <typeparam name="G">Dummy type variable</typeparam>
    public BoardInfo OnLeaveState(IAssociatedStateEnter<BoardInfo> _)
    {
        // finish board creation
        BoardCreationHandler bh = BoardCreationHandler.GetHandler();
        bh.VirtualBoardUsed.DestroyBoard();
        BoardInfo createdBoard = bh.FinalizeBoard();
        return createdBoard;
    }



    /// <summary>
    /// Ensures all User Interface on this canvas works during the process
    /// </summary>
    /// <param name="bh">Associated BoardCreationHandler</param>
    private void SetupUIs(BoardCreationHandler bh)
    {
        selectPieceScrView.RepopulatePieceButtons
            (
                pieceButtonTemplate,
                (btn, index) => bh.PieceSelected = index // notifies board handler
            );
    }
}