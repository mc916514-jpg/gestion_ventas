using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using GestionComercial.Data;
using GestionComercial.Repository;
using GestionComercial.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar soporte para Controladores y Vistas MVC
builder.Services.AddControllersWithViews();

// 2. Registrar DbConnectionFactory como Singleton
builder.Services.AddSingleton<DbConnectionFactory>();

// 3. Registrar Capa de Repositorios (Repository)
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();
builder.Services.AddScoped<IHistorialAccionRepository, HistorialAccionRepository>();

// 4. Registrar Capa de Servicios (Services)
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<IContactoService, ContactoService>();
builder.Services.AddScoped<IHistorialAccionService, HistorialAccionService>();

// 5. Configurar Autenticación basada en Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// 6. Configurar Sesiones (Sesión Nativa para el Carrito de Compras)
builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Asegurar soporte de archivos estáticos tradicionales
app.UseStaticFiles();

app.UseRouting();

// 7. Habilitar middleware de Sesión, Autenticación y Autorización
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
