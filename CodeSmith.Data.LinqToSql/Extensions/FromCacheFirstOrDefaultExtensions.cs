// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.FromCacheFirstOrDefaultExtensions
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Caching;
using CodeSmith.Data.LinqToSql;
using System.Data.Linq;
using System.Linq;

namespace CodeSmith.Data.Linq
{
  /// <summary>Extension Methods for Future Cache FirstOrDefault.</summary>
  public static class FromCacheFirstOrDefaultExtensions
  {
    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// Queries, caches, and returns only the first entity.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
    /// <returns>The first or default result of the query.</returns>
    public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, int duration, params ITable[] sqlCacheDependencyTables)
    {
      DataContext dataContext = LinqToSqlDataContextProvider.GetDataContext((IQueryable) query);
      CacheSettings settings = new CacheSettings(duration).AddCacheDependency(dataContext.Connection.Database, sqlCacheDependencyTables);
      return Queryable.Take<T>(query, 1).FromCache<T>(settings).FirstOrDefault<T>();
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// Queries, caches, and returns only the first entity.
    /// </summary>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
    /// <returns>The first or default result of the query.</returns>
    public static object FromCacheFirstOrDefault(this IQueryable query, int duration, params ITable[] sqlCacheDependencyTables)
    {
      return Queryable.Cast<object>(query).FromCacheFirstOrDefault<object>(duration, sqlCacheDependencyTables);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// Queries, caches, and returns only the first entity.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>The first or default result of the query.</returns>
    public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, int duration, params string[] sqlCacheDependencyTableNames)
    {
      DataContext dataContext = LinqToSqlDataContextProvider.GetDataContext((IQueryable) query);
      CacheSettings settings = new CacheSettings(duration).AddCacheDependency(dataContext.Connection.Database, sqlCacheDependencyTableNames);
      return Queryable.Take<T>(query, 1).FromCache<T>(settings).FirstOrDefault<T>();
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// Queries, caches, and returns only the first entity.
    /// </summary>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>The first or default result of the query.</returns>
    public static object FromCacheFirstOrDefault(this IQueryable query, int duration, params string[] sqlCacheDependencyTableNames)
    {
      return Queryable.Cast<object>(query).FromCacheFirstOrDefault<object>(duration, sqlCacheDependencyTableNames);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// Queries, caches, and returns only the first entity.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
    /// <returns>The first or default result of the query.</returns>
    public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, string profileName, params ITable[] sqlCacheDependencyTables)
    {
      DataContext dataContext = LinqToSqlDataContextProvider.GetDataContext((IQueryable) query);
      CacheSettings settings = CacheManager.GetProfile(profileName).AddCacheDependency(dataContext.Connection.Database, sqlCacheDependencyTables);
      return Queryable.Take<T>(query, 1).FromCache<T>(settings).FirstOrDefault<T>();
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// Queries, caches, and returns only the first entity.
    /// </summary>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
    /// <returns>The first or default result of the query.</returns>
    public static object FromCacheFirstOrDefault(this IQueryable query, string profileName, params ITable[] sqlCacheDependencyTables)
    {
      return Queryable.Cast<object>(query).FromCacheFirstOrDefault<object>(profileName, sqlCacheDependencyTables);
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// Queries, caches, and returns only the first entity.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>The first or default result of the query.</returns>
    public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, string profileName, params string[] sqlCacheDependencyTableNames)
    {
      DataContext dataContext = LinqToSqlDataContextProvider.GetDataContext((IQueryable) query);
      CacheSettings settings = CacheManager.GetProfile(profileName).AddCacheDependency(dataContext.Connection.Database, sqlCacheDependencyTableNames);
      return Queryable.Take<T>(query, 1).FromCache<T>(settings).FirstOrDefault<T>();
    }

    /// <summary>
    /// Returns the result of the query; if possible from the cache, otherwise
    /// the query is materialized and the result cached before being returned.
    /// Queries, caches, and returns only the first entity.
    /// </summary>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>The first or default result of the query.</returns>
    public static object FromCacheFirstOrDefault(this IQueryable query, string profileName, params string[] sqlCacheDependencyTableNames)
    {
      return Queryable.Cast<object>(query).FromCacheFirstOrDefault<object>(profileName, sqlCacheDependencyTableNames);
    }
  }
}
