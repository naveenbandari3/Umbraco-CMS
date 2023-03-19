using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Serialization;

namespace Umbraco.Cms.Core.Cache;

public sealed class DictionaryCacheRefresher : PayloadCacheRefresherBase<DictionaryCacheRefresherNotification, DictionaryCacheRefresher.JsonPayload>
{
    public static readonly Guid UniqueId = Guid.Parse("D1D7E227-F817-4816-BFE9-6C39B6152884");

    [Obsolete("Use constructor that takes all parameters instead")]
    public DictionaryCacheRefresher(AppCaches appCaches, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory factory)
       : this(appCaches, StaticServiceProvider.Instance.GetRequiredService<IJsonSerializer>(), eventAggregator, factory)
    {
    }

    public DictionaryCacheRefresher(AppCaches appCaches, IJsonSerializer jsonSerializer, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory factory)
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

    public override Guid RefresherUniqueId => UniqueId;

    public override string Name => "Dictionary Cache Refresher";

    public override void Refresh(int id)
    {
        ClearCache();

        base.Refresh(id);
    }

    public override void Remove(int id)
    {
        ClearCache();

        base.Remove(id);
    }

    public override void Refresh(JsonPayload[] payloads)
    {
        ClearCache();

        base.Refresh(payloads);
    }

    private void ClearCache()
        => ClearAllIsolatedCacheByEntityType<IDictionaryItem>();
}
