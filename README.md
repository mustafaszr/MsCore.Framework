# MsCore.Framework

**MsCore.Framework** .NET tabanlı uygulamalar için tasarlanmış, sade, genişletilebilir ve modüler yapıya sahip bir temel framework setidir. Tekrar eden altyapı kodlarını minimize etmeyi, geliştiricilerin iş mantığına odaklanmasını sağlamayı amaçlar. Aşağıdaki modüllerden oluşur:

- **MsCore.Framework** – Tüm alt modülleri bir arada sunan çekirdek pakettir. Yalnızca bu paketin projeye dahil edilmesiyle tüm bileşenler aktif hale gelir. API yanıtlarını tutarlı hale getiren standart bir response modeli içerir. ValidationException gibi doğrulama hatalarını otomatik olarak yakalayıp bu standart formata dönüştüren middleware sunar. Ayrıca global exception middleware sayesinde sistem genelindeki beklenmeyen hatalar merkezi olarak ele alınır ve aynı response yapısı ile yönetilir. Bu yaklaşım, API katmanında uniform bir yanıt yapısı sağlayarak client tarafında entegrasyon sürecini sadeleştirir ve hata yönetimini öngörülebilir hale getirir.

- **MsCore.Framework.Logging** – Kapsamlı HTTP request/response izleme ve loglama altyapısı. HTTP context extension methodları ile request body okuma, request/response logger middleware, dependency injection pattern ile entegre File ve Database logger implementasyonları (concurrent kullanım desteği), hazır LogEntity modeli ile mevcut DbContext entegrasyonu veya dedicated logging context desteği sunar.

- **MsCore.Framework.Repository** – Veri erişim katmanını sadeleştirmek ve standart hale getirmek amacıyla geliştirilmiş, genişletilebilir bir Generic Repository ve Unit of Work altyapısı sunar. CRUD, filtreleme, sayfalama, sıralama ve toplu işlemler gibi birçok operasyonu tek bir yapı altında toplayarak, tekrar eden kodları ortadan kaldırır. Dependency Injection ile kolayca entegre edilir, dinamik sorgu oluşturma ve performanslı veri yönetimi için hazır yardımcı bileşenler içerir.

- **MsCore.Framework.Utilities** – Genel yardımcı işlevleri merkezi bir yapıda sunar. Collection, DateTime, int ve string işlemleri için pratik extension metodları; dosya işlemleri, HTTP istekleri, caching ve IConfiguration okuma gibi yaygın senaryolar için kullanıma hazır helper sınıfları içerir.

### 🎯 Hedef .NET Sürümü

**MsCore.Framework**, .NET 8 ile tam uyumludur.

## 🔧 Kurulum

NuGet üzerinden yüklemek için:

```bash
dotnet add package MsCore.Framework
```

Alt Modül Kurulumları:

```bash
# Logging işlemleri için gerekli modül
dotnet add package MsCore.Logging
```

```bash
# Veri erişim katmanı için Repository desenini uygulayan modül
dotnet add package MsCore.Repository
```

```bash
# Yardımcı araçlar ve genel kullanım için çeşitli yardımcı fonksiyonları içeren modül
dotnet add package MsCore.Utilities
```

# 🧩 MsCore.Framework

## 🔹 MsApiResponse

MsCore.Framework'ün standart API yanıt yapısını sağlayan merkezi response modeli. Tüm API endpoint'lerinde tutarlı response formatı oluşturur ve hata yönetimini standartlaştırır.

### `MsApiResponse<T>`

Generic response sınıfı, herhangi bir veri tipini destekler:

```csharp
public class MsApiResponse<T> where T : class
{
    public T? Data { get; set; }                    // Generic data
    public HttpStatusCode StatusCode { get; set; }  // HTTP durum kodu
    public bool IsSuccessful { get; set; }          // İşlem başarı durumu
    public MsErrorDto? Error { get; set; }          // Hata bilgileri
    public string? Message { get; set; }            // Açıklama mesajı
}
```

### `MsApiResponse`

Veri içermeyen response'lar için:

```csharp
public class MsApiResponse : MsApiResponse<NoContentDto>
{
    // NoContentDto ile temel response yapısını miras alır
}
```

