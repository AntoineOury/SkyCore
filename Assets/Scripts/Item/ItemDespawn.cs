using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDespawn : MonoBehaviour
{
    [SerializeField]
    private int _timer = 300;

    void Start()
    {
        StartCoroutine(Despawn());
    }

    /// <summary>
    /// Timer to DeSpawn objects after a certain amount of time
    /// </summary>
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(_timer);
        Destroy(gameObject);
    }
}
