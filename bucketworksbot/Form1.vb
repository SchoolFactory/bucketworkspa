Imports System.Linq
Imports Amrykid.Web.IRC
Imports System.IO
Imports System.Speech.Synthesis
Imports System
Imports System.ComponentModel
Imports System.Threading
Imports System.Xml
Imports System.ServiceModel.Syndication
Imports System.Speech.AudioFormat

Public Class Form1
    Private Const Server As String = "irc.freenode.org"
    Public versionnumber As String = "1.7"
    Private Const Port As Integer = 6667
    Private Const Nick As String = "bucketbotpa"
    Private Const User As String = "bucketbotpa"
    Public Const Channel As String = "#bucketworks1"
    Public networkissues As Long = 0
    Public Shared Volume As Integer = 100
    Public Shared Itemssaid As Long = 0
    Public Rate As Integer = 10
    Private WithEvents irc As IRC
    Public Shared audiostream As String = ""
    Public Shared Tempstring2 As String = ""
    Public Shared Tempstring As String = ""
    Public Shared numberofaudioclipsplayed As Long = 0
    Public warnings As Long = 0
    Public enableweathercheck As Boolean = True
    Public voice As String = "Bridget"
    Dim t As System.Threading.Thread
    Delegate Sub SetUrl([url] As String)
    Public Sub New()
        InitializeComponent()
        ' --------------
        irc = New IRC("Sample")
    End Sub
    'irc messages decode.

    Public Sub Privateirc(ByVal message As String, ByVal user As User) Handles irc.IRCPrivateMSGRecieved
        If (IsAuthed(user.Nick)) Then

            If message.ToLower.StartsWith("!remove") Then
                Try
                    Dim removeuser As String()
                    removeuser = message.ToString.Split(" ")
                    RemoveLine(Path + "Masters.txt", removeuser(1))
                    RemoveLine(Path + "Authed.txt", removeuser(1))
                Catch ex As Exception

                End Try

            End If


            If (message.ToLower.StartsWith("!time")) Then
                Dim time As String = DateTime.Now.ToLocalTime().ToString()
                irc.SendPrivateMessage(time, user.Nick)
            End If
            If (message.StartsWith("!stats")) Then
                irc.SendPrivateMessage("total number of things said:" & Itemssaid.ToString, user.Nick)
                irc.SendPrivateMessage("Current volume set to:" & Volume.ToString, user.Nick)
            End If

            If (message.ToLower.StartsWith("!add")) Then
                If ((IsMaster(user.Nick))) Then
                    Dim username As String()
                    username = message.ToString.Split(" ")
                    Try
                        Dim userpassword As String = Mid(message, message.LastIndexOf(" ") + 2, message.Length)
                        File.AppendAllText(Path + "Masters.txt", username(1) + " " + userpassword + vbCrLf)
                        File.AppendAllText(Path + "Authed.txt", username(1) + vbCrLf)
                        irc.SendPrivateMessage(username(1) & " has have been added as a Master and automatically Authed", username(1))
                        irc.SendPrivateMessage(username(1) & " has have been added as a Master and automatically Authed", user.Nick)

                    Catch ex As Exception
                        irc.SendPrivateMessage(user.Nick, "Something went wrong try again.")

                    End Try
                Else
                    irc.SendPrivateMessage(user.Nick, "You're already added as a Master")
                End If
            End If



            If (message.ToLower.StartsWith("!help")) Then

                irc.SendPrivateMessage("!add [username] [password]", user.Nick)
                irc.SendPrivateMessage("!remove [username]", user.Nick)
                irc.SendPrivateMessage("!time", user.Nick)
                irc.SendPrivateMessage("!stats", user.Nick)
            End If

        Else
            irc.SendNotice(user.Nick, "You either have not Authed with me, or you do not have permission. If you are a Master and haven't Authed then please /msg " + Nick + " Auth " + user.Nick + " <pass>")

        End If




        If (message.ToLower.StartsWith("auth")) Then



            If (IsMaster(user.Nick)) Then
                If (Not (IsAuthed(user.Nick))) Then
                    Dim userAuth As String = user.Nick + " " + Mid(message, message.LastIndexOf(" ") + 2, message.Length)

                    Dim master As String = File.ReadAllText(Path + "Masters.txt")
                    Dim masters() As String = Split(master, vbCrLf)

                    For i As Integer = 0 To UBound(masters)
                        If (userAuth = masters(i)) Then
                            File.AppendAllText(Path + "Authed.txt", user.Nick + vbCrLf)
                            irc.SendNotice(user.Nick, "You have been Authed with me, you may now use Master commands")
                            Exit For
                        End If
                    Next
                Else
                    irc.SendNotice(user.Nick, "You're already Authed with me")
                End If
            Else
                irc.SendNotice(user.Nick, "You are not on the Master list")
            End If
        End If







    End Sub
    Private Delegate Sub MyDelegate()

    Public Sub Setaudiostream()
        If (Me.AxWindowsMediaPlayer2.InvokeRequired) Then

            Dim setaudiodel As MyDelegate
            setaudiodel = AddressOf Setaudiostream
            Try

                Me.AxWindowsMediaPlayer2.Invoke(setaudiodel)

            Catch ex As Exception

            End Try




        Else


            Me.AxWindowsMediaPlayer2.settings.volume = Volume

            Me.AxWindowsMediaPlayer2.currentPlaylist.clear()

            Me.AxWindowsMediaPlayer2.URL = audiostream
            numberofaudioclipsplayed = numberofaudioclipsplayed + 1
        End If
    End Sub

    Public Sub Setaudiovolume()
        If (Me.AxWindowsMediaPlayer2.InvokeRequired) Then
            Dim setaudiodel As MyDelegate
            setaudiodel = AddressOf Setaudiostream
            Try

                Me.AxWindowsMediaPlayer2.Invoke(setaudiodel)

            Catch ex As Exception

            End Try
        Else
            audiostream = ""
            Me.AxWindowsMediaPlayer2.settings.volume = Volume

            numberofaudioclipsplayed = numberofaudioclipsplayed + 1
        End If
    End Sub


    Public Sub Ircmessage(sender As Object, message As String, user As Amrykid.Web.IRC.User, chan As Amrykid.Web.IRC.IRCChannel) Handles irc.IRCMessageRecieved
        If (IsAuthed(user.Nick)) Then


            If message.ToLower.Equals("!time") Then
                Dim time As String = DateTime.Now.ToLocalTime().ToString()
                Speak(time)

                irc.SendMessage(time)
            End If
            If message.ToLower.Equals("!help") Then
                irc.SendNotice(user.Nick, "vaild options")
                irc.SendNotice(user.Nick, "!vol [volume] 0 - 100")
                irc.SendNotice(user.Nick, "!time - Current time of bot")
                System.Threading.Thread.Sleep(1500)
                irc.SendNotice(user.Nick, "!say [what you want to come over pa system")
                irc.SendNotice(user.Nick, "!play {url} will automaticly download and play set url")
                irc.SendNotice(user.Nick, "!stopplayer will stop the current audio track over the pa speaker")
                System.Threading.Thread.Sleep(1500)
                irc.SendNotice(user.Nick, "!voice [Bridget|Julie|Zira| will stop the current audio track over the pa speaker")

                irc.SendNotice(user.Nick, "!disableweather will disable weather checking")
                irc.SendNotice(user.Nick, "!enableweather will enable weather checking")

            End If

            Try

                If message.ToLower.Substring(0, 6).Equals("!voice") Then
                    voice = message.ToLower.Substring(7, message.Length - 7)
                End If
            Catch ex As Exception
                irc.SendNotice(user.Nick, "Unable to set voice")

            End Try


            Try

                If message.ToLower.Substring(0, 4).Equals("!vol") Then
                    Volume = CInt(message.Substring(4, message.Length - 4))
                    Speak("Volume set to:" & Volume)
                    Setaudiovolume()

                End If
            Catch ex As Exception
                irc.SendNotice(user.Nick, "Unable to set volume")

            End Try

            Try
                If message.ToLower.StartsWith("!stopplayer") Then
                    audiostream = ""
                    AxWindowsMediaPlayer2.currentPlaylist.clear()


                End If
            Catch ex As Exception

            End Try


            Try
                If message.ToLower.StartsWith("!version") Then
                    irc.SendMessage("1.5")

                End If

            Catch ex As Exception

            End Try

            Try
                If message.ToLower.StartsWith("!disableweather") Then
                    enableweathercheck = False
                End If
            Catch ex As Exception

            End Try

            Try
                If message.ToLower.StartsWith("!enableweather") Then
                    enableweathercheck = True
                End If
            Catch ex As Exception

            End Try

            Try
                If message.ToLower.StartsWith("!play") Then

                
                    Dim temp1 As String = message.Substring(6, message.Length - 6)
                    audiostream = temp1
                    If audiostream.ToLower.Contains("/watch?v=") Then
                        audiostream = audiostream.Replace("/watch?v=", "/v/")
                        audiostream = audiostream & "&h1=en&autoplay=1"
                    End If
                    Dim thread1 As Thread
                    thread1 = New System.Threading.Thread(AddressOf Setaudiostream)
                    thread1.SetApartmentState(ApartmentState.STA)
                    thread1.Start()

                End If
            Catch ex As Exception

            End Try

            Try
                If message.Substring(0, 4).ToLower.Equals("!say") Then
                    Itemssaid = Itemssaid + 1
                    Speak((message.Substring(4, message.Length - 4)))

                End If

            Catch ex As Exception

            End Try



        Else
            irc.SendNotice(user.Nick, "You either have not Authed with me, or you do not have permission. If you are a Master and haven't Authed then please /msg " + Nick + " Auth " + user.Nick + " <pass>")

        End If

    End Sub



    Private Shared _path As String = My.Application.Info.DirectoryPath + "\Settings\"
