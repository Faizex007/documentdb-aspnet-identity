using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DocumentDB.AspNet.Identity
{
  public class IdentityUser : IUser<string>
  {
    public IdentityUser()
    {
      Id = Guid.NewGuid().ToString();
      Claims = new List<IdentityUserClaim>();
      Roles = new List<string>();
      Logins = new List<UserLoginInfo>();
    }

    public IdentityUser(string userName) : this()
    {
      UserName = userName;
    }

    #region Public Properties

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    public string UserName { get; set; }

    public virtual string SecurityStamp { get; set; }

    public virtual string PasswordHash { get; set; }

    public virtual string Email { get; set; }

    public virtual bool EmailConfirmed { get; set; }

    public virtual string PhoneNumber { get; set; }

    public virtual bool PhoneNumberConfirmed { get; set; }

    public virtual bool TwoFactorEnabled { get; set; }

    public virtual DateTime? LockoutEndDateUtc { get; set; }

    public virtual bool LockoutEnabled { get; set; }

    public virtual int AccessFailedCount { get; set; }

    public List<string> Roles { get; }

    public string TenantId { get; set; }

    public virtual List<UserLoginInfo> Logins { get; private set; }

    public virtual List<IdentityUserClaim> Claims { get; private set; }

    #endregion

    #region Roles

    public virtual void AddRole(string role)
    {
      Roles.Add(role);
    }

    public virtual void RemoveRole(string role)
    {
      Roles.Remove(role);
    }

    #endregion

    #region Logins

    public virtual void AddLogin(UserLoginInfo login)
    {
      Logins.Add(login);
    }

    public virtual void RemoveLogin(UserLoginInfo login)
    {
      var loginsToRemove = Logins.Where(l => l.LoginProvider == login.LoginProvider).Where(l => l.ProviderKey == login.ProviderKey);
      Logins = Logins.Except(loginsToRemove).ToList();
    }

    #endregion

    #region Claims

    public virtual void AddClaim(Claim claim)
    {
      Claims.Add(new IdentityUserClaim(claim));
    }

    public virtual void RemoveClaim(Claim claim)
    {
      var claimsToRemove = Claims.Where(c => c.Type == claim.Type).Where(c => c.Value == claim.Value);
      Claims = Claims.Except(claimsToRemove).ToList();
    }

    #endregion
  }
}