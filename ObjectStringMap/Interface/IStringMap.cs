
namespace ObjectStringMapping.Interface
{
    public interface IStringMap<TObject>
    {
        string Source { get; }

        TObject Map(string str);

        string Map(
            TObject obj,
            bool allowPartialMap = false);            
    }
}
