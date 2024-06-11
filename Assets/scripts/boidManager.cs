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

    private void Start()
    {
        foreach (GameObject go in m_boids) { 
            if(Mathf.Sign(go.transform.position.x) == 1)
            {
                go.GetComponent<boids>().team = 0;
                go.GetComponent<SpriteRenderer>().color = Color.red;
                m_redTeam.Add(go);
            }
            else
            {
                go.GetComponent<boids>().team = 1;
                go.GetComponent<SpriteRenderer>().color = Color.blue;
                m_bluTeam.Add(go);
            }
        }
    }
    private void Update()
    {
        
        for(int i = 0; i< m_redTeam.Count-1; i++)
        {
            if (m_redTeam[i].GetComponent<boids>().m_jailed)
            {
              //  Destroy(m_redTeam[i]);
            }
        }
        for (int i = 0; i < m_bluTeam.Count - 1; i++)
        {
            if (m_bluTeam[i].GetComponent<boids>().m_jailed)
            {
               
              //  Destroy(m_bluTeam[i]);
                
            }
        }

    }
    public Goal? GetRandomGoal(byte team)
    {
        List<Goal> filteredGoals = new List<Goal>();
        foreach (var goal in m_goals)
        {
            if (goal.team == team)
            {
                filteredGoals.Add(goal);
            }
        }

        if (filteredGoals.Count == 0)
        {
            return null;
        }

 
        float totalWeight = 0f;
        foreach (var goal in filteredGoals)
        {
            totalWeight += goal.weight;
        }


        float randomWeight = Random.Range(0f, totalWeight);


        float cumulativeWeight = 0f;
        foreach (var goal in filteredGoals)
        {
            cumulativeWeight += goal.weight;
            if (randomWeight <= cumulativeWeight)
            {
                return goal;
            }
        }

        return filteredGoals[filteredGoals.Count - 1];
    }
}
