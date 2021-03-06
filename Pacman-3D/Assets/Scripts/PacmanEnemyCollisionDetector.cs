﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanEnemyCollisionDetector : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 15) // pacman collided with an enemy
        {
            GhostController gc = collider.gameObject.GetComponentInParent<GhostController>();
            if (gc != null)
            {
                if (gc.state == GhostState.Dead)
                    return;

                if (gc.state != GhostState.Frightened)
                {
                    GetComponentInParent<PacmanController>().Die();
                }
                else
                {
                    gc.Die();
                }
            }
        }
    }
}
