using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public AudioSource m_MovementAudio_Idle;
    public AudioSource m_MovementAudio_Driving;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_OriginalPitchRange = 0.2f;

    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;
    private bool isIdle = true;

    [SerializeField] float currentPitchRange = 0.2f;
    [SerializeField] float rearMovementReduction = 0.7f;
    [SerializeField] float maxEngineVolume = 0.5f;
    TankHealth tankHealth;
    float currentHealthPercentage;



    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        tankHealth = GetComponent<TankHealth>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio_Idle.pitch;
        currentPitchRange = m_OriginalPitch;
        m_MovementAudio_Driving.pitch = m_OriginalPitch;

        m_MovementAudio_Idle.volume = maxEngineVolume * 0.7f;
        m_MovementAudio_Driving.volume = 0f;
        
    }

    private void Update()
    {
        PitchAsHealthDrops();

        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
            isIdle = true;
        else
            isIdle = false;

        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }

    private void PitchAsHealthDrops()
    {
        if(tankHealth.HealthAsPercentage() == 0)
        {
            currentPitchRange = m_OriginalPitchRange + 10f;
            return;
        }
        currentHealthPercentage = 1 / Mathf.Max(tankHealth.HealthAsPercentage(), 0.01f);
        currentPitchRange = m_OriginalPitchRange + 0.1f * currentHealthPercentage;
    }

    private void EngineAudio()
    {
        if(isIdle)
        {
            if(m_MovementAudio_Idle.volume != maxEngineVolume * 0.7f || m_MovementAudio_Driving.volume != 0)
            {
                m_MovementAudio_Idle.volume = Mathf.Clamp(m_MovementAudio_Idle.volume + Time.deltaTime * 0.5f, 0f, maxEngineVolume * 0.7f);
                m_MovementAudio_Driving.volume = Mathf.Clamp(m_MovementAudio_Driving.volume - Time.deltaTime * 0.5f, 0f, maxEngineVolume);
                m_MovementAudio_Idle.pitch = Random.Range(m_OriginalPitch - currentPitchRange / 2, m_OriginalPitch + currentPitchRange / 2);
            }
        }
        else
        {
            if(m_MovementAudio_Driving.volume != maxEngineVolume || m_MovementAudio_Idle.volume != 0)
            {
                m_MovementAudio_Driving.volume = Mathf.Clamp(m_MovementAudio_Driving.volume + Time.deltaTime * 0.5f, 0f, maxEngineVolume);
                m_MovementAudio_Idle.volume = Mathf.Clamp(m_MovementAudio_Idle.volume - Time.deltaTime * 0.5f, 0f, maxEngineVolume * 0.7f);
            }

            if (m_MovementInputValue > 0f)
            {
                m_MovementAudio_Driving.pitch = Random.Range(m_OriginalPitch - currentPitchRange / 2, m_OriginalPitch + currentPitchRange);
            }
            else
            {
                m_MovementAudio_Driving.pitch = Random.Range(m_OriginalPitch - currentPitchRange, m_OriginalPitch + currentPitchRange / 2);
            }
        }
        /*
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if(m_MovementAudio_Idle.clip == m_EngineDriving)
            {
                m_MovementAudio_Idle.clip = m_EngineIdling;
                m_MovementAudio_Idle.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio_Idle.Play();
            }
        }
        else
        {
            if (m_MovementAudio_Idle.clip == m_EngineIdling && m_MovementInputValue > 0f)
            {
                m_MovementAudio_Idle.clip = m_EngineDriving;
                m_MovementAudio_Idle.pitch = Random.Range(m_OriginalPitch, m_OriginalPitch + m_PitchRange);
                m_MovementAudio_Idle.Play();
            }
            else if(m_MovementAudio_Idle.clip == m_EngineIdling && m_MovementInputValue < 0f)
            {
                m_MovementAudio_Idle.clip = m_EngineDriving;
                m_MovementAudio_Idle.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch);
                m_MovementAudio_Idle.Play();
            }
        }
        */
    }


    private void FixedUpdate()
    {
        Move();
        Turn();
    }


    private void Move()
    {
        //Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        //m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        if(m_MovementInputValue > 0f)
            m_Rigidbody.velocity = transform.forward * m_MovementInputValue * m_Speed;
        else
            m_Rigidbody.velocity = transform.forward * m_MovementInputValue * m_Speed * rearMovementReduction;
    }


    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}