using MovementModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
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

            foreach (var (localTransform, wheel, entity)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<WheelData>>().WithEntityAccess())
            {
                var newLocalTransform = localTransform;

                Entity vehicleEntity = wheel.ValueRO.Vehicle;
                if (vehicleEntity == Entity.Null)
                    return;

                int vehicleIndex = world.GetRigidBodyIndex(vehicleEntity);
                if (-1 == vehicleIndex || vehicleIndex >= world.NumDynamicBodies)
                    return;

                // vehicle entity info
                var vehicleTransform = SystemAPI.GetComponent<LocalTransform>(vehicleEntity);
                var vehiclePosition = vehicleTransform.Position;
                var vehicleRotation = vehicleTransform.Rotation;
                var vehicleUp = math.mul(vehicleRotation, new float3(0f, 1f, 0f));
                var vehicleForward = math.mul(vehicleRotation, new float3(0f, 0f, 1f));
                var vehicleRight = math.mul(vehicleRotation, new float3(1f, 0f, 0f));

                // wheel entity info
                var wheelPosition = localTransform.ValueRO.Position;
                var wheelUp = vehicleUp;
                var wheelForward = vehicleForward;
                var wheelRight = vehicleRight;
                var wheelVelocity = world.GetLinearVelocity(vehicleIndex, wheelPosition);
                var wheelRay = new RaycastInput
                {
                    Start = wheelPosition,
                    End = wheelPosition - new float3(0f, 1f, 0f),
                    Filter = world.GetCollisionFilter(vehicleIndex),
                };
                var wheelHit = world.CastRay(wheelRay, out var wheelRayResult);

                // Apply rotate steering wheels
                if (wheel.ValueRO.UsedForSteering != 0)
                {
                    float desiredSteeringAngle = SystemAPI.HasComponent<VehicleSteering>(vehicleEntity)
                        ? SystemAPI.GetComponent<VehicleSteering>(vehicleEntity).DesiredSteeringAngle
                        : 0f;

                    // desiredSteeringAngle - in radian!!!
                    quaternion wRotation = quaternion.AxisAngle(vehicleUp, desiredSteeringAngle);
                    wheelRight = math.rotate(wRotation, wheelRight);
                    wheelForward = math.rotate(wRotation, wheelForward);

                    newLocalTransform.ValueRW.Rotation = quaternion.AxisAngle(math.up(), desiredSteeringAngle);
                }

                // get speed
                float driveDesiredSpeed = 0f;
                bool driveEngaged = false;
                if (SystemAPI.HasComponent<VehicleSpeed>(vehicleEntity))
                {
                    var vehicleSpeedData = SystemAPI.GetComponent<VehicleSpeed>(vehicleEntity);
                    driveDesiredSpeed = vehicleSpeedData.DesiredSpeed;
                    driveEngaged = vehicleSpeedData.DriveEngaged != 0;
                }

                // Apply physics
                if (!driveEngaged && wheelHit)
                {
                    float currentSpeedForward = math.dot(wheelVelocity, wheelForward);
                    float deltaSpeedForward = math.clamp(driveDesiredSpeed - currentSpeedForward, -10.0f, 10.0f);

                    var impulse = deltaSpeedForward * wheelForward * 25f;
                    var groundIndex = wheelRayResult.RigidBodyIndex;
                    var isStatic = groundIndex < 0 || groundIndex >= world.NumDynamicBodies;

                    world.ApplyImpulse(vehicleIndex, impulse, wheelPosition);
                    if (!isStatic)
                        world.ApplyImpulse(groundIndex, -impulse, wheelPosition);
                }

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