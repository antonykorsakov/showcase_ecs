using Unity.Entities;

namespace VehicleModule.Data
{
    public struct VehicleSteering : IComponentData
    {
        public float MaxSteeringAngle;
        public float DesiredSteeringAngle;
        public float Damping;
    }
}