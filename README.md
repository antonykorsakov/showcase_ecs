!!! IN PROGRESS !!!

version 0.0.1

# Technology stack
- Unity 6
- 3D URP
- ECS
  - Unity ECS (without plugins)
  - Animations (Rukhanka plugin)
  - [Physics](https://github.com/Unity-Technologies/EntityComponentSystemSamples/tree/master/PhysicsSamples)

# Assumed hierarchy of car items:
```plaintext
<Car>
 ├─ chassis
 │   └─ mechanics
 │      └─ suspension
 │         └─ wheel (rotates about yaw axis and translates along suspension up)
 │            └─ graphic (rotates about pitch axis)
```

# Car
- https://github.com/Unity-Technologies/EntityComponentSystemSamples/tree/master/PhysicsSamples
- https://github.com/Unity-Technologies/ECS-Network-Racing-Sample
- https://assetstore.unity.com/packages/tools/physics/vehicle-physics-pro-community-edition-153556
- https://assetstore.unity.com/packages/tools/physics/wheel-controller-3d-74512
- https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/da028e7d-1a4f-48e6-927a-f8fcf0f44b39.webp
- https://nwhvehiclephysics.com/doku.php/Setup/VehicleController
- https://www.youtube.com/watch?v=SKXqWcaoTGE
- https://youtu.be/O08UhoIxGRc?si=-bi2tpDyRjnr_NqB&t=245
- https://docs.unity3d.com/6000.0/Documentation/ScriptReference/UnityEngine.VehiclesModule.html