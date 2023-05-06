using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;
using NexusForever.Script.Template.Filter.Dynamic;

namespace NexusForever.Script.Main.AI
{
    [ScriptFilterDynamic<IScriptFilterDynamicEntitySpline>]
    [ScriptFilterIgnore]
    public class SplineAI : IWorldEntityScript, IOwnedScript<INonPlayer>
    {
    }
}
