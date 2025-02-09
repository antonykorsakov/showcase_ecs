using Unity.Entities;

namespace SpawnModule.Data
{
    public struct SpawnTimerData : IComponentData
    {
        public float TimeRemaining;
    }
}