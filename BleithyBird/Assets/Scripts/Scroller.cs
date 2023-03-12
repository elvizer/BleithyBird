using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    [SerializeField] private GameObject[] backGrounds = new GameObject[5];

    [SerializeField] private GameObject clone;

    private float scrollSpeed = -3f;

    void Update()
    {
        transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime, Space.World);
        if (transform.position.x <= -20.53f)
        {
            Destroy(clone);
            transform.position = new Vector3(17.67f, Camera.main.transform.position.y, 0);
            SpawnNew();
        }
    }

    void SpawnNew()
    {
        GameObject copy = Instantiate(backGrounds[Random.Range(0, backGrounds.Length)], transform.position, Quaternion.identity);
        copy.transform.parent = transform;
        clone = copy;
    }
}
