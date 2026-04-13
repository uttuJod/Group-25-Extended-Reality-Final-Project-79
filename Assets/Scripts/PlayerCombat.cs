using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Camera playerCamera;
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
        Debug.Log("PlayerCombat running on object: " + gameObject.name);
        Debug.Log("Blood prefab at Start: " + (bloodEffectPrefab != null ? bloodEffectPrefab.name : "NULL"));
        Debug.Log("UIFeedback at Start: " + (uiFeedback != null ? uiFeedback.name : "NULL"));
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("Shoot called on object: " + gameObject.name);
        Debug.Log("Blood prefab before shot: " + (bloodEffectPrefab != null ? bloodEffectPrefab.name : "NULL"));
        Debug.Log("UIFeedback before shot: " + (uiFeedback != null ? uiFeedback.name : "NULL"));

        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.TryUseAmmo(ammoPerShot))
            {
                Debug.Log("No ammo");
                return;
            }
        }

        if (shootAudioSource != null && shootClip != null)
            shootAudioSource.PlayOneShot(shootClip);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.red, 1.0f);

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange, hitMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);

            BodyPartHitbox bodyPart = hit.collider.GetComponent<BodyPartHitbox>();
            if (bodyPart == null)
                bodyPart = hit.collider.GetComponentInParent<BodyPartHitbox>();

            if (bodyPart != null && bodyPart.owner != null)
            {
                Debug.Log("BodyPart branch reached");

                SpawnHitEffect(hit, true);
                bodyPart.owner.TakeBodyPartDamage(bodyPart.zone, damage, headshotAmmoReward);

                if (uiFeedback != null)
                {
                    Debug.Log("Showing hitmarker from body part branch");
                    uiFeedback.ShowHitmarker();
                }
                else
                {
                    Debug.Log("uiFeedback is NULL in body part branch");
                }

                return;
            }

            ZombieHealth zombie = hit.collider.GetComponent<ZombieHealth>();
            if (zombie == null)
                zombie = hit.collider.GetComponentInParent<ZombieHealth>();

            if (zombie != null)
            {
                Debug.Log("Zombie branch reached");

                SpawnHitEffect(hit, true);
                zombie.TakeBodyPartDamage(BodyPartZone.Torso, damage, 0);

                if (uiFeedback != null)
                {
                    Debug.Log("Showing hitmarker from zombie branch");
                    uiFeedback.ShowHitmarker();
                }
                else
                {
                    Debug.Log("uiFeedback is NULL in zombie branch");
                }

                return;
            }

            Debug.Log("Non-zombie surface hit");
            SpawnHitEffect(hit, false);
        }
        else
        {
            Debug.Log("Raycast missed");
        }
    }

    void SpawnHitEffect(RaycastHit hit, bool hitZombie)
    {
        GameObject effectPrefab = hitZombie ? bloodEffectPrefab : defaultHitEffectPrefab;

        if (effectPrefab == null)
        {
            Debug.Log("No effect prefab assigned for this hit type");
            return;
        }

        Vector3 spawnPosition = hit.point + hit.normal * hitEffectOffset;
        Quaternion spawnRotation = Quaternion.LookRotation(hit.normal);

        Debug.Log("Spawning effect: " + effectPrefab.name + " at " + spawnPosition);

        GameObject fx = Instantiate(effectPrefab, spawnPosition, spawnRotation);
        Destroy(fx, hitEffectLifetime);
    }
}