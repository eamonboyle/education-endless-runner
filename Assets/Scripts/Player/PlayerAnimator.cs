using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("dancing", true);
        }
    }
}
