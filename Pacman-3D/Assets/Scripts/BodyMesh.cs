using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMesh : MonoBehaviour
{

    private MeshRenderer mr;
    private Color defaultColor;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        defaultColor = mr.material.color;
    }

    public void Disable()
    {
        mr.enabled = false;
    }

    public void Enable()
    {
        mr.enabled = true;
    }

    public void SetColor(Color color)
    {
        mr.material.color = color;
    }

    public void ResetColor()
    {
        mr.material.color = defaultColor;
    }
}
