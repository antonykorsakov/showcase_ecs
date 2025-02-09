using Unity.Entities;

namespace SpeedModifiersModule.Data
{
    public struct SpeedBoostData : IComponentData
    {
        public float Value;
        public float Max;
    }
}