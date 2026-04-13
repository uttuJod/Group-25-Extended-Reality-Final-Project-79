using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float damageDistance = 1.5f;
    public int damageAmount = 10;
    public float damageCooldown = 1f;

    Transform target;
    float lastDamageTime;
    Animator animator;

    void Start()
    {
        if (Camera.main != null)
        {
            target = Camera.main.transform;
            Debug.Log(gameObject.name + " found target: " + target.name);
        }
        else
        {
            Debug.LogError(gameObject.name + " could not find Camera.main");
        }

        animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
    }

    void Update()
    {
        if (target == null) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        Vector3 targetPos = target.position;
        targetPos.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist > damageDistance)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            if (dir != Vector3.zero)
                transform.forward = dir;

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            if (dir != Vector3.zero)
                transform.forward = dir;

            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
            }

            if (Time.time >= lastDamageTime + damageCooldown)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.TakeDamage(damageAmount);

                lastDamageTime = Time.time;
            }
        }
    }

    void OnDisable()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }
}