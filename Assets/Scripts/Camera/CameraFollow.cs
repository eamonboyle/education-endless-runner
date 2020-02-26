using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;
    private float offset2;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        offset2 = transform.position.z - player.transform.position.z;
    }

    private void LateUpdate()
    {
        //transform.position = player.transform.position + offset;
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset2);
    }
}
