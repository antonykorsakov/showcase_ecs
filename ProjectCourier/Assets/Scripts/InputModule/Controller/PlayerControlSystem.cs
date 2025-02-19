using InputModule.Data;
using MovementModule.Data;
using SpeedModifiersModule.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using VehicleModule.Data;

namespace InputModule.Controller
{
    public partial class PlayerControlSystem : SystemBase
    {
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private InputAction _interactAction;

        private InputAction _selectAction;
        private InputAction _completeAction;

        private bool _prepared;

        protected override void OnCreate()
        {
            var inputTagQuery = GetEntityQuery(ComponentType.ReadOnly<InputTag>());
            var controlledQuery = GetEntityQuery(ComponentType.ReadOnly<InputControlledTag>());

            RequireForUpdate(inputTagQuery);
            RequireForUpdate(controlledQuery);
        }

        protected override void OnStartRunning()
        {
            var inputTagQuery = GetEntityQuery(ComponentType.ReadOnly<InputTag>());
            if (inputTagQuery.CalculateEntityCount() > 1)
            {
                Debug.LogError("Multiple InputTag entities detected! Ensure only one exists.");
            }
        }

        protected override void OnUpdate()
        {
            if (!_prepared)
                Prepare();
            else
                InputHandler();
        }

        private void Prepare()
        {
            var entities = EntityManager.CreateEntityQuery(typeof(InputActionAsset)).ToEntityArray(Allocator.Temp);
            if (entities.Length == 0)
                return;

            var inputActionAsset = EntityManager.GetComponentObject<InputActionAsset>(entities[0]);

            _moveAction = inputActionAsset.FindAction("Move");
            _moveAction.Enable();
            _sprintAction = inputActionAsset.FindAction("Sprint");
            _sprintAction.Enable();
            _interactAction = inputActionAsset.FindAction("Interact");
            _interactAction.Enable();

            _selectAction = inputActionAsset.FindAction("Select");
            _selectAction.Enable();
            _completeAction = inputActionAsset.FindAction("Complete");
            _completeAction.Enable();

            entities.Dispose();

            Debug.Log("PlayerControlSystem input");
            _prepared = true;
        }

        private void InputHandler()
        {
            bool movePerformed = _moveAction.IsPressed();
            bool sprintPerformed = _sprintAction.IsPressed();
            bool interactPerformed = _interactAction.IsPressed();

            bool selectPerformed = _selectAction.WasPerformedThisFrame();
            bool completePerformed = _completeAction.WasPerformedThisFrame();

            bool anyPerformed =
                movePerformed || sprintPerformed || interactPerformed ||
                selectPerformed || completePerformed;

            if (!anyPerformed)
                return;

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            foreach (var (_, entity) in SystemAPI.Query<RefRW<InputControlledTag>>().WithEntityAccess())
            {
                if (movePerformed)
                {
                    float2 inputDirection = _moveAction.ReadValue<Vector2>();

                    // character
                    if (EntityManager.HasComponent<MoveDirectionData>(entity))
                    {
                        ecb.SetComponent(entity, new MoveDirectionData { Value = inputDirection });

                        if (!EntityManager.HasComponent<SpeedBoostData>(entity))
                        {
                            var data = sprintPerformed
                                ? new SpeedBoostData { Value = 0.1f, Max = 6f }
                                : new SpeedBoostData { Value = 0.05f, Max = 3f };

                            ecb.AddComponent(entity, data);
                        }
                    }

                    // vehicle
                    if (EntityManager.HasComponent<VehicleData>(entity))
                    {
                        if (math.abs(inputDirection.x) > 0)
                        {
                            var sign = math.sign(inputDirection.x);
                            ecb.SetComponent(entity, new VehicleSteering { DesiredSteeringAngle = 0.5f * sign });
                        }

                        if (math.abs(inputDirection.y) > 0)
                        {
                            // var vehicleSpeedData = EntityManager.GetComponentData<VehicleSpeed>(entity);
                            // var speed = math.clamp(vehicleSpeedData.DesiredSpeed + inputDirection.y, 0, 3);
                            var sign = math.sign(inputDirection.y);
                            ecb.SetComponent(entity, new VehicleSpeed { DesiredSpeed = 3 * sign });
                        }
                    }
                }

                if (interactPerformed)
                {
                    // Получаем текущий объект с управлением
                    Entity currentEntity = entity;

                    // Ищем ближайший объект с InteractableTag
                    Entity nearestInteractable = Entity.Null;
                    float minDistance = float.MaxValue;

                    foreach (var (interactablePos, interactableEntity) in
                             SystemAPI.Query<RefRO<LocalTransform>>().WithAll<InteractableTag>().WithEntityAccess())
                    {
                        if (!EntityManager.HasComponent<LocalTransform>(currentEntity))
                            continue;

                        float distance = math.distance(interactablePos.ValueRO.Position,
                            EntityManager.GetComponentData<LocalTransform>(currentEntity).Position);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestInteractable = interactableEntity;
                        }
                    }

                    if (nearestInteractable != Entity.Null)
                    {
                        ecb.RemoveComponent<InputControlledTag>(currentEntity);
                        ecb.AddComponent<InputControlledTag>(nearestInteractable);

                        Debug.Log($"Switched control from {currentEntity.Index} to {nearestInteractable.Index}");
                    }
                }

                if (selectPerformed)
                {
                    int selectedOrder = int.Parse(_selectAction.activeControl.name);
                    Debug.Log($"Handle Input-Select: {selectedOrder};");
                }

                if (completePerformed)
                {
                    Debug.Log("Handle Input-Complete; reset 'SelectedOrderNumber';");
                }
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }


        // for other features (for big count items)
        private void InputHandler_BackupOption()
        {
            if (_selectAction.WasPerformedThisFrame())
            {
                int selectedOrder = int.Parse(_selectAction.activeControl.name);

                Entities
                    .WithoutBurst()
                    .ForEach((Entity entity, ref InputTag inputActionConfig) =>
                    {
                        inputActionConfig.SelectedOrderNumber = selectedOrder;
                    }).Run();
            }
        }
    }
}