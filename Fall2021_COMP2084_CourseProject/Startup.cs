using Fall2021_COMP2084_CourseProject.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fall2021_COMP2084_CourseProject
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false) //Email confirmation:OFF
                .AddRoles<IdentityRole>()  //Enable to Role Management for Authorization to track the account role, which is OFF by default
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            //Enable the external authentications to sign in
            services.AddAuthentication()

                //Google Authentication
                .AddGoogle( options =>
                {
                    //Access the Google Authentication section of appsettings.json
                    IConfigurationSection googleAuthSection = Configuration.GetSection("Authentication:Google");

                    //Read the Google API Key values from configure section and set as options
                    options.ClientId = googleAuthSection["ClientId"];
                    options.ClientSecret = googleAuthSection["ClientSecret"];
                })

                //Facebook Authentication
                .AddFacebook(options =>
                {
                    //Access the Facebook Authentication section of appsettings.json
                    IConfigurationSection facebookAuthSection = Configuration.GetSection("Authentication:Facebook");

                    //Read the Facebook API Key values from configure section and set as options
                    options.AppId = facebookAuthSection["AppId"];
                    options.AppSecret = facebookAuthSection["AppSecret"];
                });

            //Enable session support
            services.AddSession();

            //Enable Swagger for API document file
            services.AddSwaggerGen();//Swagger for API document
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Session support - Don't put at the bottom if you want it work!
            //Retrieve the session data
            app.UseSession();
           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            //When app starts, use Swagger to inspect any API controllers and generate a new documentation file
            //Any time your API is changed, the Swagger document will get regenerated to be always up to date.
            //And, normally, you'd also add the authentication to the API documentation
            //so your data through the Swagger can be protected
            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint(
                    "/swagger/v1/swagger.json", //The endpoint of the URI
                    "Search In Room API"//The title of the API documentation file
                );
            });

        }
    }
}
