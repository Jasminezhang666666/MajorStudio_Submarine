using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static Vector3 currentGasStationPosition;
    private static float currentPlayerDamage;

    // save the gas station position and damage
    public static void SaveGasStation(Vector3 position, float damageAmount)
    {
        currentGasStationPosition = position;
        currentPlayerDamage = damageAmount;
    }

    public static Vector3 LoadGasStationPosition()
    {
        return currentGasStationPosition != Vector3.zero ? currentGasStationPosition : Vector3.zero;
    }

    public static float LoadPlayerDamage()
    {
        return currentPlayerDamage != 0f ? currentPlayerDamage : 30f; // Default to 100 if no data
    }

    public static void ClearData()
    {
        currentGasStationPosition = Vector3.zero;
        currentPlayerDamage = 30f;
        Debug.Log("Game state cleared!");
    }
}
