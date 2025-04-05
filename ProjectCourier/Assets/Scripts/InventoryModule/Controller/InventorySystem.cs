using InventoryModule.Data;
using Unity.Entities;

namespace InventoryModule.Controller
{
    public partial struct InventorySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var entityManager = state.EntityManager;

            var inventoryEntity = entityManager.CreateEntity();
            entityManager.AddComponent<InventoryData>(inventoryEntity);
        }
    }
}