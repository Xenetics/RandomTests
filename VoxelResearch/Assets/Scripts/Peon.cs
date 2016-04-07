using System.Collections;
using System.Threading;
using System;

[Serializable]
public class Peon
{
    private Thread m_Thread;
    public bool working = false;
    public int priority;

    public void Init(Action func)
    {
        m_Thread = new Thread(() => func());
    }

    public void GetToWork()
    {
        m_Thread.Start();
        working = true;
    }

    public void EndWork()
    {
        m_Thread.Abort();
        working = false;
    }

    public void Status()
    {
        if (!m_Thread.IsAlive)
        {
            EndWork();
        }
	}
}
