using System.Collections;
using UnityEngine;

public class HallwayChaseEasy : MonoBehaviour
{
    [Header("Arrastra aquí")]
    public Transform player;
    public Transform monsterRoot;
    public Transform pointB;

    [Header("Movimiento")]
    public float monsterSpeed = 3.5f;
    public float rotateSpeed = 10f;

    [Header("Animación")]
    public string walkingBool = "IsWalking";

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

    void Awake()
    {
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

        SetIdle();
    }

    void OnTriggerEnter(Collider other)
    {
        if (oneShot && triggered) return;
        if (isChasing) return;

        bool isPlayer = (player && other.transform == player) || other.CompareTag("Player");
        if (!isPlayer) return;

        triggered = true;
        StartCoroutine(ChaseRoutine());
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

        SetWalk();

        while (true)
        {
            // mover hacia el player (sin navmesh)
            Vector3 target = player.position;
            target.y = monsterRoot.position.y; // evita subir/bajar raro

            monsterRoot.position = Vector3.MoveTowards(
                monsterRoot.position,
                target,
                monsterSpeed * Time.deltaTime
            );

            // rotar hacia el player
            Vector3 dir = (target - monsterRoot.position);
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                monsterRoot.rotation = Quaternion.Slerp(monsterRoot.rotation, look, rotateSpeed * Time.deltaTime);
            }

            // si llega al punto B -> reset
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
            // Si el player tiene CharacterController, hay que apagarlo para teleport
            var cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            player.position = playerStartPos;
            player.rotation = playerStartRot;

            if (cc) cc.enabled = true;

            // Si tiene Rigidbody, frenar
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
    }

    void SetWalk()
    {
        if (monsterAnimator) monsterAnimator.SetBool(walkingBool, true);
    }

    void SetIdle()
    {
        if (monsterAnimator) monsterAnimator.SetBool(walkingBool, false);
    }
}
