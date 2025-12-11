using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Camera_Mover : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cursor;
    [SerializeField] public TextMeshProUGUI leftHandText;
    [SerializeField] public TextMeshProUGUI rightHandText;
    [SerializeField] public TextMeshProUGUI leftInteractText;
    [SerializeField] public TextMeshProUGUI rightInteractText;
    [SerializeField] public TextMeshProUGUI alarmText;

    [SerializeField] public float interact_distance;

    [SerializeField] float sensX;
    [SerializeField] float sensY;
    [SerializeField] float moveSpeed;

    [SerializeField] float angleSensitivity;
    [SerializeField] float waypointSensitivity;
    [SerializeField] Transform[] marks;
    [SerializeField] Transform[] waypoints0;
    [SerializeField] Transform[] waypoints1;
    [SerializeField] Transform[] waypoints2;
    [SerializeField] Transform[] waypoints3;
    [SerializeField] Transform finalWaypoint;
    [SerializeField] Transform deathPoint;
    [SerializeField] GameObject deathDoor;
    Swing_Open DDScript;
    [SerializeField] float STUCKLIMIT;
    [SerializeField] int ESCAPETHRESHOLD;

    bool pastA;
    bool pastB;
    bool pastC;
    bool toDeath;
    bool atDeath;
    float stuckTime;
    int escapeClick = 0;

    public int deliveriesMade = 0;


    Rigidbody rb;
    Transform cam;

    [SerializeField] float REFRESH_TIME;
    float textClearer;

    AudioPlayer audioGoat;

    public bool inControl = true;
    bool escapePossible;
    bool gameOver;

    //bool inHouse = true;

    float STARTING_HEIGHT;

    float yPos;

    float mouseX;
    float mouseY;
    float yRotation;
    float xRotation;


    [SerializeField] public GameObject leftHand;
    [SerializeField] public GameObject rightHand;

    public int leftGoal;
    public int rightGoal;



    void Start()
    {
        leftGoal = 0;
        rightGoal = 0;
        leftHandText.text = "";
        rightHandText.text = "";
        leftInteractText.text = "";
        rightInteractText.text = "";

        textClearer = 0f;

        pastA = false;
        pastB = false;
        pastC = false;
        toDeath = false;
        atDeath = false;
        escapePossible = true;
        gameOver = true;
        stuckTime = 0f;
        escapeClick = 0;


        DDScript = deathDoor.GetComponent<Swing_Open>();

        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        STARTING_HEIGHT = transform.position.y;
        yPos = STARTING_HEIGHT;
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(0);
        yRotation = transform.rotation.y;
        xRotation = transform.rotation.x;
        audioGoat = cam.GetComponent<AudioPlayer>();
    }

    public void GiveUpControl()
    {
        if (transform.position.x > -7f || transform.position.x < -18f || transform.position.z > 13.5f || transform.position.z < 4f)
        {
            audioGoat.Play3();
            audioGoat.audios[1].Stop();
            inControl = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        textClearer += Time.deltaTime;
        if (textClearer > REFRESH_TIME)
        {
            textClearer -= REFRESH_TIME;
            leftInteractText.text = "";
            rightInteractText.text = "";
        }

        if (inControl && escapePossible)
        {
            if (transform.position.y < 2f)
            {
                yPos = STARTING_HEIGHT;
            }
            else
            {
                yPos = STARTING_HEIGHT + 4.036f;
            }
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

            GetCursorInput();
            yRotation += mouseX;
            xRotation -= mouseY;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            xRotation = Mathf.Clamp(xRotation, -50f, 50f);
            cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            rb.rotation = Quaternion.Euler(0f, 0f, 0f);

            if (Input.GetKey(KeyCode.W))
            {
                if (!audioGoat.audios[3].isPlaying)
                {
                    audioGoat.audios[3].Play();
                }
                rb.velocity = moveSpeed * transform.forward;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (!audioGoat.audios[3].isPlaying)
                {
                    audioGoat.audios[3].Play();
                }
                rb.velocity = moveSpeed * -transform.forward;
            }
            else
            {
                audioGoat.audios[3].Stop();
                rb.velocity = Vector3.zero;
            }
        }
        else if (gameOver)
        {
            ForceText();
            ThresholdCheck();
            WaypointCheck();
            Transform lead = WhichPoint();
            GoTo(lead);
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && escapePossible)
            {
                escapeClick++;
                if (escapeClick > ESCAPETHRESHOLD)
                {
                    ControlBack();
                }
            }
        }
    }

    public void ControlBack() 
    {
        pastA = false;
        pastB = false;
        pastC = false;
        toDeath = false;
        escapeClick = 0;
        alarmText.text = "";
        stuckTime = 0f;
        inControl = true;
        audioGoat.audios[2].Stop();
    }

    void GoTo(Transform t)
    {
        var lookPos = t.position - transform.position;
        lookPos.y = 0;
        int damping = 2;
        var rotation = Quaternion.LookRotation(lookPos);
        var rotatedVector = rotation * Vector3.forward;
        float currentAngle = Mathf.Abs(Vector3.SignedAngle(transform.forward, rotatedVector, Vector3.up));
        stuckTime += Time.deltaTime;

        if (currentAngle > angleSensitivity)
        {
            rb.freezeRotation = false;
            
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
        else
        {
            rb.freezeRotation = true;
            rb.velocity = moveSpeed * transform.forward;
        }

        if (stuckTime > STUCKLIMIT)
        {
            rb.freezeRotation = true;
            transform.position = new Vector3(t.position.x- 1f, transform.position.y, t.position.z);
            stuckTime = 0f;
        }
    }

    void GetCursorInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        mouseY = Input.GetAxisRaw("Mouse Y") * sensY;
    }

    void ThresholdCheck()
    {
        if (transform.position.x > marks[2].position.x)
        {
            pastC = true;
        }
        if (transform.position.x < marks[0].position.x)
        {
            pastA = true;
        }
        if (transform.position.x < marks[1].position.x)
        {
            pastB = true;
        }
    }

    void WaypointCheck()
    {
        foreach (Transform t in waypoints0)
        {
            if (Vector3.Distance(t.position, transform.position) < waypointSensitivity)
            {
                pastA = true;
                pastC = true;
                stuckTime = 0f;
            }
        }
        foreach (Transform t in waypoints1)
        {
            if (Vector3.Distance(t.position, transform.position) < waypointSensitivity)
            {
                pastA = true;
                pastB = true;
                pastC = true;
                stuckTime = 0f;
            }
        }
        foreach (Transform t in waypoints2)
        {
            if (Vector3.Distance(t.position, transform.position) < waypointSensitivity)
            {
                pastA = true;
                pastB = true;
                pastC = true;
                toDeath = true;
                stuckTime = 0f;
            }
        }
        foreach (Transform t in waypoints3)
        {
            if (Vector3.Distance(t.position, transform.position) < waypointSensitivity)
            {
                pastA = true;
                pastB = true;
                pastC = true;
                stuckTime = 0f;
            }
        }
        if (Vector3.Distance(finalWaypoint.position, transform.position) < waypointSensitivity)
        {
            pastA = true;
            pastB = true;
            pastC = true;
            toDeath = true;
            atDeath = true;
            stuckTime = 0f;
            if (escapePossible)
            {
                UponDoom();
            }
            alarmText.text = "";
        }
        if (Vector3.Distance(deathPoint.position, transform.position) < waypointSensitivity)
        {
            gameOver = false;
            stuckTime = 0f;
            cursor.color = Color.white;
            cursor.text = "You Delivered " + deliveriesMade + " Objects...";
        }
    }
    Transform WhichPoint()
    {
        Transform t;
        if (!pastC)
        {
            if (Vector3.Distance(transform.position, waypoints3[0].position) < Vector3.Distance(transform.position, waypoints3[1].position))
            {
                t = waypoints3[0];
            }
            else
            {
                t = waypoints3[1];
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, waypoints0[0].position) < Vector3.Distance(transform.position, waypoints0[1].position))
            {
                t = waypoints0[0];
            }
            else
            {
                t = waypoints0[1];
            }
        }
        if (pastC && pastA)
        {
            if (Vector3.Distance(transform.position, waypoints1[0].position) < Vector3.Distance(transform.position, waypoints1[1].position))
            {
                t = waypoints1[0];
            }
            else
            {
                t = waypoints1[1];
            }
        }
        if (pastC && pastA && pastB)
        {
            if (Vector3.Distance(transform.position, waypoints2[0].position) < Vector3.Distance(transform.position, waypoints2[1].position))
            {
                t = waypoints2[0];
            }
            else
            {
                t = waypoints2[1];
            }
        }
        if (pastC && pastA && pastB && toDeath)
        {
            t = finalWaypoint;
        }
        if (atDeath)
        {
            DDScript.unlocked = true;
            DDScript.swing_to = DDScript.MAX_ROT;
            t = deathPoint;
        }
        return t;
    }

    void UponDoom()
    {
        cursor.text = "";
        alarmText.text = "";
        escapePossible = false;
        audioGoat.audios[2].Stop();
    }

    void ForceText()
    {
        leftInteractText.text = "";
        rightInteractText.text = "";
        leftHandText.text = "";
        rightHandText.text = "";
        if (escapePossible)
        {
            alarmText.text = "Click to escape!!";
        }
    }
}
