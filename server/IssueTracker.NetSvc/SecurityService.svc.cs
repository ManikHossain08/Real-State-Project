using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using InitVent.DataServices.NHibernate.Repository;
using iMFAS.Services.Logger;
using InitVent.DataServices.Domain;
using InitVent.Common.Util;
using System.Configuration;
using IssueTracker.Domain.Authentication;
using IssueTracker.Domain.Authentication.Exceptions;

namespace IssueTracker.NetSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SecurityService" in code, svc and config file together.
    public class SecurityService : ISecurityService
    {
        public LoggerService<ISecurityService> securityLogger = new LoggerService<ISecurityService>();
        public ActionLogger actionLogger = new ActionLogger();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public SessionInfo Login(string userName, string pass)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info) { { "username", userName } };
            securityLogger.Log(logMessage);
            userName = System.Uri.UnescapeDataString(userName);
            pass = System.Uri.UnescapeDataString(pass);
            //checking empty username and password
            if (string.IsNullOrWhiteSpace(userName))
            {
                securityLogger.Log(EnumLogLevel.Debug, string.Format(ApplicationErrorMessages.EmptyCredentials, userName, pass));
                throw AuthorizationHelper.GenerateServiceError(ApplicationErrorMessages.EmptyCredentials, HttpStatusCode.Unauthorized, logMessage.Clone(EnumLogLevel.Warn));
            }

            try
            {
                var userSession = SecurityServices.CreateSession(userName, pass);

                var sessionDto = CreateSessionInfo(userSession);
                //var insertQuery = "Insert into application_access_log (id, user_id, session_id, log_in, userId, userName) VALUES ('" + Guid.NewGuid() + "','" + userSession.User.Id + "','" + sessionDto.SessionId + "','" + DateTime.Now + "','" + userSession.User.UserId + "','" + userSession.User.UserName + "')";
                //userSession.User.ExecuteNonQuery(insertQuery);
                securityLogger.Log(EnumLogLevel.Info, "Login successful for user:" + userSession.User.UserId);

                return sessionDto;
            }
            catch (SessionCreationFailure)
            {
                throw AuthorizationHelper.GenerateServiceError(ApplicationErrorMessages.InvalidCredentials, HttpStatusCode.Unauthorized, logMessage.Clone(EnumLogLevel.Warn));
            }
        }


        /// <summary>
        /// Creating required info of user which are required just after login
        /// </summary>
        /// <param name="userSession">user's session information</param>
        /// <returns></returns>
        public SessionInfo CreateSessionInfo(UserSession userSession)
        {

            //Creating Session Info object for logged in user
            var sessionInfo = new SessionInfo();
            sessionInfo.SessionId = userSession.SessionToken.ToString();
            sessionInfo.UserInfo = new UserInfo()
            {
                Id = userSession.User.Id,
                UserId = userSession.User.UserId,
                UserName = userSession.User.UserName
            };
            sessionInfo.UserInfo.UserRole = IssueTrackerServiceUtil.ConvertRoleToRoleInfo(userSession.User.UserRole);
            sessionInfo.UserInfo.UserProjects = (from up in IssueTrackerDataServices.UserProjects
                                                 join p in IssueTrackerDataServices.Projects
                                                 on up.ProjectId equals p.Id
                                                 where up.UserId == userSession.User.Id
                                                 select new UserProjectInfo()
                                                 {
                                                     Id = up.Id,
                                                     UserId = up.UserId,
                                                     ProjectId = up.ProjectId,
                                                     ProjectName = p.ProjectName
                                                 }).ToList();
            if (sessionInfo.UserInfo.UserProjects.Count == 1) sessionInfo.UserInfo.Project_Id = sessionInfo.UserInfo.UserProjects[0].ProjectId;
            actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, sessionInfo.SessionId, ActionLogItem.Branch, ActionItem.Read, "Data read successfully");
            sessionInfo.PluginName = ConfigurationManager.AppSettings["default_Plugin"].ToString();
            return sessionInfo;
        }

        /// <summary>
        /// Log out from application
        /// </summary>
        public bool Logout()
        {
            // Validate current session.
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                SecurityServices.CloseSession(userSession.SessionToken.ToString());
            }
            return true;
        }

        /// <summary>
        /// Changing user's password
        /// </summary>
        /// <param name="newPasswordInfo"></param>
        public void ChangePassword(ChangePassword newPasswordInfo)
        {
            // Validate current session.
            var session = AuthorizationHelper.GetSession();

            if (newPasswordInfo.CurrentPassword == newPasswordInfo.NewPassword)
            {
                throw AuthorizationHelper.GenerateServiceError(ApplicationErrorMessages.ChooseAnotherPassword, HttpStatusCode.BadRequest, null, IMFASSerivceErrorCode.PreviouslyUsedPassword);
            }

            //checking for password format
            string regExpPattern = System.Configuration.ConfigurationManager.AppSettings["PasswordRegExp"];
            if (!string.IsNullOrWhiteSpace(regExpPattern) && !Regex.Match(newPasswordInfo.NewPassword, regExpPattern).Success)
            {
                throw AuthorizationHelper.GenerateServiceError(ApplicationErrorMessages.InvalidPasswordFormat, HttpStatusCode.BadRequest, null, IMFASSerivceErrorCode.PasswordFormatIsInvalid);
            }


            var user = IssueTrackerDataServices.Users.First(u => u.UserId == session.User.UserId);

            if (!SecurityServices.CheckPassword(user, newPasswordInfo.CurrentPassword))
            {
                throw AuthorizationHelper.GenerateServiceError(ApplicationErrorMessages.WrongPassword, HttpStatusCode.BadRequest, null, IMFASSerivceErrorCode.WrongPassword);
            }

            if (SecurityServices.ChangePassword(user, newPasswordInfo.NewPassword))
            {
                return;
            }
            else
            {
                // Unknown exception
                throw AuthorizationHelper.GenerateServiceError(ApplicationErrorMessages.UnknownException, HttpStatusCode.BadRequest, null, IMFASSerivceErrorCode.UnknowError);
            }
        }

    }
    #region Error Code
    public enum IMFASSerivceErrorCode
    {
        #region ChangePassword
        WrongPassword = 1001,
        PreviouslyUsedPassword = 1002,
        PasswordCannotBeEmpty = 1003,
        PasswordFormatIsInvalid = 1004,
        #endregion

        EmailSentFailed = 2000,

        UnknowError = 9999
    }
    #endregion
}
