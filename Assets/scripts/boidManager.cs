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

      
    }

    private void Update()
    {
        if (!initCompleted)
        {
            foreach (GameObject boid in m_boids)
            {
                boid.GetComponent<boids>().RegisterBoidAsGoal();
                initCompleted = true;
            }
        }

        foreach (GameObject boid in m_bluTeam)
        {
            boids boidRef = boid.GetComponent<boids>();
            boidRef.ProcessBoid();
            boidRef.m_destinationObj = GetRandomGoal(1, boidRef.m_destinationObj).Value.obj;
            if (boidRef.m_taggable) {
                if (boidRef.m_jailed) { 
                    UpdateGoalWeight(m_goals.Find(r => r.obj == boid),0);
                }
                else
                {
                    UpdateGoalWeight(m_goals.Find(r => r.obj == boid), 10);
                }
            }

        }

        foreach (GameObject boid in m_redTeam)
        {
            boids boidRef = boid.GetComponent<boids>();
            boidRef.ProcessBoid();
            boidRef.m_destinationObj = GetRandomGoal(0, boidRef.m_destinationObj).Value.obj;
            if (boidRef.m_taggable)
            {
                if (boidRef.m_jailed)
                {
                    UpdateGoalWeight(m_goals.Find(r => r.obj == boid), 0);
                }
                else
                {
                    UpdateGoalWeight(m_goals.Find(r => r.obj == boid), 10);
                }
            }
        }

    }
    public void UpdateGoalWeight(Goal _goal, int _weight)
    {
        int i = m_goals.FindIndex(r => r.obj == _goal.obj);
        Goal updatedGoal = new Goal();
        updatedGoal = m_goals[i];
        updatedGoal.weight = _weight;
        m_goals[i] = updatedGoal;
    }
    [ContextMenu("Do Something")]                  
    public Goal? GetRandomGoal(byte _team, GameObject _currentGoal)
    {
        List<Goal> filteredGoals = new List<Goal>();
        foreach (var goal in m_goals)
        {
            if (goal.team != _team)
            {
                filteredGoals.Add(goal);
            }
        }

        if (filteredGoals.Count == 0)
        {
            return null;
        }

       
        float highestWeight = 0f;
        List<Goal> highestGoals = new List<Goal>();
        foreach (var goal in filteredGoals)
        {
         
            if (highestWeight < goal.weight)
            {
                highestGoals = new List<Goal>();
                highestGoals.Add(goal);
                continue;
            }
            if (highestWeight == goal.weight)
            {
                highestGoals.Add(goal);
            }
          
        }
        
        int randomIndex = Random.Range(0, highestGoals.Count-1);

        if (m_goals.Find(r => r.obj == _currentGoal).weight >= highestGoals[randomIndex].weight)
        {
            return m_goals.Find(r => r.obj == _currentGoal);
        }
        else { 
            return highestGoals[randomIndex];
        }
    }
}
