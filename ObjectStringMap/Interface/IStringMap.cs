namespace ObjectStringMapping.Interface
{
    public interface IStringMap<TObject>
        where TObject : new()
    {
        string MapSource { get; }

        TObject Map(string str);

        string Map(
            TObject obj,
            bool allowPartialMap = false);            
    }
}
