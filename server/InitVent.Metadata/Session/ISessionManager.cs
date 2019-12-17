using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace InitVent.Metadata.Session
{
    public interface ISessionManager<TSession>
        where TSession : ISession
    {
        /// <summary>
        /// Specifies the amount of idle time a session will tolerate before it expires automatically.
        /// </summary>
        /// <remarks>
        /// A null value indicates that sessions should never timeout.
        /// </remarks>
        TimeSpan? SessionTimeout { get; set; }

        /// <summary>
        /// Saves the given session, so that it may later be retrieved by its session token.
        /// </summary>
        /// <param name="session">The session to save.</param>
        /// <remarks>
        /// In general, this method should touch the given session's MarkModified() method.
        /// </remarks>
        void SaveSession(TSession session);

        /// <summary>
        /// Gets the session associated with the given session token, or <code>default(TSession)</code>
        /// if one does not exist.
        /// </summary>
        /// <param name="sessionToken">A token identifying the session to retrieve.</param>
        /// <returns>The session associated with the given token.</returns>
        /// <remarks>
        /// The returned session may be closed (with the idea that it could be reopened), and therefore
        /// the IsOpen property should be checked before actually using the session.  However, this
        /// method should never return an open but expired session; if the target session has timed out,
        /// then this method should close the session before returning it.  On the other hand, if the
        /// session is open, then this method should touch the session's MarkAccessed() method.
        /// </remarks>
        TSession GetSession(Guid sessionToken);

        IEnumerable<TSession> GetSessions(Func<TSession, bool> predicate);

        /// <summary>
        /// Closes the session associated with the given session token, or does nothing if one does not
        /// exist.
        /// </summary>
        /// <param name="sessionToken">A token identifying the session to retrieve.</param>
        /// <remarks>
        /// The definition of a "closed" session may vary between different session managers.  That
        /// said, a subsequent call to GetSession should always return either null or the original
        /// session with its IsOpen property false.
        /// </remarks>
        void CloseSession(Guid sessionToken);

        /// <summary>
        /// Requests a clean-up of the stored sessions (i.e. closing and freeing stale sessions).
        /// </summary>
        /// <remarks>
        /// The primary use of this method is to be invoked by a clean-up timer.  However, it may be
        /// invoked from anywhere, with the only concern being the associated performance cost.
        /// </remarks>
        void CleanUpOldSessions();
    }

    public static class SessionManager
    {
        /// <summary>
        /// Creates a process that periodically calls the given session manager's CleanUpOldSessions()
        /// method at the specified interval.
        /// </summary>
        /// <typeparam name="TSession">The type of session used by the given session manager.</typeparam>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="interval">The interval at which to schedule clean-ups.</param>
        /// <returns>The Timer threading object associated with the periodic clean-up process.</returns>
        public static Timer CreateCleanUpTimer<TSession>(this ISessionManager<TSession> sessionManager, TimeSpan interval)
            where TSession : ISession
        {
            if (interval < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("interval", "Clean up interval must be positive.");

            return new Timer(state => sessionManager.CleanUpOldSessions(), null, interval, interval);
        }
    }
}