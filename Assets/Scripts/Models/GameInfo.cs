﻿using System.Collections.Generic;
using System;

/* CURRENT LIST OF CONSTRAINTS ON A GAME
 *  BOARD SIZE: 256 x 256 max, at least 1 by 1
 *  MAX NUMBER OF PIECES: 255
 * 
 * 
 * 
 */


// class representing the information needed to play a custom game 
[System.Serializable]
public class GameInfo
{
    /*** STATIC VARIABLES ***/
    // the max number of types of pieces is capped by the representing type
    //        As of the first version, this is byte, and capped at 
    //        254 pieces (possibility of no piece on a square, or no square at all)
    private const byte maxNumOfPieces = byte.MaxValue - 1;


    /*** INSTANCE VARIABLES ***/
    // number of players
    public readonly byte numOfPlayers;

    // player who gets to play at the start of the game
    public readonly byte startingPlayer;

    // state of the board at the start of the game
    public readonly BoardInfo boardAtStart;

    // pieces of the game, with indexes used as a sort of identifier
    public readonly List<PieceInfo> pieces;

    // the "resolution in cubes" of the piece 
    //  would be 'n' for pieces made on an n x n grid
    public readonly byte pieceResolution;

    // number of pieces declared so far;
    // NOTE: This should be used instead of pieces.length
    public readonly byte numOfPieces;

    // the rules of the game 
    // rules[playerN][piece] denote all the rules that player #n can use
    //   by clicking on piece.
    //   piece = noPiece denote rules that activates on clicking an empty square
    //   piece = noSquare denote panel rules 
    public readonly Dictionary<byte, Dictionary<byte, List<RuleInfo>>> rules;

    // win conditions
    //    the player (represented by byte) wins when a structure
    //    (represented by byte[,]) is found on the board
    public readonly List<Tuple<byte[,], byte>> winConditions;





    /*** INSTANCE PROPERTIES ***/
    // dimensions of the game board 
    public byte NumOfRows 
    {
        get => boardAtStart.NumOfRows;
    }

    public byte NumOfCols 
    {
        get => boardAtStart.NumOfCols;
    }

    // size of the piece spawning slots used for tiling this 
    public float SpawnSlotSize
    {
        get
        {
            return boardAtStart.SquareSize / pieceResolution;
        }
    }





    /*** CONSTRUCTORS ***/
    internal GameInfo(BoardInfo brdStrt, List<PieceInfo> pcs, byte pceRes,
                      byte numPlayers, byte startPlayer,
                      Dictionary<byte, Dictionary<byte, List<RuleInfo>>> rls, 
                      List<Tuple<byte[,], byte>> wnCnds)
    { 
        boardAtStart = brdStrt;
        pieces = pcs;
        pieceResolution = pceRes;
        numOfPieces = (byte) pcs.Count;
        numOfPlayers = numPlayers;
        startingPlayer = startPlayer;
        rules = rls;
        winConditions = wnCnds;
    }





    /*** STATIC METHODS ***/
    // randomly place pieces on the empty slots on the old board
    public BoardInfo RandomPiecePlacements(BoardInfo oldBoard) 
    {
        Random ranGen = new Random();

        for (byte r = 0; r < oldBoard.NumOfRows; r++) 
        { 
            for (byte c = 0; c < oldBoard.NumOfCols; c++) 
            {
                byte ranPiece =  (byte)ranGen.Next(numOfPieces + 1);
                if (ranPiece == numOfPieces)
                {
                    ranPiece = PieceInfo.noPiece;
                }

                oldBoard.BoardStateRepresentation[r, c] = ranPiece;
            }
        }

        return oldBoard;
    }





}