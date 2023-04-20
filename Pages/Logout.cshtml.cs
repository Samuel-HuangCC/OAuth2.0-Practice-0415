using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OAuth2._0_Practice_0415.Model;

namespace OAuth2._0_Practice_0415.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = User.Claims.FirstOrDefault(c => c.Type == "id").Value;
                var user = DbModel.GetUser(id);
                user.AccessToken = string.Empty;
				user.RefreshToken = string.Empty;
                user.IdToken = string.Empty;
                DbModel.UpdateUser(user);
                HttpContext.SignOutAsync().Wait();
			}
            
            return RedirectToPage("Index");
		}
    }
}
