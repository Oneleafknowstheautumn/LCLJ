
using Autofac;
using Autofac.Core;
using His.Core;
using His.DAL;
using His.Entities.DomainModels;
using His.Util;
using HISWEBLCLJ;
using HISWEBLCLJ.Filter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog.Filters;
using Serilog;
using System.Text;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System;
using NetCore.MSA.Base;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

#region 显示注释
builder.Services.AddSwaggerGen(options =>
{
options.SwaggerDoc("v1", new OpenApiInfo
{
Version = "v1.0",
Title = "临床路径",
Description = $"临床路径"
});
var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //IncludeXmlComments 第二参数 true 则显示 控制器 注释
options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme{
                                Reference = new OpenApiReference {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "Bearer"}
                           },new string[] { }
                        }
                    });
});
#endregion

#region 调用startup
#region startup.ConfigureServices
//把services赋值到私有成员，用于扩展Autofac
//IServiceCollection Services = builder.Services;

//MVC热编译
//services.AddRazorPages().AddRazorRuntimeCompilation();

//请求配置
builder.Services.AddHttpContextAccessor();
//services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();//直接注册

//注册session服务
builder.Services.AddSession(options =>
{
    //options.Cookie.Name = ".AdventureWorks.Session";
    //设置session过期时间120分钟
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    //options.Cookie.HttpOnly = true;//设置在浏览器不能通过js获得该cookie的值
});

//加载自定义配置读取类
ConfigurationHelper.Init(builder.Services, builder.Configuration);

builder.Services.AddControllersWithViews(options =>
{
    //全局异常过滤器
    //options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<RequestFilter>();
    //JWT
    //options.Filters.Add<ApiAuthorizeFilter>();
});

//注入sqlsugar
builder.Services.AddSqlsugarSetup();

//必须appsettings.json中配置
string corsUrls = builder.Configuration["CorsUrls"];
if (string.IsNullOrEmpty(corsUrls))
{
    throw new Exception("请配置跨请求的前端Url");
}
//全局注入，允许跨域
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins(corsUrls.Split(","))
             //添加预检请求过期时间
             .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
            .AllowCredentials()
            .AllowAnyHeader().AllowAnyMethod();
        });
    //options.AddPolicy("cors", opt => opt
    //.AllowAnyOrigin()
    //.AllowAnyHeader()
    //.AllowAnyMethod()
    //.AllowCredentials()
    //.WithExposedHeaders("X-Pagination")
    //.WithOrigins(corsUrls.Split(",")));
});
#endregion
#region 配置鉴权JWT
//添加这一段，就不用在每个控制器中设置[Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
//builder.Services.AddAuthorization(options =>
//{
//    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
//        JwtBearerDefaults.AuthenticationScheme);

//    defaultAuthorizationPolicyBuilder =
//        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

