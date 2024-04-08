using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDespawn : MonoBehaviour
{
    [SerializeField]
    private int timer = 300;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Despawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}
