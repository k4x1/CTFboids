using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class boids : MonoBehaviour
{
    [SerializeField] float m_avoidanceStr = 2;
    [SerializeField] float m_destinationStr = 1;
    //[SerializeField]
    public Vector2 m_direction;
    public int m_detectionRes = 8;
    public RaycastHit2D[][] m_hitArray;
    public float m_avoidanceRange = 1;


    public GameObject m_destinationObj;
    private Rigidbody2D m_rb;
    private BoxCollider2D m_collider;

   [SerializeField] private Vector3 m_avoidance;
   [SerializeField] private Vector3 m_destination;
    void Start()
    {
     
        m_collider = GetComponent<BoxCollider2D>();
        m_rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        
        m_avoidance = Vector2.zero;
        m_destination = Vector2.zero;
        m_hitArray = new RaycastHit2D[m_detectionRes][];
        float rayDirectionDiv = 360 / m_detectionRes;

        for (int i = 0; i < m_detectionRes; i++)
        {
            // Calculate the angle in radians
            float angle = Mathf.Deg2Rad * (rayDirectionDiv * i);

            // Calculate the direction vector based on the angle
            Vector2 rayDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            m_hitArray[i] = Physics2D.RaycastAll(transform.position, rayDirection, m_avoidanceRange);

            bool collided = false;
            Vector3 otherPos = Vector2.zero;
            foreach(RaycastHit2D hit in m_hitArray[i]) { 
                if(hit.collider == m_collider)
                {
                    continue;
                }
                else
                {
                    collided = true;
                    otherPos = hit.collider.transform.position;
                    break;
                }
            }
            if (collided)
            {
                Debug.DrawRay(transform.position, rayDirection * m_avoidanceRange, Color.red);
                float dist = Vector2.Distance(transform.position, otherPos);
                float inverseDist = dist > 0 ? 1.0f / dist : float.MaxValue;
                m_avoidance = m_avoidance - (Vector3.Normalize(otherPos - transform.position) * inverseDist);
                Debug.Log(inverseDist);
            }
            else
            {

                Debug.DrawRay(transform.position, rayDirection * m_avoidanceRange, Color.green);
            }
        }

        m_destination = Vector3.Normalize(m_destinationObj.transform.position - transform.position); 



        m_rb.velocity = Vector3.Normalize(   (m_avoidance * m_avoidanceStr) +  (m_destination * m_destinationStr)    ) * 10;

        if (m_rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(m_rb.velocity.y, m_rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
        }
    }

}
