using Unity.Entities;

namespace VehicleModule.Data
{
    public struct VehicleSpeedConfig : IComponentData
    {
        public float MaxSpeed;
        public float AccelerationRate;
        public float IdleDecelerationRate;

        // /// <summary>
        // /// Дополнительный коэффициент замедления при движении назад (если требуется отличать).
        // /// </summary>
        // public float ReverseDecelerationRate;
    }
}