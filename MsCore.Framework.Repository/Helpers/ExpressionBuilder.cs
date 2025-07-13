using System.Linq.Expressions;

namespace MsCore.Framework.Repository.Helpers
{
    public static class ExpressionBuilder
    {
        /// <summary>
        /// Property adı string olarak verilen bir entity için Expression<Func<TEntity, object>> döner. Example: x=>x.Id
        /// </summary>
        public static Expression<Func<TEntity, object>> GetPropertyExpression<TEntity>(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var parameter = Expression.Parameter(typeof(TEntity), "x");

            // Nested property destekler: "Category.Name"
            Expression propertyAccess = parameter;
            foreach (var property in propertyName.Split('.'))
            {
                var propInfo = propertyAccess.Type.GetProperty(property);
                if (propInfo == null)
                    throw new ArgumentException($"Property '{property}' not found on type '{propertyAccess.Type.Name}'");

                propertyAccess = Expression.Property(propertyAccess, propInfo);
            }

            // Boxing yapılır (object dönüş için)
            UnaryExpression convert = Expression.Convert(propertyAccess, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(convert, parameter);
        }
    }
}
