using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Extra
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class GetPlayerInputSystem : SystemBase
    {
        // private InputSystem_Actions ss;
        //
        // private DemoMovementActions _demoMovementActions;
        // private Entity _playerEntity;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();
            RequireForUpdate<PlayerMoveInput>();

            // _demoMovementActions = new DemoMovementActions();
        }

        protected override void OnStartRunning()
        {
            // _demoMovementActions.Enable();
            // _demoMovementActions.DemoMap.PlayerJump.performed += OnPlayerShoot;
            //
            // _playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            // var curMoveInput = _demoMovementActions.DemoMap.PlayerMovement.ReadValue<Vector2>();
            //
            // SystemAPI.SetSingleton(new PlayerMoveInput { Value = curMoveInput });
        }

        protected override void OnStopRunning()
        {
            // _demoMovementActions.DemoMap.PlayerJump.performed -= OnPlayerShoot;
            // _demoMovementActions.Disable();
            //
            // _playerEntity = Entity.Null;
        }

        private void OnPlayerShoot(InputAction.CallbackContext obj)
        {
            // if (!SystemAPI.Exists(_playerEntity)) return;
            //
            // SystemAPI.SetComponentEnabled<FireProjectileTag>(_playerEntity, true);
        }
    }
}