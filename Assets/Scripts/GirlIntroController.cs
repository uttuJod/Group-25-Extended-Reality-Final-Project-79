using UnityEngine;
using System.Collections;

public class GirlIntroController : MonoBehaviour
{
    [Header("References")]
    public Transform playerTarget;
    public WaveManager waveManager;
    public Animator animator;

    [Header("Detection")]
    public float detectDistance = 8f;

    [Header("Movement")]
    public float moveSpeed = 1.8f;
    public float stopDistance = 0.5f;
    public float rotationSpeed = 8f;
    public bool facePlayerAlways = true;

    [Header("Follow Position")]
    public float leftOffset = 1.2f;
    public float forwardOffset = 2.2f;
    public bool keepFollowingPlayer = true;

    [Header("Animator Parameters")]
    public string waveTriggerName = "Wave";
    public string walkBoolName = "isWalking";
    public float waveDuration = 1.5f;

    GirlHealth girlHealth;

    bool hasSeenPlayer = false;
    bool hasStartedSequence = false;
    bool isWalkingNow = false;
    bool introFinished = false;
    bool waveStarted = false;

    void Start()
    {
        if (playerTarget == null && Camera.main != null)
            playerTarget = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        girlHealth = GetComponent<GirlHealth>();
        SetWalking(false);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (girlHealth != null && girlHealth.IsDead)
        {
            SetWalking(false);
            return;
        }

        if (playerTarget == null)
            return;

        if (facePlayerAlways)
            FacePlayer();

        Vector3 playerFlat = playerTarget.position;
        playerFlat.y = transform.position.y;

        Vector3 toPlayer = playerFlat - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (!hasSeenPlayer)
        {
            if (distanceToPlayer <= detectDistance)
            {
                hasSeenPlayer = true;

                if (!hasStartedSequence)
                    StartCoroutine(IntroSequence());
            }

            return;
        }

        if (!isWalkingNow && !introFinished)
            return;

        if (introFinished && !keepFollowingPlayer)
            return;

        FollowPlayerSidePosition();
    }

    IEnumerator IntroSequence()
    {
        hasStartedSequence = true;

        if (animator != null && !string.IsNullOrEmpty(waveTriggerName))
            animator.SetTrigger(waveTriggerName);

        yield return new WaitForSeconds(waveDuration);

        isWalkingNow = true;
        SetWalking(true);
    }

    void FollowPlayerSidePosition()
    {
        Vector3 desiredPos = GetDesiredFollowPosition();
        Vector3 toDesired = desiredPos - transform.position;
        toDesired.y = 0f;

        float dist = toDesired.magnitude;

        if (dist > stopDistance)
        {
            Vector3 dir = toDesired.normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            SetWalking(true);
        }
        else
        {
            SetWalking(false);

            if (!introFinished)
            {
                introFinished = true;
                isWalkingNow = false;

                if (!waveStarted && waveManager != null)
                {
                    waveStarted = true;
                    waveManager.StartIntroWaveFlow();
                }
            }
        }
    }

    Vector3 GetDesiredFollowPosition()
    {
        Vector3 camForward = playerTarget.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camLeft = -playerTarget.right;
        camLeft.y = 0f;
        camLeft.Normalize();

        Vector3 desiredPos = playerTarget.position
                           + camLeft * leftOffset
                           + camForward * forwardOffset;

        desiredPos.y = transform.position.y;
        return desiredPos;
    }

    void FacePlayer()
    {
        if (playerTarget == null)
            return;

        Vector3 lookPos = playerTarget.position;
        lookPos.y = transform.position.y;

        Vector3 lookDir = lookPos - transform.position;

        if (lookDir.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    void SetWalking(bool walking)
    {
        if (animator != null && !string.IsNullOrEmpty(walkBoolName))
            animator.SetBool(walkBoolName, walking);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectDistance);

        if (playerTarget != null)
        {
            Vector3 camForward = playerTarget.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camLeft = -playerTarget.right;
            camLeft.y = 0f;
            camLeft.Normalize();

            Vector3 desiredPos = playerTarget.position
                               + camLeft * leftOffset
                               + camForward * forwardOffset;

            desiredPos.y = transform.position.y;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(desiredPos, 0.25f);
        }
    }
}