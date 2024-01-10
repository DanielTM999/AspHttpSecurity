using AspHttpSecurity.core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text.Json;
using static AspHttpSecurity.core.ISecurity;




namespace AspHttpSecurity.data
{
    public static class AspHttpSecurityMiddlewareExtension
    {
        public static void ConfigurarServicosHttpSecurity<E>(this IServiceCollection services) where E : class, IAuthService
        {
            services.AddSingleton<IAuthService, E>()
                .AddSingleton<AuthenticationManager>()
                .AddHttpContextAccessor();
        }
        public static IApplicationBuilder HttpSecurity(this IApplicationBuilder app, Configure security)
        {
            var _securityConfig = new HttpConfiguration();
            security(_securityConfig);
            return app.UseMiddleware<HttpSecurity>(_securityConfig);
        }
    }

    public class HttpSecurity
    {
        private readonly RequestDelegate _next;
        private HttpConfiguration _securityConfig;
        private bool isAuth = false;
        private static IAuthService? _DataContext;
        private static IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public static void AddInMemory(AuthDetails? details)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetObject("login", true);
            session.SetObject("datails", details);
        }

        public HttpSecurity(RequestDelegate next, IAuthService? DataContext, IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager, HttpConfiguration security)
        {
            _securityConfig = security;
            _DataContext = DataContext;
            _authenticationManager = authenticationManager ?? throw new ArgumentNullException(nameof(AuthenticationManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(IHttpContextAccessor));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            string path = context.Request.Path;
            try
            {
                await semaphoreSlim.WaitAsync();
                aplicarFiltrosAntes(_securityConfig.filterBefore, context);
                validateOptions(_securityConfig, path, context);
                aplicarFiltrosDepois(_securityConfig.filterBefore, context);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 403;
                ExceptionValid(context, e);
                return;
            }
            finally
            {
                semaphoreSlim.Release();
            }
            await _next(context);
        }

        private void validateOptions(HttpConfiguration config, string path, HttpContext context)
        {
            RouteIsPrivate(config, path, context);
            _authenticationManager.NextAuthDetails = null;
            _DataContext = null;
        }

        private void Authenticate(string route, HttpContext context)
        {
            if (_securityConfig._sessionPolicy == SessionPolicy.StateLess)
            {
                if (_authenticationManager.NextAuthDetails != null && _DataContext != null)
                {
                    Authenticate_StateLees(context, route);

                }
            }
            else
            {
                Authenticate_StateFull(context, route);
            }

        }

        private void Authenticate_StateLees(HttpContext context, string route)
        {
            var details = _authenticationManager.NextAuthDetails;
            var userData = _DataContext.GetUserDetails(details.getUserName());
            if (userData != null)
            {
                if (userData.getUserName().Equals(details.getUserName()))
                {
                    if (userData.getPassword().Equals(details.getPassword()))
                    {
                        conteinsRole(userData, route);
                        isAuth = true;
                    }
                }
            }
        }

        private void Authenticate_StateFull(HttpContext context, string route)
        {
            var details = _authenticationManager.NextAuthDetails;
            var session = context.Session;
            if (_securityConfig.NoDataBase)
            {
                if (details != null)
                {
                    bool islogado = true;
                    conteinsRole(details, route);
                    session.SetObject("login", islogado);
                    session.SetObject("datails", details);
                    isAuth = islogado;
                }
                else
                {
                    bool islogado = session.GetObject<bool>("login");
                    var valor = session.GetObject<AuthDetails>("details");
                    conteinsRole(valor, route);
                    isAuth = islogado;
                }
            }
            else
            {
                var userData = _DataContext.GetUserDetails(details.getUserName());
                if (userData != null)
                {
                    if (userData.getUserName().Equals(details.getUserName()))
                    {
                        if (userData.getPassword().Equals(details.getPassword()))
                        {
                            conteinsRole(userData, route);
                            session.SetObject("login", true);
                            session.SetObject("datails", details);
                            isAuth = true;
                        }
                    }
                }
            }

        }

        private void RouteIsPrivate(HttpConfiguration config, string path, HttpContext context)
        {
            if (config._pattern.req.ContainsKey(path))
            {
                if (config._pattern.req[path].method == HttpSecurityMethods.ALL || config._pattern.req[path].method.ToString().Equals(context.Request.Method))
                {
                    if (config._pattern.req[path].auth)
                    {
                        Authenticate(path, context);
                        if (!isAuth)
                        {
                            throw new Exception("erro na authenticação");
                        }
                    }
                }

            }
            else
            {
                if (config._pattern.autenticate)
                {
                    Authenticate(path, context);
                    if (!isAuth)
                    {
                        throw new Exception("erro na authenticação2");
                    }
                }
            }
        }

        private void aplicarFiltrosAntes(List<Type> filter, HttpContext context)
        {
            filter.ForEach(f =>
            {
                var filtro = Activator.CreateInstance(f) as InternalFilter;
                if (filtro != null)
                {
                    filtro.doFilter(context, _authenticationManager);

                    if (filtro is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            });
        }

        private void aplicarFiltrosDepois(List<Type> filter, HttpContext context)
        {
            filter.ForEach(f =>
            {
                var filtro = Activator.CreateInstance(f) as InternalFilter;
                if (filtro != null)
                {
                    filtro.doFilter(context, _authenticationManager);

                    if (filtro is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            });
        }

        private void conteinsRole(AuthDetails userData, string route)
        {
            if (_securityConfig._pattern.req.ContainsKey(route))
            {
                PatternConfig endpoint = _securityConfig._pattern.req[route];
                Rolesadd? raad = endpoint.rolesadd;
                if (raad != null)
                {
                    List<string> roles = raad.roles;
                    List<string> UserRoles = userData.getRoles();
                    if (roles != null)
                    {
                        roles.ForEach(r =>
                        {
                            if (!UserRoles.Contains(r))
                            {
                                throw new Exception($"Usuario sem a role {r}");
                            }
                        });
                    }
                }
            }

        }

        private void ExceptionValid(HttpContext context, Exception e)
        {
            if (_securityConfig.exceptionValid != null)
            {
                var errorMessage = _securityConfig.exceptionValid.Error(e);
                if (errorMessage != null)
                {
                    context.Response.ContentType = "application/json";
                    var jsonError = JsonSerializer.Serialize(errorMessage);
                    context.Response.WriteAsync(jsonError);
                }
            }
            else
            {
                DefaultResponseError(context, e);
            }
        }

        private void DefaultResponseError(HttpContext context, Exception e)
        {
            context.Response.ContentType = "application/json";
            var errorMessage = new { StatusCode = 403, error = e.Message };
            var jsonError = JsonSerializer.Serialize(errorMessage);
            context.Response.WriteAsync(jsonError);
        }
    }

    public enum SessionPolicy
    {
        StateLess,
        StateFull
    }

    public enum HttpSecurityMethods
    {
        GET,
        POST,
        PUT,
        DELETE,
        ALL
    }


    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }

}
