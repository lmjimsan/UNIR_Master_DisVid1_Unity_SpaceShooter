using UnityEngine;

public class Fondo3Movement : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] Vector3 direction = new Vector3(-1f, 0f, 0f);
    [SerializeField] float widthImage;
    Vector3 startPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Resto: Cuanto me queda de recorrido para alcanzar el nuevo ciclo
        float rest = (speed * Time.time) % widthImage;

        // Mi posición se va refrescando desde la inicial SUMANDO tanto como resto me quede
        // en la dirección deseada
        transform.position = startPosition + rest * direction;
    }
}
