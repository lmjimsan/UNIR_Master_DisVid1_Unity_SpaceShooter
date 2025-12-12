using UnityEngine;

public class PlayerShoot_Type1 : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float destroyPosition = 20f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        if(transform.position.x > destroyPosition)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.gameObject.CompareTag("PlayerProjectile"))
        {
            // Se destruye, siempre que el proyectil no impacte con proyectiles o naves "amigas"
            Destroy(gameObject);
        }        

    }
}
