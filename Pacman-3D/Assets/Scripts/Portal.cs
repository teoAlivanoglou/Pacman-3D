using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public Vector3 portalOut;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent.position = portalOut;
    }
}
