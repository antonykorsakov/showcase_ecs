using Unity.Entities;

namespace VehicleModule.Data
{
    public struct VehicleSpeed : IComponentData
    {
        public float TopSpeed;
        public float DesiredSpeed;
        public float Damping;
        public byte DriveEngaged;
    }
}