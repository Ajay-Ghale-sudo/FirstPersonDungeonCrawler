using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TorchFlicker : MonoBehaviour
{
    [Tooltip("External light to flicker, leave null if script attached to light.")]
    public new Light light;

    [Tooltip("Minimum light intensity.")]
    public float minIntensity = 0.0f;

    [Tooltip("Range modifier. Should be at minimum 10.")] 
    [Range(10, 15)]
    public float rangeModifier;

    [Tooltip("Maximum light intensity.")]
    public float maxIntensity = 1.0f;

    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern.")] 
    [Range(1, 85)]
    public int smoothing = 5;
    
    private Queue<float> smoothQueue;
    private float lastSum = 0;

    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

    private void Start()
    {
        // Initialize the smoothQueue with specified capacity.
        smoothQueue = new Queue<float>(smoothing);

        if (light == null)
        {
            // If no external light is provided, try get the Light component attached to the GameObject.
            if (TryGetComponent<Light>(out Light attachedLight))
            {
                //print("Got light.");
                light = attachedLight;
            }
            else
            {
                //print("Light not attached to GameObject.");
            }
        }
    }
    
    void Update()
    {
        if (light is null) { return; }

        // Keep the smoothQueue size within the specified smoothing range by removing the oldest value if necessary.
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        float newIntensityVal = Random.Range(minIntensity, maxIntensity);

        // Add the new value to the smoothQueue and update the lastSum
        smoothQueue.Enqueue(newIntensityVal);
        lastSum += newIntensityVal;
        
        // Calculate the average intensity and assign it to the light component.
        light.intensity = lastSum / (float)smoothQueue.Count;
        light.range = (lastSum / (float)smoothQueue.Count) * rangeModifier;
        //print(light.range);
        //light.range = light.range * lastSum;
    }
}
