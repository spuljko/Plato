﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Data.Abstractions.Schemas;
using Plato.Data.Migrations;

namespace Plato.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoDbContext(
            this IServiceCollection services)
        {

            //services.AddDataMigrations();

            //services.AddSingleton<IConfigureOptions<DbContextOptions>, DbContextOptionsConfigure>();         
            //services.AddTransient<IDbContext, DbContext>();


        
            services.AddTransient<IDataMigrationManager, DataMigrationManager>();
            services.AddTransient<AutomaticDataMigrations>();

            return services;
                    }


    }
}
