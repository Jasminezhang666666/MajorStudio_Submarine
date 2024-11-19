using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpotLightController : MonoBehaviour
{
    public Light2D spotLight;
    private float rotationStep = 45f; 
    private float currentAngle = 0f;

    void Update()
    {
        // switch on and off
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            spotLight.enabled = !spotLight.enabled;
        }

        // direction
        
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // Add 45 degree 
                currentAngle += rotationStep;
                if (currentAngle >= 360f) currentAngle -= 360f; 
                UpdateLightRotation();
            }

            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                // decrease 45 degree
                currentAngle -= rotationStep;
                if (currentAngle < 0f) currentAngle += 360f; 
                UpdateLightRotation();
            }
        
    }
    private void UpdateLightRotation()
    {
        spotLight.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }
}