#Region "RemoveLine(s)/RandomNumber"
    Private Shared Sub RemoveLine(ByVal fileName As String, ByVal lineToRemove As String)
        Dim str() As String = File.ReadAllLines(fileName)

        Using sw As New IO.StreamWriter(fileName)
            For Each line As String In str
                If (Not (line.Contains(lineToRemove))) Then
                    sw.WriteLine(line)
                End If
            Next
        End Using
    End Sub
#End Region


#Region "UserKicked"
    Private Sub IrcUserKicked(ByVal sender As Object, ByVal chan As String, ByVal usr As User, ByVal reason As String, ByVal kicker As User) Handles irc.IRCUserKick
        If (usr.Nick = Nick) Then
            irc.Join(chan)
        End If

        If (IsAuthed(usr.Nick)) Then
            RemoveLine(Path + "Authed.txt", usr.Nick)
        End If
    End Sub
#End Region



#Region "PartUser"
    Private Shared Sub IrcPartUser(ByVal sender As Object, ByVal user As User, ByVal chan As String, ByVal reason As String) Handles irc.IRCUserPart
        If (IsAuthed(user.Nick)) Then
            RemoveLine(Path + "Authed.txt", user.Nick)
        End If
    End Sub
#End Region

#Region "NickChange"
    Private Sub IrcNickChange(ByVal oldnick As String, ByVal newnick As String) Handles irc.IRCNickChange
        If (IsAuthed(oldnick)) Then
            RemoveLine(Path + "Authed.txt", oldnick)
            File.AppendAllText(Path + "Authed.txt", newnick + vbCrLf)
            irc.SendNotice(newnick, "Your new nick has Masters permission")
        End If
    End Sub