### `MsErrorDto`

Hata bilgilerini standardize eden model:

```csharp
public class MsErrorDto
{
    public List<string> Errors { get; set; }   // Hata mesajları listesi
    public bool IsShow { get; set; }           // Kullanıcıya gösterim durumu

    // Constructors
    public MsErrorDto(List<string> errors, bool isShow = true)
    public MsErrorDto(string error, bool isShow = true)
}
```

## 🔹 MsApiResponseFactory

Response objelerini oluşturmak için static factory metodları:

```csharp
using System.Net;
using MsCore.Framework.Models.Responses;

namespace MsCore.Framework.Factories
{
    public static class MsApiResponseFactory
    {
        public static MsApiResponse Success(HttpStatusCode statusCode, string message)
        {
            return new MsApiResponse { StatusCode = statusCode, IsSuccessful = true, Message = message };
        }

        public static MsApiResponse Fail(MsErrorDto errorDto, HttpStatusCode statusCode)
        {
            return new MsApiResponse { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }

        public static MsApiResponse Fail(string error, HttpStatusCode statusCode, bool isShow = true)
        {
            var errorDto = new MsErrorDto(error, isShow);
            return new MsApiResponse { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }

        public static MsApiResponse<T> Success<T>(T data, HttpStatusCode statusCode, string message) where T : class
        {
            return new MsApiResponse<T> { Data = data, StatusCode = statusCode, IsSuccessful = true, Message = message };
        }

        public static MsApiResponse<T> Fail<T>(MsErrorDto errorDto, HttpStatusCode statusCode) where T : class
        {
            return new MsApiResponse<T> { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }

        public static MsApiResponse<T> Fail<T>(string error, HttpStatusCode statusCode, bool isShow = true) where T : class
        {
            var errorDto = new MsErrorDto(error, isShow);
            return new MsApiResponse<T> { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }
    }
}
```

### 💡 Kullanım

```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return Ok(MsApiResponseFactory.Fail<User>("Kullanıcı bulunamadı", HttpStatusCode.NotFound));
        }

        return Ok(MsApiResponseFactory.Success<User>(user, HttpStatusCode.OK, "Kullanıcı başarıyla getirildi"));
    }

    [HttpPost]
    public async Task<MsApiResponse> CreateUser(CreateUserRequest request)
    {
        await _userService.CreateAsync(request);

        return MsApiResponseFactory.Success(HttpStatusCode.Created, "Kullanıcı başarıyla oluşturuldu");
    }
}
```

## 🔹 MsValidationMiddleware

`MsValidationMiddleware`, API istekleri sırasında oluşan doğrulama hatalarını merkezi ve tutarlı şekilde yönetmek için geliştirilmiş bir ara katmandır (middleware). Uygulama içinde ValidationException fırlatıldığında, bu middleware devreye girer ve hataları MsApiResponse formatına dönüştürerek client tarafına anlamlı ve düzenli bir şekilde iletir.

### 🔧 Kurulum

Program.cs dosyanıza aşağıdaki satırı eklemeniz yeterlidir

```csharp
app.UseMsValidationMiddleware();
```

#### ❓Nasıl Çalışır

- İstek pipeline’ında yer alır.
- Controller veya servislerdeki model doğrulama sırasında `ValidationException` atıldığında yakalar.
- Hata detaylarını toplar, MsApiResponse formatına çevirir.
- HTTP response olarak döner, böylece client tarafı tutarlı ve öngörülebilir hata mesajları alır.

## 🔹 MsGlobalExceptionMiddleware

Uygulama içerisinde beklenmeyen (handle edilmemiş) tüm hataları merkezi olarak yakalayan global hata yönetim katmanıdır. Fırlatılan tüm Exception türlerini ele alarak, detaylarını loglar ve kullanıcıya MsApiResponse formatında tutarlı bir yanıt döner. Böylece sistem genelindeki hatalar kontrol altına alınır ve client tarafına güvenli, anlamlı hata mesajları iletilmiş olur.

### 🔧 Kurulum

#### ‼️Çalışması için mutlaka <a>`MsCoreLogger`</a> DI kaydı yapılmalıdır

