using System;
using System.Linq.Expressions;
using System.Reflection;
using Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Core.DomainServices.QueryExtensions
{
    public static class SoftDeleteQueryExtension
    {
        public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityData)
        {
            var methodToCall = typeof(SoftDeleteQueryExtension)
                .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(entityData.ClrType);

            if (methodToCall is not null)
            {
                var filter = methodToCall.Invoke(null, Array.Empty<object>());
                entityData.SetQueryFilter((LambdaExpression) filter);
            }
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : class, IBaseEntitySoftDeletes
        {
            Expression<Func<TEntity, bool>> filter = x => x.DeletedAt == null;

            return filter;
        }
    }
}