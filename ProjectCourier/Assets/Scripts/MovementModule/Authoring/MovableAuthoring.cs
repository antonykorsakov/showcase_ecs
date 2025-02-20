using MovementModule.Data;
using SpeedModifiersModule.Data;
using Unity.Entities;
using UnityEngine;

namespace CharacterAnimModule.Authoring
{
#if UNITY_EDITOR
    public class MovableAuthoring : MonoBehaviour
    {
        class Baker : Baker<MovableAuthoring>
        {
            public override void Bake(MovableAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent<MoveDirectionData>(entity);
                AddComponent<MoveSpeedData>(entity);
                AddComponent(entity, new SpeedDecayData { Value = 0.01f });
            }
        }
    }
#endif
}