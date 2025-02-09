using CharacterAnimModule.Data;
using MovementModule.Data;
using Rukhanka;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace CharacterAnimModule.Controller
{
    public partial struct CharacterAnimationJob : IJobEntity
    {
        [ReadOnly] public NativeHashMap<Hash128, BlobAssetReference<AnimationClipBlob>> AnimDB;
        [ReadOnly] public NativeArray<int> Indices1;
        [ReadOnly] public NativeArray<int> Indices2;
        [ReadOnly] public NativeArray<float> Speeds;

        public float AnimTime;

        private void Execute(ref DynamicBuffer<AnimationToProcessComponent> atps,
            CharacterAnimData animData, MoveSpeedData speedData)
        {
            ScriptedAnimator.ResetAnimationState(ref atps);
            var currentSpeed = speedData.Value;

            var weights = new NativeArray<float>(Speeds.Length, Allocator.Temp);

            float totalWeight = 0f;
            for (int i = 0; i < Speeds.Length; ++i)
            {
                float distance = math.abs(Speeds[i] - currentSpeed);
                weights[i] = 1f / (distance + 0.0001f);
                totalWeight += weights[i];
            }

            for (int i = 0; i < Speeds.Length; ++i)
            {
                var blendFactor = currentSpeed / Speeds[i];

                var animHash1 = animData.AnimClips[Indices1[i]];
                var animHash2 = animData.AnimClips[Indices2[i]];
                var normalWeight = weights[i] / totalWeight;

                switch (blendFactor)
                {
                    case <= 0f:
                        PlaySingleAnimation(ref atps, animHash1, normalWeight);
                        break;

                    case >= 1f:
                        PlaySingleAnimation(ref atps, animHash2, normalWeight);
                        break;

                    default:
                        PlayBlendAnimation(ref atps, animHash1, animHash2, blendFactor, normalWeight);
                        break;
                }
            }

            weights.Dispose();
        }

        private void PlaySingleAnimation(ref DynamicBuffer<AnimationToProcessComponent> atps,
            Hash128 animHash, float weight)
        {
            AnimDB.TryGetValue(animHash, out var clipBlob);
            ScriptedAnimator.PlayAnimation(ref atps, clipBlob, AnimTime, weight);
        }

        private void PlayBlendAnimation(ref DynamicBuffer<AnimationToProcessComponent> atps,
            Hash128 animHash1, Hash128 animHash2, float blendFactor, float weight)
        {
            AnimDB.TryGetValue(animHash1, out var clip1Blob);
            AnimDB.TryGetValue(animHash2, out var clip2Blob);
            ScriptedAnimator.BlendTwoAnimations(ref atps, clip1Blob, clip2Blob, AnimTime, blendFactor, weight);
        }
    }
}