using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Security.Principal;

namespace Durados.Web.Membership
{
    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthentication
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool IsUserExist(string token);
        bool IsUserExistByEmail(string email);
        bool IsUserApproved(string token);
        bool IsUserApprovedByEmail(string email);
        void ApproveUser(string token);
        string GetUserToken(string userName);
        bool DeleteUser(string userName);
        MembershipCreateStatus CreateUser(string userName, string password, string email, bool approved);
        string GetUserName(string token);
        string GetUserName(Guid providerUserKey);
        
    }

    public class AccountMembershipService : IMembershipService
    {
        private MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {

        }

        

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? System.Web.Security.Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email, bool approved)
        {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, approved, null, out status);
            return status;
        }

        public bool DeleteUser(string userName)
        {
            return _provider.DeleteUser(userName, true);
        }

        public string GetUserToken(string userName)
        {
            return _provider.GetUser(userName, false).ProviderUserKey.ToString();
        }

        public bool ValidateUser(string userName, string password)
        {
            return _provider.ValidateUser(userName, password);
        }

        
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            return currentUser.ChangePassword(oldPassword, newPassword);
        }

        public bool IsUserExist(string token)
        {
            return IsUserExist(new Guid(token));
        }

        public bool IsUserExist(Guid providerUserKey)
        {
            return _provider.GetUser(providerUserKey, false) != null;
        }

        

        public bool IsUserExistByEmail(string email)
        {
            return _provider.GetUser(email, false) != null;
        }

        public bool IsUserApproved(string token)
        {
            return IsUserApproved(new Guid(token));
        }

        public bool IsUserApproved(Guid providerUserKey)
        {
            MembershipUser user = _provider.GetUser(providerUserKey, false);
            if (user == null)
            {
                return false;
            }
            return user.IsApproved;
        }

        public bool IsUserApprovedByEmail(string email)
        {
            MembershipUser user = _provider.GetUser(email, false);
            if (user == null)
            {
                return false;
            }
            return user.IsApproved;
        }

        public void ApproveUser(string token)
        {
            ApproveUser(new Guid(token));
        }

        public void ApproveUser(Guid providerUserKey)
        {
            MembershipUser user = _provider.GetUser(providerUserKey, false);
            if (user != null)
            {
                user.IsApproved = true;
                _provider.UpdateUser(user);
            }
        }

        
        public string GetUserName(string token)
        {
            return GetUserName(new Guid(token));
        }

        public string GetUserName(Guid providerUserKey)
        {
            return _provider.GetUser(providerUserKey, false).UserName;
        }
        

    }
}
