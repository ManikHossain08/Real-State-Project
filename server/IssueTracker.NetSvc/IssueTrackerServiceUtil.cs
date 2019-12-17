using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using InitVent.DataServices.Domain;
using InitVent.DataServices.NHibernate.Repository;
using InitVent.ORM;
using Iesi.Collections.Generic;


namespace IssueTracker.NetSvc
{
    public class IssueTrackerServiceUtil
    {

        #region ----------------------------------------------------- RolePermission

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static RoleInfo ConvertRoleToRoleInfo(Role role)
        {
            if (role == null) return new RoleInfo();
            return new RoleInfo() { Id = role.Id, RoleName = role.RoleName, Description = role.RoleDescription, Status = role.Status };
        }

        public static ModuleInfo ConvertToModuleInfo(Module module)
        {
            var moduleInfo = new ModuleInfo { Id = module.Id, ModuleName = module.ModuleName };
            var menus = module.Menus.OrderBy(m => m.SerialNo).ToList();
            var menuInfos = menus.Select(ConvertToMenuInfo).ToList();
            moduleInfo.Menus = menuInfos;
            return moduleInfo;
        }

        public static MenuInfo ConvertToMenuInfo(Menu menu)
        {
            var menuInfo = new MenuInfo { Id = menu.Id, MenuName = menu.MenuName };
            var subMenus = menu.SubMenus.OrderBy(sm => sm.SerialNo).ToList();
            var subMenuInfos = subMenus.Select(ConvertToSubMenuInfo).ToList();
            menuInfo.SubMenus = subMenuInfos;
            return menuInfo;
        }

        public static SubMenuInfo ConvertToSubMenuInfo(SubMenu subMenu)
        {
            var subMenuInfo = new SubMenuInfo { Id = subMenu.Id, SubMenuName = subMenu.SubMenuName };
            var items = subMenu.Items.OrderBy(smi => smi.SerialNo).ToList();
            var itemInfos = items.Select(ConvertToItemInfo).ToList();
            subMenuInfo.Items = itemInfos;
            return subMenuInfo;
        }

        public static ItemInfo ConvertToItemInfo(Item item)
        {
            var itemInfo = new ItemInfo { Id = item.Id, ItemName = item.ItemName };
            var permissions = item.Permissions.OrderBy(rp => rp.SerialNo).ToList();
            var permissionInfos = permissions.Select(ConvertToPermissionInfo).ToList();
            itemInfo.Permissions = permissionInfos;
            return itemInfo;
        }

        public static PermissionInfo ConvertToPermissionInfo(Permission permission)
        {
            var permissionInfo = new PermissionInfo { Id = permission.Id, PermissionName = permission.PermissionName };
            return permissionInfo;
        }

        public static PermissionInfo ConvertRolePermissionToPermissionInfo(RolePermission rolePermission)
        {
            var permissionInfo = new PermissionInfo { RoleId = rolePermission.RoleId, PermissionId = rolePermission.PermissionId, Permission = true };
            return permissionInfo;
        }

        #endregion

        public static IssueTracking ConvertIssueTrackingInfoToIssueTracking(IssueTracking oIssueTracking, IssueInfo oIIssueTrackingInfo)
        {
            if (oIssueTracking == null) oIssueTracking = new IssueTracking()
            {
                Id = Guid.NewGuid(),
                Creator = oIIssueTrackingInfo.Creator,
                CreationDate = oIIssueTrackingInfo.CreationDate
            };
            oIssueTracking.Priority = oIIssueTrackingInfo.Priority;
            oIssueTracking.IsssueTitle = oIIssueTrackingInfo.IsssueTitle;
            oIssueTracking.Severity = oIIssueTrackingInfo.Severity;
            oIssueTracking.IssueDescription = oIIssueTrackingInfo.IssueDescription;
            oIssueTracking.IssueType = oIIssueTrackingInfo.IssueType;
            oIssueTracking.Assignee = oIIssueTrackingInfo.Assignee;
            oIssueTracking.IssueNo = oIIssueTrackingInfo.IssueNo;
            oIssueTracking.IssueStatus = oIIssueTrackingInfo.IssueStatus;
            oIssueTracking.ResolutionArea = oIIssueTrackingInfo.ResolutionArea;            
            oIssueTracking.Project = oIIssueTrackingInfo.Project;
            oIssueTracking.IssuePhoto = !string.IsNullOrEmpty(oIIssueTrackingInfo.IssuePhoto) ? System.Convert.FromBase64String(oIIssueTrackingInfo.IssuePhoto) : null;
           
            if (oIIssueTrackingInfo.Modifier != null)
            {
                oIssueTracking.Modifier = oIIssueTrackingInfo.Modifier;
                oIssueTracking.ModificationDate = oIIssueTrackingInfo.ModificationDate;
            }



            return oIssueTracking;
        }

