using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankHealth : MonoBehaviour
    {
        public float m_StartingHealth = 100f;
        public Slider m_Slider;
        public Image m_FillImage;
        public Color m_FullHealthColor = Color.green;
        public Color m_ZeroHealthColor = Color.red;
        public GameObject m_ExplosionPrefab;
        
        
        private AudioSource m_ExplosionAudio;
        private ParticleSystem m_ExplosionParticles;
        private float m_CurrentHealth;
        private bool m_Dead;


        private void Awake ()
        {
            // Instanciamos la explosion y cogemos las particulas
            m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

            // La añadimos a la explosion las particulas
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();

            // Que no se activen solas las particulas
            m_ExplosionParticles.gameObject.SetActive (false);
        }


        private void OnEnable()
        {
            // Reestablecemos el tanque
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;

            // Volvemos a su valor inicial al slider de la salud
            SetHealthUI();
        }


        public void TakeDamage (float amount)
        {
            // Segun el daño recibido, baja la salud
            m_CurrentHealth -= amount;

            // Carga la salud de 
            SetHealthUI ();

            // Si la salud baja de 0 llamamos a OnDeath
            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath ();
            }
        }


        private void SetHealthUI ()
        {
            m_Slider.value = m_CurrentHealth;

            // Segun la salud va cambiando el color del slider
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }


        private void OnDeath ()
        {
            m_Dead = true;

            // Realiza la explosion del tanque en la posicion donde esta el tanque
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive (true);

            m_ExplosionParticles.Play ();

            m_ExplosionAudio.Play();

            gameObject.SetActive (false);
        }
    }
}