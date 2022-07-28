using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Arms : MonoBehaviour
{
    public GameObject rifle;
    public TwoBoneIKConstraint mover;
    private GameObject target;
    private Rifle gun;

    void Awake()
    {
        rifle = Instantiate(rifle, gameObject.transform.position, Quaternion.identity);
        gun = rifle.GetComponent<Rifle>();
        mover.data.target = gun.target.transform;
    }

    void Start()
    {
       
    }
}