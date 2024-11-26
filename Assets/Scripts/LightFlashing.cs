using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlashing : MonoBehaviour
{
    public Light2D light2D; 
    public float minIntensity = 0.5f; // min light
    public float maxIntensity = 1.5f; // max light
    public float flashSpeed = 5f; // flash

    private float targetIntensity; // target light
    private bool isIncreasing = true; 

    void Start()
    {
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>(); 
        }
        targetIntensity = light2D.intensity;
    }

    void Update()
    {
        if (isIncreasing)
        {
            light2D.intensity += flashSpeed * Time.deltaTime;
            if (light2D.intensity >= maxIntensity)
            {
                light2D.intensity = maxIntensity;
                isIncreasing = false;
            }
        }
        else
        {
            light2D.intensity -= flashSpeed * Time.deltaTime;
            if (light2D.intensity <= minIntensity)
            {
                light2D.intensity = minIntensity;
                isIncreasing = true;
            }
        }
    }
}
