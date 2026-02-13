using UnityEngine;

public class Enemy_NoNavMesh : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;              // XR Origin (o el root del player)
    public Transform respawnPlayer;       // Punto de respawn (Empty en la escena)
    public Transform enemyResetPoint;     // (opcional) punto para reset enemigo si quieres

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

    [Header("Respawn (cuando toca al player)")]
    public bool respawnOnTouch = true;
    public bool resetEnemyAfterTouch = false;
    public bool disablePlayerControllerDuringRespawn = true;

    [Tooltip("Tiempo mínimo entre respawns (evita spam y hace que funcione siempre)")]
    public float respawnCooldown = 0.6f;

    [Tooltip("Sube un pelín al respawn para evitar quedarse dentro de colliders")]
    public float respawnPushUp = 0.05f;

    [Header("Muerte")]
    public bool freezeCompletelyOnDeath = true;
    public bool disableControllerOnDeath = true;
    public bool hideAfterDeath = false;

    int wpIndex = 0;
    bool chasing = false;
    bool waiting = false;
    bool dead = false;

    int walkHash;
    int deadHash;

    Vector3 deadPos;
    Quaternion deadRot;

    // ✅ control para que se pueda repetir
    bool playerInside = false;
    float nextRespawnTime = 0f;

    void Start()
    {
        if (!controller) controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        walkHash = Animator.StringToHash(isWalkingBool);
        deadHash = Animator.StringToHash(muertoBool);

        if (animator)
        {
            animator.SetBool(deadHash, false);
            animator.SetBool(walkHash, false);
            animator.applyRootMotion = false;
        }

        if (!player) Debug.LogWarning("[Enemy_NoNavMesh] Falta asignar Player en el Inspector.", this);
        if (!respawnPlayer) Debug.LogWarning("[Enemy_NoNavMesh] Falta asignar Respawn Player en el Inspector.", this);
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
        if (waypoints == null || waypoints.Length == 0)
        {
            if (animator) animator.SetBool(walkHash, false);
            return;
        }

        Transform target = waypoints[wpIndex];
        if (!target) return;

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
        if (!controller) return;

        Vector3 dir = target - transform.position;
        dir.y = 0;

        if (dir.magnitude < 0.01f)
        {
            if (animator) animator.SetBool(walkHash, false);
            return;
        }

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

        Vector3 move = dir.normalized * speed * Time.deltaTime;
        move.y = -2f * Time.deltaTime;

        controller.Move(move);

        if (animator) animator.SetBool(walkHash, true);
    }

    // ✅ Detecta entrada
    private void OnTriggerEnter(Collider other)
    {
        if (!respawnOnTouch || dead) return;
        if (!player || !respawnPlayer) return;

        if (IsPlayer(other))
        {
            playerInside = true;
            TryRespawn();
        }
    }

    // ✅ Detecta estar dentro (para que funcione aunque no “salga” perfecto)
    private void OnTriggerStay(Collider other)
    {
        if (!respawnOnTouch || dead) return;
        if (!player || !respawnPlayer) return;

        if (playerInside && IsPlayer(other))
        {
            TryRespawn();
        }
    }

    // ✅ Rearma cuando sale
    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            playerInside = false;
        }
    }

    bool IsPlayer(Collider other)
    {
        Transform root = other.transform.root;
        return other.transform == player || root == player;
    }

    void TryRespawn()
    {
        if (Time.time < nextRespawnTime) return;
        nextRespawnTime = Time.time + respawnCooldown;

        RespawnPlayer();

        if (resetEnemyAfterTouch && enemyResetPoint)
        {
            ResetEnemyToPoint(enemyResetPoint.position, enemyResetPoint.rotation);
        }

        // ✅ importante: lo marcamos como fuera para que el siguiente toque cuente
        playerInside = false;
    }

    void RespawnPlayer()
    {
        CharacterController playerCC = player.GetComponent<CharacterController>();
        if (!playerCC) playerCC = player.GetComponentInChildren<CharacterController>();

        if (disablePlayerControllerDuringRespawn && playerCC) playerCC.enabled = false;

        player.position = respawnPlayer.position + Vector3.up * respawnPushUp;
        player.rotation = respawnPlayer.rotation;

        if (disablePlayerControllerDuringRespawn && playerCC) playerCC.enabled = true;
    }

    void ResetEnemyToPoint(Vector3 pos, Quaternion rot)
    {
        if (controller) controller.enabled = false;

        transform.position = pos;
        transform.rotation = rot;

        if (controller) controller.enabled = true;

        chasing = false;
        waiting = false;

        if (animator)
            animator.SetBool(walkHash, false);
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