        public static IssueInfo ConvertIssueTrackingToIssueTrackingInfo(IssueTracking oIssueTracking)
        {
            var oIssueTrackingInfo = new IssueInfo();
            oIssueTrackingInfo.Id = oIssueTracking.Id;
            oIssueTrackingInfo.Priority = oIssueTracking.Priority;
            oIssueTrackingInfo.IsssueTitle = oIssueTracking.IsssueTitle;
            oIssueTrackingInfo.IssueDescription = oIssueTracking.IssueDescription;
            oIssueTrackingInfo.IssueNo = oIssueTracking.IssueNo;
            oIssueTrackingInfo.IssueStatus = oIssueTracking.IssueStatus;
            oIssueTrackingInfo.IssueType = oIssueTracking.IssueType;
            oIssueTrackingInfo.ModificationDate = oIssueTracking.ModificationDate;
            oIssueTrackingInfo.Modifier = oIssueTracking.Modifier;
            oIssueTrackingInfo.Project = oIssueTracking.Project;
            oIssueTrackingInfo.ResolutionArea = oIssueTracking.ResolutionArea;
            oIssueTrackingInfo.Severity = oIssueTracking.Severity;
            oIssueTrackingInfo.Assignee = oIssueTracking.Assignee;
            oIssueTrackingInfo.CreationDate = oIssueTracking.CreationDate;
            oIssueTrackingInfo.Creator = oIssueTracking.Creator;

            oIssueTrackingInfo.IssuePhoto = (oIssueTracking.IssuePhoto != null) ? System.Convert.ToBase64String(oIssueTracking.IssuePhoto, 0, oIssueTracking.IssuePhoto.Length) : null;



            return oIssueTrackingInfo;
        }

        /// <summary>
        /// Convet ProjectInfo to Project
        /// </summary>
        /// <param name="project"> </param>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        public static Project ConvertProjectInfoToProject(Project project, ProjectInfo projectInfo)
        {
            project = project ?? new Project();
            project.Id = projectInfo.Id != Guid.Empty ? projectInfo.Id : Guid.NewGuid();
            project.ProjectName = projectInfo.ProjectName;
            project.ProjectCode = projectInfo.ProjectCode;
            project.StartDate = projectInfo.StartDate;
            return project;
        }

        /// <summary>
        /// Convert project to projectInfo
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static ProjectInfo ConvertProjectToProjectInfo(Project project)
        {
            return new ProjectInfo()
            {
                Id = project.Id,
                ProjectCode = project.ProjectCode,
                ProjectName = project.ProjectName,
                StartDate = project.StartDate
            };
        }

        /// <summary>
        /// Convert json string to CustomerSearchKeys object
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static ServerSideDataSearchKeys GetParametersFromJsonString(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(jsonString.ToCharArray())))
            {
                var serilaizer = new DataContractJsonSerializer(typeof(ServerSideDataSearchKeys));
                return (ServerSideDataSearchKeys)serilaizer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Convert domain User to UserInfo
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserInfo ConvertUserToUserInfo(User user)
        {
            if (user == null) return new UserInfo();
            return new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName,
                UserId = user.UserId,
                UserStatus = user.Status,
                Address=user.Address,
                Mobile = user.Mobile,
                Email = user.Email,                
                UserRole = ConvertRoleToRoleInfo(user.UserRole)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static User ConvertUserInfoToUser(User user, UserInfo userInfo)
        {
            user = user ?? new User() { Id = Guid.NewGuid() };
            user.UserName = userInfo.UserName;
            user.UserId = userInfo.UserId;
            user.Address = userInfo.Address;
            user.Mobile = userInfo.Mobile;
            user.Email = userInfo.Email;            
            user.Status = userInfo.UserStatus;
            user.UserRole = new Role() { Id = userInfo.UserRole.Id };
            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static IssueAttachment ConvertIssueAttachmentInfoToIssueAttachment(IssueAttachment oIssueAttachment, IssueAttachmentInfo oIssueAttachmentInfo)
        {
            if (oIssueAttachment == null) oIssueAttachment = new IssueAttachment()
            {
                Id = Guid.NewGuid()
            };           
           
            oIssueAttachment.IssueId = oIssueAttachmentInfo.IssueId;
            oIssueAttachment.AttachmentDate = oIssueAttachmentInfo.AttachmentDate;
            oIssueAttachment.FileLocation = oIssueAttachmentInfo.AttachmentFileLocation;            
            return oIssueAttachment;
        }

        public static IssueAttachmentInfo ConvertIssueAttachmentToIssueAttachmentInfo(IssueAttachment oIssueAttachment)
        {
            return new IssueAttachmentInfo()
            {
                Id = oIssueAttachment.Id,
                IssueId = oIssueAttachment.IssueId,
                AttachmentDate = oIssueAttachment.AttachmentDate,
                AttachmentFileLocation = oIssueAttachment.FileLocation
            };
        }


    

    }

    
    public enum UserLevel
    {
        OrganizationLevel = 0,
        ZoneLevel = 1,
        SubZoneLevel = 2,
        AreaLevel = 3,
        BranchLevel = 4
    }
}