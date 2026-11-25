Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.SignalR
Imports Microsoft.Extensions.Hosting

Public Class DashboardNotifierService
    Inherits BackgroundService

    Private ReadOnly _hubContext As IHubContext(Of DashboardHub)
    Private _lastMaxSalesId As Long = 0

    Public Sub New(hubContext As IHubContext(Of DashboardHub))
        _hubContext = hubContext
    End Sub

    Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
        Console.WriteLine("[SignalR Service] Real-Time Ultra-Fast WebSocket Push Service Started.")

        ' Initialize initial max sales_id
        _lastMaxSalesId = DatabaseHelper.GetMaxSalesId()

        While Not stoppingToken.IsCancellationRequested
            Try
                Dim currentMaxSalesId As Long = DatabaseHelper.GetMaxSalesId()

                ' If new transactions inserted, push instant WebSocket update immediately!
                If currentMaxSalesId <> _lastMaxSalesId Then
                    _lastMaxSalesId = currentMaxSalesId

                    Dim kpiJson = DatabaseHelper.GetKPIJson()
                    Dim trendJson = DatabaseHelper.GetSalesTrendJson()
                    Dim outletsJson = DatabaseHelper.GetOutletPerformanceJson()
                    Dim menusJson = DatabaseHelper.GetMenuPerformanceJson()

                    ' Instant Broadcast to all connected browsers
                    Await _hubContext.Clients.All.SendAsync("ReceiveDashboardUpdate", kpiJson, trendJson, outletsJson, menusJson, stoppingToken)
                End If
            Catch ex As Exception
                Console.WriteLine($"[SignalR Error] {ex.Message}")
            End Try

            ' Ultra-fast check every 150 milliseconds (sub-second instant detection)
            Await Task.Delay(150, stoppingToken)
        End While
    End Function
End Class
