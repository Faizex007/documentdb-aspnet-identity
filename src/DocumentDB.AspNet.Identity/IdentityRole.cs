using System;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DocumentDB.AspNet.Identity
{
  [Serializable]
  public class IdentityRole : IRole<string>
  {
    public IdentityRole()
    {
      Id = Guid.NewGuid().ToString();
    }

    public IdentityRole(string roleName) : this()
    {
      Name = roleName;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; }

    public string Name { get; set; }
  }
}