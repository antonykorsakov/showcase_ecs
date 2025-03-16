using InputModule.Data;
using MovementModule.Data;
using Unity.Entities;
using UnityEngine;

namespace InputModule.Authoring
{
#if UNITY_EDITOR
    public class PlayerAuthoring : MonoBehaviour
    {
        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<PlayerControlTag>(entity);
            }
        }
    }
#endif
}