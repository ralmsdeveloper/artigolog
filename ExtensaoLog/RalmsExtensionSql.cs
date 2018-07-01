﻿using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Reflection;

namespace ExtensaoLog
{
    public static class RalmsExtensionSql
    {
        private static readonly TypeInfo _queryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo _queryCompiler
            = typeof(EntityQueryProvider)
                .GetTypeInfo()
                .DeclaredFields
                .Single(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo _queryModelGenerator
            = _queryCompilerTypeInfo
                .DeclaredFields
                .Single(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo _database = _queryCompilerTypeInfo
            .DeclaredFields
            .Single(x => x.Name == "_database");

        private static readonly PropertyInfo _dependencies
            = typeof(Database)
                .GetTypeInfo()
                .DeclaredProperties
                .Single(x => x.Name == "Dependencies");

        public static string ToSql<T>(this IQueryable<T> queryable)
            where T : class
        {
            var queryCompiler = _queryCompiler.GetValue(queryable.Provider) as IQueryCompiler;
            var queryModelGen = _queryModelGenerator.GetValue(queryCompiler) as IQueryModelGenerator;
            var queryCompilationContextFactory
                = ((DatabaseDependencies)_dependencies.GetValue(_database.GetValue(queryCompiler)))
                    .QueryCompilationContextFactory;

            var queryCompilationContext = queryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();

            modelVisitor.CreateQueryExecutor<T>(queryModelGen.ParseQuery(queryable.Expression));

            return modelVisitor
                .Queries
                .FirstOrDefault()
                .ToString();
        }
    }
}
