using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Repositories;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Cms.Core.Cache;

public sealed class RelationTypeCacheRefresher : PayloadCacheRefresherBase<RelationTypeCacheRefresherNotification, RelationTypeCacheRefresher.JsonPayload>
{
    [Obsolete("Use constructor that takes all parameters instead")]
    public RelationTypeCacheRefresher(AppCaches appCaches, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory factory)
       : this(appCaches, StaticServiceProvider.Instance.GetRequiredService<IJsonSerializer>(), eventAggregator, factory)
    {
    }

    public RelationTypeCacheRefresher(AppCaches appCaches, IJsonSerializer jsonSerializer, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory factory)
        : base(appCaches, jsonSerializer, eventAggregator, factory)
    {
    }

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

    public static readonly Guid UniqueId = Guid.Parse("D8375ABA-4FB3-4F86-B505-92FBA1B6F7C9");

    public override Guid RefresherUniqueId => UniqueId;

    public override string Name => "Relation Type Cache Refresher";

    public override void RefreshAll()
    {
        ClearAllIsolatedCacheByEntityType<IRelationType>();

        base.RefreshAll();
    }

    public override void Refresh(int id)
    {
        ClearCache(id.Yield());

        base.Refresh(id);
    }

    public override void Refresh(Guid id) => throw new NotSupportedException();

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
        Attempt<IAppPolicyCache?> appCache = AppCaches.IsolatedCaches.Get<IRelationType>();
        if (appCache.Success && appCache.Result is not null)
        {
            foreach (var id in ids)
            {
                appCache.Result.Clear(RepositoryCacheKeys.GetKey<IRelationType, int>(id));
            }
        }
    }
}
