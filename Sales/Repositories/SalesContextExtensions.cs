using Cstieg.Sales.Interfaces;
using System.Data.Entity;

namespace Cstieg.Sales.Repositories
{
    public static class SalesContextExtensions
    {
        public static void Upsert(this ISalesDbContext context, ISalesEntity entity)
        {
            if (IsSavedToDatabase(context, entity))
            {
                context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                context.Entry(entity).State = EntityState.Added;
            }
        }

        public static bool IsSavedToDatabase(this ISalesDbContext context, ISalesEntity entity)
        {
            return entity.Id != 0;
        }
    }
}
