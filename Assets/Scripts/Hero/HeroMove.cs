using CameraLogic;
using Infrastructure;
using Services.Input;
using UnityEngine;

namespace Hero
{
    public class HeroMove : MonoBehaviour
    {
        [SerializeField] 
        private CharacterController CharacterController;
        [SerializeField]
        private float MovementSpeed;

        private IInputService _inputService;
        public Camera _camera;

        private void Awake()
        {
            _inputService = Game.InputService;
        }

        private void Start()
        {
            _camera = Camera.main;
        }


        private void Update()
        {
            Vector3 movementVector = Vector3.zero;
            if (_inputService.Axis.sqrMagnitude > Constants.Epsilon)
            {
                movementVector = _camera.transform.TransformDirection(_inputService.Axis);
                movementVector.y = 0;
                movementVector.Normalize();

                transform.forward = movementVector;
            }

            movementVector += Physics.gravity;

            CharacterController.Move(movementVector * (MovementSpeed * Time.deltaTime));
        }
    }
}