using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    [Header("Characters")]
    public PacmanController pacman;
    [Space]
    public GhostController blinky;
    public GhostController pinky;
    public GhostController inky;
    public GhostController clyde;

    [Header("Targets")]
    public Transform blinkyTarget;
    public Transform pinkyTarget;
    public Transform inkyTarget;
    public Transform clydeTarget;
    public float targetDistanceThresshold = 0.1f;

    [Header("GameplayAdjusters")]
    [Range(0, 20)]
    public int level = 1;
    public int pacmanScore = 0;
    public int pelletsLeft;

    [Header("UI")]
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    private const string hrt = "<sprite=0 tint=1>";
    private const string txtDefaultP1 = "Player 1:<color=#DDD300>";
    private const string txtDefaultP2 = "Player 2:<color=";
    private const string txtBlinky = "#FD4425>";
    private const string txtPinky = "#F0A6EE>";
    private const string txtInky = "#93C9F4>";
    private const string txtClyde = "#F2C06D>";

    private float baseGhostSpeed;
    private float aggressiveSpeed;
    private float aggressiveSpeedFaster;
    private int aggressivePellets;
    private int aggressivePelletsFaster;

    private Vector3 blinkyScatterPos;
    private Vector3 pinkyScatterPos;
    private Vector3 inkyScatterPos;
    private Vector3 clydeScatterPos;

    private Vector3 blinkyResetPos;
    private Vector3 pinkyResetPos;
    private Vector3 inkyResetPos;
    private Vector3 clydeResetPos;

    [Header("Debug")]
    public bool play = false;

    private Queue<GhostController> ghostQueue;

    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null)
            {
                GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                _instance = gm;
            }
            return _instance;
        }
    }

    private void Awake()
    {
        pelletsLeft = GameObject.FindGameObjectWithTag("Pickups").transform.childCount;

        blinkyScatterPos = new Vector3(11.5f, 0, 18f);
        blinkyResetPos = new Vector3(0, 0, 1);

        pinkyScatterPos = new Vector3(-11.5f, 0, 18f);
        pinkyResetPos = new Vector3(0, 0, 1);

        inkyScatterPos = new Vector3(14f, 0, -18f);
        inkyResetPos = new Vector3(-2, 0, 1);

        clydeScatterPos = new Vector3(-14f, 0, -18f);
        clydeResetPos = new Vector3(2, 0, 1);

        ghostQueue = new Queue<GhostController>();
        ghostQueue.Enqueue(pinky);
        ghostQueue.Enqueue(inky);
        ghostQueue.Enqueue(clyde);
    }

    /*
     * lvl 1   = 75% pacman speed
     * lvl 2-4 = 85% pacman speed
     * lvl 5+  = 95% pacman speed
     */

    GhostController[] allGhosts;

    private void Start()
    {
        AddLife(1, 3);
        AddLife(2, 3);

        SetupBasedOnLevel();

        allGhosts = new GhostController[] { blinky, pinky, inky, clyde };

        blinky.speed = pinky.speed = inky.speed = clyde.speed = baseGhostSpeed;
        ResetBoard();
    }

    private IEnumerator StartGame(float time)
    {
        while (!play)
        {
            yield return new WaitForSeconds(time);

            blinky.WakeUp();
            pinky.WakeUp();
            inky.WakeUp();
            clyde.WakeUp();

            pinky.Init();

            play = true;
            pacman.play = true;
            blinky.play = true;
            pinky.play = true;
            inky.play = true;
            clyde.play = true;
        }
    }

    private void SetupBasedOnLevel()
    {
        if (level <= 0)
            InitSetUp(0, 20, 80);
        else if (level <= 1)
            InitSetUp(75, 20, 80);
        else if (level <= 2)
            InitSetUp(85, 30, 90);
        else if (level <= 4)
            InitSetUp(85, 40, 90);
        else if (level <= 5)
            InitSetUp(95, 40, 100);
        else if (level <= 8)
            InitSetUp(95, 50, 100);
        else if (level <= 11)
            InitSetUp(95, 60, 100);
        else if (level <= 14)
            InitSetUp(95, 80, 100);
        else if (level <= 18)
            InitSetUp(95, 100, 100);
        else
            InitSetUp(95, 120, 100);
    }

    private void InitSetUp(float baseGhostSpeed, int aggressivePellets, int aggressiveSpeed)
    {
        this.baseGhostSpeed = pacman.speed * baseGhostSpeed * 0.01f;
        this.aggressiveSpeed = pacman.speed * aggressiveSpeed * 0.01f;
        this.aggressiveSpeedFaster = pacman.speed * (aggressiveSpeed + 5) * 0.01f;

        this.aggressivePellets = aggressivePellets;
        this.aggressivePelletsFaster = aggressivePellets / 2;
    }

    private void LateUpdate()
    {
        if (play)
        {
            BlinkyTargetUpdate();
            PinkyTargetUpdate();
            InkyTargetUpdate();
            ClydeTargetUpdate();
        }
    }

    void Update()
    {
        if (play)
        {
            BlinkyUpdate();
            PinkyUpdate();
            InkyUpdate();
            ClydeUpdate();

            if (pelletsLeft < 214 && !inky.active)
                inky.Init();
            else if (pelletsLeft < 184 && !clyde.active)
                clyde.Init();


            if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < allGhosts.Length; i++)
                {
                    if (allGhosts[i].active && allGhosts[i].state != GhostState.Frightened && allGhosts[i].state != GhostState.Dead)
                        allGhosts[i].SetFrightened(6);
                }
            }
        }
    }

    private void BlinkyUpdate()
    {
        if (blinky.state == GhostState.Dead)
        {
            blinky.speed = baseGhostSpeed * 2;
            if (Vector3.Distance(blinky.transform.position, blinkyResetPos) < targetDistanceThresshold)
            {
                blinky.Resurrect();
            }
        }
        else
        {
            blinky.speed = baseGhostSpeed;
            if (pelletsLeft < aggressivePelletsFaster)
                blinky.speed = aggressiveSpeedFaster;
            else if (pelletsLeft < aggressivePellets)
                blinky.speed = aggressiveSpeed;
        }
    }

    private void PinkyUpdate()
    {
        if (pinky.state == GhostState.Dead)
        {
            pinky.speed = baseGhostSpeed * 2;
            if (Vector3.Distance(pinky.transform.position, pinkyResetPos) < targetDistanceThresshold)
            {
                pinky.Resurrect();
            }
        }
        else
        {
            pinky.speed = baseGhostSpeed;
        }
    }

    private void InkyUpdate()
    {
        if (inky.state == GhostState.Dead)
        {
            inky.speed = baseGhostSpeed * 2;
            if (Vector3.Distance(inky.transform.position, inkyResetPos) < targetDistanceThresshold)
            {
                inky.Resurrect();
            }
        }
        else
        {
            inky.speed = baseGhostSpeed;
        }
    }

    private void ClydeUpdate()
    {
        if (clyde.state == GhostState.Dead)
        {
            clyde.speed = baseGhostSpeed * 2;
            if (Vector3.Distance(clyde.transform.position, clydeResetPos) < targetDistanceThresshold)
            {
                clyde.Resurrect();
            }
        }
        else
        {
            clyde.speed = baseGhostSpeed;
        }
    }

    private void BlinkyTargetUpdate()
    {
        if (blinky.state == GhostState.Chase)
            blinkyTarget.position = pacman.transform.position;
        else if (blinky.state == GhostState.Scatter)
            blinkyTarget.position = blinkyScatterPos;
        else if (blinky.state == GhostState.Dead)
            blinkyTarget.position = blinkyResetPos;
        //else
        //    blinkyTarget.position = 2 * blinky.transform.position - pacman.transform.position;
    }

    private void PinkyTargetUpdate()
    {
        if (pinky.state == GhostState.Chase)
            pinkyTarget.position = pacman.transform.position + pacman.transform.forward * 4f;
        else if (pinky.state == GhostState.Scatter)
            pinkyTarget.position = pinkyScatterPos;
        else if (pinky.state == GhostState.Dead)
            pinkyTarget.position = pinkyResetPos;
        //else
        //    pinkyTarget.position = 2 * pinky.transform.position - pacman.transform.position;
    }

    private void InkyTargetUpdate()
    {
        if (inky.state == GhostState.Chase)
        {
            Vector3 pivot = pacman.transform.position + pacman.transform.forward * 2f;

            inkyTarget.position = 2 * pivot - blinky.transform.position;
        }
        else if (inky.state == GhostState.Scatter)
            inkyTarget.position = inkyScatterPos;
        else if (inky.state == GhostState.Dead)
            inkyTarget.position = inkyResetPos;
        //else
        //    inkyTarget.position = 2 * inky.transform.position - pacman.transform.position;
    }

    private bool canFlip = true;

    private void ClydeTargetUpdate()
    {
        if (clyde.state == GhostState.Chase)
        {
            bool pacmanIsClose = Vector3.Distance(pacman.transform.position, clyde.transform.position) < 8;

            if (pacmanIsClose)
            {
                clyde.randomMovement = false;
                clydeTarget.position = pacman.transform.position;
            }
            else
            {
                clyde.randomMovement = true;
            }
        }
        else if (clyde.state == GhostState.Scatter)
            clydeTarget.position = clydeScatterPos;
        else if (clyde.state == GhostState.Dead)
            clydeTarget.position = clydeResetPos;
        //else
        //    clydeTarget.position = 2 * clyde.transform.position - pacman.transform.position;
    }

    public void ResetBoard()
    {
        Debug.Log("GAMEMANAGER: Reseting board...");
        play = false;
        StartCoroutine(StartGame(3.1f));
        pacman.ResetSettings();//transform.position = new Vector3(0, 0, -8);
        blinky.ResetSettings();//transform.position = new Vector3(0, 0, 4);
        pinky.ResetSettings(); //transform.position = new Vector3(0, 0, 1);
        inky.ResetSettings();  //transform.position = new Vector3(-2, 0, 1);
        clyde.ResetSettings(); //transform.position = new Vector3(2, 0, 1);
    }

    public IEnumerator Oscillate(GhostController ghost)
    {
        float progress = 0.5f;
        
        Vector3 top = new Vector3(ghost.transform.position.x, ghost.transform.position.y, ghost.transform.position.z + 0.5f);
        Vector3 bottom = new Vector3(ghost.transform.position.x, ghost.transform.position.y, ghost.transform.position.z - 0.5f);

        float speed = baseGhostSpeed / 2;

        while (!ghost.OscillationDone)
        {
            progress += speed * Time.deltaTime;

            Vector3 nextPos = Vector3.Lerp(top, bottom, TriangleWave(progress));
            ghost.transform.LookAt(nextPos, Vector3.up);
            ghost.transform.position = nextPos;

            if (ghost.OscillationShouldFinish && progress - Mathf.Floor(progress) < 0.1f)
            {
                //Debug.Log("Oscillation done!");
                ghost.OscillationDone = true;
            }

            yield return null;
        }
        ghost.transform.position = new Vector3(ghost.transform.position.x, ghost.transform.position.y, 1);
    }

    public IEnumerator Escape(GhostController ghost, Coroutine oldCr)
    {
        //StopCoroutine(oldCr);
        //Debug.Log("Triggered escape!");
        ghost.OscillationShouldFinish = true;

        Vector3 pos = ghost.defaultPosition;
        Vector3 middle = new Vector3(0, 0, 1);
        Vector3 outside = new Vector3(0, 0, 4);

        int step = 0;
        float progress = 0;

        float speed = baseGhostSpeed / 2;
        float speedMod = 2 / Vector3.Distance(pos, middle);

        while (step == 0 && progress <= 1)
        {
            if (ghost.OscillationDone)
            {
                //Debug.Log("Escaping...");
                progress += speed * speedMod * Time.deltaTime;

                Vector3 nextPos = Vector3.Lerp(pos, middle, progress);
                ghost.transform.LookAt(nextPos, Vector3.up);
                ghost.transform.position = nextPos;

                if (progress >= 1)
                {
                    progress = 0;
                    step++;
                }
            }
            yield return null;
        }

        //speedMod = 0.5f;
        speedMod = 2 / Vector3.Distance(middle, outside);

        while (step == 1 && progress <= 1)
        {
            if (ghost.OscillationDone)
            {
                progress += speed * speedMod * Time.deltaTime;

                Vector3 nextPos = Vector3.Lerp(middle, outside, progress);
                ghost.transform.LookAt(nextPos, Vector3.up);
                ghost.transform.position = nextPos;

                if (progress >= 1)
                {
                    progress = 0;
                    step++;
                }

                yield return null;
            }
        }
        ghost.active = true;
    }

    public static float TriangleWave(float x)
    {
        //return Mathf.Abs(((4 * x - 5) % 4) - 2) - 1;
        return Mathf.Abs(((2 * x - 0.5f) % 2) - 1);
    }

    private int player1Lifes = 0;
    private int player2Lifes = 0;

    public void AddLife(int player, int amount = 1)
    {
        if (player == 1)
        {
            if (player1Lifes + amount <= 3)
                player1Lifes += amount;

            player1Text.text = txtDefaultP1;
            for (int i = 0; i < player1Lifes; i++)
            {
                player1Text.text += hrt;
            }
        }
        else if (player == 2)
        {
            if (player2Lifes + amount <= 3)
                player2Lifes += amount;

            player2Text.text = txtDefaultP2;

            if (!blinky.ai) player2Text.text += txtBlinky;
            else if (!pinky.ai) player2Text.text += txtPinky;
            else if (!inky.ai) player2Text.text += txtInky;
            else if (!clyde.ai) player2Text.text += txtClyde;

            for (int i = 0; i < player2Lifes; i++)
            {
                player2Text.text += hrt;
            }
        }
    }

    public void LoseLife(int player)
    {
        if (player == 1)
        {
            if (player1Lifes > 0)
                player1Lifes--;
            player1Text.text = txtDefaultP1;
            for (int i = 0; i < player1Lifes; i++)
            {
                player1Text.text += hrt;
            }
        }
        else if (player == 2)
        {
            if (player2Lifes > 0)
                player2Lifes--;
            player2Text.text = txtDefaultP2;

            if (!blinky.ai) player2Text.text += txtBlinky;
            else if (!pinky.ai) player2Text.text += txtPinky;
            else if (!inky.ai) player2Text.text += txtInky;
            else if (!clyde.ai) player2Text.text += txtClyde;

            for (int i = 0; i < player2Lifes; i++)
            {
                player2Text.text += hrt;
            }
        }
    }

}


public enum AudioPlayMode
{
    once,
    continuous,
    mixed
}