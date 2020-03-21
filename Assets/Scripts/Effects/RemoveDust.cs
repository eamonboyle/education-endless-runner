using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveDust : MonoBehaviour
{
    public float timer = .2f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Remove", timer);
    }

    void Remove()
    {
        Destroy(gameObject);
    }
}
