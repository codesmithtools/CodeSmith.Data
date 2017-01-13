// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.TableExtensions
// Assembly: CodeSmith.Data.LinqToSql, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CBD6C95-6C56-4BE1-932E-E8F5BD9B3E7B
// Assembly location: C:\_Git\VerifyPlatform\External\CodeSmith.Data.LinqToSql.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Caching;

namespace CodeSmith.Data.Linq
{
  public static class TableExtensions
  {
    /// <summary>
    /// Immediately deletes all entities from the collection with a single delete command.
    /// </summary>
    /// <typeparam name="TEntity">Represents the object type for rows contained in <paramref name="table" />.</typeparam>
    /// <param name="table">Represents a table for a particular type in the underlying database containing rows to be deleted.</param>
    /// <param name="entities">Represents the collection of items which are to be removed from <paramref name="table" />.</param>
    /// <returns>The number of rows deleted from the database.</returns>
    /// <remarks>
    /// <para>Similar to stored procedures, and opposite from DeleteAllOnSubmit, rows provided in <paramref name="entities" /> will be deleted immediately with no need to call <see cref="M:System.Data.Linq.DataContext.SubmitChanges" />.</para>
    /// <para>Additionally, to improve performance, instead of creating a delete command for each item in <paramref name="entities" />, a single delete command is created.</para>
    /// </remarks>
    public static int Delete<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
    {
      using (DbCommand deleteBatchCommand = table.GetDeleteBatchCommand<TEntity>(entities))
        return table.Context.ExecuteCommand(deleteBatchCommand);
    }

    /// <summary>
    /// Immediately deletes all entities from the collection with a single delete command.
    /// </summary>
    /// <typeparam name="TEntity">Represents the object type for rows contained in <paramref name="table" />.</typeparam>
    /// <param name="table">Represents a table for a particular type in the underlying database containing rows to be deleted.</param>
    /// <param name="filter">Represents a filter of items to be deleted in <paramref name="table" />.</param>
    /// <returns>The number of rows deleted from the database.</returns>
    public static int Delete<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> filter) where TEntity : class
    {
      return table.Delete<TEntity>(table.Where<TEntity>(filter));
    }

    /// <summary>
    /// Immediately updates all entities in the collection with a single update command based on a <typeparamref name="TEntity" /> created from a Lambda expression.
    /// </summary>
    /// <typeparam name="TEntity">Represents the object type for rows contained in <paramref name="table" />.</typeparam>
    /// <param name="table">Represents a table for a particular type in the underlying database containing rows to be updated.</param>
    /// <param name="entities">Represents the collection of items which are to be updated in <paramref name="table" />.</param>
    /// <param name="evaluator">A Lambda expression returning a <typeparamref name="TEntity" /> that defines the update assignments to be performed on each item in <paramref name="entities" />.</param>
    /// <returns>The number of rows updated in the database.</returns>
    /// <remarks>
    /// <para>Similar to stored procedures, and opposite from InsertAllOnSubmit, rows provided in <paramref name="entities" /> will be updated immediately with no need to call <see cref="M:System.Data.Linq.DataContext.SubmitChanges" />.</para>
    /// <para>Additionally, to improve performance, instead of creating an update command for each item in <paramref name="entities" />, a single update command is created.</para>
    /// </remarks>
    public static int Update<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
    {
      using (DbCommand updateBatchCommand = table.GetUpdateBatchCommand<TEntity>(entities, evaluator))
        return table.Context.ExecuteCommand(updateBatchCommand);
    }

    /// <summary>
    /// Immediately updates all entities in the collection with a single update command based on a <typeparamref name="TEntity" /> created from a Lambda expression.
    /// </summary>
    /// <typeparam name="TEntity">Represents the object type for rows contained in <paramref name="table" />.</typeparam>
    /// <param name="table">Represents a table for a particular type in the underlying database containing rows to be updated.</param>
    /// <param name="filter">Represents a filter of items to be updated in <paramref name="table" />.</param>
    /// <param name="evaluator">A Lambda expression returning a <typeparamref name="TEntity" /> that defines the update assignments to be performed on each item in <paramref name="filter" />.</param>
    /// <returns>The number of rows updated in the database.</returns>
    public static int Update<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
    {
      return table.Update<TEntity>(table.Where<TEntity>(filter), evaluator);
    }

