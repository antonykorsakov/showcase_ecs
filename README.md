!!! IN PROGRESS !!!

version 0.0.1

# Technology stack
- Unity 6
- 3D URP
- ECS
  - Unity ECS (without plugins)
  - Animations (Rukhanka plugin)
  - Physics

# Assumed hierarchy of car items:
```plaintext
<Car>
 ├─ chassis
 │   └─ mechanics
 │      └─ suspension
 │         └─ wheel (rotates about yaw axis and translates along suspension up)
 │            └─ graphic (rotates about pitch axis)
```