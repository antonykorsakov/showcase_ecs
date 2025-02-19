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

            foreach (var (wheelTransform, wheelData, wheelEntity)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<WheelData>>().WithEntityAccess())
            {
                var newWheelTransform = wheelTransform;

                Entity vehicleEntity = wheelData.ValueRO.Vehicle;
                if (vehicleEntity == Entity.Null)
                    return;

                int vehicleIndex = world.GetRigidBodyIndex(vehicleEntity);
                if (-1 == vehicleIndex || vehicleIndex >= world.NumDynamicBodies)
                    return;

                // vehicle entity info
                var vehicleTransform = SystemAPI.GetComponent<LocalTransform>(vehicleEntity);
                var vehiclePosition = vehicleTransform.Position;
                var vehicleRotation = vehicleTransform.Rotation;
                var vehicleUp = math.mul(vehicleRotation, math.up());
                var vehicleForward = math.mul(vehicleRotation, math.forward());
                var vehicleRight = math.mul(vehicleRotation, math.right());

                RigidTransform worldFromChassis = new RigidTransform
                {
                    pos = vehiclePosition,
                    rot = vehicleRotation,
                };
                var worldFromSuspension = math.mul(worldFromChassis, wheelData.ValueRO.ChassisFromSuspension);

                // wheel entity info
                var wheelUp = vehicleUp;
                var wheelForward = vehicleForward;
                var wheelRight = vehicleRight;
                var wheelRay = new RaycastInput
                {
                    Start = worldFromSuspension.pos,
                    End = worldFromSuspension.pos - math.up(),
                    Filter = world.GetCollisionFilter(vehicleIndex),
                };
                var wheelHit = world.CastRay(wheelRay, out var wheelRayResult);
                var wheelPosition = wheelHit ? wheelRayResult.Position : wheelRay.End;
                var wheelVelocity = world.GetLinearVelocity(vehicleIndex, wheelPosition);

                // Apply rotate steering wheels
                if (wheelData.ValueRO.UsedForSteering != 0)
                {
                    float desiredSteeringAngle = SystemAPI.HasComponent<VehicleSteering>(vehicleEntity)
                        ? SystemAPI.GetComponent<VehicleSteering>(vehicleEntity).DesiredSteeringAngle
                        : 0f;

                    // desiredSteeringAngle - in radian!!!
                    quaternion wRotation = quaternion.AxisAngle(vehicleUp, desiredSteeringAngle);
                    wheelRight = math.rotate(wRotation, wheelRight);
                    wheelForward = math.rotate(wRotation, wheelForward);

                    newWheelTransform.ValueRW.Rotation = quaternion.AxisAngle(math.up(), desiredSteeringAngle);
                }

                // get speed
                float driveDesiredSpeed = 0f;
                if (SystemAPI.HasComponent<VehicleSpeed>(vehicleEntity))
                {
                    var vehicleSpeedData = SystemAPI.GetComponent<VehicleSpeed>(vehicleEntity);
                    driveDesiredSpeed = vehicleSpeedData.DesiredSpeed * 8f;
                }

                // Apply physics
                if (wheelHit)
                {
                    float3 impulse = float3.zero;

                    var surfaceEntity = wheelRayResult.Entity;
                    var frictionCoef = SystemAPI.HasComponent<SurfaceFrictionData>(surfaceEntity)
                        ? SystemAPI.GetComponent<SurfaceFrictionData>(surfaceEntity).DynamicFriction
                        : 0f;

                    // forward
                    {
                        float currentSpeedForward = math.dot(wheelVelocity, wheelForward);
                        float deltaSpeedForward = driveDesiredSpeed - currentSpeedForward;
                        impulse += deltaSpeedForward * wheelForward;

                        // +friction
                        if (frictionCoef > 0)
                        {
                            // https://www.calculatorsoup.com/calculators/physics/friction.php
                            float frictionForce = -currentSpeedForward * frictionCoef;
                            var frictionImpulse = frictionForce * wheelForward;
                            impulse += frictionImpulse;
                        }
                    }

                    // right
                    {
                        float currentSpeedRight = math.dot(wheelVelocity, wheelRight);
                        float deltaSpeedRight = 0 - currentSpeedRight; // * 0.5f;
                        impulse += deltaSpeedRight * wheelRight;

                        // +friction
                        if (frictionCoef > 0)
                        {
                            // https://www.calculatorsoup.com/calculators/physics/friction.php
                            float lateralFrictionForce = -currentSpeedRight * frictionCoef;
                            var lateralFrictionImpulse = lateralFrictionForce * wheelRight;
                            impulse += lateralFrictionImpulse;
                        }
                    }

                    // ApplyImpulse
                    {
                        world.ApplyImpulse(vehicleIndex, impulse, wheelPosition);

                        var groundIndex = wheelRayResult.RigidBodyIndex;
                        var isStatic = groundIndex < 0 || groundIndex >= world.NumDynamicBodies;
                        if (!isStatic)
                            world.ApplyImpulse(groundIndex, -impulse, wheelPosition);
                    }
                }

                if (!newWheelTransform.ValueRO.Equals(wheelTransform.ValueRO))
                {
                    commandBuffer.SetComponent(wheelEntity, newWheelTransform.ValueRO);
                }
            }

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}