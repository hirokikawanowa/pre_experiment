using UnityEngine;
using UnityEngine.InputSystem;
using VRMoL.UI; // ProgressMenuUIの名前空間

public class MenuInputController : MonoBehaviour
{
    [SerializeField] private ProgressMenuUI progressMenuUI;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Menu.performed += OnMenuButton;
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.UI.Menu.performed += OnMenuButton;
        }
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.UI.Disable();
    }

    private void OnMenuButton(InputAction.CallbackContext context)
    {
        if (progressMenuUI != null)
        {
            progressMenuUI.ToggleMenu();
        }
    }
} 