using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;            //Tiempo para que la camara se mueva     
    public float m_ScreenEdgeBuffer = 4f;      //Lo que usaremos para que los tanques no esten el los bordes
    public float m_MinSize = 6.5f;             //Tamaño minimo del zoom
    /*[HideInIspector]*/public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();
        //Recogemos la posicion para la camara
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition(){
        Vector3 averagePos = new Vector3();
        int numTargets = 0;
        //Añadimos los tanques a la posicion
        for (int i = 0; i < m_Targets.Length; i++){
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++; 
        }
        //Recoge la posicion entre el número de tanques
        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y; //No deja que la variable de la altura cambie

        m_DesiredPosition = averagePos;
        
    }    


    private void Zoom()
    {  
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()  //Calculo del tamaño que tendra la camara
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position); //vector para la posicion de los tanques

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;   //Calculo de lo que se tiene que mover para estar igual

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()  //Este metodo es publico ya que querremos acceder a la posicion y tamaño
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}