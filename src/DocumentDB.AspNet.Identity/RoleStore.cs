using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Azure.Documents.Client;

namespace DocumentDB.AspNet.Identity
{
  public class RoleStore<TRole> : IRoleStore<TRole>, IQueryableRoleStore<TRole> where TRole : IdentityRole
  {
    private readonly DocumentClient _client;

    public RoleStore(string endpoint, string accessKey, string databaseName, string collectionName)
    {
      if (string.IsNullOrEmpty(endpoint)) throw new ArgumentNullException(nameof(endpoint));
      if (string.IsNullOrEmpty(accessKey)) throw new ArgumentNullException(nameof(accessKey));
      if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException(nameof(databaseName));
      if (string.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));

      _client = new DocumentClient(new Uri(endpoint), accessKey);

      DatabaseName = databaseName;
      CollectionName = collectionName;
    }

    #region Public Properties

    public string DatabaseName { get; set; }

    public string CollectionName { get; set; }

    public virtual IQueryable<TRole> Roles => _client.CreateDocumentQuery<TRole>(GetDocumentCollectionUri());

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

    #region Public Methods

    public virtual Task CreateAsync(TRole role)
    {
      return _client.CreateDocumentAsync(GetDocumentCollectionUri(), role);
    }

    public virtual Task UpdateAsync(TRole role)
    {
      return _client.ReplaceDocumentAsync(GetDocumentUri(role.Id), role);
    }

    public virtual Task DeleteAsync(TRole role)
    {
      return _client.DeleteDocumentAsync(GetDocumentUri(role.Id));
    }

    public virtual Task<TRole> FindByIdAsync(string roleId)
    {
      return Task.Run(() => Roles.Where(f => f.Id == roleId).AsEnumerable().FirstOrDefault());
    }

    public virtual Task<TRole> FindByNameAsync(string roleName)
    {
      return Task.Run(() => Roles.Where(f => f.Name == roleName).AsEnumerable().FirstOrDefault());
    }

    public virtual void Dispose()
    {
    }

    #endregion
  }
}