using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jail : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> jailedObj;
    byte team = 0;


    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        boids otherBoid = collision.GetComponent<boids>();
        if (otherBoid == null)
        {
            return;
        }
        if (otherBoid.m_jailed)
        {
            return;
        }
        if (otherBoid.team != team) 
        { 
            
        }
    }
}
