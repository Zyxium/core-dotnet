using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using Npgsql.NameTranslation;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Core.DotNet.AggregatesModel.CommonAggregate;

namespace Core.DotNet.Extensions.DbContext;

public static class ModelBuilderExtensions
{
    private static readonly Regex KeysRegex = new Regex("^(PK|FK|IX)_", RegexOptions.Compiled);

    public static void ApplyGlobalFiltersSoftDeleted(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var foundProperty = entityType.FindProperty("DeletedAt");
            if (foundProperty != null && foundProperty.ClrType == typeof(DateTime?))
            {
                var parameter = Expression.Parameter(entityType.ClrType);
                var filter = Expression.Lambda(
                    Expression.Equal(Expression.Property(parameter, foundProperty.PropertyInfo), Expression.Constant(null)),
                    parameter
                );
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    public static void UseSnakeCaseNames(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        var caseNameTranslator = new NpgsqlSnakeCaseNameTranslator();
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            switch (databaseType)
            {
                case DatabaseType.SQLServer:
                    break;
                case DatabaseType.PostgreSQL:
                {
                    ModelBuilderExtensions.PostgreSQLConvertToSnake((INpgsqlNameTranslator)caseNameTranslator, (object)entityType);
                    foreach (IMutableProperty property in entityType.GetProperties())
                        ModelBuilderExtensions.PostgreSQLConvertToSnake((INpgsqlNameTranslator)caseNameTranslator, (object)property);
                    foreach (IMutableKey key in entityType.GetKeys())
                        ModelBuilderExtensions.PostgreSQLConvertToSnake((INpgsqlNameTranslator)caseNameTranslator, (object)key);
                    foreach (IMutableForeignKey foreignKey in entityType.GetForeignKeys())
                        ModelBuilderExtensions.PostgreSQLConvertToSnake((INpgsqlNameTranslator)caseNameTranslator, (object)foreignKey);
                    foreach (IMutableIndex index in entityType.GetIndexes())
                        ModelBuilderExtensions.PostgreSQLConvertToSnake((INpgsqlNameTranslator)caseNameTranslator, (object)index);
                }
                    break;
                case DatabaseType.MongoDb:
                    break;
                case DatabaseType.MySQL:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }
        }
    }

    #region PostgreSQL

    private static void SQLServerConvertToSnake(object entity)
    {
        
    }

    private static void PostgreSQLConvertToSnake(INpgsqlNameTranslator mapper, object entity)
    {
        switch (entity)
        {
            case IMutableEntityType table:
                table.SetTableName(ConvertGeneralToSnake(mapper, table.GetTableName()));
                break;
            case IMutableProperty property:
                property.SetColumnName(ConvertGeneralToSnake(mapper, property.GetColumnBaseName()));
                break;
            case IMutableKey primaryKey:
                primaryKey.SetName(ConvertKeyToSnake(mapper, primaryKey.GetName()));
                break;
            case IMutableForeignKey foreignKey:
                foreignKey.SetConstraintName(ConvertKeyToSnake(mapper, foreignKey.GetConstraintName()));
                break;
            case IMutableIndex indexKey:
                indexKey.SetDatabaseName(ConvertKeyToSnake(mapper, indexKey.GetDatabaseName()));
                break;
            default:
                throw new NotImplementedException("Unexpected type was provided to snake case converter");
        }
    }

    private static string ConvertKeyToSnake(INpgsqlNameTranslator mapper, string keyName) =>
        ConvertGeneralToSnake(mapper, KeysRegex.Replace(keyName, match => match.Value.ToLower()));

    private static string ConvertGeneralToSnake(INpgsqlNameTranslator mapper, string entityName) =>
        mapper.TranslateMemberName(entityName);

    #endregion PostgreSQL
    

}