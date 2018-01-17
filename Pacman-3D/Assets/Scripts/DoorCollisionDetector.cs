using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollisionDetector : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        string name = collider.transform.parent.name;
        print(name);
    }
}
