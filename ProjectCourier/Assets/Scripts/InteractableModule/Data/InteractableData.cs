using Unity.Entities;

namespace InteractableModule.Data
{
    /// <summary>
    /// For interaction via the input system.
    /// </summary>
    public struct InteractableData : IComponentData
    {
        public bool CanUse;
    }
}