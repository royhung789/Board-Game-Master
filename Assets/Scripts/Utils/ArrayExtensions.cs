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



    public static bool IsSubMatrixOf<T>(this T[,] littleArr, T[,] bigArr) 
    {
        // TODO 
        throw new System.NotImplementedException("Should find fast approach");
    }
}
