// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.FutureValue`1
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CodeSmith.Data.Linq
{
  /// <summary>
  /// Provides for defering the execution of a query to a batch of queries.
  /// </summary>
  /// <typeparam name="T">The type for the future query.</typeparam>
  /// <example>The following is an example of how to use FutureValue.
  /// <code><![CDATA[
  /// var db = new TrackerDataContext { Log = Console.Out };
  /// // build up queries
  /// var q1 = db.User.ByEmailAddress("one@test.com").FutureFirstOrDefault();
  /// var q2 = db.Task.Where(t => t.Summary == "Test").Future();
  /// // this triggers the loading of all the future queries
  /// User user = q1.Value;
  /// var tasks = q2.ToList();
  /// ]]>
  /// </code>
  /// </example>
  [DebuggerDisplay("IsLoaded={IsLoaded}, Value={UnderlingValue}")]
  public class FutureValue<T> : FutureQueryBase<T>, IFutureValue<T>, IFutureQuery
  {
    private bool _hasValue = false;

    /// <summary>
    /// Gets or sets the value assigned to or loaded by the query.
    /// </summary>
    /// <returns>The value of this deferred property.</returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Value
    {
      get
      {
        if (!this._hasValue)
        {
          this._hasValue = true;
          IEnumerable<T> result = this.GetResult();
          if (result != null)
            this.UnderlingValue = result.FirstOrDefault<T>();
        }
        if (this.Exception != null)
          throw new FutureException("An error occurred executing the future query.", this.Exception);
        return this.UnderlingValue;
      }
      set
      {
        this.UnderlingValue = value;
        this._hasValue = true;
      }
    }

    /// <summary>
    /// Gets the underling value. This property will not trigger the loading of the future query.
    /// </summary>
    /// <value>The underling value.</value>
    internal T UnderlingValue { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureValue`1" /> class.
    /// </summary>
    /// <param name="query">The query source to use when materializing.</param>
    /// <param name="loadAction">The action to execute when the query is accessed.</param>
    public FutureValue(IQueryable query, Action loadAction)
      : base(query, loadAction, (CacheSettings) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureValue`1" /> class.
    /// </summary>
    /// <param name="query">The query source to use when materializing.</param>
    /// <param name="loadAction">The action to execute when the query is accessed.</param>
    /// <param name="cacheSettings">The cache settings.</param>
    public FutureValue(IQueryable query, Action loadAction, CacheSettings cacheSettings)
      : base(query, loadAction, cacheSettings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureValue`1" /> class.
    /// </summary>
    /// <param name="underlyingValue">The underlying value.</param>
    public FutureValue(T underlyingValue)
      : base((IQueryable) null, (Action) null, (CacheSettings) null)
    {
      this.UnderlingValue = underlyingValue;
      this._hasValue = true;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="T:CodeSmith.Data.Linq.FutureValue`1" /> to <see cref="!:T" />.
    /// </summary>
    /// <param name="futureValue">The future value.</param>
    /// <returns>The result of forcing this lazy value.</returns>
    public static implicit operator T(FutureValue<T> futureValue)
    {
      return futureValue.Value;
    }
  }
}
