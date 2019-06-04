using UnityEngine;

// class representing a color of cube/plane (or lack-of) at each position
[System.Serializable]
public abstract class PosInfo
{
    private PosInfo() { } // prevents accidental creation of non-specific PosInfo

    // 2D array with all slots filled with 'Nothing's of dimension row x col
    public static PosInfo[,] NothingMatrix(byte row, byte col) 
    {
        PosInfo[,] arr = new PosInfo[row, col];
        for (byte r = 0; r < row; r++) 
        { 
            for (byte c = 0; c < col; c++) 
            {
                arr[r, c] = new PosInfo.Nothing();
            }
        }

        return arr;
    }

    // the byte value used to represent the lack of a cube or a plane
    //  used in array representation of where pieces are on a board 
    public const byte noPiece = byte.MaxValue; 

    // at each position, there is either nothing,
    //  or something of a specific RGB colour
    [System.Serializable]
    public class Nothing : PosInfo {}
    [System.Serializable]
    public class RGBData : PosInfo
    {
        public RGBData(byte r, byte g, byte b)
        { this.red = r; this.green = g; this.blue = b; }
        // uses 3 one-byte integers for R, G, B values of colours
        public byte red;
        public byte green;
        public byte blue;
    }
}
