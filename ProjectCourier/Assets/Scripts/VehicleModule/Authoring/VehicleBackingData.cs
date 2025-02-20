using Unity.Entities;
using Unity.Mathematics;

namespace VehicleModule.Data
{
    public struct WheelBakingData : IBufferElementData
    {
        public Entity Wheel;
        public Entity GraphicalRepresentation;
        public byte UsedForSteering;
        public byte UsedForDriving;
        public RigidTransform ChassisFromSuspension;
    }
}