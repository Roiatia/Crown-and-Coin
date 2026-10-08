using UnityEngine;

public class WorldToUIButtonFollower : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform worldTarget;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 40f);

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (mainCamera == null || worldTarget == null)
            return;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldTarget.position);

        rectTransform.position = new Vector3(
            screenPosition.x + screenOffset.x,
            screenPosition.y + screenOffset.y,
            screenPosition.z
        );
    }
}
