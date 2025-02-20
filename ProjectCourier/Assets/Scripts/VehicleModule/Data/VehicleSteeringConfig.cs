using Unity.Entities;

namespace VehicleModule.Data
{
    public struct VehicleSteeringConfig : IComponentData
    {
        /// <summary>
        /// in radian
        /// </summary>
        public float MaxSteeringAngle;

        public float SteeringSpeed;
        public float ReturnSpeed;
    }
}