using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace CentralServer.ApiServer;

public class XmlResult : IResult
{
    private readonly XDocument _result;

    public XmlResult(XDocument result)
    {
        _result = result;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        using var ms = new MemoryStream();

        // Serialize the object synchronously then rewind the stream
        _result.Save(ms);
        ms.Position = 0;

        httpContext.Response.ContentType = "application/xml";
        await ms.CopyToAsync(httpContext.Response.Body);
    }
}