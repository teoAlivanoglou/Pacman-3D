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
    public Vector3 defaultPosition;

    public bool ai = true;
    public bool activated = false;
    public bool active = false;
    public bool debug = false;
    public bool isEscaping = false;

    public bool randomMovement = false;

    private Rigidbody rb;
    private Queue<Action> actionQueue = new Queue<Action>();

    public bool play = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //ResetSettings();
    }

    public bool isBlinky = false;

    public void ResetSettings()
    {
        play = false;
        if (!isBlinky)
        {
            active = false;
        }
        activated = false;

        WakeUp();

        transform.position = defaultPosition;
        transform.rotation = Quaternion.Euler(0, -90, 0);
        direction = Vector3.zero;
        nextDirection = Vector3.right;

        actionQueue.Clear();
        QueueActions();
    }

    public bool OscillationDone = false;
    public bool OscillationShouldFinish = false;
    Coroutine oldCr;

    public void WakeUp()
    {
        if (!active)
        {
            transform.position = defaultPosition;
            oldCr = StartCoroutine(GameManager.Instance.Oscillate(this));
        }
    }

    public void Init()
    {
        //Debug.Log(transform.name + " init!");
        activated = true;
        StartCoroutine(GameManager.Instance.Escape(this, oldCr));
    }

    public void QueueActions()
    {
        if (!ai && state != GhostState.Dead)
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
        //FlipDirection();
        timer = time;
        state = newState;
    }

    public Vector3 direction;// = Vector3.forward;
    public Vector3 nextDirection;// = Vector3.forward;

    private void Update()
    {
        if (play)
        {
            if (active)
            {
                if (!ai && state != GhostState.Dead)
                {
                    HandleInput();
                    if (CanTurn(nextDirection))
                        direction = nextDirection;
                }

                RotateModel();
            }
            TimeGhostState();
        }
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
        if (play)
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
                {
                    rb.velocity = direction * speed;
                }
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public AINode LastNode;// { get; protected set; }

    public void NodeHit(AINode node)
    {
        LastNode = node;
        if (!ai && state != GhostState.Dead)
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
            RotateModel();
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
        if (!ai)
        {
            GameManager.Instance.LoseLife(2);
            if (LastNode != null)
                NodeHit(LastNode);
        }
    }

    public void Resurrect()
    {
        GetComponentInChildren<BodyMesh>().ResetColor();
        randomMovement = false;
        active = false;
        Init();
        QueueActions();
        GetComponentInChildren<BodyMesh>().Enable();
        //FlipDirection();
        //NodeHit(LastNode);

        //TODO: FIND OUT WHY IT CRASHES

        //ResetSettings();
        LastNode = null;
    }

    private GhostState lastState;

    public void SetFrightened(int seconds)
    {
        if (active)
            FlipDirection();

        lastState = state;
        state = GhostState.Frightened;
        GetComponentInChildren<BodyMesh>().SetColor(Color.blue);
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