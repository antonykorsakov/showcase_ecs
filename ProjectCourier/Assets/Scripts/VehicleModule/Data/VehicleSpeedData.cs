using Unity.Entities;

namespace VehicleModule.Data
{
    public struct VehicleSpeedData : IComponentData
    {
        /// <summary>
        /// +1 - forward
        /// 0 - nothing
        /// -1 - backward 
        /// </summary>
        public int ThrottleInputState;

        public float CurrentSpeed;
    }
}