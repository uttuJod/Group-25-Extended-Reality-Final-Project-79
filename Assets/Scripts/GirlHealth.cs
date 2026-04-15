using UnityEngine;

public class GirlHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 60;

    [Header("Target / Anchor")]
    public Transform attackTargetPoint;
    public Transform healthBarAnchor;

    [Header("World Health Bar")]
    public GameObject worldHealthBarPrefab;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hurtClip;

    [Header("Optional Death Animation")]
    public string dieTriggerName = "Die";

    int currentHealth;
    bool dead = false;

    Animator animator;
    GirlIntroController introController;
    GameObject spawnedHealthBar;

    public int CurrentHealth => currentHealth;
    public float HealthPercent => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    public bool IsDead => dead;

    public Transform AttackTarget
    {
        get
        {
            if (attackTargetPoint != null)
                return attackTargetPoint;

            if (healthBarAnchor != null)
                return healthBarAnchor;

            return transform;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        introController = GetComponent<GirlIntroController>();

        if (worldHealthBarPrefab != null)
        {
            spawnedHealthBar = Instantiate(worldHealthBarPrefab);

            GirlWorldHealthBar bar = spawnedHealthBar.GetComponent<GirlWorldHealthBar>();
            if (bar != null)
            {
                bar.target = this;
                bar.followTarget = healthBarAnchor != null ? healthBarAnchor : transform;
            }
            else
            {
                Debug.LogWarning("Girl health bar prefab is missing GirlWorldHealthBar script.");
            }
        }
        else
        {
            Debug.LogWarning("Girl worldHealthBarPrefab is NULL on: " + gameObject.name);
        }
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;
        if (amount <= 0) return;

        if (audioSource != null && hurtClip != null)
            audioSource.PlayOneShot(hurtClip);

        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        dead = true;

        if (spawnedHealthBar != null)
            Destroy(spawnedHealthBar);

        if (introController != null)
            introController.enabled = false;

        Collider[] allColliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < allColliders.Length; i++)
            allColliders[i].enabled = false;

        if (animator != null)
        {
            if (HasBoolParameter(animator, "isWalking"))
                animator.SetBool("isWalking", false);

            if (HasTriggerParameter(animator, dieTriggerName))
                animator.SetTrigger(dieTriggerName);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnGirlDied();
    }

    bool HasBoolParameter(Animator anim, string paramName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName && param.type == AnimatorControllerParameterType.Bool)
                return true;
        }

        return false;
    }

    bool HasTriggerParameter(Animator anim, string paramName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName && param.type == AnimatorControllerParameterType.Trigger)
                return true;
        }

        return false;
    }
}