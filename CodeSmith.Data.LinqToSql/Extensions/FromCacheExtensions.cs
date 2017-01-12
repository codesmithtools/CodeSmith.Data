// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.FromCacheExtensions
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Caching;
using CodeSmith.Data.LinqToSql;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace CodeSmith.Data.Linq
{
  /// <summary>Extension methods for FromCache</summary>
  public static class FromCacheExtensions
  {
    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, int duration, params string[] sqlCacheDependencyTableNames)
    {
      IDataContext dataConext = DataContextProvider.GetDataConext((IQueryable) query);
      string databaseName = dataConext == null ? string.Empty : dataConext.ConnectionString;
      CacheSettings settings = new CacheSettings(duration).AddCacheDependency(databaseName, sqlCacheDependencyTableNames);
      return query.FromCache<T>(settings);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// </summary>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<object> FromCache(this IQueryable query, int duration, params string[] sqlCacheDependencyTableNames)
    {
      return Queryable.Cast<object>(query).FromCache<object>(duration, sqlCacheDependencyTableNames);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, int duration, params ITable[] sqlCacheDependencyTables)
    {
      CacheSettings settings = new CacheSettings(duration).AddCacheDependency<T>(query, sqlCacheDependencyTables);
      return query.FromCache<T>(settings);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// </summary>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<object> FromCache(this IQueryable query, int duration, params ITable[] sqlCacheDependencyTables)
    {
      return Queryable.Cast<object>(query).FromCache<object>(duration, sqlCacheDependencyTables);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, string profileName, params ITable[] sqlCacheDependencyTables)
    {
      DataContext dataContext = LinqToSqlDataContextProvider.GetDataContext((IQueryable) query);
      CacheSettings settings = CacheManager.GetProfile(profileName).AddCacheDependency(dataContext.Connection.Database, sqlCacheDependencyTables);
      return query.FromCache<T>(settings);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// </summary>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<object> FromCache(this IQueryable query, string profileName, params ITable[] sqlCacheDependencyTables)
    {
      return Queryable.Cast<object>(query).FromCache<object>(profileName, sqlCacheDependencyTables);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, string profileName, params string[] sqlCacheDependencyTableNames)
    {
      DataContext dataContext = LinqToSqlDataContextProvider.GetDataContext((IQueryable) query);
      CacheSettings settings = CacheManager.GetProfile(profileName).AddCacheDependency(dataContext.Connection.Database, sqlCacheDependencyTableNames);
      return query.FromCache<T>(settings);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// </summary>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<object> FromCache(this IQueryable query, string profileName, params string[] sqlCacheDependencyTableNames)
    {
      return Queryable.Cast<object>(query).FromCache<object>(profileName, sqlCacheDependencyTableNames);
    }
  }
}
