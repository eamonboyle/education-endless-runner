using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTouch : MonoBehaviour
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

    public const float MAX_SWIPE_TIME = 0.5f;
    public const float MIN_SWIPE_DISTANCE = 0.17f;
    public float speed = 30.0f;

    public static bool swipedRight = false;
    public static bool swipedLeft = false;
    public static bool swipedUp = false;
    public static bool swipedDown = false;

    public bool debugWithArrowKeys = true;

    private Vector2 startPos;
    private float startTime;
    private Lane currentLane = Lane.Center;

    int direction;

    // Update is called once per frame
    void Update()
    {
        swipedRight = false;
        swipedLeft = false;
        swipedUp = false;
        swipedDown = false;
        MoveDirection requestedMoveDirection = MoveDirection.None;
        HandleLaneMovement(requestedMoveDirection);

        AnalyzeMovement();

        //transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, 3 * 1), 0.1f);

        // move forward
        //transform.Translate(0, 0, speed * Time.deltaTime);

        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                startPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
                startTime = Time.time;
            }

            if (t.phase == TouchPhase.Ended)
            {
                // pressed too long
                if (Time.time - startTime > MAX_SWIPE_TIME)
                {
                    return;
                }

                Vector2 endPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
                Vector2 swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

                // too short swipe
                if (swipe.magnitude < MIN_SWIPE_DISTANCE)
                {
                    return;
                }

                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                {
                    if (swipe.x > 0)
                    {
                        swipedRight = true;
                        requestedMoveDirection = MoveDirection.Right;
                        HandleLaneMovement(requestedMoveDirection);
                    }
                    else
                    {
                        swipedLeft = true;
                        requestedMoveDirection = MoveDirection.Left;
                        HandleLaneMovement(requestedMoveDirection);
                    }
                }
                else
                {
                    if (swipe.y > 0)
                    {
                        swipedUp = true;
                    }
                    else
                    {
                        swipedDown = true;
                    }
                }
            }
        }

    }

    private void HandleLaneMovement(MoveDirection requestedMoveDirection)
    {
        Debug.Log(currentLane);
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
        //transform.Translate(3.1f, 0, 0);
        direction++;
    }

    private void MoveLeft()
    {
        //transform.Translate(-3.1f, 0, 0);
        direction--;
    }

    private void AnalyzeMovement()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(3 * direction, 0, transform.position.z + speed * Time.deltaTime), 0.1f);
    }
}
