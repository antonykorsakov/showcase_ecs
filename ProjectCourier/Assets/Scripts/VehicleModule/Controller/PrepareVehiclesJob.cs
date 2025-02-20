using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using VehicleModule.Data;

namespace VehicleModule.Controller
{
    // [BurstCompile]
    public partial struct PrepareVehiclesJob : IJobEntity
    {
        private void Execute(Entity entity, ref VehicleData vehicleData, in PhysicsMass mass, in LocalTransform localTransform)
        {
            // vehicleData.WorldCenterOfMass = mass.GetCenterOfMassWorldSpace(localTransform.Position, localTransform.Rotation);
            //
            // // calculate a simple slip factor based on chassis tilt
            // float3 worldUp = math.mul(localTransform.Rotation, math.up());
            //
            // vehicleData.SlopeSlipFactor = math.pow(math.abs(math.dot(worldUp, math.up())), 4f);
        }
    }
}