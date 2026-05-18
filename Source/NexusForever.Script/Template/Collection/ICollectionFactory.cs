namespace NexusForever.Script.Template.Collection
{
    public interface ICollectionFactory
    {
        IScriptCollection CreateCollection();
        IOwnedScriptCollection<T> CreateOwnedCollection<T>();
    }
}
