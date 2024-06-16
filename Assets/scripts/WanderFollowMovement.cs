using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WanderFollowMovement : MonoBehaviour
{
    public float speed = 1.0f; // Speed of the movement
    public float radius = 5.0f; // Radius of the circular movement
    public float randomness = 0.5f; // Randomness factor
    private Quaternion initialRotation;
    public GameObject creator;
    private Vector2 center;
    private float angle;
    public byte team = 0;

    void Start()
    {
        angle = Random.Range(0f, 2f * Mathf.PI);
        StartCoroutine(UpdateRandomness());
    }

    void Update()
    {
        center = creator.transform.position;
        // Calculate the new position
        angle += randomness * speed * Time.deltaTime;
        float x = transform.position.x;
        float y = transform.position.y;

        // Check for clamping and flip direction if necessary
        bool clampedX = false;
        bool clampedY = false;

        if (team == 0)
        {
            if (x < 0 || x > 40)
            {
                clampedX = true;
                
            }
        }
        else
        {
            if (x < -40 || x > 0)
            {
                clampedX = true;
               
            }
        }

        if (y < -20 || y > 20)
        {
            clampedY = true;
            
        }

        // Apply clamping
        if (!creator.GetComponent<boids>().m_canGoToEnemySide) { 
          //  x = (team == 0) ? Mathf.Clamp(x, 0, 40) : Mathf.Clamp(x, -40, 0);
           // y = Mathf.Clamp(y, -20, 20);
        }
        if ((clampedX || clampedY)&&!creator.GetComponent<boids>().m_taggable)
        {
            angle += 180;
            randomness *= -1;
        }
        x = center.x + Mathf.Cos(angle) * radius;
        y = Mathf.Sin(angle) * radius;

        // Update position
        transform.localPosition = new Vector3(x, y, transform.localPosition.z);

       
    }
   

    private IEnumerator UpdateRandomness()
    {
        while (true)
        {
            randomness = Random.Range(-1, 1);
            yield return new WaitForSeconds(10f);
        }
    }

    void OnDrawGizmos()
    {
        // Set the color of the gizmo
     
        if (team == 0)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        // Draw a sphere at the object's position
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
