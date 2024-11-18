using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Store the gas station position and player damage temporarily during the session
    private static Vector3 currentGasStationPosition;
    private static float currentPlayerDamage;

    // Call this to save the gas station position and damage for the current session
    public static void SaveGasStation(Vector3 position, float damageAmount)
    {
        currentGasStationPosition = position;
        currentPlayerDamage = damageAmount;
        Debug.Log("Game state saved!");
    }

    // Load the gas station position for the current session (will reset after scene reload)
    public static Vector3 LoadGasStationPosition()
    {
        return currentGasStationPosition != Vector3.zero ? currentGasStationPosition : Vector3.zero;
    }

    // Load the player's damage for the current session (will reset after scene reload)
    public static float LoadPlayerDamage()
    {
        return currentPlayerDamage != 0f ? currentPlayerDamage : 100f; // Default to 100 if no data
    }

    // Optionally, call this function to reset the session data (if you want to manually clear)
    public static void ClearData()
    {
        currentGasStationPosition = Vector3.zero;
        currentPlayerDamage = 100f;
        Debug.Log("Game state cleared!");
    }
}
