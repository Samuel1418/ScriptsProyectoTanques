using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;    //Para que no se dispare seguido se usa esta variable            


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce; //Empezamos con la minima fuerza el disparo
        m_AimSlider.value = m_MinLaunchForce;   //Cuando resucitamos el disparo no va estar cargado
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber; //Claculamos el boton de disparar
        //Lo de arriba lo podemos ver en las opciones del proyecto en Input
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime; //Calculo del tiempo de carga del disparo
    }

    private void Update()
    {
        m_AimSlider.value=m_MinLaunchForce;
        if(m_CurrentLaunchForce>= m_MaxLaunchForce && !m_Fired){
            m_CurrentLaunchForce= m_MaxLaunchForce;
            Fire(); //Disparo cuando estamos cargados al maximo
        }
        
        else if(Input.GetButtonDown(m_FireButton)){
            m_Fired=false;
            m_CurrentLaunchForce= m_MinLaunchForce;
            m_ShootingAudio.clip=m_ChargingClip;
            m_ShootingAudio.Play(); //Mientras no disparemos al haber presionado el boton por primera vez sonará el audio de cargando el tiro

        }else if(Input.GetButton(m_FireButton)&& !m_Fired){
            m_CurrentLaunchForce +=m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value= m_CurrentLaunchForce; //El valor de la fuerza del disparo aumenta segun pasa el tiempo que mantenemos presionado
        }
        
        else if(Input.GetButtonUp(m_FireButton)&& !m_Fired){
            Fire(); //Disparamos cuando se levanta el boton
        }
    }


    private void Fire()
    {
    
        m_Fired=true;
        //Vamos a instanciar el Rigibody y a ponerlo en su posicion
        Rigidbody shellInstance= Instantiate(m_Shell,m_FireTransform.position,m_FireTransform.rotation) as Rigidbody;
        //Lanzamiento de la bala
        shellInstance.velocity=m_CurrentLaunchForce * m_FireTransform.forward;
        m_ShootingAudio.clip=m_FireClip;  //audio del disparo
        m_CurrentLaunchForce = m_MinLaunchForce; //Vuelta del valor al minimo al haber disparado
    }
}