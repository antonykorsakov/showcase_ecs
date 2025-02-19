using Unity.Entities;

namespace VehicleModule.Data
{
    public struct SurfaceFrictionData : IComponentData
    {
        public float StaticFriction;
        public float DynamicFriction;
    }
}