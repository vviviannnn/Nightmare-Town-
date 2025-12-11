using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing_Open : MonoBehaviour
{
    [SerializeField] Transform player;
    Camera_Mover pScript;

    Transform camm;

    AudioPlayer audioGoat;

    [SerializeField] public float MAX_ROT;
    [SerializeField] int openMode;

    Transform par;
    public float swing_to = 0f;
    public float currentRot;

    public bool unlocked;

    private void Start()
    {
        
        if (openMode != 2)
        {
            unlocked = true;
        }
        par = transform.parent;
        if (openMode == 0) {
            currentRot = par.rotation.x;
        }
        else
        {
            currentRot = par.rotation.y;
        }
        pScript = player.GetComponent<Camera_Mover>();
        camm = player.GetChild(0);
        audioGoat = camm.GetComponent<AudioPlayer>();
    }

    private void OnMouseOver()
    {
        if (Vector3.Distance(transform.position, player.position) < pScript.interact_distance && unlocked)
        {
            pScript.leftInteractText.text = "Open/Close";
            if (Input.GetMouseButtonDown(0))
            {
                audioGoat.audios[4].volume = 0.5f;
                audioGoat.audios[4].Play();
                if (swing_to == MAX_ROT)
                {
                    swing_to = 0f;
                }
                else
                {
                    swing_to = MAX_ROT;
                }
            }
        }
    }

    void Update()
    {
        if (openMode == 0)
        {
            if (currentRot - swing_to < -1f)
            {
                currentRot++;
                par.rotation = Quaternion.Euler(currentRot, par.rotation.y, par.rotation.z);
            }
            else if (currentRot - swing_to > 1f && unlocked)
            {
                currentRot--;
                par.rotation = Quaternion.Euler(currentRot, par.rotation.y, par.rotation.z);
            }
        }
        else
        {
            if (currentRot - swing_to < -1f)
            {
                currentRot++;
                par.rotation = Quaternion.Euler(par.rotation.x, currentRot, par.rotation.z);
            }
            else if (currentRot - swing_to > 1f)
            {
                currentRot--;
                par.rotation = Quaternion.Euler(par.rotation.x, currentRot, par.rotation.z);
            }
        }
    }
}
