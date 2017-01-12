// Decompiled with JetBrains decompiler
// Type: CodeSmith.Data.Linq.DataContextExtensions
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

namespace CodeSmith.Data.Linq
{
  /// <summary>
  /// http://www.aneyfamily.com/terryandann/post/2008/04/LINQ-to-SQL-Batch-UpdatesDeletes-Fix-for-Could-not-translate-expression.aspx
  /// </summary>
  public static class DataContextExtensions
  {
    /// <summary>
    /// Executes a SQL statement against a <see cref="T:System.Data.Linq.DataContext" /> connection.
    /// </summary>
    /// <param name="context">The <see cref="T:System.Data.Linq.DataContext" /> to execute the batch select against.</param>
    /// <param name="command">The DbCommand to execute.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>The DbCommand is not disposed by this call, the caller must dispose the DbCommand.</remarks>
    public static int ExecuteCommand(this DataContext context, DbCommand command)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (command == null)
        throw new ArgumentNullException("command");
      DataContextExtensions.LogCommand(context, command);
      command.Connection = context.Connection;
      if (command.Connection == null)
        throw new InvalidOperationException("The DataContext must contain a valid SqlConnection.");
      if (context.Transaction != null)
        command.Transaction = context.Transaction;
      if (command.Connection.State == ConnectionState.Closed)
        command.Connection.Open();
      return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Batches together multiple <see cref="T:System.Linq.IQueryable" /> queries into a single <see cref="T:System.Data.Common.DbCommand" /> and returns all data in
    /// a single round trip to the database.
    /// </summary>
    /// <param name="context">The <see cref="T:System.Data.Linq.DataContext" /> to execute the batch select against.</param>
    /// <param name="queries">Represents a collections of SELECT queries to execute.</param>
    /// <returns>Returns an <see cref="T:System.Data.Linq.IMultipleResults" /> object containing all results.</returns>
    /// <exception cref="T:System.ArgumentNullException">Thrown when context or queries are null.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown when context.Connection is invalid.</exception>
    public static System.Data.Linq.IMultipleResults ExecuteQuery(this DataContext context, params IQueryable[] queries)
    {
      return context.ExecuteQuery((IEnumerable<IQueryable>) ((IEnumerable<IQueryable>) queries).ToList<IQueryable>());
    }

    /// <summary>
    /// Batches together multiple <see cref="T:System.Linq.IQueryable" /> queries into a single <see cref="T:System.Data.Common.DbCommand" /> and returns all data in
    /// a single round trip to the database.
    /// </summary>
    /// <param name="context">The <see cref="T:System.Data.Linq.DataContext" /> to execute the batch select against.</param>
    /// <param name="queries">Represents a collections of SELECT queries to execute.</param>
    /// <returns>Returns an <see cref="T:System.Data.Linq.IMultipleResults" /> object containing all results.</returns>
    /// <exception cref="T:System.ArgumentNullException">Thrown when context or queries are null.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown when context.Connection is invalid.</exception>
    public static System.Data.Linq.IMultipleResults ExecuteQuery(this DataContext context, IEnumerable<IQueryable> queries)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (queries == null)
        throw new ArgumentNullException("queries");
      List<DbCommand> dbCommandList = new List<DbCommand>();
      foreach (IQueryable query in queries)
        dbCommandList.Add(context.GetCommand(query, true));
      return context.ExecuteQuery((IEnumerable<DbCommand>) dbCommandList);
    }

