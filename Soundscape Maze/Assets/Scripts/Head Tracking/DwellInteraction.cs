using System;
using UnityEngine;
using UnityEngine.Events;

public class DwellInteraction : MonoBehaviour
{
    public Camera HeadTrackedCamera;
    public GameObject InteractableCollider;
    public bool AutomaticallyResetInteraction;

    public UnityEvent onDwellOccurred;

    private bool isFocussed = false;
    private bool isAwaitingReset = false;

    private float triggerTimer = 0.0f;
    public float triggerTimeLimit = 2.0f;
    private float resetTimer = 0.0f;
    public float resetTimeLimit = 3.0f;

    // Use this for initialization
    void Start()
    {
        var interactable = InteractableCollider.GetComponent<HeadTrackingInteractable>();

        interactable.onDidGainFocus += Interactable_onDidGainFocus;
        interactable.onDidLoseFocus += Interactable_onDidLoseFocus;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFocussed)
        {
            if (isAwaitingReset)
            {
                // The user needs to look away from the dwell target before we can reset the interaction
                return;
            }

            triggerTimer += Time.deltaTime;

            if (triggerTimer >= triggerTimeLimit)
            {
                Debug.LogWarning("Dwell occurred!");
                isAwaitingReset = true;
                onDwellOccurred.Invoke();
            }
        }
        else if ((!isAwaitingReset && triggerTimer > 0.0f) || (isAwaitingReset && AutomaticallyResetInteraction))
        {
            resetTimer += Time.deltaTime;

            if (resetTimer >= resetTimeLimit)
            {
                ResetInteraction();
            }
        }
    }

    public void ResetInteraction()
    {
        Debug.LogWarning("Dwell timers reset!");
        isAwaitingReset = false;
        triggerTimer = 0.0f;
        resetTimer = 0.0f;
    }

    private void Interactable_onDidGainFocus()
    {
        Debug.Log("Dwell object gained focus...");
        isFocussed = true;
    }

    private void Interactable_onDidLoseFocus()
    {
        Debug.Log("Dwell object lost focus...");
        isFocussed = false;
    }
}
