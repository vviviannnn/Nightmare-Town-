using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    [SerializeField] Transform player;
    Camera_Mover pScript;

    private readonly float travelDistance = 4.036f;
    [SerializeField] int buttonNum;

    private void Start()
    {
        pScript = player.GetComponent<Camera_Mover>();
    }

    private void OnMouseOver()
    {
        if (Vector3.Distance(player.position, transform.position) < pScript.interact_distance)
        {
            pScript.leftInteractText.text = "Click button";
            if (Input.GetMouseButtonDown(0))
            {
                if (buttonNum == 0)
                {

                }
                else
                {
                    Elevator();
                }
            }
        }
    }

    private void Elevator()
    {
        Debug.Log("Check");
        if (buttonNum == 1)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + travelDistance, player.transform.position.z);
        }
        else
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - travelDistance, player.transform.position.z);
        }
    }
}
