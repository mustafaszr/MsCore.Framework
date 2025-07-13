using MsCore.Framework.Repository.Models;
using System.Linq.Expressions;

namespace MsCore.Framework.Repository.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        #region Query Operations

        /// <summary>
        /// Belirtilen id değerine sahip olan nesneyi getirir.
        /// </summary>
        Task<TEntity?> GetByIdAsync(object id);

        /// <summary>
        /// Tüm nesneleri getirir.
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Tüm nesneleri, ilişkili entity'lerle birlikte getirir.
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Belirtilen filtreye uyan nesneleri getirir.
        /// </summary>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Belirtilen filtreye uyan ve ilişkili entity'lerle birlikte nesneleri getirir.
        /// </summary>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Belirtilen filtreye uyan ilk nesneyi getirir. Eğer yoksa null döner.
        /// </summary>
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Belirtilen filtreye uyan ve ilişkili entity'lerle birlikte ilk nesneyi getirir. Eğer yoksa null döner.
        /// </summary>
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Filtreye uyan tek nesneyi getirir. Eğer yoksa null döner. Birden fazla sonuç varsa hata fırlatır.
        /// </summary>
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Filtreye uyan ve ilişkili entity'lerle birlikte tek nesneyi getirir. Eğer yoksa null döner. Birden fazla sonuç varsa hata fırlatır.
        /// </summary>
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        #endregion

        #region Pagination Operations

        /// <summary>
        /// Sayfalama yaparak tüm verileri getirir.
        /// </summary>
        Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Sayfalama yaparak filtre uygulanmış verileri getirir.
        /// </summary>
        Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null);

        /// <summary>
        /// Sayfalama ve sıralama ile filtrelenmiş verileri getirir.
        /// </summary>
        Task<PagedResult<TEntity>> GetPagedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TKey>>? orderBy = null, bool descending = false);

        /// <summary>
        /// Sayfalama, sıralama ve ilişkili entity'lerle birlikte filtrelenmiş verileri getirir.
        /// </summary>
        Task<PagedResult<TEntity>> GetPagedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TKey>>? orderBy = null, bool descending = false, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Queryable üzerinde sayfalama ve sıralama yaparak verileri getirir.
        /// </summary>
        Task<PagedResult<TEntity>> GetPagedAsync<TKey>(int pageNumber, int pageSize, IQueryable<TEntity> query, Expression<Func<TEntity, TKey>>? orderBy = null, bool descending = false);
        #endregion

        #region Command Operations

        /// <summary>
        /// Veritabanına yeni bir nesne ekler.
        /// </summary>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// Veritabanına birden fazla nesneyi toplu olarak ekler.
        /// </summary>
        Task AddRangeAsync(IEnumerable<TEntity> entities);


        /// <summary>
        /// Var olan bir nesneyi günceller.
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Birden fazla nesneyi toplu olarak günceller.
        /// </summary>
        void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Belirtilen nesneyi veritabanından siler.
        /// </summary>
        void Remove(TEntity entity);

        /// <summary>
        /// Birden fazla nesneyi veritabanından toplu olarak siler.
        /// </summary>
        void RemoveRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Belirtilen id değerine sahip nesneyi veritabanından siler.
        /// </summary>
        Task RemoveByIdAsync(object id);

        /// <summary>
        /// Verilen filtreye uyan nesneleri veritabanından toplu olarak siler.
        /// </summary>
        Task RemoveRangeAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Aggregation Operations

        /// <summary>
        /// Toplam kayıt sayısını getirir.
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Belirtilen filtreye göre kayıt sayısını getirir.
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Herhangi bir kayıt var mı kontrol eder.
        /// </summary>
        Task<bool> AnyAsync();

        /// <summary>
        /// Belirtilen filtreye göre herhangi bir kayıt var mı kontrol eder.
        /// </summary>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Verilen alan için en büyük değeri getirir.
        /// </summary>
        Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Verilen alan için en küçük değeri getirir.
        /// </summary>
        Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        #endregion

        #region Advanced Query Operations
        /// <summary>
        /// IQueryable döner, sorguya devam edilmesini sağlar.
        /// </summary>
        IQueryable<TEntity> GetQueryable(params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Belirtilen filtreye ve ilişkili entity'lere göre IQueryable döner.
        /// </summary>
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// SQL sorgusu çalıştırarak sonuçları getirir.
        /// </summary>
        Task<IEnumerable<TEntity>> FromSqlAsync(string sql, params object[] parameters);

        /// <summary>
        /// SQL komutu çalıştırır ve etkilenen satır sayısını döner.
        /// </summary>
        Task<int> ExecuteSqlAsync(string sql, params object[] parameters);

        #endregion

        #region Batch Operations
        /// <summary>
        /// Veritabanına çok sayıda nesneyi hızlıca ekler.
        /// </summary>
        Task BulkInsertAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Veritabanındaki çok sayıda nesneyi hızlıca günceller.
        /// </summary>
        Task BulkUpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Veritabanındaki çok sayıda nesneyi hızlıca siler.
        /// </summary>
        Task BulkDeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Belirtilen filtreye uyan nesneleri hızlıca toplu olarak siler.
        /// </summary>
        Task BulkDeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Projection Operations

        /// <summary>
        /// Seçilen alanlara göre projeksiyon yapar ve sonuçları getirir.
        /// </summary>
        Task<IEnumerable<TResult>> ProjectToAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Filtre uygulayarak ve seçilen alanlara göre projeksiyon yapar ve sonuçları getirir.
        /// </summary>
        Task<IEnumerable<TResult>> ProjectToAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        #endregion
    }
}