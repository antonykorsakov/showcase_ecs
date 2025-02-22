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
v.2 - mini
```plaintext
<Car>
└─ Chassis 
    ├─ Suspension
    │   └─ Spring/Damper
    │       └─ Wheel
    ├─ Suspension
    │   └─ Spring/Damper
    │       └─ Wheel
    ├─ Suspension
    │   └─ Spring/Damper
    │       └─ Wheel
    └─ Suspension
        └─ Spring/Damper
            └─ Wheel 
```

v.2 - full
```plaintext
<Car>
 ├─ Chassis 
 │   ├─ Front Suspension
 │   │    ├─ Spring/Damper
 │   │    │   └─ Wheel Steering Knuckle
 │   │    │          └─ Wheel
 │   │    └─ (Steering Linkages, Control Arms, etc.)
 │   ├─ Rear Suspension
 │   │    ├─ Spring/Damper
 │   │    │   └─ Rear Knuckle
 │   │    │          └─ Wheel
 │   │    └─ (Control Arms, Anti-Roll Bars, etc.)
```

v.1
```plaintext
<Car>
 ├─ chassis
 │   └─ mechanics
 │      └─ suspension
 │         └─ wheel (rotates about yaw axis and translates along suspension up)
 │            └─ graphic (rotates about pitch axis)
```

# Car
+ https://github.com/Unity-Technologies/EntityComponentSystemSamples/tree/master/PhysicsSamples
+ https://www.youtube.com/watch?v=x0LUiE0dxP0&list=PLcbsEpz1iFyjjddSqLxnnGSJthfCcmsav
- https://www.youtube.com/watch?v=CdPYlj5uZeI

- https://assetstore.unity.com/packages/tools/physics/nwh-vehicle-physics-2-166252
- https://nwhcoding.com/
- https://nwhvehiclephysics.com/doku.php

- https://github.com/Unity-Technologies/ECS-Network-Racing-Sample
- https://assetstore.unity.com/packages/tools/physics/vehicle-physics-pro-community-edition-153556
- https://assetstore.unity.com/packages/tools/physics/wheel-controller-3d-74512
- https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/da028e7d-1a4f-48e6-927a-f8fcf0f44b39.webp
- https://nwhvehiclephysics.com/doku.php/Setup/VehicleController
- https://www.youtube.com/watch?v=SKXqWcaoTGE
- https://youtu.be/O08UhoIxGRc?si=-bi2tpDyRjnr_NqB&t=245
- https://docs.unity3d.com/6000.0/Documentation/ScriptReference/UnityEngine.VehiclesModule.html