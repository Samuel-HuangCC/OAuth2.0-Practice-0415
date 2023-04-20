using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OAuth2._0_Practice_0415.Model;
using System.Web;

namespace OAuth2._0_Practice_0415.Pages
{
    [Authorize]
    public class LineNotifyModel : PageModel
    {
        public OUser UserInfo { get; set; }
        public string NotifyUrl { get; set; }
        public void OnGet()
        {
            var id = User.Claims.FirstOrDefault(c => c.Type == "id").Value;
            UserInfo = DbModel.GetUser(id);

            if (string.IsNullOrEmpty(UserInfo.ReferenceId))
            {
                NotifyUrl = "https://notify-bot.line.me/oauth/authorize" +
                    "?response_type=code" +
                    "&client_id=" + Config.NotifyClientID +
                    "&state=" + UserInfo.Id +
                    "&scope=notify" +
                    "&redirect_uri=" + HttpUtility.UrlEncode("https://" + Request.Host + "/authcallback?handler=Notify");
            }
        }

        public void OnPostSendNotify(string message)
        {
            OnGet();
            SendNotify(message, UserInfo.ReferenceId).Wait();
		}
		public void OnPostSendAll(string message)
		{
			OnGet();
            var users = DbModel.GetAllUser();
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.ReferenceId))
                {
                    SendNotify(message, user.ReferenceId).Wait();
                }
            }
		}

		public async Task<IActionResult> OnPostUnregister()
        {
			OnGet();
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {UserInfo.ReferenceId}");
            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();
            var response = await client.PostAsync("https://notify-api.line.me/api/revoke", new FormUrlEncodedContent(nameValueCollection));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UserInfo.ReferenceId = string.Empty;
                DbModel.UpdateUser(UserInfo);
                Console.WriteLine("Unregister!!");
            }
            return Page();
		}

        private async Task SendNotify(string message,string access_token)
        {
			var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");
			IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>> {
					{ new KeyValuePair<string, string>("message", message) },
                };
			var response = await client.PostAsync("https://notify-api.line.me/api/notify", new FormUrlEncodedContent(nameValueCollection));
		}

	}
}