    /// <summary>
    /// Batches together multiple <see cref="T:System.Linq.IQueryable" /> queries into a single <see cref="T:System.Data.Common.DbCommand" /> and returns all data in
    /// a single round trip to the database.
    /// </summary>
    /// <param name="context">The <see cref="T:System.Data.Linq.DataContext" /> to execute the batch select against.</param>
    /// <param name="commands">The list of commands to execute.</param>
    /// <returns>Returns an <see cref="T:System.Data.Linq.IMultipleResults" /> object containing all results.</returns>
    /// <exception cref="T:System.ArgumentNullException">Thrown when context or queries are null.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown when context.Connection is invalid.</exception>
    public static System.Data.Linq.IMultipleResults ExecuteQuery(this DataContext context, IEnumerable<DbCommand> commands)
    {
      using (DbCommand cmd = DataContextExtensions.CombineCommands(context, commands))
      {
        DataContextExtensions.LogCommand(context, cmd);
        cmd.Connection = context.Connection;
        if (cmd.Connection == null)
          throw new InvalidOperationException("The DataContext must contain a valid SqlConnection.");
        if (context.Transaction != null)
          cmd.Transaction = context.Transaction;
        DbDataReader reader;
        if (cmd.Connection.State == ConnectionState.Closed)
        {
          cmd.Connection.Open();
          reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        else
          reader = cmd.ExecuteReader();
        if (reader == null)
          return (System.Data.Linq.IMultipleResults) null;
        return context.Translate(reader);
      }
    }

    /// <summary>
    /// Provides information about SQL commands generated by LINQ to SQL.
    /// </summary>
    /// <param name="context">The DataContext to get the command from.</param>
    /// <param name="query">The query whose SQL command information is to be retrieved.</param>
    /// <param name="isForTranslate">if set to <c>true</c>, adjust the sql command to support calling DataContext.Translate().</param>
    /// <returns></returns>
    public static DbCommand GetCommand(this DataContext context, IQueryable query, bool isForTranslate)
    {
      DbTransaction transaction = context.Transaction;
      context.Transaction = (DbTransaction) null;
      DbCommand command = context.GetCommand(query);
      if (transaction != null && transaction.Connection != null)
      {
        command.Transaction = transaction;
        context.Transaction = transaction;
      }
      if (!isForTranslate)
        return command;
      System.Data.Linq.Mapping.MetaType metaType = context.Mapping.GetMetaType(query.ElementType);
      if (metaType != null && metaType.IsEntity)
        command.CommandText = DataContextExtensions.RemoveColumnAlias(command.CommandText, metaType);
      return command;
    }

    public static DbCommand GetCommand(this DataContext dataContext, Expression expression)
    {
      object provider = DataContextExtensions.GetProvider(dataContext);
      if (provider == null)
        throw new InvalidOperationException("Failed to get the DataContext provider instance.");
      Type type = provider.GetType().GetInterface("IProvider");
      if (type == (Type) null)
        throw new InvalidOperationException("Failed to cast the DataContext provider to IProvider.");
      MethodInfo method = type.GetMethod("GetCommand", BindingFlags.Instance | BindingFlags.Public);
      if (method == (MethodInfo) null)
        throw new InvalidOperationException("Failed to get the GetCommand method from the DataContext provider.");
      return method.Invoke(provider, new object[1]{ (object) expression }) as DbCommand;
    }

    /// <summary>Starts a database transaction.</summary>
    /// <param name="dataContext">The data context.</param>
    /// <returns>An object representing the new transaction.</returns>
    public static DbTransaction BeginTransaction(this DataContext dataContext)
    {
      return dataContext.BeginTransaction(IsolationLevel.Unspecified);
    }

    /// <summary>
    /// Starts a database transaction with the specified isolation level.
    /// </summary>
    /// <param name="dataContext">The data context.</param>
    /// <param name="isolationLevel">The isolation level for the transaction.</param>
    /// <returns>An object representing the new transaction.</returns>
    public static DbTransaction BeginTransaction(this DataContext dataContext, IsolationLevel isolationLevel)
    {
      if (dataContext == null)
        throw new ArgumentNullException("dataContext");
      if (dataContext.Connection.State == ConnectionState.Closed)
        dataContext.Connection.Open();
      DbTransaction dbTransaction = dataContext.Connection.BeginTransaction(isolationLevel);
      dataContext.Transaction = dbTransaction;
      return dbTransaction;
    }

    /// <summary>
    /// Combines multiple SELECT commands into a single <see cref="T:System.Data.SqlClient.SqlCommand" /> so that all statements can be executed in a
    /// single round trip to the database and return multiple result sets.
    /// </summary>
    /// <param name="context">The DataContext to get the command from.</param>
    /// <param name="selectCommands">Represents a collection of commands to be batched together.</param>
    /// <returns>
    /// Returns a single <see cref="T:System.Data.SqlClient.SqlCommand" /> that executes all SELECT statements at once.
    /// </returns>
    private static DbCommand CombineCommands(DataContext context, IEnumerable<DbCommand> selectCommands)
    {
      DbCommand command = context.Connection.CreateCommand();
      DbParameterCollection parameters1 = command.Parameters;
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (DbCommand selectCommand in selectCommands)
      {
        if (num > 0)
          stringBuilder.AppendLine();
        stringBuilder.AppendFormat("-- Query #{0}", (object) (num + 1));
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
        DbParameterCollection parameters2 = selectCommand.Parameters;
        for (int index = parameters2.Count - 1; index >= 0; --index)
        {
          DbParameter src = parameters2[index];
          DbParameter dbParameter = DataContextExtensions.CloneParameter(src);
          string newValue = src.ParameterName.Replace("@", string.Format("@q{0}", (object) num));
          selectCommand.CommandText = selectCommand.CommandText.Replace(src.ParameterName, newValue);
          dbParameter.ParameterName = newValue;
          parameters1.Add((object) dbParameter);
        }
        stringBuilder.Append(selectCommand.CommandText.Trim());
        stringBuilder.AppendLine(";");
        ++num;
      }
      command.CommandText = stringBuilder.ToString();
      return command;
    }

    /// <summary>
    /// Returns a clone (via copying all properties) of an existing <see cref="T:System.Data.Common.DbParameter" />.
    /// </summary>
    /// <param name="src">The <see cref="T:System.Data.Common.DbParameter" /> to clone.</param>
    /// <returns>Returns a clone (via copying all properties) of an existing <see cref="T:System.Data.Common.DbParameter" />.</returns>
    private static DbParameter CloneParameter(DbParameter src)
    {
      SqlParameter sqlParameter1 = src as SqlParameter;
      if (sqlParameter1 == null)
        return src;
      SqlParameter sqlParameter2 = new SqlParameter();
      sqlParameter2.Value = sqlParameter1.Value;
      sqlParameter2.Direction = sqlParameter1.Direction;
      sqlParameter2.Size = sqlParameter1.Size;
      sqlParameter2.Offset = sqlParameter1.Offset;
      sqlParameter2.SourceColumn = sqlParameter1.SourceColumn;
      sqlParameter2.SourceVersion = sqlParameter1.SourceVersion;
      sqlParameter2.SourceColumnNullMapping = sqlParameter1.SourceColumnNullMapping;
      sqlParameter2.IsNullable = sqlParameter1.IsNullable;
      sqlParameter2.CompareInfo = sqlParameter1.CompareInfo;
      sqlParameter2.XmlSchemaCollectionDatabase = sqlParameter1.XmlSchemaCollectionDatabase;
      sqlParameter2.XmlSchemaCollectionOwningSchema = sqlParameter1.XmlSchemaCollectionOwningSchema;
      sqlParameter2.XmlSchemaCollectionName = sqlParameter1.XmlSchemaCollectionName;
      sqlParameter2.UdtTypeName = sqlParameter1.UdtTypeName;
      sqlParameter2.TypeName = sqlParameter1.TypeName;
      sqlParameter2.ParameterName = sqlParameter1.ParameterName;
      sqlParameter2.Precision = sqlParameter1.Precision;
      sqlParameter2.Scale = sqlParameter1.Scale;
      return (DbParameter) sqlParameter2;
    }

    private static string RemoveColumnAlias(string sql, System.Data.Linq.Mapping.MetaType metaType)
    {
      int num = sql.IndexOf("\r\nFROM ");
      string str1 = sql.Substring(0, num);
      string str2 = sql.Substring(num);
      foreach (MetaDataMember persistentDataMember in metaType.PersistentDataMembers)
      {
        if (!persistentDataMember.IsAssociation && !(persistentDataMember.Name == persistentDataMember.MappedName))
        {
          string oldValue = string.Format("[{0}] AS [{1}]", (object) persistentDataMember.MappedName, (object) persistentDataMember.Name);
          string newValue = string.Format("[{0}]", (object) persistentDataMember.MappedName);
          str1 = str1.Replace(oldValue, newValue);
        }
      }
      return str1 + str2;
    }

    internal static void LogCommand(DataContext context, DbCommand cmd)
    {
      if (context == null || context.Log == null || cmd == null)
        return;
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
      object provider = DataContextExtensions.GetProvider(context);
      if (provider == null)
        return;
      Type type = provider.GetType();
      MethodInfo method;
      for (method = type.GetMethod("LogCommand", bindingAttr); method == (MethodInfo) null && type.BaseType != (Type) null; method = type.GetMethod("LogCommand", bindingAttr))
        type = type.BaseType;
      if (method == (MethodInfo) null)
        return;
      method.Invoke(provider, new object[2]
      {
        (object) context.Log,
        (object) cmd
      });
    }

    private static object GetProvider(DataContext dataContext)
    {
      PropertyInfo property = dataContext.GetType().GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
      if (property == (PropertyInfo) null)
        return (object) null;
      return property.GetValue((object) dataContext, (object[]) null);
    }
  }
}
