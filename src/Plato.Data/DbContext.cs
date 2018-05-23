﻿using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Data.Abstractions;

namespace Plato.Data
{
    public class DbContext : IDbContext, IDisposable
    {
        #region "Private Variables"

        private IDataProvider _provider;

        #endregion

        #region "Public Properties"

        public DbContextOptions Configuration { get; set; }

        #endregion

        #region "Dispose"

        public void Dispose()
        {
            _provider?.Dispose();
        }

        #endregion

        #region "Constructos"

        public DbContext(
            IOptions<DbContextOptions> dbContextOptions)
        {
            ConfigureInternal(dbContextOptions.Value);
        }

        public DbContext(Action<DbContextOptions> options)
        {
            var cfg = new DbContextOptions();
            options(cfg);
            Configure(options);
        }

        public void Configure(Action<DbContextOptions> options)
        {
            var cfg = new DbContextOptions();
            options(cfg);
            ConfigureInternal(cfg);
        }


        private void ConfigureInternal(DbContextOptions cfg)
        {
            var providerFactory = new DataProviderFactory(cfg);
            _provider = providerFactory.Provider;
            if (_provider != null)
            {
                _provider.OnException += (sender, args) => throw args.Exception;
            }
            Configuration = cfg;
        }

        #endregion

        #region "Implementation"

        public IDataReader ExecuteReader(CommandType commandType, string sql, params object[] commandParams)
        {
            if (_provider == null)
                return null;
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, commandParams);
            return _provider.ExecuteReader(sql, commandParams);
        }

        public async Task<DbDataReader> ExecuteReaderAsync(
            CommandType commandType,
            string sql,
            params object[] commandParams)
        {
            if (_provider == null)
                return null;
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, commandParams);
            return await _provider.ExecuteReaderAsync(sql, commandParams);
        }

        public T ExecuteScalar<T>(CommandType commandType, string sql, params object[] args)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, args);
            return _provider.ExecuteScalar<T>(sql, args);
        }

        public async Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] args)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, args);
            return await _provider.ExecuteScalarAsync<T>(sql, args);
        }

        public void Execute(CommandType commandType, string sql, params object[] args)
        {
            if (_provider == null)
                return;
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, args);
            _provider.Execute(sql, args);
        }

        #endregion


        #region "Private Methods"

        private string GenerateExecuteStoredProcedureSql(string procedureName, params object[] args)
        {
         
            var sb = new StringBuilder(";EXEC ");
            sb.Append(GetProcedureName(procedureName));
            for (var i = 0; i < args.Length; i++)
            {
                sb.Append($" @{i}");
                if (i < args.Length - 1)
                    sb.Append(",");
            }
            
            return sb.ToString();
        }

        private string GetProcedureName(string procedureName)
        {
            return !string.IsNullOrEmpty(this.Configuration.TablePrefix)
                ? this.Configuration.TablePrefix + "_" + procedureName
                : procedureName;
        }

        #endregion



    }
}