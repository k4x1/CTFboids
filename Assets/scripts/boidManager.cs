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
                m_redTeam.Add(gameObject);
            }
            else
            {
                go.GetComponent<boids>().team = 1;
                go.GetComponent<SpriteRenderer>().color = Color.blue;
                m_bluTeam.Add(gameObject);
            }
        }
    }
    private void Update()
    {
        for(int i = 0; i< m_redTeam.Count; i++)
        {
            if (m_redTeam[i].GetComponent<boids>().m_jailed)
            {

            }
        }
    }
    public Goal? GetRandomGoal(byte team)
    {
        // Filter goals by the specified team
        List<Goal> filteredGoals = new List<Goal>();
        foreach (var goal in m_goals)
        {
            if (goal.team == team)
            {
                filteredGoals.Add(goal);
            }
        }

        // If no goals are found for the specified team, return null
        if (filteredGoals.Count == 0)
        {
            return null;
        }

        // Calculate the total weight of the filtered goals
        float totalWeight = 0f;
        foreach (var goal in filteredGoals)
        {
            totalWeight += goal.weight;
        }

        // Generate a random number within the range of the total weight
        float randomWeight = Random.Range(0f, totalWeight);

        // Select the goal based on the random weight
        float cumulativeWeight = 0f;
        foreach (var goal in filteredGoals)
        {
            cumulativeWeight += goal.weight;
            if (randomWeight <= cumulativeWeight)
            {
                return goal;
            }
        }

        // Fallback in case something goes wrong (shouldn't happen)
        return filteredGoals[filteredGoals.Count - 1];
    }
}
