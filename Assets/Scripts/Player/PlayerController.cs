using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Lane
    {
        Left,
        Center,
        Right
    }

    public enum MoveDirection
    {
        Left,
        Right,
        None
    }

    public float speed = 5.0f;

    public bool jumping = false;
    public bool sliding = false;

    public Animator anim;

    private Lane currentLane = Lane.Center;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        // move forwards
        transform.Translate(0, 0, speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }

        if (jumping)
        {
            anim.SetBool("isJumping", true);

            transform.Translate(0, 0.1f, 0.1f);
        }
        else
        {
            anim.SetBool("isJumping", false);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            sliding = true;
        }
        else
        {
            sliding = false;
        }

        if (sliding)
        {
            anim.SetBool("isSliding", true);
        }
        else
        {
            anim.SetBool("isSliding", false);
        }

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    transform.Translate(-4.1f, 0, 0);
        //}


        // HANDLE LANE MOVEMENT
        MoveDirection requestedMoveDirection = MoveDirection.None;

        if (Input.GetKeyDown(KeyCode.A))
        {
            requestedMoveDirection = MoveDirection.Left;
        } 
        else if (Input.GetKeyDown(KeyCode.D))
        {
            requestedMoveDirection = MoveDirection.Right;
        }

        HandleLaneMovement(requestedMoveDirection);
    }

    private void HandleLaneMovement(MoveDirection requestedMoveDirection)
    {
        switch (requestedMoveDirection)
        {
            case MoveDirection.Left:
                if (currentLane == Lane.Left)
                {
                    break;
                }
                else if (currentLane == Lane.Center)
                {
                    MoveLeft();
                    currentLane = Lane.Left;
                }
                else if (currentLane == Lane.Right)
                {
                    MoveLeft();
                    currentLane = Lane.Center;
                }
                break;
            case MoveDirection.Right:
                if (currentLane == Lane.Right)
                {
                    break;
                }
                else if (currentLane == Lane.Center)
                {
                    MoveRight();
                    currentLane = Lane.Right;
                }
                else if (currentLane == Lane.Left)
                {
                    MoveRight();
                    currentLane = Lane.Center;
                }
                break;
            default:
                break;
        }
    }

    private void MoveRight()
    {
        Debug.Log("Move Right");
    }

    private void MoveLeft()
    {
        Debug.Log("Move Left");
    }
}
