using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Processos;
using Awfq.Processos.App.Aplicacao.Responsaveis;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Driver;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes;
using Awfq.Comuns;
using Awfq.Processos.App.Utils;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Portas.Adaptadores.Notificacao.Smtp;

namespace Awfq.Processos.Api
{
    public class Startup
    {
        private Container container = new SimpleInjector.Container();

        public Startup(IConfiguration configuration)
        {
            // Set to false. This will be the default in v5.x and going forward.
            container.Options.ResolveUnregisteredConcreteTypes = false;

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApiVersioning(cfg =>
            {
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.ReportApiVersions = true;
            });
            // Sets up the basic configuration that for integrating Simple Injector with
            // ASP.NET Core by setting the DefaultScopedLifestyle, and setting up auto
            // cross wiring.
            services.AddSimpleInjector(container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope and
                // allows request-scoped framework services to be resolved.
                options.AddAspNetCore()

                        // Ensure activation of a specific framework type to be created by
                        // Simple Injector instead of the built-in configuration system.
                        // All calls are optional. You can enable what you need. For instance,
                        // ViewComponents, PageModels, and TagHelpers are not needed when you
                        // build a Web API.
                        .AddControllerActivation();

                // Optionally, allow application components to depend on the non-generic
                // ILogger (Microsoft.Extensions.Logging) or IStringLocalizer
                // (Microsoft.Extensions.Localization) abstractions.
                options.AddLogging();
                // options.AddLocalization();
            });

            InitializeContainer();

            // MongoDb Global Settings
            // BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }

        private void InitializeContainer()
        {
            // Load application db settings
            ConfiguracoesMongoDb configuracoesMongoDb =
                Configuration.GetSection(nameof(ConfiguracoesMongoDb)).Get<ConfiguracoesMongoDb>();

            // Load application MailJet settings
            ConfiguracoesNotificadorSmtp configuracoesNotificadorSmtp =
                Configuration.GetSection(nameof(ConfiguracoesNotificadorSmtp)).Get<ConfiguracoesNotificadorSmtp>();

            // Persistencia
            container.Register<IMongoClient>(() => new MongoClient(configuracoesMongoDb.StringConexao), Lifestyle.Singleton);
            container.Register<IContextoPersistencia, ContextoPersistencia>(Lifestyle.Transient);

            // Add application services. For instance:
            container.Register<IServicoConsultaProcessos, ServicoConsultaProcessos>(Lifestyle.Transient);
            container.Register<IServicoAplicacaoResponsaveis, ServicoAplicacaoResponsaveis>(Lifestyle.Transient);
            container.Register<IServicoAplicacaoProcessos, ServicoAplicacaoProcessos>(Lifestyle.Transient);
            container.Register<IRepositorioResponsaveis, MongoDBRepositorioResponsaveis>(Lifestyle.Transient);
            container.Register<IRemovedorResponsavel, MongoDBRepositorioResponsaveis>(Lifestyle.Transient);
            container.Register<IEditorResponsavel, MongoDBRepositorioResponsaveis>(Lifestyle.Transient);
            container.Register<IServicoConsultaResponsaveis, ServicoConsultaResponsaveis>(Lifestyle.Transient);
            container.Register<IValidadorEmail, ValidadorEmail>(Lifestyle.Transient);
            container.Register<IValidadorCpf, ValidadorCpf>(Lifestyle.Transient);
            container.Register<ICriadorProcesso, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.Register<IGeradorIdentificadorProcesso, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.Register<IRemovedorProcesso, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.Register<IValidadorProcessoUnico, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.Register<IValidadorRemocaoResponsavel, MongoDBRepositorioResponsaveis>(Lifestyle.Transient);
            container.Register<IValidadorProcessoPai, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.Register<IValidadorSituacaoRemocao, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.Register<INotificadorResponsavel, NotificadorSmtp>(Lifestyle.Transient);
            container.Register<IValidaEdicaoProcesso, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.Register<IObtentorResponsavel, MongoDBRepositorioResponsaveis>(Lifestyle.Transient);
            container.Register<IObtendorProcessoPorId, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.Register<IEditorProcesso, MongoDBRepositorioProcessos>(Lifestyle.Transient);
            container.RegisterInstance<ConfiguracoesMongoDb>(configuracoesMongoDb);
            container.RegisterInstance<IConfiguracoesNotificadorSmtp>(configuracoesNotificadorSmtp);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSimpleInjector(container);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            container.Verify();
        }
    }
}
