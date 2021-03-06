﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public LayerMask obstacles;

    public float speed = 1f;
    public bool debug = false;

    public bool Dead { get; protected set; }

    private Rigidbody rb;

    public bool play = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ResetSettings();
    }

    private Vector3 direction;
    private Vector3 nextDirection;

    private void Update()
    {
        if (play)
        {
            if (!Dead)
            {
                HandleInput();

                if (CanTurn(nextDirection))
                    direction = nextDirection;

                RotateModel();
            }
        }
    }

    private void FixedUpdate()
    {
        if (play)
        {
            if (CanMoveForwardOrBackward(direction))
                rb.velocity = direction * speed;
        }
        else
            rb.velocity = Vector3.zero;
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

        if (Input.GetKey(KeyCode.W))
            wantedNextDir = Vector3.forward;
        else if (Input.GetKey(KeyCode.A))
            wantedNextDir = Vector3.left;
        else if (Input.GetKey(KeyCode.S))
            wantedNextDir = Vector3.back;
        else if (Input.GetKey(KeyCode.D))
            wantedNextDir = Vector3.right;
        else return;

        // if at least one of our check points misses, next direction should be set
        if (CanRegisterNextDir(wantedNextDir))
            nextDirection = wantedNextDir;
    }

    public void Die()
    {
        Dead = true;

        GameManager.Instance.LoseLife(1);
        GameManager.Instance.ResetBoard();
    }


    void Dbg(string s)
    {
        if (debug)
            Debug.Log(s);
    }

    public void ResetSettings()
    {
        transform.position = new Vector3(0, 0, -8);
        Dead = false;
        play = false;
    }
}
