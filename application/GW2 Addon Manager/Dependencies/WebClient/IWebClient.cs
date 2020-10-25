using System.Net;

namespace GW2_Addon_Manager.Dependencies.WebClient
{
    public interface IWebClient
    {
        WebHeaderCollection Headers { get; set; }
        string DownloadString(string address);
    }
}