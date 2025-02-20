using Unity.Entities;
using Unity.Mathematics;

namespace VehicleSpeedModule.Data
{
    public struct VehicleSpeedConfig : IComponentData
    {
        public float2 MaxInterval;
        public float ActiveAcceleration;
        public float IdleDeceleration;
    }
}