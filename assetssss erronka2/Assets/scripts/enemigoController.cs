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

    [Header("Muerte")]
    public bool freezeCompletelyOnDeath = true;
    public bool disableControllerOnDeath = true;
    public bool hideAfterDeath = false; // ⚠️ Lo controlamos desde EnemigoHit

    int wpIndex = 0;
    bool chasing = false;
    bool waiting = false;
    bool dead = false;

    int walkHash;
    int deadHash;

    Vector3 deadPos;
    Quaternion deadRot;

    void Start()
    {
        if (!controller) controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        walkHash = Animator.StringToHash(isWalkingBool);
        deadHash = Animator.StringToHash(muertoBool);

        animator.SetBool(deadHash, false);
        animator.SetBool(walkHash, false);
        animator.applyRootMotion = false;
    }

    void Update()
    {
        if (dead) return;
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
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[wpIndex];
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= reachDistance)
        {
            wpIndex = (wpIndex + 1) % waypoints.Length;
            return;
        }

        MoveTowards(target.position, patrolSpeed);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;

        if (dir.magnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

        Vector3 move = dir.normalized * speed * Time.deltaTime;
        move.y = -2f * Time.deltaTime;

        controller.Move(move);

        animator.SetBool(walkHash, true);
    }

    public void Die()
    {
        if (dead) return;

        dead = true;

        deadPos = transform.position;
        deadRot = transform.rotation;

        chasing = false;
        waiting = false;

        if (animator)
        {
            animator.SetBool(walkHash, false);
            animator.SetBool(deadHash, true);
        }

        if (disableControllerOnDeath && controller)
            controller.enabled = false;
    }
}
