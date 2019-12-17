using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitVent.Metadata.Session.Impl
{
    public class InMemorySessionManager<TSession> : ISessionManager<TSession>
        where TSession : ISession
    {
        public TimeSpan? SessionTimeout { get; set; }

        private readonly IDictionary<Guid, TSession> sessionDictionary = new ConcurrentDictionary<Guid, TSession>();

        public InMemorySessionManager()
            : this(null)
        {
            ;
        }

        public InMemorySessionManager(TimeSpan? timeout)
        {
            SessionTimeout = timeout;
        }

        public void SaveSession(TSession session)
        {
            SaveSession(session, true);
        }

        protected void SaveSession(TSession session, bool markModified)
        {
            if (session == null)
                throw new ArgumentNullException("session", "Cannot save null session.");

            if (markModified)
                session.MarkModified();

            sessionDictionary[session.SessionToken] = session;
        }

        public TSession GetSession(Guid sessionToken)
        {
            TSession session;

            var found = sessionDictionary.TryGetValue(sessionToken, out session);

            if (!found)
                return default(TSession);

            if (session.IsOpen)
            {
                if (IsExpired(session))
                {
                    session.Close();
                }
                else
                {
                    session.MarkAccessed();
                }

                SaveSession(session, false);
            }

            return session;
        }

        public IEnumerable<TSession> GetSessions(Func<TSession, bool> predicate)
        {
            return sessionDictionary.Values.Where(predicate).ToArray();
        }

        public void CloseSession(Guid sessionToken)
        {
            sessionDictionary.Remove(sessionToken);
        }

        protected virtual void CloseStaleSessions()
        {
            var now = DateTime.Now;
            var oldSessions = sessionDictionary.Where(session => IsClosedOrExpired(session.Value, now));

            foreach (var session in oldSessions)
            {
                CloseSession(session.Key);
            }
        }

        public virtual void CleanUpOldSessions()
        {
            CloseStaleSessions();
        }

        protected bool IsExpired(TSession session, DateTime? now = null)
        {
            return SessionTimeout.HasValue && ((now ?? DateTime.Now) - session.AccessTime > SessionTimeout);
        }

        protected bool IsClosedOrExpired(TSession session, DateTime? now = null)
        {
            return !session.IsOpen || IsExpired(session, now);
        }

    }
}