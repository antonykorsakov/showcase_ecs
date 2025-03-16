using Unity.Entities;
using Unity.Mathematics;

namespace VehicleSuspensionModule.Data
{
    public struct VehicleSuspensionData : IComponentData
    {
        public Entity Vehicle;
        public RigidTransform ChassisFromSuspension;
    }
}