using UnityEngine;

public class EnemyShoot_Type1 : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float destroyPosition = -12f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if(transform.position.x < destroyPosition)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!(collision.gameObject.CompareTag("EnemyProjectile") || collision.gameObject.CompareTag("Enemy")))
        {
            // Se destruye, siempre que el proyectil no impacte con proyectiles o naves "amigas"
            Destroy(gameObject);
        }        

    }
}
