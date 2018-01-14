using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    public int score = 10;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with " + other.name);
        PacmanController pacman = other.GetComponentInParent<PacmanController>();

        if (pacman != null)
        {
            GameManager.Instance.pelletsLeft--;
            pacman.Score(score);
            Destroy(gameObject);
        }
    }
}
