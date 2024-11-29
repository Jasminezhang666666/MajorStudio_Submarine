using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Buffer Area")]
    public float bufferWidth = 2f;
    public float bufferHeight = 1f;

    [Header("Smooth Damping")]
    public float smoothTime = 0.2f; // Smooth transition time
    public float movementThreshold = 0.1f; // Minimum movement threshold

    [Header("Camera Bounds")]
    public float minX = -10f; // Minimum X boundary
    public float maxX = 10f;  // Maximum X boundary
    public float minY = -5f;  // Minimum Y boundary
    public float maxY = 5f;   // Maximum Y boundary

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic)
        {
            Debug.LogError("CameraMovement requires an Orthographic Camera component.");
            enabled = false;
            return;
        }
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        // Calculate camera bounds based on buffer area
        Vector3 cameraPos = transform.position;
        Vector2 camMin = new Vector2(cameraPos.x - bufferWidth, cameraPos.y - bufferHeight);
        Vector2 camMax = new Vector2(cameraPos.x + bufferWidth, cameraPos.y + bufferHeight);

        // Determine if the player is outside the buffer area
        Vector3 targetPosition = cameraPos;

        if (player.position.x < camMin.x || player.position.x > camMax.x)
            targetPosition.x = player.position.x;

        if (player.position.y < camMin.y || player.position.y > camMax.y)
            targetPosition.y = player.position.y;

        // Clamp target position within defined bounds
        targetPosition = ClampCameraPosition(targetPosition);

        // Avoid small, unnecessary camera position updates
        if (Vector3.Distance(transform.position, targetPosition) > movementThreshold)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    private Vector3 ClampCameraPosition(Vector3 targetPosition)
    {
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        // Clamp position within numerical bounds
        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            minX + camHalfWidth,
            maxX - camHalfWidth
        );

        targetPosition.y = Mathf.Clamp(
            targetPosition.y,
            minY + camHalfHeight,
            maxY - camHalfHeight
        );

        return targetPosition;
    }

    private void OnDrawGizmos()
    {
        // Visualize buffer area
        if (player != null)
        {
            Gizmos.color = Color.green;
            Vector3 bufferSize = new Vector3(bufferWidth * 2, bufferHeight * 2, 0);
            Gizmos.DrawWireCube(transform.position, bufferSize);
        }

        // Visualize camera bounds
        Gizmos.color = Color.red;
        Vector3 boundsSize = new Vector3(maxX - minX, maxY - minY, 0);
        Vector3 boundsCenter = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Gizmos.DrawWireCube(boundsCenter, boundsSize);
    }
}
