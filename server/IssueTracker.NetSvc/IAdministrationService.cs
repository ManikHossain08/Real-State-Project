using IssueTracker.NetSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace IssueTracker.NetSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAdministrationService" in both code and config file together.
    [ServiceContract]
    public interface IAdministrationService
    {
        #region Role Permission

        [OperationContract]
        [WebGet(UriTemplate = "Roles")]
        List<RoleInfo> GetRoles();

        [OperationContract]
        [WebGet(UriTemplate = "Role/{id}")]
        RoleInfo GetRole(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "Role", Method = "POST")]
        RoleInfo CreateRole(RoleInfo roleInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "Role/{id}", Method = "PUT")]
        RoleInfo UpdateRole(string id, RoleInfo roleInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "Role/{id}", Method = "DELETE")]
        bool DeleteRole(string id);

        [OperationContract]
        [WebGet(UriTemplate = "Permissions")]
        List<PermissionInfo> GetPermissions();

        [OperationContract]
        [WebGet(UriTemplate = "ActionPermission?userId={userId}&pluginName={pluginName}&actionName={actionName}")]
        List<PermissionInfo> GetActionPermissions(string userId, string pluginName, string actionName);

        [OperationContract]
        [WebGet(UriTemplate = "Permissions?roleid={roleId}")]
        List<PermissionInfo> GetRolePermissions(string roleId);       


        #endregion
      
    }
}
