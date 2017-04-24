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
                newbox.Width = 23
                newbox.Height = 23
                newbox.TextAlign = HorizontalAlignment.Center
                newbox.Font = New Font("Arial", 10)
                newbox.Location = New Point(newbox.Width * (x - 1) + box_barrx * x + box_initx, newbox.Height * (y - 1) + box_barry * y + box_inity)
                AddHandler newbox.TextChanged, AddressOf recalculate
                Me.Controls.Add(newbox)
            Next
        Next
    End Sub
    Private Function getindex(ByVal x As Integer, ByVal y As Integer) As Integer
        Return (y - 1) * gamewidth + x
    End Function
    Private Sub recalculate()
        resetcolorntag()
        For x = 1 To gamewidth
            For y = 1 To gameheight
                Dim thisbox As TextBox
                Dim value As Integer = 0
                thisbox = getbox(x, y)
                Try
                    value = Int(thisbox.Text)
                Catch ex As Exception
                    value = 99
                End Try

                If Not thisbox.Text = "" Then
                    thisbox.BackColor = Color.Red
                End If

                If isfull(x, y, value) And Not value = 99 Then
                    cleanaround(x, y)
                End If
            Next
        Next
        If CheckBox1.Checked Then
            apply_tag(SendRequest(generate_state()))
        End If
        apply_color()
    End Sub
    Private Function getbox(ByVal x As Integer, ByVal y As Integer) As TextBox
        If x <= 0 Or x > gamewidth Or y <= 0 Or y > gameheight Then
            Return New TextBox
        End If
        Return CType(Me.Controls("Box" & getindex(x, y)), TextBox)
    End Function
    Private Sub cleanaround(ByVal x As Integer, ByVal y As Integer)
        Dim box As TextBox
        For i = -1 To 1
            For j = -1 To 1
                box = getbox(x + i, y + j)
                box.BackColor = Color.Red
            Next
        Next
    End Sub
    Private Sub resetcolorntag()
        For x = 1 To gamewidth
            For y = 1 To gameheight
                Dim thisbox As TextBox = getbox(x, y)
                thisbox.BackColor = Color.White
                thisbox.Tag = "-1"
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
                    Dim mines_around As Integer = 0
                    For i = -1 To 1
                        For j = -1 To 1
                            Dim box As TextBox = getbox(x + i, y + j)
                            If box.Text = "" And Not box.BackColor = Color.Red Then
                                If Not (x + i <= 0 Or x + i > gamewidth Or y + j <= 0 Or y + j > gameheight) Then
                                    cells.Add(x + i & "-" & y + j)
                                End If
                            End If
                            If box.Text = "X" And Not i = 0 And Not j = 0 Then
                                mines_around += 1
                            End If
                        Next
                    Next
                    If Not isfull(x, y, Int(getbox(x, y).Text)) Then
                        obj.Add("num_mines", CInt(getbox(x, y).Text) - mines_around)
                        obj.Add("cells", cells)
                        rule.Add(obj)
                        count += 1
                    End If
                Catch ex As Exception
                End Try
                'If getbox(x, y).Text = "" And Not getbox(x, y).BackColor = Color.Red Then
                If getbox(x, y).Text = "" Then
                    total_cells += 1
                End If
            Next
        Next
        state.Add("rules", rule)
        state.Add("total_cells", total_cells)
        state.Add("total_mines", total_mines)
        Return state.ToString
    End Function
    Private Function SendRequest(ByVal jsonData As String) As String
        If CheckBox2.Checked Then
            MsgBox(jsonData)
        End If
        Dim req As WebRequest = WebRequest.Create("http://mrgris.com/app/minesweepr/api/minesweeper_solve/")
        Dim jsonDataBytes As Byte()
        jsonDataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData)
        req.ContentType = "application/x-www-form-urlencodedd"
        req.Method = "POST"
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
    Private Sub apply_tag(ByVal s As String)
        Dim data As Newtonsoft.Json.Linq.JObject = Newtonsoft.Json.Linq.JObject.Parse(s)
        Dim special(data.GetValue("solution").Count - 1, 2) As String
        Dim a As Newtonsoft.Json.Linq.JToken()
        Dim index_other As Integer
        If data.GetValue("solution").Count = 0 Then
            Exit Sub
        End If
        a = data.GetValue("solution").ToArray
        For i = 0 To data.GetValue("solution").Count - 1
            Dim b, c As String()
            b = Split(a(i).ToString(), ": ")
            b(0) = b(0).Replace("""", "")
            If b(0) = "_other" Then
                index_other = i
                special(i, 0) = "0"
                special(i, 1) = "0"
                special(i, 2) = b(1)
            Else
                c = Split(b(0), "-")
                special(i, 0) = c(0)
                special(i, 1) = c(1)
                special(i, 2) = b(1)
            End If
        Next

        For x = 1 To gamewidth
            For y = 1 To gameheight
                Dim thisbox As TextBox = getbox(x, y)
                For i = 0 To special.GetLength(0) - 1
                    If special(i, 0) = x.ToString() And special(i, 1) = y.ToString() Then
                        thisbox.Tag = special(i, 2)
                        Exit For
                    Else
                        thisbox.Tag = special(index_other, 2)
                    End If
                Next
            Next
        Next
    End Sub
    Private Sub apply_color()
        For x = 1 To gamewidth
            For y = 1 To gameheight
                Dim thisbox As TextBox = getbox(x, y)
                If Not thisbox.BackColor = Color.Red Then
                    If Not thisbox.Tag = "-1" Then
                        thisbox.BackColor = getcolor(CDbl(thisbox.Tag))
                    End If
                End If
            Next
        Next
    End Sub
    Private Function getcolor(ByVal value As Double) As Color
        Return Color.FromArgb(255 - (value * 255), value * 255, 0)
    End Function
    Private Function isfull(ByVal x As Integer, ByVal y As Integer, ByVal value As Integer) As Boolean
        Dim minecount As Integer
        Dim box As TextBox
        For i = -1 To 1
            For j = -1 To 1
                box = getbox(x + i, y + j)
                If box.Text.ToUpper() = "X" Then
                    minecount += 1
                End If
            Next
        Next
        Return value <= minecount
    End Function
End Class
