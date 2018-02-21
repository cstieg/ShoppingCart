using Cstieg.Sales.Interfaces;
using System;
using System.Data.Entity;

namespace Cstieg.Sales.Repositories
{
    public static class SalesContextExtensions
    {
        public static void Upsert<TEntity>(this ISalesDbContext context, TEntity entity) where TEntity : class, ISalesEntity
        {
            if (IsSavedToDatabase(entity))
            {
                context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                IDbSet<TEntity> dbSet = GetDbSet<TEntity>(context, entity);
                dbSet.Add(entity);
            }
        }

        public static bool IsSavedToDatabase(ISalesEntity entity)
        {
            return entity.Id != 0;
        }

        public static IDbSet<TEntity> GetDbSet<TEntity>(ISalesDbContext context, ISalesEntity entity) where TEntity : class
        {
            foreach (var property in context.GetType().GetProperties())
            {
                if (property.PropertyType.GenericTypeArguments[0] == entity.GetType())
                    return (IDbSet<TEntity>)property.GetValue(context);
            }
            throw new Exception("DbSet not found");
        }

        
    }
}
