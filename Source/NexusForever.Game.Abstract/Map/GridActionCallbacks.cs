using System.Numerics;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Abstract.Map
{
    public delegate void OnAddDelegate(IBaseMap map, uint guid, Vector3 vector);

    public delegate void OnGenericErrorDelegate(GenericError error);

    public delegate void OnRelocateDelegate(Vector3 vector);

    public delegate void OnRemoveDelegate();

    public delegate void OnExceptionDelegate(Exception ex);
}
