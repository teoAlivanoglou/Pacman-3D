using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    
    public bool isPowerUp = false;

    private GhostController[] allGhosts;

    private void Awake()
    {
        if (isPowerUp)
        {
            allGhosts = new GhostController[]
            {
                GameManager.Instance.blinky,
                GameManager.Instance.pinky,
                GameManager.Instance.inky,
                GameManager.Instance.clyde
            };
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with " + other.name);
        PacmanController pacman = other.GetComponentInParent<PacmanController>();

        if (pacman != null)
        {
            GameManager.Instance.pelletsLeft--;
            gameObject.SetActive(false);
            if (isPowerUp)
            {
                for (int i = 0; i < allGhosts.Length; i++)
                {
                    if (allGhosts[i].state != GhostState.Frightened && allGhosts[i].state != GhostState.Dead)
                        allGhosts[i].SetFrightened(6);
                }
            }
        }
    }
}
