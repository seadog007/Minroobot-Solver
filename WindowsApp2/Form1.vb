Imports Newtonsoft.Json
Imports System.Net
Imports System.Text
Imports System.IO
Public Class Form1
    Dim gamewidth As Integer = 8
    Dim gameheight As Integer = 7
    Dim box_initx As Integer = 10
    Dim box_inity As Integer = 10
    Dim box_barrx As Integer = 7
    Dim box_barry As Integer = 8
    Dim gamesize As Integer = gamewidth * gameheight
    Dim total_mines As Integer = 15
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim newbox As TextBox
        For x = 1 To gamewidth
            For y = 1 To gameheight
                newbox = New TextBox
                newbox.Name = "Box" & getindex(x, y)
                newbox.Width = 22
                newbox.Location = New Point(newbox.Width * (x - 1) + box_barrx * x + box_initx, newbox.Width * (y - 1) + box_barry * y + box_inity)
                AddHandler newbox.TextChanged, AddressOf recalculate
                Me.Controls.Add(newbox)
            Next
        Next
    End Sub
    Private Function getindex(ByVal x As Integer, ByVal y As Integer) As Integer
        Return (y - 1) * gamewidth + x
    End Function
    Private Sub recalculate()
        resetcolor()
        For x = 1 To gamewidth
            For y = 1 To gameheight
                Dim nine(8) As TextBox
                Dim thisbox As TextBox
                Dim minecount As Integer = 0
                Dim value As Integer = 0
                nine(0) = getbox(x - 1, y - 1)
                nine(1) = getbox(x, y - 1)
                nine(2) = getbox(x + 1, y - 1)
                nine(3) = getbox(x - 1, y)
                nine(4) = getbox(x, y)
                nine(5) = getbox(x + 1, y)
                nine(6) = getbox(x - 1, y + 1)
                nine(7) = getbox(x, y + 1)
                nine(8) = getbox(x + 1, y + 1)
                thisbox = nine(4)
                Try
                    value = Int(thisbox.Text)
                Catch ex As Exception
                    value = 99
                End Try

                If Not thisbox.Text = "" Then
                    thisbox.BackColor = Color.Red
                End If

                For i = 0 To nine.Length - 1
                    If nine(i).Text.ToUpper() = "X" Then
                        minecount += 1
                    End If
                Next
                If value <= minecount And Not value = 99 Then
                    cleanaround(nine)
                End If
            Next
        Next
        generate_state()
    End Sub
    Private Function getbox(ByVal x As Integer, ByVal y As Integer) As TextBox
        If x <= 0 Or x > gamewidth Or y <= 0 Or y > gameheight Then
            Return New TextBox
        End If
        Return CType(Me.Controls("Box" & getindex(x, y)), TextBox)
    End Function
    Private Sub cleanaround(ByRef nine As TextBox())
        For i = 0 To nine.Length - 1
            nine(i).BackColor = Color.Red
        Next
    End Sub
    Private Sub resetcolor()
        For x = 1 To gamewidth
            For y = 1 To gameheight
                Dim thisbox As TextBox = getbox(x, y)
                thisbox.BackColor = Color.White
            Next
        Next
    End Sub
    Private Function generate_state()
        Dim state As New Newtonsoft.Json.Linq.JObject
        Dim rule As New Newtonsoft.Json.Linq.JArray
        Dim count As Integer = 0
        Dim total_cells As Integer = 0
        For x = 1 To gamewidth
            For y = 1 To gameheight
                Try
                    Int(getbox(x, y).Text)
                    Dim obj As New Newtonsoft.Json.Linq.JObject
                    Dim cells As New Newtonsoft.Json.Linq.JArray
                    For i = -1 To 1
                        For j = -1 To 1
                            Dim box As TextBox = getbox(x + i, y + j)
                            If box.Text = "" And Not box.BackColor = Color.Red Then
                                cells.Add(x + i & "-" & y + j)
                            End If
                        Next
                    Next
                    obj.Add("num_mines", CInt(getbox(x, y).Text))
                    obj.Add("cells", cells)
                    rule.Add(obj)
                    count += 1
                Catch ex As Exception
                End Try
                If getbox(x, y).Text = "" And Not getbox(x, y).BackColor = Color.Red Then
                    total_cells += 1
                End If
            Next
        Next
        state.Add("rule", rule)
        state.Add("total_cells", total_cells)
        state.Add("total_mines", total_mines)
        Return state.ToString
    End Function
    Private Function SendRequest(uri As Uri, jsonDataBytes As Byte(), contentType As String, method As String) As String
        Dim req As WebRequest = WebRequest.Create(uri)
        req.ContentType = contentType
        req.Method = method
        req.ContentLength = jsonDataBytes.Length


        Dim stream = req.GetRequestStream()
        stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
        stream.Close()

        Dim response = req.GetResponse().GetResponseStream()

        Dim reader As New StreamReader(response)
        Dim res = reader.ReadToEnd()
        reader.Close()
        response.Close()

        Return res
    End Function
End Class
