// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.ILinqToSqlFutureQuery
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Future;
using System.Data.Common;
using System.Data.Linq;

namespace CodeSmith.Data.Linq
{
  /// <summary>
  /// Interface for defering the future execution of a batch of queries.
  /// </summary>
  public interface ILinqToSqlFutureQuery : IFutureQuery
  {
    /// <summary>Gets the data command for this query.</summary>
    /// <param name="dataContext">The data context to get the command from.</param>
    /// <returns>The requested command object.</returns>
    DbCommand GetCommand(DataContext dataContext);

    /// <summary>
    /// Sets the underling value after the query has been executed.
    /// </summary>
    /// <param name="result">The <see cref="T:System.Data.Linq.IMultipleResults" /> to get the result from.</param>
    void SetResult(System.Data.Linq.IMultipleResults result);
  }
}
