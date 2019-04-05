namespace Sitecore.Support.XA.Foundation.Presentation.Pipelines.RenderRendering
{
  using Sitecore.Mvc.Pipelines.Response.RenderRendering;
  using Sitecore.XA.Foundation.Abstractions;
  using Sitecore.XA.Foundation.Presentation.Services;

  public class RenderFromCacheExclusively : Sitecore.XA.Foundation.Presentation.Pipelines.RenderRendering.RenderFromCacheExclusively
  {
    public RenderFromCacheExclusively(ICacheTokenProcessingService caheCacheTokenProcessingService, IContext context) : base(caheCacheTokenProcessingService, context)
    {
    }

    public override void Process(RenderRenderingArgs args)
    {
      args.CacheKey = args.CacheKey + "_#id:" + args.Rendering.UniqueId.ToString();
      base.Process(args);
    }
  }
}