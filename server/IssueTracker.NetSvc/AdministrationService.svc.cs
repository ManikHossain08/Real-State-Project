using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using InitVent.DataServices.Domain;
using InitVent.DataServices.NHibernate.Repository;
using iMFAS.Services.Logger;
using InitVent.Common.Util;
using IssueTracker.NetSvc;

namespace IssueTracker.NetSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AdministrationService" in code, svc and config file together.
    public class AdministrationService : IAdministrationService
    {
        public LoggerService<AdministrationService> adminServiceLogger = new LoggerService<AdministrationService>();
        public ActionLogger actionLogger = new ActionLogger();

        #region ------------------------------------------------ Role Permission

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<RoleInfo> GetRoles()
        {
            List<RoleInfo> roleInfos = new List<RoleInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            adminServiceLogger.Log(EnumLogLevel.Debug, "Call GetRoles to get role list .");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var roles = IssueTrackerDataServices.Roles.ToList();
                    foreach (var role in roles)
                    {
                        if (role.RoleName != "System Admin") roleInfos.Add(IssueTrackerServiceUtil.ConvertRoleToRoleInfo(role));
                    }
                    adminServiceLogger.Log(EnumLogLevel.Debug, "Complete GetRoles function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Role_Permission, ActionItem.Read, "Roles Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return roleInfos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoleInfo GetRole(string id)
        {
            var roleInfo = new RoleInfo();
            var logMessage = new LogMessage(EnumLogLevel.Info) { { "role id", id } };
            adminServiceLogger.Log(EnumLogLevel.Debug, "Call GetRole to get a role by id: " + id + ".");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var role = IssueTrackerDataServices.Roles.SingleOrDefault(r => r.Id.Equals(Guid.Parse(id)));
                    if (role != null) roleInfo = IssueTrackerServiceUtil.ConvertRoleToRoleInfo(role);
                    roleInfo.Permissions = GetRolePermissions(roleInfo.Id.ToString());
                    adminServiceLogger.Log(EnumLogLevel.Debug, "Complete GetRole function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Role_Permission, ActionItem.Read, "Roles Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return roleInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<PermissionInfo> GetPermissions()
        {
            List<PermissionInfo> permissions = new List<PermissionInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            adminServiceLogger.Log(EnumLogLevel.Debug, "Call GetModules to get Module list .");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    permissions = (from mod in IssueTrackerDataServices.Modules
                                   join menu in IssueTrackerDataServices.Menus on mod.Id equals menu.ModuleId
                                   join subMenu in IssueTrackerDataServices.SubMenus on menu.Id equals subMenu.MenuId
                                   join item in IssueTrackerDataServices.Items on subMenu.Id equals item.SubMenuId
                                   join permission in IssueTrackerDataServices.Permissions on item.Id equals permission.ItemId
                                   orderby mod.SerialNo, menu.SerialNo, subMenu.SerialNo, item.SerialNo, permission.SerialNo
                                   select new PermissionInfo()
                                              {
                                                  Id = permission.Id,
                                                  PermissionName = permission.PermissionName,
                                                  ItemId = item.Id,
                                                  ItemName = item.ItemName,
                                                  SubMenuId = subMenu.Id,
                                                  SubMenuName = subMenu.SubMenuName,
                                                  MenuId = menu.Id,
                                                  MenuName = menu.MenuName,
                                                  ModuleId = mod.Id,
                                                  ModuleName = mod.ModuleName
                                              }

                                  ).ToList();
                    adminServiceLogger.Log(EnumLogLevel.Debug, "Complete GetModules function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Role_Permission, ActionItem.Read, "Permission Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return permissions;
        }

        /// <summary>
        /// Get Action Permissions 
        /// </summary>
        /// <returns></returns>
        public List<PermissionInfo> GetActionPermissions(string userId, string pluginName, string actionName)
        {
            List<PermissionInfo> permissions = new List<PermissionInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            adminServiceLogger.Log(EnumLogLevel.Debug, "Call Get Action Permission list .");

            try
            {
                permissions = (from mod in IssueTrackerDataServices.Modules
                               join menu in IssueTrackerDataServices.Menus on mod.Id equals menu.ModuleId
                               join subMenu in IssueTrackerDataServices.SubMenus on menu.Id equals subMenu.MenuId
                               join item in IssueTrackerDataServices.Items on subMenu.Id equals item.SubMenuId
                               join permission in IssueTrackerDataServices.Permissions on item.Id equals permission.ItemId
                               join rolepermission in IssueTrackerDataServices.RolePermissions on permission.Id equals rolepermission.PermissionId
                               join userinfo in IssueTrackerDataServices.Users on rolepermission.RoleId equals userinfo.UserRole.Id
                               where userinfo.UserId == userId
                               && subMenu.PluginName == pluginName
                               && permission.ActionName == actionName
                               select new PermissionInfo()
                               {
                                   Id = permission.Id,
                                   PermissionName = permission.PermissionName,
                                   ItemId = item.Id,
                                   ItemName = item.ItemName,
                                   SubMenuId = subMenu.Id,
                                   SubMenuName = subMenu.SubMenuName,
                                   MenuId = menu.Id,
                                   MenuName = menu.MenuName,
                                   ModuleId = mod.Id,
                                   ModuleName = mod.ModuleName
                               }

                              ).ToList();
                adminServiceLogger.Log(EnumLogLevel.Debug, "Complete Get Action Permission function.");
            }
            catch (Exception ex)
            {
                throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
            }

            return permissions;
        }

        /// <summary>
        /// Get Permissions for a role
        /// </summary>
        /// <param name="roleId">roleId</param>
        /// <returns>Module list with permission status</returns>
        public List<PermissionInfo> GetRolePermissions(string roleId)
        {
            var permissionInfos = new List<PermissionInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            adminServiceLogger.Log(EnumLogLevel.Debug, "Call GetRolePermission to get Module list .");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var rolePermission = IssueTrackerDataServices.RolePermissions.Where(rp => rp.RoleId == Guid.Parse(roleId)).ToList();
                    permissionInfos.AddRange(rolePermission.Select(IssueTrackerServiceUtil.ConvertRolePermissionToPermissionInfo));
                    adminServiceLogger.Log(EnumLogLevel.Debug, "Complete GetRolePermission.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Role_Permission, ActionItem.Read, "Role Permission Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return permissionInfos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleInfo"></param>
        /// <returns></returns>
        public RoleInfo CreateRole(RoleInfo roleInfo)
        {

            var logMessage = new LogMessage(EnumLogLevel.Info) { { "role name", roleInfo.RoleName } };
            adminServiceLogger.Log(EnumLogLevel.Debug, "Call CreateRole to create a role by Name: " + roleInfo.RoleName + ".");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var roleId = Guid.NewGuid();
                    var role = new Role { Id = roleId, RoleName = roleInfo.RoleName, RoleDescription = roleInfo.Description, Status = roleInfo.Status};
                    role.Save();
                    foreach (var t in roleInfo.Permissions)
                    {
                        var rolePermission = new RolePermission { Id = Guid.NewGuid(), RoleId = roleId };
                        var permissionId = t.PermissionId;
                        if (permissionId != null)
                            rolePermission.PermissionId = permissionId.Value;
                        rolePermission.Save();
                    }
                    adminServiceLogger.Log(EnumLogLevel.Debug, "Complete CreateRole function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Role_Permission, ActionItem.Create, "Role Created successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return roleInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleInfo"></param>
        /// <returns></returns>
        public RoleInfo UpdateRole(string id, RoleInfo roleInfo)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info) { { "role id", roleInfo.Id } };
            adminServiceLogger.Log(EnumLogLevel.Debug, "Call UpdateRole to update a role by id: " + roleInfo.Id + ".");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var role = IssueTrackerDataServices.Roles.SingleOrDefault(r => r.Id == Guid.Parse(id));
                    if (role != null)
                    {
                        role.RoleName = roleInfo.RoleName;
                        role.RoleDescription = roleInfo.Description;
                        role.Status = roleInfo.Status;                       
                        role.Save();
                        var rolePermissions = IssueTrackerDataServices.RolePermissions.Where(rp => rp.RoleId == roleInfo.Id).ToList();
                        foreach (var rolePermission in rolePermissions)
                        {
                            rolePermission.Delete();
                        }

                        foreach (var t in roleInfo.Permissions)
                        {
                            var rolePermission = new RolePermission { Id = Guid.NewGuid(), RoleId = roleInfo.Id };
                            var permissionId = t.PermissionId;
                            if (permissionId != null)
                                rolePermission.PermissionId = permissionId.Value;
                            rolePermission.Save();
                        }
                    }
                    adminServiceLogger.Log(EnumLogLevel.Debug, "Complete UpdateRole function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Role_Permission, ActionItem.Update, "Role Updated successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return roleInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteRole(string id)
        {
            var isDeleted = false;
            var logMessage = new LogMessage(EnumLogLevel.Info) { { "role id", id } };
            adminServiceLogger.Log(EnumLogLevel.Debug, "Call DeleteRole to delete a role by id: " + id + ".");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var role = IssueTrackerDataServices.Roles.SingleOrDefault(r => r.Id == Guid.Parse(id));
                    if (role != null)
                    {
                        var rolePsermissions = IssueTrackerDataServices.RolePermissions.Where(rp => rp.RoleId == role.Id).ToList();
                        for (int i = 0; i < rolePsermissions.Count; i++)
                        {
                            rolePsermissions[i].Delete();
                        }
                        role.Delete();
                    }
                    adminServiceLogger.Log(EnumLogLevel.Debug, "Complete DeleteRole function.");
                    isDeleted = true;
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Role_Permission, ActionItem.Delete, "Role Deleted successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return isDeleted;
        }
     
        #endregion      

    }
}
