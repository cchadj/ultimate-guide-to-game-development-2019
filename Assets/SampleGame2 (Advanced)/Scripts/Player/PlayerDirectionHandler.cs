using UnityEngine;
using UnityEngine.InputSystem;

namespace Zenject.SpaceFighter
{
    public class PlayerDirectionHandler : ITickable
    {
        readonly Player _player;
        readonly Camera _mainCamera;

        public PlayerDirectionHandler(
            Camera mainCamera,
            Player player)
        {
            _player = player;
            _mainCamera = mainCamera;
        }

        public void Tick()
        {
            var mouseRay = _mainCamera.ScreenPointToRay(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0.0f));

            var mousePos = mouseRay.origin;
            mousePos.z = 0;

            var goalDir = mousePos - _player.Position;
            goalDir.z = 0;
            goalDir.Normalize();

            _player.Rotation = Quaternion.LookRotation(goalDir) * Quaternion.AngleAxis(90, Vector3.up);
        }
    }
}
