using CommonLibraryB.Library.AmrControl;
using CommonLibraryB.Library.AmrControl.Config;
using CommonLibraryB.Library.AmrControl.Property;
using CommonLibraryB.Manager.WebApiClient;
using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.Components;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.EquipName.AmrControl;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer;
using FarRobotControlWithApi_BlazorProject.Scope;
using FarRobotControlWithApi_BlazorProject.Services;
using FarRobotControlWithApi_BlazorProject.Services.Interface;
using FarRobotControlWithApi_BlazorProject.UITools.ToastMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;

//var builder = WebApplication.CreateBuilder(args);

//指定在Windows Service」環境下正確運作
var webApOpts = new WebApplicationOptions
{
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ?
        AppContext.BaseDirectory : default,
    Args = args
};
var builder = WebApplication.CreateBuilder(webApOpts);
builder.Host.UseWindowsService();

var connectString = builder.Configuration.GetConnectionString("SwarmCoreControlString")
    ?? throw new NullReferenceException("No Connection String in Config!");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMvc();

builder.Services.AddDbContext<SwarmCoreDbContext>(options => 
options.UseSqlServer(connectString));

builder.Services.AddSingleton<ILogTableOperate, LogTableLibrary>();
builder.Services.AddSingleton<IMissionTableOperate, MissionTableLibrary>();

string filePath = System.AppDomain.CurrentDomain.BaseDirectory;

builder.Services.AddScoped<IToastMessage, ToastMessage>();

builder.Services.AddSingleton<LogWritter>(provider => new LogWritter(filePath));

builder.Services.AddSingleton<WebApiClientManager>(provider => new WebApiClientManager(filePath));

builder.Services.AddSingleton<AmrControlConfigManager<EAmrControl>>(provider => new AmrControlConfigManager<EAmrControl>(filePath));
builder.Services.AddSingleton<AmrControlPropertyManager<EAmrControl>>(provider => new AmrControlPropertyManager<EAmrControl>(filePath));
builder.Services.AddSingleton<AmrControlLibrary<EAmrControl>>();

builder.Services.AddSingleton<ObserverLibrary>();

builder.Services.AddSingleton<MachineScope>();

builder.Services.AddSingleton<IMachineService, MachineService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.Run();