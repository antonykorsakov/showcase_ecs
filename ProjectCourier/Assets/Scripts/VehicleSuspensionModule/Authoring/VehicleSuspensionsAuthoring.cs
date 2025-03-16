using Unity.Entities;
using UnityEngine;
using VehicleSuspensionModule.Data;

namespace VehicleSuspensionModule.Authoring
{
#if UNITY_EDITOR
    public class VehicleSuspensionsAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject[] _suspensions;

        private float _restLength = 0.5f;

        // private float _springTravel = 0.5f;
        private int _springStiffness = 30000;
        private float _wheelRadius = 0.33f;

        class Baker : Baker<VehicleSuspensionsAuthoring>
        {
            public override void Bake(VehicleSuspensionsAuthoring authoring)
            {
                var vehicleEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<VehicleSuspensionData>(vehicleEntity);
                AddComponent(vehicleEntity, new VehicleSuspensionConfig
                {
                    RestLength = authoring._restLength,
                    SpringStiffness = authoring._springStiffness,
                    WheelRadius = authoring._wheelRadius,
                });
            }
        }
    }
#endif
}