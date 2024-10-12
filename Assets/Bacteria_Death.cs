using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bacteria_Death : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SelfDestroy());
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }
}
