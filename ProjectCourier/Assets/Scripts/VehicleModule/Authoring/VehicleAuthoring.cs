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

        class Baker : Baker<VehicleAuthoring>
        {
            public override void Bake(VehicleAuthoring authoring)
            {
                var vehicleEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<VehicleData>(vehicleEntity);
                AddComponent<VehicleSpeed>(vehicleEntity);
                AddComponent<VehicleSteering>(vehicleEntity);

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
                    var suspensionSource = authoring._suspensions[index];
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