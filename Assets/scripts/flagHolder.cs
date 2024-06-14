using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;


public class flagHolder : MonoBehaviour
{
    public byte m_team = 0;
    public int m_maxFlags = 4;
    public GameObject m_flagObj;
    public boidManager m_boidManager;
    //public GameObject[] m_flags = new GameObject[m_maxFlags];
    public int m_flagsTaken = 0;
    public Dictionary<int, GameObject> m_flags = new Dictionary<int, GameObject>();
    private void Start()
    {
        Vector2 pos = transform.position;
        for (int i = 0; i < m_maxFlags; i++)
        {
            GameObject newFlag = Instantiate(m_flagObj, new Vector3(pos.x + Random.Range(-3.0f, 3.0f), pos.y + Random.Range(-3.0f, 3.0f)), Quaternion.identity);
            newFlag.GetComponent<SpriteRenderer>().color = m_team == 1 ? Color.blue : Color.red;

            m_flags.Add(i, newFlag);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        boids otherBoid = collision.GetComponent<boids>();
        if (otherBoid == null)
        {
            return;
        }
        if (otherBoid.team != m_team)
        {
            if (!otherBoid.m_hasFlag && m_flags.Count > 0 && m_flagsTaken < m_maxFlags)
            {
                otherBoid.m_hasFlag = true;
                otherBoid.m_flagRef = m_flags[m_flagsTaken];
                m_flags[m_flagsTaken].GetComponent<flag>().m_boidFollow = otherBoid.transform;
                //      otherBoid.m_destinationObj = m_boidManager.m_goals.Find(goal => goal.team == (m_team^ 1) && goal.name == "flag").obj; 
                m_flagsTaken++;
            }
        }
    }
    private void Update()
    {
        
    }
}
