using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MsCore.Framework.Repository.Interfaces;
using MsCore.Framework.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Generic Repository ve UnitOfWork yapılarını DI konteynırına ekler.
        /// </summary>
        public static IServiceCollection AddMsCoreRepository<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddScoped<DbContext, TContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}
