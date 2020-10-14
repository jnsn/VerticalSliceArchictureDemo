using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NSwag;
using NSwag.AspNetCore;
using Scrutor;
using VerticalSliceArchictureDemo.Web.Common.Extensions;
using VerticalSliceArchictureDemo.Web.Common.MediatR;
using VerticalSliceArchictureDemo.Web.Common.Middleware;
using VerticalSliceArchictureDemo.Web.Persistence;

namespace VerticalSliceArchictureDemo.Web
{
    public class Startup
    {
        public const string ApplicationName = "VerticalSliceArchitectureDemo";

        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var assemblies = new List<Assembly> {typeof(Startup).Assembly};
            var assembliesStartingWithName = new List<string> {"VerticalSliceArchictureDemo"};

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddControllers(options => options.ReturnHttpNotAcceptable = true)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddFeatureFolders();

            services.AddHttpContextAccessor();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext = actionContext as ActionExecutingContext;
                    if (actionExecutingContext?.ModelState.ErrorCount > 0
                        && actionExecutingContext.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            services.AddSingleton<IClock>(_ => SystemClock.Instance);

            services.AddAutoMapper(assemblies.ToArray());

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            services.AddMediatR(assemblies.ToArray());

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;

                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddOpenApiDocument(options =>
            {
                options.Title = "VerticalSliceArchitecture Demo API";
                options.Description = "VerticalSliceArchitecture Demo API";
                options.Version = "1.0";
                options.DocumentName = "v1.0";
                options.ApiGroupNames = new[] {"v1.0"};
                options.FlattenInheritanceHierarchy = true;
            });

            services.AddHealthChecks();

            var typesToExclude = new List<Type>
            {
                typeof(IRequestHandler<>),
                typeof(IRequestHandler<,>),
                typeof(INotificationHandler<>)
            };

            var dbConnectionString = Configuration.GetSection(ApplicationName).GetSection("ConnectionStrings")["Database"];
            services.AddDbContext<DemoDbContext>(options => options.UseSqlServer(dbConnectionString));

            bool AssemblyScanPredicate(Assembly assembly)
            {
                return assembliesStartingWithName.Any(name => assembly.FullName?.StartsWith(name, StringComparison.InvariantCultureIgnoreCase) ?? false);
            }

            bool ClassScanPredicate(Type type)
            {
                return assembliesStartingWithName.Any(name => type.AssemblyQualifiedName?.StartsWith(name) ?? false)
                       && !typesToExclude.Any(type.IsClosedTypeOf)
                       && type.GetInterfaces().Length > 0
                       && (!type.FullName?.Contains("VerticalSliceArchictureDemo.Web.Common.Http.Exceptions") ?? true);
            }

            services.Scan(scan =>
                scan.FromExecutingAssembly()
                    .AddClasses(classes => classes.Where(ClassScanPredicate), true)
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                    .FromApplicationDependencies(AssemblyScanPredicate)
                    .AddClasses(classes => classes.Where(ClassScanPredicate), true)
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            );

            services.AddHealthChecks()
                .AddSqlServer(dbConnectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<HttpStatusCodeExceptionMiddleware>();
            app.UseMiddleware<HttpRequestHeaderLoggingMiddleware>();

            app.UseOpenApi(settings => settings.PostProcess = (document, _) => document.Schemes = new[] {OpenApiSchema.Https});
            app.UseSwaggerUi3(settings =>
            {
                settings.ValidateSpecification = false;
                settings.DocumentTitle = "VerticalSliceArchitectureDemo API";

                var swaggerUi3Routes = apiVersionDescriptionProvider.ApiVersionDescriptions
                    .Select(description => new SwaggerUi3Route(
                        description.GroupName.ToUpperInvariant(), $"/swagger/{description.GroupName}/swagger.json"))
                    .OrderByDescending(x => x.Name)
                    .ToList();

                foreach (var uiRoute in swaggerUi3Routes)
                {
                    settings.SwaggerRoutes.Add(uiRoute);
                }
            });

            app.UseHttpsRedirection();
            app.UseDefaultFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = WriteHealthCheckResponse
                });
            });
        }

        private static Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));

            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}
