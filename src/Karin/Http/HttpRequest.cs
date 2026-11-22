namespace Karin.Http;

public sealed record HttpRequest(
    string Method,
    string Path,
    string QueryString,
    string Version,
    string Body);
