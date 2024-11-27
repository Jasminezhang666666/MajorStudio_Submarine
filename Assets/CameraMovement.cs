using UnityEngine;
using System.Linq;

public class CameraMovement : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Buffer Area")]
    public float bufferWidth = 2f; 
    public float bufferHeight = 1f; 

    [Header("Smooth Damping")]
    public float smoothTime = 0.2f; // Smooth transition time

    private Vector3 velocity = Vector3.zero;
    private Bounds combinedBounds;

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

        // Find and combine all camera wall colliders
        UpdateCombinedBounds();
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
        bool outHorizontal = player.position.x < camMin.x || player.position.x > camMax.x;
        bool outVertical = player.position.y < camMin.y || player.position.y > camMax.y;

        // Adjust camera position if player is outside the buffer
        Vector3 targetPosition = cameraPos;

        if (outHorizontal)
            targetPosition.x = Mathf.Lerp(cameraPos.x, player.position.x, 0.5f);

        if (outVertical)
            targetPosition.y = Mathf.Lerp(cameraPos.y, player.position.y, 0.5f);

        // Smoothly move the camera
        targetPosition.z = cameraPos.z;

        // Clamp position within combined bounds
        targetPosition = ClampCameraPosition(targetPosition);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private Vector3 ClampCameraPosition(Vector3 targetPosition)
    {
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        if (combinedBounds.size != Vector3.zero)
        {
            // Clamp position to keep camera within combined bounds
            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                combinedBounds.min.x + camHalfWidth,
                combinedBounds.max.x - camHalfWidth
            );

            targetPosition.y = Mathf.Clamp(
                targetPosition.y,
                combinedBounds.min.y + camHalfHeight,
                combinedBounds.max.y - camHalfHeight
            );
        }

        return targetPosition;
    }

    private void UpdateCombinedBounds()
    {
        Collider2D[] cameraWalls = GameObject.FindGameObjectsWithTag("Camera_wall")
            .Select(go => go.GetComponent<Collider2D>())
            .Where(c => c != null)
            .ToArray();

        if (cameraWalls.Length > 0)
        {
            combinedBounds = cameraWalls[0].bounds;

            foreach (var collider in cameraWalls)
            {
                combinedBounds.Encapsulate(collider.bounds);
            }
        }
        else
        {
            Debug.LogWarning("No colliders found with the tag 'Camera_wall'. The camera will move freely.");
            combinedBounds = new Bounds(Vector3.zero, Vector3.positiveInfinity); // No restriction
        }
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

        // Visualize combined bounds
        if (combinedBounds.size != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(combinedBounds.center, combinedBounds.size);
        }
    }
}