//    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
//});
builder.Services.AddControllers(option =>
{
    option.Filters.Add(new AuthorizeFilter());
});
//将配置文件的JWT内容映射到类中
builder.Services.Configure<JWTTokenOption>(builder.Configuration.GetSection("JWT"));
//将配置文件的JWT内容映射到tokenOption中 
//JWTTokenOption tokenOption = new JWTTokenOption();
//builder.Configuration.Bind("JWT", tokenOption);
#region 添加jwt验证方法三
builder.Services
    .AddAuthorization() //启用授权
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //指定授权的渠道
    .AddJwtBearer(options =>
    {
        //取出私钥
        var secretByte = Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"]); //(ConfigurationHelper.GetSettingString("JWT:SecurityKey"));
        options.RequireHttpsMetadata = false; //是否用https
        options.SaveToken = true; ////授权成功后，是否储存令牌
                                  //添加校验
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            SaveSigninToken = true,//保存token,后台验证token是否生效(重要)
            ValidateIssuer = true,//是否验证Issuer 发布者
            ValidIssuer = builder.Configuration["JWT:Issuer"],//Issuer 发布者
            ValidateAudience = true,//是否验证Audience 接收者
            ValidAudience = builder.Configuration["JWT:Audience"], //Audience 接收者
            ValidateLifetime = true,//是否验证失效时间
            ValidateIssuerSigningKey = true,//是否验证SecurityKey,不验证的话可以篡改数据，不安全
            IssuerSigningKey = new SymmetricSecurityKey(secretByte),//验证私钥
            ClockSkew = TimeSpan.Zero,//这个是缓冲过期时间，也就是说，即使我们配置了过期时间，这里也要考虑进去，过期时间+缓冲，默认好像是7分钟，你可以直接设置为0
            RequireExpirationTime = true,
        };
        options.Events = new JwtBearerEvents
        {
            //此处为权限验证失败后触发的事件
            OnChallenge = context =>
            {
                //此处代码为终止.Net Core默认的返回类型和数据结果，很重要，必须
                context.HandleResponse();
                //自定义自己想要返回的结果
                var payload = System.Text.Json.JsonSerializer.Serialize(new ResponseResult()
                {
                    code = 401,
                    success = false,
                    message = $"权限验证失败!!{context.Error}!{context.ErrorDescription}!{context.ErrorUri}"
                });
                //自定义返回的数据类型
                context.Response.ContentType = "application/json";
                //自定义返回状态码，默认为401 我这里改成 200
                context.Response.StatusCode = StatusCodes.Status200OK;
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //输出Json数据结果
                context.Response.WriteAsync(payload);
                return Task.FromResult(0);
            }
        };
    });
#endregion

#endregion
#region 扩展Autofac的配置 
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(build =>
{
    builder.Services.AddModule(build, builder.Configuration);
});
#endregion
#region 日志
// 注册EncodingProvider的方法，以支持GB2312和GBK。
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
builder.Services.AddLogging(build =>
{
    // 配置 Serilog 
    Serilog.Log.Logger = new LoggerConfiguration()
    // 将配置传给 Serilog 的提供程序 
    .ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    //.WriteTo.Console(new RenderedCompactJsonFormatter())
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    //.WriteTo.File(formatter: new CompactJsonFormatter(), "logs/log_.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.File("logs/log.txt",
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)

    .Filter.ByExcluding(Matching.WithProperty<string>("Version", v =>
    {
        if (new Version(v) > new Version("1.0.0.1"))
            return true; // 不记录日志
        else
            return false; // 记录日志
    }))
    .CreateLogger();

    build.AddSerilog();
});
try
{
    Serilog.Log.Information("Starting web host");
}
catch (Exception ex)
{
    Serilog.Log.Fatal(ex, "Host terminated unexpectedly");
    return;
}
finally
{
    //Serilog.Log.CloseAndFlush();
}
builder.Host.UseSerilog(Serilog.Log.Logger, dispose: true);
#endregion
#endregion
#region Consul
builder.Services.AddSingleton<IServiceManager, ServiceManager>(); //直接注入
if (builder.Configuration["Consul:Enable"] == "1")
{
    var serviceManager = new ServiceManager(builder.Configuration);
    builder.Services.AddSingleton(serviceManager);
    try
    {
        serviceManager.Register();//注册
    }
    catch (Exception ex)
    {
        Serilog.Log.Fatal(ex, "Consul错误！");
        throw;
    }
}
#endregion


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(opt =>
{
    // 统一设置路由前缀
    opt.UseCentralRoutePrefix(new RouteAttribute("api"));
});
var app = builder.Build();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());//本质是替换ServiceProviderFactory的工厂


#region 移植Startup
//跨域
app.UseCors();

app.UseStaticFiles();

//在请求管道中启用session
app.UseSession();
app.UseRouting();

//添加jwt验证  这2句千万不能忘记了，顺序不能颠倒。
app.UseAuthentication(); //在前 鉴权
app.UseAuthorization(); //在后 授权

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "areas",
        areaName: "Admin",
        pattern: "{area:exists}/{controller=Index}/{action=Index}/{id?}"
   );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
   );
});
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();