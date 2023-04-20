using System.Net;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using OAuth2._0_Practice_0415.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace OAuth2._0_Practice_0415.Pages
{
    public class AuthCallBackModel : PageModel
    {
        public async Task<IActionResult> OnGet(string code,string state) 
        {
            if (!string.IsNullOrEmpty(code))
            {
                var client = new HttpClient();
                IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>> {
                    { new KeyValuePair<string, string>("code", code) },
                    { new KeyValuePair<string, string>("client_id", Config.ClientID) },
                    { new KeyValuePair<string, string>("client_secret", Config.SecretID) },
                    { new KeyValuePair<string, string>("redirect_uri", "https://"+ Request.Host +"/authcallback") },
                    { new KeyValuePair<string, string>("grant_type", "authorization_code") },
                    { new KeyValuePair<string, string>("id_token_key_type", "JWK") },
                };

                var response = await client.PostAsync("https://api.line.me/oauth2/v2.1/token", new FormUrlEncodedContent(nameValueCollection));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                    var obj = JsonConvert.DeserializeObject<JObject>(result);
                    if (obj != null)
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var token = handler.ReadJwtToken(obj["id_token"]?.ToString());
                        var id = token.Claims.FirstOrDefault(c => c.Type.Equals("sub"))?.Value;
                        var user = DbModel.GetUser(id);
                        if (user != null)
                        {
                            user.AccessToken = obj["access_token"].ToString();
                            user.RefreshToken = obj["refresh_token"].ToString();
                            user.IdToken = obj["id_token"].ToString();
                            DbModel.UpdateUser(user);
						}
                        else
                        {
							user = new OUser()
							{
								Id = id,
								Name = token.Claims.FirstOrDefault(c => c.Type.Equals("name"))?.Value,
								Email = token.Claims.FirstOrDefault(c => c.Type.Equals("email"))?.Value,
								AccessToken = obj["access_token"].ToString(),
								RefreshToken = obj["refresh_token"].ToString(),
								IdToken = obj["id_token"].ToString()
							};
							DbModel.InsertUser(user);
						}
                        if (user != null)
                        {
                            var claims = new List<Claim>
                            {
                                new Claim("id",user.Id)
                            };

							var claimsIdentity = new ClaimsIdentity(
								claims, CookieAuthenticationDefaults.AuthenticationScheme);

							await HttpContext.SignInAsync(
								CookieAuthenticationDefaults.AuthenticationScheme,
								new ClaimsPrincipal(claimsIdentity));
                            return RedirectToPage("LineNotify");
                        }
					}
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnGetNotify(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var client = new HttpClient();
                IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>> {
                    { new KeyValuePair<string, string>("code", code) },
                    { new KeyValuePair<string, string>("client_id", Config.NotifyClientID) },
                    { new KeyValuePair<string, string>("client_secret", Config.NotifySecretID) },
                    { new KeyValuePair<string, string>("redirect_uri", "https://"+ Request.Host +"/authcallback?handler=Notify") },
                    { new KeyValuePair<string, string>("grant_type", "authorization_code") },
                    //{ new KeyValuePair<string, string>("id_token_key_type", "JWK") },
                };
				var response = await client.PostAsync("https://notify-bot.line.me/oauth/token", new FormUrlEncodedContent(nameValueCollection));
				if (response.StatusCode == HttpStatusCode.OK)
                {
					var result = await response.Content.ReadAsStringAsync();
					Console.WriteLine(result);
					var obj = JsonConvert.DeserializeObject<JObject>(result);
					if (obj != null)
					{
						var loginuser = DbModel.GetUser(state);
                        if (loginuser != null)
                        {
                            loginuser.ReferenceId = obj["access_token"].ToString();
                            DbModel.UpdateUser(loginuser);
                            return RedirectToPage("LineNotify");
                        }
					}
				}
            }
			return Page();
		}

	}
}
