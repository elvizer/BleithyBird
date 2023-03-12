using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;
    private GameObject spawnedObj;

    private float spawnRate = 3f;
    private float maxHeight = 2f;
    private float minHeight = -2f;

    private float timeElapsed;
    private bool canSpawn = true;
    private void Update()
    {
        if (!base.IsServer) return;
        Spawn();
    }

    private void Spawn()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > spawnRate)
            canSpawn = true;
        else
            canSpawn = false;

        if (canSpawn)
        {
            timeElapsed = 0f;
            GameObject pipes = Instantiate(prefab, transform.position, Quaternion.identity);
            base.Spawn(pipes);
            pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
        }
    }
}
