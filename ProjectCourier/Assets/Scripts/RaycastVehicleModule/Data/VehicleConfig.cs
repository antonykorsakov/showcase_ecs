using Unity.Entities;

namespace VehicleModule.Data
{
    public struct VehicleConfig : IComponentData
    {
        public float RestLength;
        public int SpringStiffness;
        public float WheelRadius;
    }
}