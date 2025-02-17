using MovementModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using VehicleModule.Data;

namespace VehicleModule.Controller
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsInitializeGroup)), UpdateBefore(typeof(PhysicsSimulationGroup))]
    public partial struct VehicleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VehicleData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new PrepareVehiclesJob().ScheduleParallel(state.Dependency);
            state.Dependency.Complete();

            PhysicsWorld world = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>().ValueRW.PhysicsWorld;

            // update each wheel
            var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            foreach (var (localTransform, wheelData, entity)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<WheelData>>().WithEntityAccess())
            {
                var newLocalTransform = localTransform;

                Entity vehicleEntity = wheelData.ValueRO.Vehicle;
                if (vehicleEntity == Entity.Null)
                    return;

                int vehicleEntityIndex = world.GetRigidBodyIndex(vehicleEntity);
                if (-1 == vehicleEntityIndex || vehicleEntityIndex >= world.NumDynamicBodies)
                    return;

                var vehicleData = SystemAPI.GetComponent<VehicleData>(vehicleEntity);
                var vehicleTransform = SystemAPI.GetComponent<LocalTransform>(vehicleEntity);

                var vehiclePosition = vehicleTransform.Position;
                var vehicleRotation = vehicleTransform.Rotation;
                var vehicleUp = math.mul(vehicleRotation, new float3(0f, 1f, 0f));
                var vehicleForward = math.mul(vehicleRotation, new float3(0f, 0f, 1f));
                var vehicleRight = math.mul(vehicleRotation, new float3(1f, 0f, 0f));

                var wheelUp = vehicleUp;
                var wheelForward = vehicleForward;
                var wheelRight = vehicleRight;

                float desiredSteeringAngle = SystemAPI.HasComponent<MoveDirectionData>(vehicleEntity)
                    ? SystemAPI.GetComponent<MoveDirectionData>(vehicleEntity).Value.x * 10.0f
                    : 0f;

                Debug.LogError($"desiredSteeringAngle = {desiredSteeringAngle};");

                #region handle wheel steering

                {
                    // update yaw angle if wheel is used for steering
                    if (wheelData.ValueRO.UsedForSteering != 0)
                    {
                        quaternion wRotation = quaternion.AxisAngle(vehicleUp, desiredSteeringAngle);
                        wheelForward = math.rotate(wRotation, wheelForward);
                        wheelRight = math.rotate(wRotation, wheelRight);

                        newLocalTransform.ValueRW.Rotation = quaternion.AxisAngle(math.up(), desiredSteeringAngle);
                    }
                }

                #endregion

                #region handle wheel rotation

                {
                    float wheelRotation = 1.0f;
                    wheelRotation = math.radians(wheelRotation);

                    var rotate = quaternion.AxisAngle(new float3(1f, 0f, 0f), wheelRotation);
                    newLocalTransform.ValueRW.Rotation = math.mul(localTransform.ValueRO.Rotation, rotate);
                }

                #endregion

                if (!newLocalTransform.ValueRO.Equals(localTransform.ValueRO))
                {
                    commandBuffer.SetComponent(entity, newLocalTransform.ValueRO);
                }
            }

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}