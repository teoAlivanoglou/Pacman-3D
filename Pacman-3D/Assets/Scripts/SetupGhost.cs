using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetupGhost : MonoBehaviour {

    public Material bodyMat;
    public Color bodyColor;

    private void OnGUI()
    {
        ChangeColor(bodyColor);
    }
    
    public void ChangeColor (Color color)
    {
        if (bodyMat != null)
        {
            bodyMat.SetColor("_Color", color);
        }
    }
}
