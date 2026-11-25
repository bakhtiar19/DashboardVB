Imports System.Data
Imports System.Text.Json
Imports Microsoft.Data.SqlClient

Public Class DatabaseHelper
    Private Shared ReadOnly ConnectionString As String = "Server=DESKTOP-TEUQ9N3;Database=db_testing;Integrated Security=True;TrustServerCertificate=True;Pooling=True;Min Pool Size=5;Max Pool Size=100;"

    Public Shared Function GetMaxSalesId() As Long
        Using conn As New SqlConnection(ConnectionString)
            conn.Open()
            Using cmd As New SqlCommand("SELECT ISNULL(MAX(sales_id), 0) FROM dbo.trx_sales WITH (NOLOCK);", conn)
                Return Convert.ToInt64(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    Public Shared Function GetKPIJson() As String
        Dim dict As New Dictionary(Of String, Object)()

        Using conn As New SqlConnection(ConnectionString)
            conn.Open()
            Using cmd As New SqlCommand("dbo.sp_GetDashboardKPI", conn)
                cmd.CommandType = CommandType.StoredProcedure
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        dict("total_orders") = Convert.ToInt64(reader("total_orders"))
                        dict("total_sales_lines") = Convert.ToInt64(reader("total_sales_lines"))
                        dict("total_quantity") = Convert.ToInt64(reader("total_quantity"))
                        dict("total_revenue") = Convert.ToDecimal(reader("total_revenue"))
                        dict("total_cogs") = Convert.ToDecimal(reader("total_cogs"))
                        dict("gross_profit") = Convert.ToDecimal(reader("gross_profit"))
                        dict("gross_margin_pct") = Math.Round(Convert.ToDouble(reader("gross_margin_pct")), 2)
                        dict("avg_order_value") = Math.Round(Convert.ToDecimal(reader("avg_order_value")), 2)
                    End If
                End Using
            End Using
        End Using

        Return JsonSerializer.Serialize(dict)
    End Function

    Public Shared Function GetSalesTrendJson() As String
        Dim list As New List(Of Dictionary(Of String, Object))()

        Using conn As New SqlConnection(ConnectionString)
            conn.Open()
            Using cmd As New SqlCommand("dbo.sp_GetSalesTrend", conn)
                cmd.CommandType = CommandType.StoredProcedure
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim row As New Dictionary(Of String, Object)()
                        row("sales_date") = Convert.ToDateTime(reader("sales_date")).ToString("yyyy-MM-dd")
                        row("daily_revenue") = Convert.ToDecimal(reader("daily_revenue"))
                        row("daily_profit") = Convert.ToDecimal(reader("daily_profit"))
                        list.Add(row)
                    End While
                End Using
            End Using
        End Using

        Return JsonSerializer.Serialize(list)
    End Function

    Public Shared Function GetOutletPerformanceJson() As String
        Dim list As New List(Of Dictionary(Of String, Object))()

        Using conn As New SqlConnection(ConnectionString)
            conn.Open()
            Using cmd As New SqlCommand("dbo.sp_GetOutletPerformance", conn)
                cmd.CommandType = CommandType.StoredProcedure
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim row As New Dictionary(Of String, Object)()
                        row("outlet_name") = reader("outlet_name").ToString()
                        row("city") = reader("city").ToString()
                        row("total_orders") = Convert.ToInt64(reader("total_orders"))
                        row("total_qty") = Convert.ToInt64(reader("total_qty"))
                        row("total_revenue") = Convert.ToDecimal(reader("total_revenue"))
                        row("total_cogs") = Convert.ToDecimal(reader("total_cogs"))
                        row("gross_profit") = Convert.ToDecimal(reader("gross_profit"))
                        row("gross_margin_pct") = Math.Round(Convert.ToDouble(reader("gross_margin_pct")), 2)
                        list.Add(row)
                    End While
                End Using
            End Using
        End Using

        Return JsonSerializer.Serialize(list)
    End Function

    Public Shared Function GetMenuPerformanceJson() As String
        Dim list As New List(Of Dictionary(Of String, Object))()

        Using conn As New SqlConnection(ConnectionString)
            conn.Open()
            Using cmd As New SqlCommand("dbo.sp_GetMenuPerformance", conn)
                cmd.CommandType = CommandType.StoredProcedure
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim row As New Dictionary(Of String, Object)()
                        row("menu_name") = reader("menu_name").ToString()
                        row("category") = reader("category").ToString()
                        row("total_qty") = Convert.ToInt64(reader("total_qty"))
                        row("total_revenue") = Convert.ToDecimal(reader("total_revenue"))
                        row("total_cogs") = Convert.ToDecimal(reader("total_cogs"))
                        row("gross_profit") = Convert.ToDecimal(reader("gross_profit"))
                        row("gross_margin_pct") = Math.Round(Convert.ToDouble(reader("gross_margin_pct")), 2)
                        list.Add(row)
                    End While
                End Using
            End Using
        End Using

        Return JsonSerializer.Serialize(list)
    End Function
End Class
