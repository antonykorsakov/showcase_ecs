using InteractableModule.Data;
using Unity.Entities;
using UnityEngine;

namespace InteractableModule.Authoring
{
#if UNITY_EDITOR
    public class InteractableAuthoring : MonoBehaviour
    {
        class Baker : Baker<InteractableAuthoring>
        {
            public override void Bake(InteractableAuthoring authoring)
            {
                var inputEntity = GetEntity(TransformUsageFlags.None);
                AddComponent<InteractableData>(inputEntity);
            }
        }
    }
#endif
}