using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : NetworkBehaviour
{
    public bool start = false;

    private float duration = 1f;
    [SerializeField] private AnimationCurve curve;
    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPostion = transform.position;
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            elaspedTime += Time.deltaTime;
            float strength = curve.Evaluate(elaspedTime / duration);
            transform.position = startPostion + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPostion;
    }
}
