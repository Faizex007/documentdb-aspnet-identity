using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace DocumentDB.AspNet.Identity
{
  public class IdentityTokenProvider<TUser> : IUserTokenProvider<UserApplication, string> where TUser : class, IUser
  {
    public async Task<string> GenerateAsync(string purpose, UserManager<UserApplication, string> manager, UserApplication user)
    {
      var resetToken = Guid.NewGuid().ToString();
      user.PasswordResetToken = resetToken;
      await manager.UpdateAsync(user);

      return resetToken;
    }

    public Task<bool> IsValidProviderForUserAsync(UserManager<UserApplication, string> manager, UserApplication user)
    {
      if (manager == null)
        throw new ArgumentNullException();

      return Task.FromResult(manager.SupportsUserPassword);
    }

    public Task NotifyAsync(string token, UserManager<UserApplication, string> manager, UserApplication user)
    {
      return Task.FromResult(0);
    }

    public Task<bool> ValidateAsync(string purpose, string token, UserManager<UserApplication, string> manager, UserApplication user)
    {
      return Task.FromResult(user.PasswordResetToken == token);
    }
  }
}