Program.cs dosyanıza aşağıdaki satırı eklemeniz ve MsCoreLogger di kaydını gerçekleştirmeniz gerekmektedir.

```csharp
app.UseMsGlobalExceptionMiddleware();
```

#### ❓Nasıl Çalışır

- Middleware, tüm pipeline'ı saran bir katman olarak çalışır.
- Controller ya da servislerde beklenmeyen bir Exception oluştuğunda otomatik olarak devreye girer.
- Hata detaylarını (stack trace, mesaj vb.) loglar (eğer MsCore.Logging entegre ise).
- Kullanıcıya, IsSuccessful = false, StatusCode = 500 ve detayları içeren bir MsApiResponse döner.
- Gereksiz exception bilgileri gizlenerek güvenlik ve kullanıcı deneyimi korunur.

# 🧩 MsCore.Framework.Logging

MsCore.Framework.Logging, HTTP istek ve yanıtlarını kapsamlı şekilde izleyip loglamak için geliştirilmiş bir modüldür. Uygulamadaki tüm request/response trafiğini yakalayarak hata takibi, performans analizi ve denetim (audit) amacıyla kullanılabilir. Modül, dependency injection desteğiyle hem dosya hem de veritabanı tabanlı loglama yapabilen, genişletilebilir ve eşzamanlı (concurrent) kullanıma uygun bir altyapı sunar.

## 🔹 MsRequestLoggerMiddleware

MsRequestLoggerMiddleware, gelen HTTP isteklerini ve sunucudan dönen yanıtları uygulama pipeline’ı üzerinde yakalayarak loglayan bir ara katmandır. Tüm istek ve yanıt içeriklerini (body dahil) okuyarak, seçilen log altyapısına (dosya veya veritabanı) kaydeder. Bu sayede hataların izlenmesi, performans takibi ve denetim süreçleri için tam kapsamlı loglama sağlanır.

### 🔧 Kurulum

#### ‼️Çalışması için mutlaka <a>`MsCoreLogger`</a> DI kaydı yapılmalıdır

Program.cs dosyanıza aşağıdaki satırı eklemeniz ve MsCoreLogger di kaydını gerçekleştirmeniz gerekmektedir.

```csharp
app.UseMsCoreRequsetLoggerMiddleware();
```

## 🔹 IMsLoggerService

IMsLoggerService, uygulama genelinde kullanılacak merkezi loglama servisinin arayüzüdür. Hem dosya hem de veritabanı loglama sistemlerini aynı çatı altında birleştirir. MsCompositeLogger sınıfı bu arayüzü implemente ederek çoklu log hedeflerini (file + db) destekler.

### 🔧 Kurulum

```csharp
// MyDbContext yerine LogEntity'nin eklendiği DbContext verilmelidir.
services.AddMsCoreLogger<MyDbContext>(options =>
{
    options.UseFileLogger = true; // Dosyaya loglama aktifleştirilir
    options.UseDatabaseLogger = true; // Veritabanına loglama aktifleştirilir
    options.DirectoryPath = "C:\\Logs"; // Log dosyalarının yazılacağı klasör yolu
    options.FileName = "applog"; // Log dosyasının adı (uzantı otomatik eklenir)
    options.RotationType = RotationType.Daily; // Dosya döngüleme tipi günlük olarak ayarlanır
});
```

### IMsLoggerService

```csharp
public interface IMsLoggerService
{
    Task LogRequestAsync(LogEntityDto log);
    Task LogResponseAsync(LogEntityDto log);
    Task LogInfoAsync(LogEntityDto log);
    Task LogWarningAsync(LogEntityDto log);
    Task LogErrorAsync(LogEntityDto log);
}
```

