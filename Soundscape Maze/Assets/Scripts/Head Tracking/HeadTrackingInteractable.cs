using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrackingInteractable : MonoBehaviour
{
    public Material FocussedMaterial;

    public event Action onDidGainFocus;
    public event Action onDidLoseFocus;

    private Renderer objRenderer;
    private bool isFocussed = false;
    private Material defaultMaterial;
    private Material focussed;

    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            defaultMaterial = objRenderer.material;
        }

        gameObject.layer = 6;
    }

    public void Deactivate()
    {
        gameObject.layer = 7;
    }

    public void Reactivate()
    {
        gameObject.layer = 6;
    }

    public void gainFocus()
    {
        isFocussed = true;
        updateMaterial();
        onDidGainFocus?.Invoke();
    }

    public void loseFocus()
    {
        isFocussed = false;
        updateMaterial();
        onDidLoseFocus?.Invoke();
    }

    private void updateMaterial()
    {
        if (objRenderer == null)
        {
            return;
        }

        if (isFocussed)
        {
            objRenderer.material = FocussedMaterial;
        }
        else
        {
            objRenderer.material = defaultMaterial;
        }
    }
}
