using CharacterAnimModule.Data;
using Rukhanka.Hybrid;
using Unity.Entities;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace CharacterAnimModule.Authoring
{
#if UNITY_EDITOR
    public class CharacterAnimAuthoring : MonoBehaviour
    {
        [SerializeField] private RigDefinitionAuthoring _rigDefinitionAuthoring;
        [SerializeField] private AnimationAssetSetAuthoring _animationAssetsAuthoring;

        class Baker : Baker<CharacterAnimAuthoring>
        {
            public override void Bake(CharacterAnimAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                var avatar = authoring._rigDefinitionAuthoring.GetAvatar();
                var animationClips = authoring._animationAssetsAuthoring.animationClips;

                var data = new CharacterAnimData();
                for (var i = 0; i < animationClips.Length; ++i)
                {
                    AnimationClip animation = animationClips[i];
                    Hash128 animationHash = BakingUtils.ComputeAnimationHash(animation, avatar);
                    data.AnimClips.Add(animationHash);
                }

                AddComponent(entity, data);
            }
        }
    }
#endif
}