using System;

// classes of 2D Elem "array" which changes as an underlying Source[,] does
//   links 2 "2D arrays" by having one be a source and applying functions
//   to transform elements of the source into those of the other
public class Linked2D<Source, Elem> : IProvider2D<int, Elem>
{
    /*** INSTANCE VARIABLES ***/
    private readonly Source[,] source;
    private readonly Func<Source, Elem> translator;




    /*** INSTANCE PROPERTIES ***/
    public Elem this[int k1, int k2] // 2D indexes
    {
        get => translator(source[k1, k2]);
    }




    /*** CONSTRUCTORS ***/
    public Linked2D(Source[,] src, Func<Source, Elem> func) 
    {
        source = src;
        translator = func;
    }
    



    /*** INSTANCE METHODS ***/
    // dimensions are the same as that of source
    public int GetLength(int n) 
    {
        return source.GetLength(n);
    }
}
