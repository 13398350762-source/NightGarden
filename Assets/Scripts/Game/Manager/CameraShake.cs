using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static bool Enabled = true;

    private const float duration = 0.15f;
    private const float strength = 0.12f;
    private Vector3 offset;
    private float remaining;

    public static void ShakeMainCamera()
    {
        if (!Enabled)
            return;

        Camera camera = Camera.main;
        if (camera == null)
            return;

        if (!camera.TryGetComponent(out CameraShake shake))
            shake = camera.gameObject.AddComponent<CameraShake>();
        if (shake.isActiveAndEnabled && shake.remaining <= 0f)
            shake.remaining = duration;
    }

    private void LateUpdate()
    {
        if (remaining <= 0f)
            return;

        transform.localPosition -= offset;
        offset = Vector3.zero;
        remaining = Enabled ? Mathf.Max(0f, remaining - Time.unscaledDeltaTime) : 0f;
        if (remaining <= 0f)
            return;

        offset = (Vector3)(Random.insideUnitCircle * (strength * remaining / duration));
        transform.localPosition += offset;
    }

    private void OnDisable()
    {
        transform.localPosition -= offset;
        offset = Vector3.zero;
        remaining = 0f;
    }
}
