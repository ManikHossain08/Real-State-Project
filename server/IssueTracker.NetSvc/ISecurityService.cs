using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace IssueTracker.NetSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISecurityService" in both code and config file together.
    [ServiceContract]
    public interface ISecurityService
    {
        /// <summary>
        /// Perform authentication of the user's credentials and create a session.
        /// </summary>
        /// <param name="userName">The login name for the user.</param>
        /// <param name="pass">The clear text password for the user.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "Login?userName={userName}&pass={pass}")]
        SessionInfo Login(string userName, string pass);


        /// <summary>
        /// Perform logout which remove cokkies and cache data for the current user. 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "Logout")]
        bool Logout();

        /// <summary>
        /// Update user password in the encrypted format..
        /// </summary>
        /// <param name="newPasswordInfo">The clear text password for the user.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ChangePassword", Method = "POST")]
        void ChangePassword(ChangePassword newPasswordInfo);
    }
    [DataContract]
    public class UserLogIn
    {
        /// <summary>
        /// The log in id of the user.
        /// </summary>
        [DataMember(Name = "userID")]
        public string UserID { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

    [DataContract]
    public class UserLogIns
    {
        /// <summary>
        /// The log in id of the user.
        /// </summary>
        [DataMember(Name = "userLogIns")]
        public UserLogIn[] LogIns { get; set; }
    }

}
