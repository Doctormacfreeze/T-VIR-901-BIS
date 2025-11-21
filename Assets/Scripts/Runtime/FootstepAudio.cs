using UnityEngine;

public class SimpleFootsteps : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    public AudioSource audioSource;
    public float stepDistance = 1.5f;
    public float firstStepMultiplier = 0.7f;

    private Vector3 lastPosition;
    private float distanceCovered;
    private bool isFirstStepAfterStop = true;
    private Transform xrCamera;

    void Start()
    {
        lastPosition = transform.position;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Récupère la caméra XR (VR)
        xrCamera = Camera.main != null ? Camera.main.transform : transform;

        Debug.Log("Caméra XR trouvée: " + (xrCamera != null));
    }

    void Update()
    {
        // Utilise la position de la caméra VR pour plus de précision
        Vector3 currentPosition = xrCamera != null ? xrCamera.position : transform.position;

        // Calcule uniquement le déplacement horizontal
        Vector3 movement = currentPosition - lastPosition;
        Vector3 horizontalMovement = new Vector3(movement.x, 0f, movement.z);
        float horizontalDistance = horizontalMovement.magnitude;

        lastPosition = currentPosition;

        if (horizontalDistance > 0.01f)
        {
            distanceCovered += horizontalDistance;
            float requiredDistance = isFirstStepAfterStop ?
                stepDistance * firstStepMultiplier :
                stepDistance;

            if (distanceCovered >= requiredDistance)
            {
                PlayFootstep();
                distanceCovered = 0f;
                isFirstStepAfterStop = false;
            }
        }
        else
        {
            // Reset si arrêt du joueur
            isFirstStepAfterStop = true;
            distanceCovered = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds.Length == 0 || audioSource == null) return;

        AudioClip randomClip = footstepSounds[Random.Range(0, footstepSounds.Length)];

        // Variation naturelle
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = Random.Range(0.8f, 1.0f);

        audioSource.PlayOneShot(randomClip);

        Debug.Log("Bruit de pas joué");
    }
}
