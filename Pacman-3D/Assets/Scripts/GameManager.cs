using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("GameplayAdjusters")]
    [Range(1, 20)]
    public int level = 1;
    public int pacmanScore = 0;
    public int pelletsLeft;

    private float baseGhostSpeed;
    private float aggressiveSpeed;
    private float aggressiveSpeedFaster;
    private int aggressivePellets;
    private int aggressivePelletsFaster;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip ambient;
    public AudioClip wakka;
    public AudioClip death;


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
    }

    /*
     * lvl 1   = 75% pacman speed
     * lvl 2-4 = 85% pacman speed
     * lvl 5+  = 95% pacman speed
     */

    private void Start()
    {
        SetupBasedOnLevel();

        blinky.speed = pinky.speed = inky.speed = clyde.speed = baseGhostSpeed;

        PlayAudio(ambient, AudioPlayMode.continuous);
    }

    private void SetupBasedOnLevel()
    {
        if (level <= 1)
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

    void Update()
    {
        BlinkyTargetUpdate();
        PinkyTargetUpdate();
        InkyTargetUpdate();
        ClydeTargetUpdate();

        BlinkyUpdate();
    }

    private void BlinkyUpdate()
    {
        if (pelletsLeft < aggressivePelletsFaster)
            blinky.speed = aggressiveSpeedFaster;
        else if (pelletsLeft < aggressivePellets)
            blinky.speed = aggressiveSpeed;
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

    private void ClydeTargetUpdate()
    {
        if (clyde.state == GhostState.Chase && Vector3.Distance(pacman.transform.position, clyde.transform.position) > 8)
            clydeTarget.position = pacman.transform.position;
        else
            clydeTarget.position = new Vector3(-14f, 0, -18f);
    }

    public void PlayAudio(AudioClip clip, AudioPlayMode playMode)
    {
        if (playMode == AudioPlayMode.once)
        {
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.Play();
        }
        else if (playMode == AudioPlayMode.continuous)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (playMode == AudioPlayMode.mixed)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

public enum AudioPlayMode
{
    once,
    continuous,
    mixed
}