using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace IssueTracker.NetSvc
{    
    #region Security
    /// <summary>
    /// Information about the user's session. Contains an opaque ID that can be used to identify the session along with user 
    /// information, a list of available data warehouses ("repositories") and other information.
    /// </summary>
    [DataContract]
    public class SessionInfo
    {
        /// <summary>
        /// An opaque identifier for this session.
        /// </summary>
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// Information about the logged on user, including first name, last name and other descriptive fields.
        /// </summary>
        [DataMember(Name = "userInfo")]
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// Information about session time
        /// </summary>
        [DataMember(Name = "sessionTime")]
        public double SessionTime { get; set; }

        /// <summary>
        /// Information about User Level (Branch User/ Area User/ Zone User)
        /// </summary>
        [DataMember(Name = "userLevel")]
        public int UserLevel { get; set; }

        /// <summary>
        /// Information about LevelId (Branch Id/ Area Id/ Zone Id)
        /// </summary>
        [DataMember(Name = "levelId")]
        public Guid? LevelId { get; set; }       

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "pluginName")]
        public string PluginName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "orgHierarchy")]
        public string OrgHierarchy { get; set; }
    }

    /// <summary>
    /// Role Info
    /// </summary>
    [DataContract]
    public class RoleInfo
    {
        /** The identity of the Role. */
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /** The name of the role. */
        [DataMember(Name = "roleName")]
        public string RoleName { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "status")]
        public bool Status { get; set; }

        [DataMember(Name = "organizationHierarchyVisibility")]
        public bool OrganizationHierarchyVisibility { get; set; }

        [DataMember(Name = "administrativeRole")]
        public string AdministrativeRole { get; set; }

        [DataMember(Name = "permissions")]
        public List<PermissionInfo> Permissions { get; set; }

    }

    /// <summary>
    /// Module Info
    /// </summary>
    [DataContract]
    public class ModuleInfo
    {
        /** The identity of the module. */
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /** The name of the module. */
        [DataMember(Name = "moduleName")]
        public string ModuleName { get; set; }

        [DataMember(Name = "menus")]
        public List<MenuInfo> Menus { get; set; }

    }

    /// <summary>
    /// Menu Info
    /// </summary>
    [DataContract]
    public class MenuInfo
    {
        /** The identity of the menu. */
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /** The name of the menu. */
        [DataMember(Name = "menuName")]
        public string MenuName { get; set; }

        [DataMember(Name = "subMenus")]
        public List<SubMenuInfo> SubMenus { get; set; }

    }

    /// <summary>
    /// Sub Menu Info
    /// </summary>
    [DataContract]
    public class SubMenuInfo
    {
        /** The identity of the sub menu. */
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /** The name of the sub menu. */
        [DataMember(Name = "subMenuName")]
        public string SubMenuName { get; set; }

        [DataMember(Name = "items")]
        public List<ItemInfo> Items { get; set; }

    }

    /// <summary>
    /// Item Info
    /// </summary>
    [DataContract]
    public class ItemInfo
    {
        /** The identity of the Item. */
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /** The name of the Item. */
        [DataMember(Name = "itemName")]
        public string ItemName { get; set; }

        [DataMember(Name = "permissions")]
        public List<PermissionInfo> Permissions { get; set; }

    }

    /// <summary>
    /// Permission Info
    /// </summary>
    [DataContract]
    public class PermissionInfo
    {
        /** The identity of the Permission. */
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "roleId")]
        public Guid? RoleId { get; set; }

        /** The identity of the module. */
        [DataMember(Name = "moduleId")]
        public Guid? ModuleId { get; set; }

        /** The name of the module. */
        [DataMember(Name = "moduleName")]
        public string ModuleName { get; set; }

        /** The identity of the menu. */
        [DataMember(Name = "menuId")]
        public Guid? MenuId { get; set; }

        /** The name of the menu. */
        [DataMember(Name = "menuName")]
        public string MenuName { get; set; }

        /** The identity of the sub menu. */
        [DataMember(Name = "subMenuId")]
        public Guid? SubMenuId { get; set; }

        /** The name of the sub menu. */
        [DataMember(Name = "subMenuName")]
        public string SubMenuName { get; set; }

        /** The identity of the Item. */
        [DataMember(Name = "itemId")]
        public Guid? ItemId { get; set; }

        /** The name of the Item. */
        [DataMember(Name = "itemName")]
        public string ItemName { get; set; }

        [DataMember(Name = "permissionId")]
        public Guid? PermissionId { get; set; }

        /** The name of the Permission. */
        [DataMember(Name = "permissionName")]
        public string PermissionName { get; set; }

        /** The Permission (true/false). */
        [DataMember(Name = "permission")]
        public bool Permission { get; set; }
    }


    /// <summary>
    /// Object to present User
    /// </summary>
    [DataContract]
    public class UserInfo
    {
        /** The identity of the user. */
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /** The identity of the user. */
        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        /** The identity of the user. */
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        /** The employee related to the user. */
        [DataMember(Name = "employeeId")]
        public Guid? EmployeeId { get; set; }

        /** The password of the user. */
        [DataMember(Name = "userPassword")]
        public string UserPassword { get; set; }

        /** The status of the user. */
        [DataMember(Name = "userStatus")]
        public bool UserStatus { get; set; }

        /** The Organization LevelId of the user. */
        [DataMember(Name = "userLevel")]
        public int UserLevel { get; set; }

        /** The Organization LevelId of the user. */
        [DataMember(Name = "userLevelId")]
        public Guid? UserLevelId { get; set; }

        /** The roles of the user. */
        [DataMember(Name = "userRole")]
        public RoleInfo UserRole { get; set; }

        [DataMember(Name = "mobile")]
        public string Mobile { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "project_Id")]
        public Guid? Project_Id { get; set; }

        [DataMember(Name = "projects")]
        public List<UserProjectInfo> UserProjects { get; set; }

    }

    [DataContract]
    public class UserProjectInfo
    {
        /** The identity of the user. */
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /** The identity of the user. */
        [DataMember(Name = "userId")]
        public Guid UserId { get; set; }

        [DataMember(Name = "projectId")]
        public Guid ProjectId { get; set; }

        [DataMember(Name = "projectName")]
        public string ProjectName { get; set; }
    }

    /// <summary>
    /// The json object the client sends to the server.
    /// </summary>
    [DataContract]
    public class ChangePassword
    {
        /// <summary>
        /// The current password for this user.
        /// </summary>
        [DataMember(Name = "currentPassword")]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// The updated new password for this user.
        /// </summary>
        [DataMember(Name = "newPassword")]
        public string NewPassword { get; set; }
    }

    #endregion    

    [DataContract]
    public class TopMenu
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "action")]
        public string Action { get; set; }
    }

    [DataContract]
    public class OptionItem
    {
        /// <summary>
        /// Id 
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Value / Text what will be shown 
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

    }

    [DataContract]
    public class ServerSideDataSearchKeys
    {
        /// <summary>
        /// Display startIndex in dataTable
        /// </summary>
        [DataMember(Name = "iDisplayStart")]
        public int DisplayStart { get; set; }

        /// <summary>
        /// Display records in dataTable
        /// </summary>
        [DataMember(Name = "iDisplayLength")]
        public int DisplayLength { get; set; }

        /// <summary>
        /// Sort columns in table
        /// </summary>
        [DataMember(Name = "sColumns")]
        public string SortColumns { get; set; }

        /// <summary>
        /// SearchText in dataTable
        /// </summary>
        [DataMember(Name = "sSearch")]
        public string SearchText { get; set; }

        /// <summary>
        /// Sorting Columns and direction in dataTable
        /// </summary>
        [DataMember(Name = "aaSorting")]
        public string[][] AaSorting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "entryDate")]
        public string EntryDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "userId")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "sessionId")]
        public Guid? SessionId { get; set; }
    }

    [DataContract]
    public class ApplicationAccessLogResult
    {
        [DataMember(Name = "searchLogs")]
        public List<ApplicationAccessLogInfo> SearchLogs { get; set; }

        [DataMember(Name = "totalRecords")]
        public int TotalRecords { get; set; }

        [DataMember(Name = "displayRecords")]
        public int DisplayRecords { get; set; }
    }
    /// <summary>
    /// Object to present ApplicationAccessLog
    /// </summary>
    [DataContract]
    public class ApplicationAccessLogInfo
    {

        [DataMember(Name = "id")]
        public Guid Id { get; set; }


        [DataMember(Name = "userId")]
        public string UserId { get; set; }


        [DataMember(Name = "userName")]
        public string UserName { get; set; }


        [DataMember(Name = "logInTime")]
        public DateTime? LogInTime { get; set; }


        [DataMember(Name = "logOutTime")]
        public DateTime? LogOutTime { get; set; }

        [DataMember(Name = "sessionId")]
        public Guid SessionId { get; set; }

    }

    [DataContract]
    public class ApplicationAuditLogResult
    {
        [DataMember(Name = "searchLogs")]
        public List<ApplicationAuditLogInfo> SearchLogs { get; set; }

        [DataMember(Name = "totalRecords")]
        public int TotalRecords { get; set; }

        [DataMember(Name = "displayRecords")]
        public int DisplayRecords { get; set; }
    }

    /// <summary>
    /// Object to present ApplicationAuditLog
    /// </summary>
    [DataContract]
    public class ApplicationAuditLogInfo
    {

        [DataMember(Name = "id")]
        public Guid Id { get; set; }


        [DataMember(Name = "userId")]
        public string UserId { get; set; }


        [DataMember(Name = "userName")]
        public string UserName { get; set; }


        [DataMember(Name = "itemName")]
        public string ItemName { get; set; }


        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "reference")]
        public string Reference { get; set; }

    }   
    

    [DataContract]
    public class MenuButton
    {
        [DataMember(Name = "btnID")]
        public string BtnID { get; set; }

        [DataMember(Name = "btnName")]
        public string BtnName { get; set; }

        [DataMember(Name = "controllerInfo")]
        public ControllerInfo ControllerInfo { get; set; }

        [DataMember(Name = "children")]
        public MenuButton[] Children { get; set; }

    }

    [DataContract]
    public class ControllerInfo
    {
        [DataMember(Name = "controllerHelperName")]
        public string ControllerHelperName { get; set; }

    }

    [DataContract]
    public class IssueInfo
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "priority")]
        public string Priority { get; set; }

        [DataMember(Name = "isssueTitle")]
        public string IsssueTitle { get; set; }

        [DataMember(Name = "severity")]
        public string Severity { get; set; }

        [DataMember(Name = "issueDescription")]
        public string IssueDescription { get; set; }

        [DataMember(Name = "resolutionArea")]
        public string ResolutionArea { get; set; }

        [DataMember(Name = "issueType")]
        public string IssueType { get; set; }

        [DataMember(Name = "developer")]
        public Guid? Assignee { get; set; }     

        [DataMember(Name = "issueNo")]
        public string IssueNo { get; set; }

        [DataMember(Name = "issueStatus")]
        public string IssueStatus { get; set; }

        [DataMember(Name = "project")]
        public Guid? Project { get; set; }

        [DataMember(Name = "creator")]
        public Guid? Creator { get; set; }

        [DataMember(Name = "creationDate")]
        public DateTime? CreationDate { get; set; }

        [DataMember(Name = "modifier")]
        public Guid? Modifier { get; set; }

        [DataMember(Name = "modificationDate")]
        public DateTime? ModificationDate { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        [DataMember(Name = "projectName")]
        public string ProjectName { get; set; }

        [DataMember(Name = "issuePhoto")]
        public string IssuePhoto { get; set; }

        [DataMember(Name = "attachmentFiles")]
        public List<IssueAttachmentInfo> AttachmentFiles { get; set; }

        [DataMember(Name = "creatorName")]
        public string CreatorName { get; set; }

        [DataMember(Name = "modifiedUser")]
        public string ModifiedUser { get; set; }

    }
    
    
    [DataContract]
    public class ProjectInfo
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "projectCode")]
        public string ProjectCode { get; set; }

        [DataMember(Name = "projectName")]
        public string ProjectName { get; set; }

        [DataMember(Name = "startDate")]
        public DateTime? StartDate { get; set; }
    }


    [DataContract]
    public class IssueAttachmentInfo
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "issueId")]
        public Guid IssueId { get; set; }

        [DataMember(Name = "attachmentDate")]
        public DateTime? AttachmentDate { get; set; }

        //[DataMember(Name = "attachmentFile")]
        //public string AttachmentFile { get; set; }
       
        [DataMember(Name = "attachmentFileLocation")]
        public string AttachmentFileLocation { get; set; }
        
    }

  


    [DataContract(Name = "SmtpConfig", Namespace = "")]
    public class SmtpConfig
    {
        [DataMember(Name = "HostName")]
        public string HostName { get; set; }

        [DataMember(Name = "PortNumber")]
        public Int32 PortNumber { get; set; }

        [DataMember(Name = "UserId")]
        public string UserId { get; set; }

        [DataMember(Name = "Password")]
        public string Password { get; set; }
    }


}

