using System.Collections;
using UnityEngine;

public class HallwayChaseEasy : MonoBehaviour
{
    [Header("Arrastra aquí")]
    public Transform player;
    public Transform monsterRoot;   // Objeto que tiene el Animator
    public Transform pointB;

    [Header("Movimiento")]
    public float monsterSpeed = 3.5f;
    public float rotateSpeed = 10f;

    [Header("Animación")]
    public string runningBool = "IsRunning";      // <- ahora usamos IsRunning
    public string startIdleStateName = "idle";    // <- pon EXACTO el nombre del estado idle (en tu captura es "idle")

    [Header("Audio (Susto)")]
    public AudioSource scareAudioSource;
    public AudioClip scareClip;
    [Range(0f, 1f)] public float scareVolume = 1f;
    public bool playScareSoundOnce = true;

    [Header("Reset")]
    public float reachDistanceToB = 0.6f;
    public float resetDelay = 0.2f;
    public bool oneShot = false;

    Animator monsterAnimator;

    Vector3 playerStartPos;
    Quaternion playerStartRot;
    Vector3 monsterStartPos;
    Quaternion monsterStartRot;

    bool isChasing = false;
    bool triggered = false;
    bool scareSoundPlayed = false;

    int runningBoolHash;

    void Awake()
    {
        runningBoolHash = Animator.StringToHash(runningBool);

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (monsterRoot)
        {
            monsterAnimator = monsterRoot.GetComponentInChildren<Animator>(true);
            monsterStartPos = monsterRoot.position;
            monsterStartRot = monsterRoot.rotation;
        }

        if (player)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
        }
    }

    void Start()
    {
        if (monsterAnimator)
        {
            monsterAnimator.Rebind();
            monsterAnimator.Update(0f);

            monsterAnimator.SetBool(runningBoolHash, false);
            monsterAnimator.Play(startIdleStateName, 0, 0f);
            monsterAnimator.Update(0f);
        }
        else
        {
            Debug.LogError("[HallwayChaseEasy] No encuentro Animator. Revisa monsterRoot (tiene que ser el objeto con Animator).");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (oneShot && triggered) return;
        if (isChasing) return;

        bool isPlayer = (player && other.transform == player) || other.CompareTag("Player");
        if (!isPlayer) return;

        triggered = true;

        PlayScareSound();
        StartCoroutine(ChaseRoutine());
    }

    void PlayScareSound()
    {
        if (!scareAudioSource || !scareClip) return;
        if (playScareSoundOnce && scareSoundPlayed) return;

        scareAudioSource.PlayOneShot(scareClip, scareVolume);
        scareSoundPlayed = true;
    }

    IEnumerator ChaseRoutine()
    {
        isChasing = true;

        if (!player || !monsterRoot || !pointB)
        {
            Debug.LogError("[HallwayChaseEasy] Faltan referencias (player/monsterRoot/pointB).");
            isChasing = false;
            yield break;
        }

        SetRun();

        while (true)
        {
            Vector3 target = player.position;
            target.y = monsterRoot.position.y;

            monsterRoot.position = Vector3.MoveTowards(
                monsterRoot.position,
                target,
                monsterSpeed * Time.deltaTime
            );

            Vector3 dir = target - monsterRoot.position;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                monsterRoot.rotation = Quaternion.Slerp(monsterRoot.rotation, look, rotateSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(monsterRoot.position, pointB.position) <= reachDistanceToB)
                break;

            yield return null;
        }

        SetIdle();
        yield return new WaitForSeconds(resetDelay);

        ResetPositions();

        isChasing = false;
        if (!oneShot) triggered = false;
    }

    void ResetPositions()
    {
        if (player)
        {
            var cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            player.position = playerStartPos;
            player.rotation = playerStartRot;

            if (cc) cc.enabled = true;

            var rb = player.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (monsterRoot)
        {
            monsterRoot.position = monsterStartPos;
            monsterRoot.rotation = monsterStartRot;
        }

        SetIdle();

        if (monsterAnimator)
        {
            monsterAnimator.Play(startIdleStateName, 0, 0f);
            monsterAnimator.Update(0f);
        }
    }

    void SetRun()
    {
        if (monsterAnimator) monsterAnimator.SetBool(runningBoolHash, true);
    }

    void SetIdle()
    {
        if (monsterAnimator) monsterAnimator.SetBool(runningBoolHash, false);
    }
}
