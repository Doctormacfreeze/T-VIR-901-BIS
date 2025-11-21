using UnityEngine;

public class VRProjectile : MonoBehaviour
{
    public float speed = 20f;      // Vitesse de la balle
    public float lifetime = 5f;    // Dur�e avant destruction

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Propulsion de la balle dans la direction du forward du muzzle
        rb.linearVelocity = transform.forward * speed;

        // D�truire la balle apr�s 'lifetime' secondes
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ici tu peux g�rer les impacts, d�g�ts, effets etc.
        // Par exemple : Debug.Log("Impact sur : " + collision.gameObject.name);

        // D�truire la balle au contact
        Destroy(gameObject);
    }
}
