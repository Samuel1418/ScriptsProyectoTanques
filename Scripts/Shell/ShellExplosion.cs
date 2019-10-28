using UnityEngine;

namespace Complete
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
        public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
        public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
        public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
        public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
        public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
        public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.


        private void Start ()
        {
            // Si no esta destruido, destruye la bala cuando se acabe su tiempo de vida
            Destroy (gameObject, m_MaxLifeTime);
        }


        private void OnTriggerEnter (Collider other)
        {
			// Recoge todos los colisionadores en una esfera desde la posición actual del proyectil hasta un radio del radio de explosión.
            Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

            // Va a traves de todos los colisionadores
            for (int i = 0; i < colliders.Length; i++)
            {
                // Y se encuentra con su fisica.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

                // Si no tiene fisica, va al siguiente collider.
                if (!targetRigidbody)
                    continue;

                // Añadimos una fuerza de explosion
                targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

                // Encuentra TankHealth script asociado con el rigidbody.
                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

                // Si no hay TankHealth script en el gameobject, va al siguiente collider.
                if (!targetHealth)
                    continue;

                // Calcula la cantidad de daño que el objetivo debería recibir basado en su distancia con la bala.
                float damage = CalculateDamage (targetRigidbody.position);

                // Hace el daño al tanque
                targetHealth.TakeDamage (damage);
            }

            // Desaparecen las partículas del caparazón.
            m_ExplosionParticles.transform.parent = null;

            // Enciende las particulas
            m_ExplosionParticles.Play();

            // Enciende sonido de la explosion
            m_ExplosionAudio.Play();

            // cuando las particulas acaban, destruye el gameobject en el que estan.
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy (m_ExplosionParticles.gameObject, mainModule.duration);

            // Destruye la bala
            Destroy (gameObject);
        }


        private float CalculateDamage (Vector3 targetPosition)
        {
            // Crea un vector desde la bala al objetivo.
            Vector3 explosionToTarget = targetPosition - transform.position;

            // Calcula la distancia entre el objetivo y la bala.
            float explosionDistance = explosionToTarget.magnitude;

            // Calcula la proporcion de la distancia maxima (el radio de la explosion) a la que esta el enemigo.
            float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

            // calcula el daño como esta proporción del daño máximo posible.
            float damage = relativeDistance * m_MaxDamage;

            // El daño minimo es 0 siempre.
            damage = Mathf.Max (0f, damage);

            return damage;
        }
    }
}