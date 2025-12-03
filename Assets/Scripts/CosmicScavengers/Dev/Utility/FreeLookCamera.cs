using UnityEngine;

namespace CosmicScavengers.Dev.Utility
{
    /// <summary>
    /// Provides a simple, free-moving "spectator mode" camera for inspecting environments in Play Mode.
    /// Controls:
    /// - WASD: Horizontal movement
    /// - QE: Vertical movement (Up/Down)
    /// - Right Mouse Button + Mouse Movement: Look around
    /// - Scroll Wheel: Adjust movement speed
    /// </summary>
    public class FreeLookCamera : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("The base speed of camera movement.")]
        public float baseMoveSpeed = 10f;

        [Tooltip("The factor by which speed is multiplied when holding Shift.")]
        public float fastMoveFactor = 2.0f;

        [Tooltip("The current speed of camera movement.")]
        private float currentMoveSpeed;

        [Header("Look Settings")]
        [Tooltip("Sensitivity for mouse movement.")]
        public float lookSensitivity = 5f;

        private float rotationX = 0f;
        private float rotationY = 0f;

        private void Start()
        {
            currentMoveSpeed = baseMoveSpeed;
            // Optionally lock the cursor in the center of the screen when playing
            Cursor.lockState = CursorLockMode.None; // Start unlocked, lock when needed
        }

        private void Update()
        {
            HandleMovementInput();

            // Only allow looking around when the Right Mouse Button is held down
            if (Input.GetMouseButton(1)) // Right Mouse Button
            {
                // Lock the cursor for better camera control
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                HandleLookInput();
            }
            else
            {
                // Unlock the cursor when the right button is released
                if (Cursor.lockState != CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        private void HandleMovementInput()
        {
            // 1. Adjust Speed

            // Allow boosting speed by holding Shift
            float speed = currentMoveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speed *= fastMoveFactor;
            }

            // Allow adjusting the base speed with the scroll wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                currentMoveSpeed = Mathf.Max(1f, currentMoveSpeed + scroll * 5f); // 1f is min speed
                speed = currentMoveSpeed; // Update current speed immediately
            }

            // 2. Get Directional Input (WASD, QE)

            Vector3 moveDirection = Vector3.zero;

            // Horizontal (WASD)
            moveDirection.x = Input.GetAxis("Horizontal"); // A/D keys
            moveDirection.z = Input.GetAxis("Vertical");   // W/S keys

            // Vertical (QE)
            if (Input.GetKey(KeyCode.Q))
            {
                moveDirection.y = -1f; // Down
            }
            else if (Input.GetKey(KeyCode.E))
            {
                moveDirection.y = 1f; // Up
            }

            // Normalize the vector to prevent faster diagonal movement
            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            // 3. Apply Movement

            // transform.forward/right/up ensures movement is relative to the camera's current rotation
            Vector3 movement = transform.TransformDirection(moveDirection) * speed * Time.deltaTime;
            transform.position += movement;
        }

        private void HandleLookInput()
        {
            // Get mouse delta movement
            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

            // Yaw (Y-axis rotation, looking left/right)
            rotationX += mouseX;

            // Pitch (X-axis rotation, looking up/down)
            rotationY -= mouseY;
            // Clamp the vertical rotation to prevent flipping (e.g., -90 to 90 degrees)
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            // Apply rotations
            // Horizontal rotation (Yaw) is applied to the Parent (or self if we want local rotation)
            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0);
        }
    }
}