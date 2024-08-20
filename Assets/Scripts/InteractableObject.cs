using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Switch params")] 
    public float value;

    public GameObject pairedObject;

    public void Interact()
    {
        CheckConditionsAndActivate();
    }

    private void CheckConditionsAndActivate()
    {
        // Implement way to check for pairing of key from interface to value of this object.
        if (pairedObject.CompareTag("Unlocked"))
        {
            pairedObject.SetActive(!pairedObject.activeSelf);
        }
    }
}
