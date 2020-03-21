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

    public AudioClip slideSound;
    public GameObject dust;

    public float forwardSpeed;
    public float directionAmount = 2.0f;
    public float directionSpeed = 1.4f;

    public Lane currentLane = Lane.Center;
    private PlayerController playerController;
    private AudioSource audioSource;
    private int direction = 0;

    private void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!GameState.IsRunning())
        {
            return;
        }

        // get the currentSpeed
        forwardSpeed = GameState.GetCharacterSpeed();

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
        switch (requestedDirection)
        {
            case MoveDirection.Left:
                if (currentLane == Lane.Left)
                {
                    break;
                }
                else if (currentLane == Lane.Center)
                {
                    PlaySlideSound();
                    direction--;
                    currentLane = Lane.Left;
                }
                else if (currentLane == Lane.Right)
                {
                    PlaySlideSound();
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
                    PlaySlideSound();
                    direction++;
                    currentLane = Lane.Right;
                }
                else if (currentLane == Lane.Left)
                {
                    PlaySlideSound();
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
        newPosition.x = Mathf.Lerp(transform.position.x, directionAmount * direction, directionSpeed);
        transform.position = newPosition;
    }

    void PlaySlideSound()
    {
        audioSource.clip = slideSound;
        audioSource.Play();

        // spawn dust particles
        Vector3 dustSpawn = transform.position;
        dustSpawn.z -= .5f;
        Instantiate(dust, dustSpawn, Quaternion.identity, transform);
    }
}