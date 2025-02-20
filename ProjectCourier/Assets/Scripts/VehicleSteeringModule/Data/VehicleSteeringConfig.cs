using Unity.Entities;
using Unity.Mathematics;

namespace VehicleSteeringModule.Data
{
    public struct VehicleSteeringConfig : IComponentData
    {
        public float2 MaxInterval;
        public float ActiveAcceleration;
        public float IdleDeceleration;
    }
}