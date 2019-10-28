using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 5;            
        public float m_StartDelay = 3f;             
        public float m_EndDelay = 3f;               
        public CameraControl m_CameraControl;       
        public Text m_MessageText;                 
        public GameObject m_TankPrefab;            
        public TankManager[] m_Tanks;               

        
        private int m_RoundNumber;                 
        private WaitForSeconds m_StartWait;         
        private WaitForSeconds m_EndWait;          
        private TankManager m_RoundWinner;          
        private TankManager m_GameWinner;           


        private void Start()
        {
            //Crea los delay para que solo tengan que hacerse una vez.
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);

            SpawnAllTanks();
            SetCameraTargets();

            // Una vez que los tanques se creen y la cámara los esté utilizando como objetivos, comienza el juego.
            StartCoroutine (GameLoop ());
        }


        private void SpawnAllTanks()
        {
            // Todos los tanques
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // Se crean y se les asocia todo lo que necesitan
                m_Tanks[i].m_Instance =
                    Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
        }


        private void SetCameraTargets()
        {
            // Tantos tarjet como tanques
            Transform[] targets = new Transform[m_Tanks.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                // Los pone en su lugar correcto
                targets[i] = m_Tanks[i].m_Instance.transform;
            }

            // Todos los tarjet que la camara tendra
            m_CameraControl.m_Targets = targets;
        }


        // Es llamado desde el inicio y se ejecuta en bucle
        private IEnumerator GameLoop ()
        {
            yield return StartCoroutine (RoundStarting ());

            // Cuando la anterior acaba se inicia esta
            yield return StartCoroutine (RoundPlaying());

            yield return StartCoroutine (RoundEnding());

            // Cuando acaba la partida ve quien es el ganador
            if (m_GameWinner != null)
            {
                // Si ya hay ganador se reinicia la partida
               Application.LoadLevel(Application.loadedLevel);
            }
            else
            {
                // Si no hay ganador se reinicia el game loop
                StartCoroutine (GameLoop ());
            }
        }


        private IEnumerator RoundStarting ()
        {
            // Cuando empieza la partida no deja que los tanques se muevan
            ResetAllTanks ();
            DisableTankControl ();

            // Se ajusta la camara
            m_CameraControl.SetStartPositionAndSize ();

            // Aumenta la ronda y dice en cual estamos
            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;

            // Espera a que acabe el tiempo
            yield return m_StartWait;
        }


        private IEnumerator RoundPlaying ()
        {
            // Cuando empiece la partida da el control
            EnableTankControl ();

            // Quita el mensaje de pantalla
            m_MessageText.text = string.Empty;

            // Mientras no haya solo un tanque se juega la tonda
            while (!OneTankLeft())
            {
                yield return null;
            }
        }


        private IEnumerator RoundEnding ()
        {
            // Para los tanques
            DisableTankControl ();

            // Quita el ganador anterior
            m_RoundWinner = null;

            // Si encuentra ganador acaba la ronda
            m_RoundWinner = GetRoundWinner ();

            // Si hay ganador le sube la puntuacion
            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // Aumenta el marcador del ganador
            m_GameWinner = GetGameWinner ();

            // Muestra los resultados y si hay un ganador
            string message = EndMessage ();
            m_MessageText.text = message;

            yield return m_EndWait;
        }


        // Esto se usa para verificar si hay uno o menos tanques restantes y, por lo tanto, la ronda debería terminar
        private bool OneTankLeft()
        {
            // Empieza la cuenta de los tanques que quedan a 0
            int numTanksLeft = 0;

            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // Si aun hay aumenta el numero
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // Si aun hay tankes devuelve true si no, false
            return numTanksLeft <= 1;
        }
        
        
        // Vemos si hay un ganador de la ronda
        // Esta función se llama asumiendo que 1 o menos tanques están actualmente activos
        private TankManager GetRoundWinner()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // El que esta activo es el ganador
                if (m_Tanks[i].m_Instance.activeSelf)
                    return m_Tanks[i];
            }

            // Si no hay ningun tanque activo devuelve un null que es el empate
            return null;
        }


        private TankManager GetGameWinner()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // Si algun tanque tiene las rondas necesarias para ganar devuelve esto
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                    return m_Tanks[i];
            }

            // Si no, devuelve un null
            return null;
        }


        // Mensaje al final de ronda
        private string EndMessage()
        {
            // Por defecto, cuando finaliza una ronda, no hay ganadores, por lo que el mensaje de finalización predeterminado es un empate
            string message = "DRAW!";

            // Si hay ganador cambia el mensaje
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            // Agrega algunos saltos de línea después del mensaje inicial
            message += "\n\n\n\n";

            // Revisa todos los tanques y agrega cada uno de sus puntos al mensaje
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            }

            // Si hay ganador cambia el mensaje para reflejarlo
            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }


        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }
    }