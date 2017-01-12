// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.LinqToSql.CacheSettingsExtensions
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web.Caching;

namespace CodeSmith.Data.LinqToSql
{
  /// <summary>Cache Settings Extension Methods.</summary>
  public static class CacheSettingsExtensions
  {
    /// <summary>Adds a Cache Dependency.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="settings"></param>
    /// <param name="queryable"></param>
    /// <param name="sqlCacheDependencyTables"></param>
    /// <returns></returns>
    public static CacheSettings AddCacheDependency<T>(this CacheSettings settings, IQueryable<T> queryable, params ITable[] sqlCacheDependencyTables)
    {
      IDataContext dataConext = DataContextProvider.GetDataConext((IQueryable) queryable);
      string databaseName = dataConext == null ? string.Empty : dataConext.ConnectionString;
      return settings.AddCacheDependency(databaseName, sqlCacheDependencyTables);
    }

    /// <summary>Adds a Cache Dependency.</summary>
    /// <param name="settings"></param>
    /// <param name="databaseName"></param>
    /// <param name="sqlCacheDependencyTables"></param>
    /// <returns></returns>
    public static CacheSettings AddCacheDependency(this CacheSettings settings, string databaseName, params ITable[] sqlCacheDependencyTables)
    {
      if (sqlCacheDependencyTables == null)
        return settings;
      IEnumerable<SqlCacheDependency> source = ((IEnumerable<ITable>) sqlCacheDependencyTables).Select<ITable, SqlCacheDependency>((Func<ITable, SqlCacheDependency>) (t => new SqlCacheDependency(databaseName, t.TableName())));
      return settings.AddCacheDependency(source.Cast<CacheDependency>());
    }
  }
}
