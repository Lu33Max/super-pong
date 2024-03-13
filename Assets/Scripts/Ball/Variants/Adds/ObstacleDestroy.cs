using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime = 5f;

    private float spawnTime;

    private void Awake()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        if(Time.time > spawnTime + destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
