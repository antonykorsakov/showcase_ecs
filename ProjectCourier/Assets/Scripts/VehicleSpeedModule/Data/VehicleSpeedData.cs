using Unity.Entities;

namespace VehicleSpeedModule.Data
{
    public struct VehicleSpeedData : IComponentData
    {
        /// <summary>
        /// +1 - forward
        /// 0 - nothing
        /// -1 - backward 
        /// </summary>
        public int InputState;

        public float Value;
    }
}