using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace MorePracticeMalodyServer.Middleware.Verification;

/// <summary>
///     Use to check Api version and user id validation.
/// </summary>
public class VerificationMiddleware
{
    private static readonly RSACryptoServiceProvider _rsa;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VerificationMiddleware> _logger;
    private readonly RequestDelegate _next;

    static VerificationMiddleware()
    {
        var publicKey =
            "<RSAKeyValue><Modulus>sgQb7aIukw8OqyqveicRQe75C11EA0QLpMGXtS0QCbVaid1zICJeyIhiYBmCm05ygs" +
            "Fkfoh+qahey/8NtU51NvJByBGe3CpgSTiaH9uhAdsLI4LttVqhUYQDJpI0NbRZ4FpTMAd9rcPwV7p4N3K8oHaKaF" +
            "Lbffyd1i9Pl001RXk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        _rsa = new RSACryptoServiceProvider();
        _rsa.FromXmlString(publicKey);
    }


    public VerificationMiddleware(RequestDelegate next, IConfiguration configuration,
        ILogger<VerificationMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var query = context.Request.Query;

        // Check api version.
        var version = 0;
        if (query["api"] != StringValues.Empty)
        {
            version = int.Parse(query["api"]);

            if (version > Consts.API_VERSION || version < Consts.MIN_SUPPORT)
            {
                _logger.LogError($"Server now doesn't support api version {version}");

                context.Response.StatusCode = 418; // JUST A JOKE.
                return; // Short-circuiting the pipeline.
            }
        }


        if (_configuration["CheckUid"]?.ToLower() == "true" && version >= 202112)
        {
            if (query["key"] == StringValues.Empty || query["uid"] == StringValues.Empty)
            {
                // Why you don't have a key or uid?
                context.Response.StatusCode = 403;
                return; // Short-circuiting the pipeline.
            }

            // Verify user id and key.
            if (!VerifyKey(query["uid"], query["key"]))
            {
                context.Response.StatusCode = 403;
                return; // Short-circuiting the pipeline.
            }
        }


        await _next(context);
    }

    private bool VerifyKey(string uid, string key)
    {
        return _rsa.VerifyData(Encoding.ASCII.GetBytes(uid), "SHA256", Base64UrlSafeDecode(key));
    }

    private byte[] Base64UrlSafeDecode(string raw)
    {
        raw = raw.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(raw);
    }
}

public static class VerificationMiddlewareExtensions
{
    /// <summary>
    ///     Check Api version and user id validation.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseVerification(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<VerificationMiddleware>();
    }
}