using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using VehicleSteeringModule.Data;

namespace VehicleSteeringModule.Authoring
{
#if UNITY_EDITOR
    public class VehicleSteeringAuthoring : MonoBehaviour
    {
        // in radian
        [SerializeField] private float2 _maxInterval = new(-0.785f, 0.785f);
        [SerializeField] private float _activeAcceleration = 10.0f;
        [SerializeField] private float _idleDeceleration = 3.0f;

        private void OnValidate()
        {
            Assert.IsTrue(_activeAcceleration > 0);
            Assert.IsTrue(_idleDeceleration > 0);
        }

        class Baker : Baker<VehicleSteeringAuthoring>
        {
            public override void Bake(VehicleSteeringAuthoring authoring)
            {
                var vehicleEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<VehicleSteeringData>(vehicleEntity);
                AddComponent(vehicleEntity, new VehicleSteeringConfig
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