using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ThreadLord : MonoBehaviour
{
    [Serializable]
    public class Job
    {
        public Job(ThreadTypes _type, int _priority, Action _job)
        {
            type = _type;
            priority = _priority;
            job = _job;
        }
        public ThreadTypes type;
        public int priority;
        public Action job;
    }

    public enum ThreadTypes { Water, Sound, Terrain, Serialization }
    
    [SerializeField]
    private int[] m_ThreadCounts;
    [SerializeField]
    private List<Peon>[] m_ThreadLists = { new List<Peon>(), new List<Peon>(), new List<Peon>(), new List<Peon>() };

    private List<Job> m_Backlog = new List<Job>();

    void Start()
    {
        for (int i = 0; i < 10; ++i)
        {
            int id = i;
            int priority = 0;
            CreateJob(ThreadTypes.Water, priority, () => Nothing(priority, new Vector3(1 * i,0,0)));
            //Debug.Log("Thread Count: " + m_ThreadLists[0].Count);
        }

        for (int i = 0; i < 10; ++i)
        {
            //m_ThreadLists[(int)ThreadTypes.Water][i].GetToWork();
        }
    }

    //TESTING ONLY
    public GameObject box;
    public int totalBoxes;
    public List<GameObject> Boxes = new List<GameObject>();
    public List<Vector3> BoxQue = new List<Vector3>();
    public int threadsMade = 0;
    System.Random rnd = new System.Random(DateTime.Now.Millisecond);
    public void Nothing(int priority, Vector3 spawnPoint)
    {
        BoxQue.Add(spawnPoint);

        System.Threading.Thread.Sleep(17);
        
        if (Boxes.Count + BoxQue.Count < totalBoxes)
        {
            Vector3 newSpawn = spawnPoint;
            newSpawn.x += rnd.Next(0, 2);
            newSpawn.y += rnd.Next(0, 2);
            newSpawn.z += rnd.Next(0, 2);
            CreateJob(ThreadTypes.Water, priority, () => Nothing(priority + 1, newSpawn));
            Debug.Log("Building...");
        }
    }

    public void NewBox(Vector3 spawnPoint)
    {
        GameObject newBox = Instantiate(box, spawnPoint, Quaternion.identity) as GameObject;
        Boxes.Add(newBox);
    }

    void Update()
    {
        UpdateWorkers();

        HandleBacklog();

        threadsMade = m_ThreadLists[(int)ThreadTypes.Water].Count;
        int count = BoxQue.Count;
        for(int i = 0; i < count; i++)
        { 
            NewBox(BoxQue[0]);
            BoxQue.RemoveAt(0);
        }
    }

    public void CreateJob(ThreadTypes type, int priority, Action job)
    {
        bool jobTaken = false;
        for(int i = 0; i < m_ThreadLists[(int)type].Count; ++i)
        {
            if(!m_ThreadLists[(int)type][i].working)
            {
                m_ThreadLists[(int)type][i].NewJob(priority, job);
                jobTaken = true;
                break;
            }
        }

        if(!jobTaken && m_ThreadLists[(int)type].Count < m_ThreadCounts[(int)type])
        {
            CreatePeon(type, priority, job);
        }
        else
        {
            BacklogJob(type, priority, job);
        }
    }

    public void CreatePeon(ThreadTypes type, int priority, Action job)
    {
        Peon newPeon;
        newPeon = new Peon(priority, job);
        m_ThreadLists[(int)type].Add(newPeon);
        SortList(ref m_ThreadLists[(int)type]);
    }

    public void BacklogJob(ThreadTypes type, int priority, Action job)
    {
        m_Backlog.Add(new Job(type, priority, job));
    }

    private void SortList(ref List<Peon> list)
    {
        List<Peon> newList = list.OrderByDescending(o => o.priority).ToList();
        list = newList;
    }

    private void SortBacklogs(ref List<Job> list)
    {
        List<Job> newList = list.OrderByDescending(o => o.priority).ToList();
        list = newList;
    }

    private void UpdateWorkers()
    {
        for (int i = 0; i < m_ThreadLists.Length; ++i)
        {
            if (m_ThreadLists[i].Count >= m_ThreadCounts[(int)ThreadTypes.Water])
            {
                for (int j = 0; j < m_ThreadLists[i].Count; ++j)
                {
                    m_ThreadLists[i][j].Status();
                }
            }
        }
    }

    private void HandleBacklog()
    {
        //SortBacklogs(ref m_Backlog);
        bool jobGiven = false;
        if (m_Backlog.Count > 0)
        {
            for (int i = 0; i < m_Backlog.Count; ++i)
            {
                if(m_Backlog[i] == null)
                {
                    m_Backlog.RemoveAt(i);
                    return;
                }
                for (int j = 0; j < m_ThreadLists[(int)(m_Backlog[i].type)].Count; ++j)
                {
                    if (!m_ThreadLists[(int)(m_Backlog[i].type)][j].working)
                    {
                        m_ThreadLists[(int)(m_Backlog[i].type)][j].NewJob(m_Backlog[i].priority, m_Backlog[i].job);
                        m_Backlog.RemoveAt(i);
                        jobGiven = true;
                        return;
                    }
                }
            }
        }
    }
}
