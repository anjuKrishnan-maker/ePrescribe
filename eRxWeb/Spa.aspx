<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Spa.aspx.cs" Inherits="eRxWeb.Spa" %>

<%@ Import Namespace="Allscripts.Impact" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <meta http-equiv="x-ua-compatible" content="IE=edge" />
    <title>Veradigm ePrescribe</title>
    <link href="<%=ResolveUrl("~/Style/AllscriptsStyle.css?version="+Version)%>" rel="stylesheet" />
    <link href="Style/bootstrap.min.css" rel="stylesheet" />
    <link href="Style/jquery.jqtimeline.css" rel="stylesheet" />
    <link href="Style/angular-material-need-only.css" rel="stylesheet" />
    <link href="<%=ResolveUrl("~/Style/Default.css?version="+Version)%>" rel="stylesheet" />
    <link id="lnkStyleSheet" runat="server" rel="stylesheet" type="text/css" />
    <link id="PageIcon" rel="SHORTCUT ICON" runat="server" href="images/favicon.ico" />
    <script src="<%=ResolveUrl("jquery/js/jquery.min.js")%>"></script>
    <script src="<%=ResolveUrl("js/bootstrap.min.js")%>"></script>
    <!--this js gets all function to comunicate between content and angular-->
    <script src="<%=ResolveUrl("~/SPA/Script/spa.bridge.js?version="+Version)%>"></script>
    <base href="/">
</head>
<body>

    <erx-app>
        <div id="divLoading">
        </div>
        <div>
            <div>
                 <img id="mainLogo" src="images/shim.gif" width="120" height="65" alt="Allscripts" />
                 <img src="images/powered_by.gif" id="poweredByImage" class="sponsored" alt="Allscripts" />
            </div>
            <div class="divider">

            </div>
        </div>       
    </erx-app>
    <script type="text/javascript">
        var redirectPage = '<%= RedirectPage %>'

        window.version = '<%=Version%>';
        if (document['documentMode'] || /Edge/.test(navigator.userAgent)) {
            window.__Zone_enable_cross_context_check = true;
        }
    </script>

    <%Response.WriteFile("~/spa/dist/ChunkBundleReference.html");%>

    <%
        if (!IsDevelopmentMode)
        {%>

    <script type="text/javascript">
        var appInsights = window.appInsights || function (a) {
            function b(a) { c[a] = function () { var b = arguments; c.queue.push(function () { c[a].apply(c, b) }) } } var c = { config: a }, d = document, e = window; setTimeout(function () { var b = d.createElement("script"); b.src = a.url || "https://az416426.vo.msecnd.net/scripts/a/ai.0.js", d.getElementsByTagName("script")[0].parentNode.appendChild(b) }); try { c.cookie = d.cookie } catch (a) { } c.queue = []; for (var f = ["Event", "Exception", "Metric", "PageView", "Trace", "Dependency"]; f.length;)b("track" + f.pop()); if (b("setAuthenticatedUserContext"), b("clearAuthenticatedUserContext"), b("startTrackEvent"), b("stopTrackEvent"), b("startTrackPage"), b("stopTrackPage"), b("flush"), !a.disableExceptionTracking) { f = "onerror", b("_" + f); var g = e[f]; e[f] = function (a, b, d, e, h) { var i = g && g(a, b, d, e, h); return !0 !== i && c["_" + f](a, b, d, e, h), i } } return c
        }({
            instrumentationKey: "<%=ConfigKeys.AppInsightsKey%>"
        });

        window.appInsights = appInsights, appInsights.queue && 0 === appInsights.queue.length && appInsights.trackPageView();
    </script>
    <%if (IsGaEnabled)
        { %>
    <script type="text/javascript">
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
                m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', 'https://www.google-analytics.com/analytics.js', 'ga');

        ga('create', '<%=GaAccountId%>', 'auto');
    </script>
    <%} %>


    <% } %>
</body>
</html>
