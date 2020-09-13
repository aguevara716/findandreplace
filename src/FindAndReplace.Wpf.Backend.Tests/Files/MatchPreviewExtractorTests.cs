using System.Collections.Generic;
using FindAndReplace.Wpf.Backend.Files;
using FindAndReplace.Wpf.Backend.Results;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class MatchPreviewExtractorTests
    {
        // Dependencies
        private IMatchPreviewExtractor _matchPreviewExtractor;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _matchPreviewExtractor = new MatchPreviewExtractor();
        }

        // Private Methods

        // MatchPreviewExtractionResult ExtractMatchPreviews(string filePath, string fileContent, IList<TextMatch> textMatches);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ExtractMatchPreviews_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            string filePath = isNull
                ? null
                : string.Empty;
            var fileContent = "asdf";
            var textMatches = new List<TextMatch>();

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeFalse();
            matchPreviewExtractionResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_ReturnFailureIfFileContentIsNull()
        {
            var filePath = "asdf";
            string fileContent = null;
            var textMatches = new List<TextMatch>();

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeFalse();
            matchPreviewExtractionResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_ReturnFailureIfTextMatchesIsNull()
        {
            var filePath = "asdf";
            var fileContent = "asdf";
            List<TextMatch> textMatches = null;

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeFalse();
            matchPreviewExtractionResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_ReturnSuccessWithEmptyPayloadIfFileContentIsEmpty()
        {
            var filePath = "asdf";
            var fileContent = string.Empty;
            List<TextMatch> textMatches = new List<TextMatch>() { new TextMatch { Length = 1, StartIndex = 1 } };

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeTrue();
            matchPreviewExtractionResult.Previews.Should().BeEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_ReturnSuccessWithEmptyPayloadIfTextMatchCollectionIsEmpty()
        {
            var filePath = "asdf";
            var fileContent = "asdf";
            var textMatches = new List<TextMatch>();

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeTrue();
            matchPreviewExtractionResult.Previews.Should().BeEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_Thing()
        {
            // Data is from a real execution of fnr.exe
            var filePath = "asdf";
            var fileContent = "using EnergyPi.Web.Builders;\r\nusing EnergyPi.Web.Data;\nusing EnergyPi.Web.DataServices;\nusing EnergyPi.Web.Entities;\nusing EnergyPi.Web.Repositories;\nusing Microsoft.AspNetCore.Builder;\nusing Microsoft.AspNetCore.Hosting;\nusing Microsoft.AspNetCore.Identity;\nusing Microsoft.EntityFrameworkCore;\nusing Microsoft.Extensions.Configuration;\nusing Microsoft.Extensions.DependencyInjection;\nusing Microsoft.Extensions.Hosting;\n\nnamespace EnergyPi.Web\n{\n    public class Startup\n    {\n        public Startup(IConfiguration configuration)\n        {\n            Configuration = configuration;\n        }\n\n        public IConfiguration Configuration { get; }\n\n        // This method gets called by the runtime. Use this method to add services to the container.\n        public void ConfigureServices(IServiceCollection services)\n        {\n            services.AddDbContext<AuthenticationDbContext>(options => options.UseMySql(Configuration.GetConnectionString(\"AuthenticationConnection\")));\n            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AuthenticationDbContext>();\n\n            services.AddDbContext<DataDbContext>(options => options.UseMySql(Configuration.GetConnectionString(\"DataConnection\")));\n\n            services = AddCustomServices(services);\n\n            services.AddControllersWithViews().AddRazorRuntimeCompilation();\n            services.AddRazorPages();\n        }\n\n        private IServiceCollection AddCustomServices(IServiceCollection services)\n        {\n            // repositories\n            services.AddTransient<IRepository<EnergyLogs>, DataRepository<EnergyLogs>>();\n            services.AddTransient<IRepository<WeatherLogs>, DataRepository<WeatherLogs>>();\n\n            // data services\n            services.AddTransient<IEnergyLogsDataService, EnergyLogsDataService>();\n            services.AddTransient<IWeatherLogsDataService, WeatherLogsDataService>();\n\n            // builders\n            services.AddTransient<ITodayViewModelBuilder, TodayViewModelBuilder>();\n            services.AddTransient<IHistoryViewModelBuilder, HistoryViewModelBuilder>();\n\n            return services;\n        }\n\n        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.\n        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)\n        {\n            if (env.IsDevelopment())\n            {\n                app.UseDeveloperExceptionPage();\n                app.UseDatabaseErrorPage();\n            }\n            else\n            {\n                app.UseExceptionHandler(\"/Home/Error\");\n                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.\n                app.UseHsts();\n            }\n            app.UseHttpsRedirection();\n            app.UseStaticFiles();\n\n            app.UseRouting();\n\n            app.UseAuthentication();\n            app.UseAuthorization();\n\n            app.UseEndpoints(endpoints =>\n            {\n                endpoints.MapControllerRoute\n                (\n                    name: \"default\",\n                    pattern: \"{controller=Home}/{action=Index}/{id?}\"\n                );\n                endpoints.MapRazorPages();\n            });\n        }\n\n    }\n}\n";
            var textMatches = new List<TextMatch>
            {
                new TextMatch
                {
                    Length = 5,
                    StartIndex = 473
                }
            };
            var expectedPreviewText = "14 namespace EnergyPi.Web\r\n15 {\r\n16     public class Startup\r\n17     {\r\n18         public Startup(IConfiguration configuration)\r\n";
            
            // TODO test the actual logic
            Assert.Fail();
        }

    }
}
