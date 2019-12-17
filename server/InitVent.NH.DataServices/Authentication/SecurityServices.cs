
using System.ServiceModel;
using System.ServiceModel.Channels;
using InitVent.Common.Security;
using InitVent.DataServices.Domain;
using InitVent.DataServices.NHibernate.Repository;

namespace IssueTracker.Domain.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Web;
    using InitVent.Metadata.Session;
    using InitVent.Metadata.Session.Impl;


    public static class SecurityServices
    {
        private static readonly TimeSpan? DefaultSessionTimeout = null;

        private static readonly ISessionManager<UserSession> UserSessionManager;
        private static readonly Timer CleanUpTimer;

        static SecurityServices()
        {
            var timeoutSetting = ConfigurationManager.AppSettings["SessionTimeoutMinutes"];
            var timeout = timeoutSetting == null ? DefaultSessionTimeout : TimeSpan.FromMinutes(Convert.ToDouble(timeoutSetting));

            // TODO construct via properties in the *.config file
            UserSessionManager = new InMemorySessionManager<UserSession>(timeout);
            if (timeout.HasValue)
            {
                CleanUpTimer = UserSessionManager.CreateCleanUpTimer(timeout.Value);
            }
        }

        #region Password Helper Members

        /// <summary>
        /// A wrapper method for salted password hashing with PBKDF2-SHA1.
        /// </summary>
        /// <param name="password">User password</param>
        /// <returns>Encrypted password string</returns>
        public static string CreatePasswordHash(string password)
        {
            return PasswordHash.Create(password).ToString();
        }

        /// <summary>
        /// During this transition period, we will also support plain text password.
        /// </summary>
        /// <param name="user">User object</param>
        /// <param name="password">Password text passed in</param>
        /// <returns></returns>
        public static bool CheckPassword(User user, string password)
        {
            try
            {
                return PasswordHash.Parse(user.UserPassword).CheckPassword(password);
            }
            catch (FormatException)
            {
                // HACK: Allow plain text during transition period.
                return password == user.UserPassword;
            }
        }

        /// <summary>
        /// Update user password in the encrypted format.
        /// </summary>
        /// <param name="user">User object</param>
        /// <param name="newPassword">New password passed back</param>
        /// <returns></returns>
        public static bool ChangePassword(User user, string newPassword)
        {
            try
            {
                user.UserPassword = CreatePasswordHash(newPassword);
                user.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// This is the main interface method that would be used by other application.
        /// It creates a UserSession object by validating the login and password combination.
        /// </summary>
        /// <param name="login">Unique User ID</param>
        /// <param name="password">User password</param>
        /// <returns>UserSession object</returns>
        /// <exception cref="Exceptions.SessionCreationFailure"></exception>
        public static UserSession CreateSession(string login, string password)
        {
            var users = (from user in IssueTrackerDataServices.Users
                           where user.UserId == login   //WITCHCRAFT!
                           
                           select new { user }).ToList();

            // Do password comparison in memory to ensure exact match (not normally captured by SQL Server)
            var results = users.Where(x => CheckPassword(x.user, password)).ToList();

            if (results.Count != 1)
            {
                if (users.Count == 1)
                {
                    // Save login failure count
                    //SaveUserLoginRecord(users[0].user, false);
                }

                throw new Exceptions.SessionCreationFailure();
            }
            // Save login success count
            // SaveUserLoginRecord(results[0].user, true); // TODO Right now it seems we may have a primary key conflict issue with MySQL database so remove this piece of functionality for now.

            var session = results.Select(x => new UserSession(x.user)).Single();
            UserSessionManager.SaveSession(session);

            return session;
        }       

        /// <summary>
        /// This method is only valid when this service is integrated in WCF services.
        /// It will retrieve the client IP address.
        /// </summary>
        /// <returns></returns>
        public static string GetIpFromContext()
        {
            var context = OperationContext.Current;
            if (context != null)
            {
                var prop = context.IncomingMessageProperties;
                var endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                return endpoint.Address;
            }
            else
            {
                return null;
            }
        }

        /// <exception cref="Exceptions.SessionCreationFailure"></exception>
        public static UserSession CreateSession(string login, string password, Func<ICollection<IAccessible>> GetAccessibles)
        {
            var session = CreateSession(login, password);
            session.GetAccessibles = GetAccessibles;
            return session;
        }

        /// <summary>
        /// Retrieves the session associated with the specified session token, or
        /// null if one could not be found.
        /// </summary>
        /// <param name="sessionToken">A token identifying the session to retrieve.</param>
        /// <returns>The session matching the given token, or null.</returns>
        private static UserSession GetSession(string sessionToken)
        {
            Guid token;
            var goodToken = Guid.TryParse(sessionToken, out token);

            if (!goodToken)
            {
                return null;
            }

            return UserSessionManager.GetSession(token);
        }

        /// <summary>
        /// Retrieves the active session associated with the specified session token.
        /// </summary>
        /// <param name="sessionToken">A token identifying the session to retrieve.</param>
        /// <returns>The valid, open session matching the given token.</returns>
        /// <exception cref="Exceptions.InvalidSessionFailure">If there is no active session associated with the specified token.</exception>
        public static UserSession LookupSession(string sessionToken)
        {
            var session = GetSession(sessionToken);
            if (session == null || !session.IsOpen)
                throw new Exceptions.InvalidSessionFailure();

            return session;
        }

        
        public static void CloseAllForUser(User target)
        {
            IEnumerable<UserSession> todestroy = UserSessionManager.GetSessions(aa => aa.User.UserId == target.UserId);
            foreach (InitVent.Metadata.Session.ISession ses in todestroy)
            {
                CloseSession(ses.SessionToken.ToString());
            }
        }

        /// <summary>
        /// Closes the session associated with the specified session token.
        /// </summary>
        /// <param name="sessionToken">A token identifying the session to close.</param>
        /// <remarks>
        /// Note: This method always returns cleanly, whether or not the given session was in fact closed.
        /// </remarks>
        public static void CloseSession(string sessionToken)
        {
            Guid token;
            var goodToken = Guid.TryParse(sessionToken, out token);
            if (!goodToken)
                return;

            UserSessionManager.CloseSession(token);
        }
    }
}