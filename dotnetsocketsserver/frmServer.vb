Public Class frmServer

    Dim WithEvents server As New EIS.SocketServer(9898)
    Delegate Sub MessageReceivedDelegate(ByVal argMessage As String, ByVal argClientSocket As Net.Sockets.Socket)
    Delegate Sub ClientConnectedDelegate(ByVal argClientSocket As Net.Sockets.Socket)
    Delegate Sub ClientDisconnectedDelegate(ByVal argClientSocket As Net.Sockets.Socket)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'server.InitializeServer()
        server.StartServer()
        btnStartServer.Text = "Stop Server"
    End Sub

    Private Sub btnStartServer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartServer.Click
        If btnStartServer.Text = "Start Server" Then
            server.StartServer()
            btnStartServer.Text = "Stop Server"
        Else
            server.StopServer()
            btnStartServer.Text = "Start Server"
        End If
    End Sub

    Private Sub server_ClientConnected(ByVal argClientSocket As System.Net.Sockets.Socket) Handles server.ClientConnected
        Invoke(New ClientConnectedDelegate(AddressOf ClientConnected), argClientSocket)
    End Sub

    Sub ClientConnected(ByVal argClientSocket As Net.Sockets.Socket)
        ListBox1.Items.Add(argClientSocket.RemoteEndPoint)
    End Sub

    Private Sub server_ClientDisconnected(ByVal argClientSocket As System.Net.Sockets.Socket) Handles server.ClientDisconnected
        Invoke(New ClientDisconnectedDelegate(AddressOf ClientDisconnected), argClientSocket)
    End Sub

    Sub ClientDisconnected(ByVal argClientSocket As Net.Sockets.Socket)
        ListBox1.Items.Remove(argClientSocket.RemoteEndPoint)
    End Sub

    Private Sub server_MessageReceived(ByVal argMessage As String, ByVal argClient As Net.Sockets.Socket) Handles server.MessageReceived
        Invoke(New MessageReceivedDelegate(AddressOf MessageReceived), New Object() {argMessage, argClient})
    End Sub

    Sub MessageReceived(ByVal argMessage As String, ByVal argClient As Net.Sockets.Socket)
        If txtReceiveLog.Text <> "" Then txtReceiveLog.Text = vbNewLine & txtReceiveLog.Text
        txtReceiveLog.Text = argClient.RemoteEndPoint.ToString & ": " & argMessage & txtReceiveLog.Text
    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Dim Sockets As New List(Of Net.Sockets.Socket)
        For Each Item As Net.EndPoint In ListBox1.SelectedItems
            For Each Socket As Net.Sockets.Socket In server.ClientList
                If Socket.RemoteEndPoint.Equals(Item) Then
                    Sockets.Add(Socket)
                End If
            Next
        Next
        SendMessage(txtSend.Text, Sockets)

    End Sub

    Sub SendMessage(Message As String, Clients As List(Of Net.Sockets.Socket))
        For Each Client As Net.Sockets.Socket In Clients
            server.SendMessageToClient("/say " & Message, Client)
        Next
    End Sub

End Class
