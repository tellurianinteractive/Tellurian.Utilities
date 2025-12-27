using System.Net;

namespace Tellurian.Utilities.Web;

public static class HttpExtensions
{
    extension(HttpStatusCode code)
    {
        /// <summary>
        /// Gets a value indicating whether the response represents a successful status code.
        /// </summary>
        public bool IsSuccess => (int)code >= 200 && (int)code < 300;
    }
}
