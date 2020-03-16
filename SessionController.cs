using Ashmo.ApiVersioning;
using HealthCare.API.AzureB2C;
using HealthCare.Application.Configuration;
using HealthCare.Application.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.API.Controllers
{
	[ApiController]
	[Route(VersionRoute.Url)]
	[ApiVersion("1")]
	[Produces("application/json")]
	public class SessionController : ControllerBase
	{
		private readonly string baseUrl;

		public SessionController(AzureAdB2C b2cOptions, BaseConfiguration baseConfiguration)
		{
			AzureAdB2COptions = b2cOptions;
			this.baseUrl = baseConfiguration.ClientURL;
		}

		public AzureAdB2C AzureAdB2COptions { get; set; }

		[HttpGet("signIn")]
		public IActionResult SignIn(string redirectUrl)
		{
			return Challenge(
					new AuthenticationProperties { RedirectUri = redirectUrl.GetClientUrl(baseUrl) },
					OpenIdConnectDefaults.AuthenticationScheme);
		}

		[HttpGet("resetPassword")]
		public IActionResult ResetPassword(string redirectUrl)
		{
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl.GetClientUrl(baseUrl) };
			properties.Items[AzureAdB2C.PolicyAuthenticationProperty] = AzureAdB2COptions.ResetPasswordPolicyId;
			return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
		}

		[HttpGet("editProfile")]
		public IActionResult EditProfile(string redirectUrl)
		{
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl.GetClientUrl(baseUrl) };
			properties.Items[AzureAdB2C.PolicyAuthenticationProperty] = AzureAdB2COptions.EditProfilePolicyId;
			return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
		}

		[HttpGet("signOut")]
		public IActionResult SignOut()
		{
			var callbackUrl = Url.Action(nameof(SignedOut), "Session", values: null, protocol: Request.Scheme);
			return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl },
					CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
		}

		[HttpGet("signedOut")]
		public IActionResult SignedOut()
		{
			if (User.Identity.IsAuthenticated)
			{
				// Redirect to home page if the user is authenticated.
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			return Ok("SignedOut");
		}
	}
}