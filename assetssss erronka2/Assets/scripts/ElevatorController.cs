using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public Transform platform;     // <-- arrastra aquí "suelo"
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;

    private bool goingUp;
    private bool moving;

    void Update()
    {
        if (!moving || platform == null) return;

        Transform target = goingUp ? topPoint : bottomPoint;

        platform.position = Vector3.MoveTowards(
            platform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(platform.position, target.position) < 0.01f)
            moving = false;
    }

    public void ToggleElevator()
    {
        goingUp = !goingUp;
        moving = true;
        Debug.Log("ToggleElevator -> " + (goingUp ? "SUBE" : "BAJA"));
    }
}
