namespace Sitecore.Support.XA.Foundation.Presentation.Pipelines.RenderRendering
{
  using Sitecore.Caching;
  using Sitecore.Mvc.Extensions;
  using Sitecore.Mvc.Pipelines.Response.RenderRendering;
  using Sitecore.Mvc.Presentation;
  using Sitecore.Sites;
  using Sitecore.XA.Foundation.Abstractions;
  using Sitecore.XA.Foundation.Multisite.Extensions;
  using Sitecore.XA.Foundation.Presentation.Services;
  using System;
  using System.IO;

  public class RenderFromCacheExclusively : RenderFromCache
  {
    private readonly ICacheTokenProcessingService _cacheTokenProcessingService;

    protected IContext Context
    {
      get;
    }

    public RenderFromCacheExclusively(ICacheTokenProcessingService caheCacheTokenProcessingService, IContext context)
    {
      _cacheTokenProcessingService = caheCacheTokenProcessingService;
      Context = context;
    }

    public override void Process(RenderRenderingArgs args)
    {
      if (args.CacheKey == null)
      {
        args.CacheKey = args.CacheKey + "_#id:" + args.Rendering.UniqueId.ToString();
      }
      base.Process(args);
    }

    protected override bool Render(string cacheKey, TextWriter writer, RenderRenderingArgs args)
    {
      if (!Context.Site.IsSxaSite())
      {
        return base.Render(cacheKey, writer, args);
      }
      HtmlCache htmlCache = Context.Site.ValueOrDefault((SiteContext site) => CacheManager.GetHtmlCache(site));
      if (htmlCache == null)
      {
        return false;
      }
      string html = htmlCache.GetHtml(cacheKey);
      if (html == null)
      {
        return false;
      }
      IDisposable item = RenderingContext.EnterContext(args.Rendering);
      args.Disposables.Add(item);
      string html2 = ProcessPlaceholders(html, args);
      html2 = ProcessRenderings(html2, args);
      writer.Write(html2);
      return true;
    }

    protected virtual string ProcessPlaceholders(string html, RenderRenderingArgs args)
    {
      return _cacheTokenProcessingService.ProcessPlaceholderToken(html, args);
    }

    protected virtual string ProcessRenderings(string html, RenderRenderingArgs args)
    {
      return _cacheTokenProcessingService.ProcessRenderingToken(html, args);
    }
  }
}