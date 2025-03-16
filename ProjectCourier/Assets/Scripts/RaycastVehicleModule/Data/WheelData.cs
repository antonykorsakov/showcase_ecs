using Unity.Entities;
using Unity.Mathematics;

namespace VehicleModule.Data
{
    public struct WheelData : IComponentData
    {
        public Entity Vehicle;
        public Entity GraphicalRepresentation;
        public byte UsedForSteering;
        public byte UsedForDriving;
        public RigidTransform ChassisFromSuspension;
    }
}