using System.Collections;
using UnityEngine;

public class Tentacles : MonoBehaviour
{
    [Header("General")]

    [SerializeField] private GameObject normalStateObject;
    [SerializeField] private GameObject attackStateObject;
    [SerializeField] private float damageAmount = 5f;

    [Header("Space Bar Escape")]

    [SerializeField] private float PressTimeFrame = 5f;
    [SerializeField] private Vector2 requiredPressRange = new Vector2(7, 10); // Random range of required presses
    [SerializeField] private float coolDownDuration = 7f; // Cooldown time after reverting to normal state
    [SerializeField] private float shakeIntensityDuringMiniGame = 0.3f; // Reduced intensity during mini-game
    [SerializeField] private float shakeDurationDuringMiniGame = 0.1f; // Duration of each shake while in mini-game

    private bool inCoolDown = false;
    private int requiredPresses;
    private int currentPressCount;
    private Coroutine PressCoroutine;
    private Coroutine shakeCoroutine;
    private GameObject playerShip;



    private void Start()
    {
        SetNormalState();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !inCoolDown)
        {
            EnterAttackState(collision.gameObject);
        }
    }

    private void EnterAttackState(GameObject player)
    {
        if (attackStateObject != null && normalStateObject != null)
        {
            attackStateObject.SetActive(true);
            normalStateObject.SetActive(false);

            player.transform.position = attackStateObject.transform.position;
            Ship playerShipScript = player.GetComponent<Ship>();
            if (playerShipScript != null)
            {
                playerShipScript.CanMove = false; // Disable movement
                playerShipScript.TakeDamage(damageAmount);
            }

            playerShip = player;
            StartPressMiniGame();
        }
    }


    private void SetNormalState()
    {
        if (attackStateObject != null && normalStateObject != null)
        {
            attackStateObject.SetActive(false);
            normalStateObject.SetActive(true);
        }

        if (playerShip != null)
        {
            Ship playerShipScript = playerShip.GetComponent<Ship>();
            if (playerShipScript != null)
            {
                playerShipScript.CanMove = true; // Re-enable movement
            }
        }

        playerShip = null;
    }

    private void StartPressMiniGame()
    {
        requiredPresses = Random.Range((int)requiredPressRange.x, (int)requiredPressRange.y + 1);
        currentPressCount = 0;

        if (PressCoroutine != null)
        {
            StopCoroutine(PressCoroutine);
        }

        // Start continuous shaking with reduced intensity
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(StartContinuousShake());

        PressCoroutine = StartCoroutine(PressMiniGameCoroutine());
    }

    private IEnumerator StartContinuousShake()
    {
        while (true)
        {
            CameraShake.Instance.ShakeCamera(shakeIntensityDuringMiniGame, shakeDurationDuringMiniGame);
            yield return new WaitForSeconds(shakeDurationDuringMiniGame); // Delay between shakes
        }
    }

    private IEnumerator PressMiniGameCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < PressTimeFrame)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentPressCount++;
                if (currentPressCount >= requiredPresses)
                {
                    ExitAttackState();
                    yield break;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restart mini-game if failed, and reduce health
        Ship playerShipScript = playerShip.GetComponent<Ship>();
        if (playerShipScript != null)
        {
            playerShipScript.TakeDamage(damageAmount); // Reduce health if failed
        }

        StartPressMiniGame(); // Restart the mini-game
    }

    private void ExitAttackState()
    {
        StopCoroutine(PressCoroutine);

        // Stop shaking when exiting the attack state
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        CameraShake.Instance.ShakeCamera(0.3f, 0.5f);
        SetNormalState();
        StartCoroutine(CoolDownCoroutine());
    }

    private IEnumerator CoolDownCoroutine()
    {
        inCoolDown = true;
        yield return new WaitForSeconds(coolDownDuration);
        inCoolDown = false;
    }
}
