using System.Numerics;

namespace OpenTabletDriver.Plugin.Tablet
{
    public interface IExtraIntuosReport
    {
        bool FarProximity { get; }
        bool NearProximity { get; }
        uint HoverDistance { get; }
    }
}
