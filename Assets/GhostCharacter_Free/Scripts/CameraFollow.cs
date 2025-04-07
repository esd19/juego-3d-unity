using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El personaje que la cámara va a seguir
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Posición relativa a tu personaje
    public float smoothSpeed = 0.125f; // Suavidad del movimiento

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            transform.LookAt(target); // Para que la cámara mire al personaje
        }
    }
}
