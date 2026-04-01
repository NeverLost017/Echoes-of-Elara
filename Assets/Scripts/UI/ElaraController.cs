using UnityEngine;

public class ElaraController : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private bool canMove = true;

    public void MoveTo(Vector3 target, float speed)
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(target, speed));
    }

    private System.Collections.IEnumerator MoveRoutine(Vector3 target, float speed)
    {
        canMove = true;
        animator.SetBool("IsWalking", true);

        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            Vector3 direction = (target - transform.position).normalized;

            // 👉 YÖN DÜZELTME
            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;

            transform.position += direction * speed * Time.deltaTime;

            yield return null;
        }

        transform.position = target;
        animator.SetBool("IsWalking", false);
        canMove = false;
    }

    // 👉 BAKMA FONKSİYONU
    public void LookAtTarget(Transform target)
    {
        if (target.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }

    // 👉 ANİ STOP (BUG FIX)
    public void StopMovement()
    {
        StopAllCoroutines();
        animator.SetBool("IsWalking", false);
    }
}