using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{

    public Transform target;
    public PacmanController pacman;
    public LayerMask obstacles;
    public GhostState state;

    public float force = 1f;

    public bool ai = true;
    public bool debug = false;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (ai)
            nextDirection = Vector3.left;
    }

    private Vector3 direction;// = Vector3.forward;
    private Vector3 nextDirection;// = Vector3.forward;

    private void Update()
    {
        if (!pacman.Dead)
        {
            if (!ai)
            {
                HandleInput();
            }
            if (CanTurn(nextDirection))
                direction = nextDirection;

            RotateModel();
        }
        else
        {
            rb.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        //if (!pacman.Dead)
        if (CanMoveForwardOrBackward(direction))
            rb.velocity = direction * force;
    }

    public AINode LastNode { get; protected set; }

    public void NodeHit(AINode node)
    {
        LastNode = node;
        if (node != null)
        {
            Vector3[] allowedDirs = GetAllowedDirections();

            int minIndex = -1;
            float minDistance = float.PositiveInfinity;

            for (int i = 0; i < allowedDirs.Length; i++)
            {
                float currentDistance = Vector3.Distance(
                    transform.position + allowedDirs[i], target.position);

                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    minIndex = i;
                }
            }

            nextDirection = allowedDirs[minIndex];
            transform.position = node.transform.position;
        }
    }

    private Vector3[] GetAllowedDirections()
    {
        List<Vector3> list = new List<Vector3>();

        //Debug.DrawRay(transform.position, Vector3.forward * rayCastDistance, Color.red, 5f);
        //Debug.DrawRay(transform.position, Vector3.left * rayCastDistance, Color.red, 5f);
        //Debug.DrawRay(transform.position, Vector3.back * rayCastDistance, Color.red, 5f);
        //Debug.DrawRay(transform.position, Vector3.right * rayCastDistance, Color.red, 5f);

        if (direction != Vector3.back && !Physics.Raycast(transform.position, Vector3.forward, rayCastDistance, obstacles))// && !LastNode.isSpecialNode)
            list.Add(Vector3.forward);
        if (direction != Vector3.right && !Physics.Raycast(transform.position, Vector3.left, rayCastDistance, obstacles))
            list.Add(Vector3.left);
        if (direction != Vector3.forward && !Physics.Raycast(transform.position, Vector3.back, rayCastDistance, obstacles))
            list.Add(Vector3.back);
        if (direction != Vector3.left && !Physics.Raycast(transform.position, Vector3.right, rayCastDistance, obstacles))
            list.Add(Vector3.right);

        return list.ToArray();
    }

    private void RotateModel()
    {
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }

    public float rayCastDistance = 1.5f;
    public float rayCastOrigin = 1f;

    private bool CanMoveForwardOrBackward(Vector3 checkDir)
    {
        Vector3 left = transform.position - transform.right * rayCastOrigin;
        Vector3 right = transform.position + transform.right * rayCastOrigin;

        if (!Physics.Raycast(left, checkDir, rayCastDistance, obstacles) &&
            !Physics.Raycast(right, checkDir, rayCastDistance, obstacles))
        {

            return true;
        }
        else return false;
    }

    private bool CanRegisterNextDir(Vector3 checkDir)
    {
        if (direction == checkDir || -direction == checkDir)
        {
            return false;
        }
        else
        {
            Vector3 forward = transform.position + transform.forward * rayCastOrigin;
            return !Physics.Raycast(forward, checkDir, rayCastDistance, obstacles) &&
                !Physics.Raycast(transform.position, checkDir, rayCastDistance, obstacles);
        }
    }

    private bool CanTurn(Vector3 checkDir)
    {
        if (direction == checkDir || -direction == checkDir)
        {
            return CanMoveForwardOrBackward(checkDir);
        }
        else
        {
            Vector3 forward = transform.position + transform.forward * rayCastOrigin;
            Vector3 middle = transform.position;
            Vector3 backward = transform.position - transform.forward * rayCastOrigin;

            return !Physics.Raycast(forward, checkDir, rayCastDistance, obstacles) &&
                !Physics.Raycast(middle, checkDir, rayCastDistance, obstacles) &&
                !Physics.Raycast(backward, checkDir, rayCastDistance, obstacles);
        }
    }


    private void HandleInput()
    {
        // temporary direction, to check input validity
        Vector3 wantedNextDir = direction;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            wantedNextDir = Vector3.forward;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            wantedNextDir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            wantedNextDir = Vector3.back;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            wantedNextDir = Vector3.right;
        else return;

        // if at least one of our check points misses, next direction should be set
        if (CanRegisterNextDir(wantedNextDir))
            nextDirection = wantedNextDir;
    }

    void Dbg(string s)
    {
        if (debug)
            Debug.Log(s);
    }
}

public enum GhostState
{
    Scatter,
    Chase,
    Frightened
}