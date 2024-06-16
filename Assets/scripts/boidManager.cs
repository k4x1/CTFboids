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
  
    public List<GameObject> m_taggable;
    public List<GameObject> m_boids;
    public List<GameObject> m_redTeam;
    public List<GameObject> m_bluTeam;
    public List<Goal> m_goals;
    public float m_boidSpeed = 100;
    public bool m_win = false;




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
            boids boidRef = boid.GetComponent<boids>();
            boidRef.RegisterBoidAsGoal();
            boidRef.m_wanderFollow = new GameObject("wanderFollow");
            WanderFollowMovement wanderRef = boidRef.m_wanderFollow.AddComponent<WanderFollowMovement>();
            wanderRef.creator = boidRef.gameObject;
            wanderRef.team = boidRef.team;
           
            //yeah i couldve made a prefab but the sunk cost falacy stoped me

        }
        foreach (GameObject boid in m_boids)
        {
            boid.GetComponent<boids>().initGoals();
        }

    }

    private void Update()
    {
        int rInOtherSide = 0;
       
        
        for (int i = 0; i < m_redTeam.Count; i++)
        {

            boids boidRef = m_redTeam[i].GetComponent<boids>();
            boidRef.m_canGoToEnemySide = true;
            if (rInOtherSide == 1)
            {
                boidRef.m_canGoToEnemySide = false;
            }
            else if (rInOtherSide == 2)
            {
                boidRef.m_canGoToEnemySide = true;
            }
            else { 
                

                if (boidRef.m_taggable && !boidRef.m_jailed)
                {
                    i = 0;
                    rInOtherSide  = 1;
                }
                else if( i == m_redTeam.Count)
                {
                    i = 0;
                    rInOtherSide = 2;
                }
            
            }
        }
        int bInOtherSide = 0;


        for (int i = 0; i < m_bluTeam.Count; i++)
        {

            boids boidRef = m_bluTeam[i].GetComponent<boids>();
            boidRef.m_canGoToEnemySide = true;
            if (bInOtherSide == 1)
            {
                boidRef.m_canGoToEnemySide = false;
            }
            else if (bInOtherSide == 2)
            {
                boidRef.m_canGoToEnemySide = true;
            }
            else
            {


                if (boidRef.m_taggable && !boidRef.m_jailed)
                {
                    i = 0;
                    bInOtherSide = 1;
                }
                else if (i == m_bluTeam.Count)
                {
                    i = 0;
                    bInOtherSide = 2;
                }

            }
        }

        if (m_win)
        {
            foreach (GameObject boid in m_boids)
            {
                Destroy(boid.GetComponent<boids>().m_wanderFollow);
                Destroy(boid);
                
               
            }
            foreach (Goal goal in m_goals)
            {
                Destroy(goal.obj);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        boids otherBoid = collision.GetComponent<boids>();
        if (otherBoid != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                foreach (GameObject boid in m_boids)
                {
                    boids boidRef = boid.GetComponent<boids>();
                    boidRef.m_playable = false;
                    boidRef.m_playableSet = false;
                }
             
                foreach(GameObject playableBorder in GameObject.FindGameObjectsWithTag("visual"))
                {
                    Destroy(playableBorder);
                }
                otherBoid.m_playable = !otherBoid.m_playable;
            }
        }
    } 

}
