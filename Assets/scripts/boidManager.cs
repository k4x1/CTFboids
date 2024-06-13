using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Goal
{
    public float weight;
    public byte team;
    public GameObject obj;
    public string name;
}

public class boidManager : MonoBehaviour
{
    public List<GameObject> m_redTeam;
    public List<GameObject> m_bluTeam;
    public List<GameObject> m_taggable;
    public List<GameObject> m_boids;
    public List<Goal> m_goals;
    public float m_boidSpeed = 100;



    private bool initCompleted = false;
    private void Start()
    {
        foreach (GameObject go in m_boids)
        {
            boids boidComponent = go.GetComponent<boids>();
            boidComponent.m_speed = m_boidSpeed;
            if (Mathf.Sign(go.transform.position.x) == 1)
            {
                boidComponent.team = 0;
                go.GetComponent<SpriteRenderer>().color = Color.red;
                m_redTeam.Add(go);
            }
            else
            {
                boidComponent.team = 1;
                go.GetComponent<SpriteRenderer>().color = Color.blue;
                m_bluTeam.Add(go);
            }
        }
        foreach (GameObject boid in m_boids)
        {
            boid.GetComponent<boids>().RegisterBoidAsGoal();

          
        }
        foreach (GameObject boid in m_boids)
        {
            boid.GetComponent<boids>().initGoals();
        }

    }

    private void Update()
    {
       

    }

        

}
