using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WanderFollowMovement : MonoBehaviour
{

    public Vector2 randomPos;
    private Quaternion initialRotation;
    public GameObject creator;
    private Vector2 center;
    private float angle;
    public byte team = 0;

    void Start()
    {
        
        StartCoroutine(UpdateRandomness());
    }

    void Update()
    {
        transform.position = randomPos;
    }
   

    private IEnumerator UpdateRandomness()
    {
        while (true)
        {
            randomPos = new Vector2(Random.Range(0, -40*Mathf.Sign(team-1)), Random.Range(0, -20 * Mathf.Sign(team - 1)));
            yield return new WaitForSeconds(1f);
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
