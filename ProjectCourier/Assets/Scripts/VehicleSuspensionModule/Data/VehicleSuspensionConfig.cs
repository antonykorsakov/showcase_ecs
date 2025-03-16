using Unity.Entities;

namespace VehicleSuspensionModule.Data
{
    public struct VehicleSuspensionConfig : IComponentData
    {
        public float RestLength;
        public int SpringStiffness;
        public float WheelRadius;
    }
}