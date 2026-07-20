using Entities.Components;
using Entities.Player;
using Time;
using UnityEngine;
using Zenject;

namespace Input
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private IInputProvider _inputProvider;
        private PlayerWeapon _playerWeapon;
        private TimeManager _timeManager;
        private ArtifactHolder _artifactHolder;

        [Inject]
        public void Construct(
            PlayerMovement playerMovement, 
            IInputProvider inputProvider, 
            PlayerWeapon playerWeapon, 
            TimeManager timeManager,
            ArtifactHolder artifactHolder) // 2. Запрашиваем контекст у Zenject
        {
            _playerMovement = playerMovement;
            _inputProvider = inputProvider;
            _playerWeapon = playerWeapon;
            _timeManager = timeManager;
            _artifactHolder = artifactHolder;
        }

        private void Update()
        {
            HandleArtifactInput();

            if (_timeManager.IsRewinding) return;

            HandleMovement();
            HandleCombat();
        }

        private void HandleArtifactInput()
        {
            if (_artifactHolder.ActiveArtifact == null) return;

            // Вызовы стали кристально чистыми
            if (_inputProvider.IsArtifactPressed())
                _artifactHolder.UseArtifactStart();
            
            if (_inputProvider.IsArtifactReleased())
                _artifactHolder.UseArtifactRelease();
        }

        private void HandleMovement()
        {
            Vector3 direction = _inputProvider.GetMovementDirection();
            _playerMovement.Move(direction);
            _playerMovement.AimTowardsMouse(_inputProvider.GetAimPosition());
        }

        private void HandleCombat()
        {
            if (_inputProvider.IsFiring())
            {
                _playerWeapon.TryFire();
            }
        }
    }
}