### 💡 Kullanım

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMsLoggerService _logger;

    public AuthController(IMsLoggerService logger)
    {
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            await _logger.LogInfoAsync(new LogEntityDto()); // LogEntityDto ihtiyaca göre doldurulmalıdır.

            return Ok("Login başarılı");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(new LogEntityDto()); // LogEntityDto ihtiyaca göre doldurulmalıdır.
            return StatusCode(500, "Bir hata oluştu.");
        }
    }
}
```

## 🔹 LogEntity

LogEntity uygulama içerisindeki log kayıtlarının veri modelidir. Veritabanına kayıt yapılabilmesi için, istenilen DbContext’e DbSet<LogEntity> olarak eklenmesi ve AddMsCoreLogger metoduna bu DbContext'in generic parametre olarak gönderilmesi gerekir. Böylece Entity Framework üzerinden log kayıtları kolayca yönetilebilir ve saklanabilir.

```csharp
public class LogEntity
{
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public LogTypeEnum? LogType { get; set; }
    public string? Error { get; set; }
    public string? Detail { get; set; }
    public string? HttpMethod { get; set; }
    public string? Path { get; set; }
    public string? User { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public string? QueryString { get; set; }
    public long? ElapsedMs { get; set; }
}
```

### 💡 Kullanım

```csharp
using Microsoft.EntityFrameworkCore;
using MsCore.Framework.Logging.Models;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<LogEntity> AppLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
```

## 🔹 HttpContextExtensions

HttpContextExtensions sınıfı, HttpContext nesnesi üzerinde genişletme metodu (extension method) sağlar.

```csharp
public static async Task<string> ReadRequestBodyAsync(this HttpContext context)
{
    string body = string.Empty;

    if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
    {
        return body;
    }

    context.Request.EnableBuffering();

    if (context.Request.Body.CanSeek)
    {
        context.Request.Body.Position = 0;
    }

    using var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);

    body = await reader.ReadToEndAsync();

    if (context.Request.Body.CanSeek)
    {
        context.Request.Body.Position = 0;
    }

    return body;
}
```

# 🧩 MsCore.Framework.Repository

MsCore.Framework.Repository, veri erişim katmanını sadeleştirmek ve standartlaştırmak amacıyla geliştirilmiş bir repository altyapısıdır. Tekrarlayan CRUD işlemlerini soyutlayarak, iş mantığının veri erişim detaylarından ayrılmasını sağlar.
Generic Repository ve Unit of Work desenlerini temel alarak genişletilebilir ve test edilebilir bir yapı sunar.

### 🔧 Kurulum

```csharp
// Uygulamanızın DbContext'i verilmelidir.
builder.Services.AddMsCoreRepository<TestDbContext>();
```

```csharp
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
```

## 🔹 IUnitOfWork

IUnitOfWork pattern'i, birden fazla repository işlemini tek bir veritabanı transaction'ı altında yönetmeyi sağlar. Bu sayede business logic'inizde yapılan tüm değişiklikler ya hep birlikte commit edilir ya da hata durumunda rollback yapılır, böylece data consistency korunur.

```csharp
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

## 🔹 IGenericRepository

IGenericRepository, tüm entity'ler için ortak CRUD operasyonlarını soyutlayan generic bir arayüzdür. Bu pattern sayesinde her entity için ayrı repository yazmak yerine, tek bir generic implementation ile tüm temel database işlemlerini (Create, Read, Update, Delete) gerçekleştirebilirsiniz.

```csharp
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
```

## 🔹 PagedRequestDto & PagedResult

Modül içerisinde büyük veri kümeleri üzerinde çalışan servislerde sıklıkla ihtiyaç duyulan sayfalama (pagination) ve filtreleme işlemleri için kullanılmak üzere iki temel yapı sağlanmıştır: PagedRequestDto ve PagedResult<T>.

Bu yapılar; frontend ile backend arasındaki veri alışverişinde tutarlılığı artırmak, performanslı veri çekimini kolaylaştırmak ve tekrar eden pagination/response formatı kodlarını soyutlayarak geliştiriciyi sade bir kullanım modeline yönlendirmek amacıyla tasarlanmıştır.

### PagedRequestDto

```csharp
public class PagedRequestDto
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortColumnName { get; set; }
    public bool? OrderByDescending { get; set; } = false;
    public List<PagedRequestFilterDto>? Filters { get; set; }

    public PagedRequestDto()
    {

    }

    public PagedRequestDto(int pageNumgber, int pageSize, string? sortColumnName = null, bool? orderByDescendin = null,List<PagedRequestFilterDto>? filters = null)
    {
        PageNumber = pageNumgber;
        PageSize = pageSize;
        SortColumnName = sortColumnName;
        OrderByDescending = orderByDescendin;
        Filters = filters;
    }
}
selector);
```

