using Unity.Entities;

namespace VehicleModule.Data
{
    public struct VehicleSteeringData : IComponentData
    {
        /// <summary>
        /// -1 - left
        ///  0 - neutral
        ///  1 - right
        /// </summary>
        public int InputState;

        public float Value;
    }
}