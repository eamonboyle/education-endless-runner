using UnityEngine;

/// <summary>
/// MonoBehaviour attached to obstacle prefabs. Handles collision with the
/// player (losing a life via <see cref="LivesSystem"/>) and auto-destruction
/// once the obstacle falls behind the player.
/// </summary>
public class Obstacle : MonoBehaviour
{
    [SerializeField, Tooltip("The type of this obstacle.")]
    private ObstacleSpawner.ObstacleType obstacleType;

    [SerializeField, Tooltip("Z distance behind the player at which the obstacle is destroyed.")]
    private float cleanupDistance = 20f;

    [SerializeField, Tooltip("Horizontal movement speed for MovingWall obstacles (units/s).")]
    private float moveSpeed = 2f;

    private Transform playerTransform;
    private float moveDirection = 1f;

    /// <summary>Gets or sets the obstacle type. Used by <see cref="ObstacleSpawner"/>.</summary>
    public ObstacleSpawner.ObstacleType Type
    {
        get => obstacleType;
        set => obstacleType = value;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        if (obstacleType == ObstacleSpawner.ObstacleType.MovingWall)
        {
            moveDirection = Random.value > 0.5f ? 1f : -1f;
        }
    }

    private void Update()
    {
        if (obstacleType == ObstacleSpawner.ObstacleType.MovingWall)
        {
            UpdateMovingWall();
        }

        CleanupIfBehindPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (obstacleType == ObstacleSpawner.ObstacleType.Gap) return;

        var powerUps = PowerUpSystem.Instance;
        if (powerUps != null && powerUps.TryConsumeShield())
        {
            Destroy(gameObject);
            return;
        }

        LivesSystem lives = LivesSystem.Instance;
        if (lives != null)
        {
            bool alive = lives.LoseLife();
            if (!alive)
            {
                GameState.ShowGameOverUI();
                PlayPlayerFallAnimation();
            }
        }
        else
        {
            GameState.ShowGameOverUI();
            PlayPlayerFallAnimation();
        }
    }

    private void UpdateMovingWall()
    {
        Vector3 pos = transform.position;
        pos.x += moveSpeed * moveDirection * Time.deltaTime;

        if (pos.x > MathRunner.Core.GameConstants.RIGHT_LANE)
        {
            pos.x = MathRunner.Core.GameConstants.RIGHT_LANE;
            moveDirection = -1f;
        }
        else if (pos.x < MathRunner.Core.GameConstants.LEFT_LANE)
        {
            pos.x = MathRunner.Core.GameConstants.LEFT_LANE;
            moveDirection = 1f;
        }

        transform.position = pos;
    }

    private void CleanupIfBehindPlayer()
    {
        if (playerTransform == null) return;

        if (transform.position.z < playerTransform.position.z - cleanupDistance)
        {
            Destroy(gameObject);
        }
    }

    private static void PlayPlayerFallAnimation()
    {
        GameObject player = GameObject.Find("PlayerObject");
        if (player == null) return;

        Animator animator = player.GetComponent<Animator>();
        if (animator == null) return;

        animator.Play("stumbleBackwards");
        animator.SetBool("isRunning", false);
    }
}
