using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Roman.Echo
{
    public class Startup
    {


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Run(async (context) => {

                var nextUrls = Configuration["next_url"]?.Split('|');
                bool? successfullCall = null;
                try
                {
                    Parallel.ForEach(nextUrls, nextUrl => {
                        if (!string.IsNullOrWhiteSpace(nextUrl))
                        {
                            var str = new HttpClient().GetAsync(nextUrl).GetAwaiter().GetResult();
                            successfullCall = true;
                        }
                    });

                }
                catch (Exception)
                {
                    successfullCall = false;
                }

                var response = new
                {
                    requestHeaders = context.Request.Headers.ToDictionary(e => e.Key, v => v.Value.ToArray()),
                    env = Environment.GetEnvironmentVariables(),
                    calledChild = nextUrls != null && nextUrls.Length > 0,
                    successfullCall = successfullCall,
                };

                var responseString = JsonConvert.SerializeObject(response, Formatting.Indented);
                context.Response.Headers.Add("Content-Type", "application/json");
                await context.Response.WriteAsync(responseString);

            });

            app.UseMvc();
        }
    }

   
}
