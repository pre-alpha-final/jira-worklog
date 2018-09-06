using JiraWorklog.Core.Services;
using JiraWorklog.Core.Services.Implementation;
using JiraWorklog.Core.Stores;
using JiraWorklog.Data.AzureBlob.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JiraWorklog
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IJiraWorklogService, JiraWorklogService>();
			services.AddTransient<IReportStore>(
				e => new ReportStore(Configuration.GetConnectionString("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING")));

			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseStaticFiles();

			app.UseMvc();
		}
	}
}
