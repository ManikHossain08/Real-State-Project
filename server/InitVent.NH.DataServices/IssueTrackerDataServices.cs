//-----------------------------------------------------------------------
// <copyright file="IssueTrackerDataServices.cs" company="InitVent Consulting Services.">
//     Copyright (c) InitVent Consulting Services. All rights reserved.
// </copyright>
// <author>S.M. Saiful Islam</author>
//-----------------------------------------------------------------------
namespace InitVent.DataServices.NHibernate.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using InitVent.ORM;
    using System.Text.RegularExpressions;
    using InitVent.DataServices.Domain;

    /// <summary>
    /// UserManagement module data services, used to access all the objects in this domain.
    /// Extra auxiliary methods are also provided.
    /// </summary>
    public static class IssueTrackerDataServices
    {
        /// <summary>
        /// The key to read domain assembly class names from app.config or web.config.
        /// </summary>
        public const string dataServicesKey = "IssueTrackerDataServices";

        /// <summary>
        /// The default schema name for all the domain objects under UserManage module.
        /// </summary>
        public const string defaultSchema = "dbo";

        /// <summary>
        /// This services list include the current data services and all the data services that this one depends on.
        /// </summary>
        private static string[] dataServicesList = new string[] { "InitVent.DataServices.NHibernate.Repository.IssueTrackerDataServices, InitVent.NH.DataServices" };

        /// <summary>
        /// The data services instance.
        /// </summary>
        private static NHDataServices theDataService = new NHDataServices(
            System.Configuration.ConfigurationManager.ConnectionStrings[dataServicesKey].ToString(),
            System.Configuration.ConfigurationManager.AppSettings[dataServicesKey].ToString().Split(','),
            dataServicesList
        );

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        // http://www.yoda.arachsys.com/csharp/singleton.html
        static IssueTrackerDataServices()
        {
        }

        /// <summary>
        /// Gets current active session for the data service.
        /// </summary>
        public static GenericSession CurrentSession
        {
            get
            {
                return DataServiceInstance.CurrentSession;
            }
        }

        /// <summary>
        /// Gets the singleton instance that wraps all the methods provided by NHibernate framework on session.
        /// </summary>
        public static NHDataServices DataServiceInstance
        {
            get
            {
                return theDataService;
            }
        }

        #region Entity IQueryables


        /// <summary>
        ///  Gets objects saved in table User.
        /// </summary>
        public static IQueryable<User> Users
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<User>();
            }
        }
       
        #region Role Permission


        public static IQueryable<Role> Roles
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<Role>();
            }
        }

        public static IQueryable<Module> Modules
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<Module>();
            }
        }

        public static IQueryable<Menu> Menus
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<Menu>();
            }
        }

        public static IQueryable<SubMenu> SubMenus
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<SubMenu>();
            }
        }
        
        public static IQueryable<Item> Items
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<Item>();
            }
        }

        public static IQueryable<Permission> Permissions
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<Permission>();
            }
        }

        public static IQueryable<RolePermission> RolePermissions
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<RolePermission>();
            }
        }
                

        #endregion

        #endregion

        #region Auxilary Methods
       
        /// <summary>
        /// Remove unnecessary characters
        /// </summary>
        /// <param name="v">String value</param>
        /// <returns>String with "\(\)\s" removed.</returns>
        private static string RemoveChar(string v)
        {
            Regex rgx = new Regex(@"(\s+)?(\.)?(\*)?(\()?(\))?");
            return rgx.Replace(v, string.Empty);
        }
        #endregion

        public static IQueryable<IssueTracking> IssueTrackings //populate data at main page as a list
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<IssueTracking>();
            }
        }

        public static IQueryable<IssueAttachment> IssueAttachments //populate data at main page as a list
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<IssueAttachment>();
            }
        }


        public static IQueryable<Project> Projects //populate data at main page as a list
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<Project>();
            }
        }
        /// <summary>
        /// Get project for user 
        /// </summary>
        public static IQueryable<UserProject> UserProjects 
        {
            get
            {
                return DataServiceInstance.CurrentSession.Query<UserProject>();
            }
        }
    
    }
}
