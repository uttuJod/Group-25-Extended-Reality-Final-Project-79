using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Gun Visual Setup")]
    public Transform firePoint;
    public Animator gunAnimator;
    public string shootTriggerName = "Shoot";

    [Header("Shooting")]
    public float shootRange = 50f;
    public int damage = 1;
    public LayerMask hitMask;

    [Header("Ammo")]
    public int ammoPerShot = 1;
    public int headshotAmmoReward = 2;

    [Header("Shoot FX")]
    public AudioSource shootAudioSource;
    public AudioClip shootClip;

    [Header("Hit FX")]
    public GameObject bloodEffectPrefab;
    public GameObject defaultHitEffectPrefab;
    public float hitEffectLifetime = 1.0f;
    public float hitEffectOffset = 0.02f;

    [Header("UI Feedback")]
    public UIFeedbackController uiFeedback;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    public void Shoot()
    {
        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.TryUseAmmo(ammoPerShot))
                return;
        }

        if (gunAnimator != null && !string.IsNullOrEmpty(shootTriggerName))
            gunAnimator.SetTrigger(shootTriggerName);

        if (shootAudioSource != null && shootClip != null)
            shootAudioSource.PlayOneShot(shootClip);

        if (playerCamera == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.green, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange, hitMask, QueryTriggerInteraction.Collide))
        {
            BodyPartHitbox bodyPart = hit.collider.GetComponent<BodyPartHitbox>();
            if (bodyPart == null)
                bodyPart = hit.collider.GetComponentInParent<BodyPartHitbox>();

            if (bodyPart != null && bodyPart.owner != null)
            {
                SpawnHitEffect(hit, true);
                bodyPart.owner.TakeBodyPartDamage(bodyPart.zone, damage, headshotAmmoReward);

                if (uiFeedback != null)
                    uiFeedback.ShowHitmarker();

                return;
            }

            ZombieHealth zombie = hit.collider.GetComponent<ZombieHealth>();
            if (zombie == null)
                zombie = hit.collider.GetComponentInParent<ZombieHealth>();

            if (zombie != null)
            {
                SpawnHitEffect(hit, true);
                zombie.TakeBodyPartDamage(BodyPartZone.Torso, damage, 0);

                if (uiFeedback != null)
                    uiFeedback.ShowHitmarker();

                return;
            }

            SpawnHitEffect(hit, false);
        }
    }

    void SpawnHitEffect(RaycastHit hit, bool hitZombie)
    {
        GameObject effectPrefab = hitZombie ? bloodEffectPrefab : defaultHitEffectPrefab;

        if (effectPrefab == null)
            return;

        Vector3 spawnPosition = hit.point + hit.normal * hitEffectOffset;
        Quaternion spawnRotation = Quaternion.LookRotation(hit.normal);

        GameObject fx = Instantiate(effectPrefab, spawnPosition, spawnRotation);
        Destroy(fx, hitEffectLifetime);
    }
}