using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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

    public float forwardSpeed = 40.0f;
    public float directionSpeed = 2.0f;

    private Lane currentLane = Lane.Center;
    private PlayerController playerController;
    private int direction = 0;

    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
    }


    void Update()
    {
        if (playerController.swipeRight)
        {
            CalculateLane(MoveDirection.Right);
        }

        if (playerController.swipeLeft)
        {
            CalculateLane(MoveDirection.Left);
        }

        MoveCharacter();
    }

    private void CalculateLane(MoveDirection requestedDirection)
    {
        //Debug.Log("CURRENT LANE: " + currentLane);
        switch (requestedDirection)
        {
            case MoveDirection.Left:
                if (currentLane == Lane.Left)
                {
                    break;
                }
                else if (currentLane == Lane.Center)
                {
                    direction--;
                    currentLane = Lane.Left;
                }
                else if (currentLane == Lane.Right)
                {
                    direction--;
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
                    direction++;
                    currentLane = Lane.Right;
                }
                else if (currentLane == Lane.Left)
                {
                    direction++;
                    currentLane = Lane.Center;
                }
                break;
            default:
                break;
        }
    }

    private void MoveCharacter()
    {
        Vector3 newPosition = transform.position;
        newPosition.z = Mathf.Lerp(transform.position.z, transform.position.z + forwardSpeed * Time.deltaTime, 0.1f);
        newPosition.x = Mathf.Lerp(transform.position.x, directionSpeed * direction, 0.9f);
        transform.position = newPosition;
    }
}
