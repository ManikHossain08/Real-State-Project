using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using System.Web;
using iMFAS.Services.Logger;
using IssueTracker.Domain.Authentication;

namespace IssueTracker.NetSvc
{
    public abstract class AuthorizationHelper
    {
        static LoggerService<SecurityService> serviceLogger = new LoggerService<SecurityService>();
        /// <returns>The valid, open session matching the token specified by the "sessionid" query parameter.</returns>
        /// <exception cref="WebFaultException{T}">If the session token was either not provided or not valid.</exception>
        public static UserSession GetSession()
        {
            var sessionId = WebOperationContext.Current.IncomingRequest.UriTemplateMatch != null ? WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["sessionid"] : null;
            if (sessionId == null)
            {
                // Try it again in the header.
                sessionId = WebOperationContext.Current.IncomingRequest.Headers["SessionID"];
            }

            serviceLogger.Log(EnumLogLevel.Info, "fetching session: " + sessionId);

            try
            {
                var userSession = SecurityServices.LookupSession(sessionId);
                userSession.SelectedBranchCode = Convert.ToString(WebOperationContext.Current.IncomingRequest.Headers["BranchCode"]);
                userSession.SelectedBranchId = (WebOperationContext.Current.IncomingRequest.Headers["BranchId"] != null && Convert.ToString(WebOperationContext.Current.IncomingRequest.Headers["BranchId"]) != "") ? Guid.Parse(WebOperationContext.Current.IncomingRequest.Headers["BranchId"]) : Guid.Empty;
                if (WebOperationContext.Current.IncomingRequest.Headers["CurrentDate"] != null && WebOperationContext.Current.IncomingRequest.Headers["CurrentDate"] != "")
                    userSession.CurrentDate = DateTime.Parse(WebOperationContext.Current.IncomingRequest.Headers["CurrentDate"]);
                return userSession;
            }
            catch (Exceptions.InvalidSessionFailure)
            {
                throw GenerateServiceError("Session has expired.", HttpStatusCode.Unauthorized, new LogMessage(EnumLogLevel.Warn, new Exceptions.InvalidSessionFailure()));
            }
            catch (Exception)
            {
                throw GenerateServiceError("Session has expired.", HttpStatusCode.Unauthorized, new LogMessage(EnumLogLevel.Warn, new Exceptions.InvalidSessionFailure()));
            }
        }

        public static WebFaultException<String> GenerateServiceError(String message, HttpStatusCode statusCode, LogMessage logMessage = null, IMFASSerivceErrorCode errorCode = 0)
        {
            if (logMessage == null)
            {
                logMessage = new LogMessage(EnumLogLevel.Warn);
            }

            serviceLogger.Log(EnumLogLevel.Warn, message);

            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.Headers.Add("ErrorCode", string.Format("{0}", (int)errorCode));
                WebOperationContext.Current.OutgoingResponse.Headers.Add("ErrorMsg", Regex.Replace(message, @"[\r\n]+", " "));
            }
            return new WebFaultException<String>(message, statusCode);
        }


    }//class
}