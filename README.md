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
- https://autovogdenie.ru/wp-content/uploads/2020/06/2020-06-24_9-33-13.jpg

есть идея замнеить "Ray cast" на "Overlap" из-за проблемы с переходами между блоками
+ https://docs.unity3d.com/Packages/com.unity.physics@1.3/manual/collision-queries.html

- https://assetstore.unity.com/packages/tools/physics/nwh-vehicle-physics-2-166252
- https://nwhcoding.com/
- https://nwhvehiclephysics.com/doku.php
- https://docs.unity3d.com/Manual/class-WheelCollider.html

- https://github.com/Unity-Technologies/ECS-Network-Racing-Sample
- https://assetstore.unity.com/packages/tools/physics/vehicle-physics-pro-community-edition-153556
- https://assetstore.unity.com/packages/tools/physics/wheel-controller-3d-74512
- https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/da028e7d-1a4f-48e6-927a-f8fcf0f44b39.webp
- https://nwhvehiclephysics.com/doku.php/Setup/VehicleController
- https://www.youtube.com/watch?v=SKXqWcaoTGE
- https://youtu.be/O08UhoIxGRc?si=-bi2tpDyRjnr_NqB&t=245
- https://docs.unity3d.com/6000.0/Documentation/ScriptReference/UnityEngine.VehiclesModule.html
- suspension https://global.honda/en/newsroom/worldnews/1997/4970702b.html
- https://www.bgak.by/education_uch/sat/images/7-2/image002.jpg
- https://lh4.googleusercontent.com/proxy/9pH6JPLzAkvfO51nt2oS0ieE37LCL3WcqKqdWXhpVkD8vXEcY6x-NafxfI9FHeR8Jj2S1RhtNW6FKyc
- https://www.researchgate.net/profile/Bram-Cornelis/publication/288791064/figure/fig4/AS:668584002986001@1536414256551/System-under-investigation-steering-knuckle-component-of-McPherson-suspension-system.ppm
- https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTZ1g2RvWujhF3_n_r--cgVETCy9uh7hNeZwohakFYRqgHxFcyZzeifJ_ErL0jXbtaMUIw&usqp=CAU