using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField] float TICK_TIME;
    [SerializeField] float sunBoost;
    [SerializeField] float sunSpeeder;
    [SerializeField] GameObject audioCam;

    AudioPlayer audioGoat;

    public bool tickPassed = false;
    public bool evilTickPassed = false;

    public float totalDayTime;
    float markerTime;
    float sunRotation;
    Light sunlight;

    void Start()
    {
        markerTime = 0;
        totalDayTime = 0;
        sunRotation = 0;
        sunlight = GetComponent<Light>();
        sunlight.intensity = Mathf.Sin(sunRotation) + sunBoost;
        audioGoat = audioCam.GetComponent<AudioPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        markerTime += Time.deltaTime;
        if (markerTime > TICK_TIME)
        {
            ClockTick();
        }
    }

    void ClockTick()
    {
        markerTime -= TICK_TIME;
        totalDayTime += (TICK_TIME) * sunSpeeder;
        totalDayTime %= 480f;
        if (totalDayTime == 10)
        {
            audioGoat.Play1();
        }
        else if (totalDayTime == 240)
        {
            audioGoat.Play2();
        }
        sunRotation += 7.5f;
        sunRotation %= 360f;
        transform.rotation = Quaternion.Euler(sunRotation, 0f, 0f);
        sunlight.intensity = Mathf.Sin(sunRotation*Mathf.PI/180f) + sunBoost;
        tickPassed = true;
        evilTickPassed = true;
    }
}
