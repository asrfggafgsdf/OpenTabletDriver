namespace OpenTabletDriver.Plugin.Tablet
{
    public interface IProximityReport
    {
        bool FarProximity { get; }
        bool NearProximity { get; }
        uint HoverDistance { get; }
    }
}