#End Region

#Region "JoinUser"
    Private Sub IrcJoinUser(ByVal sender As Object, ByVal user As User, ByVal chan As String) Handles irc.IRCUserJoin
        If (IsMaster(user.Nick)) Then

            irc.SendNotice(user.Nick, "You have just joined the channel " + chan + " and are not Authed with me, Your messages will not go over pa system unless you Auth please /msg " + Nick + " Auth " + user.Nick + " <pass>")
        End If
    End Sub
#End Region

#Region "QuitUser"
    Private Shared Sub IrcQuitUser(ByVal sender As Object, ByVal user As User, ByVal reason As String) Handles irc.IRCUserQuit
        If (IsAuthed(user.Nick)) Then
            RemoveLine(Path + "Authed.txt", user.Nick)
        End If
    End Sub
#End Region


#Region "Public Properties"
    Public Shared ReadOnly Property Path() As String
        Get
            Return _path
        End Get
    End Property
#End Region


    Private Sub SetUpDirectory()
        'SetupSettingsXML()
        If (Not Directory.Exists(Path)) Then
            Directory.CreateDirectory(Path)
        End If
        If (Not File.Exists(Path + "CmdChar.txt")) Then
            File.AppendAllText(Path + "CmdChar.txt", Nothing)
        End If
        If (Not File.Exists(Path + "Masters.txt")) Then
            File.AppendAllText(Path + "Masters.txt", Nothing)
        End If
        If (Not File.Exists(Path + "Quotes.txt")) Then
            File.AppendAllText(Path + "Quotes.txt", Nothing)
        End If
        If (Not File.Exists(Path + "Authed.txt")) Then
            File.AppendAllText(Path + "Authed.txt", Nothing)
        End If

    End Sub



#Region "isAuthed/Master"
    Private Shared Function IsMaster(ByVal nick As String) As Boolean
        Dim master As String = File.ReadAllText(Path + "Masters.txt")
        Dim masters() As String = Split(master, vbCrLf)

        For i As Integer = 0 To UBound(masters)
            If (Not (masters(i) = "")) Then
                If (nick = Mid(masters(i), 1, masters(i).IndexOf(" "))) Then
                    Return True
                End If
            End If
        Next

        Return False
    End Function

    Private Shared Function IsAuthed(ByVal nick As String) As Boolean
        Dim auth As String = File.ReadAllText(Path + "Authed.txt")
        Dim auths() As String = Split(auth, vbCrLf)

        For i As Integer = 0 To UBound(auths)
            If (nick = auths(i)) Then
                Return True
                Exit For
            End If
        Next

        Return False
    End Function
