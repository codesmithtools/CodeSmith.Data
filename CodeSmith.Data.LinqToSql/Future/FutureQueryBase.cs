// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.FutureQueryBase`1
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;

namespace CodeSmith.Data.Linq
{
  /// <summary>Base class for future queries.</summary>
  /// <typeparam name="T">The type for the future query.</typeparam>
  [DebuggerDisplay("IsLoaded={IsLoaded}")]
  public class FutureQueryBase<T> : ILinqToSqlFutureQuery, IFutureQuery
  {
    private readonly Action _loadAction;
    private readonly IQueryable _query;
    private IEnumerable<T> _result;
    private bool _isLoaded;
    private CacheSettings _cacheSettings;

    /// <summary>
    /// Gets the action to execute when the query is accessed.
    /// </summary>
    /// <value>The load action.</value>
    protected Action LoadAction
    {
      get
      {
        return this._loadAction;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is loaded.
    /// </summary>
    /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
    public bool IsLoaded
    {
      get
      {
        if (this._isLoaded)
          return this._isLoaded;
        this.CheckCache();
        return this._isLoaded;
      }
    }

    /// <summary>Gets or sets the query execute exception.</summary>
    /// <value>The query execute exception.</value>
    public Exception Exception { get; set; }

    IQueryable IFutureQuery.Query
    {
      get
      {
        return this._query;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureQuery`1" /> class.
    /// </summary>
    /// <param name="query">The query source to use when materializing.</param>
    /// <param name="loadAction">The action to execute when the query is accessed.</param>
    public FutureQueryBase(IQueryable query, Action loadAction)
      : this(query, loadAction, (CacheSettings) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureQuery`1" /> class.
    /// </summary>
    /// <param name="query">The query source to use when materializing.</param>
    /// <param name="loadAction">The action to execute when the query is accessed.</param>
    /// <param name="cacheSettings">The cache settings.</param>
    public FutureQueryBase(IQueryable query, Action loadAction, CacheSettings cacheSettings)
    {
      this._query = query;
      this._loadAction = loadAction;
      this._cacheSettings = cacheSettings;
      this._result = (IEnumerable<T>) null;
    }

    /// <summary>Checks the cache for the results.</summary>
    private void CheckCache()
    {
      if (this._cacheSettings == null)
        return;
      ICollection<T> resultCache = QueryResultCache.GetResultCache<T>(this.GetKey(), this._cacheSettings);
      if (resultCache == null)
        return;
      this._isLoaded = true;
      this._result = (IEnumerable<T>) resultCache;
    }

    /// <summary>Gets the key used when caching the results.</summary>
    /// <returns></returns>
    protected virtual string GetKey()
    {
      return QueryResultCache.GetHashKey(this._query);
    }

    /// <summary>
    /// Gets the result by invoking the <see cref="P:CodeSmith.Data.Linq.FutureQueryBase`1.LoadAction" /> if not already loaded.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that can be used to iterate through the collection.
    /// </returns>
    protected virtual IEnumerable<T> GetResult()
    {
      if (this.IsLoaded)
        return this._result;
      if (this.LoadAction == null)
      {
        this._isLoaded = true;
        this._result = this._query as IEnumerable<T>;
        return this._result;
      }
      this.LoadAction();
      return this._result;
    }

    DbCommand ILinqToSqlFutureQuery.GetCommand(DataContext dataContext)
    {
      return this.GetCommand(dataContext);
    }

    /// <summary>Gets the data command for this query.</summary>
    /// <param name="dataContext">The data context to get the command from.</param>
    /// <returns>The requested command object.</returns>
    protected virtual DbCommand GetCommand(DataContext dataContext)
    {
      return dataContext.GetCommand(this._query, true);
    }

    void ILinqToSqlFutureQuery.SetResult(System.Data.Linq.IMultipleResults result)
    {
      this.SetResult(result);
    }

    /// <summary>
    /// Sets the underling value after the query has been executed.
    /// </summary>
    /// <param name="result">The <see cref="T:System.Data.Linq.IMultipleResults" /> to get the result from.</param>
    protected virtual void SetResult(System.Data.Linq.IMultipleResults result)
    {
      this._isLoaded = true;
      List<T> objList = (List<T>) null;
      try
      {
        IEnumerable<T> result1 = result.GetResult<T>();
        objList = result1 != null ? result1.ToList<T>() : new List<T>();
        this._result = (IEnumerable<T>) objList;
      }
      catch (Exception ex)
      {
        this.Exception = ex;
      }
      if (this._cacheSettings == null || objList == null)
        return;
      QueryResultCache.SetResultCache<T>(this.GetKey(), this._cacheSettings, (ICollection<T>) objList);
    }
  }
}
