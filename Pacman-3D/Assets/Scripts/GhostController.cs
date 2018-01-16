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

    //public float force = 1f;
    public float speed;

    public bool ai = true;
    public bool active = false;
    public bool debug = false;

    public bool randomMovement = false;

    private Rigidbody rb;
    private Queue<Action> actionQueue = new Queue<Action>();
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.enabled = false;
        if (ai)
            nextDirection = Vector3.right;
    }

    private void Start()
    {
        QueueActions();
        //if (!active)
        //    anim.Play("GhostUpDownAnim", PlayMode.StopAll);

    }

    public bool OscillationDone = false;
    public bool OscillationShouldFinish = false;
    Coroutine oldCr;

    public void WakeUp()
    {
        //anim.SetTrigger("WakeUp");
        if (!active)
            oldCr = StartCoroutine(GameManager.Instance.Oscillate(this));
    }

    public void Init()
    {
        //anim.SetTrigger("GoOut");
        StartCoroutine(GameManager.Instance.Escape(this, oldCr));
        //active = true;
    }

    public void QueueActions()
    {
        if (!ai)
            return;

        timer = 0;
        actionQueue.Enqueue(() => ChangeState(7, GhostState.Scatter));
        actionQueue.Enqueue(() => ChangeState(20, GhostState.Chase));

        actionQueue.Enqueue(() => ChangeState(7, GhostState.Scatter));
        actionQueue.Enqueue(() => ChangeState(20, GhostState.Chase));

        actionQueue.Enqueue(() => ChangeState(5, GhostState.Scatter));
        actionQueue.Enqueue(() => ChangeState(20, GhostState.Chase));

        actionQueue.Enqueue(() => ChangeState(5, GhostState.Scatter));
        actionQueue.Enqueue(() => ChangeState(1, GhostState.Chase));
    }

    private float timer;

    private void ChangeState(float time, GhostState newState)
    {
        //if (debug) Debug.Log("I'll " + newState + " for " + time + "seconds.");
        timer = time;
        state = newState;
    }

    private Vector3 direction;// = Vector3.forward;
    private Vector3 nextDirection;// = Vector3.forward;

    private void Update()
    {
        if (active)
        {
            if (!pacman.Dead)
            {
                if (!ai)
                {
                    HandleInput();
                    if (CanTurn(nextDirection))
                        direction = nextDirection;
                }

                RotateModel();

                if (Input.GetKeyDown(KeyCode.Space))
                    if (ai && state != GhostState.Frightened && state != GhostState.Dead)
                        SetFrightened(6);

            }
            else
            {
                rb.isKinematic = true;
            }
        }
        TimeGhostState();
    }

    void TimeGhostState()
    {
        if (actionQueue.Count > 0 && state != GhostState.Frightened)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else
            {
                FlipDirection();
                actionQueue.Dequeue()();
            }
        }
    }

    private void FixedUpdate()
    {
        if (active)
        {
            if (setPositionToLastNode)
            {
                if (LastNode != null)
                    transform.position = LastNode.transform.position;
                setPositionToLastNode = false;
            }

            if (CanMoveForwardOrBackward(direction))
                rb.velocity = direction * speed;
        }
    }

    public AINode LastNode;// { get; protected set; }

    public void NodeHit(AINode node)
    {
        LastNode = node;
        if (!ai)
            return;

        if (node != null)
        {
            List<Vector3> allowedDirs = node.GetAllowedDirs(state, direction);// GetAllowedDirections();

            int minIndex = -1;
            float minDistance = float.PositiveInfinity;

            if (allowedDirs.Count > 0)
            {
                if (!randomMovement)
                {
                    for (int i = 0; i < allowedDirs.Count; i++)
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
                }
                else
                {
                    nextDirection = allowedDirs[UnityEngine.Random.Range(0, allowedDirs.Count)];
                }
            }
            else
            {
                nextDirection = Vector3.forward;
            }

            setPositionToLastNode = true;
            direction = nextDirection;
        }
    }

    bool setPositionToLastNode = false;

    public void FlipDirection()
    {
        //if (lastState != GhostState.Dead)
        //if (LastNode != null)
            direction = nextDirection = -direction;
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
        else
        {
            //Debug.Log("cant move forward :/");
            return false;
        }
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

            Debug.DrawRay(forward, checkDir * rayCastDistance, Color.green, 2f);

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

        if (Input.GetKey(KeyCode.UpArrow))
            wantedNextDir = Vector3.forward;
        else if (Input.GetKey(KeyCode.LeftArrow))
            wantedNextDir = Vector3.left;
        else if (Input.GetKey(KeyCode.DownArrow))
            wantedNextDir = Vector3.back;
        else if (Input.GetKey(KeyCode.RightArrow))
            wantedNextDir = Vector3.right;
        else return;

        // if at least one of our check points misses, next direction should be set
        if (CanRegisterNextDir(wantedNextDir))
            nextDirection = wantedNextDir;
    }

    public void Die()
    {
        state = GhostState.Dead;
        GetComponentInChildren<BodyMesh>().Disable();
        actionQueue.Clear();
        randomMovement = false;
        StopAllCoroutines();
    }

    public void Resurrect()
    {
        GetComponentInChildren<BodyMesh>().ResetColor();
        randomMovement = false;
        LastNode = null;
        QueueActions();
        GetComponentInChildren<BodyMesh>().Enable();
        direction = nextDirection = Vector3.forward;
    }

    private GhostState lastState;

    public void SetFrightened(int seconds)
    {
        lastState = state;
        state = GhostState.Frightened;
        GetComponentInChildren<BodyMesh>().SetColor(Color.blue);
        FlipDirection();
        randomMovement = true;
        StartCoroutine(EndFrightened(seconds));
    }

    IEnumerator EndFrightened(int time)
    {
        while (state == GhostState.Frightened)
        {
            yield return new WaitForSeconds(time);
            randomMovement = false;
            state = lastState;
            GetComponentInChildren<BodyMesh>().ResetColor();
        }
    }

    public void Score()
    {

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
    Frightened,
    Dead
}