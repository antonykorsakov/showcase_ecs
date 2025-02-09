using SpawnModule.Data;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace SpawnModule.Authoring
{
#if UNITY_EDITOR
    public class SpawnConfigAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _characterPrefab;
        [SerializeField] private GameObject _carPrefab;
        [SerializeField] private GameObject _treePrefab;

        [SerializeField] private GameObject _spawnPlace;
        [SerializeField] private float _spawnInterval;

        class Baker : Baker<SpawnConfigAuthoring>
        {
            public override void Bake(SpawnConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SpawnConfigData
                {
                    CharacterPrefab = new EntityPrefabReference(authoring._characterPrefab),
                    CarPrefab = new EntityPrefabReference(authoring._carPrefab),
                    TreePrefab = new EntityPrefabReference(authoring._treePrefab),

                    SpawnPositionY = authoring._spawnPlace.transform.position.y,
                    SpawnInterval = authoring._spawnInterval
                });
            }
        }
    }
#endif
}