using iMFAS.Services.Logger;
using InitVent.Common.Security;
using InitVent.Common.Util;
using InitVent.DataServices.Domain;
using InitVent.DataServices.NHibernate.Repository;
using IssueTracker.Domain.Authentication;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

namespace IssueTracker.NetSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "IssueTrackerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select IssueTrackerService.svc or IssueTrackerService.svc.cs at the Solution Explorer and start debugging.
    public class IssueTrackerService : IIssueTrackerService
    {

        private static LoggerService<IssueTrackerService> issueTrackerLogger = new LoggerService<IssueTrackerService>();
        public ActionLogger actionLogger = new ActionLogger();
        private static readonly Object lockObj = new Object();

        /// <summary>
        /// Get Module List
        /// </summary>
        /// <returns>List of modules</returns>
        public List<TopMenu> GetTopMenuList()
        {
            List<TopMenu> modules = new List<TopMenu>();
            var userSession = AuthorizationHelper.GetSession();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    string sql = @"select distinct mo.id, mo.name as ModuleName, mo.slNo
                    from role r
                    INNER JOIN RolePermission rp on rp.role_id=r.id
                    INNER JOIN Permission p on p.id=rp.permission_id
                    INNER JOIN Item i on i.id=p.item_id
                    INNEr Join SubMenu sm on sm.id=i.submenu_id
                    Inner Join Menu m on m.id=sm.menu_id
                    Inner Join Module mo on mo.id=m.module_id
                    where r.id='" + userSession.User.UserRole.Id.ToString() + "' order by mo.slNo asc";

                    IDataReader oReader = new Menu().ExecuteQuery(sql);
                    while (oReader.Read())
                    {
                        modules.Add(new TopMenu() { Action = oReader.GetValue(0).ToString(), Name = oReader.GetValue(1).ToString() }
                        );
                    }
                    oReader.Close();
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete GetMenus .");
                    //actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Savings_Product, ActionItem.Read, "Product Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return modules;
        }

        /// <summary>
        /// Get Menu Button List
        /// </summary>
        /// <param name="topMenuId">Module Id</param>
        /// <returns>object of MenuButtonList</returns>
        public List<MenuButton> GetMenuButtonList(string topMenuId)
        {
            var menuButtonList = new List<MenuButton>();
            var userSession = AuthorizationHelper.GetSession();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    string sql = @"select Distinct m.name as MenuName, sm.name as SubMenuName,sm.plugin_name as Pluginname, m.slNo,sm.slNo 
                    from role r
                    INNER JOIN RolePermission rp on rp.role_id=r.id
                    INNER JOIN Permission p on p.id=rp.permission_id
                    INNER JOIN Item i on i.id=p.item_id
                    INNEr Join SubMenu sm on sm.id=i.submenu_id
                    Inner Join Menu m on m.id=sm.menu_id
                    where r.id='" + userSession.User.UserRole.Id.ToString() + "' "
                    + "AND m.module_id='" + topMenuId + "'"
                    + " order by m.slNo, m.name,sm.slNo, sm.name";

                    IDataReader oReader = new Menu().ExecuteQuery(sql);
                    string menuName = "";
                    List<MenuButton> children = new List<MenuButton>();

                    while (oReader.Read())
                    {
                        if (menuName == "" || menuName == oReader.GetValue(0).ToString())
                        {
                            children.Add(
                               new MenuButton()
                               {
                                   BtnID = oReader.GetValue(1).ToString(),
                                   BtnName = oReader.GetValue(1).ToString(),
                                   ControllerInfo =
                                       new ControllerInfo() { ControllerHelperName = oReader.GetValue(2).ToString() }
                               });
                            menuName = oReader.GetValue(0).ToString();

                        }
                        else
                        {
                            menuButtonList.Add(
                                new MenuButton()
                                {
                                    BtnID = menuName,
                                    BtnName = menuName,
                                    ControllerInfo = children[0].ControllerInfo,
                                    Children = children.ToArray()
                                }
                            );
                            menuName = oReader.GetValue(0).ToString();
                            children = new List<MenuButton>();
                            children.Add(
                                new MenuButton()
                                {
                                    BtnID = oReader.GetValue(1).ToString(),
                                    BtnName = oReader.GetValue(1).ToString(),
                                    ControllerInfo =
                                        new ControllerInfo() { ControllerHelperName = oReader.GetValue(2).ToString() }
                                });
                        }
                    }
                    // for Last one 
                    menuButtonList.Add(
                            new MenuButton()
                            {
                                BtnID = menuName,
                                BtnName = menuName,
                                ControllerInfo = children[0].ControllerInfo,
                                Children = children.ToArray()
                            }
                        );
                    oReader.Close();
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete GetMenus .");
                    //actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Savings_Product, ActionItem.Read, "Product Read successfully");
                }
                catch (Exception)
                {
                    //throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return menuButtonList;
        }

        /// <summary>
        /// Get Action Permissions 
        /// </summary>
        /// <returns></returns>
        public List<PermissionInfo> GetActionPermissions(string userId, string pluginName, string actionName)
        {
            List<PermissionInfo> permissions = new List<PermissionInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call Get Action Permission list .");

            try
            {
                permissions = (from mod in IssueTrackerDataServices.Modules
                               join menu in IssueTrackerDataServices.Menus on mod.Id equals menu.ModuleId
                               join subMenu in IssueTrackerDataServices.SubMenus on menu.Id equals subMenu.MenuId
                               join item in IssueTrackerDataServices.Items on subMenu.Id equals item.SubMenuId
                               join permission in IssueTrackerDataServices.Permissions on item.Id equals permission.ItemId
                               join rolepermission in IssueTrackerDataServices.RolePermissions on permission.Id equals rolepermission.PermissionId
                               join role in IssueTrackerDataServices.Roles on rolepermission.RoleId equals role.Id
                               join userinfo in IssueTrackerDataServices.Users on role.Id equals userinfo.UserRole.Id
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
                issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete Get Action Permission function.");
            }
            catch (Exception ex)
            {
                throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
            }

            return permissions;
        }

        /// <summary>
        ///  Create Fixed Asset Opening 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IssueInfo CreateIssueTracking(IssueInfo objIssueTrackingInfo)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call Save FixedAssetOpening ");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var selectedProjectId = objIssueTrackingInfo.Project;
                    var oIssueAttachmentInfo = new List<IssueAttachmentInfo>();
                    oIssueAttachmentInfo = objIssueTrackingInfo.AttachmentFiles;

                    var oIssueConvertedValue = IssueTrackerServiceUtil.ConvertIssueTrackingInfoToIssueTracking(null, objIssueTrackingInfo);
                    var maxIssueNo = IssueTrackerDataServices.IssueTrackings.Where(it => it.Project == selectedProjectId).Max(it => it.IssueNo);

                    var issueNo = string.Empty;
                    string PrefixDigit = "D" + 5.ToString();
                    if (maxIssueNo == null)
                    {
                        var projectInfo = IssueTrackerDataServices.Projects.FirstOrDefault(p => p.Id == selectedProjectId);
                        issueNo = projectInfo.ProjectName.Substring(0, 3).ToUpper() + 1.ToString(PrefixDigit);
                    }
                    else
                    {
                        issueNo = maxIssueNo.Substring(0, 3).ToUpper() + (Convert.ToInt32(maxIssueNo.Substring(3)) + 1).ToString(PrefixDigit);
                    }
                    oIssueConvertedValue.IssueNo = issueNo;
                    oIssueConvertedValue.Project = selectedProjectId;
                    oIssueConvertedValue.Save();  // Save Issue Details 

                    // Attachment Files List

                    if (oIssueAttachmentInfo.Count > 0)
                    {
                        var oIssueDetails = IssueTrackerDataServices.IssueTrackings.Where(it => it.IssueNo == issueNo).FirstOrDefault();

                        foreach (var attachmentFiles in oIssueAttachmentInfo)
                        {

                            attachmentFiles.IssueId = oIssueDetails.Id;
                            attachmentFiles.AttachmentDate = oIssueDetails.CreationDate;
                            var oAttachmentFileSave = IssueTrackerServiceUtil.ConvertIssueAttachmentInfoToIssueAttachment(null, attachmentFiles);
                            oAttachmentFileSave.Save();  // Save Attachment
                        }
                        issueTrackerLogger.Log(EnumLogLevel.Debug, "Attachment Files has been Saved.");
                    }
                    var UserInfo = IssueTrackerDataServices.Users.FirstOrDefault(u => u.Id == objIssueTrackingInfo.Assignee);
                    var creatorInfo = IssueTrackerDataServices.Users.FirstOrDefault(u => u.Id == objIssueTrackingInfo.Creator);
                    if (UserInfo.Email != creatorInfo.Email)
                    {
                        SendMessage(UserInfo.Email, objIssueTrackingInfo);
                        SendMessage(creatorInfo.Email, objIssueTrackingInfo);
                    }
                    else
                    {
                        SendMessage(creatorInfo.Email, objIssueTrackingInfo);
                    }
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Attachment Files has been Saved.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Loan_Disburse_Realization_Opening, ActionItem.Create, "Settings Finances  Created successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }

            }


            return objIssueTrackingInfo;
        }

        //private bool UploadFileWhileCreatingIssue(string fileName, Stream stream)
        //{
        //    var value = false;


        //    return value;
        //}

        public string UploadFile(string fileName, Stream stream)
        {
            var newFileName = Guid.NewGuid() + "_" + fileName;
            string FilePath = ConfigurationManager.AppSettings["FileResourcePath"] + newFileName;
            string path = ConfigurationManager.AppSettings["FileResourcePath"];
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int length = 0;
            using (FileStream writer = new FileStream(FilePath, FileMode.Create))
            {
                int readCount;
                var buffer = new byte[8192];
                while ((readCount = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    writer.Write(buffer, 0, readCount);
                    length += readCount;
                }
            }

            return newFileName;
        }




        public Stream DownloadFile(string fileName, string fileExtension)
        {
            string downloadFilePath = ConfigurationManager.AppSettings["FileResourcePath"] + fileName + "." + fileExtension;
            String headerInfo = "attachment; filename=" + fileName + "." + fileExtension;
            WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = headerInfo;
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
            return File.OpenRead(downloadFilePath);
        }


        public string DeleteFile(string fileName)
        {
            string[] filePaths = Directory.GetFiles(ConfigurationManager.AppSettings["FileResourcePath"] + fileName);
            foreach (string filePath in filePaths)
                File.Delete(filePath);

            return fileName;
        }


        /// <summary>
        ///  Asset Disposal List
        /// </summary>
        /// <returns></returns>
        public List<IssueInfo> GetIssues(string projectId)
        {
            var issuTrackers = new List<IssueInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call Get Issutracker list.");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {

                    if (userSession.User.UserRole.RoleName.ToLower() == "user")
                    {
                        issuTrackers = (from It in IssueTrackerDataServices.IssueTrackings
                                        join u in IssueTrackerDataServices.Users
                                        on It.Assignee equals u.Id
                                        join cu in IssueTrackerDataServices.Users
                                        on It.Creator equals cu.Id
                                        where It.Project == Guid.Parse(projectId)
                                        && It.Assignee == userSession.User.Id
                                        select new IssueInfo()
                                        {
                                            Id = It.Id,
                                            Priority = It.Priority,
                                            IssueStatus = It.IssueStatus,
                                            IsssueTitle = It.IsssueTitle,
                                            Creator = It.Creator,
                                            IssueNo = It.IssueNo,
                                            CreationDate = It.CreationDate,
                                            ModificationDate = It.ModificationDate,
                                            UserName = u.UserName,
                                            CreatorName = cu.UserName
                                        }).ToList();
                    }
                    else
                    {
                        issuTrackers = (from It in IssueTrackerDataServices.IssueTrackings
                                        join u in IssueTrackerDataServices.Users
                                        on It.Assignee equals u.Id
                                        join cu in IssueTrackerDataServices.Users
                                        on It.Creator equals cu.Id
                                        where It.Project == Guid.Parse(projectId)
                                        select new IssueInfo()
                                        {
                                            Id = It.Id,
                                            Priority = It.Priority,
                                            IssueStatus = It.IssueStatus,
                                            IsssueTitle = It.IsssueTitle,
                                            Creator = It.Creator,
                                            IssueNo = It.IssueNo,
                                            CreationDate = It.CreationDate,
                                            ModificationDate = It.ModificationDate,
                                            UserName = u.UserName,
                                            CreatorName = cu.UserName
                                        }).ToList();
                    }
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete Get Asset Disposals List.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Savings_Product, ActionItem.Read, "Savings Product Type List  Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return issuTrackers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public IssueInfo GetIssue(string id)
        {
            var issueInfos = new IssueInfo();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call Get Issutracker list.");
            var userSession = AuthorizationHelper.GetSession();
            var dtpCollectionDate = userSession.CurrentDate;

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    issueInfos = (from It in IssueTrackerDataServices.IssueTrackings
                                  join u in IssueTrackerDataServices.Users
                                  on It.Assignee equals u.Id
                                  join cu in IssueTrackerDataServices.Users
                                  on It.Creator equals cu.Id
                                  where It.Id == Guid.Parse(id)
                                  select new IssueInfo()
                                  {
                                      Id = It.Id,
                                      Priority = It.Priority,
                                      IssueStatus = It.IssueStatus,
                                      IsssueTitle = It.IsssueTitle,
                                      Creator = It.Creator,
                                      IssueNo = It.IssueNo,
                                      CreationDate = It.CreationDate,
                                      ModificationDate = It.ModificationDate,
                                      UserName = u.UserName,
                                      CreatorName = cu.UserName,
                                      Modifier = It.Modifier,
                                      IssueDescription = It.IssueDescription,
                                      Project = It.Project,
                                      Severity = It.Severity
                                  }).ToList()[0];
                    if (issueInfos != null)
                    {
                        if (issueInfos.Modifier != null) issueInfos.ModifiedUser = IssueTrackerDataServices.Users.SingleOrDefault(u => u.Id == issueInfos.Modifier).UserName;
                        var AttachmentFiles = IssueTrackerDataServices.IssueAttachments.Where(afl => afl.IssueId == Guid.Parse(id)).ToList();
                        issueInfos.AttachmentFiles = new List<IssueAttachmentInfo>();
                        issueInfos.AttachmentFiles.AddRange(AttachmentFiles.Select(IssueTrackerServiceUtil.ConvertIssueAttachmentToIssueAttachmentInfo));
                    }

                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete GetIssutracker function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Fine_Table, ActionItem.Create, "Generate Dropout Transaction successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }

            }
            return issueInfos;
        }

        public bool DeleteIssue(string id)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call DeleteIssueTrack to delete Issue.");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {

                var Issueupdate = IssueTrackerDataServices.IssueTrackings.FirstOrDefault(ad => ad.Id == Guid.Parse(id));
                if (userSession.User.Id == Issueupdate.Creator || userSession.User.UserId == "admin")
                {
                    try
                    {
                        var itemIssue = IssueTrackerDataServices.IssueTrackings.FirstOrDefault(ad => ad.Id == Guid.Parse(id));
                        var AttachmentFiles = IssueTrackerDataServices.IssueAttachments.Where(afl => afl.IssueId == itemIssue.Id).ToList();
                        var fileLocationFolder = ConfigurationManager.AppSettings["FileResourcePath"].ToString();
                        string[] filePaths = Directory.GetFiles(fileLocationFolder);
                        for (int i = 0; i < AttachmentFiles.Count; i++)
                        {
                            var file = filePaths.FirstOrDefault(f => f == fileLocationFolder + AttachmentFiles[i].FileLocation);
                            if (file != null) File.Delete(file);
                            AttachmentFiles[i].Delete();
                        }

                        if (itemIssue != null) itemIssue.Delete();
                        issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete DeleteAssetDisposal function.");
                        actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Fine_Table, ActionItem.Delete, "Delete SecondLedgerEntry successfully");
                    }

                    catch (Exception ex)
                    {
                        throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                    }
                }
                else
                {
                    throw AuthorizationHelper.GenerateServiceError("You are not authorized to Delete this 'ISSUE' . Only Issue Crator can Delete this ISSUE . ", HttpStatusCode.ExpectationFailed, logMessage.Clone(EnumLogLevel.Warn));
                }

            }
            return true;
        }

        public IssueInfo UpdateIssue(string id, IssueInfo oIssueInfo)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call UpdateAssetDisposal to update AssetDisposal.");
            var userSession = AuthorizationHelper.GetSession();
            var oIssueAttachmentInfo = new List<IssueAttachmentInfo>();
            oIssueAttachmentInfo = oIssueInfo.AttachmentFiles;

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var Issueupdate = IssueTrackerDataServices.IssueTrackings.FirstOrDefault(ad => ad.Id == Guid.Parse(id));
                    if (userSession.User.Id == Issueupdate.Assignee || userSession.User.Id == Issueupdate.Creator || userSession.User.UserId == "admin")
                    {
                        var IssueupdateValue = IssueTrackerServiceUtil.ConvertIssueTrackingInfoToIssueTracking(Issueupdate, oIssueInfo);
                        IssueupdateValue.Save();
                        issueTrackerLogger.Log(EnumLogLevel.Debug, "Completion of Update Issueinfo.");
                        actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Fine_Table, ActionItem.Update, "Update ISSUE successfully");

                        // Attachment Files List
                        if (oIssueAttachmentInfo.Count > 0)
                        {

                            foreach (var attachmentFiles in oIssueAttachmentInfo)
                            {
                                attachmentFiles.IssueId = oIssueInfo.Id;
                                attachmentFiles.AttachmentDate = oIssueInfo.CreationDate;
                                var oAttachmentFileSave = IssueTrackerServiceUtil.ConvertIssueAttachmentInfoToIssueAttachment(null, attachmentFiles);
                                oAttachmentFileSave.Save();  // Save Attachment
                            }
                            var UserInfo = IssueTrackerDataServices.Users.FirstOrDefault(u => u.Id == oIssueInfo.Assignee);
                            var creatorInfo = IssueTrackerDataServices.Users.FirstOrDefault(u => u.Id == oIssueInfo.Creator);
                            if (UserInfo.Email != creatorInfo.Email)
                            {
                                SendMessage(UserInfo.Email, oIssueInfo);
                                SendMessage(creatorInfo.Email, oIssueInfo);
                            }
                            else { SendMessage(creatorInfo.Email, oIssueInfo); }
                            issueTrackerLogger.Log(EnumLogLevel.Debug, "Attachment Files has been Saved.");
                        }
                    }
                    else
                    {
                        throw AuthorizationHelper.GenerateServiceError("You are not authorized to update this 'ISSUE' . Only assigned Person can UPDATE this ISSUE . ", HttpStatusCode.ExpectationFailed, logMessage.Clone(EnumLogLevel.Warn));
                    }




                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }

            }
            return oIssueInfo;
        }

        /// <summary>
        ///  User
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> GetUserList(string projectId)
        {
            var users = new List<UserInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call Get Issutracker list.");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    users = (from u in IssueTrackerDataServices.Users
                             join up in IssueTrackerDataServices.UserProjects
                             on u.Id equals up.UserId
                             where up.ProjectId == Guid.Parse(projectId)
                             select new UserInfo()
                             {
                                 Id = u.Id,
                                 UserId = u.UserId,
                                 UserName = u.UserName
                             }).ToList().GroupBy(u => u.Id).Select(u => u.First()).ToList();

                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete Get Users List.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Savings_Product, ActionItem.Read, "Savings Product Type List  Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return users;
        }


        private void SendMessage(String email, IssueInfo objIssueTrackingInfo)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);

            var subject = "" + objIssueTrackingInfo.IsssueTitle + ", Priority : " + objIssueTrackingInfo.Priority;
            var mailBody = "Issue Status: " + objIssueTrackingInfo.IssueStatus + "\nSeverity: " + objIssueTrackingInfo.Severity
                + "\nIssue Type: " + objIssueTrackingInfo.IssueType + "\n\nDescription: \n" + objIssueTrackingInfo.IssueDescription;

             var File = @"E:\Resources\IssueTracker\TMS.txt";

             Attachment data = new Attachment(File);

            var smtpConfig = GetSMTPConfiguration();
            SmtpClient smtpClient = new SmtpClient(smtpConfig.HostName, smtpConfig.PortNumber);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpConfig.UserId, smtpConfig.Password);

            MailMessage message = new MailMessage();
            message.From = new MailAddress("issue@initvent.com");
            message.Subject = subject;
            message.Body = mailBody;
            message.Attachments.Add(data);

            message.To.Clear();

            message.To.Add(email);
            smtpClient.Send(message);

        }

        private SmtpConfig GetSMTPConfiguration()
        {
            var smtpConfig = new SmtpConfig();
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\SMTP.config";
            XmlRootAttribute attribute = new XmlRootAttribute();
            attribute.Namespace = string.Empty;
            attribute.ElementName = "SmtpConfig";
            XmlSerializer serializer = new XmlSerializer(typeof(SmtpConfig), attribute);
            XmlTextReader textReader = new XmlTextReader(filePath);
            if (serializer.CanDeserialize(textReader))
            {
                smtpConfig = (SmtpConfig)serializer.Deserialize(textReader);
            }
            return smtpConfig;
        }


        #region Project
        /// <summary>
        /// Get list of ProjectInfo
        /// </summary>
        /// <returns></returns>  
        public List<ProjectInfo> GetProjects()
        {
            var projects = new List<ProjectInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call GetProjects to get projects.");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    List<Project> projectList = null;
                    if (userSession.User.UserId == "admin")
                    {
                        projectList = IssueTrackerDataServices.Projects.ToList();
                        projects.AddRange(projectList.Select(IssueTrackerServiceUtil.ConvertProjectToProjectInfo));
                    }
                    else
                    {
                        projects.AddRange((from p in IssueTrackerDataServices.Projects
                                           join up in IssueTrackerDataServices.UserProjects
                                           on p.Id equals up.ProjectId
                                           where up.UserId == userSession.User.Id
                                           select new ProjectInfo()
                                           {
                                               Id = p.Id,
                                               ProjectCode = p.ProjectCode,
                                               ProjectName = p.ProjectName,
                                               StartDate = p.StartDate
                                           }).ToList());
                    }
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete GetProjects function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Project, ActionItem.Read, "Projects Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return projects;
        }

        /// <summary>
        /// Create a project
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        public ProjectInfo CreateProject(ProjectInfo projectInfo)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call CreateProject function to create a project.");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var duplicateMessage = CheckForDuplicateProject(true, projectInfo);
                    if (string.IsNullOrEmpty(duplicateMessage))
                    {
                        var project = IssueTrackerServiceUtil.ConvertProjectInfoToProject(null, projectInfo);
                        project.Save();
                        projectInfo.Id = project.Id;
                    }
                    else
                    {
                        throw AuthorizationHelper.GenerateServiceError(duplicateMessage, HttpStatusCode.Conflict, logMessage.Clone(EnumLogLevel.Warn));
                    }
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete CreateProject function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Project, ActionItem.Create, "Project Created successfully");
                }
                catch (WebFaultException<string> ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }

            return projectInfo;
        }

        /// <summary>
        /// Update a project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="projectInfo"></param>
        public ProjectInfo UpdateProject(string id, ProjectInfo projectInfo)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call UpdateProject function to update a project.");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var duplicateMessage = CheckForDuplicateProject(false, projectInfo, projectInfo.Id);
                    if (string.IsNullOrEmpty(duplicateMessage))
                    {
                        var project = IssueTrackerDataServices.Projects.FirstOrDefault(d => d.Id == projectInfo.Id);
                        project = IssueTrackerServiceUtil.ConvertProjectInfoToProject(project, projectInfo);
                        project.Save();
                    }
                    else
                    {
                        throw AuthorizationHelper.GenerateServiceError(duplicateMessage, HttpStatusCode.Conflict, logMessage.Clone(EnumLogLevel.Warn));
                    }
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete UpdateProject function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Project, ActionItem.Update, "Project Updated successfully");
                }
                catch (WebFaultException<string> ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return projectInfo;
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteProject(string id)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call DeleteProject function to delete a project.");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var project = IssueTrackerDataServices.Projects.FirstOrDefault(p => p.Id == Guid.Parse(id));
                    if (project != null)
                        project.Delete();
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete DeleteProject function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.Project, ActionItem.Delete, "Project Deleted successfully");
                }
                catch (WebFaultException<string> ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return true;
        }

        /// <summary>
        /// Check for duplicate project and return message
        /// </summary>
        /// <param name="isNew"> </param>
        /// <param name="projectInfo"> </param>
        /// <param name="projectId"> </param>
        /// <returns></returns>
        private string CheckForDuplicateProject(bool isNew, ProjectInfo projectInfo, Guid? projectId = null)
        {
            var returnString = string.Empty;
            var existingProjects = IssueTrackerDataServices.Projects.Where(p => p.ProjectCode == projectInfo.ProjectCode).ToList();
            if ((isNew && existingProjects.Count > 0) || (!isNew && existingProjects.Count > 1) || (!isNew && existingProjects.Count == 1 && existingProjects[0].Id != projectId))
                returnString = string.Format(ApplicationErrorMessages.DuplicateProject, "Project Code '" + projectInfo.ProjectCode + "'"); ;
            if (returnString == string.Empty)
            {
                existingProjects = IssueTrackerDataServices.Projects.Where(p => p.ProjectName == projectInfo.ProjectName).ToList();
                if ((isNew && existingProjects.Count > 0) || (!isNew && existingProjects.Count > 1) || (!isNew && existingProjects.Count == 1 && existingProjects[0].Id != projectId))
                    returnString = string.Format(ApplicationErrorMessages.DuplicateProject, "Project Name '" + projectInfo.ProjectName + "'"); ;
            }
            return returnString;
        }
        #endregion


        /// <summary>
        /// Get User List
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> GetUsers()
        {
            var userInfos = new List<UserInfo>();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call GetUsers to get users.");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    IQueryable<User> users = null;
                    if (userSession.User.UserId == "admin")
                    {
                        users = IssueTrackerDataServices.Users;
                        userInfos.AddRange(users.Select(IssueTrackerServiceUtil.ConvertUserToUserInfo));
                    }
                    else
                    {
                        userInfos = (from u in IssueTrackerDataServices.Users
                                     join up in IssueTrackerDataServices.UserProjects
                                     on u.Id equals up.UserId
                                     select new UserInfo()
                                     {
                                         Id = u.Id,
                                         UserName = u.UserName,
                                         UserId = u.UserId,
                                         UserStatus = u.Status,
                                         Address = u.Address,
                                         Mobile = u.Mobile,
                                         Email = u.Email,
                                         UserRole = IssueTrackerServiceUtil.ConvertRoleToRoleInfo(u.UserRole)
                                     }).ToList().GroupBy(u => u.Id).Select(u => u.First()).ToList();

                    }

                    issueTrackerLogger.Log(EnumLogLevel.Debug, "Complete GetUsers function.");
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.User_Info, ActionItem.Read, "User Read successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError,
                                                                   logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return userInfos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserInfo GetUser(string userId)
        {
            var userInfo = new UserInfo();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call GetUser to get a user ");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var user = IssueTrackerDataServices.Users.SingleOrDefault(u => u.UserId == userId);
                    if (user == null)
                    {
                        userInfo.UserId = userId;

                    }
                    else
                    {
                        userInfo = IssueTrackerServiceUtil.ConvertUserToUserInfo(user);
                        userInfo.UserProjects = (from up in IssueTrackerDataServices.UserProjects
                                                 where up.UserId == user.Id
                                                 select new UserProjectInfo()
                                                 {
                                                     Id = up.Id,
                                                     UserId = up.UserId,
                                                     ProjectId = up.ProjectId
                                                 }).ToList();
                    }
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "GetUser execute successfully.");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return userInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public UserInfo CreateUser(UserInfo userInfo)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call CreateUser to create a user ");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var user = IssueTrackerServiceUtil.ConvertUserInfoToUser(null, userInfo);
                    user.UserPassword = PasswordHash.Create(userInfo.UserPassword).ToString();
                    user.Save();
                    if (userInfo.UserProjects != null && userInfo.UserProjects.Count > 0)
                    {
                        for (int i = 0; i < userInfo.UserProjects.Count; i++)
                        {
                            var userProject = new UserProject()
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.Id,
                                ProjectId = userInfo.UserProjects[i].ProjectId
                            };
                            userProject.Save();
                        }
                    }
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "CreateUser executed successfully.");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return userInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public UserInfo UpdateUser(string id, UserInfo userInfo)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call UpdateUser to update an exinsting user");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var user = IssueTrackerDataServices.Users.SingleOrDefault(u => u.Id == Guid.Parse(id));
                    user = IssueTrackerServiceUtil.ConvertUserInfoToUser(user, userInfo);
                    user.Save();
                    if (userInfo.UserProjects != null && userInfo.UserProjects.Count > 0)
                    {
                        new UserProject().ExecuteNonQuery("Delete from User_Project where user_id ='" + user.Id + "'");
                        for (int i = 0; i < userInfo.UserProjects.Count; i++)
                        {
                            var userProject = new UserProject()
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.Id,
                                ProjectId = userInfo.UserProjects[i].ProjectId
                            };
                            userProject.Save();
                        }
                    }
                    issueTrackerLogger.Log(EnumLogLevel.Debug, "UpdateUser executed successfully.");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return userInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(string id)
        {
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call DeleteUser to inactive a user ");
            var userSession = AuthorizationHelper.GetSession();
            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var user = IssueTrackerDataServices.Users.SingleOrDefault(u => u.Id == Guid.Parse(id));
                    if (user != null) user.Delete();
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError, logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserInfo GenerateUserPassword(string id)
        {
            var userInfo = new UserInfo();
            var logMessage = new LogMessage(EnumLogLevel.Info);
            issueTrackerLogger.Log(EnumLogLevel.Debug, "Call GetUsers to get users.");
            var userSession = AuthorizationHelper.GetSession();

            if (!string.IsNullOrEmpty(userSession.User.UserId))
            {
                try
                {
                    var user = IssueTrackerDataServices.Users.FirstOrDefault(u => u.Id == Guid.Parse(id));
                    var newPassword = Guid.NewGuid().ToString().Split('-')[0];
                    user.UserPassword = SecurityServices.CreatePasswordHash(newPassword);
                    user.Save();
                    userInfo.Id = user.Id;
                    userInfo.UserName = user.UserName;
                    userInfo.UserPassword = newPassword;
                    actionLogger.Log(userSession.User.Id, userSession.User.UserId, userSession.User.UserName, userSession.SessionToken.ToString(), ActionLogItem.User_Info, ActionItem.Create, "User Password Created successfully");
                }
                catch (Exception ex)
                {
                    throw AuthorizationHelper.GenerateServiceError(ex.Message, HttpStatusCode.InternalServerError,
                                                                   logMessage.Clone(EnumLogLevel.Warn));
                }
            }
            return userInfo;
        }
    }
}
