using UnityEngine;


public class WinCondCreationHandler : ProcessHandler<WinCondCreationHandler>
{
    /*** STATIC VARIABLES ***/
    // colour of square that allows anything to be on (or not on) it
    internal static Color anythingColour =
        new Color(220 / 255f, 220 / 255f, 220 / 255f, 0.8f);

    // colour of a square with no piece allowed on it
    internal static Color noPieceColour =
        new Color(20 / 255f, 200 / 255f, 20 / 255f, 0.8f);

    // colour of button selected for piece selection
    internal static Color selectedPieceColour =
        new Color(36 / 255f, 185 / 255f, 46 / 255f, 1);





    /*** INSTANCE VARIABLES ***/
    // info for win condition being made
    internal byte[,] winStructure;
    internal byte winner;

    internal byte pieceSelected;





    /*** INSTANCE PROPERTIES ***/
    internal VirtualBoard<PieceSpawningSlot> VirtualBoardUsed { get; set; }





    /*** STATIC METHODS ***/
    // returns colour of square under this 'piece' (or lack of) 
    internal static PosInfo ColourOf(byte pce) 
    { 
        switch (pce) 
        {
            case PieceInfo.noSquare:
                return new PosInfo.RGBWithAlpha(anythingColour);
            case PieceInfo.noPiece:
                return new PosInfo.RGBWithAlpha(noPieceColour);
            default:
                return new PosInfo.RGBWithAlpha(selectedPieceColour);
        }
    }





    /*** INSTANCE METHODS ***/
    internal WinCondInfo FinalizeWinCond(string nm) 
    {
        // destroys board
        VirtualBoardUsed.DestroyBoard();

        // creates and returns win condition info
        WinCondInfo winCondMade = new WinCondInfo(name, winStructure, winner); 
        return winCondMade;
    }





    internal void StartNewWinCond(byte size, byte winningPlayer) 
    {
        // resets old stored info
        winStructure = new byte[size, size];
        winStructure.ReplaceAllWith((i, j) => PieceInfo.noSquare);
        winner = winningPlayer;

        pieceSelected = PieceInfo.noPiece;

        // generates new board 
        GameCreationHandler gameHandler = GameCreationHandler.GetHandler();

        VirtualBoardUsed = new VirtualBoard<PieceSpawningSlot>
            (
                winStructure, 
                new Linked2D<byte, PosInfo>(winStructure, ColourOf),
                gameHandler.pieceResolution,  
                gameHandler.SquareSize,
                gameHandler.GapSize, 
                (vboard, r, c) => 
                {
                    // toggle piece (if piece is selected)
                    if (pieceSelected == winStructure[r, c] && 
                        pieceSelected != PieceInfo.noSquare)
                    {
                        winStructure[r, c] = PieceInfo.noPiece;
                    }
                    else
                    {
                        winStructure[r, c] = pieceSelected;
                    }
                }
            );

        // spawns board
        VirtualBoardUsed.SpawnBoard(SpatialConfigs.commonBoardOrigin);
    }
}
