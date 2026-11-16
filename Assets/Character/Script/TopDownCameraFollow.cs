using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -7f);
    [SerializeField] private float followSmoothTime = 0.15f;
    [SerializeField] private float lookHeight = 1f;

    [Header("Camera Rotation")]
    [SerializeField] private float turnDuration = 0.35f;

    private Vector3 followVelocity;
    private int quarterTurns;
    private float currentTurnAngle;
    private float turnStartAngle;
    private float turnTargetAngle;
    private float turnProgress;
    private bool isTurning;

    private void Update()
    {
        if (target == null)
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            turnStartAngle = currentTurnAngle;
            turnTargetAngle = currentTurnAngle + 90f;
            turnProgress = 0f;
            isTurning = true;
            quarterTurns++;
        }

        if (isTurning)
        {
            turnProgress += Time.deltaTime / Mathf.Max(turnDuration, 0.01f);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, turnProgress);

            currentTurnAngle = Mathf.Lerp(
                turnStartAngle,
                turnTargetAngle,
                smoothProgress
            );

            if (turnProgress >= 1f)
            {
                currentTurnAngle = turnTargetAngle;
                isTurning = false;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 rotatedOffset = Quaternion.Euler(0f, currentTurnAngle, 0f) * offset;
        Vector3 targetPosition = target.position + rotatedOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref followVelocity,
            followSmoothTime
        );

        Vector3 lookTarget = target.position + Vector3.up * lookHeight;
        transform.rotation = Quaternion.LookRotation(lookTarget - transform.position);
    }
}