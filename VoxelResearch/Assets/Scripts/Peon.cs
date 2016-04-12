using System.Collections;
using System.Threading;
using System;

namespace Moonray.Threading
{
    [Serializable]
    public class Peon
    {
        private Thread m_Thread;
        public bool working = false;
        public int priority;
        private Action m_DoubleCheck;

        public Peon(int _priority, Action func, Func<bool> check)
        {
            priority = _priority;
            m_Thread = new Thread(() => func());

            if (check())
            {
                GetToWork();
            }
            else
            {

            }
        }

        public void NewJob(int _priority, Action func, Func<bool> check)
        {
            if (!m_Thread.IsAlive)
            {
                priority = _priority;
                m_Thread.Abort();

                m_Thread = new Thread(() => func());

                if (check())
                {
                    GetToWork();
                }
                else
                {

                }
            }
        }

        public void GetToWork()
        {
            m_Thread.Start();
            working = true;
        }

        public void EndWork()
        {
            m_Thread.Join(1);
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
}
