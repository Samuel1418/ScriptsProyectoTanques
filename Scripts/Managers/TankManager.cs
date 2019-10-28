using System;
using UnityEngine;

[Serializable]
 public class TankManager
    {
        // Con esta clase controlamos varios ajustes del tanque
        // Funciona con el GameManager class para controlar como se comporta el tanque
        // y si dejar a los jugadores tener control del tanque en diferentes partes de la partida

        public Color m_PlayerColor;                             
        public Transform m_SpawnPoint;                         
        [HideInInspector] public int m_PlayerNumber;            
        [HideInInspector] public string m_ColoredPlayerText;    
        [HideInInspector] public GameObject m_Instance;        
        [HideInInspector] public int m_Wins;                    
        

        private TankMovement m_Movement;                       
        private TankShooting m_Shooting;                        
        private GameObject m_CanvasGameObject;                 


        public void Setup ()
        {
            // Damos las referencias a los componentes
            m_Movement = m_Instance.GetComponent<TankMovement> ();
            m_Shooting = m_Instance.GetComponent<TankShooting> ();
            m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas> ().gameObject;

            m_Movement.m_PlayerNumber = m_PlayerNumber;
            m_Shooting.m_PlayerNumber = m_PlayerNumber;

            //Crear un string usando el color correcto que dice 'PLAYER 1' etc basado en el color del tanque y el numero del jugador
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

            // Coje todos los renderers de tank.
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer> ();

            //Recorre todos los renders
            for (int i = 0; i < renderers.Length; i++)
            {
                // ... pone su material color al color especifico del tanque
                renderers[i].material.color = m_PlayerColor;
            }
        }


        public void DisableControl ()
        {
            m_Movement.enabled = false;
            m_Shooting.enabled = false;

            m_CanvasGameObject.SetActive (false);
        }


        public void EnableControl ()
        {
            m_Movement.enabled = true;
            m_Shooting.enabled = true;

            m_CanvasGameObject.SetActive (true);
        }


        // Para iniciar la ronda pone el tanque en su sitio
        public void Reset ()
        {
            m_Instance.transform.position = m_SpawnPoint.position;
            m_Instance.transform.rotation = m_SpawnPoint.rotation;

            m_Instance.SetActive (false);
            m_Instance.SetActive (true);
        }
    }