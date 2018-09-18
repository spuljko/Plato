﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Features.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {
        
        public string Version { get; } = "1.0.0";

        private readonly SchemaTable _shellFeatures = new SchemaTable()
        {
            Name = "ShellFeatures",
            Columns = new List<SchemaColumn>()
            {
                new SchemaColumn()
                {
                    PrimaryKey = true,
                    Name = "Id",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "ModuleId",
                    DbType = DbType.String,
                    Length = "255"
                },
                new SchemaColumn()
                {
                    Name = "[Version]",
                    DbType = DbType.String,
                    Length = "10"
                },
                new SchemaColumn()
                {
                    Name = "Settings",
                    DbType = DbType.String,
                    Length = "max"
                }
            }
        };


        private readonly ISchemaBuilder _schemaBuilder;
    
        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }

        public override async Task SetUp(
            SetUpContext context,
            Action<string, string> reportError)
        {
            
            // --------------------------
            // Build core schemas
            // --------------------------
            
            using (var builder = _schemaBuilder)
            {
                
                // configure
                Configure(builder);

                // features schema
                Features(builder);

                // Did any errors occur?

                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Message, error.StackTrace);
                    }
                 
                }
            }
            
        }


        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = base.ModuleId;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Features(ISchemaBuilder builder)
        {
         
            builder
                .CreateTable(_shellFeatures)
                .CreateDefaultProcedures(_shellFeatures)

                .CreateProcedure(new SchemaProcedure("SelectShellFeaturesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_shellFeatures)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Id",
                            DbType = DbType.Int32
                        },
                        new SchemaColumn()
                        {
                            Name = "ModuleId",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));
            
        }

        #endregion


    }

}
