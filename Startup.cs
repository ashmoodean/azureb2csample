using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ashmo.Swagger;
using Ashmo.Configuration;
using Ashmo.ApiVersioning;
using Ashmo.Configuration.Extensions;
using CorrelationId;
using Ashmo.Logging.Adapters;
using Ashmo.Logging.Utilities;
using FluentValidation.AspNetCore;
using Ashmo.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using HealthCare.API.AzureB2C;
using System;
using Microsoft.AspNetCore.Http;
using HealthCare.Application.Configuration;
using HealthCare.Application.Interface;
using HealthCare.Repository;
using HealthCare.Domain.Data;
using HealthCare.Interface;
using HealthCare.Survey.Manager;

namespace HealthCare.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			//services.AddAuthentication(sharedOptions =>
			//{
			//	sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			//	sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			//})
			//.AddAzureAdB2C(options => Configuration.Bind("Authentication:AzureAdB2C", options))
			//.AddCookie();

			services.AddMvc()
				.AddFluentValidation(fvc =>
				{
					fvc.RegisterValidatorsFromAssemblyContaining<Startup>();
				});

			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromHours(1);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			services.AddCorrelationId();

			services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
			services.AddSingleton(typeof(IConnectionFactory<>), typeof(ConnectionFactory<>));

			services.AddVersioning();

			services.AddTypedConfiguration<ServiceConfiguration>(Configuration);
			services.AddTypedConfiguration<BaseConfiguration>(Configuration);
			services.AddTypedConfiguration<AzureAdB2C>(Configuration);
			services.AddTypedConfigurationValidation();

			services.AddTransient<ISurveyRepository, SurveyRepository>();
			services.AddTransient<ISurveyManager, SurveyManager>();

			services.AddSwagger();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			LoggerUtilities.ConfigureLogger(env, loggerFactory, Configuration);

			app.ConfigureSwagger();

			app.UseHttpsRedirection();

			app.UseRouting();

			//app.UseAuthentication();
			//app.UseAuthorization();

			app.UseSecurityMiddlewares();

			app.UseCorsMiddleware();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
