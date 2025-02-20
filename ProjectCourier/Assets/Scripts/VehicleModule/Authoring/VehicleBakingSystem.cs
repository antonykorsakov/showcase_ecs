using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using VehicleModule.Data;

namespace VehicleModule.Authoring
{
    public partial struct VehicleBakingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WheelBakingData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (buffer, carEntity)
                     in SystemAPI.Query<DynamicBuffer<WheelBakingData>>().WithEntityAccess())
            {
                foreach (var wheelBakingData in buffer)
                {
                    commandBuffer.AddComponent(wheelBakingData.Wheel, new WheelData
                    {
                        Vehicle = carEntity,
                        GraphicalRepresentation = wheelBakingData.GraphicalRepresentation,
                        UsedForSteering = wheelBakingData.UsedForSteering,
                        UsedForDriving = wheelBakingData.UsedForDriving,
                        ChassisFromSuspension = wheelBakingData.ChassisFromSuspension,
                    });
                }

                commandBuffer.RemoveComponent<WheelBakingData>(carEntity);
            }

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();

            // MyDebug(ref state, typeof(WheelData));
            // MyDebug(ref state, typeof(MoveDirectionData));
            // MyDebug(ref state, typeof(VehicleData));
        }

        private void MyDebug(ref SystemState state, Type tag)
        {
            var query = state.EntityManager.CreateEntityQuery(tag);
            var items = query.ToEntityArray(Allocator.Temp);

            Debug.LogError($"{tag.Name}.count = {items.Length}");
            foreach (var item in items)
                Debug.LogError($"{tag.Name}: ID = {item.Index};");

            items.Dispose();
        }
    }
}