#End Region

    Public last_alert As String = ""
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        



        Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        '  Setaudiostream()

        SetUpDirectory()



        irc.Connect(Server, 6667)
        irc.Logon(Nick, Nick)
        irc.Join(Channel, "freeloader")
        irc.ProcessEventsAsync()
        irc.SendMessage("Bucketworks Pa Bot online version " & versionnumber.ToString, Channel)

    End Sub


    Public Sub Speak(ByVal stringtosay As String)
        Dim wasplaying As Boolean = False
        If AxWindowsMediaPlayer2.playState = WMPLib.WMPPlayState.wmppsPlaying Then
            wasplaying = True
            Me.AxWindowsMediaPlayer2.Ctlcontrols.pause()

        End If


        Dim pObjSynth1 As New SpeechSynthesizer


        pObjSynth1.Volume = Volume
        ' pObjSynth1.Rate = -1
        ' 
        Try
            If voice.ToLower.Equals("bridget") Then
                pObjSynth1.SelectVoice("VW Bridget")
            Else
                If voice.ToLower.Equals("zira") Then
                    pObjSynth1.SelectVoice("Microsoft Zira Desktop")
                Else
                    If voice.ToLower.Equals("julie") Then
                        pObjSynth1.SelectVoice("VW Julie")
                    Else
                        pObjSynth1.SelectVoice("VW Bridget")
                    End If
                    
                End If
            End If

           
            
        Catch ex As Exception
            pObjSynth1.SelectVoice("VW Bridget")

        End Try




        '  pObjSynth1.SetOutputToAudioStream(

        pObjSynth1.Speak(stringtosay.ToString)
        System.Threading.Thread.Sleep(100)
        Dim speakingstate As String = pObjSynth1.State.ToString

        'System.Threading.Thread.Sleep(1000)

        While speakingstate.ToString.Equals("Speaking")

            speakingstate = pObjSynth1.State.ToString

        End While
        pObjSynth1.Dispose()
        If wasplaying = True Then
            Me.AxWindowsMediaPlayer2.Ctlcontrols.play()
            wasplaying = False
        End If
        
    End Sub








    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Label13.Text = Itemssaid
        Label14.Text = Volume
        Label2.Text = warnings
        Label4.Text = enableweathercheck.ToString

    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Dim wasplaying As Boolean = False
        Timer3.Enabled = False
        If enableweathercheck Then
            Try

                Dim messageList As New Generic.List(Of String)
                'http://alerts.weather.gov/cap/wwaatmget.php?x=WIC079&y=0
                Using feedReader = XmlReader.Create("http://alerts.weather.gov/cap/wwaatmget.php?x=WIC079&y=0")
                    networkissues = 0
                    Dim feedContent = SyndicationFeed.Load(feedReader)
                    If feedContent Is Nothing Then Return
                    For Each item As Object In feedContent.Items

                        Dim tempstring As String = Convert.ToString(item.Title.Text)
                        If (Not tempstring.Equals(last_alert)) Then
                            Dim PObjSynth1 As New SpeechSynthesizer
                            If tempstring.Equals("There are no active watches, warnings or advisories") Then
                            Else
                                warnings = warnings + 1

                                last_alert = tempstring

                                If AxWindowsMediaPlayer2.playState = WMPLib.WMPPlayState.wmppsPlaying Then
                                    Me.AxWindowsMediaPlayer2.Ctlcontrols.pause()
                                    wasplaying = True
                                End If
                                


                                PObjSynth1.Volume = Volume
                                PObjSynth1.Rate = -1
                                PObjSynth1.SelectVoice("VW Bridget")
                                PObjSynth1.SpeakAsync(Convert.ToString(item.Title.Text))
                                Try
                                    PObjSynth1.SpeakAsync(Convert.ToString(item.summary.text))

                                Catch ex As Exception

                                End Try
                            End If
                            Try
                                Dim speakingstate As String = PObjSynth1.State

                                While speakingstate.ToString.Equals("1")

                                    speakingstate = PObjSynth1.State

                                End While
                            Catch ex As Exception

                            End Try
                            If wasplaying = True Then
                                Me.AxWindowsMediaPlayer2.Ctlcontrols.play()

                            End If
                           
                        End If


                    Next


                End Using

            Catch ex As Exception


                If networkissues > 1 Then
                    Speak("Network issues detected")

                End If
                networkissues = networkissues + 1
            End Try

        End If
        Timer3.Enabled = True
    End Sub

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
        If DateTime.Now.ToString("HH:mm:ss") Then

        End If
    End Sub
End Class
