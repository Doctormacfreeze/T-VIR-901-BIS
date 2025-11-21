using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Zone")]
    public Vector3 spawnZoneSize = new Vector3(10f, 0f, 8f); // Taille de la zone
    public int maxDummys = 5; // Nombre maximum de dummys

    [Header("Dummy Prefab")]
    public GameObject dummyPrefab; // Préfab du dummy

    [Header("Spawn Settings")]
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;

    private List<TargetDummy> activeDummys = new List<TargetDummy>();
    private List<Vector3> usedPositions = new List<Vector3>(); // CORRIGÉ : ajout de List<Vector3>()

    private void Start()
    {
        // Spawn des dummys initiaux
        for (int i = 0; i < maxDummys; i++)
        {
            SpawnDummy();
        }
    }

    private void SpawnDummy()
    {
        if (dummyPrefab == null)
        {
            Debug.LogError("Dummy Prefab non assigné !");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Instancier le dummy
        GameObject newDummyObj = Instantiate(dummyPrefab, spawnPosition, Quaternion.identity);
        TargetDummy newDummy = newDummyObj.GetComponent<TargetDummy>();

        if (newDummy != null)
        {
            // S'abonner à l'événement de mort du dummy
            newDummy.OnDummyDied += OnDummyDied;
            activeDummys.Add(newDummy);
            usedPositions.Add(spawnPosition);

            Debug.Log($"Dummy spawn à {spawnPosition}");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition;
        int attempts = 0;
        int maxAttempts = 20;

        do
        {
            // Position aléatoire dans la zone
            float x = Random.Range(-spawnZoneSize.x / 2f, spawnZoneSize.x / 2f);
            float z = Random.Range(-spawnZoneSize.z / 2f, spawnZoneSize.z / 2f);

            spawnPosition = transform.position + new Vector3(x, 0f, z);
            attempts++;

            // Éviter les boucles infinies
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Impossible de trouver une position libre après " + maxAttempts + " tentatives");
                break;
            }

        } while (IsPositionTooClose(spawnPosition, 2f)); // Éviter les spawn trop proches

        return spawnPosition;
    }

    private bool IsPositionTooClose(Vector3 position, float minDistance)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDummyDied(TargetDummy dummy)
    {
        // Retirer le dummy de la liste
        activeDummys.Remove(dummy);
        usedPositions.Remove(dummy.transform.position);

        // Désabonner l'événement
        dummy.OnDummyDied -= OnDummyDied;

        // Respawn après un délai aléatoire
        float spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        Invoke(nameof(SpawnDummy), spawnDelay);

        Debug.Log($"Dummy détruit, respawn dans {spawnDelay} secondes");
    }

    // Méthode pour forcer le respawn de tous les dummys
    public void RespawnAllDummys()
    {
        // Détruire tous les dummys existants
        foreach (TargetDummy dummy in activeDummys)
        {
            if (dummy != null)
            {
                dummy.OnDummyDied -= OnDummyDied;
                Destroy(dummy.gameObject);
            }
        }

        activeDummys.Clear();
        usedPositions.Clear();

        // Respawn
        for (int i = 0; i < maxDummys; i++)
        {
            SpawnDummy();
        }
    }

    // Visualisation de la zone de spawn dans l'éditeur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(spawnZoneSize.x, 0.1f, spawnZoneSize.z));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnZoneSize.x, 0.1f, spawnZoneSize.z));
    }
}