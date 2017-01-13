// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.UtilityExtensions
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.LinqToSql;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeSmith.Data.Linq
{
  /// <summary>Extension Methods.</summary>
  public static class UtilityExtensions
  {
    /// <summary>
    /// Specifies the related objects to include in the query results.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <typeparam name="TInclude">The type of the include.</typeparam>
    /// <param name="query">The query to be materialized.</param>
    /// <param name="includeExpression">The expression of related objects to return in the query results.</param>
    /// <returns>The result of the query.</returns>
    public static IEnumerable<T> Include<T, TInclude>(this IQueryable<T> query, Expression<Func<T, TInclude>> includeExpression)
    {
      DataContext dataContext = LinqToSqlDataContextProvider.GetDataContext((IQueryable) query);
      if (dataContext == null)
        throw new ArgumentException("The query must originate from a DataContext.", "query");
      if (!dataContext.ObjectTrackingEnabled || !dataContext.DeferredLoadingEnabled)
        throw new ArgumentException("The query's originating DataContext must have ObjectTrackingEnabled and DeferredLoadingEnabled set to true.", "query");
      ParameterExpression parameterExpression = includeExpression.Parameters.Single<ParameterExpression>();
      Type type = typeof (UtilityExtensions.Tuple<T, TInclude>);
      Expression<Func<T, UtilityExtensions.Tuple<T, TInclude>>> selector = Expression.Lambda<Func<T, UtilityExtensions.Tuple<T, TInclude>>>((Expression) Expression.New(type.GetConstructor(new Type[2]
      {
        typeof (T),
        typeof (TInclude)
      }), (IEnumerable<Expression>) new Expression[2]
      {
        (Expression) parameterExpression,
        includeExpression.Body
      }, (MemberInfo) type.GetProperty("Item1"), (MemberInfo) type.GetProperty("Item2")), new ParameterExpression[1]
      {
        parameterExpression
      });
      return query.Select<T, UtilityExtensions.Tuple<T, TInclude>>(selector).AsEnumerable<UtilityExtensions.Tuple<T, TInclude>>().Select<UtilityExtensions.Tuple<T, TInclude>, T>((Func<UtilityExtensions.Tuple<T, TInclude>, T>) (t => t.Item1));
    }

    private class Tuple<T1, T2>
    {
      public T1 Item1 { get; private set; }

      public T2 Item2 { get; private set; }

      public Tuple(T1 item1, T2 item2)
      {
        this.Item1 = item1;
        this.Item2 = item2;
      }
    }
  }
}
