using System.Collections.Generic;
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
        [SerializeField] private List<GameObject> _wheels = new();
        [SerializeField] private List<GameObject> _wheelsPivot = new();
        [SerializeField] private List<GameObject> _steeringWheels = new();
        [SerializeField] private List<GameObject> _driveWheels = new();

        class Baker : Baker<VehicleAuthoring>
        {
            public override void Bake(VehicleAuthoring authoring)
            {
                var vehicleEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<VehicleData>(vehicleEntity);
                AddComponent<VehicleSpeedData>(vehicleEntity);
                AddComponent(vehicleEntity, new VehicleSpeedConfig
                {
                    MaxSpeed = 80,
                    AccelerationRate = 30,
                    IdleDecelerationRate = 20,
                });
                AddComponent<VehicleSteeringData>(vehicleEntity);
                AddComponent(vehicleEntity, new VehicleSteeringConfig
                {
                    MaxSteeringAngle = 0.785f,
                    SteeringSpeed = 10,
                    ReturnSpeed = 3,
                });


                RigidTransform worldFromChassis = new()
                {
                    pos = authoring._chassis.transform.position,
                    rot = authoring._chassis.transform.rotation
                };

                int count = authoring._wheels.Count;

                var buffer = AddBuffer<WheelBakingData>(vehicleEntity);
                for (var index = 0; index < count; index++)
                {
                    //
                    var wheelSource = authoring._wheels[index];
                    //
                    var wheelGraphicalSource = wheelSource.transform;
                    //
                    var suspensionSource = authoring._wheelsPivot[index];
                    RigidTransform worldFromSuspension = new()
                    {
                        pos = suspensionSource.transform.position,
                        rot = suspensionSource.transform.rotation
                    };
                    var chassisFromSuspension = math.mul(math.inverse(worldFromChassis), worldFromSuspension);

                    buffer.Add(new WheelBakingData()
                    {
                        Wheel = GetEntity(wheelSource, TransformUsageFlags.Dynamic),
                        GraphicalRepresentation = GetEntity(wheelGraphicalSource, TransformUsageFlags.Dynamic),
                        UsedForSteering = (byte)(authoring._steeringWheels.Contains(wheelSource) ? 1 : 0),
                        UsedForDriving = (byte)(authoring._driveWheels.Contains(wheelSource) ? 1 : 0),
                        ChassisFromSuspension = chassisFromSuspension,
                    });
                }
            }
        }
    }
#endif
}