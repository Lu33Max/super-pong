using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private List<ItemWithProb> itemList;
    [SerializeField] private List<int> bulletBanStages;

    [SerializeField] private int minUsages;
    [SerializeField] private int maxUsages;

    private GameManager gameManager;

    public event EventHandler<CollectedEventArgs> OnCollected;

    public GameObject itemGO;
    public int usages;

    [Serializable]
    internal class ItemWithProb
    {
        public GameObject item;
        public float prob;
    }

    public class CollectedEventArgs
    {
        public Vector2 position;
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Auf Stages mit zu steilen Winkeln kann der Bullet-Ball stecken bleiben, weshalb er bei einer solchen Stage aus der Auswahl entfernt wird
        if(bulletBanStages.Exists(e => e == gameManager.GetSelectedStage()))
        {
            itemList.Remove(itemList.Find(e => e.item.GetComponent<IBall>().BallData.BallName == "Bullet"));
        }

        // Zuerst wird der Wert aller eingestellten Wahrscheinlichkeiten zusammengerechnet.
        float totalChance = 0;

        for(int i = 0; i < itemList.Count; i++)
        {
            totalChance += itemList[i].prob;
        }

        // Es wird eine zufällige Zahl zwischen 0 und der absoluten Wahrscheinlichkeit generiert. 
        float generatedItem = UnityEngine.Random.Range(0, totalChance);

        // Die Wahrscheinlichkeiten jedes Balls werden nun nacheinander addiert und sobald die Gesamtzahl größer als die generierte Zahl ist, wird der Ball an diesem Index als Item ausgewählt.
        float addedChances = 0;

        for(int i = 0; i < itemList.Count; i++)
        {
            addedChances += itemList[i].prob;

            if(addedChances > generatedItem)
            {
                itemGO = itemList[i].item;
                usages = Mathf.FloorToInt(UnityEngine.Random.Range(minUsages, maxUsages));

                gameObject.GetComponent<SpriteRenderer>().sprite = itemGO.GetComponent<IBall>().BallData.BallSprite;

                break;
            }
        }
    }

    private void OnDestroy()
    {
        OnCollected?.Invoke(this, new CollectedEventArgs { position = transform.position });
    }
}
