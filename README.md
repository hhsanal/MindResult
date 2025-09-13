# MindResult

.NET projelerinde tip güvenli ve esnek bir Result yapısı sağlar. Hem data içeren hem data içermeyen sonuçları destekler, ek alan eklemeyi opsiyonel olarak sunar ve ASP.NET Core DI ile uyumludur.

## Özellikler

- **Data içeren (`MindResult<T>`) ve içermeyen (`MindResult`) result yapıları**
- **Success / Failure durumlarını kolay yönetme**
- **Ek alan ekleme opsiyonu (tip güvenli enum veya default)**
- **ASP.NET Core DI entegrasyonu (Scoped / Singleton / Transient)**
- **Opsiyonel ek alanlar ve opsiyonel data desteği**
- **Monadik operasyonlar (Map, Bind) fonksiyonel programlama için**
- **Fluent API tasarımı**

## Kurulum

```bash
dotnet add package MindResult.Core
```

## Temel Kullanım

### MindResult (Data içermeyen)

```csharp
using MindResult.Core;

// Başarılı sonuç
var success = MindResult.Success();

// Başarısız sonuç  
var failure = MindResult.Failure("Bir hata oluştu");

// Durumu kontrol etme
if (result.IsSuccess)
{
    Console.WriteLine("İşlem başarılı!");
}
else
{
    Console.WriteLine($"Hata: {result.ErrorMessage}");
}
```

### MindResult\<T> (Data içeren)

```csharp
using MindResult.Core;

// Başarılı sonuç (data ile)
var userResult = MindResult<User>.Success(new User { Id = 1, Name = "Ahmet" });

// Başarılı sonuç (data olmadan)
var emptyResult = MindResult<User>.Success();

// Başarısız sonuç
var errorResult = MindResult<User>.Failure("Kullanıcı bulunamadı");

// Data kontrolü
if (userResult.HasData)
{
    Console.WriteLine($"Kullanıcı: {userResult.Data.Name}");
}
```

### Ek Alanlar (Additional Fields)

```csharp
using MindResult.Core;

// Tip güvenli ek alanlar
var result = MindResult<string>.Success("data")
    .WithWarning("Bu bir uyarı mesajıdır")
    .WithCorrelationId("abc-123")
    .WithExecutionTime(TimeSpan.FromMilliseconds(150));

// Ek alan okuma
var warning = result.GetField<string, string>(MindResultFieldType.Warning);
var executionTime = result.GetField<string, TimeSpan>(MindResultFieldType.ExecutionTime);

// Manuel ek alan ekleme
var customResult = result.WithAdditionalField("CustomKey", "CustomValue");
var customValue = customResult.GetAdditionalField<string>("CustomKey");
```

### Fluent API ve Method Chaining

```csharp
// OnSuccess / OnFailure
result
    .OnSuccess(data => Console.WriteLine($"Başarılı: {data}"))
    .OnFailure(error => Console.WriteLine($"Hata: {error}"));

// Sadece data varsa çalıştır
result.OnSuccessWithData(data => ProcessData(data));
```

### Monadik Operasyonlar

```csharp
// Map - Veriyi dönüştür
var numberResult = MindResult<int>.Success(42);
var stringResult = numberResult.Map(x => $"Sayı: {x}");

// Bind - Zincirleme operasyonlar
var finalResult = numberResult
    .Bind(x => ValidateNumber(x))
    .Bind(x => ProcessNumber(x));

static MindResult<int> ValidateNumber(int number)
{
    return number > 0 
        ? MindResult<int>.Success(number)
        : MindResult<int>.Failure("Sayı pozitif olmalı");
}
```

### İmplicit Conversion

```csharp
// Data'dan Result'a dönüşüm
MindResult<string> result = "Hello World"; // Otomatik başarılı result

// Result'tan data'ya dönüşüm
string data = result; // result.Data döner
```

## ASP.NET Core DI Entegrasyonu

### Startup.cs / Program.cs

