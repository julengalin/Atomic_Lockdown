using UnityEngine;
using System.Collections;

public class Enemy_NoNavMesh : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform respawnPlayer;
    public Transform enemyResetPoint;

    public CharacterController controller;
    public Animator animator;

    [Header("Waypoints")]
    public Transform[] waypoints;
    public float waitAtWaypoint = 0.5f;

    [Header("Movimiento")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.5f;
    public float reachDistance = 0.4f;
    public float rotateSpeed = 10f;

    [Header("Distancias")]
    public float chaseDistance = 4f;
    public float loseDistance = 6f;

    [Header("Animación")]
    public string isWalkingBool = "IsWalking";
    public string muertoBool = "muerto";

    [Header("Audio - DOS AudioSource")]
    public AudioSource walkSource;
    public AudioClip footstepsClip;
    [Range(0f, 1f)] public float footstepsVolume = 0.8f;

    public AudioSource scareSource;
    public AudioClip scareClip;
    [Range(0f, 1f)] public float scareVolume = 1f;

    [Header("Touch settings")]
    public float touchCooldown = 1f;

    [Header("Muerte")]
    public bool dieOnTouchPlayer = false;
    public bool freezeCompletelyOnDeath = true;
    public bool disableControllerOnDeath = true;

    [Header("Desaparecer al morir")]
    public bool hideAfterDeath = true;
    public float hideDelay = 2.0f;

    int wpIndex = 0;
    bool chasing = false;
    bool waiting = false;
    bool touchLocked = false;
    bool dead = false;

    int walkHash;
    int deadHash;

    Coroutine waitRoutine;

    Vector3 deadPos;
    Quaternion deadRot;

    void Start()
    {
        if (!controller) controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        walkHash = Animator.StringToHash(isWalkingBool);
        deadHash = Animator.StringToHash(muertoBool);

        if (animator)
        {
            animator.SetBool(deadHash, false);
            animator.SetBool(walkHash, false);
            animator.applyRootMotion = false;
        }

        if (walkSource)
        {
            walkSource.playOnAwake = false;
            walkSource.loop = true;
            walkSource.spatialBlend = 1f;
            walkSource.clip = footstepsClip;
            walkSource.volume = footstepsVolume;
        }

        if (scareSource)
        {
            scareSource.playOnAwake = false;
            scareSource.loop = false;
            scareSource.spatialBlend = 1f;
            scareSource.volume = scareVolume;
        }
    }

    void Update()
    {
        if (dead) return;
        if (!player || !controller) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!chasing && dist <= chaseDistance) chasing = true;
        else if (chasing && dist >= loseDistance) chasing = false;

        if (chasing) MoveTowards(player.position, chaseSpeed);
        else Patrol();
    }

    void LateUpdate()
    {
        if (!dead) return;
        if (!freezeCompletelyOnDeath) return;

        transform.position = deadPos;
        transform.rotation = deadRot;

        if (animator) animator.SetBool(walkHash, false);
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0 || waypoints[wpIndex] == null)
        {
            StopWalking();
            return;
        }

        Transform target = waypoints[wpIndex];
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= reachDistance && !waiting)
        {
            if (waitRoutine != null) StopCoroutine(waitRoutine);
            waitRoutine = StartCoroutine(WaitAndNext());
            return;
        }

        MoveTowards(target.position, patrolSpeed);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;

        if (dir.magnitude < 0.01f)
        {
            StopWalking();
            return;
        }

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

        Vector3 move = dir.normalized * speed * Time.deltaTime;
        move.y = -2f * Time.deltaTime;

        controller.Move(move);
        StartWalking();
    }

    void StartWalking()
    {
        if (dead) return;

        if (animator) animator.SetBool(walkHash, true);

        if (walkSource && footstepsClip && !walkSource.isPlaying)
        {
            walkSource.clip = footstepsClip;
            walkSource.volume = footstepsVolume;
            walkSource.Play();
        }
    }

    void StopWalking()
    {
        if (animator) animator.SetBool(walkHash, false);
        if (walkSource && walkSource.isPlaying) walkSource.Stop();
    }

    IEnumerator WaitAndNext()
    {
        waiting = true;
        StopWalking();

        yield return new WaitForSeconds(waitAtWaypoint);

        if (dead) yield break;

        wpIndex = (wpIndex + 1) % waypoints.Length;
        waiting = false;
    }

    // ✅ ENTRA
    void OnTriggerEnter(Collider other)
    {
        TryTouchPlayer(other);
    }

    // ✅ SE QUEDA DENTRO (esto arregla que la 2ª vez no funcione)
    void OnTriggerStay(Collider other)
    {
        TryTouchPlayer(other);
    }

    void TryTouchPlayer(Collider other)
    {
        if (dead) return;
        if (touchLocked) return;

        // ✅ detecta al player aunque el collider sea de un hijo
        if (!player) return;
        if (other.transform != player && other.transform.root != player.root) return;

        StartCoroutine(TouchCooldownRoutine());

        if (dieOnTouchPlayer)
        {
            Die();
            return;
        }

        PlayScareSound();
        RespawnThePlayer();
        ResetEnemy();
        chasing = false;
    }


    IEnumerator TouchCooldownRoutine()
    {
        touchLocked = true;
        yield return new WaitForSeconds(touchCooldown);
        touchLocked = false;
    }

    void PlayScareSound()
    {
        if (!scareSource || !scareClip) return;
        scareSource.PlayOneShot(scareClip, scareVolume);
    }

    void RespawnThePlayer()
    {
        if (!player || !respawnPlayer) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        player.position = respawnPlayer.position;
        player.rotation = respawnPlayer.rotation;

        if (cc) cc.enabled = true;
    }

    void ResetEnemy()
    {
        StopWalking();

        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waitRoutine = null;
        }

        if (!enemyResetPoint) return;

        if (controller && !controller.enabled) controller.enabled = true;

        controller.enabled = false;
        transform.position = enemyResetPoint.position;
        transform.rotation = enemyResetPoint.rotation;
        controller.enabled = true;

        wpIndex = 0;
        waiting = false;
        chasing = false;

        if (animator)
        {
            animator.SetBool(deadHash, false);
            animator.SetBool(walkHash, false);
            animator.applyRootMotion = false;
        }

        dead = false;
    }

    public void Die()
    {
        if (dead) return;
        dead = true;

        deadPos = transform.position;
        deadRot = transform.rotation;

        chasing = false;
        waiting = false;

        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waitRoutine = null;
        }

        StopWalking();

        if (animator)
        {
            animator.applyRootMotion = false;
            animator.SetBool(walkHash, false);
            animator.SetBool(deadHash, true);
        }

        if (disableControllerOnDeath && controller)
            controller.enabled = false;

        if (hideAfterDeath)
            StartCoroutine(HideAfterSeconds());
    }

    IEnumerator HideAfterSeconds()
    {
        yield return new WaitForSeconds(hideDelay);
        gameObject.SetActive(false);
    }
}
