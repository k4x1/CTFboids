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

    private void Start()
    {
        foreach (GameObject go in m_boids) {
            go.GetComponent<boids>().m_speed = m_boidSpeed;
            if (Mathf.Sign(go.transform.position.x) == 1)
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

        ProcessBoids(m_bluTeam);
        ProcessBoids(m_redTeam);
        

    }
    void ProcessBoids(List<GameObject> team)
    {
        for (int i = 0; i < team.Count; i++)
        {
            boids boidRef = team[i].GetComponent<boids>();
            if (boidRef.m_jailed)
            {
                GameObject jailRef = m_goals.Find(goal => goal.team == (boidRef.team ^ 1) && goal.name == "jail").obj;
                Vector2 pos = jailRef.transform.position;
                team[i].transform.position = new Vector3(pos.x + Random.Range(-3.0f, 3.0f), pos.y + Random.Range(-3.0f, 3.0f));
                boidRef.m_speed = 0;
                boidRef.m_hasFlag = false;
                boidRef.m_jailed = false;
            }

            if (boidRef.m_destinationObj == null)
            {
                Goal goal = GetRandomGoal(boidRef.team).Value;
                boidRef.m_destinationObj = goal.obj;
            }
        }
    }
    public Goal? GetRandomGoal(byte team)
    {
        List<Goal> filteredGoals = new List<Goal>();
        foreach (var goal in m_goals)
        {
            if (goal.team != team)
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
