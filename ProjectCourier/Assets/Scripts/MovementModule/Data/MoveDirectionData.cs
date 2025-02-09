using Unity.Entities;
using Unity.Mathematics;

namespace MovementModule.Data
{
    public struct MoveDirectionData : IComponentData
    {
        public float2 Value;
    }
}