# Middleware de Segurança HTTP para ASP.NET

Este middleware oferece recursos de segurança HTTP para aplicativos ASP.NET. Ele inclui autenticação, autorização e outros recursos de segurança configuráveis. 

```csharp
// Adicione o serviço de autenticação no método ConfigureServices em Startup.cs
builder.Services.ConfigurarServicosHttpSecurity<ContextDb>();

// Adicione o middleware de segurança HTTP no pipeline no método Configure em Startup.cs
app.HttpSecurity(http => 
    http.sessionPolicy(SessionPolicy.StateLess).
    ExceptionValidation<Error>().
    RequestPatterns(req => 
        req.
        Request("/WeatherForecast").authenticate().hasRole().
        Request("ola").permitAll()
        .any().permitAll()
    ).
    addFilterBefore(typeof(Filter))
);
```

```csharp
// classe que é o banco de dados deve implementar
  namespace AspHttpSecurity.core
{
    public interface AuthDetails
    {
        public string getUserName();
        public string getPassword();
        public List<string> getRoles();
        public bool isBlock();
    }
}
  
```
