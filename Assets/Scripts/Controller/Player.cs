using platform.Movement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using platform.Combat;
namespace platform.Controller
{
    public class Player : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] float walkSpeedFract = 0.5f;
        //[SerializeField, Range(0, 1)] float sprintSpeedFract = 1f;
        [SerializeField] UnityEvent onPause;

        PlayerInput playerInput;
        Mover mover;
        GunBuster gunBuster;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            mover = GetComponent<Mover>();
            gunBuster = GetComponent<GunBuster>();
        }


        void Update()
        {
            HandlePause();

            if (Time.timeScale == 0f) 
            {
                return; // Bloquea los demás controles si el juego está pausado
            }

            HandleMovement();
            HandleJump();
            HandleFire();
            HandleDash();

            
            

            //FUTURO SPRINT

            //InputAction fireAction = playerInput.actions["Fire"];
            //if (fireAction.WasPerformedThisFrame())
            //{
            //    print("FIRE!!!!");
            //}

            //Movimiento

        }

        private void HandleMovement()
        {
            Vector2 inputValue = playerInput.actions["Locomotion"].ReadValue<Vector2>();
            //InputAction sprintAction = playerInput.actions["Sprint"];

            mover.MoveTo(inputValue.x, walkSpeedFract);
            /*
            if (sprintAction.IsPressed())
            {
                mover.MoveTo(inputValue.x, sprintSpeedFract);
            }
            else
            {
                mover.MoveTo(inputValue.x, walkSpeedFract);
            }
            */
        }

        private void HandleJump()
        {
            InputAction jumpAction = playerInput.actions["Jump"];

            if (jumpAction.WasPressedThisFrame())
            {
                if (mover.IsGrounded())
                {
                    mover.Jump();
                }
                else if (mover.IsTouchingWall())
                {
                    mover.WallJump();
                }
            }
            else if (jumpAction.WasReleasedThisFrame())
            {
                mover.CancelJump();
            }
            
            mover.SetJumpHeld(jumpAction.IsPressed());
        }

        void HandleDash()
        {
            InputAction dashAction = playerInput.actions["Dash"];

            if (dashAction.WasPressedThisFrame())
            {
                Vector2 inputValue = playerInput.actions["Locomotion"].ReadValue<Vector2>();

                float directionX = inputValue.x != 0 ? Mathf.Sign(inputValue.x) : transform.localScale.x;
                mover.Dash(directionX);
            }

        }
        void HandleFire()
        {
            InputAction fireAction = playerInput.actions["Fire"];
            if (fireAction.WasPerformedThisFrame())
            {
                print("FIRE!!!!");
                gunBuster.Shoot(transform.localScale.x);
            }
        }

        void HandlePause()
        {
            InputAction pauseAction = playerInput.actions["Pause"];
            if (pauseAction.WasPressedThisFrame())
            {
                onPause?.Invoke();
            }
        }
    }
}