    private static DbCommand GetDeleteBatchCommand<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
    {
      DbCommand command = table.Context.GetCommand((IQueryable) entities);
      command.CommandText = string.Format("DELETE {0}\r\n", (object) table.GetDbName<TEntity>()) + TableExtensions.GetBatchJoinQuery<TEntity>(table, entities);
      return command;
    }

    private static DbCommand GetUpdateBatchCommand<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
    {
      DbCommand updateCommand = table.Context.GetCommand((IQueryable) entities);
      StringBuilder setSB = new StringBuilder();
      int memberInitCount = 1;
      evaluator.Visit<MemberInitExpression>((Func<MemberInitExpression, Expression>) (expression =>
      {
        if (memberInitCount > 1)
          throw new NotImplementedException("Currently only one MemberInitExpression is allowed for the evaluator parameter.");
        ++memberInitCount;
        setSB.Append(TableExtensions.GetDbSetStatement<TEntity>(expression, table, updateCommand));
        return (Expression) expression;
      }));
      updateCommand.CommandText = string.Format("UPDATE {0}\r\n{1}\r\n\r\n{2}", (object) table.GetDbName<TEntity>(), (object) setSB, (object) TableExtensions.GetBatchJoinQuery<TEntity>(table, entities));
      if (updateCommand.CommandText.IndexOf("[arg0]") >= 0)
        throw new NotSupportedException(string.Format("The evaluator Expression<Func<{0},{0}>> has processing that needs to be performed once the query is returned (i.e. string.Format()) and therefore can not be used during batch updating.", (object) table.GetType()));
      return updateCommand;
    }

