using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace VehicleModule.Data
{
    [TemporaryBakingType]
    public struct VehicleBackingData : IComponentData
    {
        public NativeArray<WheelBakingInfo> Wheels;
    }

    public struct WheelBakingInfo
    {
        public Entity Wheel;
        public Entity GraphicalRepresentation;
        public byte UsedForSteering;
        public byte UsedForDriving;
        public RigidTransform ChassisFromSuspension;
    }
    
    public struct WheelBakingData : IBufferElementData
    {
        public Entity Wheel;
        public Entity GraphicalRepresentation;
        public byte UsedForSteering;
        public byte UsedForDriving;
        public RigidTransform ChassisFromSuspension;
    }
    
    // public struct VehicleBackingData : IComponentData
    // {
    //     public BlobAssetReference<VehicleBackingBlob> WheelsBlob;
    // }
    //
    // public struct VehicleBackingBlob
    // {
    //     public BlobArray<WheelBakingInfo> Wheels;
    // }
    //
    // public struct WheelBakingInfo
    // {
    //     public Entity Wheel;
    //     public Entity GraphicalRepresentation;
    //     public byte UsedForSteering;
    //     public byte UsedForDriving;
    //     public RigidTransform ChassisFromSuspension;
    // }
}