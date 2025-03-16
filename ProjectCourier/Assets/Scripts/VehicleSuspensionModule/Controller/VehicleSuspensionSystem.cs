using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using VehicleSuspensionModule.Data;

namespace VehicleSuspensionModule.Controller
{
    public partial struct VehicleSuspensionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<VehicleSuspensionData, VehicleSuspensionConfig>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // PhysicsWorld world = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>().ValueRW.PhysicsWorld;
            // var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            //
            // foreach (var (data, config, localTransform, entity)
            //          in SystemAPI.Query<
            //              RefRW<VehicleSuspensionData>,
            //              RefRO<VehicleSuspensionConfig>,
            //              RefRO<LocalTransform>
            //          >().WithEntityAccess())
            // {
            //     Entity vehicleEntity = data.ValueRO.Vehicle;
            //     if (vehicleEntity == Entity.Null)
            //         return;
            //
            //     int vehicleIndex = world.GetRigidBodyIndex(vehicleEntity);
            //     if (-1 == vehicleIndex || vehicleIndex >= world.NumDynamicBodies)
            //         return;
            //
            //     var wheelRay = new RaycastInput
            //     {
            //         // Start = worldFromSuspension.pos,
            //         // End = worldFromSuspension.pos - math.up(),
            //         // Filter = world.GetCollisionFilter(vehicleIndex),
            //     };
            //     var wheelHit = world.CastRay(wheelRay, out var wheelRayResult);
            //     var wheelPosition = wheelHit ? wheelRayResult.Position : wheelRay.End;
            // }
        }
    }
}