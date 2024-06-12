using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flag : MonoBehaviour
{
    public Transform m_boidFollow;
    public Vector3 m_position;

    // Update is called once per frame
    private void Start()
    {
        m_position = transform.position;
    }
    void Update()
    {
        if (m_boidFollow != null)
        {
            transform.position = m_boidFollow.position;
        }
        else
        {
            transform.position = m_position;
        }
    }
}
