Public Class Form1
    Dim gamewidth As Integer = 8
    Dim gameheight As Integer = 7
    Dim box_initx As Integer = 10
    Dim box_inity As Integer = 10
    Dim box_barrx As Integer = 7
    Dim box_barry As Integer = 8
    Dim gamesize As Integer = gamewidth * gameheight
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
End Class
