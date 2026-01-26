using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;

    private bool goingUp;
    private bool moving;

    void Update()
    {
        if (!moving) return;

        Transform target = goingUp ? topPoint : bottomPoint;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
            moving = false;
    }

    public void ToggleElevator()
    {
        goingUp = !goingUp;
        moving = true;
    }
}
