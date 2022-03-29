using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AccessibilityActionHelper : MonoBehaviour
{
    [SerializeField]
    private KeyCode accessibilityKey = KeyCode.Space;

    [SerializeField]
    private UnityEvent onInvoked;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(accessibilityKey))
        {
            onInvoked?.Invoke();
        }
    }
}
