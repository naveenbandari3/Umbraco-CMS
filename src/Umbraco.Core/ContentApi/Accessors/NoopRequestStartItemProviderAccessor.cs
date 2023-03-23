﻿using System.Diagnostics.CodeAnalysis;

namespace Umbraco.Cms.Core.ContentApi.Accessors;

public sealed class NoopRequestStartItemProviderAccessor : IRequestStartItemProviderAccessor
{
    public bool TryGetValue([NotNullWhen(true)] out IRequestStartItemProvider? requestStartItemProvider)
    {
        requestStartItemProvider = new NoopRequestStartItemProvider();
        return true;
    }
}
