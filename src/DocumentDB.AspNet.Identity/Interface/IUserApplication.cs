using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace DocumentDB.AspNet.Identity.Interface
{
  public interface IUserApplication
  {
    /// <summary>
    /// Bson Id of the user
    /// </summary>
    string UserId { get; set; }

    /// <summary>
    /// Unique key for the user
    /// </summary>
    /// <value>The identifier.</value>
    /// <returns>The unique key for the user</returns>
    string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password hash.
    /// </summary>
    /// <value>The password hash.</value>
    string PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the security stamp.
    /// </summary>
    /// <value>The security stamp.</value>
    string SecurityStamp { get; set; }

    /// <summary>
    /// Gets the roles.
    /// </summary>
    /// <value>The roles.</value>
    List<string> Roles { get; }

    /// <summary>
    /// Gets the claims.
    /// </summary>
    /// <value>The claims.</value>
    List<IdentityUserClaim> Claims { get; }

    /// <summary>
    /// Gets the logins.
    /// </summary>
    /// <value>The logins.</value>
    List<UserLoginInfo> Logins { get; }

    Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserApplication> manager, string authenticationType);
  }
}