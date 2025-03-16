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
using VehicleSpeedModule.Data;
using VehicleSteeringModule.Data;

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
                var vehicleData = SystemAPI.GetComponent<VehicleData>(vehicleEntity);
                var vehicleConfig = SystemAPI.GetComponent<VehicleConfig>(vehicleEntity);
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
                RigidTransform chassisFromSuspension = wheelData.ValueRO.ChassisFromSuspension;
                RigidTransform suspensionFromWheel = new RigidTransform
                {
                    pos = wheelTransform.ValueRO.Position,
                    rot = wheelTransform.ValueRO.Rotation
                };
                var chassisFromWheel = math.mul(chassisFromSuspension, suspensionFromWheel);
                var worldFromLocal = math.mul(worldFromChassis, chassisFromWheel);
                var worldFromSuspension = math.mul(worldFromChassis, chassisFromSuspension);

                // wheel entity info
                var wheelUp = vehicleUp;
                var wheelForward = vehicleForward;
                var wheelRight = vehicleRight;
                var wheelRay = new RaycastInput
                {
                    Start = worldFromSuspension.pos,
                    End = -vehiclePosition * (vehicleConfig.RestLength) + worldFromSuspension.pos,
                    Filter = world.GetCollisionFilter(vehicleIndex),
                };
                var wheelHit = world.CastRay(wheelRay, out var wheelRayResult);
                var wheelPosition = wheelHit ? wheelRayResult.Position : wheelRay.End;
                var wheelVelocity = world.GetLinearVelocity(vehicleIndex, wheelPosition);
                
                // #region Suspension
                // {
                //     // Calculate and apply the impulses
                //     var posA = rayEnd;
                //     var posB = rayResult.Position;
                //     var lvA = currentSpeedUp * wheelUp;
                //     var lvB = world.GetLinearVelocity(rayResult.RigidBodyIndex, posB);
                //
                //     var impulse = mechanics.suspensionStrength * (posB - posA) + mechanics.suspensionDamping * (lvB - lvA);
                //     impulse = impulse * invWheelCount;
                //     float impulseUp = math.dot(impulse, wheelUp);
                //
                //     // Suspension shouldn't necessarily pull the vehicle down!
                //     float downForceLimit = -0.25f;
                //     if (downForceLimit < impulseUp)
                //     {
                //         impulse = impulseUp * wheelUp;
                //         world.ApplyImpulse(vehicleIndex, impulse, posA);
                //     }
                // }
                // #endregion

                // Apply rotate steering wheels
                if (wheelData.ValueRO.UsedForSteering != 0)
                {
                    float steeringAngle = SystemAPI.GetComponent<VehicleSteeringData>(vehicleEntity).Value;

                    // desiredSteeringAngle - in radian!!!
                    quaternion wRotation = quaternion.AxisAngle(vehicleUp, steeringAngle);
                    wheelRight = math.rotate(wRotation, wheelRight);
                    wheelForward = math.rotate(wRotation, wheelForward);

                    newWheelTransform.ValueRW.Rotation = quaternion.AxisAngle(math.up(), steeringAngle);
                }

                // get speed
                var speed = SystemAPI.GetComponent<VehicleSpeedData>(vehicleEntity).Value;


                var restLength = 0.5f;
                var springStiffness = 30000;
                var wheelRadius = 0.33f;

                var hitDistance = math.length(wheelRay.Start - wheelRayResult.Position);
                var springLength = hitDistance - wheelRadius;
                var springForce = springStiffness * (restLength - springLength);

                // // Apply physics
                // if (wheelHit)
                // {
                //     float3 impulse = float3.zero;
                //
                //     var surfaceEntity = wheelRayResult.Entity;
                //     var frictionCoef = SystemAPI.HasComponent<SurfaceFrictionData>(surfaceEntity)
                //         ? SystemAPI.GetComponent<SurfaceFrictionData>(surfaceEntity).DynamicFriction
                //         : 0f;
                //
                //     // forward
                //     {
                //         float currentSpeedForward = math.dot(wheelVelocity, wheelForward);
                //         float deltaSpeedForward = speed - currentSpeedForward;
                //         impulse += deltaSpeedForward * wheelForward;
                //
                //         // +friction
                //         // if (frictionCoef > 0)
                //         // {
                //         //     // https://www.calculatorsoup.com/calculators/physics/friction.php
                //         //     float frictionForce = -currentSpeedForward * frictionCoef;
                //         //     var frictionImpulse = frictionForce * wheelForward;
                //         //     impulse += frictionImpulse;
                //         // }
                //     }
                //
                //     // right
                //     {
                //         float currentSpeedRight = math.dot(wheelVelocity, wheelRight);
                //         float deltaSpeedRight = 0 - currentSpeedRight; // * 0.5f;
                //         impulse += deltaSpeedRight * wheelRight;
                //
                //         // +friction
                //         // if (frictionCoef > 0)
                //         // {
                //         //     // https://www.calculatorsoup.com/calculators/physics/friction.php
                //         //     float lateralFrictionForce = -currentSpeedRight * frictionCoef;
                //         //     var lateralFrictionImpulse = lateralFrictionForce * wheelRight;
                //         //     impulse += lateralFrictionImpulse;
                //         // }
                //     }
                //
                //     // ApplyImpulse
                //     {
                //         world.ApplyImpulse(vehicleIndex, impulse, wheelPosition);
                //
                //         var groundIndex = wheelRayResult.RigidBodyIndex;
                //         var isStatic = groundIndex < 0 || groundIndex >= world.NumDynamicBodies;
                //         if (!isStatic)
                //             world.ApplyImpulse(groundIndex, -impulse, wheelPosition);
                //     }
                // }
                //
                // if (!newWheelTransform.ValueRO.Equals(wheelTransform.ValueRO))
                // {
                //     commandBuffer.SetComponent(wheelEntity, newWheelTransform.ValueRO);
                // }
            }

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}