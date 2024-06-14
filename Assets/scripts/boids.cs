using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
public class boids : MonoBehaviour
{
    [SerializeField] float m_avoidanceStr = 2;
    [SerializeField] float m_destinationStr = 1;
    [SerializeField] float m_maxAvoidanceStr = 10;

    public bool m_playable;
    public GameObject m_playerHighlight;

    public Vector2 m_direction;
    public int m_detectionRes = 8;
    public RaycastHit2D[][] m_hitArray;
    public float m_avoidanceRange = 1;
    public float m_speed = 10;

    public bool m_canMove = true;
    public bool m_jailed = false;
    private bool m_hasBeenJailed = false;
    public bool m_taggable = false;
    public bool m_hasFlag = false;

    public float m_goalChangeCooldown = 5f;
    private float m_goalChangeTimer = 0f;
    public float m_changeThreshold = 1.0f;

    public GameObject m_flagRef = null;
    public byte team = 0;
    public GameObject m_destinationObj;

    private Rigidbody2D m_rb;
    private BoxCollider2D m_collider;
    private boidManager m_boidManagerRef;
    public GameObject m_wanderFollow;

    [SerializeField] private Vector3 m_avoidance;
    [SerializeField] private Vector3 m_destination;

    public List<Goal> personalGoals = new List<Goal>();
    void Start()
    {
        InitializeComponents();

        if (m_playable)
        {
            Instantiate(m_playerHighlight, transform);
           // m_destinationObj = m_boidManagerRef.gameObject;
        }
    }
    private void LateUpdate()
    {
        if (!m_jailed) { 
            if (!m_playable) { 
           
            
                m_hasBeenJailed = false;
                ProcessMovement();
              
                ProcessGoal();
            }
            else
            {
                CheckTeamSpecific();
                m_destination = Vector3.Normalize(m_boidManagerRef.transform.position - transform.position);


                m_rb.velocity = (new Vector3(1, 1, 0)) + Vector3.Normalize((m_destination * m_destinationStr)) * m_speed * 5;

            }
        }
        
        else if (!m_hasBeenJailed)
        {
            m_hasBeenJailed = true;
            MoveToJail();
        }
    }



