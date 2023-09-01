using System;
using System.Net;
using System.Net.Cache;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public class AdeWebClient : WebClient
{
    /// <summary>
    /// Response Uri after any redirects.
    /// </summary>
    public Uri? ResponseUri { get; private set; }

    public AdeWebClient()
    {
        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
        Headers[HttpRequestHeader.UserAgent] = string.IsNullOrEmpty(Properties.Settings.Default.WebClient_UserAgent)
            ? Properties.DefaultSettings.Default.WebClient_UserAgent
            : Properties.Settings.Default.WebClient_UserAgent;
    }

    /// <inheritdoc />
    protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
    {
        var webResponse = base.GetWebResponse(request, result);
        ResponseUri = webResponse.ResponseUri;
        return webResponse;
    }
}
