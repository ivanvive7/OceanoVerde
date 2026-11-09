using UnityEngine;

/// <summary>
/// La cámara empieza en el centro de la escena y solo sigue al jugador en vertical.
/// Se añade a la Main Camera.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;                 // el jugador

    [Header("Ajustes")]
    public float yOffset = 1f;               // la cámara va un poco por encima del jugador
    public float smoothSpeed = 5f;           // 0 = sin suavizado (instantáneo)

    private float startX;
    private float startZ;

    private void Start()
    {
        // Guardamos la posición inicial para mantener fijo el eje que no se sigue
        startX = transform.position.x;
        startZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float targetY = Mathf.Clamp(target.position.y + yOffset, -0.5f, 1.5f);
        Vector3 desired = new Vector3(startX, targetY, startZ);

        if (smoothSpeed <= 0f)
            transform.position = desired;
        else
            transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}