    private void InitializeComponents()
    {
        m_boidManagerRef = GameObject.FindWithTag("boidManager").GetComponent<boidManager>();
        m_collider = GetComponent<BoxCollider2D>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    public void RegisterBoidAsGoal()
    {
        Goal ownGoal;
        ownGoal.name = "boid";
        ownGoal.weight = 0;
        ownGoal.obj = gameObject;
        ownGoal.team = team;
        m_boidManagerRef.m_goals.Add(ownGoal);
    }

    private void ProcessMovement()
    {
        CheckTeamSpecific();
        ResetVectors();
        PerformRaycasting();
        CalculateDestination();
        UpdateVelocity();
        RotateBoid();
    }

    private void HandleGoalChange()
    {
        m_goalChangeTimer = m_goalChangeCooldown;
        
    }

    private void MoveToJail()
    {
        int index = m_boidManagerRef.m_goals.FindIndex(goal => goal.team == (team ^ 1) && goal.name == "jail");
        GameObject jailRef = m_boidManagerRef.m_goals[index].obj;
        Vector2 pos = jailRef.transform.position;
        transform.position = new Vector3(pos.x + Random.Range(-3.0f, 3.0f), pos.y + Random.Range(-3.0f, 3.0f));
        m_hasFlag = false;

     
        int boidIndex = m_boidManagerRef.m_goals.FindIndex(goal => goal.obj == gameObject);
        Goal updatedGoal = m_boidManagerRef.m_goals[boidIndex];
        updatedGoal.weight = 0;
        m_boidManagerRef.m_goals[boidIndex] = updatedGoal;

  
        Goal jailGoal = m_boidManagerRef.m_goals[index];
        m_boidManagerRef.m_goals[index] = jailGoal;
        m_rb.velocity = Vector3.zero;
    }

    private void ResetVectors()
    {
        m_destination = Vector2.zero;
        m_avoidance = Vector2.zero;
    }

    private void PerformRaycasting()
    {
        m_hitArray = new RaycastHit2D[m_detectionRes][];
        float rayDirectionDiv = 360 / m_detectionRes;

        for (int i = 0; i < m_detectionRes; i++)
        {
            float angle = Mathf.Deg2Rad * (rayDirectionDiv * i);
            Vector2 rayDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            m_hitArray[i] = Physics2D.RaycastAll(transform.position, rayDirection, m_avoidanceRange);

            bool collided = false;
            Vector3 otherPos = Vector2.zero;
            foreach (RaycastHit2D hit in m_hitArray[i])
            {
                Collider2D collider = hit.collider;
                boids boidRef = collider.GetComponent<boids>();
                if (boidRef != null && (boidRef.m_taggable) )
                {
                    continue;
                }
             
                if (collider == m_collider || collider.gameObject == m_destinationObj || collider.gameObject.CompareTag("noAvoid"))
                {
                    continue;
                }
                else
                {
                    collided = true;
                    otherPos = hit.point;
                    break;
                }
            }
            if (collided)
            {
                Debug.DrawRay(transform.position, rayDirection * m_avoidanceRange, Color.red);
                float dist = Vector2.Distance(transform.position, otherPos);
                float inverseDist = (dist > 0 ? (-(m_maxAvoidanceStr / m_avoidanceRange) * dist) + m_maxAvoidanceStr : float.MaxValue);
                m_avoidance = m_avoidance - (Vector3.Normalize(otherPos - transform.position) * inverseDist);
            }
            else
            {
                Debug.DrawRay(transform.position, rayDirection * m_avoidanceRange, Color.green);
            }
        }
    }

    private void CalculateDestination()
    {
        if (m_destinationObj != null)
        {
            m_destination = Vector3.Normalize(m_destinationObj.transform.position - transform.position);
        }
        else
        {
            m_destination = Vector3.zero;
        }
    }

    private void UpdateVelocity()
    {
        m_rb.velocity = (new Vector3(1, 1, 0)) + Vector3.Normalize((m_avoidance * m_avoidanceStr) + (m_destination * m_destinationStr)) * m_speed;
    }

    private void RotateBoid()
    {
        if (m_rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(m_rb.velocity.y, m_rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
    }

    private void CheckTeamSpecific()
    {

        
        if (m_taggable)
        {
            if ((team == 0 && Mathf.Sign(transform.position.x) == 1) ||
                (team != 0 && Mathf.Sign(transform.position.x) == -1))
            {
                m_taggable = false;

            }
        }
        else
        {
            if ((team == 0 && Mathf.Sign(transform.position.x) == -1) ||
                (team != 0 && Mathf.Sign(transform.position.x) == 1))
            {
                m_taggable = true;

            }
        }
    }
    public void initGoals()
    {
        
        foreach (var goal in m_boidManagerRef.m_goals)
        {
            if (goal.team != team)
            {
                personalGoals.Add(goal);
            }
        }
        Goal wanderGoal = new Goal();
        wanderGoal.weight = 10.0f;
        wanderGoal.team = team;
        wanderGoal.obj = m_wanderFollow;
        wanderGoal.name = "wander";
        personalGoals.Add(wanderGoal);
    }
    public void UpdateGoalWeight(Goal _goal , int _weight)
    {
        int i = personalGoals.FindIndex(r => r.obj == _goal.obj );
        Goal updatedGoal = new Goal();
        updatedGoal = personalGoals[i];
        updatedGoal.weight = _weight;
        personalGoals[i] = updatedGoal;
    }
    public void ProcessGoal()
    {
        List<(Goal goal, int weight)> goalsToUpdate = new List<(Goal goal, int weight)>();
      
        foreach (Goal _goal in personalGoals)
        {
            if (_goal.name == "flag")
            {
                flagHolder flagRef = _goal.obj.GetComponent<flagHolder>();
                if (flagRef.m_flagsTaken >= flagRef.m_maxFlags)
                {
                    goalsToUpdate.Add((_goal, 0));
                }
            }
            if (_goal.name == "boid")
            {
                boids boidRef = _goal.obj.GetComponent<boids>();

                if (boidRef.m_jailed || m_taggable)
                {

                  
                    goalsToUpdate.Add((_goal, 0));
                    m_destinationObj = null;
                }
                else if(boidRef.m_taggable)
                {
                    goalsToUpdate.Add((_goal, 300));
                }
                
            }
            if (_goal.name == "jail")
            {
                int newWeight = 0;
                foreach (GameObject obj in m_boidManagerRef.m_boids)
                {
                    
                    boids boidRef = obj.GetComponent<boids>();
                    if (boidRef.team != team)
                    {
                        continue;
                    }

                    if (boidRef.m_jailed)
                    {
                        newWeight += 110;

                    }
                }
                goalsToUpdate.Add((_goal, newWeight));
            }
        }

        // Apply the updates after the iteration
        foreach (var (goal, weight) in goalsToUpdate)
        {
            UpdateGoalWeight(goal, weight);
        }

        var randomGoal = GetRandomGoal(1, m_destinationObj);
        if (randomGoal.HasValue)
        {
            m_destinationObj = randomGoal.Value.obj;
        }
    
        
    }


   
    public Goal? GetRandomGoal(byte _team, GameObject _currentGoal)
    {
        float highestWeight = 0f;
        if (m_hasFlag)
        {
            return m_boidManagerRef.m_goals.Find(r => r.team == team && r.name == "flag");
        }
        List<Goal> highestGoals = new List<Goal>();
        Vector3 currentPosition = transform.position;

        foreach (var goal in personalGoals)
        {
            if(goal.weight <= 0)
            {
                continue;
            }
            float distance = Vector3.Distance(currentPosition, goal.obj.transform.position);


            float adjustedWeight = goal.weight- (distance * 0.1f);
            if (goal.weight != 0 && adjustedWeight == 0)
            {
                adjustedWeight = 0.01f;
                
            }
            if (highestWeight < adjustedWeight)
            {
                highestGoals = new List<Goal> { goal };
                highestWeight = adjustedWeight;
            }
            else if (highestWeight == adjustedWeight)
            {
                highestGoals.Add(goal);
            }
       }
       

        int randomIndex = Random.Range(0, highestGoals.Count);

     
        Goal currentGoal = personalGoals.Find(r => r.obj == _currentGoal);
        if (highestGoals.Count == 0)
        {
   
            return currentGoal;
        }
        if (currentGoal.obj != null && currentGoal.weight >= highestGoals[randomIndex].weight)
        {
            return currentGoal;
        }
        else
        {
            return highestGoals[randomIndex];
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        boids otherBoid = collision.GetComponent<boids>();
        if (otherBoid == null)
        {
            if (m_boidManagerRef.m_goals.Find(r => r.obj == collision.gameObject && r.name == "flag" && r.team == team).obj == m_destinationObj)
            {
              
                if (m_hasFlag)
                {
                    Vector2 pos = m_destinationObj.transform.position;
                    m_flagRef.GetComponent<flag>().m_boidFollow = null;
                    m_flagRef.transform.position = new Vector3(pos.x + Random.Range(-3.0f, 3.0f), pos.y + Random.Range(-3.0f, 3.0f));
                    m_hasFlag = false;
                    m_destinationObj = null;
                }
            }
            else if (m_boidManagerRef.m_goals.Find(r => r.name == "jail" && r.team != team).obj == collision.gameObject && !m_jailed)
            {
                foreach (GameObject boids in m_boidManagerRef.m_boids)
                {
                    boids boidRef = boids.GetComponent<boids>();
                    if(boidRef.team == team )
                    {
                        if (boidRef.m_jailed)
                        {
                            boidRef.m_jailed = false;
                        }
                    }
                }
            }

        }
        else
        {
            if (otherBoid.m_taggable && otherBoid.team != team)
            {
                otherBoid.m_jailed = true;
                otherBoid.m_taggable = false;

                m_destinationObj = null;
                if (otherBoid.m_hasFlag)
                {
                    
                    otherBoid.m_flagRef.GetComponent<flag>().m_boidFollow = null;
                    
                    otherBoid.m_hasFlag = false;
                    GameObject flahHolderRef = m_boidManagerRef.m_goals.Find(r => r.team == team && r.name == "flag").obj;
                    flahHolderRef.GetComponent<flagHolder>().m_flagsTaken--;
                    Vector2 pos = flahHolderRef.transform.position;
                    otherBoid.m_flagRef.transform.position = new Vector3(pos.x + Random.Range(-3.0f, 3.0f), pos.y + Random.Range(-3.0f, 3.0f));


                    otherBoid.m_destinationObj = null;
                }
            }
        }
        if (collision.gameObject == m_destinationObj)
        {
            Goal currentGoal = personalGoals.Find(r => r.obj == m_destinationObj);
            m_destinationObj = null;

            return;
        }
    }
}
