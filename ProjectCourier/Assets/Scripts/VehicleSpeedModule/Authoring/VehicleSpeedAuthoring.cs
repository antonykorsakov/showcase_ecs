using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using VehicleSpeedModule.Data;

namespace VehicleSpeedModule.Authoring
{
#if UNITY_EDITOR
    public class VehicleSpeedAuthoring : MonoBehaviour
    {
        [SerializeField] private float2 _maxInterval = new(-20f, 35f);
        [SerializeField] private float _activeAcceleration = 30.0f;
        [SerializeField] private float _activeDeceleration = 150.0f;
        [SerializeField] private float _idleDeceleration = 10.0f;

        private void OnValidate()
        {
            Assert.IsTrue(_activeAcceleration > 0);
            Assert.IsTrue(_activeDeceleration > 0);
            Assert.IsTrue(_idleDeceleration > 0);
        }

        class Baker : Baker<VehicleSpeedAuthoring>
        {
            public override void Bake(VehicleSpeedAuthoring authoring)
            {
                var vehicleEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<VehicleSpeedData>(vehicleEntity);
                AddComponent(vehicleEntity, new VehicleSpeedConfig
                {
                    MaxInterval = authoring._maxInterval,
                    ActiveAcceleration = authoring._activeAcceleration,
                    ActiveDeceleration = authoring._activeDeceleration,
                    IdleDeceleration = authoring._idleDeceleration,
                });
            }
        }
    }
#endif
}