using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace IssueTracker.NetSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IIssueTrackerService" in both code and config file together.
    [ServiceContract]
    public interface IIssueTrackerService
    {
        [OperationContract]
        [WebGet(UriTemplate = "TopMenus")]
        List<TopMenu> GetTopMenuList();

        [OperationContract]
        [WebGet(UriTemplate = "Menus?mId={topMenuId}")]
        List<MenuButton> GetMenuButtonList(string topMenuId);

        [OperationContract]
        [WebGet(UriTemplate = "ActionPermission?userId={userId}&pluginName={pluginName}&actionName={actionName}")]
        List<PermissionInfo> GetActionPermissions(string userId, string pluginName, string actionName);

        [OperationContract]
        [WebInvoke(UriTemplate = "Issue", Method = "POST")]
        IssueInfo CreateIssueTracking(IssueInfo objIssueTrackingInfo);

        [OperationContract]
        [WebGet(UriTemplate = "Issues/{projectId}")]
        List<IssueInfo> GetIssues(string projectId);

        [OperationContract]
        [WebGet(UriTemplate = "Issue/{id}")]
        IssueInfo GetIssue(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "Issue/{id}", Method = "DELETE")]
        bool DeleteIssue(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "Issue/{id}", Method = "PUT")]
        IssueInfo UpdateIssue(string id, IssueInfo oIssueTrackerInfo);


        [OperationContract]
        [WebGet(UriTemplate = "Assignee/{projectId}")]
        List<UserInfo> GetUserList(string projectId);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UploadFile?fileName={fileName}")]
        string UploadFile(string fileName, Stream stream);

        [OperationContract]
        [WebInvoke(UriTemplate = "DeleteFile?fileName={fileName}")]
        string DeleteFile(string fileName);

       
        [OperationContract]
        [WebGet(UriTemplate = "File?fileName={fileName}&fileExtension={fileExtension}")]
        Stream DownloadFile(string fileName, string fileExtension);
        

        #region Project
        
        [OperationContract]
        [WebGet(UriTemplate = "Projects")]
        List<ProjectInfo> GetProjects();

        [OperationContract]
        [WebInvoke(UriTemplate = "Project", Method = "POST")]
        ProjectInfo CreateProject(ProjectInfo projectInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "Project/{id}", Method = "PUT")]
        ProjectInfo UpdateProject(string id, ProjectInfo projectInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "Project/{id}", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool DeleteProject(string id);
        #endregion

        #region user

        [OperationContract]
        [WebGet(UriTemplate = "Users")]
        List<UserInfo> GetUsers();

        [OperationContract]
        [WebGet(UriTemplate = "User/{userId}")]
        UserInfo GetUser(string userId);
        
        [OperationContract]
        [WebInvoke(UriTemplate = "User", Method = "POST")]
        UserInfo CreateUser(UserInfo userInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "User/{id}", Method = "PUT")]
        UserInfo UpdateUser(string id, UserInfo userInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "User/{id}", Method = "DELETE")]
        bool DeleteUser(string id);

        [OperationContract]
        [WebGet(UriTemplate = "User/{id}/Password")]
        UserInfo GenerateUserPassword(string id);       

        #endregion

    }
}
