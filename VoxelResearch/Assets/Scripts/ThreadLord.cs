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

    private List<Peon> m_WaterThreads = new List<Peon>();
    private List<Peon> m_SoundThreads = new List<Peon>();
    private List<Peon> m_TerrainThreads = new List<Peon>();
    private List<Peon> m_SerializationThreads = new List<Peon>();

    private List<Job> m_Backlog = new List<Job>();

    void Start()
    {
        //for (int i = 0; i < 1; ++i)
        //{
        //    CreatePeon(ThreadTypes.Water, 1, () => Nothing(0, i));
        //    Debug.Log("Thread Count: " + m_WaterThreads.Count);
        //}

        //for (int i = 0; i < m_WaterThreads.Count; ++i)
        //{
        //    m_WaterThreads[i].GetToWork();
        //    Debug.Log(i + " Thread Started");
        //}
    }

    void Update()
    {
        UpdateWorkers();
    }

    public void StartJob(ThreadTypes type, int priority, Action job)
    {
        bool jobTaken = false;
        switch (type)
        {
            case ThreadTypes.Water:
                for(int i = 0; i < m_WaterThreads.Count; ++i)
                {
                    if(!m_WaterThreads[i].working)
                    {
                        m_WaterThreads[i].priority = priority;
                        m_WaterThreads[i].Init(job);
                        jobTaken = true;
                        break;
                    }
                }

                if(!jobTaken && m_WaterThreads.Count < m_ThreadCounts[(int)type])
                {
                    CreatePeon(type, priority, job);
                }
                else
                {
                    BacklogJob(type, priority, job);
                }
                break;
            case ThreadTypes.Sound:

                break;
            case ThreadTypes.Terrain:

                break;
            case ThreadTypes.Serialization:

                break;
        }
    }

    public void CreatePeon(ThreadTypes type, int priority, Action job)
    {
        Peon newPeon;
        switch(type)
        {
            case ThreadTypes.Water:
                newPeon = new Peon();
                newPeon.priority = priority;
                newPeon.Init(job);
                m_WaterThreads.Add(newPeon);
                SortList(ref m_WaterThreads);
                break;
            case ThreadTypes.Sound:
                newPeon = new Peon();
                newPeon.priority = priority;
                newPeon.Init(job);
                m_SoundThreads.Add(newPeon);
                break;
            case ThreadTypes.Terrain:
                newPeon = new Peon();
                newPeon.priority = priority;
                newPeon.Init(job);
                m_TerrainThreads.Add(newPeon);
                break;
            case ThreadTypes.Serialization:
                newPeon = new Peon();
                newPeon.priority = priority;
                newPeon.Init(job);
                m_SerializationThreads.Add(newPeon);
                break;
        }
    }

    public void BacklogJob(ThreadTypes type, int priority, Action job)
    {
        m_Backlog.Add(new Job(type, priority, job));
    }

    //TESTING ONLY
    public void Nothing(float tni, int thread)
    {
        int id = thread;
        while(tni < 100f)
        {
            tni += 1f;
            Debug.Log("Thread " + id + " " + tni);
        }

        m_WaterThreads[id].EndWork();
    }

    private void SortList(ref List<Peon> list)
    {
        List<Peon> newList = list.OrderByDescending(o => o.priority).ToList();
        list = newList;
    }

    private void UpdateWorkers()
    {
        for (int i = 0; i < m_WaterThreads.Count; ++i)
        {
            m_WaterThreads[i].Status();
        }
        for (int i = 0; i < m_WaterThreads.Count; ++i)
        {
            m_WaterThreads[i].Status();
        }
        for (int i = 0; i < m_WaterThreads.Count; ++i)
        {
            m_WaterThreads[i].Status();
        }
        for (int i = 0; i < m_WaterThreads.Count; ++i)
        {
            m_WaterThreads[i].Status();
        }
    }

    private void HandleBacklog()
    {
        for(int i = 0; i < m_Backlog.Count; ++i)
        {
            switch (m_Backlog[i].type)
            {
                case ThreadTypes.Water:
                    if (m_WaterThreads.Count < m_ThreadCounts[(int)m_Backlog[i].type])
                    {
                        for (int j = 0; j < m_WaterThreads.Count; ++j)
                        {
                            if (!m_WaterThreads[j].working)
                            {
                                m_WaterThreads[j].priority = m_Backlog[i].priority;
                                m_WaterThreads[j].Init(m_Backlog[i].job);
                                m_Backlog.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    break;
                case ThreadTypes.Sound:

                    break;
                case ThreadTypes.Terrain:

                    break;
                case ThreadTypes.Serialization:

                    break;
            }
            if(m_Backlog[i] == null)
            {
                break;
            }
        }
    }
}
