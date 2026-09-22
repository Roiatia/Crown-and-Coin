using UnityEngine;
using UnityEngine.InputSystem;

public class StrategyCameraController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference zoomAction;
    [SerializeField] private InputActionReference dragAction;
    [SerializeField] private InputActionReference pointerPositionAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float dragSpeed = 0.01f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.02f;
    [SerializeField] private float minZoom = 4f;
    [SerializeField] private float maxZoom = 14f;

    private Camera cameraComponent;
    private Vector2 lastPointerPosition;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        zoomAction.action.Enable();
        dragAction.action.Enable();
        pointerPositionAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        zoomAction.action.Disable();
        dragAction.action.Disable();
        pointerPositionAction.action.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleDrag();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);

        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomInput = zoomAction.action.ReadValue<float>();

        if (Mathf.Abs(zoomInput) < 0.01f)
            return;

        cameraComponent.orthographicSize -= zoomInput * zoomSpeed;
        cameraComponent.orthographicSize = Mathf.Clamp(cameraComponent.orthographicSize, minZoom, maxZoom);
    }

    private void HandleDrag()
    {
        Vector2 pointerPosition = pointerPositionAction.action.ReadValue<Vector2>();

        if (dragAction.action.WasPressedThisFrame())
        {
            lastPointerPosition = pointerPosition;
        }

        if (dragAction.action.IsPressed())
        {
            Vector2 delta = pointerPosition - lastPointerPosition;
            Vector3 movement = new Vector3(-delta.x, -delta.y, 0f);

            transform.position += movement * dragSpeed;
            lastPointerPosition = pointerPosition;
        }
    }
}