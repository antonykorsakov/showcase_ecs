using Unity.Entities;

namespace InputModule.Data
{
    /// <summary>
    /// The game can handle input system. This tag is not for game entities.
    /// </summary>
    public struct InputTag : IComponentData
    {
        public int SelectedOrderNumber;
    }
}