﻿using System;
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
    public float targetDistanceThresshold = 0.1f;

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
    public AudioClip audioAmbient;
    public AudioClip audioWakka;
    public AudioClip audioPacmanEaten;
    public AudioClip audioGhostEaten;


    private Vector3 blinkyScatterPos;
    private Vector3 pinkyScatterPos;
    private Vector3 inkyScatterPos;
    private Vector3 clydeScatterPos;

    private Vector3 blinkyResetPos;
    private Vector3 pinkyResetPos;
    private Vector3 inkyResetPos;
    private Vector3 clydeResetPos;


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

        pinkyScatterPos  = new Vector3(-11.5f, 0, 18f);
        pinkyResetPos = new Vector3(0, 0, 1);

        inkyScatterPos = new Vector3(14f, 0, -18f);
        inkyResetPos = new Vector3(-2, 0, 1);

        clydeScatterPos = new Vector3(-14f, 0, -18f);
        clydeResetPos = new Vector3(2, 0, 1);
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

        PlayAudio(audioAmbient, AudioPlayMode.continuous);
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
        PinkyUpdate();
        InkyUpdate();
        ClydeUpdate();
    }

    private void BlinkyUpdate()
    {
        if (blinky.state == GhostState.Dead)
        {
            blinky.speed = baseGhostSpeed * 2;
            if (Vector3.Distance(blinky.transform.position, blinkyResetPos) < targetDistanceThresshold)
            {
                blinky.Resurect();
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
                pinky.Resurect();
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
                inky.Resurect();
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
                clyde.Resurect();
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
    }

    private void PinkyTargetUpdate()
    {
        if (pinky.state == GhostState.Chase)
            pinkyTarget.position = pacman.transform.position + pacman.transform.forward * 4f;
        else if (pinky.state == GhostState.Scatter)
            pinkyTarget.position = pinkyScatterPos;
        else if (pinky.state == GhostState.Dead)
            pinkyTarget.position = pinkyResetPos;
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
    }

    private void ClydeTargetUpdate()
    {
        if (clyde.state == GhostState.Chase && Vector3.Distance(pacman.transform.position, clyde.transform.position) > 8)
            clydeTarget.position = pacman.transform.position;
        else if (clyde.state == GhostState.Scatter)
            clydeTarget.position = clydeScatterPos;
        else if (clyde.state == GhostState.Dead)
            clydeTarget.position = clydeResetPos;
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