using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
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

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        //Valores que recibe el player para moverse
        m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }


    private void EngineAudio()
    {
        // Segun lo que haga el tanque que haga un sonido
        if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
            {
                // Si esta sonando el sonido de moverse que cambie al de parado
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    // Cambiar el audio para cuando el tanque se para
                    m_MovementAudio.clip = m_EngineIdling;
                    //esto cambia el tono entre un valor de 1.2 y 0.8 para evitar los 'numeros magicos'
                    m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play ();
                }
            }
            else
            {
                // Si el tanque tiene el sonido de parado que cambie al de moverse
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    // ... change the clip to driving and play.
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
    
    }


    private void FixedUpdate() // Lo que va pasar cada frame
    {
        
        Move ();
        Turn ();
    }


    private void Move()
    {
        // Ajustamos la posicion del tanque segun el imput que le demos
        // Crear vector de direccion para que el tanque se mueva basado en el input, velocidad y los frames por segundo.
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

            // Aplicar a las fisicas este movimiento
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // AAjustamos la rotacion del tanque segun el imput que le demos
        // Determinamos los grados a rotar segun el input, la velocidad y los fps.
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

            //Hace la rotacion sobre el eje Y para el tanque
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

            // Aplicar esta rotacion creada a las fisicas
            m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }
}