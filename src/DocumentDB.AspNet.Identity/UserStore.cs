using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Azure.Documents.Client;

namespace DocumentDB.AspNet.Identity
{
  public class UserStore<TUser> : Interface.IUserStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserRoleStore<TUser>,
    IUserLoginStore<TUser>,
    IUserSecurityStampStore<TUser>,
    IUserEmailStore<TUser>,
    IUserClaimStore<TUser>,
    IQueryableUserStore<TUser>,
    IUserPhoneNumberStore<TUser>,
    IUserTwoFactorStore<TUser, string>,
    IUserLockoutStore<TUser, string>,
    IUserStore<TUser>
    where TUser : IdentityUser
  {
    #region Public Properties

    private readonly DocumentClient _client;

    public string TenantId { get; set; } = "global";

    public string DatabaseName { get; set; }

    public string CollectionName { get; set; }

    #endregion

    #region Constructors

    public UserStore(string endpoint, string accessKey, string databaseName, string collectionName)
    {
      if (string.IsNullOrEmpty(endpoint)) throw new ArgumentNullException(nameof(endpoint));
      if (string.IsNullOrEmpty(accessKey)) throw new ArgumentNullException(nameof(accessKey));
      if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException(nameof(databaseName));
      if (string.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));

      _client = new DocumentClient(new Uri(endpoint), accessKey);

      DatabaseName = databaseName;
      CollectionName = collectionName;
    }

    #endregion

    #region Uri Factory

    public Uri GetDocumentCollectionUri()
    {
      return UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
    }

