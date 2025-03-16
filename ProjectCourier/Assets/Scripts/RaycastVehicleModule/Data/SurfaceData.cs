using Unity.Entities;

namespace VehicleModule.Data
{
    public struct SurfaceData : IComponentData
    {
        public float StaticFriction;
        public float DynamicFriction;
    }
}