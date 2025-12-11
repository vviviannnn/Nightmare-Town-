using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class Submission_Box : MonoBehaviour
{
    [SerializeField] Transform player;
    Camera_Mover pScript;
    [SerializeField] int boxNum;

    [SerializeField] GameObject market;
    Market mScript;

    MeshFilter m1;
    MeshFilter m2;


    void Start()
    {
        pScript = player.GetComponent<Camera_Mover>();
        m1 = pScript.leftHand.GetComponent<MeshFilter>();
        m2 = pScript.rightHand.GetComponent<MeshFilter>();
        mScript = market.GetComponent<Market>();
    }

    private void OnMouseOver()
    {
        if (Vector3.Distance(player.position, transform.position) < pScript.interact_distance)
        {
            if (pScript.leftGoal == boxNum)
            {
                pScript.leftInteractText.text = "Deposit";
                if (Input.GetMouseButtonDown(0))
                {
                    mScript.UnlockWindow(boxNum);
                    pScript.leftGoal = 0;
                    pScript.deliveriesMade++;
                    pScript.leftHandText.text = "";
                    m1.mesh = null;
                }
            }
            if (pScript.rightGoal == boxNum)
            {
                pScript.rightInteractText.text = "Deposit";
                if (Input.GetMouseButtonDown(1))
                {
                    mScript.UnlockWindow(boxNum);
                    pScript.rightGoal = 0;
                    pScript.deliveriesMade++;
                    pScript.rightHandText.text = "";
                    m2.mesh = null;
                }
            }
        }
    }
}
