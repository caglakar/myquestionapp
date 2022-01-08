using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using QandA.Data;
using QandA.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace QandA
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
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            EnsureDatabase.For.SqlDatabase(connectionString);
            var upgrader = DeployChanges.To.SqlDatabase(connectionString, null).WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly()).WithTransaction().Build();
            if(upgrader.IsUpgradeRequired())
            { 
                upgrader.PerformUpgrade(); 
            }
            services.AddMemoryCache(p=> p.SizeLimit=100);
            services.AddSingleton<IQuestionCache, QuestionCache>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QandA", Version = "v1" });
            });

		    services.AddScoped<IDataRepository,DataRepository>();

            #region Auth0-JWT configurations
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.Authority = Configuration["Auth0:Authority"];
                opt.Audience = Configuration["Auth0:Audience"];
            });
            #endregion
            services.AddHttpClient();
            services.AddAuthorization(opt => opt.AddPolicy("MustBeQuestionAuthor", p => p.Requirements.Add(new MustBeQuestionAuthorRequirement())));
            services.AddScoped<IAuthorizationHandler, MustBeQuestionAuthorHandler>();
            services.AddHttpContextAccessor();

            services.AddCors(options => 
            options.AddPolicy("CorsPolicy", 
                builder =>
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(Configuration["Frontend"])));
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QandA v1"));
            }
            

            //geliştime yaparken usehttpsredirection'ı kapattık çünkü frontend http protokolünü kullanıyor.
            //firefox farklı protokoldan gelen isteklere izin vermiyormuş,
            //biz geliştirme yaparken hem frontendin hem de backendin http kullanması için usehttpsredirectionı kapattık.
           // app.UseHttpsRedirection();

            app.UseRouting();
            //bu middleware routing ve authentication arasına gelmeli
            app.UseCors("CorsPolicy");
            //bu middleware routing ve authorization arasına gelmeli
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