```csharp
using MindResult.Core.DependencyInjection;

// Temel kayıt (Scoped)
builder.Services.AddMindResult();

// Özel yaşam döngüsü
builder.Services.AddMindResult(ServiceLifetime.Singleton);

// Özel konfigürasyon
builder.Services.AddMindResult(options =>
{
    options.IncludeCorrelationId = true;
    options.IncludeExecutionTime = true;
    options.IncludeUserContext = false;
    options.CorrelationIdHeader = "X-Correlation-ID";
});
```

### Controller'da Kullanım

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMindResultFactory _resultFactory;
    private readonly IMindResultContext _context;

    public UsersController(IMindResultFactory resultFactory, IMindResultContext context)
    {
        _resultFactory = resultFactory;
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MindResult<User>>> GetUser(int id)
    {
        // Context bilgilerini ayarla
        _context.UserContext = User.Identity?.Name;
        
        var user = await _userService.GetUserAsync(id);
        
        if (user == null)
        {
            return Ok(_resultFactory.Failure<User>("Kullanıcı bulunamadı"));
        }

        return Ok(_resultFactory.Success(user)
            .WithWarning("Bu kullanıcı yakında silinecek"));
    }
}
```

### Service'de Kullanım

```csharp
public class UserService
{
    private readonly IMindResultFactory _resultFactory;

    public UserService(IMindResultFactory resultFactory)
    {
        _resultFactory = resultFactory;
    }

    public async Task<MindResult<User>> CreateUserAsync(CreateUserRequest request)
    {
        var validationResult = ValidateRequest(request);
        if (validationResult.IsFailure)
        {
            return _resultFactory.Failure<User>(validationResult.ErrorMessage);
        }

        try
        {
            var user = new User { Name = request.Name, Email = request.Email };
            await _repository.AddAsync(user);
            
            return _resultFactory.Success(user)
                .WithExecutionTime(stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            return _resultFactory.Failure<User>($"Kullanıcı oluşturulamadı: {ex.Message}");
        }
    }
}
```

## Predefined Field Types

MindResult aşağıdaki önceden tanımlanmış field türlerini destekler:

- `Warning` - Uyarı mesajları
- `Debug` - Debug bilgileri
- `Metadata` - İşlem metadata'sı
- `ValidationErrors` - Validasyon hataları
- `ExecutionTime` - İşlem süresi
- `Source` - İşlem kaynağı
- `CorrelationId` - İzleme ID'si
- `UserContext` - Kullanıcı context bilgisi
- `Pagination` - Sayfalama bilgisi
- `Cache` - Cache bilgisi

## Örnekler

### E-commerce Sepet İşlemi

```csharp
public async Task<MindResult<Order>> ProcessOrderAsync(Cart cart)
{
    return await ValidateCart(cart)
        .Bind(async validCart => await CalculateTotal(validCart))
        .Bind(async cartWithTotal => await CreateOrder(cartWithTotal))
        .Map(order => order.WithCorrelationId(Guid.NewGuid().ToString()));
}

private MindResult<Cart> ValidateCart(Cart cart)
{
    if (!cart.Items.Any())
        return MindResult<Cart>.Failure("Sepet boş olamaz");
        
    if (cart.Items.Any(i => i.Quantity <= 0))
        return MindResult<Cart>.Failure("Geçersiz ürün miktarı")
            .WithField(MindResultFieldType.ValidationErrors, "Quantity must be > 0");
            
    return MindResult<Cart>.Success(cart)
        .WithWarning($"Sepette {cart.Items.Count} ürün var");
}
```

### API Response Handling

```csharp
[HttpPost]
public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequest request)
{
    var result = await _productService.CreateProductAsync(request);
    
    return result.IsSuccess 
        ? Ok(new { 
            Data = result.Data, 
            CorrelationId = result.GetField<string>(MindResultFieldType.CorrelationId),
            Warning = result.GetField<string>(MindResultFieldType.Warning)
          })
        : BadRequest(new { 
            Error = result.ErrorMessage,
            CorrelationId = result.GetField<string>(MindResultFieldType.CorrelationId)
          });
}
```

## Test Etme

```bash
# Tüm testleri çalıştır
dotnet test

# Sadece core testleri
dotnet test tests/MindResult.Tests
```

## Lisans

MIT License
