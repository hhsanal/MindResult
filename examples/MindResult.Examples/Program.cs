using MindResult.Core;

Console.WriteLine("=== MindResult Örnekleri ===\n");

// Temel kullanım örnekleri
BasicExamples();

// Monadik operasyonlar
MonadicExamples();

// Ek alanlar ve field türleri
AdditionalFieldsExamples();

static void BasicExamples()
{
    Console.WriteLine("1. Temel Kullanım:");
    
    // Başarılı sonuç
    var success = MindResult.Core.MindResult.Success();
    Console.WriteLine($"  Başarılı: {success.IsSuccess}");
    
    // Başarısız sonuç
    var failure = MindResult.Core.MindResult.Failure("Bir hata oluştu");
    Console.WriteLine($"  Başarısız: {failure.IsFailure}, Hata: {failure.ErrorMessage}");
    
    // Data ile başarılı sonuç
    var userResult = MindResult<string>.Success("Ahmet");
    Console.WriteLine($"  Kullanıcı: {userResult.Data}");
    
    // İmplicit conversion
    MindResult<int> numberResult = 42;
    int number = numberResult;
    Console.WriteLine($"  İmplicit conversion: {number}");
    
    Console.WriteLine();
}

static void MonadicExamples()
{
    Console.WriteLine("2. Monadik Operasyonlar:");
    
    var result = MindResult<int>.Success(10);
    
    // Map operasyonu
    var stringResult = result.Map(x => $"Sayı: {x}");
    Console.WriteLine($"  Map sonucu: {stringResult.Data}");
    
    // Bind operasyonu
    var finalResult = result
        .Bind(x => x > 5 ? MindResult<int>.Success(x * 2) : MindResult<int>.Failure("Çok küçük"))
        .Map(x => $"Final: {x}");
    
    Console.WriteLine($"  Bind sonucu: {finalResult.Data}");
    
    // OnSuccess ve OnFailure
    result
        .OnSuccess(data => Console.WriteLine($"  OnSuccess: İşlem başarılı, data: {data}"))
        .OnFailure(error => Console.WriteLine($"  OnFailure: {error}"));
    
    Console.WriteLine();
}

static void AdditionalFieldsExamples()
{
    Console.WriteLine("3. Ek Alanlar:");
    
    var result = MindResult<string>.Success("test data")
        .WithWarning("Bu bir uyarı mesajıdır")
        .WithCorrelationId("abc-123")
        .WithExecutionTime(TimeSpan.FromMilliseconds(150))
        .WithAdditionalField("CustomKey", "CustomValue");
    
    // Ek alanları okuma
    var warning = result.GetField<string, string>(MindResultFieldType.Warning);
    var correlationId = result.GetField<string, string>(MindResultFieldType.CorrelationId);
    var executionTime = result.GetField<string, TimeSpan>(MindResultFieldType.ExecutionTime);
    var customValue = result.GetAdditionalField<string>("CustomKey");
    
    Console.WriteLine($"  Uyarı: {warning}");
    Console.WriteLine($"  Correlation ID: {correlationId}");
    Console.WriteLine($"  Çalışma süresi: {executionTime.TotalMilliseconds}ms");
    Console.WriteLine($"  Özel alan: {customValue}");
    
    // E-commerce örneği
    Console.WriteLine("\n4. E-commerce Sepet Örneği:");
    var cartResult = ProcessCart(new[] { "Laptop", "Mouse" });
    
    cartResult
        .OnSuccess(items => Console.WriteLine($"  Sepet işlendi: {string.Join(", ", items ?? Array.Empty<string>())}"))
        .OnFailure(error => Console.WriteLine($"  Sepet hatası: {error}"));
    
    var warning2 = cartResult.GetField<string[], string>(MindResultFieldType.Warning);
    if (!string.IsNullOrEmpty(warning2))
    {
        Console.WriteLine($"  Uyarı: {warning2}");
    }
}

static MindResult<string[]> ProcessCart(string[] items)
{
    if (items == null || items.Length == 0)
    {
        return MindResult<string[]>.Failure("Sepet boş olamaz");
    }
    
    var result = MindResult<string[]>.Success(items)
        .WithCorrelationId(Guid.NewGuid().ToString("N")[..8])
        .WithExecutionTime(TimeSpan.FromMilliseconds(50));
    
    if (items.Length > 5)
    {
        result = result.WithWarning("Sepette çok fazla ürün var");
    }
    
    return result;
}
