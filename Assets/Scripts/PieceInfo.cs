using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class containing information about a piece, e.g. graphics 
[System.Serializable]
public class PieceInfo
{
    // 2D array representing the piece
    public PosInfo[,] visualRepresentation; 
    // name of piece
    public string pieceName;
    
    public PieceInfo(string name, PosInfo[,] visRep)  
    {
        this.pieceName = name;
        this.visualRepresentation = visRep;
    }


}
