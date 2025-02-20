using InputModule.Data;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputModule.Authoring
{
#if UNITY_EDITOR
    public class InputAuthoring : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActionAsset;

        class Baker : Baker<InputAuthoring>
        {
            public override void Bake(InputAuthoring authoring)
            {
                var inputEntity = GetEntity(TransformUsageFlags.None);
                AddComponent<InputTag>(inputEntity);
                AddComponentObject(inputEntity, authoring._inputActionAsset);
            }
        }
    }
#endif
}