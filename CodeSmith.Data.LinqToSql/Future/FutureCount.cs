// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.FutureCount
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Caching;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeSmith.Data.Future;

namespace CodeSmith.Data.Linq
{
  /// <summary>
  /// Provides for defering the execution of a count query to a batch of queries.
  /// </summary>
  /// <example>The following is an example of how to use FutureCount to page a
  /// list and get the total count in one call.
  /// <code><![CDATA[
  /// var db = new TrackerDataContext { Log = Console.Out };
  /// // base query
  /// var q = db.Task.ByPriority(Priority.Normal).OrderBy(t => t.CreatedDate);
  /// // get total count
  /// var q1 = q.FutureCount();
  /// // get first page
  /// var q2 = q.Skip(0).Take(10).Future();
  /// // triggers sql execute as a batch
  /// var tasks = q2.ToList();
  /// int total = q1.Value;
  /// ]]>
  /// </code>
  /// </example>
  [DebuggerDisplay("IsLoaded={IsLoaded}, Value={ValueForDebugDisplay}")]
  public class FutureCount : FutureValue<int>
  {
    private static MethodInfo _countMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureCount" /> class.
    /// </summary>
    /// <param name="query">The query source to use when materializing.</param>
    /// <param name="loadAction">The action to execute when the query is accessed.</param>
    public FutureCount(IQueryable query, Action loadAction)
      : base(query, loadAction, (CacheSettings) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureCount" /> class.
    /// </summary>
    /// <param name="query">The query source to use when materializing.</param>
    /// <param name="loadAction">The action to execute when the query is accessed.</param>
    /// <param name="cacheSettings">The cache settings.</param>
    public FutureCount(IQueryable query, Action loadAction, CacheSettings cacheSettings)
      : base(query, loadAction, cacheSettings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureCount" /> class.
    /// </summary>
    /// <param name="underlyingValue">The underlying value.</param>
    public FutureCount(int underlyingValue)
      : base(underlyingValue)
    {
    }

    /// <summary>Gets the key used when caching the results.</summary>
    /// <returns></returns>
    protected override string GetKey()
    {
      return base.GetKey() + "_count";
    }

    /// <summary>Gets the data command for this query.</summary>
    /// <param name="dataContext">The data context to get the command from.</param>
    /// <returns>The requested command object.</returns>
    protected override DbCommand GetCommand(DataContext dataContext)
    {

        var query = ((IFutureQuery) this).Query;
      //IQueryable query = this.Query;
      FutureCount.FindCountMethod();
      MethodCallExpression methodCallExpression = Expression.Call((Expression) null, FutureCount._countMethod.MakeGenericMethod(query.ElementType), new Expression[1]
      {
        query.Expression
      });
      return dataContext.GetCommand((Expression) methodCallExpression);
    }

    private static void FindCountMethod()
    {
      if (FutureCount._countMethod != (MethodInfo) null)
        return;
      FutureCount._countMethod = ((IEnumerable<MethodInfo>) typeof (Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "Count" && m.IsGenericMethod && m.GetParameters().Length == 1)).FirstOrDefault<MethodInfo>();
    }
  }
}
