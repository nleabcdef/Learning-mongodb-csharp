using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PassphraseManagerSvc.Models;
using PassphraseManagerSvc.Repo;
using System.Text.Json;

namespace PassphraseManagerSvc
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
            services.AddSingleton<IMapper>(new Mapper(CreateConfiguration()));

            ConfigRepos(services);

            services.AddControllers();
            services.AddSwaggerGen();
        }

        void ConfigRepos(IServiceCollection services)
        {
            IPassStoreRepo<PasswordStoreModel> repo = null;

            var repos = Configuration.GetSection("Repos").Get<string[]>();
            var option = int.Parse(Configuration.GetValue<string>("PreferredRepo"));
            if (repos[option - 1].Equals("mongo", StringComparison.OrdinalIgnoreCase))
            {
                var cs = Configuration.GetSection("Mongo").GetValue<string>("DbConnection");
                var di = cs.LastIndexOf("/");
                var db = cs?.Substring(di+1, cs.IndexOf("?") - di -1);
                repo = new PassStoreMongo(new MongoClient(cs),db);
            }

            services.AddSingleton<IPassStoreRepo<PasswordStoreModel>>(repo ?? new PassStoreInMemory());

            new TaskFactory().StartNew(() => {
                LoadTestData(services);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseExceptionHandler("/ErrorResponse");
            
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Password Store Api");
                opt.RoutePrefix = string.Empty;
            });
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public MapperConfiguration CreateConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(GetType().Assembly);
            });

            return config;
        }

        async void LoadTestData(IServiceCollection services)
        {
            if(!Configuration.GetValue<bool>("LoadTestData"))
                return;
            
            try
            {
                var fpath = Configuration.GetValue<string>("DataFile");
                FileInfo file =  new FileInfo(fpath);
                if(!string.IsNullOrWhiteSpace(fpath))
                if(!file.Exists)
                    file = new FileInfo(Path.Combine(
                        GetType().Assembly.Location.Substring(0, 
                        GetType().Assembly.Location.LastIndexOf(Path.DirectorySeparatorChar)+1), file.Name));

                if(!string.IsNullOrWhiteSpace(fpath) && file.Exists)
                {
                    var lstPasss = JsonSerializer.Deserialize(File.ReadAllText(fpath), 
                        typeof(List<PasswordStoreModel>),
                        new JsonSerializerOptions() { IgnoreNullValues = true }) as List<PasswordStoreModel>;
                    if(lstPasss.Any())
                    {
                        var svc = services.First(svc => svc.ServiceType == typeof(IPassStoreRepo<PasswordStoreModel>))?
                            .ImplementationInstance as IPassStoreRepo<PasswordStoreModel>;
                        foreach(var psdoc in lstPasss)
                            try { await svc.Create(psdoc);} catch{}
                    }
                }
            }
            catch(Exception ex) {}
        }
    }
}
