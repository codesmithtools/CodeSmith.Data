// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.DataContextBase
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;
using CodeSmith.Data.LinqToSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CodeSmith.Data.Linq
{
  /// <summary>
  /// A base class for DataContext that includes future query support.
  /// </summary>
  public class DataContextBase : DataContext, IDataContext, IDisposable, IFutureContext
  {
    private readonly List<ILinqToSqlFutureQuery> _futureQueries = new List<ILinqToSqlFutureQuery>();

    string IDataContext.ConnectionString
    {
      get
      {
        return this.Connection != null ? this.Connection.ConnectionString : string.Empty;
      }
    }

    public bool HasOpenTransaction
    {
      get
      {
        return this.Transaction != null;
      }
    }

    IEnumerable<IFutureQuery> IFutureContext.FutureQueries
    {
      get
      {
        return (IEnumerable<IFutureQuery>) this._futureQueries;
      }
    }

    static DataContextBase()
    {
      DataContextProvider.Register((IDataContextProvider) new LinqToSqlDataContextProvider());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.DataContextBase" /> class.
    /// </summary>
    /// <param name="fileOrServerOrConnection">The file or server or connection.</param>
    public DataContextBase(string fileOrServerOrConnection)
      : base(fileOrServerOrConnection)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.DataContextBase" /> class.
    /// </summary>
    /// <param name="fileOrServerOrConnection">The file or server or connection.</param>
    /// <param name="mapping">The mapping.</param>
    public DataContextBase(string fileOrServerOrConnection, MappingSource mapping)
      : base(fileOrServerOrConnection, mapping)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.DataContextBase" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    public DataContextBase(IDbConnection connection)
      : base(connection)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.DataContextBase" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="mapping">The mapping.</param>
    public DataContextBase(IDbConnection connection, MappingSource mapping)
      : base(connection, mapping)
    {
    }

    void IDataContext.Detach(params object[] enities)
    {
      foreach (object enity in enities)
      {
        ILinqEntity linqEntity = enity as ILinqEntity;
        if (linqEntity != null)
          linqEntity.Detach();
      }
    }

    IDisposable IDataContext.BeginTransaction()
    {
      return (IDisposable) this.BeginTransaction();
    }

    /// <summary>Starts a database transaction.</summary>
    /// <param name="dataContext">The data context.</param>
    /// <returns>An object representing the new transaction.</returns>
    public DbTransaction BeginTransaction()
    {
      return DataContextExtensions.BeginTransaction(this);
    }

    /// <summary>
    /// Starts a database transaction with the specified isolation level.
    /// </summary>
    /// <param name="dataContext">The data context.</param>
    /// <param name="isolationLevel">The isolation level for the transaction.</param>
    /// <returns>An object representing the new transaction.</returns>
    public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
      return DataContextExtensions.BeginTransaction(this, isolationLevel);
    }

    public void CommitTransaction()
    {
      this.Transaction.Commit();
    }

    public void RollbackTransaction()
    {
      this.Transaction.Rollback();
    }

    /// <summary>Executes the future queries.</summary>
    public void ExecuteFutureQueries()
    {
      if (this._futureQueries.Count == 0)
        return;
      List<DbCommand> dbCommandList = new List<DbCommand>();
      List<ILinqToSqlFutureQuery> toSqlFutureQueryList = new List<ILinqToSqlFutureQuery>();
      foreach (ILinqToSqlFutureQuery futureQuery in this._futureQueries)
      {
        if (!futureQuery.IsLoaded)
        {
          DbCommand command = futureQuery.GetCommand((DataContext) this);
          dbCommandList.Add(command);
          toSqlFutureQueryList.Add(futureQuery);
        }
      }
      if (dbCommandList.Count > 0)
      {
        using (System.Data.Linq.IMultipleResults result = this.ExecuteQuery((IEnumerable<DbCommand>) dbCommandList))
        {
          foreach (ILinqToSqlFutureQuery toSqlFutureQuery in toSqlFutureQueryList)
            toSqlFutureQuery.SetResult(result);
        }
      }
      this._futureQueries.Clear();
    }

    IEnumerable<T> IFutureContext.Future<T>(IQueryable<T> query, CacheSettings cacheSettings)
    {
      Action loadAction = new Action(this.ExecuteFutureQueries);
      FutureQuery<T> futureQuery = new FutureQuery<T>((IQueryable) query, loadAction, cacheSettings);
      this._futureQueries.Add((ILinqToSqlFutureQuery) futureQuery);
      return (IEnumerable<T>) futureQuery;
    }

    IFutureValue<T> IFutureContext.FutureFirstOrDefault<T>(IQueryable<T> query, CacheSettings cacheSettings)
    {
      Action loadAction = new Action(this.ExecuteFutureQueries);
      FutureValue<T> futureValue = new FutureValue<T>((IQueryable) query, loadAction, cacheSettings);
      this._futureQueries.Add((ILinqToSqlFutureQuery) futureValue);
      return (IFutureValue<T>) futureValue;
    }

    IFutureValue<int> IFutureContext.FutureCount<T>(IQueryable<T> query, CacheSettings cacheSettings)
    {
      Action loadAction = new Action(this.ExecuteFutureQueries);
      FutureCount futureCount = new FutureCount((IQueryable) query, loadAction, cacheSettings);
      this._futureQueries.Add((ILinqToSqlFutureQuery) futureCount);
      return (IFutureValue<int>) futureCount;
    }

    void IDataContext.SubmitChanges()
    {
      this.SubmitChanges();
    }

    //[SpecialName]
    //bool IDataContext.get_ObjectTrackingEnabled()
    //{
    //  return this.ObjectTrackingEnabled;
    //}

    //[SpecialName]
    //void IDataContext.set_ObjectTrackingEnabled(bool value)
    //{
    //  this.ObjectTrackingEnabled = value;
    //}
  }
}
