using System.Collections; // Required for IEnumerator
using UnityEngine;

public class TriggerUIManager : MonoBehaviour
{
    public RectTransform uiLeft;
    public RectTransform uiRight;
    public float moveSpeed = 500f;

    private Vector3 uiLeftStartPos;
    private Vector3 uiRightStartPos;

    private bool isMovingOut = false;

    private void Start()
    {
        // Save initial positions
        if (uiLeft != null) uiLeftStartPos = uiLeft.anchoredPosition;
        if (uiRight != null) uiRightStartPos = uiRight.anchoredPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Ship>() != null)
        {
            StopAllCoroutines();
            isMovingOut = false;
            StartCoroutine(MoveUIBack());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Ship>() != null && !isMovingOut)
        {
            StartCoroutine(MoveUIOut());
        }
    }


    public IEnumerator MoveUIOut()
    {
        // The same code as before
        isMovingOut = true;

        while (uiLeft.anchoredPosition.x > -Screen.width || uiRight.anchoredPosition.x < Screen.width * 2)
        {
            if (uiLeft != null)
            {
                uiLeft.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;
            }

            if (uiRight != null)
            {
                uiRight.anchoredPosition += Vector2.right * moveSpeed * Time.deltaTime;
            }

            yield return null;
        }

        isMovingOut = false;
    }
    private IEnumerator MoveUIBack()
    {
        while ((uiLeft != null && Vector3.Distance(uiLeft.anchoredPosition, uiLeftStartPos) > 0.1f) ||
               (uiRight != null && Vector3.Distance(uiRight.anchoredPosition, uiRightStartPos) > 0.1f))
        {
            if (uiLeft != null)
            {
                uiLeft.anchoredPosition = Vector3.MoveTowards(uiLeft.anchoredPosition, uiLeftStartPos, moveSpeed * Time.deltaTime);
            }

            if (uiRight != null)
            {
                uiRight.anchoredPosition = Vector3.MoveTowards(uiRight.anchoredPosition, uiRightStartPos, moveSpeed * Time.deltaTime);
            }

            yield return null;
        }

        // Ensure the UI elements are exactly at their start positions
        if (uiLeft != null) uiLeft.anchoredPosition = uiLeftStartPos;
        if (uiRight != null) uiRight.anchoredPosition = uiRightStartPos;
    }
}
