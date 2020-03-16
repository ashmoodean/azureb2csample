namespace HealthCare.API.AzureB2C
{
	public class AzureAdB2C
  {
    public const string PolicyAuthenticationProperty = "Policy";

    public AzureAdB2C() => AzureAdB2CInstance = "https://erashc.b2clogin.com/tfp/";

    public string ClientId { get; set; }
    public string AzureAdB2CInstance { get; set; }
    public string Tenant { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string SignInPolicyId { get; set; }
    public string SignUpPolicyId { get; set; }
    public string ResetPasswordPolicyId { get; set; }
    public string EditProfilePolicyId { get; set; }
    public string RedirectUri { get; set; }

    public string DefaultPolicy => SignUpSignInPolicyId;
    public string Authority => $"{AzureAdB2CInstance}/{Tenant}/{DefaultPolicy}/v2.0";
  }
}
