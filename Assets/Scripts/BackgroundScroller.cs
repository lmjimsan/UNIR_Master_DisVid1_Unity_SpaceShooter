using UnityEngine;

// Mueve el objeto entero suavemente (para imágenes únicas como planetas)
public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] Vector2 moveAmount = new Vector2(0.5f, 0.3f); // Cuánto se mueve en cada dirección
    [SerializeField] float moveSpeed = 0.2f; // Velocidad del movimiento
    [SerializeField] bool useUnscaledTime = true;

    Vector3 startPosition;
    float timeOffset;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 100f); // Offset aleatorio para que cada fondo sea diferente
    }

    void Update()
    {
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        timeOffset += deltaTime * moveSpeed;

        // Movimiento circular/orbital suave
        float offsetX = Mathf.Sin(timeOffset) * moveAmount.x;
        float offsetY = Mathf.Cos(timeOffset * 0.7f) * moveAmount.y;

        transform.position = startPosition + new Vector3(offsetX, offsetY, 0f);
    }
}
