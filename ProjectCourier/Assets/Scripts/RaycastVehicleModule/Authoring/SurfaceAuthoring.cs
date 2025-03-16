using Unity.Entities;
using UnityEngine;
using VehicleModule.Data;

namespace VehicleModule.Authoring
{
#if UNITY_EDITOR
    public class SurfaceAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Static friction coefficient.
        /// Determines how hard it is to start moving on this surface.
        /// - Road: 0.8 – 1.0 (high grip, no slipping)
        /// - Swamp: 0.4 – 0.6 (low grip, possible slipping)
        /// </summary>
        [SerializeField] private float _staticFriction = 0.9f;

        /// <summary>
        /// Dynamic friction coefficient.
        /// Determines resistance when the vehicle is already moving.
        /// - Road: 0.7 – 0.9 (smooth movement, slight resistance)
        /// - Swamp: 0.2 – 0.4 (strong resistance, vehicle slows down quickly)
        /// </summary>
        [SerializeField] private float _dynamicFriction = 0.8f;

        class Baker : Baker<SurfaceAuthoring>
        {
            public override void Bake(SurfaceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.WorldSpace | TransformUsageFlags.Renderable);
                AddComponent(entity, new SurfaceData
                {
                    StaticFriction = authoring._staticFriction,
                    DynamicFriction = authoring._dynamicFriction,
                });
            }
        }
    }
#endif
}