using InteractableModule.Data;
using Unity.Entities;
using UnityEngine;

namespace InteractableModule.Authoring
{
#if UNITY_EDITOR
    public class InteractableAuthoring : MonoBehaviour
    {
        [SerializeField] private bool _isStoneSource;

        class Baker : Baker<InteractableAuthoring>
        {
            public override void Bake(InteractableAuthoring authoring)
            {
                var inputEntity = GetEntity(TransformUsageFlags.None);
                AddComponent<InteractableData>(inputEntity);
                if (authoring._isStoneSource)
                    AddComponent<StoneSourceTag>(inputEntity);
                else
                    AddComponent<WoodSourceTag>(inputEntity);
            }
        }
    }
#endif
}