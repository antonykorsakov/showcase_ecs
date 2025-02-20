using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputModule.Controller.Controls
{
    public struct AppControl
    {
        public void HandleInput(ref SystemState state, ref EntityCommandBuffer ecb, InputAction quitAction)
        {
            if (quitAction.WasPerformedThisFrame())
                Application.Quit();
        }
    }
}