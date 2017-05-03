using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace DocumentDB.AspNet.Identity.Interface
{
  public interface IUserStore<TUser> where TUser : IdentityUser
  {
    string CollectionName { set; }

    Task AddClaimAsync(TUser user, Claim claim);
    Task<IList<Claim>> GetClaimsAsync(TUser user);
    Task RemoveClaimAsync(TUser user, Claim claim);

    Task CreateAsync(TUser user);
    Task DeleteAsync(TUser user);
    Task<TUser> FindByIdAsync(string userId);
    Task<TUser> FindByNameAsync(string userName);
    Task UpdateAsync(TUser user);

    void Dispose();
    Task AddLoginAsync(TUser user, UserLoginInfo login);
    Task<TUser> FindAsync(UserLoginInfo login);
    Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user);
    Task RemoveLoginAsync(TUser user, UserLoginInfo login);

    Task<string> GetPasswordHashAsync(TUser user);
    Task<bool> HasPasswordAsync(TUser user);
    Task SetPasswordHashAsync(TUser user, string passwordHash);

    Task AddToRoleAsync(TUser user, string role);
    Task<IList<string>> GetRolesAsync(TUser user);
    Task<bool> IsInRoleAsync(TUser user, string role);
    Task RemoveFromRoleAsync(TUser user, string role);

    Task<string> GetSecurityStampAsync(TUser user);
    Task SetSecurityStampAsync(TUser user, string stamp);
  }
}