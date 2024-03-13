using System;
using UnityEngine;
using static BallSwitch;

/// <summary>
/// Stellt Funktionalit�ten f�r das Hinzuf�gen eingesammelter Items zum Inventar bereit.
/// </summary>
public class ItemCollector : MonoBehaviour
{
    [SerializeField] private PlayerSO playerData;

    public event EventHandler<BallUpdatesEventArgs> OnBallUpdated;
    public event EventHandler<BallAddEventArgs> OnBallAdded;

    private BallController ball;

    public class BallAddEventArgs
    {
        public InventorySlot ballToAdd;
    }

    private void Awake()
    {
        ball = FindObjectOfType<BallController>();

        if (ball != null)
        {
            ball.OnItemCollected += AddItemToInventory;
        }
    }

    /// <summary>
    /// F�gt den �bergebenen Ball mitsamt seiner Anzahl an Nutzen dem Spielerinventar hinzu.
    /// </summary>
    /// <param name="e">playerTag - Tag des Spielers, der das Item eingesammelt hat | item - GameObject des Balls sowie Anzahl der Nutzungen</param>
    private void AddItemToInventory(object sender, BallController.ItemCollectionEventArgs e)
    {
        // F�gt das Item nur hinzu, wenn der �bergebene Tag mit dem eigenem �bereinstimmt
        if (gameObject.CompareTag(e.playerTag))
        {
            // �berpr�ft zun�chst, ob sich bereits ein Ball der gleichen Art im Inventar befindet
            int itemIndex = playerData.Inventory.FindIndex(o => o.Ball.GetComponent<IBall>().BallData.BallName == e.item.Ball.GetComponent<IBall>().BallData.BallName);

            // Wurde ein Eintrag gefunden, so werden dessen Nutzungen entsprechend erh�ht.
            if (itemIndex >= 0)
            {
                playerData.Inventory[itemIndex].Usages += e.item.Usages;
                OnBallUpdated?.Invoke(this, new BallUpdatesEventArgs { index = itemIndex });
            }
            // Wurde kein Eintrag gefunden, wird ein neuer Ball zum Inventar hinzugef�gt.
            else
            {
                playerData.Inventory.Add(e.item);
                OnBallAdded?.Invoke(this, new BallAddEventArgs { ballToAdd = e.item });
            }
        }
    }
}
