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
    [Tooltip("Defaults to the main camera when left empty.")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 targetPosition;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private TransitionDirection direction;

    private bool isTransitioning;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning || cameraTransform == null || !other.CompareTag("Player"))
        {
            return;
        }

        Rigidbody2D playerRb = other.attachedRigidbody;
        if (playerRb == null || !IsMovingInTransitionDirection(playerRb.linearVelocity.x))
        {
            return;
        }

        StartCoroutine(SlideCamera());
    }

    private bool IsMovingInTransitionDirection(float horizontalVelocity)
    {
        return direction == TransitionDirection.Right
            ? horizontalVelocity > 0f
            : horizontalVelocity < 0f;
    }

    private IEnumerator SlideCamera()
    {
        isTransitioning = true;

        Vector3 startPosition = cameraTransform.position;
        // Keep the camera's own depth so it doesn't clip through the scene.
        Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, startPosition.z);

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            cameraTransform.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return null;
        }

        cameraTransform.position = endPosition;
        isTransitioning = false;
    }
}
