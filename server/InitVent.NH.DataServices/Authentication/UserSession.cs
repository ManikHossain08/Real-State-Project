
using InitVent.DataServices.Domain;

namespace IssueTracker.Domain.Authentication
{
    using InitVent.Metadata.Session.Impl;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web;
    public class UserSession : BasicSession
    {
        // FIXME: User contains password in plain text
        public User User { get; private set; }
        public Guid SelectedBranchId { get; set; }
        public string SelectedBranchCode { get; set; }
        public DateTime? CurrentDate { get; set; }
        public Func<ICollection<IAccessible>> GetAccessibles;

        private ICollection<IAccessible> accessibles;
        public UserSession()
        {
            
        }
        public UserSession(User user)
            : base()
        {
            User = user;
        }

        public UserSession(UserSession session)
            : base(session)
        {
            this.User = session.User;
            this.Accessibles = session.Accessibles;
        }
        public ICollection<IAccessible> SelectedAccessibles { get { return this.Accessibles.Where(acc => acc.Selected).ToList(); } }

        public ICollection<IAccessible> Accessibles
        {
            get
            {
                if (this.accessibles == null)
                {
                    // Lazy loading of accessibles, the application need to make sure everything required is loaded when it is trying to read accessible objects.
                    this.accessibles = this.GetAccessibles();
                }

                return this.accessibles;
            }

            set
            {
                this.accessibles = value;
            }
        }
    }
}