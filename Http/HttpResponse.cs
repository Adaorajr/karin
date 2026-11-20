namespace Karin.Http;

public sealed record HttpResponse(
    string StatusLine,
    string Body,
    string? ContentType = "text/plain; charset=utf-8");
