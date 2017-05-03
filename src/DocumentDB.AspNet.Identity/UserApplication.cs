using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DocumentDB.AspNet.Identity.Interface;
using Microsoft.AspNet.Identity;

namespace DocumentDB.AspNet.Identity
{
  // You can add profile data for the user by adding more properties to your UserApplication class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  /// <summary>
  /// User information
  /// </summary>
  [Serializable]
  public class UserApplication : IdentityUser, IUserApplication, IUser
  {
    /// <summary>
    /// Bson Id of the user
    /// </summary>
    public string UserId { get; set; }
    public string PasswordResetToken { get; set; }

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserApplication> manager, string authenticationType)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

      // Add custom user claims here
      return userIdentity;
    }
  }
}