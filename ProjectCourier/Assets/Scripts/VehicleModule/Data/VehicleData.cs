using Unity.Entities;
using Unity.Mathematics;

namespace VehicleModule.Data
{
    public struct VehicleData : IComponentData
    {
        public float SlopeSlipFactor;
        public float3 WorldCenterOfMass;
    }
}