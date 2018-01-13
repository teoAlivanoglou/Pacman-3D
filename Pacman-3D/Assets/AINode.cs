using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AINode : MonoBehaviour {

    public GhostController[] ghosts;
    public bool isSpecialNode = false;
    public float distanceThresshold;

    private void Awake()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Ghost").Select(x=>x.GetComponent<GhostController>()).ToArray();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (Vector3.Distance(ghosts[i].transform.position, transform.position) < distanceThresshold)
                if (ghosts[i].LastNode != this)
                    ghosts[i].NodeHit(this);
        }
    }
}
