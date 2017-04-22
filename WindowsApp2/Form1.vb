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
        For x = 1 To gamewidth
            For y = 1 To gameheight
                Dim nine(8) As TextBox
                Dim thisbox As TextBox

            Next
        Next
    End Sub
    Private Function getbox(ByVal x As Integer, ByVal y As Integer) As TextBox
        If x <= 0 Or x > gamewidth Or y <= 0 Or y > gameheight Then
            Return New TextBox
        End If

    End Function
End Class
