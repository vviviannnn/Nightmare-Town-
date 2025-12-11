using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Market : MonoBehaviour
{
    [SerializeField] float killFloor;
    [SerializeField] int deathWindow;
    [SerializeField] GameObject audioCam;
    

    [SerializeField] GameObject playerBody;
    [SerializeField] GameObject sunObject;
    [SerializeField] GameObject[] shopItems;
    [SerializeField] Transform[] itemDroppers;

    [SerializeField] GameObject[] doors1;
    [SerializeField] GameObject[] doors2;
    [SerializeField] GameObject[] doors3;


    AudioPlayer audioGoat;
    public GameObject[] currentItems = { null, null, null };
    Shop_Item[] itemScripts = { null, null, null };
    int[] targetBoxes = { 0, 0, 0 };

    public bool doomsday = false;
    
    Sun sun;
    Camera_Mover player;

    public bool emergency = false;

    MeshFilter m1;
    MeshFilter m2;


    [SerializeField] int spawnRarity = 3; // 1/spawnRarity items per tick
    [SerializeField] int evilRarity = 2;
    int randomized;

    public int windowsOpen = 0;
    public int windowsOpenNow;

    void Start()
    {
        windowsOpenNow = 0;
        sun = sunObject.GetComponent<Sun>();
        player = playerBody.GetComponent<Camera_Mover>();

        m1 = player.leftHand.GetComponent<MeshFilter>();
        m2 = player.rightHand.GetComponent<MeshFilter>();
        audioGoat = audioCam.GetComponent<AudioPlayer>();
    }

    void DoomsMode()
    {
        if (sun.evilTickPassed && player.inControl)
        {
            int rando = Random.Range(0, evilRarity);
            if (rando == 0)
            {
                audioGoat.audios[4].volume = 0.5f;
                audioGoat.audios[4].Play();

                Swing_Open dScript;
                int gen = GenerateValidTarget();
                int tens = gen / 10;
                int units = gen % 10;
                if (tens == 1)
                {
                    dScript = doors1[units - 1].GetComponent<Swing_Open>();
                    dScript.swing_to = dScript.MAX_ROT;
                }
                else if (tens == 2)
                {
                    dScript = doors2[units - 1].GetComponent<Swing_Open>();
                    dScript.swing_to = dScript.MAX_ROT;
                }
                else if (tens == 3)
                {
                    dScript = doors3[units - 1].GetComponent<Swing_Open>();
                    dScript.swing_to = dScript.MAX_ROT;
                }
            }
            sun.evilTickPassed = false;
        }
        if (windowsOpenNow >= deathWindow || doomsday)
        {
            doomsday = false;
            player.GiveUpControl();
            foreach (int i in targetBoxes)
            {
                UnlockWindow(i);
            }
            foreach (GameObject door in doors1)
            {
                if (door != null)
                {
                    Swing_Open swing = door.GetComponent<Swing_Open>();
                    swing.unlocked = true;
                    swing.swing_to = 0f;
                }
            }
            foreach (GameObject door in doors2)
            {
                Swing_Open swing = door.GetComponent<Swing_Open>();
                swing.unlocked = true;
                swing.swing_to = 0f;
            }
            foreach (GameObject door in doors3)
            {
                Swing_Open swing = door.GetComponent<Swing_Open>();
                swing.unlocked = true;
                swing.swing_to = 0f;
            }
            for (int i = 0;i < itemDroppers.Length;i++)
            {
                targetBoxes[i] = 0;
                itemScripts[i] = null;
                GameObject.Destroy(currentItems[i]);
                currentItems[i] = null;
            }
            m1.mesh = null;
            m2.mesh = null;
            player.leftGoal = 0;
            player.rightGoal = 0;
        }
        else if (windowsOpenNow >= deathWindow -1)
        {
            emergency = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        windowsOpenNow = WindowCheck();
        if (sun.totalDayTime <= 240 && player.inControl == false)
        {
            player.ControlBack();
        }
        if (player.inControl)
        {
            if (sun.totalDayTime > 240)
            {
                DoomsMode();
            }
            if (sun.tickPassed)
            {
                randomized = Random.Range(0, spawnRarity);
                if (randomized == 0)
                {
                    CreateItem();
                }
                sun.tickPassed = false;
            }
            for (int i = 0; i < itemDroppers.Length; i++)
            {
                if (itemScripts[i] != null)
                {
                    if (currentItems[i].transform.position.y < killFloor)
                    {
                        UnlockWindow(targetBoxes[i]);
                        GameObject.Destroy(currentItems[i]);
                        currentItems[i] = null;
                        itemScripts[i] = null;
                        targetBoxes[i] = 0;
                    }
                    else
                    {
                        if (itemScripts[i].isOver && Vector3.Distance(currentItems[i].transform.position, playerBody.transform.position) < player.interact_distance)
                        {
                            itemScripts[i].clickable = true;
                            if (player.leftGoal == 0)
                            {
                                player.leftInteractText.text = "Pick up item";
                            }
                            if (player.rightGoal == 0)
                            {
                                player.rightInteractText.text = "Pick up item";
                            }
                        }
                        itemScripts[i].clickable = false;
                        itemScripts[i].isOver = false;
                        if (itemScripts[i].clicked)
                        {
                            MeshFilter meshi = currentItems[i].GetComponent<MeshFilter>();
                            if (itemScripts[i].leftHanded)
                            {
                                if (player.leftGoal == 0)
                                {
                                    m1.mesh = meshi.mesh;
                                    GameObject.Destroy(currentItems[i]);
                                    currentItems[i] = null;
                                    itemScripts[i] = null;
                                    player.leftGoal = targetBoxes[i];
                                    player.leftHandText.text = ("Delivery for house " + WindowNumInterpreter(targetBoxes[i]));
                                    targetBoxes[i] = 0;
                                }
                                else
                                {
                                    itemScripts[i].clicked = false;
                                }
                            }
                            else
                            {
                                if (player.rightGoal == 0)
                                {
                                    m2.mesh = meshi.mesh;
                                    GameObject.Destroy(currentItems[i]);
                                    currentItems[i] = null;
                                    itemScripts[i] = null;
                                    player.rightGoal = targetBoxes[i];
                                    player.rightHandText.text = ("Delivery for house " + WindowNumInterpreter(targetBoxes[i]));
                                    targetBoxes[i] = 0;
                                }
                                else
                                {
                                    itemScripts[i].clicked = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void CreateItem()
    {
        if (sun.totalDayTime >= 180f)
        {
            randomized = Random.Range(0, 7);
        }
        else
        {
            randomized = Random.Range(0, 5);
        }

        for (int i = 0; i < itemDroppers.Length; i++)
        {
            if (currentItems[i] == null)
            {
                currentItems[i] = Instantiate(shopItems[randomized], new Vector3(itemDroppers[i].position.x, itemDroppers[i].position.y, itemDroppers[i].position.z), Quaternion.identity);
                itemScripts[i] = currentItems[i].GetComponent<Shop_Item>();
                targetBoxes[i] = GenerateValidTarget();
                break;
            }
        }
    }

    int GenerateValidTarget()
    {
        while (true)
        {
            int units = Random.Range(1, 6);
            int tens = Random.Range(1, 4);
            int possibleTarget = (tens * 10) + units;
            if (possibleTarget != 12 && possibleTarget != 13 && possibleTarget != targetBoxes[0] && possibleTarget != targetBoxes[1] && possibleTarget != targetBoxes[2])
            {
                if (possibleTarget != player.leftGoal && possibleTarget != player.rightGoal)
                {
                    LockWindowOpen(possibleTarget);

                    return possibleTarget;
                }
            }
        }
    }

    void LockWindowOpen(int windowNum)
    {
        Swing_Open doorScript;
        int tens = windowNum / 10;
        int units = windowNum % 10;
        if (tens == 1)
        {
            doorScript = doors1[units-1].GetComponent<Swing_Open>();
            doorScript.unlocked = false;
            doorScript.swing_to = doorScript.MAX_ROT;
        }
        else if (tens == 2)
        {
            doorScript = doors2[units - 1].GetComponent<Swing_Open>();
            doorScript.unlocked = false;
            doorScript.swing_to = doorScript.MAX_ROT;
        }
        else if (tens == 3)
        {
            doorScript = doors3[units - 1].GetComponent<Swing_Open>();
            doorScript.unlocked = false;
            doorScript.swing_to = doorScript.MAX_ROT;
        }
    }

    public void UnlockWindow(int windowNum)
    {
        Swing_Open doorScript;
        int tens = windowNum / 10;
        int units = windowNum % 10;
        if (tens == 1)
        {
            doorScript = doors1[units - 1].GetComponent<Swing_Open>();
            doorScript.unlocked = true;
        }
        else if (tens == 2)
        {
            doorScript = doors2[units - 1].GetComponent<Swing_Open>();
            doorScript.unlocked = true;
        }
        else if (tens == 3)
        {
            doorScript = doors3[units - 1].GetComponent<Swing_Open>();
            doorScript.unlocked = true;
        }
    }

    int WindowCheck()
    {
        Swing_Open doorScript;
        int windowsOpen = 0;
        for (int i = 0; i < doors1.Length; i++)
        {
            if (i != 1 &&  i != 2)
            {
                doorScript = doors1[i].GetComponent<Swing_Open>();
                if (doorScript.currentRot > 1f)
                {
                    windowsOpen++;
                }
            }
        }
        for (int i = 0; i < doors2.Length; i++)
        {
            doorScript = doors2[i].GetComponent<Swing_Open>();
            if (doorScript.currentRot > 1f)
            {
                windowsOpen++;
            }
        }
        for (int i = 0; i < doors3.Length; i++)
        {
            doorScript = doors3[i].GetComponent<Swing_Open>();
            if (doorScript.currentRot > 1f)
            {
                windowsOpen++;
            }
        }
        return windowsOpen;
    }

    string WindowNumInterpreter(int windowNum)
    {
        int tens = windowNum / 10;
        int units = windowNum % 10;

        if (tens == 1)
        {
            return ("A"+units).ToString();
        }
        if (tens == 2)
        {
            return ("B" + units).ToString();
        }
        if (tens == 3)
        {
            return ("C" + units).ToString();
        }
        return "failure to interpret target num";
    }
}
