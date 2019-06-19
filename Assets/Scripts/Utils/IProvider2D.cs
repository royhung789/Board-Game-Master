// interface for collections with 2D indexers with 2D lengths defined
public interface IProvider2D<Key, Elem>
{
    Elem this[Key key1, Key key2] { get; }
    int GetLength(int n);
}
