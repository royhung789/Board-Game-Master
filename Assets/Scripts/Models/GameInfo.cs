using System.Collections.Generic;
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
    // the name of the game
    public readonly string name;

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
    public readonly List<WinCondInfo> winConditions;





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
                      List<WinCondInfo> wnCnds)
    { 
        boardAtStart = brdStrt;
        pieces = pcs;
        pieceResolution = pceRes;
        numOfPieces = (byte) pcs.Count;
        numOfPlayers = numPlayers;
        startingPlayer = startPlayer;

        rules = rls;
        for (byte plyr = 0; plyr < numPlayers; plyr++) 
        { 
            if (!rules.ContainsKey(plyr)) 
            {
                rules.Add(plyr, new Dictionary<byte, List<RuleInfo>>());
            }
            Dictionary<byte, List<RuleInfo>> theirMoves = rules[plyr];

            // for clicking on board and panel rules
            if (!theirMoves.ContainsKey(PieceInfo.noPiece)) 
            {
                theirMoves.Add(PieceInfo.noPiece, new List<RuleInfo>());
            }
            List<RuleInfo> clickBoardRules = theirMoves[PieceInfo.noPiece];
            if (clickBoardRules == null) 
            {
                theirMoves[PieceInfo.noPiece] = new List<RuleInfo>();
            }

            if (!theirMoves.ContainsKey(PieceInfo.noSquare)) 
            {
                theirMoves.Add(PieceInfo.noSquare, new List<RuleInfo>());
            }
            List<RuleInfo> panelRules = theirMoves[PieceInfo.noSquare];
            if (panelRules == null)
            {
                theirMoves[PieceInfo.noSquare] = new List<RuleInfo>();
            }

            // on click pieces
            for (byte pce = 0; pce < pcs.Count; pce++) 
            { 
                if (!theirMoves.ContainsKey(pce)) 
                {
                    theirMoves.Add(pce, new List<RuleInfo>());
                }
                List<RuleInfo> triggeredRules = theirMoves[pce];
                if (triggeredRules == null) 
                {
                    theirMoves[pce] = new List<RuleInfo>();
                }
            }
        } // end double for loop, filling nulls in rules

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





    /*** INSTANCE VARIABLES ***/
    // creates a "PosInfo[,][,] array-like obj" which updates when source is updated
    internal Linked2D<byte, PosInfo[,]> LinkVisRepTo(byte[,] source)
    {
        return new Linked2D<byte, PosInfo[,]>
            (
                source,
                (i) =>
                {
                    if (i == PieceInfo.noPiece || i == PieceInfo.noSquare)
                    {
                        return PosInfo.NothingMatrix(pieceResolution, pieceResolution);
                    }
                    else
                    {
                        return pieces[i].visualRepresentation;
                    }
                }
            );
    }
}
