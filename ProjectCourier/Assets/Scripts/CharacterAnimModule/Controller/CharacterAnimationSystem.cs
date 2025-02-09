using Rukhanka;
using Unity.Collections;
using Unity.Entities;

namespace CharacterAnimModule.Controller
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(RukhankaAnimationSystemGroup))]
    [UpdateBefore(typeof(AnimationProcessSystem))]
    public partial class CharacterAnimationSystem : SystemBase
    {
        private float _animTime;

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingleton<BlobDatabaseSingleton>(out var blobDB))
                return;

            _animTime += SystemAPI.Time.DeltaTime;
            var animJob = new CharacterAnimationJob
            {
                AnimDB = blobDB.animations,
                Indices1 = new NativeArray<int>(2, Allocator.TempJob) { [0] = 0, [1] = 0 },
                Indices2 = new NativeArray<int>(2, Allocator.TempJob) { [0] = 1, [1] = 2 },
                Speeds = new NativeArray<float>(2, Allocator.TempJob) { [0] = 3.0f, [1] = 4.0f },

                AnimTime = _animTime,
            };
            animJob.ScheduleParallel();
        }
    }
}