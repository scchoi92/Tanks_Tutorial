using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDamaged : MonoBehaviour
{
    [SerializeField] float maxWarningTime = 3f;
    [SerializeField] float remainingWarningTime;

    private Light warningLight;
    [SerializeField] bool lightOn;

    [SerializeField] float warningSoundMaxVolume = 0.7f;
    private AudioSource warningSound;
    
    // Start is called before the first frame update
    void Awake()
    {
        warningLight = GetComponent<Light>();
        warningSound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        warningSound.volume = warningSoundMaxVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if(remainingWarningTime > 0)
        {
            remainingWarningTime -= Time.deltaTime;

            if (warningLight.intensity <= 0f)
                lightOn = true;
            else if (warningLight.intensity >= 10f)
                lightOn = false;

            if(!warningSound.isPlaying)
                warningSound.Play();

            if(remainingWarningTime <= warningSoundMaxVolume)
                warningSound.volume -= Time.deltaTime;
        }
        else
        {
            lightOn = false;
            warningSound.Stop();
        }

        if (lightOn)
            warningLight.intensity = Mathf.Clamp(warningLight.intensity += Time.deltaTime * 100f, 0f, 10f);
        else
            warningLight.intensity = Mathf.Clamp(warningLight.intensity -= Time.deltaTime * 100f, 0f, 10f);
    }

    public void SetWarningTime()
    {
        remainingWarningTime = maxWarningTime;
        warningSound.volume = warningSoundMaxVolume;
    }

    public void SetToDefault()
    {
        remainingWarningTime = 0f;
        warningSound.volume = warningSoundMaxVolume;
    }
}
