Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)
        
        ' Add CORS
        builder.Services.AddCors(Sub(options)
                                     options.AddDefaultPolicy(Sub(p)
                                                                  p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                                                              End Sub)
                                 End Sub)

        ' Add SignalR for Instant WebSocket Push
        builder.Services.AddSignalR()

        ' Register Real-Time Background Notifier Service
        builder.Services.AddHostedService(Of DashboardNotifierService)()

        Dim app = builder.Build()

        app.UseCors()
        app.UseDefaultFiles()
        app.UseStaticFiles()

        ' Map SignalR Hub Endpoint
        app.MapHub(Of DashboardHub)("/dashboardHub")

        ' API Routes for Real-Time Dashboard (Fallback / Initial Load)
        app.MapGet("/api/kpi", Function(ctx As HttpContext)
                                   ctx.Response.ContentType = "application/json"
                                   Return ctx.Response.WriteAsync(DatabaseHelper.GetKPIJson())
                               End Function)

        app.MapGet("/api/sales-trend", Function(ctx As HttpContext)
                                           ctx.Response.ContentType = "application/json"
                                           Return ctx.Response.WriteAsync(DatabaseHelper.GetSalesTrendJson())
                                       End Function)

        app.MapGet("/api/outlets", Function(ctx As HttpContext)
                                       ctx.Response.ContentType = "application/json"
                                       Return ctx.Response.WriteAsync(DatabaseHelper.GetOutletPerformanceJson())
                                   End Function)

        app.MapGet("/api/menus", Function(ctx As HttpContext)
                                     ctx.Response.ContentType = "application/json"
                                     Return ctx.Response.WriteAsync(DatabaseHelper.GetMenuPerformanceJson())
                                 End Function)

        Console.WriteLine("==========================================================================")
        Console.WriteLine(" SSB REAL-TIME DASHBOARD (VB.NET + SIGNALR WEBSOCKETS + SQL SERVER)")
        Console.WriteLine(" Server active on: http://localhost:5000")
        Console.WriteLine(" True Real-Time WebSocket Push (SignalR) Enabled!")
        Console.WriteLine("==========================================================================")

        app.Run("http://localhost:5000")
    End Sub
End Module
