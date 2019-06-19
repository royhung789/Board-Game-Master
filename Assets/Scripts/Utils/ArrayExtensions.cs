using System;

// provides extension methods for arrays, to make handling them easy
public static class ArrayExtensions
{
    /*** EXTENSION METHODS ***/
    // replace all 'null' elements in a 2D array with value provided by supplier
    public static void FillWith<T>(this T[,] arr, Func<int, int, T> supplier)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                if (arr[i, j] == null) // never true for value types (?)
                {
                    arr[i, j] = supplier(i, j);
                }
            }
        }
    }



    // set every unaffected area to object of type SquareChange.Unaffected 
    //   this is to speed up computation of application of rule during game play
    public static void NoteUnaffected(this RuleInfo.SquareChange[,] changes) 
    { 
        for (int r = 0; r < changes.GetLength(0); r++) 
        { 
            for (int c = 0; c < changes.GetLength(1); c++) 
            {
                switch (changes[r, c]) 
                {
                    case RuleInfo.SquareChange.Unaffected un:
                        break; // do nothing for already unaffected objects
                    case RuleInfo.SquareChange.Changed changed:
                        // it is unaffected if it is a singleton list with
                        //   same piece as piece changed to
                        if (changed.pieceChangedFrom.Count == 1 &&
                            changed.pieceChangedFrom[0] == changed.pieceChangedTo)
                        {
                            changes[r, c] = new RuleInfo.SquareChange.Unaffected();
                        }
                        // or if changeFrom is empty (no piece) and 
                        //   changed to is also no piece
                        else if (changed.pieceChangedFrom.Count == 0 &&
                                 changed.pieceChangedTo == PieceInfo.noPiece) 
                        {
                            changes[r, c] = new RuleInfo.SquareChange.Unaffected();
                        }
                        // otherwise, keep as is
                        break;
                    default:
                        throw new ArgumentException("Unaccounted for SquareChange type");
                }
            }
        } // end of double for loop traversing changes 2D array
    }



    // replace all elements in a 2D array with value provided by supplier
    public static void ReplaceAllWith<T>(this T[,] arr, Func<int, int, T> supplier)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                arr[i, j] = supplier(i, j);
            }
        }
    }



    public static bool IsSubMatrixOf<T>(this T[,] littleArr, T[,] bigArr)
        where T : IEquatable<T>
    {
        UnityEngine.Debug.Log("CHECKING SUBARR CONTAINMENT");
        // TODO MAKE THIS FASTER
        int lengthDiff0 = bigArr.GetLength(0) - littleArr.GetLength(0);
        int lengthDiff1 = bigArr.GetLength(1) - littleArr.GetLength(1);
        for (int i = 0; i <= lengthDiff0; i++) 
        { 
            for (int j = 0; j <= lengthDiff1; j++) 
            {
                bool shownDifferent = false;
                for (int r = 0; r < littleArr.GetLength(0); r++) 
                { 
                    for (int c = 0; c < littleArr.GetLength(1); c++) 
                    { 
                        shownDifferent |= !littleArr[r, c].Equals(bigArr[i + r, j + c]);
                    }
                } // end of inner double for loop, checking one subarr spot

                if (!shownDifferent)
                {
                    UnityEngine.Debug.Log("SHOWN DIFF REACHED");
                    return true;
                }
            }
        } // end of quad for loop, checking all subarr spots

        return false;
    }



    // returns another visual representation similar to the one given,
    //   with the alpha set to value specified
    public static PosInfo[,] WithAlpha(this PosInfo[,] visRep, byte alpha)
    {
        PosInfo[,] result = new PosInfo[visRep.GetLength(0), visRep.GetLength(1)];
        for (int r = 0; r < result.GetLength(0); r++)
        {
            for (int c = 0; c < result.GetLength(1); c++)
            {
                switch (visRep[r, c])
                {
                    case PosInfo.RGBData rgb: //RGBWithAlpha is dealt with here, too
                        PosInfo.RGBWithAlpha resRgba =
                            new PosInfo.RGBWithAlpha(rgb.red, rgb.blue, rgb.green, alpha);
                        result[r, c] = resRgba;
                        break;
                    case PosInfo.Nothing nothing:
                        result[r, c] = new PosInfo.Nothing();
                        break;
                }
            }
        } // end of double for loop, looping through result and visRep


        return result;
    }
}
