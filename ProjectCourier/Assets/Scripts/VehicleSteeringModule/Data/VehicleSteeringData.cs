using Unity.Entities;

namespace VehicleSteeringModule.Data
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