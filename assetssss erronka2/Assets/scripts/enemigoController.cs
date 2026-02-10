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

    [Header("Audio - DOS AudioSource")]
    public AudioSource walkSource;       // SOLO pasos
    public AudioClip footstepsClip;
    [Range(0f, 1f)] public float footstepsVolume = 0.8f;

    public AudioSource scareSource;      // SOLO susto
    public AudioClip scareClip;
    [Range(0f, 1f)] public float scareVolume = 1f;

    [Header("Touch settings")]
    public float touchCooldown = 1f;

    int wpIndex = 0;
    bool chasing = false;
    bool waiting = false;
    bool touchLocked = false;

    int walkHash;

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

        // Config pasos
        if (walkSource)
        {
            walkSource.playOnAwake = false;
            walkSource.loop = true;
            walkSource.spatialBlend = 1f; // 3D
            walkSource.clip = footstepsClip;
            walkSource.volume = footstepsVolume;
        }

        // Config susto
        if (scareSource)
        {
            scareSource.playOnAwake = false;
            scareSource.loop = false;
            scareSource.spatialBlend = 1f; // 3D
            scareSource.volume = scareVolume;
        }
    }

    void Update()
    {
        if (!player || !controller) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!chasing && dist <= chaseDistance) chasing = true;
        else if (chasing && dist >= loseDistance) chasing = false;

        if (chasing)
            MoveTowards(player.position, chaseSpeed);
        else
            Patrol();
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
            StartCoroutine(WaitAndNext());
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

        // “pegar al suelo” con CharacterController
        move.y = -2f * Time.deltaTime;

        controller.Move(move);

        StartWalking();
    }

    void StartWalking()
    {
        if (animator) animator.SetBool(walkHash, true);

        if (walkSource && footstepsClip)
        {
            if (walkSource.clip != footstepsClip) walkSource.clip = footstepsClip;
            walkSource.volume = footstepsVolume;

            if (!walkSource.isPlaying)
                walkSource.Play();
        }
    }

    void StopWalking()
    {
        if (animator) animator.SetBool(walkHash, false);

        if (walkSource && walkSource.isPlaying)
            walkSource.Stop();
    }

    IEnumerator WaitAndNext()
    {
        waiting = true;
        StopWalking();

        yield return new WaitForSeconds(waitAtWaypoint);

        wpIndex = (wpIndex + 1) % waypoints.Length;
        waiting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (touchLocked) return;

        if (!other.CompareTag("Player")) return;

        StartCoroutine(TouchCooldownRoutine());

        // Susto AL TOCAR
        PlayScareSound();

        // Respawn del player
        RespawnThePlayer();

        // Reset enemigo a WP1 (enemyResetPoint)
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

        // reproduce susto sin cortar pasos (son dos AudioSource distintos)
        scareSource.volume = scareVolume;
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

        if (!enemyResetPoint) return;

        controller.enabled = false;
        transform.position = enemyResetPoint.position;
        transform.rotation = enemyResetPoint.rotation;
        controller.enabled = true;

        wpIndex = 0;
        waiting = false;
        chasing = false;
    }
}
