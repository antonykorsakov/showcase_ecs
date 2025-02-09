using Unity.Entities;
using UnityEngine;

namespace Extra
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float MoveSpeed;
        public GameObject ProjectilePrefab;
    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var playerEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent<PlayerTag>(playerEntity);
            AddComponent<PlayerMoveInput>(playerEntity);
            
            AddComponent<FireProjectileTag>(playerEntity);
            SetComponentEnabled<FireProjectileTag>(playerEntity, false);
            
            AddComponent(playerEntity, new PlayerMoveSpeed
            {
                Value = authoring.MoveSpeed
            });
            AddComponent(playerEntity, new ProjectilePrefab
            {
                Value = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}