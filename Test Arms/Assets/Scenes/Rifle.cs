using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    public GameObject target;
    void Awake()
    {
        foreach (GameObject item in GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (item.tag == "Target")
            {
                target = item;
                print(item);
            }
        }
    }
    void Update()
    {
        
    }
}
