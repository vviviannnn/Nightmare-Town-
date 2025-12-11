using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Security_Camera : MonoBehaviour
{
    [SerializeField] Transform playerBody;
    [SerializeField] float cammove;
    float playerDif;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        playerDif = Mathf.Clamp((playerBody.position.z - transform.position.z) * 6f, -60f, 60f);
        transform.rotation = Quaternion.Euler(transform.rotation.x, playerDif, cammove);
    }
}