### PagedResult

```csharp
public class PagedResult<T>
{
    public IReadOnlyList<T>? Data { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }

    public PagedResult()
    {

    }

    public PagedResult(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
    {
        Data = data is IReadOnlyList<T> readOnlyList ? readOnlyList : data.ToList().AsReadOnly();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);
        HasPreviousPage = PageNumber > 1;
        HasNextPage = PageNumber < TotalPages;
    }
}
```

## 🔹 ExpressionBuilder

ExpressionBuilder, çalışma zamanında (runtime) dinamik olarak property adı üzerinden LINQ expression oluşturmaya yarayan yardımcı bir sınıftır. Sıralama veya filtreleme işlemleri gibi durumlarda, property adının string olarak bilindiği senaryolarda kullanılır.

```csharp
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
```

# 🧩 MsCore.Framework.Utilities

MsCore.Framework.Utilities, .NET projelerinde yaygın olarak ihtiyaç duyulan işlevsellikleri merkezi bir yapıda toplayan yardımcı bir kütüphanedir. Bu katman, koleksiyon, tarih, sayı ve string türlerine ait özelleştirilmiş extension method'lar, HTTP istekleri, dosya işlemleri, konfigürasyon ve cache gibi işlemleri kolaylaştıran statik yardımcı sınıflar içerir

Bu yapı sayesinde:

- ✅Kod tekrarı azaltılır,
- ✅Geliştirme süreci hızlanır,
- ✅Sınıflar arası ayrım ve test edilebilirlik artar.

