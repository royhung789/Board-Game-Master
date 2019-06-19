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
    //private Text congratulatoryText;

    // the game currently being played 
    internal Game gameBeingPlayed;

    // true iff. game has ended (a player has won) 
    internal bool gameHasEnded;






    /*** INSTANCE PROPERTIES ***/
    // virtual board encoding data about game board
    internal VirtualBoard<PieceSpawningSlot> VirtualBoardUsed { get; set; }





    /*** INSTANCE METHODS ***/
    // starts the process of playing the custom game 
    internal void StartGame(Game game)
    {
        gameBeingPlayed = game;

        Debug.Log("PIECE RES: " + game.Info.pieceResolution);

        // generates virtual board for the game
        VirtualBoardUsed = new VirtualBoard<PieceSpawningSlot>
            ( 
                game.Info.boardAtStart.BoardStateRepresentation, 
                game.Info.boardAtStart.BoardShapeRepresentation,
                game.Info.pieceResolution, 
                game.Info.boardAtStart.SquareSize,
                game.Info.boardAtStart.GapSize,
                (brd, r, c) => 
                {
                    // lock board (do nothing) if game has ended 
                    if (gameHasEnded) 
                    {
                        return;
                    }

                    byte curPlayer = gameBeingPlayed.currentPlayer;
                    byte pceClicked =
                        gameBeingPlayed
                            .boardState
                            .BoardStateRepresentation[r, c];

                    List<RuleInfo> rules = 
                        gameBeingPlayed.Info
                                       .rules[curPlayer][pceClicked];

                    Debug.Log("NUMBER OF TRIGGERED RULES: " + rules.Count);

                    PlayGame playGame = PlayGame.GetProcess();
                    // clear old rules listed on panel
                    playGame.movesScrView.Clear(playGame.moveButtonTemplate);

                    foreach (RuleInfo rule in rules)    
                    {
                        List<Game> possibleGames = rule.Apply(gameBeingPlayed, r, c);

                        Debug.Log("APPLICATION RESULTS: " + possibleGames.Count + " POSSIBLE GAMES");
                        // dont display non-applicable rules
                        if (possibleGames.Count == 0) 
                        {
                            continue;
                        }

                        Utility.CreateButton
                            (
                                playGame.moveButtonTemplate,
                                playGame.movesScrView.content, 
                                rule.name, 
                                delegate 
                                {
                                    // for now, only allow first state
                                    gameBeingPlayed = possibleGames[0];
                                    byte[,] brdState = gameBeingPlayed
                                                        .boardState
                                                        .BoardStateRepresentation;

                                    // updates virtual board
                                    VirtualBoardUsed.boardRepresentation =
                                        gameBeingPlayed.Info
                                                       .LinkVisRepTo(brdState);

                                    // refresh all squares
                                    VirtualBoardUsed.RefreshBoard();

                                    // move onto next term
                                    NextTurn(rule.nextPlayer);
                                }
                            );
                    }
                }
            );

        VirtualBoardUsed.SpawnBoard(SpatialConfigs.commonBoardOrigin);

        NextTurn(gameBeingPlayed.Info.startingPlayer);
    }



    // moves onto the next turn of the game
    public void NextTurn(byte player) 
    {
        // clear previous moves 
        PlayGame playGame = PlayGame.GetProcess();
        playGame.movesScrView.Clear(playGame.moveButtonTemplate);

        // update current player
        gameBeingPlayed.currentPlayer = player;

        List<byte> winners = new List<byte>();
        // check if game has been won 
        foreach (WinCondInfo winCond in gameBeingPlayed.Info.winConditions)
        {
            // player has won if there is a sub-structure of that type
            if ( winCond.Check(gameBeingPlayed, out byte winner) ) 
            {
                winners.Add(winner);
            }
        }

        // TODO ask whether multiple winners is a TIE or all wins or what
        if (winners.Count > 0)
        {
            GameEnded(winners);
        }
    }



    // announces that game has been won and ends the game
    private void GameEnded(List<byte> winners) 
    {
        gameHasEnded = true;

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
            Debug.Log(
                "Oh no! No one has won!");
        } 
        else if (winners.Count == 1) // if there is 1 clear winner, annouce it
        {
            Debug.Log(
                "The game has ended! The winner is: Player No." + winners[0]);
        }
        else // list all of the winners 
        {
            // TODO 
            Debug.Log(
                "The game has ended! Multiple people won!");
        }
    }
}