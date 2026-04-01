using UnityEngine;

public class ElaraAnimatorController : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (animator == null) return;

        float deltaX = transform.position.x - lastPosition.x;
        float moveAmount = Vector3.Distance(transform.position, lastPosition);

        bool isWalking = moveAmount > 0.001f;
        animator.SetBool("IsWalking", isWalking);

        if (spriteRenderer != null)
        {
            if (deltaX > 0.001f)
                spriteRenderer.flipX = false;
            else if (deltaX < -0.001f)
                spriteRenderer.flipX = true;
        }

        lastPosition = transform.position;
    }
}