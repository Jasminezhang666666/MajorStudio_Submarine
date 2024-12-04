using System.Collections;
using UnityEngine;

public class Tentacles : MonoBehaviour
{
    [SerializeField] private GameObject normalStateObject;
    [SerializeField] private GameObject attackStateObject;
    [SerializeField] private float wPressTimeFrame = 5f; // Time frame to press "W" multiple times
    [SerializeField] private Vector2 requiredWPressRange = new Vector2(7, 10); // Random range of required "W" presses
    [SerializeField] private float coolDownDuration = 7f; // Cooldown time after reverting to normal state

    private bool inCoolDown = false;
    private int requiredWPresses;
    private int currentWPressCount;
    private Coroutine wPressCoroutine;
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
            }

            playerShip = player;
            StartWPressMiniGame();
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

    private void StartWPressMiniGame()
    {
        requiredWPresses = Random.Range((int)requiredWPressRange.x, (int)requiredWPressRange.y + 1);
        currentWPressCount = 0;

        if (wPressCoroutine != null)
        {
            StopCoroutine(wPressCoroutine);
        }
        wPressCoroutine = StartCoroutine(WPressMiniGameCoroutine());
    }

    private IEnumerator WPressMiniGameCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < wPressTimeFrame)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                currentWPressCount++;
                if (currentWPressCount >= requiredWPresses)
                {
                    ExitAttackState();
                    yield break;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restart mini-game if failed
        StartWPressMiniGame();
    }

    private void ExitAttackState()
    {
        StopCoroutine(wPressCoroutine);
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
