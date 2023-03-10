﻿using Umbraco.Cms.Api.Management.Mapping.TemporaryFile;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Mapping;

namespace Umbraco.Cms.Api.Management.DependencyInjection;

internal static class TTemporaryFileBuilderExtensions
{
    internal static IUmbracoBuilder AddTemporaryFiles(this IUmbracoBuilder builder)
    {
        builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
            .Add<TemporaryFileViewModelsMapDefinition>();

        return builder;
    }
}
