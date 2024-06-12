using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class boids : MonoBehaviour
{
    [SerializeField] float m_avoidanceStr = 2;
    [SerializeField] float m_destinationStr = 1;
    [SerializeField] float m_maxAvoidanceStr = 10;
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

    [SerializeField] private Vector3 m_avoidance;
    [SerializeField] private Vector3 m_destination;

    List<Goal> filteredGoals = new List<Goal>();
    void Start()
    {
        InitializeComponents();
   
    }

    void Update()
    {
        
        if (!m_jailed)
        {
            m_hasBeenJailed = false;
            ProcessMovement();
            HandleGoalChange();
        }
    }

    public void ProcessBoid()
    {
        if (m_jailed && !m_hasBeenJailed)
        {
            m_hasBeenJailed = true;
            MoveToJail();
        }
        /*
        if (m_destinationObj == null)
        {
            AssignNewGoal();
        }*/
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
                if (boidRef != null && boidRef.m_taggable)
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
                /*
                m_boidManagerRef.m_taggable.Remove(gameObject);

                int index = m_boidManagerRef.m_goals.FindIndex(r => r.obj == gameObject);
                Goal updatedGoal = m_boidManagerRef.m_goals[index];
                updatedGoal.weight = 0;
                m_boidManagerRef.m_goals[index] = updatedGoal;
                */
            }
        }
        else
        {
            if ((team == 0 && Mathf.Sign(transform.position.x) == -1) ||
                (team != 0 && Mathf.Sign(transform.position.x) == 1))
            {
                m_taggable = true;
               /*
                m_boidManagerRef.m_taggable.Add(gameObject);

                int index = m_boidManagerRef.m_goals.FindIndex(r => r.obj == gameObject);
                Goal updatedGoal = m_boidManagerRef.m_goals[index];
                updatedGoal.weight = 20;
                m_boidManagerRef.m_goals[index] = updatedGoal;
               */
            }
        }
    }
    public void initGoals()
    {

        foreach (var goal in m_boidManagerRef.m_goals)
        {
            if (goal.team != team)
            {
                filteredGoals.Add(goal);
            }
        }
    }
    public void UpdateGoalWeight(Goal _goal , int _weight)
    {
        int i = filteredGoals.FindIndex(r => r.obj == _goal.obj );
        Goal updatedGoal = new Goal();
        updatedGoal = filteredGoals[i];
        updatedGoal.weight = _weight;
        filteredGoals[i] = updatedGoal;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        boids otherBoid = collision.GetComponent<boids>();
        if (otherBoid == null)
        {
            return;
        }
        else
        {
            if (otherBoid.m_taggable && otherBoid.team != team)
            {
                otherBoid.m_jailed = true;
                otherBoid.m_taggable = false;
                m_destinationObj = null;
            }
        }
        if (collision.gameObject == m_destinationObj)
        {
            m_destinationObj = null;
            return;
        }
    }


}