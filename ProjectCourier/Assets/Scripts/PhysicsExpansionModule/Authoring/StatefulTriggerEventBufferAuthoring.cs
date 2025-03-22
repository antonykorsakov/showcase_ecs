using PhysicsExpansionModule.Data;
using Unity.Entities;
using UnityEngine;

namespace PhysicsExpansionModule.Authoring
{
#if UNITY_EDITOR
    public class StatefulTriggerEventBufferAuthoring : MonoBehaviour
    {
        class Baker : Baker<StatefulTriggerEventBufferAuthoring>
        {
            public override void Bake(StatefulTriggerEventBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<StatefulTriggerEvent>(entity);
            }
        }
    }
#endif
}