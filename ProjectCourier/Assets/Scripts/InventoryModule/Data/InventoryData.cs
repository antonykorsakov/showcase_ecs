using Unity.Entities;

namespace InventoryModule.Data
{
    public struct InventoryData : IComponentData
    {
        public int Wood;
        public int Stone;
    }
}