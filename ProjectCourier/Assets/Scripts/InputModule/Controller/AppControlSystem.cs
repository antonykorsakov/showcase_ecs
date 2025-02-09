using InputModule.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputModule.Controller
{
    public partial class AppControlSystem : SystemBase
    {
        private InputAction _quitAction;
        private bool _prepared;

        protected override void OnCreate()
        {
            var inputTagQuery = GetEntityQuery(ComponentType.ReadOnly<InputTag>());
            RequireForUpdate(inputTagQuery);
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
                HandleInput();
        }

        private void Prepare()
        {
            var entities = EntityManager.CreateEntityQuery(typeof(InputActionAsset)).ToEntityArray(Allocator.Temp);
            if (entities.Length == 0)
                return;

            var inputActionAsset = EntityManager.GetComponentObject<InputActionAsset>(entities[0]);

            _quitAction = inputActionAsset.FindAction("Quit");
            _quitAction.Enable();

            entities.Dispose();

            Debug.Log("AppControlSystem prepared.");
            _prepared = true;
        }

        private void HandleInput()
        {
            if (_quitAction.WasPerformedThisFrame())
            {
                Debug.Log("Quit action performed. Exiting...");
                Application.Quit();
            }
        }
    }
}