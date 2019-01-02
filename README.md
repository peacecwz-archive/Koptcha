# Koptcha

Koptcha is simple Captcha library for .NET Core Projects

## Getting Started

1. Add Koptcha as a dependency to your .NET Core Projects

```bash
PS> Install-Package Koptcha
```

or

```bash
$ dotnet add package Koptcha
```

2. Configure settings in Startup.cs file. You have to use a Cache Provider. You can use SQL Server Cache, In Memory Cache or Distributed Cache (like a Redis)

```csharp

public void ConfigureServices(IServiceCollection services)
{
    services
        .AddDistributedMemoryCache()
        .AddCaptcha();
    
    services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
}


```

3. Configure your appsettings.json file

```json

"Captcha": {
    "DomainName": "Koptcha.API",
    "CaptchaTokenHeaderName": "x-captcha-token",
    "CaptchaControlBaseUrl": "https://www.google.com/recaptcha/api/siteverify?secret={captcha_secretkey}&response={captcha_token}",
    "CaptchaSecretKey": "{captcha_token}",
    "CaptchaSecretKeyHolder": "{captcha_secretkey}",
    "Duration": 60,
    "Threshold": 5, 
    "IsEnabledDuration": true,
    "IsEnabledThreshold": true,
    "IsCaptchaEnabled": true
  } 

```

## How to Use

### Use with Action Filter Attribute

#### IP Based Captcha

```csharp
  [Captcha(CaptchaType = CaptchaType.Ip, Duration = 5, Threshold = 20)]
  [HttpGet("test1")]
  public IActionResult Get()
  {
      return Ok("Test Ok");
  }
```

#### Email Based Captcha

```csharp
  [Captcha(CaptchaType = CaptchaType.Email, Duration = 5, Threshold = 20, FieldName = "EmailAddress")]
  [HttpPost("test2")]
  public IActionResult GetWithEmail([FromBody]EmailRequest request)
  {
      return Ok($"Test Ok with {request.EmailAddress}");
  }
```

#### IP and Email Based Captcha

```csharp
  [Captcha(CaptchaType = CaptchaType.IpAndEmail, Duration = 5, Threshold = 20, FieldName = "EmailAddress")]
  [HttpPost("test3")]
  public IActionResult GetWithEmailAndIp([FromBody]EmailRequest request)
  {
      return Ok($"Test Ok with {request.EmailAddress}");
  }
```

#### Custom Field Captcha

```csharp
  [Captcha(CaptchaType = CaptchaType.Custom, Duration = 5, Threshold = 20, FieldName = "Username")]
  [HttpPost("test4")]
  public IActionResult GetWithUsername([FromBody]EmailRequest request)
  {
      return Ok($"Test Ok with {request.Username}");
  }
```

### Use with Dependency Service

Coming Soon...

## TODO

- [ ] Documentation
- [ ] Unit Testing
- [ ] API Test Project
- [ ] MVC Test Project

## License

This project is licensed under the MIT License