**Extensions/**  
 Koleksiyonlar, tarih, sayı ve string türlerine ait pratik extension method’ları içerir. Kod okunabilirliğini artırır.

**Helpers/**  
 Uygulama genelinde tekrar eden işlemler (cache, config, http, dosya yönetimi gibi) için statik yardımcı sınıflar içerir.

**Interfaces/**  
 Yardımcı sınıfların dışa bağımlılıklarını soyutlayan arayüzleri tanımlar.

**Providers/**  
 Belirli yardımcı sınıfların (örneğin HTTP) provider implementasyonlarını içerir. IoC container ile kullanılmaya uygundur.

## 🔹 IMsHttpHelper

IMsHttpHelper, uygulama içinde dış servislerle HTTP tabanlı iletişim kurmak için kullanılan yardımcı servislerin soyutlanmasını sağlar. Özellikle GET, POST, PUT, DELETE gibi temel HTTP işlemlerini generic ve asenkron olarak tanımlayan method imzalarını içerir.

🎯 Desteklenen Özellikler:
| Özellik | Açıklama |
| -------------------------- | ------------------------------------------------------- |
| `Authorization: Bearer` | Opsiyonel token desteği ile header'a eklenir. |
| Custom headers | Dictionary olarak gönderilebilir. |
| JSON serialize/deserialize | `System.Text.Json` ile yapılır. |
| Generic dönüş tipi | Tüm dönüşler `TResponse` olarak strongly-typed yapılır. |
| Exception handling | Deserialize hatalarında özel exception fırlatılır. |

### 🔧 Kurulum

Program.cs dosyanıza aşağıdaki satırı eklemeniz yeterlidir. Ardından IMsHttpHelper arayüzü, ihtiyaç duyduğunuz servis veya controller sınıflarında Dependency Injection (DI) yoluyla kullanılabilir.

```csharp
builder.Services.AddMsCoreHttpHelper();
```

### 💡 Kullanım

```csharp
[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly IMsHttpHelper _msHttpHelper;
    public TestController(IMsHttpHelper msHttpHelper)
    {
        _msHttpHelper = msHttpHelper;
    }

    [HttpGet]
    public async Task<IActionResult> TestHttpAsync()
    {
        var filters = new List<PagedRequestFilterDto>
        {
            new PagedRequestFilterDto("Name", "Ay")
        };

        PagedRequestDto data = new PagedRequestDto(1, 10, "Name", false, filters);

        return Ok(await _msHttpHelper.PostAsync<PagedRequestDto, MsApiResponse<PagedResult<TestEntity>>>("http://localhost:5102/WeatherForecast/TestGenericRepositoryGetTable", data));
    }
}

```

## 🔹 Extensions

.NET'in temel veri türleri üzerinde çalışmayı kolaylaştıran çeşitli extension method'ları içerir. Bu sınıflar sayesinde koleksiyonlar, tarih-saat değerleri, sayılar ve string ifadeler üzerinde daha okunabilir ve pratik işlemler yapılabilir.

Detay için ilgili extension sınıflarını inceleyebilir; hangi methodların ne işe yaradığını doğrudan görerek uygulamanıza entegre edebilirsiniz.

- ✅ CollectionExtensions
- ✅ DateTimeExtensions
- ✅ NumericExtensions
- ✅ StringExtensions

## 🔹 Helpers

Uygulama genelinde sıkça ihtiyaç duyulan işlemleri merkezi ve yeniden kullanılabilir şekilde sunan statik yardımcı sınıfları içerir. Bu sınıflar, dosya işlemlerinden cache yönetimine, yapılandırma okuma işlemlerine kadar birçok farklı senaryoda geliştirme sürecini hızlandırır.

- ✅ MsCacheManager
- ✅ MsConfigurationHelper
- ✅ MsFileHelper

# 📦 Özellikler

- ✅ MsApiResponse – Standart API response modeli (Data, StatusCode, Message, Error), tüm katmanlarda uyumlu kullanım
- ✅ MsApiResponseFactory – Başarılı ve başarısız yanıtları kolay oluşturmak için hazır factory metotları
- ✅ FluentValidation uyumlu MsValidationMiddleware – ValidationException'ları yakalayarak tutarlı MsApiResponse çıktısı döner
- ✅ MsGlobalExceptionMiddleware – Global exception handling ile sistem genelinde beklenmeyen hatalar yakalanır ve loglanarak response döner
- ✅ Exception yönetimi ve hata detaylarını merkezi olarak özelleştirebilme imkanı
- ✅ IGenericRepository<TEntity> – Add, Update, Delete, Get, Any, Count gibi temel CRUD işlemlerini kapsayan generic yapı
- ✅ IUnitOfWork – Transaction bazlı işlem yönetimi (Begin, Commit, Rollback)
- ✅ PagedRequestDto & PagedResult<T> – Sayfalama işlemleri için hazır DTO ve response modeli
- ✅ ExpressionBuilder – String ile verilen property adına göre LINQ expression oluşturur (sıralama/dinamik filtreleme)
- ✅ Asenkron destekli repository yapısı
- ✅ Test edilebilirlik için soyut interface altyapısı
- ✅ EF Core uyumlu genişletilebilir yapı
- ✅ MsRequestLoggerMiddleware – HTTP request/response’ları yakalayarak loglar
- ✅ IMsLoggerService – File ve Database logger'larını birlikte yöneten composite servis
- ✅ MsFileLogger / MsDbLogger – Dosya ve veritabanına log yazma desteği (tekil ya da birlikte çalışabilir)
- ✅ LogEntity – Log kayıtlarının EF Core ile veritabanına yazılması için hazır model
- ✅ HttpContextExtensions.ReadRequestBodyAsync() – Request body içeriğini tekrar okunabilir şekilde sağlar
- ✅ Günlük rotasyon desteği (RotationType – daily, size-based)
- ✅ Dependency Injection üzerinden yapılandırılabilir LoggerOptions
- ✅ DateTime, string, collection işlemleri için hazır extension metodlar
- ✅ EnumHelper, FileHelper, HttpHelper, CacheHelper gibi birçok sık kullanılan yardımcı sınıf
- ✅ IConfiguration, environment ve generic veriye erişim için pratik wrapper metotlar
- ✅ Kod tekrarı azaltan, kullanım kolaylığı sağlayan araç seti
- ✅ Tüm modüller IServiceCollection üzerinden kolayca entegre edilir
- ✅ DI uyumlu, mocklanabilir yapılar
- ✅ Genişletilebilir ara katman ve servis mimarisi
- ✅ Modüler kullanım: sadece ihtiyaç duyulan paket yüklenebilir

## 📁 License

Bu proje MIT Lisansı altında lisanslanmıştır.
