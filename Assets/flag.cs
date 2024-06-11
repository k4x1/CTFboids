using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flag : MonoBehaviour
{
    public Transform m_boidFollow;
    
    // Update is called once per frame
    void Update()
    {
        if(m_boidFollow != null)
        {
            transform.position = m_boidFollow.position;
        }
    }
}
