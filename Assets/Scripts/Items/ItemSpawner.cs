using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ist dafür zuständig in zufälligen Abständen an zufälligen Orten zufällige Items erscheinen zu lassen.
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    /// <summary>
    /// Eine Liste an Collidern, die das Gebiet abstecken innerhalb dessen Items erscheinen können.
    /// </summary>
    [SerializeField] private Collider2D[] boundaries;

    [SerializeField] private int maxItemCount;
    [SerializeField] private float minItemDistance;
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;

    private float lastSpawnTime;
    private float spawnTime;

    private GameManager gameManager;

    private List<Vector2> itemPositions = new();

    private void Start()
    {
        // Die letzte Spawn-Zeit und die Zeit bis zum nächsten Spawn werden gesetzt.
        lastSpawnTime = Time.time;
        spawnTime = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);

        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        // Solange die noch läuft wird, sobald die zufällig generierte Wartezeit abgelaufen ist, ein neues Item generiert und eine neue Wartezeit gesetzt.
        if(gameManager.GetGameState() == GameStates.PlayState)
        {
            if (Time.time > lastSpawnTime + spawnTime)
            {
                lastSpawnTime = Time.time;
                spawnTime = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);

                StartCoroutine(SpawnItem());
            }
        }
    }

    /// <summary>
    /// Lässt ein neues Item an einer zufälligen Position erscheinen.
    /// </summary>
    private IEnumerator SpawnItem()
    {
        yield return null;

        // Ein Item kann nur erscheinen, solange weniger als 2 bereits auf dem Feld sind.
        if (itemPositions.Count < maxItemCount)
        {
            Collider2D spawnCollider = null;
            float totalArea = 0;

            // Summiert die gesamte Fläche aller Gebiete
            foreach (var bound in boundaries)
            {
                totalArea += bound.bounds.extents.x * bound.bounds.extents.y;
            }

            // Generiert einen zufälligen Wert zwischen 0 und der summierten Gebietsgröße.
            float random = UnityEngine.Random.Range(0, totalArea);

            // Die Größen aller Gebiete werden nacheinander aufsummiert und sobald die Summe größer als der generierte Wert ist, wird das aktuelle Gebiet benutzt,
            // um darin das Item erscheinen zu lassen.
            float addedArea = 0;

            foreach (var bound in boundaries)
            {
                addedArea += bound.bounds.extents.x * bound.bounds.extents.y;

                if (random <= addedArea)
                {
                    spawnCollider = bound;
                    break;
                }
            }

            if (spawnCollider != null)
            {
                // Ermittelt eine zufällige Position für das Item
                float xPos = UnityEngine.Random.Range(spawnCollider.bounds.min.x, spawnCollider.bounds.max.x);
                float yPos = UnityEngine.Random.Range(spawnCollider.bounds.min.y, spawnCollider.bounds.max.y);

                Vector2 position = new(xPos, yPos);

                int maxTries = 10;
                bool positionFound = itemPositions.Count > 0 ? false : true;

                if (!positionFound)
                {
                    while (maxTries > 0)
                    {
                        for (int i = 0; i < itemPositions.Count; i++)
                        {
                            if (Vector2.Distance(position, itemPositions[i]) < minItemDistance)
                            {
                                break;
                            }

                            if (i == itemPositions.Count - 1)
                            {
                                positionFound = true;
                            }
                        }

                        if (positionFound)
                        {
                            break;
                        }

                        xPos = UnityEngine.Random.Range(spawnCollider.bounds.min.x, spawnCollider.bounds.max.x);
                        yPos = UnityEngine.Random.Range(spawnCollider.bounds.min.y, spawnCollider.bounds.max.y);

                        position = new Vector2(xPos, yPos);

                        maxTries--;
                    }
                }

                if (positionFound)
                {
                    itemPositions.Add(position);

                    // Instanziiert das neue Item an der generierten Position und weißt das OnCollected-Event zu
                    var newItem = Instantiate(itemPrefab, position, itemPrefab.transform.rotation);
                    newItem.GetComponent<Item>().OnCollected += ClearReference;
                }
                else
                {
                    Debug.Log("No position for item found.");
                }
            }
            else
            {
                Debug.LogError("No Collider found to spawn item.");
            }
        }
    }

    private void ClearReference(object sender, Item.CollectedEventArgs e)
    {
        itemPositions.Remove(e.position);
    }
}
