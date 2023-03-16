using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Repositories;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Cms.Core.Cache;

public sealed class UserCacheRefresher : PayloadCacheRefresherBase<UserCacheRefresherNotification, UserCacheRefresher.JsonPayload>
{
    #region Define

    public static readonly Guid UniqueId = Guid.Parse("E057AF6D-2EE6-41F4-8045-3694010F0AA6");

    [Obsolete("Use constructor that takes all parameters instead")]
    public UserCacheRefresher(AppCaches appCaches, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory factory)
        : this(appCaches, StaticServiceProvider.Instance.GetRequiredService<IJsonSerializer>(), eventAggregator, factory)
    {
    }

    public UserCacheRefresher(AppCaches appCaches, IJsonSerializer serializer, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory factory)
        : base(appCaches, serializer, eventAggregator, factory)
    {
    }

    public override Guid RefresherUniqueId => UniqueId;

    public override string Name => "User Cache Refresher";

    #endregion

    #region Json

    public class JsonPayload
    {
        public JsonPayload(int id, Guid key, bool removed)
        {
            Id = id;
            Key = key;
            Removed = removed;
        }

        public int Id { get; }

        public Guid Key { get; }

        public bool Removed { get; }
    }

    #endregion

    #region Refresher

    public override void RefreshAll()
    {
        ClearAllIsolatedCacheByEntityType<IUser>();

        base.RefreshAll();
    }

    public override void Refresh(int id)
    {
        ClearCache(id.Yield());

        base.Refresh(id);
    }

    public override void Remove(int id)
    {
        ClearCache(id.Yield());

        base.Remove(id);
    }

    public override void Refresh(JsonPayload[] payloads)
    {
        ClearCache(payloads.Select(x => x.Id));

        base.Refresh(payloads);
    }

    private void ClearCache(IEnumerable<int> ids)
    {
        Attempt<IAppPolicyCache?> userCache = AppCaches.IsolatedCaches.Get<IUser>();
        if (userCache.Success && userCache.Result is not null)
        {
            foreach (int id in ids)
            {
                userCache.Result.Clear(RepositoryCacheKeys.GetKey<IUser, int>(id));
                userCache.Result.ClearByKey(CacheKeys.UserContentStartNodePathsPrefix + id);
                userCache.Result.ClearByKey(CacheKeys.UserMediaStartNodePathsPrefix + id);
                userCache.Result.ClearByKey(CacheKeys.UserAllContentStartNodesPrefix + id);
                userCache.Result.ClearByKey(CacheKeys.UserAllMediaStartNodesPrefix + id);
            }
        }
    }

    #endregion
}
