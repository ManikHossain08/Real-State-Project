using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Metadata.Session.Impl
{
    public class BasicSession : ISession
    {
        public Guid SessionToken { get; private set; }

        public DateTime CreationTime { get; private set; }
        public DateTime AccessTime { get; private set; }
        public DateTime ModificationTime { get; private set; }

        public bool IsOpen { get; private set; }

        public BasicSession()
        {
            SessionToken = Guid.NewGuid();
            CreationTime = DateTime.Now;
            AccessTime = CreationTime;
            ModificationTime = CreationTime;

            IsOpen = true;
        }

        public BasicSession(ISession session)
        {
            SessionToken = session.SessionToken;
            CreationTime = session.CreationTime;
            AccessTime = session.AccessTime;

            MarkModified();

            IsOpen = session.IsOpen;
        }

        public void MarkAccessed()
        {
            AccessTime = DateTime.Now;
        }

        public void MarkModified()
        {
            ModificationTime = DateTime.Now;
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }
    }
}
