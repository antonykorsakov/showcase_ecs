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
        [SerializeField] private float2 _maxInterval = new(-15f, 80f);
        [SerializeField] private float _activeAcceleration = 30.0f;
        [SerializeField] private float _idleDeceleration = 20.0f;

        private void OnValidate()
        {
            Assert.IsTrue(_activeAcceleration > 0);
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
                    IdleDeceleration = authoring._idleDeceleration,
                });
            }
        }
    }
#endif
}