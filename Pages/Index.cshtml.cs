using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OAuth2._0_Practice_0415.Model;

namespace OAuth2._0_Practice_0415.Pages
{
    
    public class IndexModel : PageModel
    {
        public string? loginurl { get; set; }

        public void OnGet()
        {
            loginurl = "https://access.line.me/oauth2/v2.1/authorize" +
                "?response_type=code" +
                "&client_id=" + Config.ClientID +
                "&state=999999" +
                "&scope=" + HttpUtility.UrlEncode("openid email profile") +
                "&redirect_uri=" + HttpUtility.UrlEncode("https://" + Request.Host + "/authcallback");
        }
    }
}