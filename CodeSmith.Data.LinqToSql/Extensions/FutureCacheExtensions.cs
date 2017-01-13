// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.FutureCacheExtensions
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Caching;
using System.Collections.Generic;
using System.Linq;

namespace CodeSmith.Data.Linq
{
  /// <summary>Extension Methods for Future Cache.</summary>
  public static class FutureCacheExtensions
  {
    /// <summary>
    /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
    /// <param name="profileName">Name of the cache profile to use.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence.
    /// </returns>
    /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1" />
    public static IEnumerable<T> FutureCache<T>(this IQueryable<T> source, string profileName, params string[] sqlCacheDependencyTableNames)
    {
      CacheSettings cacheSettings = CacheManager.GetProfile(profileName).AddCacheDependency<T>(source, sqlCacheDependencyTableNames);
      return source.FutureCache<T>(cacheSettings);
    }

    /// <summary>
    /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
    /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
    /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence.
    /// </returns>
    /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1" />
    public static IEnumerable<T> FutureCache<T>(this IQueryable<T> source, int duration, params string[] sqlCacheDependencyTableNames)
    {
      CacheSettings cacheSettings = new CacheSettings(duration).AddCacheDependency<T>(source, sqlCacheDependencyTableNames);
      return source.FutureCache<T>(cacheSettings);
    }
  }
}
