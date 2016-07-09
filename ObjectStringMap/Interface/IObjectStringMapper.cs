
namespace ObjectStringMap.Interface
{
    public interface IObjectStringMapper<TObject>
    {
        string Map(
            string template, 
            TObject obj, 
            bool allowPartialMap = false);

        TObject Map(
            string template, 
            string str);
    }
}
