using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This script manages custom games in play mode, 
//  deals with what happens when user interacts with pieces and boards 
public class GamePlayHandler : ProcessHandler<GamePlayHandler>
{
    /*** STATIC VARIABLES ***/
    // the position of the camera at the start of a custom game
    private static readonly Vector3 cameraStartPosition =
        new Vector3(0, 100, 0);
        




    /*** INSTANCE VARIABLES ***/
    // congratulatory text for the winner(s) 
    private Text congratulatoryText;

    // the game currently being played 
    internal Game gameBeingPlayed;






    /*** INSTANCE PROPERTIES ***/
    // virtual board encoding data about game board
    VirtualBoard<PieceSpawningSlot> VirtualBoardUsed { get; set; }





    /*** INSTANCE METHODS ***/
    // starts the process of playing the custom game 
    internal void StartGame(Game game)
    {
        gameBeingPlayed = game;

        Debug.Log("PIECE RES: " + game.Info.pieceResolution);

        // generates virtual board for the game
        VirtualBoard<PieceSpawningSlot> vboard = new VirtualBoard<PieceSpawningSlot>
            ( 
                game.Info.boardAtStart, 
                game.Info.pieces, 
                game.Info.pieceResolution, 
                Prefabs.GetPrefabs().pieceSpawningSlot, 
                (brd, r, c) => 
                { 
                    //TODO 
                }
            );

        vboard.SpawnBoard(SpatialConfigs.commonBoardOrigin - game.Info.boardAtStart.BottomLeft);
        VirtualBoardUsed = vboard;
        // TODO
    }



    // moves onto the next turn of the game
    public void NextTurn() 
    {
        //TODO 

        List<byte> winners = new List<byte>();
        // check if game has been won 
        foreach ((byte[,] state, byte winner) in gameBeingPlayed.Info.winConditions)
        {
            // player has won if there is a sub-structure of that type
            if (state.IsSubMatrixOf(gameBeingPlayed.boardState.BoardStateRepresentation)) 
            {
                winners.Add(winner);
            }
        }

        // TODO ask whether multiple winners is a TIE or all wins or what
        GameEnded(winners);
    }



    // announces that game has been won and ends the game
    private void GameEnded(List<byte> winners) 
    {
        // TODO 
        // VERY TEMP.
        Debug.Log("List of Winners:");
        foreach (byte winner in winners) 
        {
            Debug.Log("\tPlayer No." + winner);
        }


        // TODO add custom text
        // declare that no one has won... if no one has won
        if (winners.Count == 0) 
        {
            congratulatoryText.text = 
                "Oh no! No one has won!";
        } 
        else if (winners.Count == 1) // if there is 1 clear winner, annouce it
        {
            congratulatoryText.text = 
                "The game has ended! The winner is: Player No." + winners[0];
        }
        else // list all of the winners 
        {
            // TODO 
            congratulatoryText.text = 
                "The game has ended! Multiple people won!";
        }
    }
}