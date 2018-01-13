using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public PacmanController pacman;

    public GhostController blinky;
    public GhostController pinky;
    public GhostController inky;

    public Transform blinkyTarget;
    public Transform pinkyTarget;
    public Transform inkyTarget;
    

    void Update ()
    {
        BlinkyTargetUpdate();
        PinkyTargetUpdate();
        InkyTargetUpdate();
    }

    private void BlinkyTargetUpdate()
    {
        if (blinky.state == GhostState.Chase)
            blinkyTarget.position = pacman.transform.position;
        else
            blinkyTarget.position = new Vector3(11.5f, 0, 18f);
    }

    private void PinkyTargetUpdate()
    {
        if (pinky.state == GhostState.Chase)
            pinkyTarget.position = pacman.transform.position + pacman.transform.forward * 4f;
        else
            pinkyTarget.position = new Vector3(-11.5f, 0, 18f);
    }

    private void InkyTargetUpdate()
    {
        if (inky.state == GhostState.Chase)
        {
            Vector3 pivot = pacman.transform.position + pacman.transform.forward * 2f;

            inkyTarget.position = 2 * pivot - blinky.transform.position;
        }
        else
            inkyTarget.position = new Vector3(14f, 0, -18f);
    }
}
