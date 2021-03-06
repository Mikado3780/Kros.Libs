﻿using FluentAssertions;
using Kros.Data;
using Kros.Data.BulkActions;
using Kros.KORM.Data;
using Kros.KORM.Materializer;
using Kros.KORM.Metadata;
using Kros.KORM.Query;
using Kros.KORM.Query.Expressions;
using Kros.KORM.Query.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kros.KORM.UnitTests.Query.Sql
{
    /// <summary>
    /// Base class for Linq translation tests
    /// </summary>
    public abstract class LinqTranslatorTestBase
    {
        //Dátumové funkcie

        /// <summary>
        /// Create query for testing.
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <returns>Query for testing.</returns>
        public IQuery<T> Query<T>() => new Database(new SqlConnection(), new FakeQueryProviderFactory()).Query<T>();

        /// <summary>
        /// Create visitor for translate query to SQL.
        /// </summary>
        protected virtual ISqlExpressionVisitor CreateVisitor() =>
           new DefaultQuerySqlGenerator(Database.DatabaseMapper);

        /// <summary>
        /// Query should be equel to <paramref name="expectedSql"/>.
        /// </summary>
        /// <typeparam name="T">Model type.</typeparam>
        /// <param name="value">Testing query.</param>
        /// <param name="expectedSql">Expected sql query.</param>
        protected void AreSame<T>(IQueryable<T> value, string expectedSql, params object[] parameters)
        {
            var expression = value.Expression;
            AreSame(expectedSql, parameters, expression);
        }

        private void AreSame(string expectedSql, object[] parameters, Expression expression)
        {
            var visitor = CreateVisitor();
            var sql = visitor.GenerateSql(expression);

            sql.Should().BeEquivalentTo(expectedSql);
            parameters.Should().BeEquivalentTo(ParameterExtractor.ExtractParameters(expression));
        }

        /// <summary>
        /// Wases the generated same SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="expectedSql">The expected SQL.</param>
        /// <param name="parameters">The parameters.</param>
        protected void WasGeneratedSameSql<T>(IQuery<T> value, string expectedSql, params object[] parameters)
        {
            var provider = value.Provider as FakeQueryProvider;

            AreSame(expectedSql, parameters, provider.LastExpression.Expression);
        }

        private class ParameterExtractor : ExpressionVisitor
        {
            private List<object> _parameters;

            private ParameterExtractor(List<object> parameters)
            {
                _parameters = parameters;
            }

            public static IEnumerable<object> ExtractParameters(Expression expression)
            {
                var ret = new List<object>();

                (new ParameterExtractor(ret)).Visit(expression);

                return ret;
            }

            public override Expression Visit(Expression node)
            {
                if (node is ArgsExpression expression)
                {
                    VisitArgs(expression);
                    return node;
                }
                else if (node != null)
                {
                    return base.Visit(node.Reduce());
                }
                else
                {
                    return node;
                }
            }

            private void VisitArgs(ArgsExpression argsExpression)
            {
                if (argsExpression.Parameters?.Count() > 0)
                {
                    _parameters.AddRange(argsExpression.Parameters);
                }
            }
        }

        public class FakeQueryProvider : KORM.Query.IQueryProvider
        {
            /// <summary>
            /// Gets the last generated SQL.
            /// </summary>
            public IQueryable LastExpression { get; private set; }

            public DbProviderFactory DbProviderFactory => throw new NotImplementedException();

            public ITransaction BeginTransaction(IsolationLevel isolationLevel)
            {
                throw new NotImplementedException();
            }

            public IBulkInsert CreateBulkInsert()
            {
                throw new NotImplementedException();
            }

            public IBulkUpdate CreateBulkUpdate() => throw new NotImplementedException();

            public DbCommand CreateCommand()
            {
                throw new NotImplementedException();
            }

            public DbConnection CreateConnection()
            {
                throw new NotImplementedException();
            }

            public IIdGenerator CreateIdGenerator(string tableName, int batchSize)
            {
                throw new NotImplementedException();
            }

            public IQueryable CreateQuery(Expression expression)
            {
                throw new NotImplementedException();
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
                => new Query<TElement>(this, expression);

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<T> Execute<T>(IQuery<T> query)
            {
                throw new NotImplementedException();
            }

            public object Execute(Expression expression)
            {
                throw new NotImplementedException();
            }

            public TResult Execute<TResult>(Expression expression)
            {
                var query = new Query<TResult>(this, expression);

                LastExpression = query;

                return default(TResult);
            }

            public void ExecuteInTransaction(Action action)
            {
                throw new NotImplementedException();
            }

            public Task ExecuteInTransactionAsync(Func<Task> action)
            {
                throw new NotImplementedException();
            }

            public int ExecuteNonQuery(string query)
            {
                throw new NotImplementedException();
            }

            public int ExecuteNonQuery(string query, CommandParameterCollection parameters)
            {
                throw new NotImplementedException();
            }

            public int ExecuteNonQueryCommand(IDbCommand command)
            {
                throw new NotImplementedException();
            }

            public Task<int> ExecuteNonQueryCommandAsync(DbCommand command)
            {
                throw new NotImplementedException();
            }

            public object ExecuteScalar<T>(IQuery<T> query)
            {
                throw new NotImplementedException();
            }

            public TResult ExecuteStoredProcedure<TResult>(string storedProcedureName)
            {
                throw new NotImplementedException();
            }

            public TResult ExecuteStoredProcedure<TResult>(string storedProcedureName, CommandParameterCollection parameters)
            {
                throw new NotImplementedException();
            }

            public DbCommand GetCommandForCurrentTransaction()
            {
                throw new NotImplementedException();
            }

            public void SetParameterDbType(DbParameter parameter, string tableName, string columnName)
            {
                throw new NotImplementedException();
            }
        }

        public class FakeQueryProviderFactory : IQueryProviderFactory
        {
            public KORM.Query.IQueryProvider Create(DbConnection connection, IModelBuilder modelBuilder, IDatabaseMapper databaseMapper)
                => new FakeQueryProvider();

            public KORM.Query.IQueryProvider Create(ConnectionStringSettings connectionString, IModelBuilder modelBuilder, IDatabaseMapper databaseMapper)
            {
                throw new NotImplementedException();
            }
        }
    }
}
