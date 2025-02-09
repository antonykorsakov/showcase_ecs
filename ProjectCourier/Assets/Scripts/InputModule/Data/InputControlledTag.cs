using Unity.Entities;

namespace InputModule.Data
{
    /// <summary>
    /// The input system changes entities with this tag. This tag is only for game entities.
    /// </summary>
    public struct InputControlledTag : IComponentData
    { }
}