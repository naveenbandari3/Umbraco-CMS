using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Repositories;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Cms.Core.Cache;

public sealed class TemplateCacheRefresher : PayloadCacheRefresherBase<TemplateCacheRefresherNotification, TemplateCacheRefresher.JsonPayload>
{
    public static readonly Guid UniqueId = Guid.Parse("DD12B6A0-14B9-46e8-8800-C154F74047C8");

    private readonly IContentTypeCommonRepository _contentTypeCommonRepository;
    private readonly IIdKeyMap _idKeyMap;

    [Obsolete("Use constructor that takes all parameters instead")]
    public TemplateCacheRefresher(AppCaches appCaches, IIdKeyMap idKeyMap, IContentTypeCommonRepository contentTypeCommonRepository, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory factory)
        : this(appCaches, StaticServiceProvider.Instance.GetRequiredService<IJsonSerializer>(), eventAggregator, factory, idKeyMap, contentTypeCommonRepository)
    {
    }

    public TemplateCacheRefresher(AppCaches appCaches, IJsonSerializer jsonSerializer, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory factory, IIdKeyMap idKeyMap, IContentTypeCommonRepository contentTypeCommonRepository)
        : base(appCaches, jsonSerializer, eventAggregator, factory)
    {
        _idKeyMap = idKeyMap;
        _contentTypeCommonRepository = contentTypeCommonRepository;
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

    public override string Name => "Template Cache Refresher";

    public override void Refresh(int id)
    {
        ClearCache(id.Yield());

        base.Refresh(id);
    }

    public override void Remove(int id)
    {
        ClearCache(id.Yield(), true);

        base.Remove(id);
    }

    public override void Refresh(JsonPayload[] payloads)
    {
        ClearCache(payloads.Select(x => x.Id), payloads.Any(x => x.Removed));

        base.Refresh(payloads);
    }

    private void ClearCache(IEnumerable<int> ids, bool removed = false)
    {
        foreach (var id in ids)
        {
            _idKeyMap.ClearCache(id);
            AppCaches.RuntimeCache.Clear($"{CacheKeys.TemplateFrontEndCacheKey}{id}");
        }

        // need to clear the runtime cache for templates
        ClearAllIsolatedCacheByEntityType<ITemplate>();

        if (removed)
        {
            // During removal we need to clear the runtime cache for templates, content and content type instances!!!
            // all three of these types are referenced by templates, and the cache needs to be cleared on every server,
            // otherwise things like looking up content type's after a template is removed is still going to show that
            // it has an associated template.
            ClearAllIsolatedCacheByEntityType<IContent>();
            ClearAllIsolatedCacheByEntityType<IContentType>();
            _contentTypeCommonRepository.ClearCache();
        }
    }
}
