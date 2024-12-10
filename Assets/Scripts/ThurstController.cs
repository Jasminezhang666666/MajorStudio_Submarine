using UnityEngine;

public class GroundTentacle : MonoBehaviour
{
    [Header("Tentacle Settings")]
    [SerializeField] public float detectionDistance = 10f; 
    [SerializeField] public float thrustSpeed = 10f; 
    [SerializeField] public float thrustDistance = 5f; 
    [SerializeField] public float cooldownTime = 3f; 
    [SerializeField] public bool thrustUpwards = false; 

    private Vector3 initialPosition; 
    private bool isThrusting = false; 
    private float cooldownTimer = 0f; 

    private void Start()
    {
        
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
       
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        //distance between player and tentacle
        Vector2 rayDirection = thrustUpwards ? Vector2.up : Vector2.down; 
        Vector2 rayCastStart = new Vector2(transform.position.x, transform.position.y + rayDirection.y * GetComponent<BoxCollider2D>().size.y);
        RaycastHit2D hit = Physics2D.Raycast(rayCastStart, rayDirection, detectionDistance);

        //Debug.DrawRay(rayCastStart, rayDirection * detectionDistance, Color.white);
        Debug.DrawLine(transform.position, rayCastStart, Color.red);

        //Debug.Log("hit collider" + hit.collider.gameObject.name);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log("stretch out");
            //thrust if in distance
            if (!isThrusting)
            {
                
                StartCoroutine(Thrust());
                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.ShakeCamera(1.5f, 0.5f);
                }


            }
        }
    }

    private System.Collections.IEnumerator Thrust()
    {
        isThrusting = true;

        // ¼ÆËãÍ»´ÌÄ¿±êÎ»ÖÃ
        Vector3 targetPosition = initialPosition + (thrustUpwards ? Vector3.up : Vector3.down) * thrustDistance;

        // Í»´Ì½×¶Î
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * thrustSpeed;
            yield return null;
        }

        // ·µ»Ø½×¶Î
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, elapsedTime);
            elapsedTime += Time.deltaTime * thrustSpeed;
            yield return null;
        }

        // ÀäÈ´Âß¼­
        isThrusting = false;
        cooldownTimer = cooldownTime;
    }
}
