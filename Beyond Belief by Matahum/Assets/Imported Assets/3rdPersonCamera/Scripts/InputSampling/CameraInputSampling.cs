#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThirdPersonCamera
{
    [RequireComponent(typeof(FreeForm))]
    public class CameraInputSampling : MonoBehaviour
    {
        private TPC_Input.CameraMovementActions cameraActions;
        private bool waitForRelease;

        private TPC_Input inputMap;

        public Vector2 MouseSensitivity = new Vector2(2, 1.5f);
        public Vector2 ControllerSensitivity = new Vector2(2, 1.5f);

        public bool MouseInvertY;
        public bool ControllerInvertY;

        [SerializeField]
        private CameraInput cameraInput;

        private FreeForm freeFormComponent;
        private LockOnTarget lockOnComponent;
        private OverTheShoulder shoulderComponent;

        private bool sampleShoulderInput;
        private bool sampleLockOnInput;
        private bool sampleInterface;

        private EventSystem currentEventSystem;

        private void OnEnable()
        {
            cameraInput = default;

            cameraInput.InputFreeForm.ControllerSensitivity = ControllerSensitivity;
            cameraInput.InputFreeForm.MouseSensitivity = MouseSensitivity;
            cameraInput.InputFreeForm.MouseInvertY = MouseInvertY;
            cameraInput.InputFreeForm.ControllerInvertY = ControllerInvertY;

            inputMap = new TPC_Input();
            inputMap.Enable();
            inputMap.CameraMovement.Enable();

            cameraActions = inputMap.CameraMovement;

            freeFormComponent = GetComponent<FreeForm>();
            lockOnComponent = GetComponent<LockOnTarget>();
            shoulderComponent = GetComponent<OverTheShoulder>();
            currentEventSystem = EventSystem.current;

            sampleLockOnInput = lockOnComponent != null;
            sampleShoulderInput = shoulderComponent != null;
            sampleInterface = currentEventSystem != null;
        }

        private void OnDisable()
        {
            inputMap.Disable();
            inputMap.CameraMovement.Disable();
        }

        private void Update()
        {
            bool interfaceHovered = false;
            ref var freeLookInput = ref cameraInput.InputFreeForm;

            var tmpInputFreeLook = cameraActions.EnableFreeLook.IsPressed();

            if (sampleInterface)
            {
                interfaceHovered = currentEventSystem.IsPointerOverGameObject();

                if (interfaceHovered && !waitForRelease)
                {
                    if (tmpInputFreeLook)
                        waitForRelease = true;
                }

                if (waitForRelease)
                {
                    if (!tmpInputFreeLook)
                        waitForRelease = false;
                }
            }

            //freeLookInput.InputFreeLook = (tmpInputFreeLook && !interfaceHovered && !waitForRelease);
            freeLookInput.InputFreeLook = !Cursor.visible;

            freeLookInput.ForceTargetDirection = cameraActions.ForceTargetDirection.IsPressed();
            // freeLookInput.ZoomInput = cameraActions.Zoom.ReadValue<float>();
            //USE THIS LINE WHEN READY: freeLookInput.ZoomInput = UITriggerContainer.IsPromptUIActive ? 0f : cameraActions.Zoom.ReadValue<float>();

            freeLookInput.CameraInput = cameraActions.CameraAxis.ReadValue<Vector2>() * freeLookInput.MouseSensitivity * (freeLookInput.MouseInvertY ? new Vector2(1, -1) : new Vector2(1, 1));

            Vector2 controllerInput = cameraActions.ControllerCameraAxis.ReadValue<Vector2>();
            freeLookInput.CameraInput += controllerInput * freeLookInput.ControllerSensitivity * (freeLookInput.ControllerInvertY ? new Vector2(1, -1) : new Vector2(1, 1));

            // Always enable freelook when gamepad has input
            if (controllerInput.sqrMagnitude > 0)
            {
                freeLookInput.InputFreeLook = true;
            }

            freeFormComponent.UpdateInput(freeLookInput);

            if (sampleShoulderInput)
            {
                ref var shoulderInput = ref cameraInput.InputShoulder;

                shoulderInput.Aiming = cameraActions.Aim.IsPressed();

                if (cameraActions.SwitchAimSide.WasPressedThisFrame())
                {
                    shoulderInput.Left = !shoulderInput.Left;
                }

                shoulderComponent.UpdateInput(shoulderInput);
            }

            if (sampleLockOnInput)
            {
                ref var lockOnInput = ref cameraInput.InputLockOn;

                if (cameraActions.LockOn.WasPressedThisFrame())
                {
                    lockOnInput.CycleIndex = 0;
                    lockOnInput.LockOnTarget = true;
                }
                else
                {
                    lockOnInput.LockOnTarget = false;
                }

                if (cameraActions.NextLockOnTarget.WasPressedThisFrame())
                {
                    lockOnInput.CycleIndex++;
                    lockOnInput.ForceCycle = true;
                }
                else if (cameraActions.PreviousLockOnTarget.WasPressedThisFrame())
                {
                    lockOnInput.CycleIndex--;
                    lockOnInput.ForceCycle = true;
                }
                else
                {
                    lockOnInput.ForceCycle = false;
                }

                lockOnComponent.UpdateInput(lockOnInput);
            }
        }

        public ref CameraInput GetInput()
        {
            return ref cameraInput;
        }

        // Keeps the cursor centered when locked
        public void CenterCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None; // Reset position
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
#endif
