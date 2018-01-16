using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AINode : MonoBehaviour
{
    public PacmanController pacman;
    public GhostController[] ghosts;
    public float distanceThresshold;

    public bool isSpecialNode = false;
    public bool isHouseNode = false;

    public bool up = true;
    public bool left = true;
    public bool down = true;
    public bool right = true;

    private void Awake()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Ghost").Select(x => x.GetComponent<GhostController>()).ToArray();
        pacman = GameObject.FindGameObjectWithTag("Pacman").GetComponent<PacmanController>();
    }

    public List<Vector3> GetAllowedDirs(GhostState state, Vector3 direction)
    {
        List<Vector3> res = new List<Vector3>();
        if (up && (Vector3.forward != -direction)) res.Add(Vector3.forward);
        if (left && (Vector3.left != -direction)) res.Add(Vector3.left);
        if (down && (Vector3.back != -direction)) res.Add(Vector3.back);
        if (right && (Vector3.right != -direction)) res.Add(Vector3.right);

        if (isHouseNode)
            if (state != GhostState.Dead && !pacman.Dead)
                res.Remove(Vector3.back);

        return res;
    }

    [ContextMenu("Setup bools")]
    public void SetupBools()
    {
        up = !Physics.Raycast(transform.position, Vector3.forward, 1.5f, 256);
        left = !Physics.Raycast(transform.position, Vector3.left, 1.5f, 256);
        down = !Physics.Raycast(transform.position, Vector3.back, 1.5f, 256);
        right = !Physics.Raycast(transform.position, Vector3.right, 1.5f, 256);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (up)
            Gizmos.DrawRay(transform.position, Vector3.forward * 0.5f);
        if (left)
            Gizmos.DrawRay(transform.position, Vector3.left * 0.5f);
        if (down)
            Gizmos.DrawRay(transform.position, Vector3.back * 0.5f);
        if (right)
            Gizmos.DrawRay(transform.position, Vector3.right * 0.5f);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (ghosts[i].LastNode != this)
            {
                if (Vector3.Distance(ghosts[i].transform.position, transform.position) < distanceThresshold)
                {
                    ghosts[i].NodeHit(this);
                }
            }
            else if (Vector3.Distance(ghosts[i].transform.position, transform.position) > distanceThresshold)
            {
                ghosts[i].LastNode = null;
            }
        }
    }
}
