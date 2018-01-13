using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public LayerMask obstacles;

    public float force = 1f;
    public bool debug = false;

    public bool Dead { get; protected set; }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private Vector3 direction;// = Vector3.forward;
    private Vector3 nextDirection;// = Vector3.forward;

    private void Update()
    {
        if (!Dead)
        {
            HandleInput();

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
        if (CanMoveForwardOrBackward(direction))
            rb.velocity = direction * force;
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
        Vector3 forward = transform.position + transform.forward * rayCastOrigin;
        return !Physics.Raycast(forward, checkDir, rayCastDistance, obstacles) &&
            !Physics.Raycast(transform.position, checkDir, rayCastDistance, obstacles);
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

        if (Input.GetKeyDown(KeyCode.W))
            wantedNextDir = Vector3.forward;
        else if (Input.GetKeyDown(KeyCode.A))
            wantedNextDir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.S))
            wantedNextDir = Vector3.back;
        else if (Input.GetKeyDown(KeyCode.D))
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


    public void Score(int score)
    {

    }
}
