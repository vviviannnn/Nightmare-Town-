using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siren : MonoBehaviour
{
    [SerializeField] GameObject market;
    [SerializeField] float max_count = 10;
    [SerializeField] GameObject player;
    Camera_Mover pScript;
    Market marketScript;
    public bool emergence;
    public float countDown;

    void Start()
    {
        emergence = false;
        marketScript = GetComponent<Market>();
        pScript = GetComponent<Camera_Mover>();
        countDown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (marketScript.emergency)
        {
            if (!emergence)
            {
                countDown = 0;
                emergence = true;
            }
            countDown += Time.deltaTime;
            Debug.Log(countDown);
            if (countDown > max_count)
            {
                marketScript.doomsday = true;
                marketScript.emergency = false;
            }
        }
        else
        {
            emergence = false;
        }
    }
}