    private static string GetBatchJoinQuery<TEntity>(Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
    {
      MetaTable table1 = table.Context.Mapping.GetTable(typeof (TEntity));
      var datas = table1.RowType.DataMembers.Where<MetaDataMember>((Func<MetaDataMember, bool>) (mdm => mdm.IsPrimaryKey)).Select(mdm =>
      {
        var data = new
        {
          MappedName = mdm.MappedName
        };
        return data;
      });
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      foreach (var data in datas)
      {
        stringBuilder1.AppendFormat("j0.[{0}] = j1.[{0}] AND ", (object) data.MappedName);
        stringBuilder2.AppendFormat("[t0].[{0}], ", (object) data.MappedName);
      }
      string commandText = table.Context.GetCommand((IQueryable) entities).CommandText;
      string str1 = stringBuilder1.ToString();
      if (str1 == "")
        throw new MissingPrimaryKeyException(string.Format("{0} does not have a primary key defined.  Batch updating/deleting can not be used for tables without a primary key.", (object) table1.TableName));
      string str2 = str1.Substring(0, str1.Length - 5);
      string str3 = commandText.Substring(0, commandText.IndexOf("[t0]"));
      bool flag = str3.IndexOf(" TOP ") < 0 && commandText.IndexOf("\r\nORDER BY ") > 0;
      string str4 = str3 + (flag ? (object) "TOP 100 PERCENT " : (object) "") + (object) stringBuilder2;
      string str5 = str4.Substring(0, str4.Length - 2) + commandText.Substring(commandText.IndexOf("\r\nFROM "));
      return string.Format("FROM {0} AS j0 INNER JOIN (\r\n\r\n{1}\r\n\r\n) AS j1 ON ({2})\r\n", (object) table.GetDbName<TEntity>(), (object) str5, (object) str2);
    }

    private static string GetDbSetStatement<TEntity>(MemberInitExpression memberInitExpression, Table<TEntity> table, DbCommand updateCommand) where TEntity : class
    {
      Type entityType = typeof (TEntity);
      if (memberInitExpression.Type != entityType)
        throw new NotImplementedException(string.Format("The MemberInitExpression is initializing a class of the incorrect type '{0}' and it should be '{1}'.", (object) memberInitExpression.Type, (object) entityType));
      StringBuilder stringBuilder = new StringBuilder();
      string dbName = table.GetDbName<TEntity>();
      var source = table.Context.Mapping.GetTable(entityType).RowType.DataMembers.Select(mdm =>
      {
        var data = new
        {
          MappedName = mdm.MappedName,
          Name = mdm.Name
        };
        return data;
      });
      foreach (MemberBinding binding in memberInitExpression.Bindings)
      {
        MemberAssignment memberAssignment = binding as MemberAssignment;
        if (memberAssignment == null)
          throw new NotImplementedException("All bindings inside the MemberInitExpression are expected to be of type MemberAssignment.");
        ParameterExpression entityParam = (ParameterExpression) null;
        memberAssignment.Expression.Visit<ParameterExpression>((Func<ParameterExpression, Expression>) (p =>
        {
          if (p.Type == entityType)
            entityParam = p;
          return (Expression) p;
        }));
        string name = binding.Member.Name;
        var data = source.Where(c => c.Name == name).FirstOrDefault();
        if (data == null)
          throw new ArgumentOutOfRangeException(name, string.Format("The corresponding field on the {0} table could not be found.", (object) dbName));
        if (entityParam == null)
        {
          object obj = Expression.Lambda(memberAssignment.Expression, (ParameterExpression[]) null).Compile().DynamicInvoke();
          if (obj == null)
          {
            stringBuilder.AppendFormat("[{0}] = null, ", (object) data.MappedName);
          }
          else
          {
            stringBuilder.AppendFormat("[{0}] = @p{1}, ", (object) data.MappedName, (object) updateCommand.Parameters.Count);
            updateCommand.Parameters.Add((object) new SqlParameter(string.Format("@p{0}", (object) updateCommand.Parameters.Count), obj));
          }
        }
        else
        {
          MethodCallExpression selectExpression = Expression.Call(typeof (Queryable), "Select", new Type[2]
          {
            entityType,
            memberAssignment.Expression.Type
          }, new Expression[2]
          {
            (Expression) Expression.Constant((object) table),
            (Expression) Expression.Lambda(memberAssignment.Expression, new ParameterExpression[1]
            {
              entityParam
            })
          });
          stringBuilder.AppendFormat("[{0}] = {1}, ", (object) data.MappedName, (object) TableExtensions.GetDbSetAssignment((ITable) table, selectExpression, updateCommand, name));
        }
      }
      string str = stringBuilder.ToString();
      return "SET " + str.Substring(0, str.Length - 2);
    }

    private static string GetDbSetAssignment(ITable table, MethodCallExpression selectExpression, DbCommand updateCommand, string bindingName)
    {
      TableExtensions.ValidateExpression(table, (Expression) selectExpression);
      IQueryable query = table.Provider.CreateQuery((Expression) selectExpression);
      DbCommand command = table.Context.GetCommand(query);
      string commandText = command.CommandText;
      string str1 = commandText.Substring(7, commandText.IndexOf("\r\nFROM ") - 7).Replace("[t0].", "").Replace(" AS [value]", "").Replace("@p", "@p" + bindingName);
      foreach (DbParameter dbParameter in command.Parameters.Cast<DbParameter>())
      {
        string str2 = string.Format("@p{0}", (object) updateCommand.Parameters.Count);
        str1 = str1.Replace(dbParameter.ParameterName.Replace("@p", "@p" + bindingName), str2);
        updateCommand.Parameters.Add((object) new SqlParameter(str2, dbParameter.Value));
      }
      return str1;
    }

    private static string GetDbName<TEntity>(this Table<TEntity> table) where TEntity : class
    {
      Type rowType = typeof (TEntity);
      string str = table.Context.Mapping.GetTable(rowType).TableName;
      if (!str.StartsWith("["))
        str = string.Format("[{0}]", (object) string.Join("].[", str.Split('.')));
      return str;
    }

    public static void EnableSqlCacheDependency<TEntity>(this Table<TEntity> table) where TEntity : class
    {
      Type rowType = typeof (TEntity);
      MetaTable table1 = table.Context.Mapping.GetTable(rowType);
      SqlCacheDependencyAdmin.EnableTableForNotifications(table.Context.Connection.ConnectionString, table1.TableName);
    }

    public static string TableName<TEntity>(this Table<TEntity> table) where TEntity : class
    {
      Type rowType = typeof (TEntity);
      return table.Context.Mapping.GetTable(rowType).TableName;
    }

    public static string TableName(this ITable table)
    {
      return table.Context.Mapping.GetTable(table.GetType().GetGenericArguments()[0]).TableName;
    }

    private static void ValidateExpression(ITable table, Expression expression)
    {
      DataContext context = table.Context;
      object obj = context.GetType().GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object) context, (object[]) null);
      obj.GetType().GetMethod("System.Data.Linq.Provider.IProvider.Compile", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, new object[1]
      {
        (object) expression
      });
    }
  }
}
