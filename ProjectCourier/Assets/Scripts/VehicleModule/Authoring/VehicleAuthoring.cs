using System.Collections.Generic;
using InputModule.Data;
using MovementModule.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using VehicleModule.Data;

namespace VehicleModule.Authoring
{
#if UNITY_EDITOR
    public class VehicleAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _chassis;
        [SerializeField] private List<GameObject> _suspensions = new();
        [SerializeField] private List<GameObject> _wheels = new();
        [SerializeField] private List<GameObject> _steeringWheels = new();
        [SerializeField] private List<GameObject> _driveWheels = new();

        // class Baker : Baker<VehicleAuthoring>
        // {
        //     public override void Bake(VehicleAuthoring authoring)
        //     {
        //         var vehicleEntity = GetEntity(TransformUsageFlags.Dynamic);
        //         AddComponent<VehicleData>(vehicleEntity);
        //         AddComponent<InteractableTag>(vehicleEntity);
        //         AddComponent<MoveDirectionData>(vehicleEntity);
        //
        //         RigidTransform worldFromChassis = new()
        //         {
        //             pos = authoring._chassis.transform.position,
        //             rot = authoring._chassis.transform.rotation
        //         };
        //
        //         int count = authoring._wheels.Count;
        //         using var builder = new BlobBuilder(Allocator.Temp);
        //         ref var root = ref builder.ConstructRoot<VehicleBackingBlob>();
        //         var wheelsArray = builder.Allocate(ref root.Wheels, count);
        //
        //         for (var index = 0; index < count; index++)
        //         {
        //             var wheelSource = authoring._wheels[index];
        //             var wheelGraphicalSource = wheelSource.transform;
        //             var suspensionSource = authoring._suspensions[index];
        //
        //             RigidTransform worldFromSuspension = new()
        //             {
        //                 pos = suspensionSource.transform.position,
        //                 rot = suspensionSource.transform.rotation
        //             };
        //             var chassisFromSuspension = math.mul(math.inverse(worldFromChassis), worldFromSuspension);
        //
        //             wheelsArray[index] = new WheelBakingInfo
        //             {
        //                 Wheel = GetEntity(wheelSource, TransformUsageFlags.Dynamic),
        //                 GraphicalRepresentation = GetEntity(wheelGraphicalSource, TransformUsageFlags.Dynamic),
        //                 UsedForSteering = (byte)(authoring._steeringWheels.Contains(wheelSource) ? 1 : 0),
        //                 UsedForDriving = (byte)(authoring._driveWheels.Contains(wheelSource) ? 1 : 0),
        //                 ChassisFromSuspension = chassisFromSuspension,
        //             };
        //         }
        //
        //         var blobAsset = builder.CreateBlobAssetReference<VehicleBackingBlob>(Allocator.Persistent);
        //         AddComponent(vehicleEntity, new VehicleBackingData { WheelsBlob = blobAsset });
        //     }
        // }

        class Baker : Baker<VehicleAuthoring>
        {
            public override void Bake(VehicleAuthoring authoring)
            {
                var vehicleEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<VehicleData>(vehicleEntity);

                RigidTransform worldFromChassis = new()
                {
                    pos = authoring._chassis.transform.position,
                    rot = authoring._chassis.transform.rotation
                };

                int count = authoring._wheels.Count;
                var array = new NativeArray<WheelBakingInfo>(count, Allocator.Temp);
                for (var index = 0; index < count; index++)
                {
                    //
                    var wheelSource = authoring._wheels[index];
                    //
                    var wheelGraphicalSource = wheelSource.transform;
                    //
                    var suspensionSource = authoring._suspensions[index];
                    RigidTransform worldFromSuspension = new()
                    {
                        pos = suspensionSource.transform.position,
                        rot = suspensionSource.transform.rotation
                    };
                    var chassisFromSuspension = math.mul(math.inverse(worldFromChassis), worldFromSuspension);

                    array[index] = new WheelBakingInfo
                    {
                        Wheel = GetEntity(wheelSource, TransformUsageFlags.Dynamic),
                        GraphicalRepresentation = GetEntity(wheelGraphicalSource, TransformUsageFlags.Dynamic),
                        UsedForSteering = (byte)(authoring._steeringWheels.Contains(wheelSource) ? 1 : 0),
                        UsedForDriving = (byte)(authoring._driveWheels.Contains(wheelSource) ? 1 : 0),
                        ChassisFromSuspension = chassisFromSuspension,
                    };
                }

                AddComponent(vehicleEntity, new VehicleBackingData
                {
                    Wheels = array,
                });
            }

            NativeArray<Entity> ToNativeArray(List<GameObject> list, Allocator allocator)
            {
                if (list == null)
                    return default;

                var array = new NativeArray<Entity>(list.Count, allocator);
                for (int i = 0; i < list.Count; ++i)
                    array[i] = GetEntity(list[i], TransformUsageFlags.Dynamic);

                return array;
            }
        }
    }
#endif
}