    public Uri GetDocumentUri(string id)
    {
      return UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id);
    }

    #endregion

    #region User Management

    public Task CreateAsync(TUser user)
    {
      return _client.CreateDocumentAsync(GetDocumentCollectionUri(), user);
    }

    public Task UpdateAsync(TUser user)
    {
      if (user == null) throw new ArgumentNullException(nameof(user));

      if (string.IsNullOrWhiteSpace(user.TenantId))
        user.TenantId = TenantId;

      return _client.ReplaceDocumentAsync(GetDocumentUri(user.Id), user);
    }

    public Task DeleteAsync(TUser user)
    {
      if (user == null) throw new ArgumentNullException(nameof(user));
      return _client.DeleteDocumentAsync(GetDocumentUri(user.Id));
    }

    #endregion

    #region Find User

    public virtual IQueryable<TUser> Users => _client.CreateDocumentQuery<TUser>(GetDocumentCollectionUri());

    public Task<TUser> FindByIdAsync(string userId)
    {
      return Task.Run(() => Users.Where(f => f.Id == userId).AsEnumerable().FirstOrDefault());
    }

    public Task<TUser> FindByNameAsync(string userName)
    {
      return Task.Run(() => Users.Where(f => f.UserName == userName).AsEnumerable().FirstOrDefault());
    }

    #endregion

    #region Password Management

    public Task SetPasswordHashAsync(TUser user, string passwordHash)
    {
      if (user == null) throw new ArgumentNullException();
      user.PasswordHash = passwordHash;
      return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(TUser user)
    {
      if (user == null) throw new ArgumentNullException();
      return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(TUser user)
    {
      if (user == null) throw new ArgumentNullException();
      return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
    }

    #endregion

    #region Role Management

    public Task AddToRoleAsync(TUser user, string role)
    {
      if (!user.Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase))
        user.AddRole(role);

      return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(TUser user, string role)
    {
      user.RemoveRole(role);
      return Task.CompletedTask;
    }

    public Task<IList<string>> GetRolesAsync(TUser user)
    {
      return Task.FromResult((IList<string>)user.Roles);
    }

    public Task<bool> IsInRoleAsync(TUser user, string role)
    {
      return Task.FromResult(user.Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase));
    }

    #endregion

    #region Login Management

    public Task AddLoginAsync(TUser user, UserLoginInfo login)
    {
      if (!user.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
        user.AddLogin(login);

      return Task.FromResult(true);
    }

    public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
    {
      user.RemoveLogin(login);
      return Task.FromResult(0);
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
    {
      return Task.FromResult((IList<UserLoginInfo>)user.Logins);
    }

    public Task<TUser> FindAsync(UserLoginInfo login)
    {
      return Task.Run(() => Users.AsEnumerable().FirstOrDefault(f => f.Logins.Any(l => l.LoginProvider == login.LoginProvider &&
                                                                                       l.ProviderKey == login.ProviderKey)));
    }

    #endregion

    #region Security Stamp

    public Task<string> GetSecurityStampAsync(TUser user)
    {
      return Task.FromResult(user.SecurityStamp);
    }

    public Task SetSecurityStampAsync(TUser user, string stamp)
    {
      user.SecurityStamp = stamp;
      return Task.CompletedTask;
    }

    #endregion

    #region Email

    public virtual Task<bool> GetEmailConfirmedAsync(TUser user)
    {
      return Task.FromResult(user.EmailConfirmed);
    }

    public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed)
    {
      user.EmailConfirmed = confirmed;
      return Task.CompletedTask;
    }

    public virtual Task SetEmailAsync(TUser user, string email)
    {
      user.Email = email;
      return Task.CompletedTask;
    }

    public virtual Task<string> GetEmailAsync(TUser user)
    {
      return Task.FromResult(user.Email);
    }

    public virtual Task<TUser> FindByEmailAsync(string email)
    {
      return Task.Run(() => Users.Where(f => f.Email == email).AsEnumerable().FirstOrDefault());
    }

    #endregion

    #region Claim Management

    public Task AddClaimAsync(TUser user, Claim claim)
    {
      if (!user.Claims.Any(x => x.Type == claim.Type && x.Value == claim.Value))
        user.AddClaim(claim);

      return Task.CompletedTask;
    }

    public Task<IList<Claim>> GetClaimsAsync(TUser user)
    {
      return Task.FromResult((IList<Claim>)user.Claims.Select(c => c.ToSecurityClaim()).ToList());
    }

    public Task RemoveClaimAsync(TUser user, Claim claim)
    {
      user.RemoveClaim(claim);
      return Task.CompletedTask;
    }

    #endregion

    #region Lockout Management

    public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
    {
      return Task.FromResult(user.LockoutEndDateUtc ?? new DateTimeOffset());
    }

    public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
    {
      user.LockoutEndDateUtc = new DateTime(lockoutEnd.Ticks, DateTimeKind.Utc);
      return Task.CompletedTask;
    }

    public virtual Task<int> IncrementAccessFailedCountAsync(TUser user)
    {
      user.AccessFailedCount++;
      return Task.FromResult(user.AccessFailedCount);
    }

    public virtual Task ResetAccessFailedCountAsync(TUser user)
    {
      user.AccessFailedCount = 0;
      return Task.CompletedTask;
    }

    public virtual Task<int> GetAccessFailedCountAsync(TUser user)
    {
      return Task.FromResult(user.AccessFailedCount);
    }

    public virtual Task<bool> GetLockoutEnabledAsync(TUser user)
    {
      return Task.FromResult(user.LockoutEnabled);
    }

    public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled)
    {
      user.LockoutEnabled = enabled;
      return Task.CompletedTask;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
    }

    #endregion

    #region Phone Number

    public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
    {
      user.PhoneNumber = phoneNumber;
      return Task.CompletedTask;
    }

    public Task<string> GetPhoneNumberAsync(TUser user)
    {
      return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
    {
      return Task.FromResult(user.PhoneNumberConfirmed);
    }

    public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
    {
      user.PhoneNumberConfirmed = confirmed;
      return Task.CompletedTask;
    }

    #endregion

    #region Two Factor

    public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
    {
      user.TwoFactorEnabled = enabled;
      return Task.CompletedTask;
    }

    public Task<bool> GetTwoFactorEnabledAsync(TUser user)
    {
      return Task.FromResult(user.TwoFactorEnabled);
    }

    #endregion
  }
}