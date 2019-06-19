using System;


// class representing a win condition for the game
[System.Serializable]
public class WinCondInfo
{
    /*** INSTANCE VARIABLES ***/
    public string name;

    // (structure, winner). If structure is present in board, winner wins
    public readonly byte[,] winStructure;
    public readonly byte winner;





    /*** CONSTRUCUTORS ***/
    public WinCondInfo(string nm, byte[,] winStruct, byte win) 
    {
        name = nm;
        winStructure = winStruct;
        winner = win;
    }





    /*** INSTANCE METHODS ***/
    // checks whether a player has won, 
    // true iff. someone won, and if true: winner = the person who won
    public bool Check(Game game, out byte win) 
    {
        if (winStructure.IsSubMatrixOf(game.boardState.BoardStateRepresentation)) 
        {
            win = winner;
            return true;
        }
        else 
        {
            win = 255; 
            return false;
        }
    }
}
