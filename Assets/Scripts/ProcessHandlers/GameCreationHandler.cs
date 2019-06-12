using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// type alias, (# of players, # of rows, # of cols, piece resolution, relative gap size)
using DimensionsData = System.Tuple<byte, byte, byte, byte, float>;


// script which controls the behaviour of the game creation process 
public class GameCreationHandler : ProcessHandler<GameCreationHandler>
{
    /*** INSTANCE VARIABLES ***/
    // dimension data - stored for other handlers
    internal byte numOfPlayers;
    internal byte numOfRows;
    internal byte numOfCols;
    internal float gapSize;

    // board to be used in the game
    internal BoardInfo boardAtStart;

    // pieces to be used in the game, each piece is tied to its index in this list
    internal List<PieceInfo> pieces;

    // resolution of the pieces
    internal byte pieceResolution;

    // rules of the game, sorted according to player's turn and trigger piece 
    //   with noSquare denoting panel rules: rules[(player, trigPiece)] = info
    internal Dictionary<byte, Dictionary<byte, List<RuleInfo>>> rules;

    // player who plays at the start of the game
    internal byte startingPlayer;

    // the winning conditions of the game
    //   in pairs of (structure, winner) where if structure is found in the board
    //   somewhere, then winner will win the game
    internal List<Tuple<byte[,], byte>> winConditions;





    /*** INSTANCE PROPERTIES ***/
    internal byte NumOfRows 
    {
        get 
        {
            return boardAtStart.NumOfRows;
        }
    }

    internal byte NumOfCols
    {
        get
        {
            return boardAtStart.NumOfCols;
        }
    }

    internal float GapSize
    { 
        get 
        {
            return boardAtStart.GapSize;
        }
    }






    /*** INSTANCE METHODS ***/
    // finishes game creation
    internal GameInfo FinalizeGame(string gameName) 
    {
        // creates game info
        GameInfo gameMade = new GameInfo(boardAtStart, pieces, pieceResolution,
                                         numOfPlayers, startingPlayer,
                                         rules, winConditions);


        // serializes it to file with name of game in games folder
        BinaryFormatter binFormat = new BinaryFormatter();
        FileStream gameFile = File.Create(
            ProgramData.gamesFolderPath + "/" + gameName.Replace(' ', '_') + ".gam");
        binFormat.Serialize(gameFile, gameMade);
        gameFile.Close();

        return gameMade;
    }



    // resets variables used for creation of game
    internal void StartNewGame(DimensionsData data) 
    {
        // unpacks and assigns data
        (numOfPlayers, numOfRows, numOfCols, pieceResolution, gapSize) = data;

        boardAtStart = BoardInfo.DefaultBoard
            (
                numOfRows, numOfCols,
                BoardCreationHandler.GetHandler().BoardSquareSize, gapSize,
                BoardCreationHandler.defaultBoardColour
            );

        pieces = new List<PieceInfo>();
        rules = new Dictionary<byte, Dictionary<byte, List<RuleInfo>>>();
        winConditions = new List<Tuple<byte[,], byte>>();
    }
}
