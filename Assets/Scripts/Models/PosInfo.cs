using System.Collections.Generic;

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





    /*** INNER CLASSES ***/
    // at each position, there is either nothing,
    //  or something of a specific colour
    [System.Serializable]
    public class Nothing : PosInfo {}
    [System.Serializable]
    public class RGBData : PosInfo
    {
        // uses 3 one-byte integers for R, G, B values of colours
        public byte red;
        public byte green;
        public byte blue;

        public RGBData(byte r, byte g, byte b)
        { this.red = r; this.green = g; this.blue = b; }
    }
    // RGB colour with alpha value (for opacity)
    [System.Serializable]
    public class RGBWithAlpha : PosInfo.RGBData
    {
        public byte alpha; // alpha is stored as byte for consistency

        public RGBWithAlpha(byte r, byte g, byte b, byte a) : base(r, g, b)
        {
            alpha = a;
        }
    }





    /*** STATIC METHODS ***/
    // addition of PosInfo according to these rules:
    //   thing1 + nothing = thing1
    //   addition of RGBWithAlpha is done component-wise, capped at 255
    //      e.g. r_total = (byte)min(255, (int)r1 + (int)r2)
    //   RGBData is treated as RGBWithAlpha with alpha set to max (255)
    //   Addition is commutative 
    //
    // NOTE: This may convert an RGBData to an RGBWithAlpha
    public static PosInfo operator +(PosInfo p1, PosInfo p2) 
    { 
        switch (p1) 
        {
            case RGBWithAlpha rgba1:
                switch (p2) 
                {
                    case RGBWithAlpha rgba2:
                        rgba1.red.AddCheck(rgba2.red, out byte red);
                        rgba1.blue.AddCheck(rgba2.blue, out byte blue);
                        rgba1.green.AddCheck(rgba2.green, out byte green);
                        rgba1.alpha.AddCheck(rgba2.red, out byte alpha);
                        return new RGBWithAlpha(red, green, blue, alpha);
                    case RGBData rgb2:
                        return rgba1 + p2;
                    case Nothing n2:
                        return p1;
                    default:
                        throw new System.ArgumentException("Unaccounted for PosInfo subtype");
                }
            case RGBData rgb1:
                return p2 + new RGBWithAlpha(rgb1.red, rgb1.green, rgb1.blue, 255);
            case Nothing n1:
                return p2;
            default:
                throw new System.ArgumentException("Unaccounted for PosInfo subtype");
        }
    }

           

    // "overlays" each visual object in the list on top of each other 
    //   that is, add together the R, G, B, A values of each 
    //   (with all of them capped at 255)
    // ASSUMPTION: visual objects have same "resolution" = 'res' (array dimensions)
    internal static PosInfo[,] Overlay(List<PosInfo[,]> pces, byte res) 
    {
        PosInfo[,] accu = NothingMatrix(res, res); // accumulation variable
        if (pces.Count == 0) // returns visual representation with nothing on it with empty list
        {
            return accu;
        }  
        else // add each 'pixel' in the same spot
        {
            for (int r = 0; r < res; r++) 
            { 
                for (int c = 0; c < res; c++) 
                {
                    pces.ForEach((p) => accu[r, c] += p[r, c]);
                }
            }

            return accu;
        }
    }
}
