using Unity.Entities;
using Unity.Entities.Serialization;

namespace SpawnModule.Data
{
    public struct SpawnConfigData : IComponentData
    {
        public EntityPrefabReference CharacterPrefab;
        public EntityPrefabReference CarPrefab;
        public EntityPrefabReference TreePrefab;

        public float SpawnPositionY;
        public float SpawnInterval;
    }
}