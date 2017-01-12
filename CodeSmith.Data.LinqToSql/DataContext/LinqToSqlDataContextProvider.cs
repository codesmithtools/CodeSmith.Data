// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.LinqToSql.LinqToSqlDataContextProvider
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using CodeSmith.Data.Future;
using System.Data.Linq;
using System.Linq;
using System.Reflection;

namespace CodeSmith.Data.LinqToSql
{
  /// <summary>
  /// 
  /// </summary>
  public class LinqToSqlDataContextProvider : IDataContextProvider
  {
    public static DataContext GetDataContext(IQueryable query)
    {
      if (query == null)
        return (DataContext) null;
      FieldInfo field = query.GetType().GetField("context", BindingFlags.Instance | BindingFlags.NonPublic);
      if (field == (FieldInfo) null)
        return (DataContext) null;
      object obj = field.GetValue((object) query);
      if (obj == null)
        return (DataContext) null;
      return obj as DataContext;
    }

    IDataContext IDataContextProvider.GetDataConext(IQueryable query)
    {
      return LinqToSqlDataContextProvider.GetDataContext(query) as IDataContext;
    }

    /// <summary>Gets the Future Context.</summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public IFutureContext GetFutureContext(IQueryable query)
    {
      return LinqToSqlDataContextProvider.GetDataContext(query) as IFutureContext;
    }
  }
}
