
using System.Collections;
using UnityEngine;

public class CameraAreaTransition : MonoBehaviour
{
    public enum TransitionDirection
    {
        Left,
        Right
    }

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 targetPosition;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private TransitionDirection direction;

    private bool isTransitioning;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning || !other.CompareTag("Player"))
            return;

        Rigidbody2D playerRb = other.attachedRigidbody;

        if (playerRb == null)
            return;

        float horizontalVelocity = playerRb.linearVelocity.x;

        // Only allow transitions in the intended direction.
        if (direction == TransitionDirection.Right &&
            horizontalVelocity <= 0f)
            return;

        if (direction == TransitionDirection.Left &&
            horizontalVelocity >= 0f)
            return;

        StartCoroutine(SlideCamera());
    }

    private IEnumerator SlideCamera()
    {
        isTransitioning = true;

        Vector3 startPosition = cameraTransform.position;
        Vector3 endPosition = new Vector3(
            targetPosition.x,
            targetPosition.y,
            startPosition.z
        );

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / transitionDuration
            );

            float smoothProgress =
                progress * progress * (3f - 2f * progress);

            cameraTransform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                smoothProgress
            );

            yield return null;
        }

        cameraTransform.position = endPosition;
        isTransitioning = false;
    }
}
