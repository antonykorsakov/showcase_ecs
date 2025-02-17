using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Authoring;
using VehicleModule.Data;

namespace VehicleModule.Authoring
{
    [RequireMatchingQueriesForUpdate]
    [UpdateAfter(typeof(EndColliderBakingSystem))]
    [UpdateAfter(typeof(PhysicsBodyBakingSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    public partial struct VehicleBakingSystem : ISystem
    {
        private const EntityQueryOptions QueryOptions = EntityQueryOptions.IncludePrefab
                                                        | EntityQueryOptions.IncludeDisabledEntities;

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (vehicleTmpData, vehicleEntity)
                     in SystemAPI.Query<RefRO<VehicleBackingData>>().WithEntityAccess().WithOptions(QueryOptions))
            {
                foreach (var wheelBakingInfo in vehicleTmpData.ValueRO.Wheels)
                {
                    var wheelEntity = wheelBakingInfo.Wheel;

                    commandBuffer.AddComponent(wheelEntity, new WheelData
                    {
                        Vehicle = vehicleEntity,
                        GraphicalRepresentation = wheelBakingInfo.GraphicalRepresentation,
                        UsedForSteering = wheelBakingInfo.UsedForSteering,
                        UsedForDriving = wheelBakingInfo.UsedForDriving,
                        ChassisFromSuspension = wheelBakingInfo.ChassisFromSuspension,
                    });
                }
            }

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}