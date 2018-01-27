Public Class dna
    'Author:Peter-Dziezyk:Skype:pdziezyk:dnaspider:14752239770:MVS2017cwu:1.27.2018:v2.2.5.13:cs202
    Private Declare Function GetAsyncKeyState Lib "user32.dll" (ByVal vKey As Int32) As UShort
    Private Declare Function SetCursorPos Lib "user32.dll" (ByVal X As Int32, ByVal Y As Int32) As UShort
    Private Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)
    Private Declare Sub keybd_event Lib "user32" Alias "keybd_event" (ByVal bVk As Integer, bScan As Integer, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)
    Sub leftclick()
        mouse_event(&H2, 0, 0, 0, 0)
        mouse_event(&H4, 0, 0, 0, 0)
    End Sub
    Sub lefthold()
        mouse_event(&H2, 0, 0, 0, 0)
    End Sub
    Sub leftrelease()
        mouse_event(&H4, 0, 0, 0, 0)
    End Sub
    Sub middleclick()
        mouse_event(&H20, 0, 0, 0, 0)
        mouse_event(&H40, 0, 0, 0, 0)
    End Sub
    Sub middlehold()
        mouse_event(&H20, 0, 0, 0, 0)
    End Sub
    Sub middlerelease()
        mouse_event(&H40, 0, 0, 0, 0)
    End Sub
    Sub rightclick()
        mouse_event(&H8, 0, 0, 0, 0) '&H2
        mouse_event(&H10, 0, 0, 0, 0) '&H4
    End Sub
    Sub righthold()
        mouse_event(&H8, 0, 0, 0, 0)
    End Sub
    Sub rightrelease()
        mouse_event(&H10, 0, 0, 0, 0)
    End Sub

    Sub webBg(ByVal s As String, show As Boolean, tabOrBg As Integer)
        If WebBrowser1.Visible = True And show = False Then
            WebBrowser1.Visible = False
            Exit Sub
        End If

        Dim xWB = WebBrowser1
        If tabOrBg = 1 Then
            xWB.Parent = TabPage3 'tab 1
            xWB.SendToBack() '
        ElseIf tabOrBg = 2 Then
            xWB.Parent = Me 'Me 2
        End If
        xWB.Visible = True
        xWB.Anchor = AnchorStyles.Top Or AnchorStyles.Right Or AnchorStyles.Bottom Or AnchorStyles.Left
        xWB.Show()
        xWB.Top = -15
        xWB.Left = -10

        xWB.Width = Me.Width + 12
        xWB.Height = Me.Height + 15
        xWB.Navigate(s)
        xWB.ScrollBarsEnabled = False
    End Sub

    Private Sub dna_Click(sender As Object, e As EventArgs) Handles Me.Click
        If GetAsyncKeyState(Keys.LControlKey) Then moveable()
    End Sub

    Sub moveable()
        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable And Me.ControlBox = False Then '7 

            If GetAsyncKeyState(Keys.LControlKey) Or GetAsyncKeyState(Keys.RControlKey) Then My.Settings.SettingMoveBar = True 'reset setting

            If TabControl1.Height = Me.Height + txtLength.Height - 15 - SplitContainer1.SplitterWidth Then
                TabControl1.Height += SplitContainer1.SplitterWidth 'hide move bar
                My.Settings.SettingMoveBar = False
            Else
                If My.Settings.SettingMoveBar = False Then 'load setting
                    TabControl1.Height = Me.Height + txtLength.Height - 15 - SplitContainer1.SplitterWidth 'show move bar
                    TabControl1.Height += SplitContainer1.SplitterWidth
                    Exit Sub
                End If
                TabControl1.Height = Me.Height + txtLength.Height - 15 - SplitContainer1.SplitterWidth 'show move bar
                My.Settings.SettingMoveBar = True
            End If

        End If
    End Sub

    Private Sub Form1_DoubleClick(sender As Object, e As EventArgs) Handles Me.DoubleClick
        Dim ts = txtString.ZoomFactor
        changeView()
        txtString.ZoomFactor = ts
        If Me.Height <= 120 Then TabControl1.Visible = False
    End Sub

    Sub reloadDb()
        Me.ListBox1.Items.Clear()

        Dim num As Integer = My.Settings.SettingTxtCodeLength 'code length

        containsws_g = False '‹›>
        For Each item As String In My.Settings.Settingdb
            If item.ToString.Contains("«ws»") Or item.ToString.Contains("«-ws»") Or
                item.ToString.Contains("‹") Or item.ToString.Contains("›") Or
                My.Settings.SettingIgnoreWhiteSpace = True Then containsws_g = True ' Else containsws_g = False 'white space

            If Microsoft.VisualBasic.Left(item, 1) = "«" Or Microsoft.VisualBasic.Left(item, 2) = "//" Or Microsoft.VisualBasic.Left(item, 1) = "'" Or Microsoft.VisualBasic.Right(item, 2) = "//" Or item.StartsWith("http") Then 'header
                ListBox1.Items.Add(item)
                Continue For
            End If

            If item.Length <= num Then 'error
                ListBox1.Items.Add(item)
                Continue For
            End If

            'no tab; make item with tab
            If Not GetChar(item, num + 1) = Chr(9) Then item = Microsoft.VisualBasic.Left(item, num) & Chr(9) & Microsoft.VisualBasic.Right(item, item.Length - num) 'if missing tab, reinsert

            ListBox1.Items.Add(item)  'print items to listbox
        Next

        'select 1st item
        If My.Settings.SettingStartFromBottom = True Then selectBottomItem()

        Me.Refresh()

    End Sub

    Sub saveSettings()
        Select Case WindowState
            Case FormWindowState.Normal
                My.Settings.SettingWindowState = 0
            Case FormWindowState.Minimized
                My.Settings.SettingWindowState = 1
            Case FormWindowState.Maximized
                My.Settings.SettingWindowState = 2
        End Select

        If My.Settings.SettingOpacity <= 0.1 Then My.Settings.SettingOpacity = 1
        My.Settings.SettingOpacity = Me.Opacity

        If txtString.Focused Then My.Settings.SettingTabIndex = 23 'txtString
        If ListBox1.Focused Then My.Settings.SettingTabIndex = 24 'lst
        My.Settings.SettingSelectionStart = txtString.SelectionStart
        My.Settings.SettingSelectionLength = txtString.SelectionLength
        My.Settings.txtStringText = txtString.Text 'save txtstring text
        My.Settings.txtStringZoomFactor = txtString.ZoomFactor 'save txtstring zoomfactor 

        If WindowState = 0 Then
            My.Settings.SettingHeight = Me.Height 'save height
            My.Settings.SettingWidth = Me.Width 'save width
        End If
        My.Settings.SettingLstFontSize = Me.ListBox1.Font.Size 'save listbox1 font size

        My.Settings.SettingSplitterDistanceMaster = Me.SplitContainer1.SplitterDistance

        'save views
        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable And Me.ControlBox = False Then My.Settings.SettingSizeableBorder = True Else My.Settings.SettingSizeableBorder = False 'sizeable
        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None And TabControl1.Visible = True Then My.Settings.SettingTabOnly = True Else My.Settings.SettingTabOnly = False 'tabOnly
        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None And TabControl1.Visible = False Then My.Settings.SettingBgOnly = True Else My.Settings.SettingBgOnly = False 'bgOnly
        If TabControl1.Top = -txtLength.Height - 1 And Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None And TabControl1.Visible = True Then My.Settings.SettingNSTabOnly = True Else My.Settings.SettingNSTabOnly = False 'nstabOnly

        'location

        If Me.Top <> -32000 Then My.Settings.SettingLocationTop = Me.Top
        If Me.Left <> -32000 Then My.Settings.SettingLocationLeft = Me.Left
        'My.Settings.SettingLocationTop = Me.Top
        'My.Settings.SettingLocationLeft = Me.Left

        'start hidden
        If HideToolStripMenuItem.Checked = True Then My.Settings.SettingHidden = True

        'listindex
        My.Settings.SettingLastListIndex = ListBox1.SelectedIndex

        'SplitContainer1.SplitterWidth = 23
        My.Settings.SettingSplitterWidth = SplitContainer1.SplitterWidth

        If TabControl1.Visible = False Then My.Settings.SettingHideTabsOnStartUp = True Else My.Settings.SettingHideTabsOnStartUp = False

        'db
        My.Settings.Save()

        'on
        If chk_timer1_on_val.Checked = True Then Timer1.Start()
    End Sub

    Sub autoRelease()
        keyRelease(Keys.RMenu)
        keyRelease(Keys.LMenu)
        keyRelease(Keys.Menu)
        shiftRelease()
        ctrlRelease()
    End Sub

    Private Sub dna_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        saveSettings()
        autoRelease()
    End Sub

    Sub tabStyleAppearance()

        'tabs style
        If My.Settings.SettingTabAppearance = 1 Then
            TabControl1.SizeMode = TabSizeMode.Normal
            TabControl1.Appearance = TabAppearance.Normal
        End If
        If My.Settings.SettingTabAppearance = 2 Then
            TabControl1.SizeMode = TabSizeMode.Normal
            TabControl1.Appearance = TabAppearance.Buttons
        End If
        If My.Settings.SettingTabAppearance = 3 Then
            TabControl1.SizeMode = TabSizeMode.Normal
            TabControl1.Appearance = TabAppearance.FlatButtons
        End If
        If My.Settings.SettingTabAppearance = 4 Then
            TabControl1.SizeMode = TabSizeMode.Fixed
            TabControl1.Appearance = TabAppearance.Normal
        End If
        If My.Settings.SettingTabAppearance = 5 Then
            TabControl1.SizeMode = TabSizeMode.Fixed
            TabControl1.Appearance = TabAppearance.Buttons
        End If
        If My.Settings.SettingTabAppearance = 6 Then
            TabControl1.SizeMode = TabSizeMode.Fixed
            TabControl1.Appearance = TabAppearance.FlatButtons
        End If
    End Sub

    Sub settipstyle()
        ToolTip1.IsBalloon = My.Settings.SettingTipBalloon
        ToolTip1.ForeColor = My.Settings.SettingTipForeColor
        ToolTip1.BackColor = My.Settings.SettingTipBackColor
    End Sub


    Sub mainColorSet()
        On Error Resume Next
        Me.ListBox1.ForeColor = My.Settings.SettingForeColor
        Me.txtString.ForeColor = My.Settings.SettingForeColor
        Me.txtLength.ForeColor = My.Settings.SettingForeColor
        'Me.BackColor = My.Settings.SettingForeColor
        Me.ForeColor = My.Settings.SettingForeColor

        TabPage1.ForeColor = My.Settings.SettingForeColor
        TabPage2.ForeColor = My.Settings.SettingForeColor
        TabPage4.ForeColor = My.Settings.SettingForeColor

        Me.ListBox1.BackColor = My.Settings.SettingBgColor
        Me.txtString.BackColor = My.Settings.SettingBgColor
        Me.txtLength.BackColor = My.Settings.SettingBgColor


        Me.ListBox1.BackgroundImage = Image.FromFile(My.Settings.SettingBgImg)
        Me.txtString.BackgroundImage = Image.FromFile(My.Settings.SettingBgImg)
        Me.txtLength.BackgroundImage = Image.FromFile(My.Settings.SettingBgImg)
        Me.TabPage1.BackgroundImage = Image.FromFile(My.Settings.SettingBgImg)
        Me.TabPage2.BackgroundImage = Image.FromFile(My.Settings.SettingBgImg)
        Me.TabPage3.BackgroundImage = Image.FromFile(My.Settings.SettingBgImg)
        Me.TabPage4.BackgroundImage = Image.FromFile(My.Settings.SettingBgImg)

        Me.BackgroundImage = Image.FromFile(My.Settings.SettingBgImgMain)

        If TabPage3.BackgroundImage.ToString = "" Then 'tab bgcolor
            TabPage4.BackColor = My.Settings.SettingTabColor
            TabPage3.BackColor = My.Settings.SettingTabColor
            TabPage2.BackColor = My.Settings.SettingTabColor
            TabPage1.BackColor = My.Settings.SettingTabColor

        End If

        If My.Settings.SettingBgImgMain = Nothing Then 'main bg color
            Me.BackColor = My.Settings.SettingMainBgColor
        End If

        'set border box
        If My.Settings.SettingBorder = True Then
            txtString.BorderStyle = BorderStyle.FixedSingle
            ListBox1.BorderStyle = BorderStyle.FixedSingle
        Else
            txtString.BorderStyle = BorderStyle.None
            ListBox1.BorderStyle = BorderStyle.None
        End If
        If My.Settings.SettingBorder2 = True Then SplitContainer1.BorderStyle = BorderStyle.FixedSingle
        If My.Settings.SettingBorder2 = False Then SplitContainer1.BorderStyle = BorderStyle.None

        'splitter.width
        SplitContainer1.SplitterWidth = My.Settings.SettingSplitterWidth
        ListBox1.Height = SplitContainer1.Panel1.Height

        'multi sb
        If My.Settings.SettingMulti = True And My.Settings.SettingViewMultiScrollBar = False Then ListBox1.Height = SplitContainer1.Panel1.Height + 33


        'tab style
        tabStyleAppearance()

        'tips
        settipstyle()

        'font
        If My.Settings.SettingFont.ToString > "" Then
            'txtString.Font = My.Settings.SettingFont
            ListBox1.Font = My.Settings.SettingFont
            Me.Font = My.Settings.SettingFont
            Me.ListBox1.Font = New System.Drawing.Font(txtString.Font.Name, My.Settings.SettingLstFontSize)
            My.Settings.SettingLstFontSize = ListBox1.Font.Size
        End If

        'redraw if font size pts >=16
        If TabControl1.Height > Me.Height Then
            Me.Height = My.Settings.SettingHeight
            Me.Width = My.Settings.SettingWidth
        End If

        TabControl1.Visible = False
        reStack()
        TabControl1.Visible = True

        sameClFx()

    End Sub
    Sub sameClFx()
        If My.Settings.SettingTabColor = My.Settings.SettingForeColor Then 'same color fix
            Dim nc As Color = Color.Lime
            If ForeColor = Color.Lime Then nc = Color.Black
            If ForeColor = Color.White Then nc = Color.Black
            If ForeColor = Color.Black And My.Settings.SettingTabColor = Color.Black Then nc = Color.White
            TabPage1.ForeColor = nc
            TabPage2.ForeColor = nc
            TabPage4.ForeColor = nc
        End If
    End Sub

    Sub mainColorPick()
        'break escape
        If GetAsyncKeyState(Keys.Pause) Then
            keyRelease(Keys.Pause)
            keyClear(Keys.Pause)
            Exit Sub
        End If

        'font color
        Dim x As String = My.Settings.SettingBgImg.ToString 'no tab img then color string
        If x = "" Then x = "no image set. using:(" & My.Settings.SettingBgColor.ToString & ")" '
        Dim xx As String = My.Settings.SettingBgImgMain.ToString 'no tab img then color string
        If xx = "" Then xx = "no image set. using:(" & My.Settings.SettingBgColor.ToString & ")" '

        Dim fc = MsgBox("fore color: " & LCase(My.Settings.SettingForeColor.ToString), vbYesNoCancel, "change fore colors?")
        If fc = MsgBoxResult.Cancel Then Exit Sub
        If fc = MsgBoxResult.Yes Then
            If ColorDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Me.ListBox1.ForeColor = ColorDialog1.Color
                Me.txtString.ForeColor = ColorDialog1.Color
                Me.txtLength.ForeColor = ColorDialog1.Color
                Me.ForeColor = ColorDialog1.Color
                TabPage1.ForeColor = ColorDialog1.Color
                TabPage2.ForeColor = ColorDialog1.Color
                TabPage4.ForeColor = ColorDialog1.Color
                My.Settings.SettingForeColor = ColorDialog1.Color
            ElseIf DialogResult.Cancel Then
                My.Settings.SettingForeColor = Nothing
                TabPage1.ForeColor = Nothing
                TabPage2.ForeColor = Nothing
                TabPage4.ForeColor = Nothing
                Me.ListBox1.ForeColor = Nothing
                Me.txtString.ForeColor = Nothing
                Me.txtLength.ForeColor = Nothing
                Me.ForeColor = Nothing
            End If

        End If

        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        Dim bc = MsgBox("back color: " & LCase(My.Settings.SettingBgColor.ToString), vbYesNoCancel, "change back color?")
        If bc = MsgBoxResult.Cancel Then Exit Sub
        If bc = vbYes Then
            If ColorDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Me.ListBox1.BackColor = ColorDialog1.Color
                Me.txtString.BackColor = ColorDialog1.Color
                Me.txtLength.BackColor = ColorDialog1.Color
                My.Settings.SettingBgColor = ColorDialog1.Color
            ElseIf DialogResult.Cancel Then
                Me.ListBox1.BackColor = Nothing
                Me.txtString.BackColor = Nothing
                Me.txtLength.BackColor = Nothing
                My.Settings.SettingBgColor = Nothing
            End If
            sameClFx()
        End If

tabimg:
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'tabs
        '"html background: " & LCase(My.Settings.SettingHtmlTab.ToString) & vbNewLine &
        Dim bgi = MsgBox("tab background image: " & LCase(My.Settings.SettingBgImg.ToString) & vbNewLine &
                         "tab background color: " & LCase(My.Settings.SettingTabColor.ToString), vbYesNoCancel, "change tabs background image or background color?")
        If bgi = MsgBoxResult.Cancel Then Exit Sub
        If bgi = MsgBoxResult.Yes Then
            If (OpenFileDialog1.ShowDialog() = DialogResult.OK) Then
                If OpenFileDialog1.FileName.EndsWith(".htm") Or OpenFileDialog1.FileName.EndsWith(".html") Then 'html bg
                    webBg(OpenFileDialog1.FileName, True, 1)
                    My.Settings.SettingHtmlTab = OpenFileDialog1.FileName.ToString 'settings html tab
                Else
                    Try
                        Me.ListBox1.BackgroundImage = Image.FromFile(OpenFileDialog1.FileName)
                        Me.txtString.BackgroundImage = Image.FromFile(OpenFileDialog1.FileName)
                        Me.txtLength.BackgroundImage = Image.FromFile(OpenFileDialog1.FileName)
                        Me.TabPage1.BackgroundImage = Image.FromFile(OpenFileDialog1.FileName)
                        Me.TabPage2.BackgroundImage = Image.FromFile(OpenFileDialog1.FileName)
                        Me.TabPage3.BackgroundImage = Image.FromFile(OpenFileDialog1.FileName)
                        Me.TabPage4.BackgroundImage = Image.FromFile(OpenFileDialog1.FileName)
                        My.Settings.SettingBgImg = OpenFileDialog1.FileName
                    Catch ex As Exception
                        MsgBox("error")
                        GoTo tabimg
                    End Try

                End If
            ElseIf DialogResult.Cancel Then
                If WebBrowser1.Visible = True And TabPage3.Text = "db" Then WebBrowser1.Visible = False
                If My.Settings.SettingHtmlBg > "" Then WebBrowser1.Visible = True
                My.Settings.SettingHtmlTab = ""
                My.Settings.SettingBgImg = ""
                Me.ListBox1.BackgroundImage = Nothing
                Me.txtString.BackgroundImage = Nothing
                Me.txtLength.BackgroundImage = Nothing
                Me.TabPage1.BackgroundImage = Nothing
                Me.TabPage2.BackgroundImage = Nothing
                Me.TabPage3.BackgroundImage = Nothing
                Me.TabPage4.BackgroundImage = Nothing
                'cancel goto tab bgcolors
                If My.Settings.SettingBgImg = "" Then
                    If ColorDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Me.TabPage3.BackColor = ColorDialog1.Color
                        Me.TabPage2.BackColor = ColorDialog1.Color
                        Me.TabPage1.BackColor = ColorDialog1.Color
                        Me.TabPage4.BackColor = ColorDialog1.Color
                        My.Settings.SettingTabColor = ColorDialog1.Color
                        sameClFx()
                    ElseIf DialogResult.Cancel Then
                        My.Settings.SettingTabColor = Nothing
                        Me.TabPage3.BackColor = Nothing
                        Me.TabPage2.BackColor = Nothing
                        Me.TabPage1.BackColor = Nothing
                        Me.TabPage4.BackColor = Nothing
                    End If
                End If
            End If
        End If

mainimg:
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'main img
        '"html background: " & LCase(My.Settings.SettingHtmlBg.ToString) & vbNewLine &
        editMainBgImg()

        'border box msg
        Dim v = True
        If My.Settings.SettingBorder = True Then
            v = False
        Else
            v = True
        End If


        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        Dim q = MsgBox("borders: " & LCase(My.Settings.SettingBorder) & vbNewLine & vbNewLine & "hold shift + no: toggle border #2", vbYesNoCancel, "change text box and list box borders to " & LCase(v) & "?")
        If q = MsgBoxResult.Cancel Then Exit Sub
        If q = MsgBoxResult.Yes Then

            My.Settings.SettingBorder = v
            If v = True Then
                txtString.BorderStyle = BorderStyle.FixedSingle
                ListBox1.BorderStyle = BorderStyle.FixedSingle
            Else
                txtString.BorderStyle = BorderStyle.None
                ListBox1.BorderStyle = BorderStyle.None
            End If

        Else
            If GetAsyncKeyState(Keys.LShiftKey) Or GetAsyncKeyState(Keys.RShiftKey) Then 'hold shift tweak border
                If SplitContainer1.BorderStyle = BorderStyle.None Then
                    My.Settings.SettingBorder2 = True
                    SplitContainer1.BorderStyle = BorderStyle.FixedSingle
                    SplitContainer1.Left = 3
                Else
                    My.Settings.SettingBorder2 = False
                    SplitContainer1.BorderStyle = BorderStyle.None
                    SplitContainer1.Left = 5
                End If
            End If
        End If

        'multi columns
        v = True
        If My.Settings.SettingMultiColumn = True Then
            v = False
        Else
            v = True
        End If
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        q = MsgBox("multi column: " & LCase(My.Settings.SettingMultiColumn), vbYesNoCancel, "change db multi colum to " & LCase(v) & "?")
        If q = MsgBoxResult.Cancel Then Exit Sub
        If q = MsgBoxResult.Yes Then
            If v = True Then
                ListBox1.MultiColumn = True
                My.Settings.SettingMultiColumn = True
            Else
                My.Settings.SettingMultiColumn = False
                ListBox1.MultiColumn = False
                ListBox1.Height = SplitContainer1.Panel1.Height
            End If
        End If

        'multi ColumnWidth
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        If My.Settings.SettingMultiColumn = True Then
            Dim w = InputBox("column width: " & My.Settings.SettingViewMultiWidth, "change column width?", My.Settings.SettingViewMultiWidth)
            If IsNumeric(w) Then
                If w >= 1480 Or w <= 5 Then w = 5
                ListBox1.ColumnWidth = w
                My.Settings.SettingViewMultiWidth = w
            End If
        End If '

        If My.Settings.SettingMultiColumn = True Then 'multi columns sb
            v = True
            If My.Settings.SettingViewMultiScrollBar = True Then
                v = False
            Else
                v = True
            End If
            q = MsgBox("multi column scroll bar: " & LCase(My.Settings.SettingViewMultiScrollBar), vbYesNoCancel, "change multi column scroll bar to " & LCase(v) & "?")

            If q = MsgBoxResult.Cancel Then Exit Sub
            If q = MsgBoxResult.Yes Then

                If v = True Then
                    My.Settings.SettingViewMultiScrollBar = True
                    ListBox1.Height = SplitContainer1.Panel1.Height
                Else
                    My.Settings.SettingViewMultiScrollBar = False
                    ListBox1.Height = SplitContainer1.Panel1.Height + 33
                End If

            Else
                If My.Settings.SettingViewMultiScrollBar = False Then ListBox1.Height = SplitContainer1.Panel1.Height + 33 'multi sb
            End If
        End If



        'splitter
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        Dim t = InputBox("splitter thickness: " & My.Settings.SettingSplitterWidth, "change workspace splitter bar thickness?", My.Settings.SettingSplitterWidth)
        If IsNumeric(t) Then
            If t >= 80 Or t <= 0 Then t = 43
            SplitContainer1.SplitterWidth = t
            My.Settings.SettingSplitterWidth = t
        End If


        '
        Dim br = My.Settings.SettingMoveBar

        'tabs style
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        Dim ta = InputBox("tab style appearance: " & My.Settings.SettingTabAppearance & vbNewLine & vbNewLine & "0: reset" & vbNewLine & "1: normal" & vbNewLine & "2: buttons" & vbNewLine & "3: flat buttons" & vbNewLine & "4: normal, fixed width" & vbNewLine & "5: buttons, fixed width" & vbNewLine & "6: flat buttons, fixed width" & vbNewLine & "7: hide tabs, sizeable" & vbNewLine & vbNewLine, "change tab style appearance?", "")
        If IsNumeric(ta) And ta > "" Then
            Select Case ta
                Case "0"
                    TabControl1.SizeMode = TabSizeMode.Normal
                    TabControl1.Appearance = TabAppearance.Normal
                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
                    Me.MinimizeBox = True
                    Me.MaximizeBox = True
                    Me.tipsDnaToolStripMenuItem.Checked = My.Settings.SettingDnaX
                    Me.ControlBox = True
                    lblMove.Visible = False
                    lblMoveTop.Visible = False
                    Me.ShowIcon = True
                    Me.BackColor = My.Settings.SettingBgColor
                    If My.Settings.SettingBgImgMain > "" Then Me.BackgroundImage = Image.FromFile(My.Settings.SettingBgImgMain)
                    reStack()
                    If My.Settings.SettingScrollBar = False Then showScrollBar(False)


                Case "1"
                    TabControl1.SizeMode = TabSizeMode.Normal
                    TabControl1.Appearance = TabAppearance.Normal
                Case "2"
                    TabControl1.SizeMode = TabSizeMode.Normal
                    TabControl1.Appearance = TabAppearance.Buttons
                Case "3"
                    TabControl1.SizeMode = TabSizeMode.Normal
                    TabControl1.Appearance = TabAppearance.FlatButtons
                Case "4"
                    TabControl1.SizeMode = TabSizeMode.Fixed
                    TabControl1.Appearance = TabAppearance.Normal
                Case "5"
                    TabControl1.SizeMode = TabSizeMode.Fixed
                    TabControl1.Appearance = TabAppearance.Buttons
                Case "6"
                    TabControl1.SizeMode = TabSizeMode.Fixed
                    TabControl1.Appearance = TabAppearance.FlatButtons
                Case "7"
                    preSizeable()
                    My.Settings.SettingSizeableBorder = True
                    'Case Else
            End Select

            My.Settings.SettingTabAppearance = ta
        End If

        ''tips .
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        v = True 'balloon
        If My.Settings.SettingTipBalloon = True Then v = False Else v = True
        Dim qt = MsgBox("use balloon style: " & LCase(My.Settings.SettingTipBalloon.ToString), vbYesNoCancel, "change tip balloon style to " & LCase(v) & "?")
        If qt = MsgBoxResult.Cancel Then Exit Sub
        If qt = MsgBoxResult.Yes Then My.Settings.SettingTipBalloon = v
        ToolTip1.IsBalloon = v

        ''scrollbar
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        v = True
        If My.Settings.SettingScrollBar = True Then v = False Else v = True
        Dim sb = MsgBox("show db tab scroll bars: " & LCase(My.Settings.SettingScrollBar.ToString), vbYesNoCancel, "change show scroll bars to " & LCase(v) & "?")
        If sb = MsgBoxResult.Cancel Then Exit Sub
        If sb = MsgBoxResult.Yes Then My.Settings.SettingScrollBar = v
        showScrollBar(v)


        If GetAsyncKeyState(Keys.Pause) Then Exit Sub

fnt:
        Try
            Dim fo = MsgBox("font: " & LCase(My.Settings.SettingFont.Name.ToString) & vbNewLine & "style: " & LCase(My.Settings.SettingFont.Style.ToString) & vbNewLine & "size: " & My.Settings.SettingFont.Size & vbNewLine & vbNewLine & "yes > cancel: reset to default font settings", vbYesNo, "change font?")
            If fo = MsgBoxResult.Yes Then
                If FontDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtString.Font = FontDialog1.Font
                        ListBox1.Font = FontDialog1.Font
                        Me.Font = FontDialog1.Font
                        My.Settings.SettingFont = FontDialog1.Font
                        My.Settings.SettingFontPass = True
                        txtString.ZoomFactor = 1
                    Catch ex As Exception
                        My.Settings.SettingFont = txtLength.Font
                        MsgBox("error")
                    End Try
                Else
                    txtString.Font = Nothing 'cancel font pick
                    txtString.ForeColor = ListBox1.ForeColor
                    ListBox1.Font = Nothing
                    Me.Font = Nothing
                    My.Settings.SettingFont = Nothing
                    My.Settings.SettingLstFontSize = Nothing
                    My.Settings.SettingFontPass = False
                    txtString.ZoomFactor = 1 '.01
                End If
            End If
        Catch ex As Exception
            'If (Err.Number = 91) Then 
            My.Settings.SettingFont = txtLength.Font
            GoTo fnt
        End Try

        'reStack()
        My.Settings.SettingLstFontSize = ListBox1.Font.Size


        If br = True Then moveBarRe()

        sameClFx()

    End Sub

    Sub moveBarRe()
        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable And ControlBox = False Then TabControl1.Height = Me.Height + txtLength.Height - 15 - SplitContainer1.SplitterWidth : My.Settings.SettingMoveBar = True 'if movebar showing, keep 
    End Sub

    Sub changeFont(tf As Boolean)
        Dim br = My.Settings.SettingMoveBar

        'On Error GoTo er1
        'font
        If tf = False Then GoTo tf
        'er1:

        If (Err.Number = 91) Then My.Settings.SettingFont = txtLength.Font

        Try
            If My.Settings.SettingFont.Name = Nothing Then Exit Sub
        Catch ex As Exception
            My.Settings.SettingFont = txtLength.Font
        End Try



        Dim fo = MsgBox("font: " & LCase(My.Settings.SettingFont.Name.ToString) & vbNewLine & "style: " & LCase(My.Settings.SettingFont.Style.ToString) & vbNewLine & "size: " & My.Settings.SettingFont.Size & vbNewLine & vbNewLine & "yes > cancel: reset to default font settings", vbYesNo, "change font?")
        If fo = MsgBoxResult.No Then Exit Sub
        If fo = MsgBoxResult.Yes Then
            If FontDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                txtString.Font = FontDialog1.Font
                ListBox1.Font = FontDialog1.Font
                Me.Font = FontDialog1.Font
                My.Settings.SettingFont = FontDialog1.Font
                My.Settings.SettingFontPass = True
                txtString.ZoomFactor = 1
            Else
tf:
                My.Settings.SettingTabAppearance = 1
                txtString.Font = Nothing 'cancel font pick
                txtString.ForeColor = ListBox1.ForeColor
                ListBox1.Font = Nothing
                Me.Font = Nothing
                My.Settings.SettingFont = Nothing
                My.Settings.SettingLstFontSize = Nothing
                My.Settings.SettingFontPass = False
                txtString.ZoomFactor = 1
            End If
        End If

        My.Settings.SettingLstFontSize = ListBox1.Font.Size

        Dim h As Integer
        Dim w As Integer
        Dim s As Integer
        If tf = False Then
            h = Me.Height
            w = Me.Width
            s = SplitContainer1.SplitterDistance
        End If

        reStack()
        reStyle()
        tabOnly()
        changeView()

        If My.Settings.SettingScrollBar = False Then
            showScrollBar(False)
        End If

        If SplitContainer1.BorderStyle = BorderStyle.FixedSingle And My.Settings.SettingScrollBar = True Then 'border
            SplitContainer1.Left = 4
        End If

        If br = True Then moveBarRe()

        If tf = False Then
            Me.Height = h
            Me.Width = w
            Try
                SplitContainer1.SplitterDistance = s
            Catch ex As Exception
            End Try
            Me.SplitContainer1.Height = TabControl1.Height - 34
            If Me.Height <= 80 Then TabControl1.Visible = False
            tabStyleAppearance()
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load 'remember options when form opens 
        keyRelease(Keys.Space) ':X
        keyClear(Keys.Space)

        'tip balloon /style
        settipstyle()

        'set global var for tooltip/tt 
        ttl = ToolTip1.GetToolTip(ListBox1)

        'set tabcontrol1.top g var
        ttop = TabControl1.Top

        TabControl1.Visible = False

        Dim s1 As Integer = My.Settings.splitterDistance 'dim setting

        Me.chk_timer1_on_val.Checked = My.Settings.SettingTimer1_chk_on_val 'main timer /engine enable
        Me.lbl_timer1_interval_val.Text = My.Settings.SettingTimer1_lbl_interval_val 'main timer / engine interval

        If lbl_timer1_interval_val.Text > 0 Then 'turn timer / engine on and set interval
            Timer1.Enabled = True
            Timer1.Interval = lbl_timer1_interval_val.Text
        End If

        Me.chk_top.Checked = My.Settings.SettingMain_chk_top 'top

        Me.chk_tips.Checked = My.Settings.SettingMain_chk_tips 'tool tips
        If chk_tips.Checked = False Then Me.ToolTip1.Active = False '

        txtLength.Text = My.Settings.SettingTxtCodeLength 'code length

        'txtString.ZoomFactor
        txtString.ZoomFactor = My.Settings.txtStringZoomFactor

        'txtstring text
        txtString.Text = My.Settings.txtStringText

        'check dna > x
        tipsDnaToolStripMenuItem.Checked = My.Settings.SettingDnaX

        'chk tips delete
        tipsDeleteToolStripMenuItem2.Checked = My.Settings.SettingTipsDelete

        'listbox1 font size
        Me.ListBox1.Font = New System.Drawing.Font(txtString.Font.Name, My.Settings.SettingLstFontSize)

        'ignore pausebreak
        PauseBreakToolStripMenuItem.Checked = My.Settings.SettingIgnorePauseBreak

        'ignore, wedge
        chkWedgee.Checked = My.Settings.SettingIgnoreWedgee
        PlayPauseToolStripMenuItem.Checked = My.Settings.SettingIgnoreMediaPlayPause 'media
        VolumeUpToolStripMenuItem.Checked = My.Settings.SettingIgnoreVoluemUp
        VolumeDownToolStripMenuItem.Checked = My.Settings.SettingIgnoreVolumeDown
        VolumeMuteToolStripMenuItem.Checked = My.Settings.SettingIgnoreVolumeMute
        PrintScreenToolStripMenuItem2.Checked = My.Settings.SettingIgnorePrintScreen
        PageUpToolStripMenuItem.Checked = My.Settings.SettingIgnorePageUp
        PageDownToolStripMenuItem.Checked = My.Settings.SettingIgnorePageDown
        HomeToolStripMenuItem1.Checked = My.Settings.SettingIgnoreHome
        EndToolStripMenuItem1.Checked = My.Settings.SettingIgnoreEnd

        chkAz.Checked = My.Settings.SettingChkAz
        AToolStripMenuItem.Checked = My.Settings.SettingIgnoreA 'a-z
        BToolStripMenuItem.Checked = My.Settings.SettingIgnoreB
        CToolStripMenuItem.Checked = My.Settings.SettingIgnoreC
        DToolStripMenuItem.Checked = My.Settings.SettingIgnoreD
        EToolStripMenuItem.Checked = My.Settings.SettingIgnoreE
        FToolStripMenuItem.Checked = My.Settings.SettingIgnoreF
        GToolStripMenuItem.Checked = My.Settings.SettingIgnoreG
        HToolStripMenuItem.Checked = My.Settings.SettingIgnoreH
        IToolStripMenuItem.Checked = My.Settings.SettingIgnoreI
        JToolStripMenuItem.Checked = My.Settings.SettingIgnoreJ
        KToolStripMenuItem.Checked = My.Settings.SettingIgnoreK
        LToolStripMenuItem.Checked = My.Settings.SettingIgnoreL
        MToolStripMenuItem.Checked = My.Settings.SettingIgnoreM
        NToolStripMenuItem.Checked = My.Settings.SettingIgnoreN
        OToolStripMenuItem.Checked = My.Settings.SettingIgnoreO
        PToolStripMenuItem.Checked = My.Settings.SettingIgnoreP
        QToolStripMenuItem.Checked = My.Settings.SettingIgnoreQ
        RToolStripMenuItem.Checked = My.Settings.SettingIgnoreR
        SToolStripMenuItem.Checked = My.Settings.SettingIgnoreS
        TToolStripMenuItem.Checked = My.Settings.SettingIgnoreT
        UToolStripMenuItem.Checked = My.Settings.SettingIgnoreU
        VToolStripMenuItem.Checked = My.Settings.SettingIgnoreV
        WToolStripMenuItem.Checked = My.Settings.SettingIgnoreW
        XToolStripMenuItem.Checked = My.Settings.SettingIgnoreX
        YToolStripMenuItem.Checked = My.Settings.SettingIgnoreY
        ZToolStripMenuItem.Checked = My.Settings.SettingIgnoreZ

        chk09.Checked = My.Settings.SettingChk09
        ToolStripMenuItem14.Checked = My.Settings.SettingIgnore0 'num 0-9
        ToolStripMenuItem15.Checked = My.Settings.SettingIgnore1
        ToolStripMenuItem16.Checked = My.Settings.SettingIgnore2
        ToolStripMenuItem17.Checked = My.Settings.SettingIgnore3
        ToolStripMenuItem18.Checked = My.Settings.SettingIgnore4
        ToolStripMenuItem19.Checked = My.Settings.SettingIgnore5
        ToolStripMenuItem20.Checked = My.Settings.SettingIgnore6
        ToolStripMenuItem21.Checked = My.Settings.SettingIgnore7
        ToolStripMenuItem22.Checked = My.Settings.SettingIgnore8
        ToolStripMenuItem23.Checked = My.Settings.SettingIgnore9

        chkF1f12.Checked = My.Settings.SettingChkF1f12
        F1ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref1 'f1-f12
        F2ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref2
        F3ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref3
        F4ToolStripMenuItem.Checked = My.Settings.SettingIgnoref4
        F5ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref5
        F6ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref6
        F7ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref7
        F8ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref8
        F9ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref9
        F10ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref10
        F11ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref11
        F12ToolStripMenuItem1.Checked = My.Settings.SettingIgnoref12

        chkMisc.Checked = My.Settings.SettingChkMisc 'misc
        ToolStripMenuItemChkMiscSc.Checked = My.Settings.SettingChkMiscSc
        ToolStripMenuItemChkMiscFs.Checked = My.Settings.SettingChkMiscFs
        ToolStripMenuItemChkMiscTil.Checked = My.Settings.SettingChkMiscTil
        ToolStripMenuItemChkMiscLb.Checked = My.Settings.SettingChkMiscLb
        ToolStripMenuItemChkMiscBs.Checked = My.Settings.SettingChkMiscBs
        ToolStripMenuItemChkMiscRb.Checked = My.Settings.SettingChkMiscRb
        ToolStripMenuItemChkMiscRem.Checked = My.Settings.SettingChkMiscRem
        ToolStripMenuItemChkMiscPeriod.Checked = My.Settings.SettingChkMiscPeriod
        ToolStripMenuItemChkMiscComma.Checked = My.Settings.SettingChkMiscComma
        ToolStripMenuItemChkMiscMinus.Checked = My.Settings.SettingChkMiscMinus
        ToolStripMenuItemChkMiscPlus.Checked = My.Settings.SettingChkMiscPlus '/misc

        chkNumPad.Checked = My.Settings.SettingChkNumbPad
        ToolStripMenuItem4.Checked = My.Settings.SettingIgnoren0 'num-pad 0-9
        ToolStripMenuItem5.Checked = My.Settings.SettingIgnoren1
        ToolStripMenuItem6.Checked = My.Settings.SettingIgnoren2
        ToolStripMenuItem7.Checked = My.Settings.SettingIgnoren3
        ToolStripMenuItem8.Checked = My.Settings.SettingIgnoren4
        ToolStripMenuItem9.Checked = My.Settings.SettingIgnoren5
        ToolStripMenuItem10.Checked = My.Settings.SettingIgnoren6
        ToolStripMenuItem11.Checked = My.Settings.SettingIgnoren7
        ToolStripMenuItem12.Checked = My.Settings.SettingIgnoren8
        ToolStripMenuItem13.Checked = My.Settings.SettingIgnoren9

        chkArrows.Checked = My.Settings.SettingChkArrows
        UpToolStripMenuItem1.Checked = My.Settings.SettingIgnoreup 'up, down, left, right
        DownToolStripMenuItem1.Checked = My.Settings.SettingIgnoredown
        LeftToolStripMenuItem1.Checked = My.Settings.SettingIgnoreleft
        RightToolStripMenuItem1.Checked = My.Settings.SettingIgnoreright

        PSToolStripMenuItem3.Checked = My.Settings.SettingIgnorePS
        chkOther.Checked = My.Settings.SettingChkOther 'other
        PauseToolStripMenuItem2.Checked = My.Settings.SettingIgnorePause
        AltToolStripMenuItem1.Checked = My.Settings.SettingChkOtherAlt
        SpaceToolStripMenuItem.Checked = My.Settings.SettingChkOtherSpace
        BackspaceToolStripMenuItem.Checked = My.Settings.SettingChkOtherBs
        ControlToolStripMenuItem.Checked = My.Settings.SettingChkOtherControl
        LeftControlToolStripMenuItem.Checked = My.Settings.SettingChkOtherLCtrl
        RightControlToolStripMenuItem.Checked = My.Settings.SettingChkOtherRCtrl
        EnterToolStripMenuItem1.Checked = My.Settings.SettingChkOtherEnter
        ShiftToolStripMenuItem1.Checked = My.Settings.SettingChkOtherShft
        LeftShiftToolStripMenuItem.Checked = My.Settings.SettingChkOtherLShft
        RightShiftToolStripMenuItem.Checked = My.Settings.SettingChkOtherRShft
        CapsToolStripMenuItem.Checked = My.Settings.SettingChkOtherCaps
        TabToolStripMenuItem1.Checked = My.Settings.SettingChkOtherTab
        InsertToolStripMenuItem1.Checked = My.Settings.SettingChkOtherIns
        WinToolStripMenuItem1.Checked = My.Settings.SettingChkOtherWin
        DeleteToolStripMenuItem2.Checked = My.Settings.SettingChkOtherDelete
        EscToolStripMenuItem1.Checked = My.Settings.SettingChkOtherEsc '/other
        '/ignore

        If Not ListBox1.Items.Count <= 0 Then ListBox1.SelectedItem() = ListBox1.Items.Item(0) 'select 1'st item


        Me.Height = My.Settings.SettingHeight 'height
        Me.Width = My.Settings.SettingWidth 'widht

        TabControl1.SelectTab(3) 'select db tab '9.27.13
        TabControl1.Visible = True


        'chk on shift + esc
        ShiftEscapeToolStripMenuItem.Checked = My.Settings.SettingChkOnShiftEscape

        'tags
        LongTagsToolStripMenuItem.Checked = My.Settings.SettingChkTags

        'other > right alt
        RightAltToolStripMenuItem.Checked = My.Settings.SettingChkOtherRightAlt
        LeftAltToolStripMenuItem.Checked = My.Settings.SettingChkOtherLeftAlt

        'multi & zone
        MultiToolStripMenuItem.Checked = My.Settings.SettingMulti
        ZoneToolStripMenuItem.Text = "zone: " & My.Settings.SettingZone

        'border2
        If My.Settings.SettingBorder2 = True Then SplitContainer1.BorderStyle = BorderStyle.FixedSingle

        'TabControl1.Appearance
        tabStyleAppearance()

        ttAdjust() 'tips

        TabControl1.Visible = False
        mainColorSet() 'mainColor()
        TabControl1.Visible = True

        'word wrap
        If My.Settings.SettingWordWrap = True Then
            txtString.WordWrap = False
            WordWrapToolStripMenuItem.Checked = True
        Else
            WordWrapToolStripMenuItem.Checked = False
            txtString.WordWrap = True
        End If

        'multi column
        If My.Settings.SettingMultiColumn = True Then ListBox1.MultiColumn = True
        If ListBox1.MultiColumn = True Then My.Settings.SettingMultiColumn = True

        'multi ColumnWidth
        If ListBox1.MultiColumn = True Then ListBox1.ColumnWidth = My.Settings.SettingViewMultiWidth()


        'tablet chk
        TabletToolStripMenuItem.Checked = My.Settings.SettingTabletSwipe


        'sizeable borders
        If My.Settings.SettingSizeableBorder = True Then sizeable()

        'icon
        If My.Settings.SettingIcon.ToString > "" Then
            Try
                Me.Icon = New Icon(My.Settings.SettingIcon) '
            Catch ex As Exception
            End Try
        End If

        'scrollbar
        If My.Settings.SettingScrollBar = True Then showScrollBar(True) Else showScrollBar(False)

        'fix height  if error
        If Me.Height < 20 And Me.Width < 20 Then
            Me.Height = 330
            Me.Width = 700
        End If

        'ctrl = .
        RightCtrllToolStripMenuItem.Checked = My.Settings.SettingRctrleqdot

        'osk
        OskToolStripMenuItem.Checked = My.Settings.SettingOsk

        'select 1st item
        If My.Settings.SettingStartFromBottom = True Then selectBottomItem()

        'ignore mouse
        If My.Settings.SettingMouse = True Then ChkMouse.Checked = True
        If My.Settings.SettingMouseLeft = True Then LeftClickToolStripMenuItem1.Checked = True
        If My.Settings.SettingMouseRight = True Then RightClickToolStripMenuItem1.Checked = True
        If My.Settings.SettingMouseM = True Then MiddleClickToolStripMenuItem1.Checked = True
        If My.Settings.SettingLscroll = True Then LscrollToolStripMenuItem.Checked = True
        If My.Settings.SettingRscroll = True Then RscrollToolStripMenuItem.Checked = True

        'startup tabonly style
        If My.Settings.SettingTabOnly = True Then tabOnly()
        'startup nstabonly style
        If My.Settings.SettingNSTabOnly = True Then nstabOnly()
        'startup bgonly style
        If My.Settings.SettingBgOnly = True Then bgOnly()

        'location
        Me.Top = My.Settings.SettingLocationTop
        Me.Left = My.Settings.SettingLocationLeft

        'Resize bottom container height
        spUp()
        spDown()

        'movebar
        'If My.Settings.SettingMoveBar = False Then moveable()
        If My.Settings.SettingSizeableBorder = True Then Me.Height += txtLength.Height

        'splitterdistance
        SplitContainer1.SplitterDistance = My.Settings.SettingSplitterDistanceMaster
        Me.Refresh()

        'reload last list index
        If My.Settings.SettingLastListIndex >= 0 And My.Settings.SettingLastListIndex > 0 Then ListBox1.SelectedIndex() = My.Settings.SettingLastListIndex

        'reselect
        If My.Settings.SettingStartFromBottom = True Then selectBottomItem()

        'set splitter val -default
        sp = False

        'ttdelay
        Me.ToolTip1.AutoPopDelay = 30999

        'lblmove
        lblMove.Left = Me.Width - lblMove.Width + 5

        'run 1st line when start
        If ListBox1.Items.Count > 0 Then
            If (ListBox1.Items.Item(0).ToString).Contains("«dna»") Then
                TextBox1.Text = "'"
                'Dim l = ListBox1.SelectedIndex
                ListBox1.SelectedIndex = 0
                Me.Hide()
                runList()
                TextBox1.Text = ""
                'ListBox1.SelectedIndex = l
            End If
        End If

        'listbox.focus
        If My.Settings.SettingTabAppearance = 1 Or My.Settings.SettingTabAppearance = 7 Then ListBox1.Focus()

        'start hidden
        If My.Settings.SettingHidden = True Then Me.Visible = False : HideToolStripMenuItem.Checked = True 'HideToolStripMenuItem.Checked = True

        ' w7
        If (My.Computer.Info.OSFullName).Contains("Windows 7") Then
            My.Settings.SettingChkW7 = True
        End If

        sizeFix() 'fix

        htmlBg(1) 'html tab
        htmlBg(2) 'html bg

        'sizeable
        If GetAsyncKeyState(Keys.Space) Then sizeable()

        'txt lst width
        If Me.Visible = True Then
            Me.Top += 1
            Me.Top -= 1
        End If

        'bg img layout
        Select Case My.Settings.SettingBackgroundImageLayout
            Case 0
                Me.BackgroundImageLayout = 0'none
            Case 1
                Me.BackgroundImageLayout = 1'tile
            Case 2
                Me.BackgroundImageLayout = 2'center
            Case 3
                Me.BackgroundImageLayout = 3'stretch
            Case 4
                Me.BackgroundImageLayout = 4 'zoom

            Case Else
                Me.BackgroundImageLayout = 1
        End Select

        'view chk
        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable And Me.ControlBox = False And Me.TabControl1.Visible = False And Me.Text = "" Then TabControl1.Visible = True

        'size open err
        If Me.Width <= 176 And Me.MaximizeBox = True And Me.MinimizeBox = True Then
            Me.MaximizeBox = False
            Me.MinimizeBox = False
        End If

        'gc
        If My.Settings.SettingGCCollect = True Then GC.Collect()

        If My.Settings.SettingRctrleqMod = "»" And RightCtrllToolStripMenuItem.Checked = True And My.Settings.SettingAutoLockEmode = True Then 'freeze 
            TextBox1.Text = "»"
            dnaTxt()
        Else
            TextBox1.Clear()
        End If

        If My.Settings.SettingShowIcon = False Then Me.ShowIcon = False

        'no length mode
        NoLengthToolStripMenuItem.Checked = My.Settings.SettingNoLengthMode
        If My.Settings.SettingNoLengthMode = True Then txtLength.Visible = False

        'g_ SettingMaxKeyLen
        g_maxkeylen = My.Settings.SettingMaxKeyLength

        If My.Settings.SettingHidden = False Then Me.Show()
        If ListBox1.Font.Size = 15.75 And ListBox1.Font.Name = "Impact" Then dbfocus() 'db tab

        Select Case My.Settings.SettingTabIndex
            Case 23
                txtString.Focus()
            Case 24
                ListBox1.Focus()
            Case Else
                ListBox1.Focus()
        End Select

        If txtString.Focused Then
            txtString.SelectionStart = My.Settings.SettingSelectionStart
            txtString.SelectionLength = My.Settings.SettingSelectionLength
        End If

        Me.Opacity = My.Settings.SettingOpacity

        Select Case My.Settings.SettingWindowState
            Case 0
                WindowState = FormWindowState.Normal
            Case 1
                WindowState = FormWindowState.Minimized
            Case 2
                WindowState = FormWindowState.Maximized
        End Select

        'splitterwidth
        SplitContainer1.SplitterWidth = My.Settings.SettingSplitterWidth

    End Sub

    Sub sizeFix()
        Me.Width = My.Settings.SettingWidth
        Me.Height = My.Settings.SettingHeight
        If Me.Width <= 136 Then TabControl1.Visible = False
        If Me.Height <= 144 Then TabControl1.Visible = False ' 39
        If My.Settings.SettingHideTabsOnStartUp = True Then TabControl1.Visible = False
    End Sub

    Sub htmlBg(opt As Integer)
        Dim mysetting As String = ""
        Select Case opt
            Case 1
                mysetting = My.Settings.SettingHtmlTab
            Case 2
                mysetting = My.Settings.SettingHtmlBg
        End Select

        If mysetting > "" And
            mysetting.EndsWith(".htm") Or
            mysetting.EndsWith(".html") Then  'settings html tab
            Try
                webBg(mysetting, True, opt)
                If chk_tips.Checked = True Then MsgBox("(dna > «st + enter)" & vbNewLine & vbNewLine & "right ctrl + st + enter: hide/show tabs", vbOKOnly, vbInformation)
                lblMove.Hide()
                lblMoveTop.Hide()
            Catch

            End Try
        End If
    End Sub

    Sub nstabOnly()
        Me.BackColor = Color.GhostWhite
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        If Me.tipsDnaToolStripMenuItem.Checked = True Then Me.tipsDnaToolStripMenuItem.Checked = False
        Me.Text = ""
        Me.ControlBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        TabControl1.Top = -txtLength.Height - 1
        TabControl1.SizeMode = TabSizeMode.Fixed
        TabControl1.Appearance = TabAppearance.FlatButtons
        TabControl1.Visible = True
        showMoveBar()
        Me.Height = My.Settings.SettingHeight
        Me.Width = My.Settings.SettingWidth
        SplitContainer1.SplitterDistance = My.Settings.SettingSplitterDistanceMaster
        If My.Settings.SettingStartFromBottom = True Then selectBottomItem()
    End Sub
    Sub tabOnly()
        Me.BackColor = Color.GhostWhite
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        If Me.tipsDnaToolStripMenuItem.Checked = True Then Me.tipsDnaToolStripMenuItem.Checked = False
        Me.Text = ""
        Me.ControlBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        TabControl1.Visible = True
        showMoveBar()
        Me.Height = My.Settings.SettingHeight
        Me.Width = My.Settings.SettingWidth
        Try
            If My.Settings.SettingSplitterDistanceMaster > 0 Then SplitContainer1.SplitterDistance = My.Settings.SettingSplitterDistanceMaster
        Catch ex As Exception
        End Try
        If My.Settings.SettingStartFromBottom = True Then selectBottomItem()
    End Sub
    Sub bgOnly()
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        If Me.tipsDnaToolStripMenuItem.Checked = True Then Me.tipsDnaToolStripMenuItem.Checked = False
        Me.Text = ""
        Me.ControlBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        TabControl1.Visible = False
        'Me.BackColor = Color.GhostWhite
        showMoveBar()
        Me.Height = My.Settings.SettingHeight
        Me.Width = My.Settings.SettingWidth
        If My.Settings.SettingStartFromBottom = True Then selectBottomItem()
        If My.Settings.SettingBgImgMain > "" Then Me.BackColor = Color.GhostWhite
    End Sub

    Sub hideTT()
        If chk_tips.Checked = False Then
            If GetAsyncKeyState(Keys.F1) Then Exit Sub
            ToolTip1.Active = False
            Exit Sub
        End If
    End Sub
    Sub ttAdjust()
        hideTT()

        SpacerToolStripMenuItem.Text = "spacer: " & My.Settings.SettingSpacer 'spacer mnu txt 
        ZoneToolStripMenuItem.Text = "zone: " & My.Settings.SettingZone 'zone mnu txt

        If chk_tips.CheckState = CheckState.Unchecked Then 'tooltip adjust when right clicked clears
            OskToolStripMenuItem.ToolTipText = ""
            HideToolStripMenuItem.ToolTipText = ""
            SendkeysToolStripMenuItem.ToolTipText = ""
            AudioToolStripMenuItem.ToolTipText = ""
            EditToolStripMenuItem.ToolTipText = ""
            DeleteToolStripMenuItem.ToolTipText = ""
            AddToolStripMenuItem.ToolTipText = ""
            XyToolStripMenuItem.ToolTipText = ""
            CbToolStripMenuItem.ToolTipText = ""
            UrlToolStripMenuItem.ToolTipText = ""
            ReplaceToolStripMenuItem.ToolTipText = ""
            ReturnMouseToolStripMenuItem.ToolTipText = ""
            XyToolStripMenuItem.ToolTipText = ""
            BsToolStripMenuItem.ToolTipText = ""
            EscToolStripMenuItem.ToolTipText = ""
            PgupToolStripMenuItem.ToolTipText = ""
            PgdownToolStripMenuItem.ToolTipText = ""
            AppToolStripMenuItem.ToolTipText = ""
            WebToolStripMenuItem.ToolTipText = ""
            ExportToolStripMenuItem.ToolTipText = ""
            ImportToolStripMenuItem.ToolTipText = ""
            TimeoutToolStripMenuItem.ToolTipText = ""
            tipsDnaToolStripMenuItem.ToolTipText = ""
            CbToolStripMenuItem.ToolTipText = ""
            UrlToolStripMenuItem.ToolTipText = ""
            tipsDeleteToolStripMenuItem2.ToolTipText = ""
            DeleteAllToolStripMenuItem.ToolTipText = ""
            NumberToolStripMenuItem.ToolTipText = ""
            LetterToolStripMenuItem.ToolTipText = ""
            ShiftEscapeToolStripMenuItem.ToolTipText = ""
            InsertHereToolStripMenuItem.ToolTipText = ""
            LongTagsToolStripMenuItem.ToolTipText = ""
            LeftHoldToolStripMenuItem.ToolTipText = ""
            RightHoldToolStripMenuItem.ToolTipText = ""
            RightReleaseToolStripMenuItem.ToolTipText = ""
            LeftReleaseToolStripMenuItem.ToolTipText = ""
            PauseToolStripMenuItem1.ToolTipText = ""
            UndoToolStripMenuItem.ToolTipText = ""
            RedoToolStripMenuItem.ToolTipText = ""
            TabPage3.ToolTipText = ""
            CopyToolStripMenuItem.ToolTipText = ""
            CopyToolStripMenuItem1.ToolTipText = ""
            PasteToolStripMenuItem.ToolTipText = ""
            ClearToolStripMenuItem.ToolTipText = ""
            WordWrapToolStripMenuItem.ToolTipText = ""
            MultiToolStripMenuItem.ToolTipText = ""
            ZoneToolStripMenuItem.ToolTipText = ""
            ClickSwipeToolStripMenuItem.ToolTipText = ""
            ExitToolStripMenuItem.ToolTipText = ""
            KeyboardToolStripMenuItem.ToolTipText = ""
            MouseToolStripMenuItem.ToolTipText = ""
            InternalsToolStripMenuItem.ToolTipText = ""
            TabletToolStripMenuItem.ToolTipText = ""
            SpacerToolStripMenuItem.ToolTipText = ""
            RightCtrllToolStripMenuItem.ToolTipText = ""
            WaitToolStripMenuItem.ToolTipText = ""
            OptionsToolStripMenuItem.ToolTipText = ""
            SkinToolStripMenuItem.ToolTipText = ""
            LengthToolStripMenuItem.ToolTipText = ""
            ToolStripMenuItem28.ToolTipText = ""
            ToolStripMenuItem29.ToolTipText = ""
            ToolStripMenuItem30.ToolTipText = ""
            ToolStripMenuItem31.ToolTipText = ""
            If My.Settings.SettingDbTip = True Then OnToolStripMenuItem.ToolTipText = "dna > " & TextBox1.Text Else OnToolStripMenuItem.ToolTipText = ""
            NoLengthToolStripMenuItem.ToolTipText = ""
            PauseBreakToolStripMenuItem.ToolTipText = ""

        Else 'tooltip adjust when right fills
            PauseBreakToolStripMenuItem.ToolTipText = "press pause break key: clear, abort"
            NoLengthToolStripMenuItem.ToolTipText = "no length run mode." & vbNewLine & "press right ctrl, release right ctrl, then type a beginning db «code» to run" & vbNewLine & "example: dna > «code" & vbNewLine & "tip: add '«code-»test' to db, then rctrl, «code» (or lctrl + lshift + bs, «code»)"
            CopyToolStripMenuItem1.ToolTipText = "left click: copy db item" & vbNewLine & "right click: clipboard to db"
            OnToolStripMenuItem.ToolTipText = "dna > " & TextBox1.Text & vbNewLine & "interval: " & lbl_timer1_interval_val.Text & vbNewLine & "left click: toggle engine" & vbNewLine & "right click: toggle tabs"
            ToolStripMenuItem28.ToolTipText = "output clipboard value + # | example: «+:5»"
            ToolStripMenuItem29.ToolTipText = "output clipboard value - # | example: «-:5»"
            ToolStripMenuItem30.ToolTipText = "output clipboard value + 1 | " & Val(Clipboard.GetText) - 1
            ToolStripMenuItem31.ToolTipText = "output clipboard value - 1 | " & Val(Clipboard.GetText) - 1
            LengthToolStripMenuItem.ToolTipText = "left click: change dna > 'key length' (" & txtLength.Text & ")" & vbNewLine & "right click: toggle no length mode" & vbNewLine & "al + enter: toggle auto lock"
            SkinToolStripMenuItem.ToolTipText = "customize app appearance" & vbNewLine & "pause break: abort"
            OptionsToolStripMenuItem.ToolTipText = "database options"
            WaitToolStripMenuItem.ToolTipText = "wait, sleep, timeout, pause"
            OskToolStripMenuItem.ToolTipText = "checked: on screen keyboard auto release ctrl, shift, alt before run"
            RightCtrllToolStripMenuItem.ToolTipText = "click: change custom dna > i/o for right ctrl" & vbNewLine & "checked: custom right ctrl enabled" & vbNewLine & vbNewLine & "tip: set rctrl dna > i/o to '»'; save energy mode" & vbNewLine & vbNewLine & "right ctrl: toggle modes" & vbNewLine & "0% CPU e-mode (») -> run algorithm («) -> run (code length)"
            SpacerToolStripMenuItem.ToolTipText = "spacer: " & My.Settings.SettingSpacer & vbNewLine & "adjust spacer/pause amount between swipe and run"
            TabletToolStripMenuItem.ToolTipText = "db item swipe option" & vbNewLine & "uncheck: swipe db item to show db menu" & vbNewLine & "check: swipe db item to run"
            InternalsToolStripMenuItem.ToolTipText = "api internals menu"
            MouseToolStripMenuItem.ToolTipText = "api mouse menu"
            KeyboardToolStripMenuItem.ToolTipText = "api keyboard menu (simulate keyboard)"
            ExitToolStripMenuItem.ToolTipText = "alt + f4: close" & vbNewLine & "right click: save & restart"
            ClickSwipeToolStripMenuItem.ToolTipText = "swipe settings" & vbNewLine & "tip: to swipe, simply click + hold, or right/long click + hold + " & vbNewLine & "move/swipe finger/cursor over db tab or a db item in the list"
            ZoneToolStripMenuItem.ToolTipText = "swipe zone: " & My.Settings.SettingZone & " | swipe move distance (1=short)"
            MultiToolStripMenuItem.ToolTipText = "multi swipe/multi run when swiped"
            WordWrapToolStripMenuItem.ToolTipText = "word wrap: " & LCase(My.Settings.SettingWordWrap.ToString)
            CopyToolStripMenuItem.ToolTipText = "f2: copy" & vbNewLine & "right click: toggle >, <"
            PasteToolStripMenuItem.ToolTipText = "f3: paste"
            If txtString.SelectedText > "" Then ClearToolStripMenuItem.ToolTipText = "clear '" & Microsoft.VisualBasic.Left(txtString.SelectedText, My.Settings.SettingTxtCodeLength) & "'" Else ClearToolStripMenuItem.ToolTipText = "f4: clear '" & Microsoft.VisualBasic.Left(txtString.Text, My.Settings.SettingTxtCodeLength) & "'"
            UndoToolStripMenuItem.ToolTipText = "ctrl + z"
            RedoToolStripMenuItem.ToolTipText = "ctrl + y"
            PauseToolStripMenuItem1.ToolTipText = "«milliseconds:#», «ms:#», «m:#», «pause:#», «p:#», or «Timeout:#» | # = milliseconds" & vbNewLine & "pause/break key: cancel"
            LeftReleaseToolStripMenuItem.ToolTipText = "release left click"
            RightReleaseToolStripMenuItem.ToolTipText = "release right click"
            RightHoldToolStripMenuItem.ToolTipText = "hold right click down"
            LeftHoldToolStripMenuItem.ToolTipText = "hold left click down"
            LongTagsToolStripMenuItem.ToolTipText = "check: print «algorithm-codes» from menu" & vbNewLine & "uncheck: print shortcut algorithm code symbols"
            InsertHereToolStripMenuItem.ToolTipText = "//add item here" & vbNewLine & vbNewLine & "example: test«//misc»1234" '& vbNewLine & "note: above format only (beginning of message)" & vbNewLine & "also must have added //misc previously"
            ShiftEscapeToolStripMenuItem.ToolTipText = "left shift + escape: check to enable keyboard shut off"
            LetterToolStripMenuItem.ToolTipText = "random letter generator (ex. «x»=x, «X»=X)"
            NumberToolStripMenuItem.ToolTipText = "random number generator (ex. «#» or «#:1-3»)"
            DeleteAllToolStripMenuItem.ToolTipText = "left click: permanently delete all " & ListBox1.Items.Count & " items in db" & vbNewLine & "right click: temporarily delete all items"
            tipsDeleteToolStripMenuItem2.ToolTipText = "show yes no message before deleting item"
            UrlToolStripMenuItem.ToolTipText = "left click: open url example: «win»r«-win»«m»c:\dna.png«enter»«s»" & vbNewLine & "right click: «url:notepad.exe»" & vbNewLine & "shift + right click: manual «url:» example: «url:c:\notepad.exe»"
            CbToolStripMenuItem.ToolTipText = "«clipboard:copy to clipboard» or «cb:»"
            tipsDnaToolStripMenuItem.ToolTipText = "always show 'dna > code keys'"
            TimeoutToolStripMenuItem.ToolTipText = "«wait:#», «seconds:#», «w:#», «s:#» | # = seconds" & vbNewLine & "pause/break key: cancel"
            ExportToolStripMenuItem.ToolTipText = "left click: export db items to text" & vbNewLine & "escape: cancel" & vbNewLine & "right click: export db to .txt file"
            ImportToolStripMenuItem.ToolTipText = "import (" & txtString.Lines.Length & ") to db" & vbNewLine & "escape: cancel"
            WebToolStripMenuItem.ToolTipText = "«web site address:aol.com»"
            AppToolStripMenuItem.ToolTipText = "app activate" & vbNewLine & vbNewLine & "«App:Processor ID #» or " & vbNewLine & "«app:Title»" & vbNewLine & vbNewLine & "examples:" & vbNewLine & vbNewLine & "«app:notepad»" & vbNewLine & "tip: Title can be found in task manager" & vbNewLine & "next to programs icon" & vbNewLine & vbNewLine & "«App:101»" & vbNewLine & "tip: PID can be found in task manager," & vbNewLine & "Processes, PID (right click column , PID)"
            PgdownToolStripMenuItem.ToolTipText = "page down"
            PgupToolStripMenuItem.ToolTipText = "page up"
            EscToolStripMenuItem.ToolTipText = "escape"
            BsToolStripMenuItem.ToolTipText = "backspace"
            ReturnMouseToolStripMenuItem.ToolTipText = "return mouse cursor to original position"
            XyToolStripMenuItem.ToolTipText = "press escape or p key: to get mouse cursor x and y position «xy:" & lblX.Text & "-" & lblY.Text & "»" & vbNewLine & "left click: print cursor x y position (press escape or p key first)" & vbNewLine & "right click, f8, or x + y: three second count down, get position, print «xy:X-Y» or ctrl + p after count down"
            ReplaceToolStripMenuItem.ToolTipText = "«replace from clipboard:replace this|with this»"
            CbToolStripMenuItem.ToolTipText = "«cb:copy to clipboard»"
            EditToolStripMenuItem.ToolTipText = "edit: '" & Microsoft.VisualBasic.Left(ListBox1.SelectedItem, My.Settings.SettingTxtCodeLength) & "'"
            DeleteToolStripMenuItem.ToolTipText = "left click: delete '" & Microsoft.VisualBasic.Left(ListBox1.SelectedItem, My.Settings.SettingTxtCodeLength) & "' from db" & vbNewLine & "right click: temporarily delete"
            AddToolStripMenuItem.ToolTipText = "left click: add '" & Microsoft.VisualBasic.Left(txtString.Text, My.Settings.SettingTxtCodeLength) & "' to db" & vbNewLine & "right click: temporarily add '" & Microsoft.VisualBasic.Left(txtString.Text, My.Settings.SettingTxtCodeLength) & "' to db"
            SendkeysToolStripMenuItem.ToolTipText = "«sendkeys:^+~%{x #}»" & vbNewLine & "{up}              up" & vbNewLine & "+                  shift" & vbNewLine & "~                  enter" & vbNewLine & "%                 alt" & vbNewLine & "{esc}            escape" & vbNewLine & "{tab}             tab" & vbNewLine & "^                   ctrl" & vbNewLine & "{bs}               backspace" & vbNewLine & "{left}             left" & vbNewLine & "{right 3}       right*3" & vbNewLine & "{down}        down" & vbNewLine & "{pgup}         page up" & vbNewLine & "{pgdn}         page down" & vbNewLine & "{home}        home" & vbNewLine & "{end}            end" & vbNewLine & "{delete}        delete" & vbNewLine & "print top row: +#" & vbNewLine & "ex: +1, +2, or +[ (!,@,{)"
            AudioToolStripMenuItem.ToolTipText = "select .wav file" & vbNewLine & "shift + click: manual «audio:c:/?.wav», «stop-audio» | play, stop .wav"
            HideToolStripMenuItem.ToolTipText = "h + escape: hide/show" & vbNewLine & "right click: toggle start hidden" '& vbNewLine & ""
        End If
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown, lblMove.MouseDown, lblMoveTop.MouseDown
        dragfrm()
        If MouseButtons = Windows.Forms.MouseButtons.Right Then 'right click form to show options popupmenu
            If GetAsyncKeyState(Keys.LShiftKey) Then
                toggleTabControl1Show()
                Exit Sub
            End If

            If GetAsyncKeyState(Keys.LControlKey) Then
                chkItem(chk_timer1_on_val) 'toggle
                Exit Sub
            End If
            dz = 0
            showOptionsMenu()
        End If
    End Sub

    Dim b As Integer = 0 'tablet right click /slide

    Private Sub chk_timer1_val_CheckedChanged(sender As Object, e As EventArgs) Handles chk_timer1_on_val.CheckedChanged
        Dim tOO

        If Me.chk_tips.Checked = True And Me.chk_timer1_on_val.Checked = False Then 'tt when left control + shift
            ContextMenuStripOptions.Hide()
            Dim se = ""
            If ShiftEscapeToolStripMenuItem.Checked = False Then se = "dna > " Else se = "shift + escape"
            tOO = MsgBox("engine on?", vbYesNo, se)
            If tOO = vbYes Then chk_timer1_on_val.Checked = True
        End If

        My.Settings.SettingTimer1_chk_on_val = Me.chk_timer1_on_val.CheckState 'save timer / engine checkbox option

        keyHold(Keys.Pause)
        timeout2(77)
        keyRelease(Keys.Pause)
        clearAllKeys()

        If Me.chk_timer1_on_val.Checked = False Then dnaTxt()
        If Me.chk_timer1_on_val.Checked = True Then emode()
    End Sub
    Sub changeInterval()
        On Error GoTo p
        Dim pd As String
        pd = InputBox("interval" + Chr(13) + "ex: 150", "change interval", lbl_timer1_interval_val.Text)
        If pd = "" Then Exit Sub
        If IsNumeric(pd) And pd > 0 And pd < 999999999 Then
            lbl_timer1_interval_val.Text = pd
            Me.Timer1.Interval = pd
        Else
p:
            lbl_timer1_interval_val.Text = 150 ' My.Settings.SettingTimer1_lbl_interval_val  'default
            Me.Timer1.Interval = My.Settings.SettingTimer1_lbl_interval_val '
        End If
    End Sub

    Private Sub lbl_timer1_interval_val_DoubleClick(sender As Object, e As EventArgs) Handles lbl_timer1_interval_val.DoubleClick, Label1.DoubleClick
        changeInterval()
    End Sub

    Private Sub lbl_timer1_interval_val_MouseMove(sender As Object, e As MouseEventArgs) Handles lbl_timer1_interval_val.MouseMove
        If MouseButtons = Windows.Forms.MouseButtons.Left Then 'left click drag to increase value +1
            Me.lbl_timer1_interval_val.Text += 1
        End If
        If MouseButtons = Windows.Forms.MouseButtons.Right Then 'right click drag to decrease value -1
            Me.lbl_timer1_interval_val.Text -= 1
            If Me.lbl_timer1_interval_val.Text < 0 Then Me.lbl_timer1_interval_val.Text = 0 'reset to 0 if < 0
        End If
    End Sub

    Private Sub lbl_timer1_interval_val_MouseUp(sender As Object, e As MouseEventArgs) Handles lbl_timer1_interval_val.MouseUp
        If Me.lbl_timer1_interval_val.Text <= 0 Then Me.lbl_timer1_interval_val.Text = 1 'reset to 0 if < 0
        Me.Timer1.Interval = Me.lbl_timer1_interval_val.Text 'respawn timer1 interval
        My.Settings.SettingTimer1_lbl_interval_val = (lbl_timer1_interval_val.Text) 'save timer1 interval
    End Sub

    Private Sub chk_top_CheckedChanged(sender As Object, e As EventArgs) Handles chk_top.CheckedChanged
        My.Settings.SettingMain_chk_top = Me.chk_top.CheckState  'save chk top
        If Me.chk_top.CheckState = CheckState.Checked Then Me.TopMost = True Else Me.TopMost = False 'make form stay on top
    End Sub

    Private Sub TabControl1_Click(sender As Object, e As EventArgs) Handles TabControl1.Click
        dz = 0
    End Sub

    Private Sub TabControl1_DoubleClick(sender As Object, e As EventArgs) Handles TabControl1.DoubleClick
        dz = 0

        If TabPage3.Text = "browser" Then
            TabPage3.Text = "db"
            If My.Settings.SettingHtmlBg > "" Then
                htmlBg(2)
            Else
                WebBrowser1.Visible = False
            End If
            SplitContainer1.SplitterDistance = My.Settings.SettingSplitterDistanceMaster
            Exit Sub
        End If
        If Me.TabPage3.Focus = True And GetAsyncKeyState(Keys.LControlKey) Then
            showBrowserTab()
            Exit Sub
        End If

        If Me.TabPage3.Focus = True And Me.ControlBox = True And Me.Text > "" Then runMousePosition()
    End Sub

    Private Sub TabControl1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TabControl1.KeyPress
        'Clipboard.SetText(e.KeyChar) 'copy escape char

        If txtString.Focus = True Then 'no hide tabs, cursor to txtbox
            txtString.Focus()
            Exit Sub
        End If
    End Sub

    Sub tip()
        My.Settings.SettingMain_chk_tips = Me.chk_tips.CheckState  'save tooltip option
        If Me.chk_tips.Checked = True Then ToolTip1.Active = True Else ToolTip1.Active = False 'respawn tooltip option
        If Me.chk_tips.Checked = True Then TextBox1.PasswordChar = "" Else TextBox1.PasswordChar = "*"
        If Me.chk_tips.Checked = False Then dnaTxt() : TextBox1.PasswordChar = "*" : TabPage1.ToolTipText = "" : TabPage2.ToolTipText = "" : TabPage3.ToolTipText = "" : TabPage4.ToolTipText = ""
        reStyle()
    End Sub
    Private Sub chk_tips_CheckedChanged(sender As Object, e As EventArgs) Handles chk_tips.CheckedChanged
        tip()
        settipstyle()
        Me.ToolTip1.AutoPopDelay = 30999
    End Sub

    Sub clearAllKeys()
        'when on is rechecked or after code ran 

        GetAsyncKeyState(Keys.Scroll)

        If ChkMouse.CheckState = CheckState.Unchecked Then
            GetAsyncKeyState(Keys.LButton)
            GetAsyncKeyState(Keys.RButton)
            GetAsyncKeyState(Keys.MButton)
            GetAsyncKeyState(Keys.XButton1)
            GetAsyncKeyState(Keys.XButton2)
        End If

        If chkAz.CheckState = CheckState.Unchecked Then
            GetAsyncKeyState(Keys.A)
            GetAsyncKeyState(Keys.B)
            GetAsyncKeyState(Keys.C)
            GetAsyncKeyState(Keys.D)
            GetAsyncKeyState(Keys.E)
            GetAsyncKeyState(Keys.F)
            GetAsyncKeyState(Keys.G)
            GetAsyncKeyState(Keys.H)
            GetAsyncKeyState(Keys.I)
            GetAsyncKeyState(Keys.J)
            GetAsyncKeyState(Keys.K)
            GetAsyncKeyState(Keys.L)
            GetAsyncKeyState(Keys.M)
            GetAsyncKeyState(Keys.N)
            GetAsyncKeyState(Keys.O)
            GetAsyncKeyState(Keys.P)
            GetAsyncKeyState(Keys.Q)
            GetAsyncKeyState(Keys.R)
            GetAsyncKeyState(Keys.S)
            GetAsyncKeyState(Keys.T)
            GetAsyncKeyState(Keys.U)
            GetAsyncKeyState(Keys.V)
            GetAsyncKeyState(Keys.W)
            GetAsyncKeyState(Keys.X)
            GetAsyncKeyState(Keys.Y)
            GetAsyncKeyState(Keys.Z)
        End If

        If chk09.CheckState = CheckState.Unchecked Then
            GetAsyncKeyState(Keys.D0)
            GetAsyncKeyState(Keys.D9)
            GetAsyncKeyState(Keys.D8)
            GetAsyncKeyState(Keys.D7)
            GetAsyncKeyState(Keys.D6)
            GetAsyncKeyState(Keys.D5)
            GetAsyncKeyState(Keys.D4)
            GetAsyncKeyState(Keys.D3)
            GetAsyncKeyState(Keys.D2)
            GetAsyncKeyState(Keys.D1)
        End If

        If chkF1f12.CheckState = CheckState.Unchecked Then
            GetAsyncKeyState(Keys.F1)
            GetAsyncKeyState(Keys.F2)
            GetAsyncKeyState(Keys.F3)
            GetAsyncKeyState(Keys.F4)
            GetAsyncKeyState(Keys.F5)
            GetAsyncKeyState(Keys.F6)
            GetAsyncKeyState(Keys.F7)
            GetAsyncKeyState(Keys.F8)
            GetAsyncKeyState(Keys.F9)
            GetAsyncKeyState(Keys.F10)
            GetAsyncKeyState(Keys.F11)
            GetAsyncKeyState(Keys.F12)
        End If

        If chkArrows.CheckState = CheckState.Unchecked Then
            GetAsyncKeyState(Keys.Down)
            GetAsyncKeyState(Keys.Left)
            GetAsyncKeyState(Keys.Right)
            GetAsyncKeyState(Keys.Up)
        End If

        If chkOther.CheckState = CheckState.Unchecked Then
            GetAsyncKeyState(Keys.PrintScreen)
            GetAsyncKeyState(Keys.Alt)
            GetAsyncKeyState(Keys.Menu)
            GetAsyncKeyState(Keys.LMenu)
            GetAsyncKeyState(Keys.RMenu)
            GetAsyncKeyState(Keys.Back)
            GetAsyncKeyState(Keys.Control)
            GetAsyncKeyState(Keys.LControlKey)
            GetAsyncKeyState(Keys.Enter)
            GetAsyncKeyState(Keys.Space)
            GetAsyncKeyState(Keys.Tab)
            GetAsyncKeyState(Keys.Escape)
            GetAsyncKeyState(Keys.Shift)
            GetAsyncKeyState(Keys.LShiftKey)
            GetAsyncKeyState(Keys.RShiftKey)
            GetAsyncKeyState(Keys.CapsLock)
            GetAsyncKeyState(Keys.RControlKey)
            GetAsyncKeyState(Keys.Insert)
            GetAsyncKeyState(Keys.LWin)
            GetAsyncKeyState(Keys.Delete)
        End If
        If chkWedgee.Checked = False Then
            GetAsyncKeyState(Keys.MediaPlayPause) 'shft + key
            GetAsyncKeyState(Keys.VolumeMute)
            GetAsyncKeyState(Keys.VolumeDown)
            GetAsyncKeyState(Keys.VolumeUp)
            GetAsyncKeyState(Keys.PrintScreen)
            GetAsyncKeyState(Keys.Home)
            GetAsyncKeyState(Keys.End)
            GetAsyncKeyState(Keys.PageUp)
            GetAsyncKeyState(Keys.PageDown)
        End If
        If chkNumPad.CheckState = CheckState.Unchecked Then
            GetAsyncKeyState(Keys.NumPad0)
            GetAsyncKeyState(Keys.NumPad1)
            GetAsyncKeyState(Keys.NumPad2)
            GetAsyncKeyState(Keys.NumPad3)
            GetAsyncKeyState(Keys.NumPad4)
            GetAsyncKeyState(Keys.NumPad5)
            GetAsyncKeyState(Keys.NumPad6)
            GetAsyncKeyState(Keys.NumPad7)
            GetAsyncKeyState(Keys.NumPad8)
            GetAsyncKeyState(Keys.NumPad9)
        End If

        If chkMisc.CheckState = CheckState.Unchecked Then
            GetAsyncKeyState(Keys.Oem1)
            GetAsyncKeyState(Keys.Oem2)
            GetAsyncKeyState(Keys.Oem3)
            GetAsyncKeyState(Keys.Oem4)
            GetAsyncKeyState(Keys.Oem5)
            GetAsyncKeyState(Keys.Oem6)
            GetAsyncKeyState(Keys.Oem7)
            GetAsyncKeyState(Keys.OemPeriod)
            GetAsyncKeyState(Keys.Oemcomma)
            GetAsyncKeyState(Keys.OemMinus)
            GetAsyncKeyState(Keys.Oemplus)
        End If

    End Sub

    Sub dnaTxt()
        If TextBox1.Text = "'" Then TextBox1.Clear()
        If TextBox1.TextLength >= 25 Then
            If My.Settings.SettingAutoLockEmode = True Then
                TextBox1.Text = "»" 'emode
            Else
                TextBox1.Clear() 'auto clear
            End If
        End If

        If tipsDnaToolStripMenuItem.CheckState = CheckState.Checked Then Me.Text = "dna > " & TextBox1.Text Else If Not Me.FormBorderStyle = FormBorderStyle.Sizable And ControlBox = False Then Me.Text = "dna" 'me.text mock 
        Me.ShowIcon = My.Settings.SettingShowIcon
    End Sub

    Sub f1tt()
        ToolTip1.Hide(ListBox1) 'clear
        ToolTip1.Active = True
        ToolTip1.IsBalloon = My.Settings.SettingTipBalloon
        If ListBox1.Text > "" And ListBox1.Items.Count > 0 Then ToolTip1.SetToolTip(ListBox1, ListBox1.SelectedItem.ToString)
        SetCursorPos(MousePosition.X + 1, MousePosition.Y) 'move to reshow
        SetCursorPos(MousePosition.X - 1, MousePosition.Y)
    End Sub
    'g 
    Dim ignoreWhiteSpace_g = My.Settings.SettingIgnoreWhiteSpace
    Dim ttl As String ' = ToolTip1.GetToolTip(ListBox1) 'db itm f1 tt
    Dim gSettingHidden = My.Settings.SettingHidden
    Dim psalwayson = My.Settings.SettingPrintScreenAlwaysOn
    Dim retap = 0
    Dim clear_skmg1 As Boolean = False
    Dim containsws_g = False
    Dim g_maxkeylen = My.Settings.SettingMaxKeyLength
    Dim g_scroll = My.Settings.SettingScrollLockRun
    Dim g_remcb = My.Settings.SettingRememberClipboard

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If PauseBreakToolStripMenuItem.Checked = True Then 'pausebreak
            If GetAsyncKeyState(Keys.Pause) Then
                clearAllKeys()
                emode()
            End If
        End If

        If TextBox1.Text = "'" Then Exit Sub


        If Me.chk_timer1_on_val.Checked = True And Timer1.Interval > 0 Then


            If gSettingHidden = True And HideToolStripMenuItem.Checked = True And HideToolStripMenuItem.Visible = False Then Me.Hide() 'start hidden
            If HideToolStripMenuItem.Visible = False And Me.Visible = False Then HideToolStripMenuItem.Checked = False


            Dim specialkey = My.Settings.SettingSpecialKey 'Keys.Insert
            If GetAsyncKeyState(specialkey) Or chkOther.Checked = False And GetAsyncKeyState(Keys.RControlKey) And RightCtrllToolStripMenuItem.Checked = True Then 'rctrl toggle
                keybd_event(Keys.RControlKey, 0, &H2, 0) 'rel rc

                If TextBox1.Text.StartsWith("»") Then 'toggle"«"
                    clearAllKeys()
                    Me.TextBox1.Text = "«"
                    Exit Sub
                End If
                If TextBox1.Text.StartsWith("«") Then 'toggle  ""
                    Me.TextBox1.Clear()
                    Exit Sub
                End If

                If My.Settings.SettingRctrleqMod = "»" Then 'freeze 
                    TextBox1.Clear() 'if v2
                    Me.TextBox1.Text = "»"
                    dnaTxt()
                    Exit Sub
                End If
                If My.Settings.SettingRctrleqMod = "«" Then
                    TextBox1.Clear() 'if v2
                    Me.TextBox1.Text = My.Settings.SettingRctrleqMod.ToString
                    Exit Sub
                End If
                Me.TextBox1.Text += My.Settings.SettingRctrleqMod.ToString

            End If



            'energy saver
            If TextBox1.Text.StartsWith("»") Then Exit Sub



            If TabControl1.ContainsFocus Then 'lalt chk_tips
                If ListBox1.ContainsFocus Then 'tt db item
                    If GetAsyncKeyState(Keys.F1) Then 'f1 tt
                        keyHold(Keys.F1)
                        Exit Sub
                    End If
                    If GetAsyncKeyState(Keys.LMenu) Then 'show listbox index tt
                        My.Settings.SettingLastListIndex = ListBox1.SelectedIndex
                        keyRelease(Keys.LMenu)
                        key(Keys.LMenu)
                        f1tt()
                        Exit Sub
                    End If
                Else
                    If GetAsyncKeyState(Keys.F1) Then 'toggle chk_tips
                        Dim br = (My.Settings.SettingMoveBar) 'get mbar state
                        If My.Settings.SettingIgnoref1 = False And My.Settings.SettingChkF1f12 = False Then Me.TextBox1.Text += "!" 'print f1>!
                        chkItem(chk_tips)
                        tip()
                        reStyle()
                        If br = True Then moveBarRe()
                        Exit Sub
                    End If
                End If
            End If

            If txtString.ContainsFocus = True And Me.ContextMenuStripString.Visible = True Then 'tgl mnu
                If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.Space) Then
                    Me.ContextMenuStripString.Hide()
                    mnuItemsShow(True)
                    Me.ContextMenuStripDb.Show(MousePosition)
                End If
            End If
            If txtString.ContainsFocus = True And MouseButtons = Windows.Forms.MouseButtons.Right Then 'db swipe menu
                dz += 1
                If dz >= My.Settings.SettingZone And dz <= My.Settings.SettingZone * 2 And ContextMenuStripString.Visible = True Then
                    ttAdjust()
                    mnuItemsShow(True)
                    ToolStripMenuItem2.Visible = True
                    ContextMenuStripDb.Show(MousePosition) 'show mnu2
                    'dz = 0
                End If
                If dz >= My.Settings.SettingZone * 2 And ContextMenuStripDb.Visible = True Then 'run
                    ContextMenuStripDb.Hide()
                    rightrelease()
                    runCode()
                    dz = 0
                End If
            End If

            If Me.ContainsFocus = True And MouseButtons = Windows.Forms.MouseButtons.Right And ContextMenuStripOptions.Visible = True And TextBox1.Text <> "'" Then 'swipe mainbg/ts bar to win run tabtip keyboard
                dz += 1
                If dz >= My.Settings.SettingZone Then
                    ttAdjust()
                    ContextMenuStripOptions.Hide()
                    rightrelease()
                    keyRelease(Keys.RButton)


                    If connect = False Or strandComplete = False Then
                        rightrelease()
                        apisk("x«win»r«-win»«sleep:" + My.Settings.SettingSpacer.ToString + "»tabtip«enter»")
                    Else
                        strandComplete = True
                        connect = True
                        rightrelease()
                        apisk("x«win»r«-win»«sleep:" + My.Settings.SettingSpacer.ToString + "»tabtip«enter»")
                    End If

                    clearAllKeys()
                    If TextBox1.Text = "'" Then TextBox1.Text = ""
                    dz = 0
                    If Me.Text = "" Then dnaTxt()
                    If Me.Text > "" Then AppActivate(Me.Text.ToString)
                    txtString.Focus()
                    If Me.ControlBox = False Then
                        Me.Text = ""
                        timeout2(333)
                        txtString.Focus()
                    End If
                End If
            End If

            'no length
            If NoLengthToolStripMenuItem.Checked = True And TextBox1.Text.StartsWith("«") Or NoLengthToolStripMenuItem.Checked = False Then

                'mouse ♠♣♥♦
                If ChkMouse.CheckState = False = True Then
                    If LeftClickToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.LButton) Then Me.TextBox1.Text += "♠"
                    If RightClickToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.RButton) Then Me.TextBox1.Text += "♦"
                    If MiddleClickToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.MButton) Then Me.TextBox1.Text += "♥"
                    If LscrollToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.XButton1) Then Me.TextBox1.Text += "ɹ"
                    If RscrollToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.XButton2) Then Me.TextBox1.Text += "ɾ"
                End If


                'start 
                If chkAz.CheckState = False Then

                    If AToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.A) Then Me.TextBox1.Text += "a"
                    If BToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.B) Then Me.TextBox1.Text += "b"
                    If CToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.C) Then Me.TextBox1.Text += "c"
                    If DToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.D) Then Me.TextBox1.Text += "d"
                    If EToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.E) Then Me.TextBox1.Text += "e"
                    If FToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.F) Then Me.TextBox1.Text += "f"
                    If GToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.G) Then Me.TextBox1.Text += "g"
                    If HToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.H) Then Me.TextBox1.Text += "h"
                    If IToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.I) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "i"
                    If JToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.J) Then Me.TextBox1.Text += "j"
                    If KToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.K) Then Me.TextBox1.Text += "k"
                    If LToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.L) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "l"
                    If MToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.M) Then Me.TextBox1.Text += "m"
                    If NToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.N) Then Me.TextBox1.Text += "n"
                    If OToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.O) Then Me.TextBox1.Text += "o"
                    If PToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.P) And txtString.ContainsFocus = False Then
                        Me.TextBox1.Text += "p"
                        lblX.Text = (MousePosition.X)
                        lblY.Text = (MousePosition.Y)
                    End If
                    If QToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.Q) Then Me.TextBox1.Text += "q"
                    If RToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.R) Then Me.TextBox1.Text += "r"
                    If SToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.S) Then Me.TextBox1.Text += "s"
                    If TToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.T) Then Me.TextBox1.Text += "t"
                    If UToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.U) Then Me.TextBox1.Text += "u"
                    If VToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.V) Then Me.TextBox1.Text += "v"
                    If WToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.W) Then Me.TextBox1.Text += "w"
                    If XToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.X) Then Me.TextBox1.Text += "x"
                    If YToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.Y) Then Me.TextBox1.Text += "y"
                    If ZToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.Z) Then Me.TextBox1.Text += "z"
                End If

                If chk09.CheckState = False Then
                    If ToolStripMenuItem14.CheckState = False And GetAsyncKeyState(Keys.D0) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "0"
                    If ToolStripMenuItem15.CheckState = False And GetAsyncKeyState(Keys.D1) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "1"
                    If ToolStripMenuItem16.CheckState = False And GetAsyncKeyState(Keys.D2) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "2"
                    If ToolStripMenuItem17.CheckState = False And GetAsyncKeyState(Keys.D3) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "3"
                    If ToolStripMenuItem18.CheckState = False And GetAsyncKeyState(Keys.D4) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "4"
                    If ToolStripMenuItem19.CheckState = False And GetAsyncKeyState(Keys.D5) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "5"
                    If ToolStripMenuItem20.CheckState = False And GetAsyncKeyState(Keys.D6) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "6"
                    If ToolStripMenuItem21.CheckState = False And GetAsyncKeyState(Keys.D7) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "7"
                    If ToolStripMenuItem22.CheckState = False And GetAsyncKeyState(Keys.D8) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "8"
                    If ToolStripMenuItem23.CheckState = False And GetAsyncKeyState(Keys.D9) And txtString.ContainsFocus = False Then Me.TextBox1.Text += "9"
                End If

                If chkF1f12.CheckState = False Then
                    If ListBox1.ContainsFocus Then Exit Sub
                    If F1ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F1) And ListBox1.ContainsFocus = False Then Me.TextBox1.Text += "!"
                    If F2ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F2) Then Me.TextBox1.Text += "@"
                    If F3ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F3) Then Me.TextBox1.Text += "#"
                    If F4ToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.F4) Then
                        Me.TextBox1.Text += "$"
                        If txtString.ContainsFocus Then txtStringClear()
                    End If
                    If F5ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F5) Then
                        Me.TextBox1.Text += "%"
                        If txtString.ContainsFocus Then runCode()
                    End If
                    If F6ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F6) Then Me.TextBox1.Text += "^"
                    If F7ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F7) Then Me.TextBox1.Text += "&"
                    If F8ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F8) Then Me.TextBox1.Text += "*"
                    If F9ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F9) Then Me.TextBox1.Text += "("
                    If F10ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F10) Then Me.TextBox1.Text += ")"
                    If F11ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F11) Then Me.TextBox1.Text += "_"
                    If F12ToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.F12) Then Me.TextBox1.Text += "="
                End If

                If chkMisc.CheckState = False Then
                    If ToolStripMenuItemChkMiscSc.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oem1) Then Me.TextBox1.Text += ";"
                    If ToolStripMenuItemChkMiscFs.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oem2) Then Me.TextBox1.Text += "/"
                    If ToolStripMenuItemChkMiscTil.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oem3) Then Me.TextBox1.Text += "`"
                    If ToolStripMenuItemChkMiscLb.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oem4) Then Me.TextBox1.Text += "["
                    If ToolStripMenuItemChkMiscBs.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oem5) Then Me.TextBox1.Text += "\"
                    If ToolStripMenuItemChkMiscRb.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oem6) Then Me.TextBox1.Text += "]"
                    If ToolStripMenuItemChkMiscRem.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oem7) Then Me.TextBox1.Text += """"
                    If ToolStripMenuItemChkMiscPeriod.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.OemPeriod) Then Me.TextBox1.Text += "."
                    If ToolStripMenuItemChkMiscComma.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oemcomma) Then Me.TextBox1.Text += ","
                    If ToolStripMenuItemChkMiscMinus.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.OemMinus) Then Me.TextBox1.Text += "-"
                    If ToolStripMenuItemChkMiscPlus.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Oemplus) Then Me.TextBox1.Text += "+"
                End If

                If chkNumPad.CheckState = CheckState.Unchecked Then
                    If ToolStripMenuItem13.CheckState = False And GetAsyncKeyState(Keys.NumPad9) Then Me.TextBox1.Text += "F" '9
                    If ToolStripMenuItem12.CheckState = False And GetAsyncKeyState(Keys.NumPad8) Then Me.TextBox1.Text += "G"
                    If ToolStripMenuItem11.CheckState = False And GetAsyncKeyState(Keys.NumPad7) Then Me.TextBox1.Text += "J"
                    If ToolStripMenuItem10.CheckState = False And GetAsyncKeyState(Keys.NumPad6) Then Me.TextBox1.Text += "K"
                    If ToolStripMenuItem9.CheckState = False And GetAsyncKeyState(Keys.NumPad5) Then Me.TextBox1.Text += "Q"
                    If ToolStripMenuItem8.CheckState = False And GetAsyncKeyState(Keys.NumPad4) Then Me.TextBox1.Text += "V"
                    If ToolStripMenuItem7.CheckState = False And GetAsyncKeyState(Keys.NumPad3) Then Me.TextBox1.Text += "W"
                    If ToolStripMenuItem6.CheckState = False And GetAsyncKeyState(Keys.NumPad2) Then Me.TextBox1.Text += "X"
                    If ToolStripMenuItem5.CheckState = False And GetAsyncKeyState(Keys.NumPad1) Then Me.TextBox1.Text += "Y" '1
                    If ToolStripMenuItem4.CheckState = False And GetAsyncKeyState(Keys.NumPad0) Then Me.TextBox1.Text += "Z" '0
                End If

                If chkArrows.CheckState = CheckState.Unchecked Then
                    If UpToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.Up) Then Me.TextBox1.Text += "U"
                    If DownToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.Down) Then Me.TextBox1.Text += "D"
                    If LeftToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.Left) Then
                        If ListBox1.Focused Then
                            selectTopItem()
                        Else
                            Me.TextBox1.Text += "L"
                        End If
                    End If
                    If RightToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.Right) Then
                        If ListBox1.Focused Then
                            selectBottomItem()
                        Else
                            Me.TextBox1.Text += "R"
                        End If
                    End If
                End If

                If chkOther.CheckState = False Then
                    If GetAsyncKeyState(Keys.Escape) And EscToolStripMenuItem1.CheckState = False Then
                        Me.TextBox1.Text += "."
                        Cursor.Show()
                        lblX.Text = MousePosition.X
                        lblY.Text = MousePosition.Y
                    End If
                    If PauseToolStripMenuItem2.CheckState = False And GetAsyncKeyState(Keys.Pause) Then
                        Me.TextBox1.Text += "ƀ"
                        keyClear(Keys.Pause)
                        shiftRelease()
                        altRelease()
                        ctrlRelease()
                    ElseIf PauseToolStripMenuItem2.CheckState = False And GetAsyncKeyState(Keys.Pause) Then
                        keyClear(Keys.Pause)
                        shiftRelease()
                        altRelease()
                        ctrlRelease()
                    End If
                    If PSToolStripMenuItem3.CheckState = False And GetAsyncKeyState(Keys.PrintScreen) Then Me.TextBox1.Text += "Ƥ" 'ps
                    If LeftAltToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.LMenu) Then Me.TextBox1.Text += "A" 'menu 'GetAsyncKeyState(Keys.Alt) Or
                    If RightAltToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.RMenu) Then Me.TextBox1.Text += "Ą" 'menu 
                    'If AltToolStripMenuItem.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.Menu) Then Me.TextBox1.Text += "A" 'menu ą
                    If WinToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.LWin) Then Me.TextBox1.Text += "M" 'lwin

                    If RightControlToolStripMenuItem.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.RControlKey) And RightCtrllToolStripMenuItem.Checked = False Then Me.TextBox1.Text += "O" 'lshiftkey
                    If LeftShiftToolStripMenuItem.CheckState = CheckState.Unchecked And GetAsyncKeyState(Keys.LShiftKey) Then Me.TextBox1.Text += "S"

                    'If GetAsyncKeyState(Keys.Back) Then Me.TextBox1.Text += "B"
                    If SpaceToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.Space) And txtString.ContainsFocus = False Then Me.TextBox1.Text += " "
                    If BackspaceToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.Back) And Len(TextBox1.Text) > 0 Then Me.TextBox1.Text = Microsoft.VisualBasic.Left(TextBox1.Text, Len(TextBox1.Text) - 1) ' backspace
                    If LeftControlToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.LControlKey) Then Me.TextBox1.Text += "C"

                    If EnterToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.Enter) Then Me.TextBox1.Text += "E"
                    If RightShiftToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.RShiftKey) Then Me.TextBox1.Text += "H"
                    If ShiftToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.Shift) Then Me.TextBox1.Text += "I"
                    If ControlToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.Control) Then Me.TextBox1.Text += "N"
                    'If RightControlToolStripMenuItem.CheckState = false And GetAsyncKeyState(Keys.RControlKey) Then Me.TextBox1.Text += "O"
                    If CapsToolStripMenuItem.CheckState = False And GetAsyncKeyState(Keys.CapsLock) Then Me.TextBox1.Text += "P"
                    If TabToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.Tab) Then Me.TextBox1.Text += "T"
                    If InsertToolStripMenuItem1.CheckState = False And GetAsyncKeyState(Keys.Insert) Then Me.TextBox1.Text += "į"
                    If DeleteToolStripMenuItem2.CheckState = False And GetAsyncKeyState(Keys.Delete) Then Me.TextBox1.Text += "º" 'deletekey

                    If chkWedgee.Checked = False Then 'wedge keyboard
                        If PlayPauseToolStripMenuItem.Checked = False And GetAsyncKeyState(Keys.MediaPlayPause) Then Me.TextBox1.Text += "Ħ" 'f1
                        If VolumeMuteToolStripMenuItem.Checked = False And GetAsyncKeyState(Keys.VolumeMute) Then Me.TextBox1.Text += "ħ" 'f2
                        If VolumeDownToolStripMenuItem.Checked = False And GetAsyncKeyState(Keys.VolumeDown) Then Me.TextBox1.Text += "ď" 'f3
                        If VolumeUpToolStripMenuItem.Checked = False And GetAsyncKeyState(Keys.VolumeUp) Then Me.TextBox1.Text += "Ď" 'f4
                        If PrintScreenToolStripMenuItem2.Checked = False And GetAsyncKeyState(Keys.PrintScreen) Then Me.TextBox1.Text += "ĕ" 'f9
                        If HomeToolStripMenuItem1.Checked = False And GetAsyncKeyState(Keys.Home) Then Me.TextBox1.Text += "Ė" 'f10
                        If EndToolStripMenuItem1.Checked = False And GetAsyncKeyState(Keys.End) Then Me.TextBox1.Text += "ė" 'f11
                        If PageUpToolStripMenuItem.Checked = False And GetAsyncKeyState(Keys.PageUp) Then Me.TextBox1.Text += "Ę" 'f12
                        If PageDownToolStripMenuItem.Checked = False And GetAsyncKeyState(Keys.PageDown) Then Me.TextBox1.Text += "ę" '
                    End If
                End If
            Else
                If psalwayson = True And GetAsyncKeyState(Keys.PrintScreen) Then Me.TextBox1.AppendText("Ƥ")
                clearAllKeys()
            End If
            '//no length mode

            If GetAsyncKeyState(Keys.X) And GetAsyncKeyState(Keys.Y) Then
                runMousePosition()
            End If
            If GetAsyncKeyState(Keys.H) And GetAsyncKeyState(Keys.Escape) Then
                If Me.Visible = False Then
                    keyRelease(Keys.H)
                    keyRelease(Keys.Escape)
                    Me.Show() 'show form
                    Me.TopMost = True
                    Me.TopMost = My.Settings.SettingMain_chk_top
                    Me.HideToolStripMenuItem.Checked = False
                    If txtString.ContainsFocus Then
                        key(Keys.Back)
                    End If
                    Exit Sub
                End If
                If Me.Visible = True Then

                    If txtString.ContainsFocus Then
                        key(Keys.Back)
                    End If

                    keyRelease(Keys.H)
                    keyRelease(Keys.Escape)
                    Me.Hide() 'hide form
                    Me.HideToolStripMenuItem.Checked = False
                    Exit Sub
                End If
            End If

            If GetAsyncKeyState(Keys.LShiftKey) And GetAsyncKeyState(Keys.Escape) And ShiftEscapeToolStripMenuItem.CheckState = CheckState.Checked Then
                If GetAsyncKeyState(Keys.LControlKey) Then Exit Sub
                If Me.Visible = False Then Me.Visible = True
                Me.chk_timer1_on_val.Checked = False 'power
                If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable And Me.ControlBox = False Then
                    sizeable()
                    My.Settings.SettingMoveBar = True 'reshow move bar
                    moveable()
                End If
                Me.CenterToScreen()
            End If

            'manual run
            If GetAsyncKeyState(Keys.Scroll) And g_scroll = True Or GetAsyncKeyState(Keys.Escape) And GetAsyncKeyState(Keys.Insert) Or GetAsyncKeyState(Keys.Escape) And GetAsyncKeyState(Keys.OemPeriod) Then
                If GetAsyncKeyState(Keys.Scroll) And GetAsyncKeyState(Keys.Escape) Or txtString.Text.StartsWith("<") Or txtString.Text.StartsWith(">") Then 'copy selected text

                    Dim c As String = "" '<-.>

                    If Clipboard.GetText = "" And GetAsyncKeyState(Keys.Escape) And GetAsyncKeyState(Keys.Scroll) Or
                        txtString.Text.StartsWith("<") Or txtString.Text.StartsWith(">") Or txtString.Text.StartsWith("'") Then

                        If txtString.Text.StartsWith("'") Or txtString.Text.StartsWith("<'") Then c = "'" '<-.>

                        keyHold(Keys.LControlKey)
                        key(Keys.C)
                        keyRelease(Keys.LControlKey)
                        keyClear(Keys.C)
                        keyClear(Keys.LControlKey)
                        timeout2(33)
                        If Clipboard.GetText = "" Then Exit Sub
                    End If

                    If txtString.Text.StartsWith(">") Then 'cbtotext
                        Dim getLine1 = txtString.GetLineFromCharIndex(txtString.SelectionStart - txtString.TextLength)
                        If txtString.Text = ">" Then txtString.Text += "«enter»" & Chr(13)
                        getLine1 = txtString.GetLineFromCharIndex(txtString.SelectionStart - txtString.TextLength)
                        Dim x13 = txtString.Lines(getLine1)
                        Me.Show()
                        If Me.Text = "" Then Me.Text = "dna"
                        AppActivate("dna")
                        apisk(x13)
                        clearAllKeys()
                        keyClear(Keys.Scroll)
                        TextBox1.Clear()
                        txtString.AppendText(Clipboard.GetText)
                        Clipboard.Clear()
                        emode() '<-.>
                        Exit Sub
                    End If

                    Dim t As String = txtString.Text.ToString 'cbtolist
                    Dim s As Integer = txtString.SelectionStart
                    txtStringClear()

                    If TextBox1.Text.StartsWith("«") Then '<-.>
                        i = TextBox1.TextLength - TextBox1.Text.Replace(".", "").Length
                        If i >= 4 Then c = TextBox1.Text + "-»" Else c = TextBox1.Text + "»"
                        c = c.Replace(".", "")
                    End If


                    txtString.AppendText(c + Clipboard.GetText)
                    addDbItm()
                    txtStringClear()
                    txtString.AppendText(t)
                    txtString.SelectionStart = s
                    keyRelease(Keys.Scroll)
                    keyClear(Keys.Scroll)
                    keyClear(Keys.Escape)
                    Clipboard.Clear()
                    emode() '<-.>
                    Exit Sub
                End If
                keyClear(Keys.Scroll)

                If GetAsyncKeyState(Keys.Insert) Then
                    If My.Settings.SettingChkEscInsRun = False Then Exit Sub
                End If
                If GetAsyncKeyState(Keys.OemPeriod) Then
                    key(Keys.Back)
                    If My.Settings.SettingChkEscPeriodRun = False Then Exit Sub
                End If
                If txtString.Text > "" Then 'run txt
                    Dim f As String '= "x" & txtString.Text 'test > txtString
                    If txtString.SelectedText().ToString() > "" Then
                        f = "x" + txtString.SelectedText().ToString()
                        If connect = False Or strandComplete = False Then
                            Dim cb = Clipboard.GetText
                            apisk(f)
                            If Not Clipboard.GetText = cb Then If g_remcb Then Clipboard.SetText(cb)
                        Else
                            strandComplete = True
                            connect = True
                            apisk(f)
                        End If
                    Else
                        f = "x" & txtString.Text 'test > txtString
                        If connect = False Or strandComplete = False Then
                            Dim cb = Clipboard.GetText
                            apisk(f)
                            If Not Clipboard.GetText = cb Then If g_remcb Then Clipboard.SetText(cb)
                        Else
                            strandComplete = True
                            connect = True
                            apisk(f)
                        End If
                    End If
                    clearAllKeys()
                    'TextBox1.Text = ""'<-.>
                    emode() '<-.>
                    Exit Sub
                End If

                If ListBox1.Text = "" Then Exit Sub 'run list item

                If ListBox1.Text.Length > Val(txtLength.Text) + 1 And ListBox1.SelectedItem > "" Then 'esc+insert '<-.> last and

                    If GetChar(ListBox1.SelectedItem.ToString, Val(txtLength.Text) + 1) = Chr(9) Then
                        Dim f1 As String = Mid(ListBox1.SelectedItem.ToString, 1, Val(txtLength.Text)) 'code
                        Dim f2 = Microsoft.VisualBasic.Right(ListBox1.SelectedItem.ToString, Len(ListBox1.SelectedItem.ToString) - Val(txtLength.Text) - 1) 'message
                        apisk("x" + f2)
                        clearAllKeys()
                        TextBox1.Text = ""
                        Exit Sub
                    End If
                End If

                If ListBox1.SelectedItem = "" Then Exit Sub '<-.>

                Dim f3 As String = ListBox1.SelectedItem.ToString 'run without tab

                If connect = False Or strandComplete = False Then
                    strandComplete = True
                    connect = True
                Else
                    If f3.StartsWith("'") Then '<-.>
                        apisk(f3)
                    Else
                        apisk("x" + f3)
                    End If
                    'apisk("x" + f3)'<-.>
                End If

                clearAllKeys()
                TextBox1.Text = ""
                Exit Sub '
            End If
            '/esc+insert

            'no code length scan *
            If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.Space) Then
                If My.Settings.SettingLctrlSpace_key = True Then
                    keyRelease(Keys.LControlKey) 'My.Computer.Keyboard.CtrlKeyDown
                    keyRelease(Keys.Space)
                    TextBox1.Text = "«"
                End If
            End If
            If GetAsyncKeyState(Keys.Escape) And GetAsyncKeyState(Keys.Oemcomma) Then
                If My.Settings.SettingEscComma_key = True Then
                    key(Keys.Back)
                    keyRelease(Keys.Escape)
                    TextBox1.Text = "«"
                End If
            End If
            If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.LShiftKey) And GetAsyncKeyState(Keys.Back) Then
                If My.Settings.SettingLctrlLshftBs_key = True Then
                    keyRelease(Keys.LControlKey)
                    keyRelease(Keys.LShiftKey)
                    TextBox1.Text = "«"
                End If
            End If

            If TextBox1.Text.IndexOf("«", 0) = 0 Then
                '«scan
                If TextBox1.TextLength >= g_maxkeylen Then emode() 'auto clear
                If TextBox1.Text = "«st" Or TextBox1.Text = "«stE" Then 'show tabs
                    If GetAsyncKeyState(Keys.Enter) Then toggleTabControl1Show()
                End If
                If TextBox1.Text = "«al" Or TextBox1.Text = "«alE" Then 'auto lock 
                    If GetAsyncKeyState(Keys.Enter) Then autoLock()
                End If
                If TextBox1.Text = "«nl" Or TextBox1.Text = "«nlE" Then 'no length 
                    If GetAsyncKeyState(Keys.Enter) Then noLengthMode()
                End If
            Else
                If Microsoft.VisualBasic.Right(TextBox1.Text, Val(txtLength.Text)) = TextBox1.Text Then
                Else
                    'finished = True
                    TextBox1.Text = Microsoft.VisualBasic.Right(TextBox1.Text, Val(txtLength.Text)) 'make only the amount of codes 'My.Settings.SettingTxtCodeLength
                End If
            End If

            'me.text mock 
            If tipsDnaToolStripMenuItem.Checked = True Then Me.Text = "dna > " & TextBox1.Text

        Else
            clearAllKeys()
        End If
    End Sub

    Sub toggleTabControl1Show()
        If TabControl1.Visible = False Then
            TabControl1.Show()
            Try
                AppActivate(Me.Text)
            Catch ex As Exception
                sleep(1)
            End Try
            If TabPage3.Focus = True Then
                txtString.Visible = True
                txtString.Focus()
            End If
            sleep(1)
        Else
            TabControl1.Hide()
        End If
    End Sub

    Private Sub TextBox1_Click(sender As Object, e As EventArgs) Handles TextBox1.Click
        TextBox1.Text = ""
        Me.TabControl1.Focus()
    End Sub

    Sub w7(d As Double) 'ms timeout
        If My.Settings.SettingChkW7 = True Then timeout2(d)
    End Sub

    Sub print(f As String, clearonly As Boolean) 'print  12.12.2013
        If GetAsyncKeyState(Keys.Pause) Then Exit Sub

        Dim shiftKey As Boolean = False
        TextBox1.Text = "'"
        Dim a As String, aa As String

        For i = 1 To f.Length
            If GetAsyncKeyState(Keys.Pause) Then Exit Sub

            shiftKey = False

            If GetAsyncKeyState(Keys.Escape) Then
                'TextBox1.Text = ""
                emode()
                Exit Sub
            End If

            a = Microsoft.VisualBasic.Left(f, i) '
            aa = Microsoft.VisualBasic.Right(a, 1) 's


            If aa = "«" And clearonly = False Then
                SendKeys.Send("«")
                w7(33)
                Continue For
            End If
            If aa = "»" And clearonly = False Then
                w7(33)
                SendKeys.Send("»")
                w7(33)
                Continue For
            End If
            If aa = "" Then Continue For
            If aa = vbLf Then aa = Keys.Enter 'enter
            If aa = vbTab Then aa = "tab" 'tab 
            If aa = " " Then aa = Keys.Space 'space
            If aa = "`" Then aa = Keys.Oem3 '`key
            If aa = "?" Then '?
                shiftKey = True
                aa = Keys.OemQuestion
            End If

            If aa = ">" Then '>
                shiftKey = True
                aa = Keys.OemPeriod
            End If

            If aa = "<" Then
                shiftKey = True
                aa = Keys.Oemcomma
            End If

            If aa = """" Then
                shiftKey = True
                aa = Keys.OemQuotes
            End If

            If aa = ":" Then
                shiftKey = True
                aa = Keys.OemSemicolon
            End If

            If aa = "|" Then
                aa = Keys.OemBackslash
                shiftKey = True
            End If

            If aa = "}" Then
                shiftKey = True
                aa = Keys.OemCloseBrackets
            End If

            If aa = "{" Then
                shiftKey = True
                aa = Keys.OemOpenBrackets
            End If

            If aa = "+" Then
                aa = Keys.Oemplus
                shiftKey = True
            End If

            If aa = "_" Then
                aa = Keys.OemMinus
                shiftKey = True
            End If
            If aa = ")" Then
                aa = Keys.D0
                shiftKey = True
            End If

            If aa = "(" Then
                aa = Keys.D9
                shiftKey = True
            End If

            If aa = "*" Then
                aa = Keys.D8
                shiftKey = True
            End If

            If aa = "&" Then
                aa = Keys.D7
                shiftKey = True
            End If

            If aa = "^" Then
                aa = Keys.D6
                shiftKey = True
            End If

            If aa = "%" Then
                aa = Keys.D5
                shiftKey = True
            End If

            If aa = "$" Then
                aa = Keys.D4
                shiftKey = True
            End If

            If aa = "#" Then
                aa = Keys.D3
                shiftKey = True
            End If

            If aa = "@" Then
                aa = Keys.D2
                shiftKey = True
            End If

            If aa = "!" Then
                aa = Keys.D1
                shiftKey = True
            End If

            If aa = "~" Then
                aa = Keys.Oem3
                shiftKey = True
            End If


            If aa = "-" Then aa = Keys.OemMinus
            If aa = "=" Then aa = Keys.Oemplus

            If aa = "]" Then aa = Keys.OemCloseBrackets
            If aa = "[" Then aa = Keys.OemOpenBrackets
            If aa = "\" Then aa = Keys.OemBackslash
            If aa = ";" Then aa = Keys.OemSemicolon
            If aa = "'" Then aa = Keys.OemQuotes
            If aa = "," Then aa = Keys.Oemcomma
            If aa = "." Then aa = Keys.OemPeriod
            If aa = "/" Then aa = Keys.OemQuestion

            If aa = "0" Then aa = Keys.D0
            If aa = "1" Then aa = Keys.D1
            If aa = "2" Then aa = Keys.D2
            If aa = "3" Then aa = Keys.D3
            If aa = "4" Then aa = Keys.D4
            If aa = "5" Then aa = Keys.D5
            If aa = "6" Then aa = Keys.D6
            If aa = "7" Then aa = Keys.D7
            If aa = "8" Then aa = Keys.D8
            If aa = "9" Then aa = Keys.D9


            If aa = "A" Then shiftKey = True
            If aa = "B" Then shiftKey = True
            If aa = "C" Then shiftKey = True
            If aa = "D" Then shiftKey = True
            If aa = "E" Then shiftKey = True
            If aa = "F" Then shiftKey = True
            If aa = "G" Then shiftKey = True
            If aa = "H" Then shiftKey = True
            If aa = "I" Then shiftKey = True
            If aa = "J" Then shiftKey = True
            If aa = "K" Then shiftKey = True
            If aa = "L" Then shiftKey = True
            If aa = "M" Then shiftKey = True
            If aa = "N" Then shiftKey = True
            If aa = "O" Then shiftKey = True
            If aa = "P" Then shiftKey = True
            If aa = "Q" Then shiftKey = True
            If aa = "R" Then shiftKey = True
            If aa = "S" Then shiftKey = True
            If aa = "T" Then shiftKey = True
            If aa = "U" Then shiftKey = True
            If aa = "V" Then shiftKey = True
            If aa = "W" Then shiftKey = True
            If aa = "X" Then shiftKey = True
            If aa = "Y" Then shiftKey = True
            If aa = "Z" Then shiftKey = True

            If aa = "a" Or aa = "A" Then aa = 65
            If aa = "b" Or aa = "B" Then aa = 66
            If aa = "c" Or aa = "C" Then aa = 67
            If aa = "d" Or aa = "D" Then aa = 68
            If aa = "e" Or aa = "E" Then aa = 69
            If aa = "f" Or aa = "F" Then aa = 70
            If aa = "g" Or aa = "G" Then aa = 71
            If aa = "h" Or aa = "H" Then aa = 72
            If aa = "i" Or aa = "I" Then aa = 73
            If aa = "j" Or aa = "J" Then aa = 74
            If aa = "k" Or aa = "K" Then aa = 75
            If aa = "l" Or aa = "L" Then aa = 76
            If aa = "m" Or aa = "M" Then aa = 77
            If aa = "n" Or aa = "N" Then aa = 78
            If aa = "o" Or aa = "O" Then aa = 79
            If aa = "p" Or aa = "P" Then aa = 80
            If aa = "q" Or aa = "Q" Then aa = 81
            If aa = "r" Or aa = "R" Then aa = 82
            If aa = "s" Or aa = "S" Then aa = 83
            If aa = "t" Or aa = "T" Then aa = 84
            If aa = "u" Or aa = "U" Then aa = 85
            If aa = "v" Or aa = "V" Then aa = 86
            If aa = "w" Or aa = "W" Then aa = 87
            If aa = "x" Or aa = "X" Then aa = 88
            If aa = "y" Or aa = "Y" Then aa = 89
            If aa = "z" Or aa = "Z" Then aa = 90


            If clearonly = True Then

            Else

                'clear = f
                If shiftKey = True Then keybd_event(Keys.LShiftKey, 0, 0, 0)

                If aa = "tab" Then
                    keybd_event(Keys.Tab, 0, 0, 0) 'press tab
                    keybd_event(Keys.Tab, 0, &H2, 0)
                Else
                    If Not IsNumeric(aa) Then 'other char
                        SendKeys.Send(aa)
                        Continue For
                    End If


                    keybd_event(aa, 0, 0, 0)  'press char 
                    keybd_event(aa, 0, &H2, 0)

                End If
                If shiftKey = True Then keybd_event(Keys.LShiftKey, 0, &H2, 0) 'release shft


                'clears
                If aa = "tab" Then 'clear tab
                    GetAsyncKeyState(Keys.Tab)
                    Continue For
                End If


                GetAsyncKeyState(aa) 'clear single/clear all

                If shiftKey = True Then GetAsyncKeyState(Keys.LShiftKey)
                shiftKey = False 'clear shift

            End If


        Next

        clearAllKeys()
        emode()

    End Sub

    Sub ctrlRelease()
        keybd_event(Keys.RControlKey, 0, &H2, 0)
        keybd_event(Keys.LControlKey, 0, &H2, 0)
        keybd_event(Keys.Control, 0, &H2, 0)
    End Sub
    Sub rCtrlRelease()
        keybd_event(Keys.RControlKey, 0, &H2, 0)
    End Sub

    Dim ori2 As String = ""
    Dim mstran As String = ""
    Dim ar As New ArrayList

    Sub infiniteLoop(middle As String)
        ar.Clear()
        mstran = Nothing
        Me.Hide()
        Me.TopMost = True
        MsgBox("infinite loop" & vbNewLine & "-> " & middle, vbInformation)
        Me.TopMost = My.Settings.SettingMain_chk_top
        If My.Settings.SettingHidden = False Then Me.Show()
        strandComplete = False
        connect = False
        dnaTxt()
    End Sub

    Dim aa As String
    Sub apisk(f As String)
        '> code
        If tipsDnaToolStripMenuItem.CheckState = CheckState.Checked Then Me.Text = "dna > " & Microsoft.VisualBasic.Right(TextBox1.Text, Val(txtLength.Text)) 'me.text mock 
        TextBox1.Text = "'"

        If GetAsyncKeyState(Keys.Pause) Then
            keyClear(Keys.Pause)
            Exit Sub
        End If 'Exit Sub

        If OskToolStripMenuItem.Checked = True Then 'auto release ctrl, shift, alt before run (for when using osk) 3.11.14
            shiftRelease()
            altRelease()
            ctrlRelease()
        End If

        Dim xx1 As String = MousePosition.X, yy1 As String = MousePosition.Y 'If f.Contains("«xy:") Then

        If Microsoft.VisualBasic.Left(f, 10) = (Chr(9) & "sendkeys:") Or Microsoft.VisualBasic.Left(f, 4) = (Chr(9) & "sk:") Or Microsoft.VisualBasic.Left(f, 3) = "sk:" Then ' = sk: or sendkeys: Then
            If Microsoft.VisualBasic.Left(f, 10) = (Chr(9) & "sendkeys:") Then f = Microsoft.VisualBasic.Right(f, Len(f) - 10) '= sendkeys:, trim sendkeys:tab" Then
            If Microsoft.VisualBasic.Left(f, 4) = (Chr(9) & "sk:") Then f = Microsoft.VisualBasic.Right(f, Len(f) - 4) '= sk:,  trim sk:tab
            SendKeys.Send(f) 'found 'run stock sendkeys (sk: or sendkeys: f)
        Else
            '>sendkeys
            Dim a As String, aa = "", pd1 As Integer
            Dim a1 As String, aa1 As String, main1 As Integer = 0

            Dim ori1 As String = "" 'original c


            For pd = 2 To f.Length '1 without chr(9) in list
loop1:
                If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort
                a = Microsoft.VisualBasic.Left(f, pd) '
                aa = Microsoft.VisualBasic.Right(a, 1) 's string / scan 1 char
#Region "internals"
                If aa = "«" Then 'found internals sk:
                    pd1 = pd
runagain:

                    For j = pd1 To f.Length
                        If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort

                        a1 = Microsoft.VisualBasic.Left(f, j) 'scan individual char
                        aa1 = Microsoft.VisualBasic.Right(a1, 1) 's / char

                        If aa1 = ("»") Then 'internals closed, see whats inside <<middle>>«»ssss
                            Dim middle As String = (Microsoft.VisualBasic.Mid(f, pd, j - pd + 1)) 'middle               '{fullc:/} 

                            If aa = ("«") Then
                                Dim bar As String

                                If middle.Length <= 0 Then
                                    If pd = f.Length + 1 Then finished = True
                                    Exit Sub ' > safe to proceed
                                End If
                                bar = Microsoft.VisualBasic.Mid(middle, 2, middle.Length - 2)                           'fullc:/ (middle)

#Region "<<  >>"
                                Dim skCode As String = bar
                                Dim bricks As String = ""

                                If skCode.Contains(":") Then
                                    Dim charRange As Char = ":"
                                    Dim startIndex As Integer = skCode.IndexOf(charRange) + 1
                                    skCode = Microsoft.VisualBasic.Left(bar, startIndex)  'key
                                    bricks = Microsoft.VisualBasic.Right(bar, bar.Length - startIndex) 'code
                                End If
                                If skCode.Contains("*") And skCode.StartsWith("*:") = False Then
                                    Dim charRange As Char = "*"
                                    Dim startIndex As Integer = skCode.IndexOf(charRange) + 1
                                    skCode = Microsoft.VisualBasic.Left(bar, startIndex)  'key
                                    skCode = Replace(skCode, "*", "")
                                    bricks = Microsoft.VisualBasic.Right(bar, bar.Length - startIndex) 'code
                                End If

                                Select Case skCode
                                    Case "top"
                                        chkItem(chk_top)
                                        aa = ""
                                    Case "audio:"
                                        My.Computer.Audio.Play(LCase(bricks.ToString))
                                        aa = "" 'clear
                                    Case "Audio:"
                                        My.Computer.Audio.Play(bricks, AudioPlayMode.WaitToComplete)
                                        aa = "" 'clear
                                    Case ":" 'sendkeys2
                                        If bricks = "{}" Then
errz:
                                            MsgBox("sendkeys:{" & vbNewLine & "SendKeys string '" & bricks & "' is not valid." & vbNewLine & "}", vbOKOnly, "error")
                                            Exit Sub
                                        End If
                                        Try
                                            SendKeys.Send(bricks)
                                            print(bricks, True) 'clear
                                            a = ""
                                        Catch ex As Exception
                                            GoTo errz
                                        End Try
                                    Case "sendkeys:"
                                        Try
                                            SendKeys.Send(bricks)
                                            print(bricks, True) 'clear
                                            a = ""
                                        Catch ex As Exception
                                            GoTo errz
                                        End Try
                                    Case "sk:"
                                        Try
                                            SendKeys.Send(bricks)
                                            print(bricks, True) 'clear
                                            a = ""
                                        Catch ex As Exception
                                            GoTo errz
                                        End Try
                                    Case "<<>>l"
                                        print("«»", False)
                                        key(Keys.Left)
                                        a = ""
                                    Case "<<>>"
                                        print("«»", False)
                                        a = ""
                                    Case "<<"
                                        print("«", False)
                                        a = ""
                                    Case ">>"
                                        print("»", False)
                                        a = ""
                                    Case "stop-audio"
                                        My.Computer.Audio.Stop()
                                        aa = ""
                                    Case "date:"  'date
                                        Dim d As String = (Date.Now.Day.ToString & "/" & Date.Now.Month.ToString & "/" & Date.Now.Year.ToString)
                                        If bricks > "" Then
                                            d = (Date.Now.Month.ToString & "/" & Date.Now.Day.ToString & "/" & Date.Now.Year.ToString)
                                            d = Replace(d, "/", bricks)
                                        End If
                                        Clipboard.SetText(d)
                                        a = ""
                                    Case "date"  'date
                                        Dim d As Date = Date.Now.Date.ToString
                                        apisk(" " + d)
                                        a = ""
                                    Case "time:"  'time
                                        Dim hourz = Date.Now.Hour.ToString
                                        Dim z As String = "AM"
                                        If hourz > 12 Then z = "PM" : hourz -= 12
                                        Dim t As String = hourz & ":" & Date.Now.Minute.ToString & ":" & Date.Now.Second.ToString & ":" & z
                                        If bricks > "" Then t = Replace(t, ":", bricks) ': t = Replace(t, " ", brick)
                                        Clipboard.SetText(t)
                                        a = ""
                                    Case "time"  'time
                                        Dim t As Date = Date.Now.TimeOfDay.ToString
                                        apisk(" " + t)
                                        a = ""
                                    Case "wait:"  'timeouts wait s
                                        If IsNumeric(bricks) Then
                                            timeout1(bricks)
                                            a = ""
                                        End If
                                    Case "w:"  'wait s
                                        If IsNumeric(bricks) Then
                                            timeout1(bricks)
                                            a = ""
                                        End If
                                    Case "Wait:"  'Wait m
                                        If IsNumeric(bricks) Then
                                            timeout2(bricks)
                                            a = ""
                                        End If
                                    Case "W:"  'wait m
                                        If IsNumeric(bricks) Then
                                            timeout2(bricks)
                                            a = ""
                                        End If
                                    Case "timeout:"  'timeout seconds
                                        If IsNumeric(bricks) Then
                                            timeout1(bricks)
                                            a = ""
                                        End If
                                    Case "Timeout:" 'timeout milliseconds
                                        If IsNumeric(bricks) Then
                                            timeout2(bricks)
                                            a = ""
                                        End If
                                    Case "Pause:"  'pause seconds
                                        If IsNumeric(bricks) Then
                                            timeout1(bricks)
                                            a = ""
                                        End If
                                    Case "seconds:"  'pause seconds
                                        If IsNumeric(bricks) Then
                                            timeout1(bricks)
                                            a = ""
                                        End If
                                    Case "milliseconds:" 'timeout milliseconds
                                        If IsNumeric(bricks) Then
                                            timeout2(bricks)
                                            a = ""
                                        End If
                                    Case "ms:" 'timeout milliseconds
                                        If IsNumeric(bricks) Then
                                            timeout2(bricks)
                                            a = ""
                                        End If
                                    Case "s" 'timeout  seconds
                                        If bar.Length = 1 Then
                                            timeout1(1)
                                            a = ""
                                        End If
                                    Case "s:" 'timeout  seconds
                                        If bar.Length > skCode.Length And IsNumeric(bricks) Then 'is "" or not#
                                            timeout1(bricks)
                                            a = ""
                                        End If
                                        If bar.Length = 2 Then
                                            timeout1(1)
                                            a = ""
                                        End If
                                    Case "M" 'timeout milliseconds
                                        If bar.Length = 1 Then
                                            timeout2(33)
                                            a = ""
                                        End If
                                    Case "m" 'timeout milliseconds
                                        If bar.Length = 1 Then
                                            timeout2(111)
                                            a = ""
                                        End If
                                    Case "m:" 'timeout milliseconds
                                        If bar.Length > skCode.Length And IsNumeric(Microsoft.VisualBasic.Right(bar, bar.Length - skCode.Length)) Then 'is "" or not#
                                            Dim brick As Double = Microsoft.VisualBasic.Right(bar, bar.Length - skCode.Length)
                                            timeout2(brick)
                                            a = ""
                                        End If
                                        If bar.Length = 2 Then
                                            timeout2(33)
                                            a = ""
                                        End If
                                    Case "pause:" 'timeout milliseconds
                                        If IsNumeric(bricks) Then
                                            timeout2(bricks)
                                            a = ""
                                        End If
                                    Case "p:" 'timeout m
                                        If IsNumeric(bricks) Then
                                            timeout2(bricks)
                                            a = ""
                                        End If
                                    Case "P:" 'timeout s
                                        If IsNumeric(bricks) Then
                                            timeout1(bricks)
                                            a = ""
                                        End If
                                    Case "T:" 'timeout m
                                        If IsNumeric(bricks) Then
                                            timeout2(bricks)
                                            a = ""
                                        End If
                                    Case "t:" 'timeout s
                                        If IsNumeric(bricks) Then
                                            timeout1(bricks)
                                            a = ""
                                        End If
                                    Case "manual-timeout:" 'timeout
                                        If IsNumeric(bricks) Then
                                            timeoutM(bricks)
                                            a = ""
                                        End If
                                    Case "sleep:" 'sleep
                                        If IsNumeric(bricks) Then
                                            Application.DoEvents()
                                            System.Threading.Thread.Sleep(1)
                                            Application.DoEvents()
                                            System.Threading.Thread.Sleep(bricks)
                                            a = ""
                                        End If
                                    Case ",:" 'sleep
                                        If IsNumeric(bricks) Then
                                            Application.DoEvents()
                                            System.Threading.Thread.Sleep(1)
                                            Try
                                                Application.DoEvents()
                                                System.Threading.Thread.Sleep(bricks)
                                            Catch ex As Exception
                                            End Try
                                            a = ""
                                        End If
                                    Case "," 'sleep
                                        Application.DoEvents()
                                        System.Threading.Thread.Sleep(1)
                                        Application.DoEvents()
                                        System.Threading.Thread.Sleep(My.Settings.SettingOEMSleep)
                                        a = ""
                                        '/timeouts
                                    Case "do" 'sleep
                                        Application.DoEvents()
                                        a = ""
                                    Case "xy" 'know mouse x y
                                        xx1 = MousePosition.X
                                        yy1 = MousePosition.Y
                                        a = ""
                                    Case "xy:" 'mouse x y
                                        If IsNumeric(bricks.Replace("-", "")) Then
                                            Dim n As Boolean = False
                                            For d = 1 To bricks.Length 'search for -
                                                Dim d1 As String = Microsoft.VisualBasic.Left(bricks, d) '
                                                Dim dd1 As String = Microsoft.VisualBasic.Right(d1, 1) 's
                                                If dd1 = "-" Then
                                                    If d = 1 And bricks.StartsWith("-") Then
                                                        bricks = Microsoft.VisualBasic.Right(bricks, bricks.Length - 1)
                                                        n = True
                                                        Continue For
                                                    End If
                                                    Dim x1 As Integer = Microsoft.VisualBasic.Left(bricks, d - 1) 'grab x
                                                    Dim y1 As Integer = Microsoft.VisualBasic.Right(bricks, bricks.Length - d) 'grab y
                                                    If IsNumeric(x1) And IsNumeric(y1) Then
                                                        If n = True Then
                                                            x1 = x1 - x1 * 2 'negative screen
                                                            SetCursorPos((x1), y1)
                                                        Else
                                                            SetCursorPos(x1, y1) 'set mouse position 'Else If chk_tips.CheckState = CheckState.Checked Then MsgBox("error in mouse position", vbExclamation, "error")
                                                        End If
                                                        Exit For
                                                    End If
                                                End If
                                            Next
                                        End If
                                        a = ""
                                    Case "io" 'clear io txt
                                        TextBox1.Clear()
                                        a = ""
                                    Case "io:" 'dna > io:
                                        a = ""
                                        Select Case bricks
                                            Case ""
                                                TextBox1.Text = "»" 'lock
                                            Case ">>"
                                                TextBox1.Text = "»"
                                            Case "<<"
                                                TextBox1.Text = "«"
                                            Case Else
                                                TextBox1.Text = bricks
                                        End Select
                                        Exit Sub
                                    Case "#:" 'randomNumb -
                                        If IsNumeric(bricks.Replace("-", "")) And Len(bricks.Replace("-", "")) > 1 Then
                                            For d = 1 To bricks.Length 'search for -
                                                Dim d1 As String = Microsoft.VisualBasic.Left(bricks, d) '
                                                Dim dd1 As String = Microsoft.VisualBasic.Right(d1, 1) 's
                                                If dd1 = "-" Then
                                                    Dim x1 As Long = Microsoft.VisualBasic.Left(bricks, d - 1) 'grab x
                                                    Dim y1 As Long = Microsoft.VisualBasic.Right(bricks, bricks.Length - d) 'grab y
                                                    If IsNumeric(x1) And IsNumeric(y1) Then
                                                        Dim x As Random
                                                        Dim n As Long
                                                        x = New Random
                                                        If x1 > y1 + 1 Then Exit Sub
                                                        If x1 > Integer.MaxValue Or y1 > Integer.MaxValue Then Exit Sub
                                                        n = x.Next(x1, y1 + 1)  '#x-#x
                                                        apisk("x" & n)
                                                        Exit For
                                                    End If
                                                End If
                                            Next
                                        End If
                                        a = ""
                                    Case "#" 'randomNumb
                                        randNumb(True, False, False)
                                        a = ""
                                    Case "x" 'randomletter a-z
                                        randNumb(False, True, False)
                                        a = ""
                                    Case "X" 'randomletter A-Z
                                        randNumb(False, True, True)
                                        a = ""
                                    Case "on" 'engine chk on
                                        chk_timer1_on_val.CheckState = CheckState.Checked
                                    Case "off" 'chk on
                                        chk_timer1_on_val.CheckState = CheckState.Unchecked
                                    Case "yesno:"
                                        yn = MsgBox(bricks, vbYesNo, "Verify")
                                        If yn = vbYes Then
                                            a = ""
                                        Else
                                            emode()
                                            Exit Sub
                                        End If
                                    Case "yesno" 'chk on
                                        yn = MsgBox("continue?", vbYesNo)
                                        If yn = vbYes Then
                                            a = ""
                                        Else
                                            emode()
                                            Exit Sub
                                        End If
                                    Case "ignore-mouse-uncheck" '«ignore-mouse-uncheck»
                                        ChkMouse.Checked = False
                                        a = ""
                                    Case "ignore-mouse-check" '«ignore-mouse-check»
                                        ChkMouse.Checked = True
                                        a = ""
                                    Case "save" 'save
                                        saveSettings()
                                        a = ""
                                    Case "restart" 'restart
                                        Application.Restart()
                                        Exit Sub
                                    Case "exit" 'exit
                                        saveSettings()
                                        Close()
                                        Exit Sub
                                    Case "show" 'show
                                        Me.Show()
                                        Me.TopMost = True
                                        Me.TopMost = My.Settings.SettingMain_chk_top
                                        a = ""
                                    Case "hide" 'hide
                                        checkIfOn()
                                        a = ""
                                    'Ӂ җ Ӝ  ӝ 
                                    Case "mouse-up" 'move mouse position up 1 px
                                        SetCursorPos(MousePosition.X, MousePosition.Y - 1)
                                        a = ""
                                    Case "mouse-down" 'mouse down
                                        SetCursorPos(MousePosition.X, MousePosition.Y + 1)
                                        a = ""
                                    Case "mouse-left" 'mouse left
                                        SetCursorPos(MousePosition.X - 1, MousePosition.Y)
                                        a = ""
                                    Case "mouse-right" 'right
                                        SetCursorPos(MousePosition.X + 1, MousePosition.Y)
                                        a = ""
                                    Case "leftclick" 'leftclick
                                        leftclick()
                                        a = ""
                                    Case "lefthold" 'lefthold
                                        lefthold()
                                        a = ""
                                    Case "leftrelease" 'leftrelease
                                        leftrelease()
                                        a = ""
                                    Case "middleclick" 'middleclick
                                        middleclick()
                                        a = ""
                                    Case "middlehold" 'middlehold
                                        middlehold()
                                        a = ""
                                    Case "middlerelease" 'middlerelease
                                        middlerelease()
                                        a = ""
                                    Case "rightclick" 'rightclick
                                        rightclick()
                                        a = ""
                                    Case "righthold" 'righthold
                                        righthold()
                                        a = ""
                                    Case "rightrelease" 'rightrelease
                                        rightrelease()
                                        a = ""
                                    Case "left-click" 'leftclick
                                        'more than 9. add skcodes '*#' and change vbar to starp
                                        If bar.Contains("*") Then
                                            Dim vbar As Integer = bar.Length - skCode.Length - 1
                                            If IsNumeric(Microsoft.VisualBasic.Right(bar, vbar)) Then
                                                For vbarf = 1 To Val(Microsoft.VisualBasic.Right(bar, vbar))
                                                    leftclick()
                                                Next
                                            End If
                                        Else
                                            leftclick()
                                        End If
                                        a = ""
                                    Case "left-hold" 'lefthold
                                        lefthold()
                                        a = ""
                                    Case "left-release" 'leftrelease
                                        leftrelease()
                                        a = ""
                                    Case "middle-click" 'middleclick
                                        middleclick()
                                        a = ""
                                    Case "middle-hold" 'middlehold
                                        middlehold()
                                        a = ""
                                    Case "middle-release" 'middlerelease
                                        middlerelease()
                                        a = ""
                                    Case "right-click" 'rightclick
                                        rightclick()
                                        a = ""
                                    Case "right-hold" 'righthold
                                        righthold()
                                        a = ""
                                    Case "right-release" 'rightrelease
                                        rightrelease()
                                        a = ""
                                    Case "l-c" 'leftclick
                                        leftclick()
                                        a = ""
                                    Case "l-h" 'lefthold
                                        lefthold()
                                        a = ""
                                    Case "l-r" 'leftrelease
                                        leftrelease()
                                        a = ""
                                    Case "m-c" 'middleclick
                                        middleclick()
                                        a = ""
                                    Case "m-h" 'middlehold
                                        middlehold()
                                        a = ""
                                    Case "m-r" 'middlerelease
                                        middlerelease()
                                        a = ""
                                    Case "r-c" 'rightclick
                                        rightclick()
                                        a = ""
                                    Case "r-h" 'righthold
                                        righthold()
                                        a = ""
                                    Case "r-r" 'rightrelease
                                        rightrelease()
                                        a = ""
                                    Case "url: " 'url
                                        Try
                                            Shell(bricks, AppWinStyle.NormalNoFocus, 1, 1)
                                            a = ""
                                        Catch ex As Exception
                                            MsgBox("File not found." & vbNewLine & bricks, vbExclamation, "«url:Error»")
                                            clearAllKeys()
                                            emode()
                                            Exit Sub
                                        End Try
                                    Case "web:" 'web
                                        WebBrowser1.Navigate(bricks, True)
                                        a = ""
                                    Case "App:" 'app activate PID
                                        If IsNumeric(Microsoft.VisualBasic.Right(bar, bar.Length - skCode.Length)) Then
                                            Try
                                                If bricks > "" Then AppActivate(bricks)
                                            Catch ex As Exception
                                                MsgBox("Process '{" & bricks & "}' was not found.", vbExclamation, "«app:Error»")
                                                clearAllKeys()
                                                emode()
                                                Exit Sub
                                            End Try
                                        End If
                                        a = ""
                                    Case "app:" 'app activate TITLE
                                        Dim tout As Integer = 0
rtapp:
                                        tout += 1
                                        If tout >= My.Settings.SettingAppErrorAutoTries Then
                                            If chk_tips.Checked = True Then MsgBox("«app:" & bricks & "» not found in time..." & vbNewLine & "ato + enter: adjust tries (" & My.Settings.SettingAppErrorAutoTries & ")", vbInformation)
                                            keybd_event(Keys.Pause, 0, 0, 0)
                                            keybd_event(Keys.Pause, 0, 2, 0)
                                        End If 'exit if no app
                                        If GetAsyncKeyState(Keys.Pause) Then Exit Sub
                                        Try
                                            If bricks > "" Then
                                                AppActivate(bricks) 'title
                                                sleep(1)
                                                a = ""
                                            End If
                                        Catch ex As Exception
                                            If My.Settings.SettingAutoRetryAppError = True Then
                                                Application.DoEvents()
                                                sleep(My.Settings.SettingOEMSleep)
                                                GoTo rtapp
                                            End If
                                            Me.TopMost = True
                                            rt = MsgBox("Process '{" & bricks & "}' was not found." & vbNewLine & vbNewLine & "Retry?" & vbNewLine & vbNewLine & "ar + enter: auto retry" & vbNewLine & "ato + enter: adjust tries (" & My.Settings.SettingAppErrorAutoTries & ")" & vbNewLine & "pause break: clear", vbYesNo, "«app:Error»")
                                            Me.TopMost = My.Settings.SettingMain_chk_top
                                            If rt = vbYes Then
                                                GoTo rtapp
                                            Else
                                                clearAllKeys()
                                                emode()
                                                Exit Sub
                                            End If
                                        End Try
                                    Case "space"
                                        skCodes("space", bar, Keys.Space)
                                    Case "up"
                                        skCodes("up", bar, Keys.Up)
                                    Case "down"
                                        skCodes("down", bar, Keys.Down)
                                    Case "left"
                                        skCodes("left", bar, Keys.Left)
                                    Case "Left-Click"
                                        skCodes("Left-Click", bar, Keys.LButton) '
                                    Case "Right-Click"
                                        skCodes("Right-Click", bar, Keys.RButton) '
                                    Case "Middle-Click"
                                        skCodes("Middle-Click", bar, Keys.MButton) '
                                    Case "right"
                                        skCodes("right", bar, Keys.Right)
                                    Case "esc"
                                        skCodes("esc", bar, Keys.Escape)
                                    Case "escape"
                                        skCodes("escape", bar, Keys.Escape)
                                    Case "tab"
                                        skCodes("tab", bar, Keys.Tab)
                                    Case "insert"
                                        skCodes("insert", bar, Keys.Insert)
                                    Case "enter"
                                        skCodes("enter", bar, Keys.Enter)
                                    Case "pause"
                                        skCodes("pause", bar, Keys.Pause)
                                    Case "break"
                                        skCodes("break", bar, Keys.Pause)
                                    Case "home"
                                        skCodes("home", bar, Keys.Home)
                                    Case "end"
                                        skCodes("end", bar, Keys.End)
                                    Case "pageup"
                                        skCodes("pageup", bar, Keys.PageUp)
                                    Case "pagedown"
                                        skCodes("pagedown", bar, Keys.PageDown)
                                    Case "page-up"
                                        skCodes("page-up", bar, Keys.PageUp)
                                    Case "page-down"
                                        skCodes("page-down", bar, Keys.PageDown)
                                    Case "win"
                                        skCodes1("win", bar, Keys.LWin)
                                    Case "-win"
                                        skCodes1("-win", bar, Keys.LWin)
                                    Case "rwin"
                                        skCodes("rwin", bar, Keys.RWin)
                                    Case "lwin"
                                        skCodes("lwin", bar, Keys.LWin)
                                    Case "menu"
                                        skCodes("menu", bar, 93)
                                    Case "alt"
                                        skCodes1("alt", bar, Keys.Menu)
                                    Case "-alt"
                                        skCodes1("-alt", bar, Keys.Menu)
                                    Case "lalt"
                                        skCodes("lalt", bar, Keys.LMenu)
                                    Case "ralt"
                                        skCodes("ralt", bar, Keys.RMenu)
                                    Case "leftalt"
                                        skCodes("leftalt", bar, Keys.LMenu)
                                    Case "rightalt"
                                        skCodes("rightalt", bar, Keys.RMenu)
                                    Case "ctrl"
                                        skCodes1("ctrl", bar, Keys.ControlKey)
                                    Case "-ctrl"
                                        skCodes1("-ctrl", bar, Keys.ControlKey)
                                    Case "lctrl"
                                        skCodes("lctrl", bar, Keys.LControlKey)
                                    Case "rctrl"
                                        skCodes("rctrl", bar, Keys.RControlKey)
                                    Case "leftctrl"
                                        skCodes("leftctrl", bar, Keys.LControlKey)
                                    Case "rightctrl"
                                        skCodes("rightctrl", bar, Keys.RControlKey)
                                    Case "shift"
                                        skCodes1("shift", bar, Keys.ShiftKey)
                                    Case "-shift"
                                        skCodes1("-shift", bar, Keys.ShiftKey)
                                    Case "lshift"
                                        skCodes("lshift", bar, Keys.LShiftKey)
                                    Case "rshift"
                                        skCodes("rshift", bar, Keys.RShiftKey)
                                    Case "leftshift"
                                        skCodes("leftshift", bar, Keys.LShiftKey)
                                    Case "rightshift"
                                        skCodes("rightshift", bar, Keys.RShiftKey)
                                    Case "delete"
                                        skCodes("delete", bar, Keys.Delete)
                                    Case "print-screen"
                                        skCodes("print-screen", bar, Keys.PrintScreen)
                                    'Case "sleep"
                                    '    skCodes("sleep", bar, Keys.Sleep)
                                    Case "printscreen"
                                        skCodes("printscreen", bar, Keys.PrintScreen)
                                    Case "f1"
                                        skCodes("f1", bar, Keys.F1)
                                    Case "f2"
                                        skCodes("f2", bar, Keys.F2)
                                    Case "f3"
                                        skCodes("f3", bar, Keys.F3)
                                    Case "f4"
                                        skCodes("f4", bar, Keys.F4)
                                    Case "f5"
                                        skCodes("f5", bar, Keys.F5)
                                    Case "f6"
                                        skCodes("f6", bar, Keys.F6)
                                    Case "f7"
                                        skCodes("f7", bar, Keys.F7)
                                    Case "f8"
                                        skCodes("f8", bar, Keys.F8)
                                    Case "f9"
                                        skCodes("f9", bar, Keys.F7)
                                    Case "f10"
                                        skCodes("f10", bar, Keys.F10)
                                    Case "f11"
                                        skCodes("f11", bar, Keys.F11)
                                    Case "f12"
                                        skCodes("f12", bar, Keys.F12)
                                    Case "volume-up"
                                        skCodes("volume-up", bar, Keys.VolumeUp)
                                    Case "volume-down"
                                        skCodes("volume-down", bar, Keys.VolumeDown)
                                    Case "volume-mute"
                                        skCodes("volume-mute", bar, Keys.VolumeMute)
                                    Case "volumeup"
                                        skCodes("volumeup", bar, Keys.VolumeUp)
                                    Case "volumedown"
                                        skCodes("volumedown", bar, Keys.VolumeDown)
                                    Case "volumemute"
                                        skCodes("volumemute", bar, Keys.VolumeMute)
                                    Case "mute"
                                        skCodes("mute", bar, Keys.VolumeMute)
                                    Case "vol+"
                                        skCodes("vol+", bar, Keys.VolumeUp)
                                    Case "vol-"
                                        skCodes("vol-", bar, Keys.VolumeDown)
                                    Case "backspace"
                                        skCodes("backspace", bar, Keys.Back)
                                    Case "back-space"
                                        skCodes("back-space", bar, Keys.Back)
                                    Case "bs"
                                        skCodes("bs", bar, Keys.Back)
                                    Case "capslock"
                                        skCodes("capslock", bar, Keys.CapsLock)
                                    Case "caps-lock"
                                        skCodes("caps-lock", bar, Keys.CapsLock)
                                    Case "caps"
                                        skCodes("caps", bar, Keys.CapsLock)
                                    Case "numlock"
                                        skCodes("numlock", bar, Keys.NumLock)
                                    Case "num-lock"
                                        skCodes("num-lock", bar, Keys.NumLock)
                                    Case "num"
                                        skCodes("num", bar, Keys.NumLock)
                                    Case "scroll"
                                        skCodes("scroll", bar, Keys.Scroll)
                                    Case "scroll-lock"
                                        skCodes("scroll-lock", bar, Keys.Scroll)
                                    Case "media-stop"
                                        skCodes("media-stop", bar, Keys.MediaStop)
                                    Case "media-play-pause"
                                        skCodes("media-play-pause", bar, Keys.MediaPlayPause)
                                    Case "media-previous-track"
                                        skCodes("media-previous-track", bar, Keys.MediaPreviousTrack)
                                    Case "media-next-track"
                                        skCodes("media-next-track", bar, Keys.MediaNextTrack)
                                    Case "media-select"
                                        skCodes("media-select", bar, Keys.SelectMedia)
                                    Case "ws" 'ignore white space t
                                        ignoreWhiteSpace_g = True
                                        a = ""
                                    Case "-ws" 'ignore white space f
                                        ignoreWhiteSpace_g = False
                                        a = ""
                                    Case "clearallkeys"
                                        clearAllKeys()
                                        a = ""
                                    Case "ucase" 'print clipboard length
                                        Dim cb As String = UCase(Clipboard.GetText())
                                        Clipboard.SetText(cb.ToString)
                                        a = ""
                                    Case "lcase" 'print clipboard length
                                        Dim cb As String = LCase(Clipboard.GetText())
                                        Clipboard.SetText(cb.ToString)
                                        a = ""
                                    Case "cbl" 'print clipboard length
                                        Dim cb As String = Clipboard.GetText.ToString.Length
                                        print(cb, False)
                                        a = ""
                                    Case "cb" 'print clipboard
                                        print(Clipboard.GetText(), False)
                                        a = ""
                                    Case "cb:" 'set clipboard
                                        If bricks > "" Then Clipboard.SetText(bricks)
                                        a = ""
                                    Case "clipboard:" 'set clipboard
                                        If bricks > "" Then Clipboard.SetText(bricks)
                                        a = ""
                                    Case "++" 'clipboard++
                                        Dim cb = Val(Clipboard.GetText) + 1
                                        sleep(1)
                                        Clipboard.SetText(cb)
                                        print(cb, False)
                                        a = ""
                                    Case "<cb" 'clipboard to list
                                        ListBox1.Items.Add(Clipboard.GetText)
                                        selectBottomItem()
                                        DeleteAllToolStripMenuItem.Text = "reload"
                                        a = ""
                                    Case ">cb" 'clipboard to txt
                                        Me.Show()
                                        If My.Settings.SettingDnaX = True Then tipsDnaToolStripMenuItem.Checked = False
                                        If Me.Text = "" Then dnaTxt()
                                        txtString.Focus()
                                        Dim getLine1 = txtString.GetLineFromCharIndex(txtString.SelectionStart - txtString.TextLength)

                                        If txtString.Text = "" Then txtString.Text += "«enter»" & Chr(13)

                                        getLine1 = txtString.GetLineFromCharIndex(txtString.SelectionStart - txtString.TextLength)
                                        Dim x13 = txtString.Lines(getLine1)

                                        AppActivate("dna")

                                        If x13.ToString.Contains("«>cb»") Then
                                            MsgBox("infinite Loop", vbOKOnly)
                                            strandComplete = False
                                            connect = False
                                            Exit Sub
                                        End If

                                        apisk(" " & x13) 'run firt line functionality
                                        txtString.AppendText(Clipboard.GetText)
                                        tipsDnaToolStripMenuItem.Checked = My.Settings.SettingDnaX
                                        a = ""
                                    Case "--" 'clipboard++
                                        Dim cb As String = Val(Clipboard.GetText) - 1
                                        sleep(1)
                                        Clipboard.SetText(cb)
                                        print(cb, False)
                                        a = ""
                                    Case "a" 'a:$
                                        Me.TopMost = True
                                        MsgBox(Clipboard.GetText, vbInformation, "cb")
                                        Me.TopMost = My.Settings.SettingMain_chk_top
                                        a = ""
                                    Case "Mod:" 'clipboard+#
                                        Try
                                            Dim brick As String = Microsoft.VisualBasic.Right(bar, bar.Length - skCode.Length)
                                            Dim cb As String = Val(Clipboard.GetText) Mod Val(brick)
                                            Clipboard.SetText(cb)
                                        Catch ex As Exception
                                            MsgBox(ex.Message, vbExclamation, "Mod:")
                                        End Try
                                        a = ""
                                    Case "*:" 'clipboard+#
                                        Try
                                            Dim cb As String = Val(Clipboard.GetText) * Val(bricks)
                                            Clipboard.SetText(cb)
                                        Catch ex As Exception
                                            MsgBox(ex.Message, vbExclamation, "*:")
                                        End Try
                                        a = ""
                                    Case "/:" 'clipboard+#
                                        Try
                                            Dim cb As String = Val(Clipboard.GetText) / Val(bricks)
                                            Clipboard.SetText(cb)
                                        Catch ex As Exception
                                            MsgBox(ex.Message, vbExclamation, "/:")
                                        End Try
                                        a = ""
                                    Case "\:" 'clipboard+#
                                        Try
                                            Dim cb As String = Val(Clipboard.GetText) \ Val(bricks)
                                            Clipboard.SetText(cb)
                                        Catch ex As Exception
                                            MsgBox(ex.Message, vbExclamation, "\:")
                                        End Try
                                        a = ""
                                    Case "+:" 'clipboard+#
                                        Try
                                            Dim cb As String = Val(Clipboard.GetText) + Val(bricks)
                                            Clipboard.SetText(cb)
                                        Catch ex As Exception
                                            MsgBox(ex.Message, vbExclamation, "+:")
                                        End Try
                                        a = ""
                                    Case "-:" 'clipboard-#
                                        Try
                                            Dim cb As String = Val(Clipboard.GetText) - Val(bricks)
                                            Clipboard.SetText(cb)
                                        Catch ex As Exception
                                            MsgBox(ex.Message, vbExclamation, "-:")
                                        End Try
                                        a = ""
                                    Case "b:" 'browser
                                        showBrowserTab()
                                        WebBrowser1.Navigate(bricks)
                                        a = ""
                                    Case "replace:" 'clipboard
                                        Dim b1 As String = bricks.ToString()
                                        For d = 1 To b1.Length 'search for -
                                            Dim d1 As String = Microsoft.VisualBasic.Left(b1, d) 'string
                                            Dim dd1 As String = Microsoft.VisualBasic.Right(d1, 1) 'chr
                                            If dd1 = "|" Then
                                                Dim x1s As String = Microsoft.VisualBasic.Left(b1, d - 1) 'grab x
                                                Dim y1s As String = Microsoft.VisualBasic.Right(b1, b1.Length - d) 'grab y
                                                Dim x1n = ""
                                                Dim x1c = Clipboard.GetText
                                                x1n = Replace(x1c, x1s, y1s)
                                                Clipboard.SetText(x1n)
                                                Exit For
                                            End If
                                        Next
                                        a = ""
                                                                                'dz@k Productions 2013, 2016
                                        'final
                                    Case "x:y" 'return mouse position
                                        SetCursorPos(xx1, yy1)
                                        a = ""
                                    Case "return-mouse" 'return mouse position
                                        SetCursorPos(xx1, yy1)
                                        a = ""
                                    Case "r-m" 'return mouse position
                                        SetCursorPos(xx1, yy1)
                                        a = ""
                                End Select

#End Region

#Region "<< connect >>"
                                'connect if «» 2.15.2016, 25, 3.24, 3.28 «*#»,«*#:#-#»
                                If Not aa = "" And middle.Contains("«") And middle.Contains("»") Then
                                    For i9 = 0 To ListBox1.Items.Count 'scan db 
                                        If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort
                                        If i9 <= -1 Or i9 >= ListBox1.Items.Count Then Exit For '< or > item count

                                        If a = "" Then Exit For

                                        If middle.StartsWith("«'") Or middle.StartsWith("«//") Or
                                           middle = "«enter»" Or middle.StartsWith("«enter*") Or
                                           middle = "«left»" Or middle.StartsWith("«left*") Or
                                           middle = "«right»" Or middle.StartsWith("«right*") Or
                                           middle = "«down»" Or middle.StartsWith("«down*") Or
                                           middle = "«up»" Or middle.StartsWith("«up*") Or
                                           middle = "«tab»" Or middle.StartsWith("«tab*") Or
                                           middle = "«bs»" Or middle.StartsWith("«bs*") Or
                                           middle = "«space»" Or middle.StartsWith("«space*") Or
                                           middle = "«escape»" Or middle = "«insert»" Or middle = "«home»" Or middle = "«end»" Or middle = "«delete»" Or
                                           middle = "«f1»" Or middle = "«f2»" Or middle = "«f3»" Or middle = "«f4»" Or middle = "«f5»" Or middle = "«f6»" Or middle = "«f7»" Or middle = "«f8»" Or middle = "«f9»" Or middle = "«f10»" Or middle = "«f11»" Or middle = "«f12»" Or
                                           middle = "«shift»" Or middle = "«ctrl»" Or middle = "«alt»" Or middle = "«win»" Or
                                           middle = "«-shift»" Or middle = "«-ctrl»" Or middle = "«-alt»" Or middle = "«-win»" Or
                                           middle = "«lshift»" Or middle = "«lctrl»" Or middle = "«lalt»" Or middle = "«lwin»" Or
                                           middle = "«-lshift»" Or middle = "«-lctrl»" Or middle = "«-lalt»" Or middle = "«-win»" Or
                                           middle = "«rshift»" Or middle = "«rctrl»" Or middle = "«ralt»" Or middle = "«rwin»" Or
                                           middle = "«-rshift»" Or middle = "«-rctrl»" Or middle = "«-ralt»" Or middle = "«-rwin»" Then
                                            Exit For
                                        End If


#Region "connect1*#"
                                        If middle.Contains("*#:") Or middle.Contains("*r:") Then 'connect1*#:#-#'rand #:#-#
                                            Dim starp As String = Microsoft.VisualBasic.Right(middle, middle.Length - middle.IndexOf("*") - 1)
                                            Dim bb As Integer, cc As Integer, ax As String, b As String
                                            cc = middle.IndexOf(":") + 2
                                            bb = middle.LastIndexOf("-") + 1
                                            ax = Microsoft.VisualBasic.Mid(middle, cc, bb - cc)
                                            b = Microsoft.VisualBasic.Right(middle, middle.Length - bb)
                                            b = Replace(b, "»", "")

                                            If IsNumeric(ax) And IsNumeric(b) Then
                                                Dim x As Random
                                                Dim n As Long
                                                x = New Random
                                                If ax > b + 1 Then Exit Sub
                                                If ax > Integer.MaxValue Or b > Integer.MaxValue Then Exit Sub
                                                n = x.Next(ax, b + 1)  '#x-#x
                                                starp = n
                                            Else
                                                MsgBox("error: '" & ax & "' and '" & b & "' must be numbers." & vbNewLine & middle, vbInformation)
                                                Exit Sub
                                            End If

                                            Dim c As String = middle
                                            c = Microsoft.VisualBasic.Left(middle, middle.IndexOf("*"))
                                            c = c & "»"

                                            mstran = c 'bcodes
                                            ar.Add(mstran)
                                            'Console.WriteLine("m:" & middle)
                                            'Console.WriteLine(mstran)

                                            If ListBox1.Items.Item(i9).ToString.StartsWith(c) Then
                                                Dim i91 As String = ""
                                                i91 = Microsoft.VisualBasic.Right(ListBox1.Items.Item(i9).ToString, ListBox1.Items.Item(i9).ToString.Length - c.Length)

                                                For cc = 0 To ar.Count - 1
                                                    If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort
                                                    'Console.WriteLine("ar:" & ar(cc))

                                                    If i91.Contains(ar(cc)) Then
                                                        infiniteLoop(middle)
                                                        Exit Sub
                                                    End If
                                                Next
                                                If strandComplete = True Then
                                                    strandComplete = False
                                                Else
                                                    If IsNumeric(starp) = 0 Then
                                                        '0
                                                    Else
                                                        If IsNumeric(starp) Then
                                                            For lc = 1 To Val(starp)
                                                                If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort
                                                                apisk(" " + i91)
                                                            Next lc
                                                        Else
                                                            infiniteLoop(middle) ' error
                                                            Exit Sub
                                                        End If
                                                    End If
                                                End If
                                                Exit For
                                            End If

                                        End If
#End Region

#Region "connect1*"

                                        If middle.Contains("*") And middle.StartsWith("«*:") = False Then 'connect1*
                                            Dim n As String = middle
                                            Dim c As String = middle
                                            c = Microsoft.VisualBasic.Left(middle, middle.IndexOf("*"))
                                            c = c & "»" 'bcode
                                            n = Microsoft.VisualBasic.Right(middle, middle.Length - middle.IndexOf("*"))
                                            n = Replace(n, "*", "")
                                            n = Replace(n, "»", "") '*#

                                            If ListBox1.Items.Item(i9).ToString.StartsWith(c) Then

                                                Dim i91 As String = ""
                                                i91 = Microsoft.VisualBasic.Right(ListBox1.Items.Item(i9).ToString, ListBox1.Items.Item(i9).ToString.Length - c.Length)

                                                mstran = c 'bcode
                                                ar.Add(mstran)
                                                'Console.WriteLine("m:" & middle)
                                                'Console.WriteLine(mstran)

                                                For cc = 0 To ar.Count - 1
                                                    If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort
                                                    'Console.WriteLine("ar:" & ar(cc))
                                                    If i91.Contains(ar(cc)) Then
                                                        infiniteLoop(middle)
                                                        Exit Sub
                                                    End If
                                                Next

                                                If strandComplete = True Then
                                                    strandComplete = False
                                                Else
                                                    For lc = 1 To Val(n) '*n
                                                        If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort
                                                        apisk(" " + i91)
                                                    Next lc
                                                End If
                                                Exit For
                                            End If
                                        End If
#End Region

#Region "connect1"
                                        If ListBox1.Items.Item(i9).ToString.StartsWith(middle) Then 'connect1
                                            If middle = "" Then Exit Sub
                                            Dim i91 As String = ""
                                            i91 = Microsoft.VisualBasic.Right(ListBox1.Items.Item(i9).ToString, ListBox1.Items.Item(i9).ToString.Length - middle.Length)

                                            mstran = Microsoft.VisualBasic.Left(ListBox1.Items.Item(i9), middle.Length) 'bcode
                                            ar.Add(mstran)

                                            'Console.WriteLine("m:" & middle)
                                            'Console.WriteLine(mstran)

                                            For cc = 0 To ar.Count - 1
                                                If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort

                                                'Console.WriteLine("ar:" & ar(cc))
                                                If i91.Contains(ar(cc)) Then
                                                    infiniteLoop(middle)
                                                    Exit Sub
                                                End If
                                            Next

                                            If strandComplete = True Then
                                                strandComplete = False
                                            Else
                                                apisk(" " + i91)
                                            End If

                                            Exit For
                                        End If
                                    Next i9


                                    ar.Clear()
                                    mstran = Nothing
#End Region

                                End If
#End Region

                            End If
                            pd = j + 1 'continue loop
                            If j = f.Length Then Exit For 'max trys
                            GoTo loop1
                        End If


                    Next

                End If '//If aa = ("«") Then


#End Region

                main1 += 1 'main count reached , exit
                If main1 > f.Length + 1 Then
                    MsgBox("syntax error in string", vbInformation)
                    txtString.SelectionStart = pd - 1 'place cursor
                    txtString.Select()
                    Exit For
                End If '


                If GetAsyncKeyState(Keys.Pause) Then
                    keyRelease(Keys.Pause)
                    emode()
                    Exit Sub 'abort
                End If

                If aa = "«" Or aa = "»" Then GoTo runagain 'more internals found

                If pd = f.Length + 1 Then Exit For 'reached max try's, abort

#Region "skmode"
                Dim skmode As Boolean = False
                If Microsoft.VisualBasic.Left(txtString.Text, 1) = "1" Then skmode = True 'sendkeys mode
                If skmode = True Then
                    If aa = "+" Then aa = "+=" 'convert to raw
                    If aa = "^" Then aa = "+6"
                    If aa = "{" Then aa = "+["
                    If aa = "}" Then aa = "+]"
                    If aa = "(" Then aa = "+9"
                    If aa = ")" Then aa = "+0"
                    If aa = "%" Then aa = "+5"
                    If aa = "^" Then aa = "+6"
                    If aa = "&" Then aa = "+7"
                    If aa = "#" Then aa = "+3"
                    If aa = "~" Then aa = "+`"
                    If Microsoft.VisualBasic.Left(txtString.Text, 1) = "0" Then If aa = Chr(13) Then aa = "" 'remove newline (internal)
                    SendKeys.Send(aa) 'individual string/s 

                Else
                    '>sendkeys
                    skmode = False

                    Dim shiftKey As Boolean = False 'start setup char for press char . single press
                    Dim shiftKey_ As Boolean = False 'setup shift hold
                    Dim shiftKey_c As Boolean = False
                    Dim ctrlKey_ As Boolean = False
                    Dim ctrlKey_c As Boolean = False
                    Dim altKey_ As Boolean = False
                    Dim altKey_c As Boolean = False
                    Dim winKey_ As Boolean = False
                    Dim winKey_c As Boolean = False

#Region "c"
                    Select Case aa
                        Case "‹" 'wsp ‹›
                            ignoreWhiteSpace_g = True
                            aa = "ws"
                        Case "›" '-wsp
                            ignoreWhiteSpace_g = False
                            aa = "ws"
                        Case vbLf
                            If ignoreWhiteSpace_g = True Then aa = "ws" Else aa = Keys.Enter  'enter
                        Case vbTab
                            If ignoreWhiteSpace_g = True Then aa = "ws" Else aa = "tab"  'tab, tab press below
                        Case " "
                            If ignoreWhiteSpace_g = True Then aa = "ws" Else aa = Keys.Space   'space
                        Case "º"
                            aa = Keys.Delete
                        Case "`"
                            aa = Keys.Oem3 '`key
                        Case "?"  '?
                            shiftKey = True
                            aa = Keys.OemQuestion
                        Case ">"  '>
                            shiftKey = True
                            aa = Keys.OemPeriod
                        Case "<"
                            shiftKey = True
                            aa = Keys.Oemcomma
                        Case """"
                            shiftKey = True
                            aa = Keys.OemQuotes
                        Case ":"
                            shiftKey = True
                            aa = Keys.OemSemicolon
                        Case "|"
                            aa = Keys.OemBackslash
                            shiftKey = True
                        Case "}"
                            shiftKey = True
                            aa = Keys.OemCloseBrackets
                        Case "{"
                            shiftKey = True
                            aa = Keys.OemOpenBrackets
                        Case "+"
                            aa = Keys.Oemplus
                            shiftKey = True
                        Case "_"
                            aa = Keys.OemMinus
                            shiftKey = True
                        Case ")"
                            aa = Keys.D0
                            shiftKey = True
                        Case "("
                            aa = Keys.D9
                            shiftKey = True
                        Case "*"
                            aa = Keys.D8
                            shiftKey = True
                        Case "&"
                            aa = Keys.D7
                            shiftKey = True
                        Case "^"
                            aa = Keys.D6
                            shiftKey = True
                        Case "%"
                            aa = Keys.D5
                            shiftKey = True
                        Case "$"
                            aa = Keys.D4
                            shiftKey = True
                        Case "#"
                            aa = Keys.D3
                            shiftKey = True
                        Case "@"
                            aa = Keys.D2
                            shiftKey = True
                        Case "!"
                            aa = Keys.D1
                            shiftKey = True
                        Case "~"
                            aa = Keys.Oem3
                            shiftKey = True
                        Case "-"
                            aa = Keys.OemMinus
                        Case "="
                            aa = Keys.Oemplus
                        Case "]"
                            aa = Keys.OemCloseBrackets
                        Case "["
                            aa = Keys.OemOpenBrackets
                        Case "\"
                            aa = Keys.OemBackslash
                        Case ";"
                            aa = Keys.OemSemicolon
                        Case "'"
                            aa = Keys.OemQuotes
                        Case ","
                            aa = Keys.Oemcomma
                        Case "."
                            aa = Keys.OemPeriod
                        Case "/"
                            aa = Keys.OemQuestion
                        Case "0"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad0 Else aa = Keys.D0
                        Case "1"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad1 Else aa = Keys.D1
                        Case "2"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad2 Else aa = Keys.D2
                        Case "3"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad3 Else aa = Keys.D3
                        Case "4"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad4 Else aa = Keys.D4
                        Case "5"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad5 Else aa = Keys.D5
                        Case "6"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad6 Else aa = Keys.D6
                        Case "7"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad7 Else aa = Keys.D7
                        Case "8"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad8 Else aa = Keys.D8
                        Case "9"
                            If My.Settings.SettingUseNumPad = True Then aa = Keys.NumPad9 Else aa = Keys.D9
                        Case "a"
                            aa = Keys.A '65
                        Case "b"
                            aa = Keys.B '66
                        Case "c"
                            aa = Keys.C '67
                        Case "d"
                            aa = Keys.D '68
                        Case "e"
                            aa = Keys.E
                        Case "f"
                            aa = Keys.F
                        Case "g"
                            aa = Keys.G
                        Case "h"
                            aa = Keys.H
                        Case "i"
                            aa = Keys.I
                        Case "j"
                            aa = Keys.J
                        Case "k"
                            aa = Keys.K
                        Case "l"
                            aa = Keys.L
                        Case "m"
                            aa = Keys.M
                        Case "n"
                            aa = Keys.N
                        Case "o"
                            aa = Keys.O
                        Case "p"
                            aa = Keys.P
                        Case "q"
                            aa = Keys.Q
                        Case "r"
                            aa = Keys.R
                        Case "s"
                            aa = Keys.S
                        Case "t"
                            aa = Keys.T
                        Case "u"
                            aa = Keys.U
                        Case "v"
                            aa = Keys.V
                        Case "w"
                            aa = Keys.W
                        Case "x"
                            aa = Keys.X
                        Case "y"
                            aa = Keys.Y
                        Case "z"
                            aa = Keys.Z
                        Case "A"
                            aa = Keys.A '65
                            shiftKey = True
                        Case "B"
                            aa = 66
                            shiftKey = True
                        Case "C"
                            aa = 67
                            shiftKey = True
                        Case "D"
                            aa = 68
                            shiftKey = True
                        Case "E"
                            aa = 69
                            shiftKey = True
                        Case "F"
                            aa = 70
                            shiftKey = True
                        Case "G"
                            aa = 71
                            shiftKey = True
                        Case "H"
                            aa = 72
                            shiftKey = True
                        Case "I"
                            aa = 73
                            shiftKey = True
                        Case "J"
                            aa = 74
                            shiftKey = True
                        Case "K"
                            aa = 75
                            shiftKey = True
                        Case "L"
                            aa = 76
                            shiftKey = True
                        Case "M"
                            aa = 77
                            shiftKey = True
                        Case "N"
                            aa = 78
                            shiftKey = True
                        Case "O"
                            aa = 79
                            shiftKey = True
                        Case "P"
                            aa = 80
                            shiftKey = True
                        Case "Q"
                            aa = 81
                            shiftKey = True
                        Case "R"
                            aa = 82
                            shiftKey = True
                        Case "S"
                            aa = 83
                            shiftKey = True
                        Case "T"
                            aa = 84
                            shiftKey = True
                        Case "U"
                            aa = 85
                            shiftKey = True
                        Case "V"
                            aa = 86
                            shiftKey = True
                        Case "W"
                            aa = 87
                            shiftKey = True
                        Case "X"
                            aa = 88
                            shiftKey = True
                        Case "Y"
                            aa = 89
                            shiftKey = True
                        Case "Z"
                            aa = 90
                            shiftKey = True
                        Case "¤"
                            aa = Keys.Back
                        Case "Ç"
                            aa = Keys.Escape
                        Case "Ӂ"  'Ӂ җ ӝ Ӝ - move mouse position 1px
                            SetCursorPos(MousePosition.X, MousePosition.Y - 1)
                            pd += 1
                            GoTo loop1
                        Case "җ"
                            SetCursorPos(MousePosition.X, MousePosition.Y + 1)
                            pd += 1
                            GoTo loop1
                        Case "ӝ"
                            SetCursorPos(MousePosition.X - 1, MousePosition.Y)
                            pd += 1
                            GoTo loop1
                        Case "Ӝ"
                            SetCursorPos(MousePosition.X + 1, MousePosition.Y)
                            pd += 1
                            GoTo loop1
                        Case "¾"
                            Me.Visible = True ' ¾ SHOW
                            pd += 1
                            GoTo loop1
                        Case "ð"
                            Me.Visible = False ' ð  HIDE
                            pd += 1
                            GoTo loop1
                        Case "¿"
                            aa = SetCursorPos(xx1, yy1) '¿ return mouse
                        Case "§"
                            leftclick()
                            pd += 1
                            GoTo loop1
                        Case "¦"
                            rightclick()
                            pd += 1
                            GoTo loop1
                        Case "¶"
                            middleclick()
                            pd += 1
                            GoTo loop1
                        Case "¡"
                            rightrelease()
                            pd += 1
                            GoTo loop1
                        Case "¢"
                            leftrelease()
                            pd += 1
                            GoTo loop1
                        Case "Ÿ"
                            lefthold()
                            pd += 1
                            GoTo loop1
                        Case "ž"
                            righthold()
                            pd += 1
                            GoTo loop1
                        Case "Í"
                            aa = Keys.F1
                        Case "Â"
                            aa = Keys.F2
                        Case "Ã"
                            aa = Keys.F3
                        Case "Ð"
                            aa = Keys.F4
                        Case "Ï"
                            aa = Keys.F5
                        Case "Æ"
                            aa = Keys.F6
                        Case "Î"
                            aa = Keys.F7
                        Case "È"
                            aa = Keys.F8
                        Case "É"
                            aa = Keys.F9
                        Case "Ê"
                            aa = Keys.F10
                        Case "Ë"
                            aa = Keys.F11
                        Case "Ì"
                            aa = Keys.F12
                        Case "û"
                            aa = 93 'pressMNU 93
                        Case "¯"
                            aa = Keys.PrintScreen ' 93
                        Case "ú"
                            aa = Keys.Pause '3
                        Case "Ü"
                            aa = Keys.PageUp '33
                        Case "Ý"
                            aa = Keys.PageDown '34
                        Case "ÿ"
                            aa = Keys.End '35
                        Case "þ"
                            aa = Keys.Home '36
                        Case "ý"
                            aa = Keys.Insert '19
                        Case "·"
                            aa = Keys.Tab
                        Case "¬"
                            aa = Keys.Enter
                        Case "€"
                            aa = Keys.Up '38 
                        Case "ƒ"
                            aa = Keys.Down '40 
                        Case "‡"
                            aa = Keys.Left '37 
                        Case "†"
                            aa = Keys.Right '39 
                        Case "˜"
                            aa = Keys.VolumeMute
                        Case "ˆ"
                            aa = Keys.VolumeUp
                        Case "Ž"
                            aa = Keys.VolumeDown
                        Case "°"
                            shiftKey_ = True 'aa = Keys.LShiftKey ' pressShift '16 °
                        Case "ø"
                            shiftKey_c = True 'aa = Keys.RShiftKey 'pressShift_r ø
                        Case "•"
                            ctrlKey_ = True 'aa = Keys.LControlKey ' pressCtrl '17•
                        Case "Þ"
                            ctrlKey_c = True 'aa = Keys. ' pressCtrl_r '17 Þ
                        Case "¹"
                            altKey_ = True 'aa = Keys.Alt ' pressAlt_r '¹  
                        Case "ª"
                            altKey_c = True 'aa = ' pressAlt  'ª
                        Case "ù"
                            winKey_ = True 'press win
                        Case "Ù"
                            winKey_c = True 'press win
                    End Select
#End Region

                    If shiftKey_ = True Or shiftKey_c = True Then 'long shift hold
                        If shiftKey_c = True Then GoTo finish
                        keybd_event(Keys.LShiftKey, 0, 1, 0)
                        GoTo finish
                    End If
                    If ctrlKey_ = True Or ctrlKey_c = True Then 'long ctrl hold
                        If ctrlKey_c = True Then GoTo finish
                        keybd_event(Keys.LControlKey, 0, 0, 0)
                        GoTo finish
                    End If
                    If altKey_ = True Or altKey_c = True Then 'long alt hold
                        If altKey_c = True Then GoTo finish 'fixed 9.21.13 alt button 
                        keybd_event(Keys.LMenu, 0, 0, 0)
                        GoTo finish
                    End If
                    If winKey_ = True Or winKey_c = True Then 'long win hold
                        If winKey_c = True Then GoTo finish
                        keybd_event(Keys.LWin, 0, 1, 0)
                        GoTo finish
                    End If


                    'master press (char)
                    If shiftKey = True Then keybd_event(Keys.LShiftKey, 0, 0, 0) 'hold shift
                    If aa = "tab" Then
                        keybd_event(Keys.Tab, 0, 0, 0) 'press tab
                        keybd_event(Keys.Tab, 0, &H2, 0)
                    Else

                        If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort

                        Try
                            If aa = "ws" Then 'white space
                                aa = ""
                            Else
                                keybd_event(aa, 0, 0, 0)  'press char 
                                keybd_event(aa, 0, 2, 0)
                            End If

                        Catch ex As Exception
                            Dim vy = MsgBox("Can not process """ + aa + """ character" + vbNewLine + vbNewLine + "solution: «:" + aa + "»", vbYesNo, "make change in editor?")
                            For ef = 1 To txtString.TextLength  'err position
                                If GetChar(txtString.Text, ef) = aa And GetChar(txtString.Text & " ", ef + 1) = "»" Then Continue For 'skip to next

                                If Microsoft.VisualBasic.Mid(txtString.Text, ef, 1) = aa Then 'err char
                                    txtString.Focus()
                                    txtString.SelectionStart = ef - 1 'select text
                                    txtString.SelectionLength = 1

                                    If vy = MsgBoxResult.Yes Then 'implement
                                        Me.Show()
                                        showTab(3)
                                        timeout2(60)
                                        SendKeys.Send("«:" + aa + "»")

                                    End If

                                    Exit For
                                End If
                            Next


                            emode()
                            Exit Sub

                        End Try



                    End If
                    If shiftKey = True Then keybd_event(Keys.LShiftKey, 0, &H2, 0) 'release shift
                    shiftKey = False 'clear 


finish:
                    If shiftKey_c = True Then 'long shift hold release
                        keybd_event(Keys.LShiftKey, 0, &H2, 0)
                        keybd_event(Keys.RShiftKey, 0, &H2, 0)
                        keybd_event(Keys.ShiftKey, 0, &H2, 0)
                        shiftKey_ = False
                        shiftKey_c = False
                    End If
                    If ctrlKey_c = True Then 'long ctrl hold release 'moved to clear
                        keybd_event(Keys.LControlKey, 0, &H2, 0)
                        keybd_event(Keys.RControlKey, 0, &H2, 0)
                        keybd_event(Keys.Control, 0, &H2, 0)
                        ctrlKey_ = False
                        ctrlKey_c = False
                    End If
                    If altKey_c = True Then 'long alt hold release
                        keybd_event(Keys.LMenu, 0, &H2, 0)
                        altKey_ = False
                        altKey_c = False
                    End If
                    If winKey_c = True Then 'long win hold release
                        keybd_event(Keys.LWin, 0, &H2, 0)
                        keybd_event(Keys.RWin, 0, &H2, 0)
                        winKey_ = False
                        winKey_c = False
                    End If


                End If
#End Region
            Next pd

            finished = True

        End If 'master

    End Sub

    Sub emode()
        If RightCtrllToolStripMenuItem.Checked = True And My.Settings.SettingRctrleqMod = "»" Then 'dna > after run clear or lock >> 
            If My.Settings.SettingAutoLockEmode = True Then TextBox1.Text = "»"
        Else
            TextBox1.Clear() 'reset and clear
        End If
        dnaTxt()
    End Sub

    Dim gi As Integer
    Dim connect As Boolean = False
    Dim strandComplete As Boolean = False
    Dim finished As Boolean = False

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text = "'" Or TextBox1.Text = "" Then Exit Sub

        If finished Then
            finished = False
            Exit Sub
        End If

        If TextBox1.Text = "«" Then Exit Sub
        If TextBox1.Text.Contains("»") And TextBox1.Text.Length >= 1 Then
            TextBox1.Text = "»"
            dnaTxt()
            Exit Sub
        End If
        If TextBox1.Text = "»" Then Exit Sub

        If Not TextBox1.Text.StartsWith("«") And NoLengthToolStripMenuItem.Checked = True And Not TextBox1.Text.Contains("Ƥ") Then Exit Sub 'do nothing  'v3 'only ps

        If TextBox1.Text.IndexOf("«", 0) = 0 Then 'v2
        Else
            If TextBox1.Text.Length < Val(txtLength.Text) Then Exit Sub 'do nothing 
            If TextBox1.Text.Length + 1 < Val(txtLength.Text) Then Exit Sub 'do nothing 
            If TextBox1.Text = "" Then Exit Sub
        End If

        Dim cl = txtLength.Text

        Dim code As String, i As Integer, h As Integer, f As String 'Me.ListBox1.Items.Item(i)

        On Error GoTo test
        For i = 0 To ListBox1.Items.Count - 1 'scan db 

            If i <= -1 Or i >= ListBox1.Items.Count Then
                TextBox1.Text = Microsoft.VisualBasic.Right(TextBox1.Text, Val(txtLength.Text))
                finished = True
                Exit Sub '< or > item count  
            End If
            On Error GoTo test
            code = Microsoft.VisualBasic.Left(ListBox1.Items.Item(i), Val(txtLength.Text)) 'My.Settings.SettingTxtCodeLengthcode 'v1
            If TextBox1.Text.IndexOf("«", 0) = 0 Then 'v2
                code = Microsoft.VisualBasic.Left(ListBox1.Items.Item(i), TextBox1.TextLength) 'code 'v2
                cl = TextBox1.TextLength
            End If

            If ToolStripMenuItemChkMiscFs.Checked = False And chkMisc.Checked = False And Microsoft.VisualBasic.Left(code, 2) = "//" Then Continue For 'skips
            If ToolStripMenuItemChkMiscRem.Checked = False And chkMisc.Checked = False And Microsoft.VisualBasic.Left(code, 1) = "'" Then Continue For
            If ToolStripMenuItemChkMiscFs.Checked = False And chkMisc.Checked = False And Microsoft.VisualBasic.Right(code, 2) = "//" Then Continue For

            If Microsoft.VisualBasic.Right(TextBox1.Text, cl) = code And Len(TextBox1.Text) >= cl Then 'scan code
                h = Len(ListBox1.Items.Item(i)) - cl 'code len
                f = Microsoft.VisualBasic.Right(ListBox1.Items.Item(i), h) '_string + tab 'reg
                On Error GoTo sk
                gi = i

                'v2 & auto bs*# (v2-) auto-
                If f.IndexOf("»", 0) = 0 Or f.IndexOf("-", 0) = 0 And f.IndexOf("»", 0) = 1 Then 'everything after Or = auto bs / v2-

                    If f.IndexOf("-", 0) = 0 Then 'v2- 
                        h -= 2 'change f.str to new .str («new-»«bs*3»f)
                        f = Microsoft.VisualBasic.Right(ListBox1.Items.Item(i), h) 'not connectd ex. <p1->1

                        'filter 
                        code = code.Replace("į", "").Replace(".", "").Replace("Ƥ", "").Replace("C", "").Replace("S", "").Replace("H", "").Replace("A", "").Replace("Ą", "").Replace("M", "").Replace("!", "").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("*", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace("=", "").Replace("ė", "").Replace("Ė", "").Replace("P", "")

                        f = "»" & "«bs*" & Len(code) - 1 & "»" & f
                    End If

                    ori2 = code
                    finished = True
                    Dim cb = Clipboard.GetText
                    apisk(f) 'aftermarket skeys 'v2 <<run>>
                    If Not Clipboard.GetText = cb Then If g_remcb Then Clipboard.SetText(cb)
                Else
                    If TextBox1.Text.IndexOf("«", 0) = 0 Then Continue For
                    If ListBox1.Items.Item(i).ToString.StartsWith("http") Then Exit Sub
                    If GetAsyncKeyState(Keys.Escape) Then keyRelease(Keys.Escape) '32bit

                    finished = True
                    Dim cb = Clipboard.GetText
                    apisk(f) 'aftermarket skeys 'reg / v1
                    If Not Clipboard.GetText = cb Then If g_remcb Then Clipboard.SetText(cb)
                End If

                gi = Nothing

                If f.Contains("<cb") Then 'select item
                    selectBottomItem()
                Else
                    ListBox1.SelectedItem() = ListBox1.Items.Item(i) 'select code
                End If

                If f.Contains("«io:") Then 'v2
                Else
                    emode()
                End If

                clearAllKeys()
                Exit For
            End If
        Next i



        Exit Sub
test:
        emode()
        Exit Sub
sk:
        If Err.Number = 5 And f.Contains("«audio:") Or Err.Number = 5 And f.Contains("«Audio:") Then
            emode()
            Exit Sub
        End If

        If Err.Number = 5 Then
            ListBox1.SelectedItem() = ListBox1.Items.Item(i)
            TextBox1.Text = "" 'reset and clear
            clearAllKeys() '
            emode()
            Exit Sub ' length error 5
        End If
        MsgBox(Err.Description, vbInformation, "error")
        TextBox1.Text = ""
        clearAllKeys()
        Exit Sub

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub txtLength_DoubleClick(sender As Object, e As EventArgs) Handles txtLength.DoubleClick
        If chk_tips.Checked = False Then Exit Sub
        LengthToolStripMenuItem.PerformClick()
    End Sub

    Private Sub txtLength_TextChanged(sender As Object, e As EventArgs) Handles txtLength.TextChanged
        If IsNumeric(txtLength.Text) Then
            Dim t As Integer = txtLength.Text
            If t > 0 Then
                My.Settings.SettingTxtCodeLength = txtLength.Text 'set len
                reloadDb() 'reload
            Else
                txtLength.Text = 4 'default
                reloadDb() 'reload
            End If
        Else
            txtLength.Text = 4 'default
            reloadDb() 'reload
        End If
        TabControl1.Focus()
    End Sub

    Sub deleteDbItmAll()
        If DeleteAllToolStripMenuItem.Text = "reload" Then
            reloadDb()
            Exit Sub
        End If
        If ListBox1.Items.Count = 0 Then Exit Sub
        Dim msg = MsgBox(ListBox1.Items.Count, vbYesNo, "delete all items?")
        If msg = vbYes Then
            'backup option
            Dim q = MsgBox("make a backup before deleting?", vbYesNo)
            If q = MsgBoxResult.Yes Then exportListToTxt1()
            My.Settings.Settingdb = Nothing 'delete all items 
            My.Settings.Settingdb = New Specialized.StringCollection 'create new setting
            ListBox1.Items.Clear()
            txtStringClear()
        End If
    End Sub

    Sub deleteDbItm()
        If chk_tips.Checked = True And ListBox1.SelectedIndex < 0 Then
            If ListBox1.Items.Count = 0 Then
                MsgBox("must add something first", vbInformation)
                Exit Sub
            End If

            MsgBox("nothing selected to delete", vbInformation)
            ListBox1.Focus()
            Exit Sub
        End If

        If ListBox1.SelectedIndex < 0 Then Exit Sub 'nothing slected

        Dim xs = ListBox1.SelectedIndex - 1 '0
        If xs = -1 Then
            GoTo reg
        End If

reg:
        Dim msg As Integer, dbR As Integer  'remove  item from db
        dbR = ListBox1.SelectedIndex ' #

        If tipsDeleteToolStripMenuItem2.CheckState = CheckState.Checked Then msg = MsgBox("[" & Microsoft.VisualBasic.Left(ListBox1.SelectedItem.ToString, Val(txtLength.Text)) & "]", vbYesNo, "delete item?") 'show msgbox before delete
        If tipsDeleteToolStripMenuItem2.CheckState = CheckState.Unchecked Then msg = vbYes 'delete without msgbox

        If msg = vbYes Then
            ListBox1.Items.RemoveAt(dbR) 'delete item
            If DeleteAllToolStripMenuItem.Text = "reload" Then
            Else
                My.Settings.Settingdb.RemoveAt(dbR) 'update settings 
            End If
        End If

        If tipsDeleteToolStripMenuItem2.CheckState = CheckState.Unchecked And ListBox1.Items.Count > 0 And (dbR - 1) >= 0 Then ListBox1.SelectedItem() = ListBox1.Items.Item(dbR - 1) 'select next item
    End Sub

    Sub clearTxtString()
        keyClear(Keys.LControlKey)
        txtString.SelectAll()   'clear
        txtString.Focus()
        If GetAsyncKeyState(Keys.LControlKey) Then '
            keyClear(Keys.LControlKey)
            txtStringClear() '
            Exit Sub '
        End If
        key(Keys.Back)
    End Sub

    Sub showDb()
        If SplitContainer1.Height - SplitContainer1.SplitterDistance <= 40 Then SplitContainer1.SplitterDistance = SplitContainer1.Height / 2 'show if can't see
    End Sub

    Sub editDbItm()
        sp = False 'stop auto size

        Dim adv = 0
        'show db
        If SplitContainer1.Height >= 20 And SplitContainer1.SplitterDistance <= 14 Then SplitContainer1.SplitterDistance = 28 'SplitContainer1.Height / 2
        If SplitContainer1.Height - SplitContainer1.SplitterDistance <= 40 Then SplitContainer1.SplitterDistance = SplitContainer1.Height / 2 'show if can't see

        If chk_tips.Checked = True And ListBox1.SelectedIndex < 0 Then
            If ListBox1.Items.Count = 0 Then
                MsgBox("must add something first", vbInformation)
                Exit Sub
            End If
            MsgBox("nothing selected to edit", vbInformation)
            ListBox1.Focus()
            Exit Sub
        End If

        If ListBox1.SelectedIndex < 0 Then Exit Sub 'nothing slected

        Dim rz = txtString.ZoomFactor  're-zoom

        Dim msg As String, dbR As Integer 'edit  item in db adv As String  
        dbR = ListBox1.SelectedIndex ' #

        If chk_tips.Checked = True Then
            If chk_top.Checked = True Then chk_top.Checked = False 'no on top so input box can be in front
            msg = InputBox("edit item?", "edit", ListBox1.SelectedItem)

        Else
            If Not txtString.Text > "" Then ' make txtstr then exit 


                txtString.Focus()

                txtString.AppendText(ListBox1.SelectedItem.ToString)

                txtString.SelectionStart = txtString.TextLength
                Exit Sub
            End If
            msg = txtString.Text


            For j = 1 To Val(txtLength.Text) 'add without keyboard key codes
                If txtString.TextLength > j And txtString.Text > "" Then
                    If GetChar(txtString.Text, j) = "«" Then
                        GoTo noformat
                        Exit Sub
                    End If
                End If
            Next


        End If

        If msg > "" Then

            If txtString.Text > "" And chk_tips.Checked = False Then 'edit update msgbox 
                If Microsoft.VisualBasic.Left(msg, 2) = "//" Or Microsoft.VisualBasic.Left(msg, 1) = "'" Or Microsoft.VisualBasic.Right(msg, 2) = "//" Or msg.StartsWith("http") Then GoTo noformat

                Dim n1 As String = txtString.Text
                If n1.Length < My.Settings.SettingTxtCodeLength + 1 Then GoTo noformat ' < 
                If Not GetChar(n1, My.Settings.SettingTxtCodeLength + 1) = Chr(9) Then n1 = Microsoft.VisualBasic.Left(n1, My.Settings.SettingTxtCodeLength) & Chr(9) & Microsoft.VisualBasic.Right(n1, n1.Length - My.Settings.SettingTxtCodeLength) 'if missing tab, reinsert

                adv = MsgBox("update:" & Chr(13) & "old:" & vbTab & ListBox1.SelectedItem & Chr(13) & "new:" & vbTab & n1, vbYesNo, "edit")
                If adv = vbNo Then Exit Sub

            End If



            If Microsoft.VisualBasic.Left(msg, 1) = "«" Or Microsoft.VisualBasic.Left(msg, 2) = "//" Or Microsoft.VisualBasic.Left(msg, 1) = "'" Or Microsoft.VisualBasic.Right(msg, 2) = "//" Then GoTo noformat

            ''
            If msg.Length < (Val(txtLength.Text) + 1) Then 'if good to format

            Else

                If GetChar(msg, Val(txtLength.Text) + 1) = Chr(9) Then 'if tab, skip to noformat
                Else
                    msg = Microsoft.VisualBasic.Left(msg, My.Settings.SettingTxtCodeLength) & Chr(9) & Microsoft.VisualBasic.Right(msg, msg.Length - My.Settings.SettingTxtCodeLength) 'if missing tab, reinsert
                End If

            End If
            ''

noformat:

            If adv <= 0 And chk_tips.Checked = False Then adv = MsgBox("update:" & Chr(13) & "old:" & vbTab & ListBox1.SelectedItem & Chr(13) & "new:" & vbTab & txtString.Text, vbYesNo, "edit")
            If adv = vbNo Then Exit Sub

            txtString.Text = ListBox1.Text 'ctrl z bkup

            If ListBox1.Items.Count <> My.Settings.Settingdb.Count Or DeleteAllToolStripMenuItem.Text = "reload" Then 'in clear mode / db was cleared
                ListBox1.Items.RemoveAt(dbR) 'temp update
                ListBox1.Items.Insert(dbR, msg)
            Else
                ListBox1.Items.RemoveAt(dbR) 'master update
                My.Settings.Settingdb.RemoveAt(dbR)
                ListBox1.Items.Insert(dbR, msg)
                My.Settings.Settingdb.Insert(dbR, msg)
            End If

            ListBox1.SelectedItem() = ListBox1.Items.Item(dbR) 'select item

            txtString.SelectAll()
            txtString.SelectedText = ""

        End If

    End Sub

    Sub addDbItm()
        If txtString.Text.Contains("«dna»") Then '«dna» , add to index 0 
            ListBox1.Items.Insert(0, txtString.Text) 'insert / place
            My.Settings.Settingdb.Insert(0, txtString.Text) 'save in db
            clearTxtString()
            Exit Sub
        End If

        Dim db As String, msg As String  'add new item to db
        If chk_tips.Checked = True And txtString.Text = "" Then
            MsgBox("nothing to add from text box" & vbNewLine & vbNewLine & "example: test123", vbInformation)
            txtString.Focus()
            Exit Sub
        End If

        If ListBox1.Items.Count <> My.Settings.Settingdb.Count Or DeleteAllToolStripMenuItem.Text = "reload" Then 'in clear mode / db was cleared
            If txtString.Text = "" Then Exit Sub

            'add item with chr(9)
            If txtString.Text.StartsWith("«") = False And txtString.Text.StartsWith("//") = False And txtString.Text.StartsWith("'") = False And txtString.Text.StartsWith("//") = False And txtString.Text.StartsWith("http") = False Then
                If txtString.Text.Length <= My.Settings.SettingTxtCodeLength Then
                    ListBox1.Items.Add(txtString.Text) 'temp add
                Else
                    ListBox1.Items.Add(Microsoft.VisualBasic.Left(txtString.Text, My.Settings.SettingTxtCodeLength) & Chr(9) & Microsoft.VisualBasic.Right(txtString.Text, txtString.Text.Length - My.Settings.SettingTxtCodeLength)) 'if missing tab, reinsert
                End If
            Else
                ListBox1.Items.Add(txtString.Text) 'temp add
            End If

            txtStringClear()
            selectBottomItem()
            Exit Sub
        End If


        If chk_tips.Checked = True Then
            If chk_top.Checked = True Then chk_top.Checked = False
            msg = "add a new dna > '" + txtLength.Text + " button/key code pattern shortcut trigger' followed by a message/«algorithm» to database?" & vbNewLine & vbNewLine & "example: test«ctrl»a«-ctrl»123" & vbNewLine & vbNewLine & "tip: right click text box and pick from «algorithm» menu, then ctrl + s to add/save new item to database." & vbNewLine & vbNewLine
            db = InputBox(msg, "db", txtString.Text)
        Else
            db = txtString.Text
        End If


        For j = 1 To Val(txtLength.Text) 'add without keyboard key codes
            If txtString.TextLength > j And txtString.Text > "" Then
                If GetChar(txtString.Text, j) = "«" Then
                    GoTo noformat
                    Exit Sub
                End If
            End If
        Next


        ' place / insert
        If Microsoft.VisualBasic.Mid(db, My.Settings.SettingTxtCodeLength + 1, 3) = "«//" Or Microsoft.VisualBasic.Mid(db, My.Settings.SettingTxtCodeLength + 1, 4) = Chr(9) & "«//" Then 'find «//x» or tab«//x»
            Dim beg As Integer = My.Settings.SettingTxtCodeLength + 1 + 3 'code + // + »» 'dim len with no tab
            If GetChar(txtString.Text, My.Settings.SettingTxtCodeLength + 1) = Chr(9) Then beg = My.Settings.SettingTxtCodeLength + 1 + 4 'dim with tab
            Dim txt = ""
            For i = (My.Settings.SettingTxtCodeLength + 1 + 3) To Len(db)
                If Microsoft.VisualBasic.Mid(db, i, 1) = "»" Then ' find till » 
                    Dim mid = Microsoft.VisualBasic.Mid(db, i - (i - beg), i - beg) '//  mid
                    For it = 0 To ListBox1.Items.Count - 1
                        If Microsoft.VisualBasic.Right(ListBox1.Items.Item(it), i - beg + 2) = "//" + mid Then '//+mid, found //add in db, (it + 1) = len(//xx)
                            Dim code = (Microsoft.VisualBasic.Left(txtString.Text, My.Settings.SettingTxtCodeLength))
                            Dim message = (Microsoft.VisualBasic.Mid(txtString.Text, Len(code) + Len(mid) + 6, Len(txtString.Text) - Len(code) - Len(mid) - 4))
                            ListBox1.Items.Insert(it + 1, code & Chr(9) & message) 'insert / place
                            My.Settings.Settingdb.Insert(it + 1, code & Chr(9) & message) 'save in db
                            txt = txtString.Text
                            txtStringClear()
                            ListBox1.SelectedIndex = it + 1
                            Exit For
                        End If
                        If it = ListBox1.Items.Count - 1 Then MsgBox("error: no //" & mid & " found in db", vbInformation)
                    Next
                    Exit For
                End If
            Next
            Exit Sub
        End If '//place

        If db > "" Then 'add item
            'skip format 
            If Microsoft.VisualBasic.Left(db, 2) = "//" Or Microsoft.VisualBasic.Left(db, 1) = "'" Or Microsoft.VisualBasic.Right(db, 2) = "//" Or db.StartsWith("http") Then GoTo noformat

            'add item with chr(9)
            If Not Microsoft.VisualBasic.Mid(db, My.Settings.SettingTxtCodeLength + 1, 1) = Chr(9) And db.Length > My.Settings.SettingTxtCodeLength Then db = Microsoft.VisualBasic.Left(db, My.Settings.SettingTxtCodeLength) & Chr(9) & Microsoft.VisualBasic.Right(db, db.Length - My.Settings.SettingTxtCodeLength) 'if missing tab, reinsert

noformat:
            ListBox1.Items.Add(db) 'print items to listbox
            My.Settings.Settingdb.Add(db) 'save items to settings
            'txtString.Text = "" 'clear
            txtString.SelectAll()
            txtString.SelectedText = ""
            ListBox1.SelectedItem() = ListBox1.Items.Item(ListBox1.Items.Count - 1) 'select 1st item
        End If


        If txtString.Text = "" Then showDb() : txtString.Focus()

        My.Settings.SettingLastListIndex = ListBox1.SelectedIndex

    End Sub

    Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click
        'Me.Text = "e"
        editDbItm()
        txtString.Font = ListBox1.Font 'color refresh
    End Sub


    Sub printShortMenu(s As String, moveleft As Boolean, moveleftagain As Boolean)
        txtString.Focus()
        print(s, False)
        print(s, True)
        If moveleft = True Then key(Keys.Left) : keyClear(Keys.Left)
        If moveleftagain = True Then key(Keys.Left) : keyClear(Keys.Left)
        NullToolStripMenuItem1.Text = s ' quick repeat
        If moveleft = True Then NullToolStripMenuItem1.Text += "{left}"
        If moveleftagain = True Then NullToolStripMenuItem1.Text += "{left}"

    End Sub


    Sub skMenuGet(s As String) 'contextmenustring
        TextBox1.Text = "'" 'safe to print
        txtString.Focus()
        SendKeys.Send(s) 'print short string
        NullToolStripMenuItem1.Text = s ' quick repeat
        clearAllKeys()
        emode()
    End Sub

    Sub skMenuGet1(s As String, ls As String, lsc As String) 'contextmenustring   's short, ls long string, lsc close
        TextBox1.Text = "'" 'safe to print
        txtString.Focus()

        If LongTagsToolStripMenuItem.Checked = True Then

            If My.Settings.SettingChkW7 = True Then 'w7 fix
                print("«", False) 'print 1 tag
                w7(33)
                print(ls, False) 'print 1 tag
                w7(33)
                print("»", False) 'print 1 tag
                w7(33)
            Else
                print("«" & ls & "»", False) 'print 1 tag
            End If


            Dim ii As Integer = Len(ls) + 3 'move left length

            If lsc > "" Then
                If My.Settings.SettingChkW7 = True Then 'w7 fix
                    print("«-", False) 'print 1 tag
                    w7(33)
                    print(ls, False) 'print 1 tag
                    w7(33)
                    print("»", False) 'print 1 tag
                    w7(33)
                Else
                    print("«-" & ls & "»", False) 'print long string
                End If


                For i = 1 To ii 'move left 
                    key(Keys.Left)
                Next
            End If

            If lsc > "" Then
                ls = "«" & ls & "»" & "«-" & ls & "»" 'store quick repeat long
                NullToolStripMenuItem1.Text = ls & "{left " & ii & "}" ' quick repeat long
                emode()
                Exit Sub
            End If

            ls = "«" & ls & "»" 'store quick repeat short
            NullToolStripMenuItem1.Text = ls ' quick repeat short
        Else
            'print(s)
            SendKeys.Send(s) 'print short string
            NullToolStripMenuItem1.Text = s ' quick repeat
        End If

        clearAllKeys()
        emode()
    End Sub

    Private Sub LeftClickToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LeftClickToolStripMenuItem.Click
        skMenuGet1("§", "left-click", "")
    End Sub

    Private Sub ReturnMouseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReturnMouseToolStripMenuItem.Click
        skMenuGet1("¿", "return-mouse", "")
    End Sub

    Sub mnuItemsShow(v As Boolean)
        'in clear mode temp  del all -> reload
        If ListBox1.Items.Count <> My.Settings.Settingdb.Count Then
            DeleteAllToolStripMenuItem.Text = "reload"
        Else
            DeleteAllToolStripMenuItem.Text = "delete all"
        End If

        SpacerToolStripMenuItem.Text = "spacer: " & My.Settings.SettingSpacer

        TabletToolStripMenuItem.Visible = v
        ClickSwipeToolStripMenuItem.Visible = v
        ToolStripMenuItem2.Visible = False
        ToolStripMenuItem2.Visible = v
        AddToolStripMenuItem.Visible = True
        EditToolStripMenuItem.Visible = True
        DeleteToolStripMenuItem.Visible = True
        ImportToolStripMenuItem.Visible = v
        ExportToolStripMenuItem.Visible = v
        DeleteAllToolStripMenuItem.Visible = v
        LongTagsToolStripMenuItem.Visible = v
        OptionsToolStripMenuItem.Visible = v
        CopyToolStripMenuItem1.Visible = v

        ClickSwipeToolStripMenuItem.Visible = My.Settings.SettingSwMnu 'swipe mnu
        LongTagsToolStripMenuItem.Visible = My.Settings.SettingAlgMnu  'algorithm
    End Sub

    Private Sub SplitContainer1_DoubleClick(sender As Object, e As EventArgs) Handles SplitContainer1.DoubleClick
        resizeSplitter()
    End Sub

    Private Sub SplitContainer1_MouseDown(sender As Object, e As MouseEventArgs) Handles SplitContainer1.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right And GetAsyncKeyState(Keys.LMenu) Or MouseButtons = Windows.Forms.MouseButtons.Right And GetAsyncKeyState(Keys.RMenu) Then 'adjust splitter.width
            SplitContainer1.SplitterWidth += 1
            If SplitContainer1.SplitterWidth >= 30 Then SplitContainer1.SplitterWidth = 1
            My.Settings.SettingSplitterWidth = SplitContainer1.SplitterWidth
            Exit Sub
        End If
        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            'select top or bottom item
            If ListBox1.SelectedIndex = ListBox1.Items.Count - 1 Then
                selectTopItem()
            Else
                selectBottomItem()
            End If
        End If
    End Sub

    Private Sub SplitContainer1_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles SplitContainer1.SplitterMoved
        c = 0
        My.Settings.splitterDistance = SplitContainer1.SplitterDistance 'save to settings 
        Me.Refresh()
        ListBox1.Height = SplitContainer1.Panel1.Height
        If My.Settings.SettingMultiColumn = True And My.Settings.SettingViewMultiScrollBar = False Then ListBox1.Height = SplitContainer1.Panel1.Height + 33
    End Sub

    Sub resizeSplitter()
        sp = False
        SplitContainer1.SplitterDistance = Me.SplitContainer1.Height / 2 'TabControl1.Height / 2 - 18
    End Sub

    Sub reStack()
        txtLength.Left = lblLength.Left + lblLength.Width
        lbl_timer1_interval_val.Left = lblLength.Left + Label1.Width

        ListBox1.Height = SplitContainer1.Panel1.Height
        txtString.Height = SplitContainer1.Panel2.Height

        If My.Settings.SettingTabAppearance = 2 Or My.Settings.SettingTabAppearance = 3 Or My.Settings.SettingTabAppearance = 5 Or My.Settings.SettingTabAppearance = 6 Or My.Settings.SettingTabAppearance = 2 Then
            txtString.Height = SplitContainer1.Panel2.Height - 3 'compinsate for tab style
        End If

        If My.Settings.SettingFontPass = False Then TabControl1.Left = 12
        If My.Settings.SettingFontPass = False Then TabControl1.Top = 12
        TabControl1.Height = Me.Height - 62

        Me.SplitContainer1.Top = 3

        Me.TabControl1.Width = Me.Width - 38 'size 0

        Me.SplitContainer1.Width = TabControl1.Width - 15 '10 '15
        Me.SplitContainer1.Height = TabControl1.Height - 34 '27 '31

        ListBox1.Top = 0
        txtString.Top = Me.SplitContainer1.Panel1.Top
        txtString.Width = Me.SplitContainer1.Width - 5
        ListBox1.Width = Me.SplitContainer1.Width - 5

        If My.Settings.SettingLstFontSize >= 15 And ListBox1.Font.Size >= 15 And My.Settings.SettingFontPass = True Then 'size 1
            TabControl1.Left = 20
            Me.TabControl1.Width = Me.Width - 60
            Me.TabControl1.Height = Me.Height - 80
            Me.SplitContainer1.Top = 5
            Me.SplitContainer1.Height = TabControl1.Height - 48
            Me.SplitContainer1.Width = TabControl1.Width - 19
            txtString.Left = ListBox1.Left
        End If
        If ListBox1.Font.Size = 15.75 And ListBox1.Font.Name = "Impact" Then
            Me.SplitContainer1.Top = 5
            Me.SplitContainer1.Height = TabControl1.Height - 48
            Me.SplitContainer1.Width = TabControl1.Width - 14
        End If
        If My.Settings.SettingLstFontSize >= 27 And ListBox1.Font.Size >= 27 And My.Settings.SettingFontPass = True Then 'size 2
            Me.TabControl1.Height = Me.Height - 120
            Me.TabControl1.Width = Me.Width - 100
            Me.SplitContainer1.Height = TabControl1.Height - 80
            Me.SplitContainer1.Width = TabControl1.Width - 29
            txtString.Left = ListBox1.Left
            ListBox1.Top = 0
        End If

        If TabControl1.Font.Size >= 72 And My.Settings.SettingFontPass = True Then 'size 2
            Me.TabControl1.Left = 10
            Me.TabControl1.Top = 10
            Me.TabControl1.Height = Me.Height - 60
            Me.TabControl1.Width = Me.Width - 35
            Me.SplitContainer1.Left = 5
            Me.SplitContainer1.Height = TabControl1.Height - 139 '70
            If TabControl1.Font.Size = 8.25 Then SplitContainer1.Height = TabControl1.Height - 130 '70
            Me.SplitContainer1.Width = TabControl1.Width - 20
            txtString.Left = ListBox1.Left
            ListBox1.Top = 0
            txtString.ForeColor = My.Settings.SettingForeColor
            txtString.Height = SplitContainer1.Panel2.Height '- 10
            SplitContainer1.SplitterWidth = My.Settings.SettingSplitterWidth
        End If

        'view > /toolstrip
        If SplitContainer1.BorderStyle = BorderStyle.None Then SplitContainer1.Left = 6 '5
        If SplitContainer1.BorderStyle = BorderStyle.FixedSingle Then SplitContainer1.Left = 5 '3

        ListBox1.Width = txtString.Width

        TabControl1.Refresh()
        If My.Settings.SettingFontPass = False Then ListBox1.Height = SplitContainer1.Panel1.Height
        If My.Settings.SettingViewMultiScrollBar = False And My.Settings.SettingMultiColumn = True Then ListBox1.Height = SplitContainer1.Panel1.Height + 33 'multi sb
    End Sub

    Dim dz As Integer = 0 'tablet right click /slide
    Private Sub TabControl1_MouseMove(sender As Object, e As MouseEventArgs) Handles TabControl1.MouseMove
        showCursor()
        If chk_tips.Checked = True And TabPage3.CanFocus Then TabControl1.ShowToolTips = True 'tip

        If chk_tips.Checked = True And TabPage3.CanFocus And TabPage3.Text <> "browser" Then TabPage3.ToolTipText = "database tab" & vbNewLine & "double click: get «xy:» countdown then print" & vbNewLine & "click + hold + swipe: menu" & vbNewLine & "right click + hold + swipe: test/run selected text" & vbNewLine & "ctrl + double click: show browser"
        If chk_tips.Checked = True And TabPage3.CanFocus And TabPage3.Text <> "browser" And Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None Then
            TabPage3.ToolTipText = "database tab" & vbNewLine & "double click: hover over item location, print «xy:»" & vbNewLine & "click + hold + swipe: db menu, swipe options" & vbNewLine & "right/long click + hold + swipe: test/run > '" & txtString.Text & "'" & vbNewLine & "ctrl + double click: show browser"
        Else
            If My.Settings.SettingDbTip = True And Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None Then
                TabControl1.ShowToolTips = True
                If TabPage3.Visible = True Then TabPage3.ToolTipText = "dna > " & TextBox1.Text Else TabPage3.ToolTipText = ""
            End If
        End If
        If chk_tips.Checked = True And TabPage3.CanFocus And TabPage3.Text = "browser" Then TabPage3.ToolTipText = "Double click: hide browser" & vbNewLine & "'" & txtString.Text & "'" & " + enter: navigate to " & "'" & txtString.Text & "'"

        If chk_tips.Checked = True And TabPage2.CanFocus = True Then TabPage2.ToolTipText = "main tab"
        If chk_tips.Checked = True And TabPage1.CanFocus = True Then TabPage1.ToolTipText = "engine tab" ' & vbNewLine & "hold left click + swipe: main menu"
        If chk_tips.Checked = True And TabPage4.CanFocus = True Then TabPage4.ToolTipText = "ignore tab (dna > i/o)" & vbNewLine & "check: ignore" ' & vbNewLine & "hold left click + swipe: main menu"


        If chk_tips.Checked = False And TabPage1.ToolTipText <> "" Then 'mute tips
            TabPage1.ToolTipText = ""
            TabPage2.ToolTipText = ""
            TabPage3.ToolTipText = ""
            TabPage4.ToolTipText = ""
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Left And TabPage3.CanFocus = True Then ' hold leftclk -db menu
            dz += 1
            If dz >= 7 Then
                ttAdjust()
                mnuItemsShow(False)
                EditToolStripMenuItem.Visible = False
                DeleteToolStripMenuItem.Visible = False
                LongTagsToolStripMenuItem.Visible = True
                TabletToolStripMenuItem.Visible = True
                ClickSwipeToolStripMenuItem.Visible = True
                OptionsToolStripMenuItem.Visible = True

                ZoneToolStripMenuItem.Text = "zone: " & My.Settings.SettingZone
                If txtString.Text = "" Then AddToolStripMenuItem.Visible = False
                If txtString.Lines.Length > 1 Then ImportToolStripMenuItem.Visible = True
                Me.ContextMenuStripDb.Show(MousePosition)
                dz = 0
            End If
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right And TabPage3.CanFocus Then 'tab 3 hold rightclk - run code/test
            dz += 1
            If dz >= My.Settings.SettingZone Then
                If My.Settings.SettingMulti = False Then rightrelease()
                runCode()
                dz = 0
            End If
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Left And TabPage2.CanFocus Then 'hold left - main tab left swipe show exit menu
            dz += 1
            If dz >= 7 Then
                showOptionsMenu()
                dz = 0
            End If
        End If
    End Sub

    Private Sub LeftHoldToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LeftHoldToolStripMenuItem.Click
        skMenuGet1("Ÿ", "left-hold", "")
    End Sub

    Private Sub LeftReleaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LeftReleaseToolStripMenuItem.Click
        skMenuGet1("¢", "left-release", "")
    End Sub

    Private Sub RightClickToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RightClickToolStripMenuItem.Click
        skMenuGet1("¦", "right-click", "")
    End Sub

    Private Sub RightHoldToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RightHoldToolStripMenuItem.Click
        skMenuGet1("ž", "right-hold", "")
    End Sub

    Private Sub RightReleaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RightReleaseToolStripMenuItem.Click
        skMenuGet1("¡", "right-release", "")
    End Sub

    Private Sub BsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BsToolStripMenuItem.Click
        If chk_tips.Checked = True Then
            skMenuGet1("¤", "back-space", "")
        Else
            skMenuGet1("¤", "bs", "")
        End If
    End Sub

    Private Sub EscToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EscToolStripMenuItem.Click
        If chk_tips.Checked = True Then
            skMenuGet1("Ç", "escape", "")
        Else
            skMenuGet1("Ç", "esc", "")

        End If
    End Sub

    Private Sub SendkeysToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SendkeysToolStripMenuItem.Click
        If LongTagsToolStripMenuItem.Checked = True Then
            skMenuGet("«sendkeys:»{left}")
        Else
            skMenuGet("«:»{left}")
        End If
    End Sub

    'generateFromDialog("wav|*.wav|all files|*.*", "audio")
    Sub generateFromDialog(strFilter As String, strTag As String)
        OpenFileDialog2.Filter = strFilter '"wav|*.wav|all files|*.*" 'auto
        If OpenFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then

            Dim s1 As Integer
            s1 = txtString.SelectionStart 'get cursor position
            'print("````") 'placeholder
            Dim s As String = ""
            For i = 1 To OpenFileDialog2.FileNames.Length
                'skMenuGet("«audio:" + OpenFileDialog2.FileNames(i - 1) + "»~") 'append
                'txtString.Text += ("«audio:" + OpenFileDialog2.FileNames(i - 1) + "»" + vbNewLine) 'append to

                If strTag = "win" Then
                    s += ("«win»r«-win»«m»" + OpenFileDialog2.FileNames(i - 1) + "«enter»«s»") '+ vbLfcollect items 'win
                Else 'url
                    s += ("«" + strTag + ":" + OpenFileDialog2.FileNames(i - 1) + "»") '+ vbLfcollect items
                End If


                If OpenFileDialog2.FileNames.Length > 1 Then s += vbNewLine 'add vbnewline if > 1
                If OpenFileDialog2.FileNames.Length = i And OpenFileDialog2.FileNames.Length > 1 Then s = Microsoft.VisualBasic.Left(s, s.Length - 2) 'remove last vbnewline

            Next
            timeout2(11) 'pause

            Dim c As String = ""
            If c <> "" Then Clipboard.GetText(c)

            Dim l As Integer = txtString.TextLength
            Clipboard.SetText(s.ToString)
            txtString.Paste()

            If c <> "" Then Clipboard.SetText(c)
            s = Nothing 'clear
        End If
    End Sub

    Private Sub AudioToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AudioToolStripMenuItem.Click
        If GetAsyncKeyState(Keys.LShiftKey) Then 'manually pick .wav file(s)
            keybd_event(Keys.LShiftKey, 0, &H2, 0)
            txtString.Focus()
            SendKeys.Send("«")
            Dim f As String = "xaudio:c:/.wav"
            apisk(f)
            SendKeys.Send("»")
            f = "x«left*5»"
            apisk(f)

            NullToolStripMenuItem1.Text = "audio"
            Exit Sub

        End If

        generateFromDialog("wav|*.wav|all files|*.*", "audio")
        NullToolStripMenuItem1.Text = "audio"
    End Sub

    Private Sub UrlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UrlToolStripMenuItem.Click
        generateFromDialog("all files|*.*", "win") '
        NullToolStripMenuItem1.Text = "-url"
    End Sub

    Private Sub UrlToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles UrlToolStripMenuItem.MouseDown
        ContextMenuStripString.Hide()

        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            generateFromDialog("all files|*.*", "win") '
            NullToolStripMenuItem1.Text = "-url"
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            If GetAsyncKeyState(Keys.LShiftKey) Then 'manual pick from *.* files
                keybd_event(Keys.LShiftKey, 0, &H2, 0)
                skMenuGet("«url:»{left}")
                Exit Sub
            End If
            generateFromDialog("all files|*.*", "url") '
            NullToolStripMenuItem1.Text = "url"
        End If
    End Sub

    Private Sub ReplaceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReplaceToolStripMenuItem.Click
        skMenuGet("«replace:|»{left 2}")
    End Sub

    Private Sub NoteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NoteToolStripMenuItem.Click
        skMenuGet("«'»{left}")
    End Sub

    Private Sub EndToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EndToolStripMenuItem.Click
        skMenuGet1("ÿ", "end", "")
    End Sub

    Private Sub HomeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HomeToolStripMenuItem.Click
        skMenuGet1("þ", "home", "")
    End Sub

    Private Sub InsertToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InsertToolStripMenuItem.Click
        skMenuGet1("ý", "insert", "")
    End Sub

    Private Sub TabToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TabToolStripMenuItem.Click
        skMenuGet1("·", "tab", "")
    End Sub

    Private Sub EnterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnterToolStripMenuItem.Click
        skMenuGet1("¬", "enter", "")
    End Sub

    Private Sub PauseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PauseToolStripMenuItem.Click
        skMenuGet1("ú", "pause", "")
    End Sub

    Private Sub PgupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PgupToolStripMenuItem.Click
        skMenuGet1("Ü", "pageup", "")
    End Sub

    Private Sub PgdownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PgdownToolStripMenuItem.Click
        skMenuGet1("Ý", "pagedown", "")
    End Sub

    Private Sub ShftToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShiftToolStripMenuItem.Click
        skMenuGet1("°ø{left}", "shift", "-")
    End Sub

    Private Sub CtrlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CtrlToolStripMenuItem.Click
        skMenuGet1("•Þ{left}", "ctrl", "-")
    End Sub

    Private Sub AltToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AltToolStripMenuItem.Click
        skMenuGet1("¹ª{left}", "alt", "-")
    End Sub

    Private Sub MnuToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MnuToolStripMenuItem.Click
        skMenuGet1("û", "menu", "")
    End Sub

    Private Sub WinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WinToolStripMenuItem.Click
        skMenuGet1("ùÙ{left}", "win", "-")
    End Sub

    Private Sub UpToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles UpToolStripMenuItem.Click
        skMenuGet1("€", "up", "")
    End Sub

    Private Sub DownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DownToolStripMenuItem.Click
        skMenuGet1("ƒ", "down", "")
    End Sub

    Private Sub LeftToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LeftToolStripMenuItem.Click
        skMenuGet1("‡", "left", "")
    End Sub

    Private Sub RightToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RightToolStripMenuItem.Click
        skMenuGet1("†", "right", "")
    End Sub

    Private Sub F1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F1ToolStripMenuItem.Click
        skMenuGet1("Í", "f1", "")
    End Sub

    Private Sub F2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F2ToolStripMenuItem.Click
        skMenuGet1("Â", "f2", "")
    End Sub

    Private Sub F3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F3ToolStripMenuItem.Click
        skMenuGet1("Ã", "f3", "")
    End Sub

    Private Sub FToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FToolStripMenuItem.Click
        skMenuGet1("Ð", "f4", "")
    End Sub

    Private Sub F5ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F5ToolStripMenuItem.Click
        skMenuGet1("Ï", "f5", "")
    End Sub

    Private Sub F6ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F6ToolStripMenuItem.Click
        skMenuGet1("Æ", "f6", "")
    End Sub

    Private Sub F7ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F7ToolStripMenuItem.Click
        skMenuGet1("Î", "f7", "")
    End Sub

    Private Sub F8ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F8ToolStripMenuItem.Click
        skMenuGet1("È", "f8", "")
    End Sub

    Private Sub F9ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F9ToolStripMenuItem.Click
        skMenuGet1("É", "f9", "")
    End Sub

    Private Sub F10ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F10ToolStripMenuItem.Click
        skMenuGet1("Ê", "f10", "")
    End Sub

    Private Sub F11ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F11ToolStripMenuItem.Click
        skMenuGet1("Ë", "f11", "")
    End Sub

    Private Sub F12ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F12ToolStripMenuItem.Click
        skMenuGet1("Ì", "f12", "")
    End Sub

    Private Sub AppToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AppToolStripMenuItem.Click
        skMenuGet("«app:»{left}")
    End Sub

    Private Sub WebToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WebToolStripMenuItem.Click
        skMenuGet("«web:»{left}")
    End Sub

    Private Sub VolToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VolToolStripMenuItem.Click
        skMenuGet1("ˆ", "volume-up", "")
    End Sub

    Private Sub VolToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VolToolStripMenuItem1.Click
        skMenuGet1("Ž", "volume-down", "")
    End Sub

    Private Function VDate() As String
        d1 = Date.Now.ToString
        d1 = Replace(d1, "/", ".")
        d1 = Replace(d1, " ", ".")
        d1 = Replace(d1, ":", ".")
        d1 = LCase(d1)

        Return d1
    End Function

    Sub startupPath()  'w8 
        Dim a As String, aa As String
        path = Application.LocalUserAppDataPath.ToString
        For i = 1 To path.ToString.Length
            a = Microsoft.VisualBasic.Left(path, i) '
            aa = Microsoft.VisualBasic.Right(path, 1) 's
            If Microsoft.VisualBasic.Right(a, 7) = "AppData" Then 'clip
                path = Microsoft.VisualBasic.Left(path, i)
                path = Replace(path, "AppData", "AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup")
                Exit For
            End If
        Next
        If chk_tips.Checked = True Then MsgBox("move dna shortcut link to:" & vbNewLine & path, vbInformation)
        apisk("x«win»r«-win»«sleep:" + My.Settings.SettingSpacer.ToString + "»" + path.ToString + "«enter»")
        clearAllKeys()
        emode()
    End Sub

    Private Function VirtualStore(vsOpen As Boolean, desktop As Boolean) As String

        Dim a As String, aa As String, d1 = VDate()
        Dim b As String
        Dim c As String
        path = Application.LocalUserAppDataPath.ToString
        If desktop = True Then
            b = "AppData"
            If My.Settings.SettingExportToOneDrive = True Then c = My.Settings.SettingExportToOneDriveDir.ToString Else c = "desktop"
        Else
            b = "Local"
            c = "Local\VirtualStore"
        End If

        For i = 1 To path.ToString.Length
            a = Microsoft.VisualBasic.Left(path, i) '
            aa = Microsoft.VisualBasic.Right(path, 1) 's


            If Microsoft.VisualBasic.Right(a, b.Length) = b Then 'clip '5 "Local"
                path = Microsoft.VisualBasic.Left(path, i)
                If vsOpen = True Then
                    path = Replace(path, b, c) '"Local\VirtualStore"
                    Exit For
                End If
                path = Replace(path, b, c & "\dna-" & d1 & ".txt") '"desktop\dna-" "Local\VirtualStore\dna-"
                Exit For
            End If
        Next

        Return path
    End Function
    Sub wsScan()
        containsws_g = False '‹›>
        For i = 0 To ListBox1.Items.Count - 1 'pre ws
            If ListBox1.Items(i).ToString.Contains("«ws»") Or ListBox1.Items(i).ToString.Contains("«-ws»") Or
                ListBox1.Items(i).ToString.Contains("‹") Or ListBox1.Items(i).ToString.Contains("›") Or
                My.Settings.SettingIgnoreWhiteSpace = True Then containsws_g = True
        Next
    End Sub

    Sub exportListToTxt1()
        Dim path = VirtualStore(False, True), d1 = VDate()
        Dim enl As String

        wsScan() 'white space scan
        If containsws_g = True Or My.Settings.SettingIgnoreWhiteSpace = True Then enl = My.Settings.SettingExportNewLine.ToString Else enl = ""

        Dim d
        Try
            d = New System.IO.StreamWriter(path)  'export to txt file
        Catch ex As Exception
            MsgBox(ex.Message.ToString & vbNewLine & vbNewLine & "Fix: " & My.Settings.SettingExportToOneDriveDir & " <- create folder", vbExclamation, "dna.exe.config: SettingExportToOneDriveDir")
            Exit Sub
        End Try

        For i = 0 To ListBox1.Items.Count - 1
            If GetAsyncKeyState(Keys.Pause) Or GetAsyncKeyState(Keys.Escape) Then 'abort
                d.Close()
                Exit Sub
            End If
            If i = ListBox1.Items.Count - 1 Then
                If ListBox1.Items.Item(i).ToString > "" Then d.Write(ListBox1.Items.Item(i) & enl)
            Else
                If ListBox1.Items.Item(i).ToString > "" Then d.Write(ListBox1.Items.Item(i) & enl & vbNewLine)
            End If
        Next

        q = MsgBox(path & vbNewLine & "please wait couple seconds before clicking yes", vbYesNo, "view?") 'If chk_tips.Checked = True Then 'msi

        path = Replace(path, ("dna-" & d1 & ".txt"), "")

        System.Threading.Thread.Sleep(1000)

        If q = vbYes Then apisk(" «win»r«-win»«,:177»" & path & "dna-" & d1 & ".txt«enter»")

        d.Close()

        reStyle()
    End Sub

    Sub reStyle()
        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable And Me.ControlBox = False Then sizeable() 'restyle
        If Me.Visible = True Then txtString.Focus()
    End Sub

    Private Sub ImportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportToolStripMenuItem.Click
        trimLastLf()
        import1()
    End Sub

    Sub timeout1(s As Double) 'call timeout seconds
        Dim d As Date = Now.AddSeconds(s)
        Do While (d) > Now
            Application.DoEvents()
            If GetAsyncKeyState(Keys.Pause) Then
                TextBox1.Text = ""
                Exit Sub
            End If
        Loop
    End Sub

    Sub timeout2(m As Double) 'call timeout milliseconds
        Dim s As Date = Now.AddMilliseconds(m)
        Do While (s) >= Now
            Application.DoEvents()
            If GetAsyncKeyState(Keys.Pause) Then
                TextBox1.Text = ""
                Exit Sub
            End If
        Loop
    End Sub

    Sub timeoutM(d As Double)
        Dim s As Date = Now.AddSeconds(d)
        Do While (s) < Now
            Application.DoEvents()
            If GetAsyncKeyState(Keys.Pause) Then Exit Sub
        Loop
    End Sub

    Private Sub TimeoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TimeoutToolStripMenuItem.Click
        If LongTagsToolStripMenuItem.Checked = True Then
            If chk_tips.Checked = True Then skMenuGet("«wait:»{left}") Else skMenuGet("«s:»{left}")
        Else
            skMenuGet("«s:»{left}")
        End If
    End Sub

    Private Sub PrintScreenToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles PrintScreenToolStripMenuItem1.Click
        skMenuGet1("¯", "print-screen", "")
    End Sub

    Sub chkItem(mnu As Object)
        If mnu.Checked = False Then 'toggle tips / dna > chk mnu
            mnu.Checked = True
        Else
            mnu.Checked = False
        End If
    End Sub

    Private Sub DnaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles tipsDnaToolStripMenuItem.Click
        chkItem(tipsDnaToolStripMenuItem)
        Me.Text = "dna"
        My.Settings.SettingDnaX = tipsDnaToolStripMenuItem.CheckState 'save tips dna > mnu
        reStyle()
    End Sub

    Private Sub chk_tips_MouseDown(sender As Object, e As MouseEventArgs) Handles chk_tips.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then 'tips mnu
            ttAdjust()
            ContextMenuStripChkTips.Show(MousePosition)
        End If
    End Sub

    Private Sub tipsDeleteToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles tipsDeleteToolStripMenuItem2.Click
        chkItem(tipsDeleteToolStripMenuItem2)
        My.Settings.SettingTipsDelete = tipsDeleteToolStripMenuItem2.CheckState 'save tips delete mnu
    End Sub

    Private Sub chkAz_CheckedChanged(sender As Object, e As EventArgs) Handles chkAz.CheckedChanged
        My.Settings.SettingChkAz = chkAz.CheckState
    End Sub

    Private Sub chk09_CheckedChanged(sender As Object, e As EventArgs) Handles chk09.CheckedChanged
        My.Settings.SettingChk09 = chk09.CheckState
    End Sub

    Private Sub chkF1f12_CheckedChanged(sender As Object, e As EventArgs) Handles chkF1f12.CheckedChanged
        My.Settings.SettingChkF1f12 = chkF1f12.CheckState
    End Sub

    Private Sub chkNumPad_CheckedChanged(sender As Object, e As EventArgs) Handles chkNumPad.CheckedChanged
        My.Settings.SettingChkNumbPad = chkNumPad.CheckState
    End Sub

    Private Sub chkMisc_CheckedChanged(sender As Object, e As EventArgs) Handles chkMisc.CheckedChanged
        My.Settings.SettingChkMisc = chkMisc.CheckState
    End Sub

    Private Sub chkArrows_CheckedChanged(sender As Object, e As EventArgs) Handles chkArrows.CheckedChanged
        My.Settings.SettingChkArrows = chkArrows.CheckState
    End Sub

    Private Sub ChkkOther_CheckedChanged(sender As Object, e As EventArgs) Handles chkOther.CheckedChanged
        My.Settings.SettingChkOther = chkOther.CheckState
    End Sub

    Private Sub chkOther_MouseDown(sender As Object, e As MouseEventArgs) Handles chkOther.MouseDown
        ttAdjust()
        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            RightCtrllToolStripMenuItem.Text = "right ctrl=" & My.Settings.SettingRctrleqMod
            ContextMenuStripChkOther.Show(MousePosition)
        End If
    End Sub

    Private Sub AltToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles AltToolStripMenuItem1.Click
        chkItem(AltToolStripMenuItem1)
        My.Settings.SettingChkOtherAlt = AltToolStripMenuItem1.CheckState
    End Sub

    Private Sub SpaceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpaceToolStripMenuItem.Click
        chkItem(SpaceToolStripMenuItem)
        My.Settings.SettingChkOtherSpace = SpaceToolStripMenuItem.CheckState
    End Sub

    Private Sub BackspaceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BackspaceToolStripMenuItem.Click
        chkItem(BackspaceToolStripMenuItem)
        My.Settings.SettingChkOtherBs = BackspaceToolStripMenuItem.CheckState
    End Sub

    Private Sub ControlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ControlToolStripMenuItem.Click
        chkItem(ControlToolStripMenuItem)
        My.Settings.SettingChkOtherControl = ControlToolStripMenuItem.CheckState
    End Sub

    Private Sub LeftControlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LeftControlToolStripMenuItem.Click
        chkItem(LeftControlToolStripMenuItem)
        My.Settings.SettingChkOtherLCtrl = LeftControlToolStripMenuItem.CheckState
    End Sub

    Private Sub RightControlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RightControlToolStripMenuItem.Click
        chkItem(RightControlToolStripMenuItem)
        My.Settings.SettingChkOtherRCtrl = RightControlToolStripMenuItem.CheckState
    End Sub

    Private Sub EnterToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EnterToolStripMenuItem1.Click
        chkItem(EnterToolStripMenuItem1)
        My.Settings.SettingChkOtherEnter = EnterToolStripMenuItem1.CheckState
    End Sub

    Private Sub ShiftToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ShiftToolStripMenuItem1.Click
        chkItem(ShiftToolStripMenuItem1)
        My.Settings.SettingChkOtherShft = ShiftToolStripMenuItem1.CheckState
    End Sub

    Private Sub LeftShiftToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LeftShiftToolStripMenuItem.Click
        chkItem(LeftShiftToolStripMenuItem)
        My.Settings.SettingChkOtherLShft = LeftShiftToolStripMenuItem.CheckState
    End Sub

    Private Sub RightShiftToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RightShiftToolStripMenuItem.Click
        chkItem(RightShiftToolStripMenuItem)
        My.Settings.SettingChkOtherRShft = RightShiftToolStripMenuItem.CheckState
    End Sub

    Private Sub CapsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CapsToolStripMenuItem.Click
        chkItem(CapsToolStripMenuItem)
        My.Settings.SettingChkOtherCaps = CapsToolStripMenuItem.CheckState
    End Sub

    Private Sub TabToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles TabToolStripMenuItem1.Click
        chkItem(TabToolStripMenuItem1)
        My.Settings.SettingChkOtherTab = TabToolStripMenuItem1.CheckState
    End Sub

    Private Sub InsertToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles InsertToolStripMenuItem1.Click
        chkItem(InsertToolStripMenuItem1)
        My.Settings.SettingChkOtherIns = InsertToolStripMenuItem1.CheckState
    End Sub

    Private Sub WinToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles WinToolStripMenuItem1.Click
        chkItem(WinToolStripMenuItem1)
        My.Settings.SettingChkOtherWin = WinToolStripMenuItem1.CheckState
    End Sub

    Private Sub EscToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EscToolStripMenuItem1.Click
        chkItem(EscToolStripMenuItem1)
        My.Settings.SettingChkOtherEsc = EscToolStripMenuItem1.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscSc_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscSc.Click
        chkItem(ToolStripMenuItemChkMiscSc)
        My.Settings.SettingChkMiscSc = ToolStripMenuItemChkMiscSc.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscFs_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscFs.Click
        chkItem(ToolStripMenuItemChkMiscFs)
        My.Settings.SettingChkMiscFs = ToolStripMenuItemChkMiscFs.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscTil_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscTil.Click
        chkItem(ToolStripMenuItemChkMiscTil)
        My.Settings.SettingChkMiscTil = ToolStripMenuItemChkMiscTil.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscLb_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscLb.Click
        chkItem(ToolStripMenuItemChkMiscLb)
        My.Settings.SettingChkMiscLb = ToolStripMenuItemChkMiscLb.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscBs_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscBs.Click
        chkItem(ToolStripMenuItemChkMiscBs)
        My.Settings.SettingChkMiscBs = ToolStripMenuItemChkMiscBs.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscRb_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscRb.Click
        chkItem(ToolStripMenuItemChkMiscRb)
        My.Settings.SettingChkMiscRb = ToolStripMenuItemChkMiscRb.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscRem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscRem.Click
        chkItem(ToolStripMenuItemChkMiscRem)
        My.Settings.SettingChkMiscRem = ToolStripMenuItemChkMiscRem.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscPeriod_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscPeriod.Click
        chkItem(ToolStripMenuItemChkMiscPeriod)
        My.Settings.SettingChkMiscPeriod = ToolStripMenuItemChkMiscPeriod.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscComma_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscComma.Click
        chkItem(ToolStripMenuItemChkMiscComma)
        My.Settings.SettingChkMiscComma = ToolStripMenuItemChkMiscComma.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscPlus_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscPlus.Click
        chkItem(ToolStripMenuItemChkMiscPlus)
        My.Settings.SettingChkMiscPlus = ToolStripMenuItemChkMiscPlus.CheckState
    End Sub

    Private Sub ToolStripMenuItemChkMiscMinus_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemChkMiscMinus.Click
        chkItem(ToolStripMenuItemChkMiscMinus)
        My.Settings.SettingChkMiscMinus = ToolStripMenuItemChkMiscMinus.CheckState
    End Sub

    Private Sub chkMisc_MouseDown(sender As Object, e As MouseEventArgs) Handles chkMisc.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            ContextMenuStripChkMisc.Show(MousePosition)
        End If
    End Sub

    Private Sub DeleteToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem2.Click
        chkItem(DeleteToolStripMenuItem2)
        My.Settings.SettingChkOtherDelete = DeleteToolStripMenuItem2.CheckState
    End Sub

    Private Sub DeleteToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem3.Click
        skMenuGet1("º", "delete", "") ' skMenuGet("Á")
    End Sub

    Sub showTab(n As Integer)
        TabControl1.Visible = True 'show tab main
        TabControl1.SelectTab(n)
    End Sub

    Sub runCode()
        If txtString.Text = "" Then Exit Sub
        Dim f As String = "x" & txtString.Text 'test > txtString
        If txtString.SelectedText > "" Then f = "x" & txtString.SelectedText

        Me.Visible = False

        If connect = False Or strandComplete = False Then
            Dim cb = Clipboard.GetText
            apisk(f)
            If Not Clipboard.GetText = cb Then If g_remcb Then Clipboard.SetText(cb)
        Else
            strandComplete = True
            connect = True
            apisk(f)
        End If


        If f.Contains("«io:»") Then 'v2
            print(f, True)
            TextBox1.Text = "«"
        Else
            If Not finished Then print(f, True)
        End If

        emode() '»
        spacer()
        If Me.IsDisposed = False Then Visible = True
    End Sub

    Sub randNumb(nu As Boolean, le As Boolean, le1 As Boolean)
        Dim x As Random
        Dim n As Integer
        x = New Random
        If nu = True Then n = x.Next(0, 10) '1-10
        If le = True Or le1 = True Then n = x.Next(0, 26) 'a-z
        Dim ke As Integer = n

        If nu = True Then
            Select Case ke
                Case 0
                    ke = Keys.D0
                Case 1
                    ke = Keys.D1
                Case 2
                    ke = Keys.D2
                Case 3
                    ke = Keys.D3
                Case 4
                    ke = Keys.D4
                Case 5
                    ke = Keys.D5
                Case 6
                    ke = Keys.D6
                Case 7
                    ke = Keys.D7
                Case 8
                    ke = Keys.D8
                Case 9
                    ke = Keys.D9
            End Select

        End If

        If le = True Then 'a-z
            Select Case ke
                Case 0
                    ke = Keys.A
                Case 1
                    ke = Keys.B
                Case 2
                    ke = Keys.C
                Case 3
                    ke = Keys.D
                Case 4
                    ke = Keys.E
                Case 5
                    ke = Keys.F
                Case 6
                    ke = Keys.G
                Case 7
                    ke = Keys.H
                Case 8
                    ke = Keys.I
                Case 9
                    ke = Keys.J
                Case 10
                    ke = Keys.K
                Case 11
                    ke = Keys.L
                Case 12
                    ke = Keys.M
                Case 13
                    ke = Keys.N
                Case 14
                    ke = Keys.O
                Case 15
                    ke = Keys.P
                Case 16
                    ke = Keys.Q
                Case 17
                    ke = Keys.R
                Case 18
                    ke = Keys.S
                Case 19
                    ke = Keys.T
                Case 20
                    ke = Keys.U
                Case 21
                    ke = Keys.V
                Case 22
                    ke = Keys.W
                Case 23
                    ke = Keys.X
                Case 24
                    ke = Keys.Y
                Case 25
                    ke = Keys.Z
                Case 26
                    ke = Keys.Z
            End Select
        End If

        If le1 = True Then 'A-Z 
            keybd_event(Keys.LShiftKey, 0, 0, 0) 'hold shift
        End If

        keybd_event(ke, 0, 0, 0)  'press char 
        keybd_event(ke, 0, &H2, 0)

        If le1 = True Then 'A-Z
            keybd_event(Keys.LShiftKey, 0, &H2, 0) 'shift release
        End If

        GetAsyncKeyState(ke) 'clear

        x = Nothing 'clear
        n = Nothing
        nu = Nothing
        le = Nothing
        le1 = Nothing
        ke = Nothing
    End Sub

    Private Sub NumberToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NumberToolStripMenuItem.Click
        skMenuGet("«#»")
    End Sub

    Private Sub LetterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LetterToolStripMenuItem.Click
        skMenuGet("«x»")
    End Sub
    Sub keyz(aa As Keys, num As Integer)
        For index = 1 To num
            keybd_event(aa, 0, 0, 0)  'press char 
            keybd_event(aa, 0, 2, 0) '&H2
        Next
        GetAsyncKeyState(aa) 'clear
    End Sub
    Sub key(aa As Keys)
        '48-57 0-9
        '65-90 a-z
        keybd_event(aa, 0, 0, 0)  'press char 
        keybd_event(aa, 0, 2, 0) '&H2
        GetAsyncKeyState(aa) 'clear
    End Sub

    Private Sub EscHToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShiftEscapeToolStripMenuItem.Click
        chkItem(ShiftEscapeToolStripMenuItem)
        My.Settings.SettingChkOnShiftEscape = ShiftEscapeToolStripMenuItem.CheckState
    End Sub

    Private Sub chk_timer1_on_val_MouseDown(sender As Object, e As MouseEventArgs) Handles chk_timer1_on_val.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            ttAdjust()
            ContextMenuStripChkOn.Show(MousePosition)
        End If
    End Sub

    Private Sub InsertHereToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InsertHereToolStripMenuItem.Click
        txtString.SelectionStart = 0 'home

        For l = 1 To My.Settings.SettingTxtCodeLength 'right 4
            key(Keys.Right)
        Next

        If Len(txtString.Text) >= My.Settings.SettingTxtCodeLength + 1 Then 'if length good
            If GetChar(txtString.Text, My.Settings.SettingTxtCodeLength + 1) = Chr(9) Then key(Keys.Right) 'right
        End If

        skMenuGet("«//»{left}") 'print
    End Sub

    Sub skCodes(skCode As String, bar As String, k As Keys)

        If bar.EndsWith("*1") Then bar = bar.Replace("*1", "") ' filter *1

        If bar = skCode Then 'run
            key(k) 'no key/digit 
            Exit Sub
        End If

        'if *# or *#:#-# 

        If bar = "" Then Exit Sub

        If aa > "" And bar.Contains("#:") Or bar.Contains("*") And bar.Contains(skCode) And bar.Length > skCode.Length And GetChar(bar, 1) = GetChar(skCode, 1) Then
            aa = ""

            Dim starp As String = Microsoft.VisualBasic.Right(bar, bar.Length - bar.IndexOf("*") - 1)

            If bar.Contains("#:") Or bar.Contains("*r:") Then 'rand #:#-#
                Dim bb As Integer, cc As Integer, a As String, b As String
                cc = bar.IndexOf(":") + 2
                bb = bar.IndexOf("-") + 1
                a = Microsoft.VisualBasic.Mid(bar, cc, bb - cc)
                b = Microsoft.VisualBasic.Right(bar, bar.Length - bb)

                If IsNumeric(a) And IsNumeric(b) Then
                    Dim x As Random
                    Dim n As Long
                    x = New Random
                    If a > b + 1 Then Exit Sub
                    If a > Integer.MaxValue Or b > Integer.MaxValue Then Exit Sub
                    n = x.Next(a, b + 1)  '#x-#x
                    starp = n
                End If
            End If
            If IsNumeric(starp) = 0 Then
                '0
            Else
                If IsNumeric(starp) Then
                    For vbarf = 1 To Val(starp)
                        If GetAsyncKeyState(Keys.Pause) Then Exit For 'abort
                        key(k) 'run*#
                    Next
                Else
                    MsgBox("error: «" & bar & "»", vbInformation) 'error
                    TextBox1.Text = ""
                    Exit Sub
                End If
            End If
        End If
    End Sub

    Sub shiftHold()
        keybd_event(Keys.LShiftKey, 0, 1, 0)
    End Sub
    Sub shiftRelease()
        keybd_event(Keys.LShiftKey, 0, 2, 0)
        keybd_event(Keys.RShiftKey, 0, 2, 0)
        keybd_event(Keys.ShiftKey, 0, 2, 0)
    End Sub
    Sub altHold()
        keybd_event(Keys.RMenu, 0, 0, 0)
    End Sub
    Sub altRelease()
        keybd_event(Keys.RMenu, 0, 2, 0)
    End Sub

    Sub skCodes1(skCode As String, bar As String, k As Keys) ', a As String) ', a As String

        If Microsoft.VisualBasic.Left(bar, skCode.Length) = skCode Then
            If skCode = "shift" Or skCode = "ctrl" Or skCode = "alt" Or skCode = "win" Then 'hold 
                If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort

                If skCode = "shift" Then 'shift hold 2.23.14
                    shiftHold()
                    Exit Sub
                End If
                If skCode = "alt" Then 'alt hold 2.26.14
                    altHold()
                    Exit Sub
                End If

                keybd_event(k, 0, 0, 0)
                Exit Sub
            End If

            If skCode = "-shift" Or skCode = "-ctrl" Or skCode = "-alt" Or skCode = "-win" Then 'release
                If GetAsyncKeyState(Keys.Pause) Then Exit Sub 'abort

                If skCode = "-shift" Then 'shift hold 2.23.14
                    shiftRelease()
                    Exit Sub
                End If
                If skCode = "-alt" Then 'alt hold 2.26.14
                    altRelease()
                    Exit Sub
                End If

                keybd_event(k, 0, &H2, 0)
                Exit Sub
            End If

            If GetChar(bar, skCode.Length + 1) = "*" Then
                If IsNumeric(GetChar(bar, skCode.Length + 2)) Then 'if #
                    Dim u As Integer = Val(GetChar(bar, skCode.Length + 2)) '* len
                    For n = 1 To u - 1 'press * - top key
                        If skCode = "shift" Or skCode = "ctrl" Or skCode = "alt" Or skCode = "win" Then 'multi hold
                            If GetAsyncKeyState(Keys.Pause) Then Exit For 'abort
                            keybd_event(k, 0, 0, 0)
                        End If

                        If skCode = "-shift" Or skCode = "-ctrl" Or skCode = "-alt" Or skCode = "win" Then 'multi release
                            If GetAsyncKeyState(Keys.Pause) Then Exit For 'abort
                            keybd_event(k, 0, &H2, 0)
                        End If
                    Next
                    Exit Sub
                End If
            End If
        End If
    End Sub

    Sub dbCode(code As String) 'txtString KeyPress !temp db. with «tag»
        cl = code.Length + 3
        If Microsoft.VisualBasic.Right(txtString.Text, cl) = ("«" & code & vbLf & "»") Then 'import
            txtString.Focus()
            For i = 1 To cl - 1
                key(Keys.Back)
            Next
            key(Keys.Delete)
            timeout1(1)
            If txtString.Text = "«" & code & vbLf & "»" Then Exit Sub
            Select Case code
                Case "import"
                    trimLastLf()
                    import1()
                Case "i"
                    import1()
                Case "add"
                    addDbItm()
                Case "a"
                    addDbItm()
                Case "edit"
                    editDbItm()
                Case "e"
                    editDbItm()
                Case "update"
                    dbToUpdate()
                Case "u"
                    dbToUpdate()
            End Select
        End If
    End Sub

    Sub sleep(m As Integer)
        System.Threading.Thread.Sleep(m)
    End Sub

    Sub autoLock()
        If My.Settings.SettingRctrleqMod <> "»" Then
            x = MsgBox("unckeck: ignore tab -> other" & vbNewLine &
                       "check: right ctrl=" & vbNewLine &
                       "set: right ctrl=»" & vbNewLine &
                       "(current: right ctrl=" & My.Settings.SettingRctrleqMod.ToString & ")", vbYesNo, "adjust settings for auto lock?")
            If x = vbYes Then
                chkOther.Checked = False
                RightCtrllToolStripMenuItem.Checked = True
                My.Settings.SettingRctrleqMod = "»"
            Else
                If GetAsyncKeyState(Keys.LControlKey) Then My.Settings.SettingRctrleqMod = "«" 'reset to «
                Exit Sub
            End If
        End If ' (settings: ignore other -> right ctrl=» |  false: right ctrl for manual lock / dna > »)

        If My.Settings.SettingAutoLockEmode = False Then
            My.Settings.SettingAutoLockEmode = True
        Else
            My.Settings.SettingAutoLockEmode = False
        End If

        Dim m As String
        If My.Settings.SettingAutoLockEmode = False And My.Settings.SettingRctrleqMod = "»" Then m = "right ctrl to manually lock" & vbNewLine Else m = ""
        If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(al + enter)" & vbNewLine & "auto lock after run: " & LCase(My.Settings.SettingAutoLockEmode) & vbNewLine & m & vbNewLine & "left ctrl + ok: set right ctrl=« (for no length mode)", vbInformation, "dna.exe.config: SettingAutoLockEmode (dna > »)")
        If GetAsyncKeyState(Keys.LControlKey) Then My.Settings.SettingRctrleqMod = "«" 'reset to «

        If chk_tips.Checked = True And My.Settings.SettingAutoLockEmode = True Then MsgBox("tip: right ctrl outside of program for best results", vbInformation, "toggle dna > » or «")

        masterClear()
    End Sub

    Sub changeColor() 'dna + enter config
        Dim fc = MsgBox("(cc + enter)" & vbNewLine & "edit: " & LCase(My.Settings.SettingChangeColor.ToString) & "?", vbYesNo, "dna.exe.config: SettingChangeColor")
        If fc = MsgBoxResult.Cancel Then Exit Sub
        If fc = MsgBoxResult.Yes Then
            If ColorDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Me.ListBox1.ForeColor = ColorDialog1.Color
                Me.txtString.ForeColor = ColorDialog1.Color
                Me.txtLength.ForeColor = ColorDialog1.Color
                Me.ForeColor = ColorDialog1.Color
                TabPage1.ForeColor = ColorDialog1.Color
                TabPage2.ForeColor = ColorDialog1.Color
                TabPage4.ForeColor = ColorDialog1.Color
                My.Settings.SettingChangeColor = ColorDialog1.Color
            End If
        End If
    End Sub

    Sub changeIcon()
        Dim fc = MsgBox("(ic + enter)" & vbNewLine & "edit: " & LCase(My.Settings.SettingIcon.ToString) & "?", vbYesNo, "dna.exe.config: SettingIcon")
        If fc = MsgBoxResult.Cancel Then Exit Sub
        If fc = MsgBoxResult.Yes Then
            If OpenFileDialogIco.ShowDialog = Windows.Forms.DialogResult.OK Then
                Try
                    Dim ico = LCase(Replace(OpenFileDialogIco.FileName.ToString, "\", "/"))
                    Me.Icon = New Icon(ico)
                    My.Settings.SettingIcon = ico
                Catch ex As Exception
                End Try
            End If
        End If
    End Sub

    Sub shrink()
        If Me.ControlBox = False Or Me.FormBorderStyle = FormBorderStyle.None Then Exit Sub
        Me.Height = 39
        If ShowIcon = False Or tipsDnaToolStripMenuItem.Checked = False Then Me.Width = 136 Else Me.Width = 150
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        'Me.TabControl1.Visible = False
        Try
            SplitContainer1.SplitterDistance = 0
        Catch ex As Exception
        End Try
        txtString.Visible = False
    End Sub

    Sub dbCode1(code As String) '«no tag»  txtString keypress enter
        If txtString.Text <> (code & vbLf) Then Exit Sub

        cl = code.Length + 1
        If Microsoft.VisualBasic.Left(txtString.Text, cl) = (code & vbLf) Then '
            txtStringClear()
            Select Case code 'keypress enter run beginning txt 'add new in 'no tag
                Case "ato"
                    ml = InputBox("«app:» error, auto tries:", "dna.exe.config: SettingAppErrorAutoTries", My.Settings.SettingAppErrorAutoTries)
                    If ml > 0 Then
                        If IsNumeric(ml) And ml > 0 Then My.Settings.SettingAppErrorAutoTries = ml
                    End If
                Case "si"
                    sizeable()
                Case "dbtip"
                    If My.Settings.SettingDbTip = True Then
                        My.Settings.SettingDbTip = False
                        TabPage3.ToolTipText = ""
                    Else
                        My.Settings.SettingDbTip = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(dbtip + enter)" & vbNewLine & "show db tab dna > tip: " & LCase(My.Settings.SettingDbTip), vbInformation, "dna.exe.config: SettingDbTip")
                Case "op"
                    op = InputBox("Opacity: " & vbNewLine & "example: .5", "dna.exe.config: SettingOpacity", My.Settings.SettingOpacity)
                    If Val(op) >= 0.1 Or Val(op) <= 100 And Val(op) > 0 Then
                        My.Settings.SettingOpacity = op
                    Else
                        My.Settings.SettingOpacity = 1
                        Exit Sub
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(op + enter)" & vbNewLine & "Opacity: " & My.Settings.SettingOpacity, vbInformation, "dna.exe.config: SettingOpacity")
                    Me.Opacity = My.Settings.SettingOpacity
                Case "export"
                    export_mr()
                Case "rf" 'reset font
                    changeFont(False)
                Case "s"
                    shrink()
                Case "ic"
                    changeIcon()
                Case "cc"
                    changeColor()
                Case "v"
                    changeView()
                    emode()
                Case "cv"
                    changeView()
                Case "o" 'open
                    SplitContainer1.SplitterDistance = SplitContainer1.Height
                    ListBox1.Focus()
                    Exit Sub
                Case "c" 'close
                    SplitContainer1.SplitterDistance = 0
                    txtString.Focus()
                    Exit Sub
                Case "font"
                    changeFont(True)
                    Exit Sub
                Case "x"
                    ExitToolStripMenuItem.PerformClick()
                Case "sl"
                    If My.Settings.SettingScrollLockRun = True Then
                        My.Settings.SettingScrollLockRun = False
                    Else
                        My.Settings.SettingScrollLockRun = True
                    End If
                    g_scroll = My.Settings.SettingScrollLockRun
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(sl + enter)" & vbNewLine & "scroll lock, run: " & LCase(My.Settings.SettingScrollLockRun), vbInformation, "dna.exe.config: SettingScrollLockRun")
                Case "ml" 'g_maxkeylen
                    ml = InputBox("dna > «length", "dna.exe.config: SettingMaxKeyLength", My.Settings.SettingMaxKeyLength)
                    If ml > "" Then
                        If IsNumeric(ml) And ml > 1 Then
                            g_maxkeylen = ml
                            My.Settings.SettingMaxKeyLength = ml
                        End If
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox(
                        "(ml + enter)" & vbNewLine &
                        "dna > «length: " & My.Settings.SettingMaxKeyLength & ", clear", vbInformation, "dna.exe.config: SettingMaxKeyLength")
                Case "odd" 'onedrive
                    odd = InputBox("OneDrive folder: ", "dna.exe.config: SettingExportToOneDriveDir", My.Settings.SettingExportToOneDriveDir)
                    If odd.ToString > "" Then My.Settings.SettingExportToOneDriveDir = odd.ToString Else odd = "OneDrive\dna"
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(od + enter)" & vbNewLine & "export to OneDrive: " & My.Settings.SettingExportToOneDriveDir, vbInformation, "dna.exe.config: SettingExportToOneDriveDir")
                Case "od" 'onedrive
                    If My.Settings.SettingExportToOneDrive = True Then
                        My.Settings.SettingExportToOneDrive = False
                    Else
                        My.Settings.SettingExportToOneDrive = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(od + enter)" & vbNewLine & "export to OneDrive: " & LCase(My.Settings.SettingExportToOneDrive), vbInformation, "dna.exe.config: SettingExportToOneDrive")
                Case "ar" 'auro retry rtapp app:error
                    If My.Settings.SettingAutoRetryAppError = True Then
                        My.Settings.SettingAutoRetryAppError = False
                    Else
                        My.Settings.SettingAutoRetryAppError = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(ar + enter)" & vbNewLine & "Auto retry if «app:» not ready: " & LCase(My.Settings.SettingAutoRetryAppError) & vbNewLine & "pause break: abort", vbInformation, "dna.exe.config: SettingAutoRetryAppError")
                Case "dna"
                    TextBox1.Text = "'"
                    dnauserconfig()
                    emode()
                    Me.ShowIcon = True
                Case "bg"
                    editMainBgImg()
                Case "ws" 'ignore white space
                    ignoreWhiteSpacef()
                Case "nl" 'no length mode
                    noLengthMode()
                Case "al" 'auto lock dna > »
                    autoLock()
                Case "cb" 'remember clipboard 
                    If My.Settings.SettingRememberClipboard = True Then
                        My.Settings.SettingRememberClipboard = False
                    Else
                        My.Settings.SettingRememberClipboard = True
                    End If
                    g_remcb = My.Settings.SettingRememberClipboard
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(cb + enter)" & vbNewLine & "remember clipboard after run: " & LCase(My.Settings.SettingRememberClipboard), vbInformation, "dna.exe.config: SettingRememberClipboard")
                Case "swipe"
                    If My.Settings.SettingSwMnu = True Then
                        My.Settings.SettingSwMnu = False
                    Else
                        My.Settings.SettingSwMnu = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(swipe + enter)" & vbNewLine & "show swipe menu: " & LCase(My.Settings.SettingSwMnu), vbInformation, "dna.exe.config: SettingSwMnu")
                    ClickSwipeToolStripMenuItem.Visible = My.Settings.SettingSwMnu
                Case "algorithm"
                    If My.Settings.SettingAlgMnu = True Then
                        My.Settings.SettingAlgMnu = False
                    Else
                        My.Settings.SettingAlgMnu = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(algorithm + enter)" & vbNewLine & "show algorithm menu: " & LCase(My.Settings.SettingAlgMnu), vbInformation, "dna.exe.config: SettingAlgMnu")
                    LongTagsToolStripMenuItem.Visible = My.Settings.SettingAlgMnu
                Case "h" 'hide tabs
                    'TabControl1.Hide()
                    checkIfOn()
                    Me.Focus()
                Case "ht" 'hide tabs
                    TabControl1.Hide()
                    Me.Focus()
                Case "browser"
                    showBrowserTab()
                Case "wb" '.html bg
                    If WebBrowser1.Visible = False Then
                        WebBrowser1.Visible = True
                        Return
                    Else
                        WebBrowser1.Visible = False
                        Return
                    End If
                Case "osk"
                    OskToolStripMenuItem.Visible = True
                Case "gc:"
                    If My.Settings.SettingGCCollect = True Then
                        My.Settings.SettingGCCollect = False
                    Else
                        My.Settings.SettingGCCollect = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(gc + enter)" & vbNewLine & "gc: " & LCase(My.Settings.SettingGCCollect), vbInformation, "dna.exe.config: SettingGCCollect")
                Case "gc"
                    GC.Collect()
                Case "delete all"
                    deleteDbItmAll()
                Case "e"
                    editDbItm()
                Case "ed"
                    editDbItm()
                Case "edit"
                    editDbItm()
                Case "d"
                    ListBox1.Focus()
                Case "db"
                    ListBox1.Focus()
                Case "t"
                    selectTopItem()
                Case "b"
                    selectBottomItem()
                Case "tips"
                    chkItem(chk_tips)
                Case "interval"
                    changeInterval()
                Case "length"
                    LengthToolStripMenuItem.PerformClick()
                Case "virtual store" 'virtual store db
                    path = VirtualStore(True, False)
                    System.Threading.Thread.Sleep(60)
                    apisk("x«win»r«-win»«sleep:" + My.Settings.SettingSpacer.ToString + "»" + path.ToString + "«enter»")
                    clearAllKeys()
                    emode()
                Case "config" 'user.config
                    If chk_tips.Checked = True Then MsgBox("edit dna.exe.config", vbInformation)
                    path = Application.StartupPath
                    apisk("x«win»r«-win»«sleep:" + My.Settings.SettingSpacer.ToString + "»" + path.ToString + "\dna.exe.config«enter»")
                    clearAllKeys()
                    emode()
                Case "a-z"
                    chkItem(chkAz)
                Case "0-9"
                    chkItem(chk09)
                Case "f1-f12"
                    chkItem(chkF1f12)
                Case "number pad"
                    chkItem(chkNumPad)
                Case "np"
                    chkItem(chkNumPad)
                Case "misc"
                    chkItem(chkMisc)
                Case "arrows"
                    chkItem(chkArrows)
                Case "media"
                    chkItem(chkWedgee)
                Case "other"
                    chkItem(chkOther)
                Case "hide" 'hide program
                    checkIfOn()
                Case "clear" 'clear db
                    ListBox1.Items.Clear()
                Case "reload" 'reload db
                    reloadDb()
                Case "restart" 'restart program
                    saveSettings()
                    Application.Restart()
                Case "startup"
                    startupPath()
                Case "help"
                    If txtString.WordWrap = False Then WordWrapToolStripMenuItem.PerformClick()
                    SplitContainer1.SplitterDistance = 0
                    txtString.Text = My.Settings.SettingHelp
                    txtString.Focus()
                Case "?"
                    If My.Settings.SettingWordWrap = False Then WordWrapToolStripMenuItem.PerformClick()
                    SplitContainer1.SplitterDistance = 0
                    txtString.Text = My.Settings.SettingHelp
                    txtString.Focus()
                    My.Settings.SettingWordWrap = False
                Case "skin"
                    skin()
                Case "ts" 'touch screen
                    My.Settings.SettingMoveBar = True
                    moveable()
                Case "ww" 'word wrap
                    WordWrapToolStripMenuItem.PerformClick()
                Case "on"
                    chk_timer1_on_val.Checked = True
                Case "off"
                    chk_timer1_on_val.Checked = False
                Case "top"
                    chkItem(chk_top)
                Case "exit" 'exit
                    Close()
                Case "close" 'exit
                    Close()
                Case "sw" 'toggle swipe to run
                    chkItem(TabletToolStripMenuItem)
                    My.Settings.SettingTabletSwipe = TabletToolStripMenuItem.CheckState
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(sw + enter)" & vbNewLine & "swipe item to run: " & LCase(My.Settings.SettingTabletSwipe), vbInformation, "dna.exe.config: SettingTabletSwipe")
                Case "mc" 'init middle click item run
                    If My.Settings.SettingRunMiddleClickInit = False Then
                        My.Settings.SettingRunMiddleClickInit = True
                    Else
                        My.Settings.SettingRunMiddleClickInit = False
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(mc + enter)" & vbNewLine & "middle click item to run: " & LCase(My.Settings.SettingRunMiddleClickInit), vbInformation, "dna.exe.config: SettingRunMiddleClickInit")
                Case "dc" 'double click to run
                    If My.Settings.SettingRunDblClick = False Then
                        My.Settings.SettingRunDblClick = True
                    Else
                        My.Settings.SettingRunDblClick = False
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(dc + enter)" & vbNewLine & "double click item run: " & LCase(My.Settings.SettingRunDblClick), vbInformation, "dna.exe.config: SettingRunDblClick")
                Case "rr" 'rclick run
                    If My.Settings.SettingRunRClick = False Then
                        My.Settings.SettingRunRClick = True
                    Else
                        My.Settings.SettingRunRClick = False
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(rr + enter)" & vbNewLine & "right click item to run: " & LCase(My.Settings.SettingRunRClick), vbInformation, "dna.exe.config: SettingRunRClick")
                Case "ep" 'esc + period
                    If My.Settings.SettingChkEscPeriodRun = True Then
                        My.Settings.SettingChkEscPeriodRun = False
                    Else
                        My.Settings.SettingChkEscPeriodRun = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(ep + enter)" & vbNewLine & "escape + period to run: " & LCase(My.Settings.SettingChkEscPeriodRun.ToString), vbInformation, "dna.exe.config: SettingChkEscPeriodRun")
                Case "ei" 'esc + insert run
                    If My.Settings.SettingChkEscInsRun = True Then
                        My.Settings.SettingChkEscInsRun = False
                    Else
                        My.Settings.SettingChkEscInsRun = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(ei + enter)" & vbNewLine & "escape + insert to run: " & LCase(My.Settings.SettingChkEscInsRun.ToString), vbInformation, "dna.exe.config: SettingChkEscInsRun")
                Case "re" 'rctrl + enter run
                    If My.Settings.SettingChkRCtrlEnterRun = True Then
                        My.Settings.SettingChkRCtrlEnterRun = False
                    Else
                        My.Settings.SettingChkRCtrlEnterRun = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(re + enter)" & vbNewLine & "right ctrl + enter to run text: " & LCase(My.Settings.SettingChkRCtrlEnterRun.ToString), vbInformation, "dna.exe.config: SettingChkRCtrlEnterRun")
                Case "cr" 'click run
                    If My.Settings.SettingRunClick = False Then
                        My.Settings.SettingRunClick = True
                    Else
                        My.Settings.SettingRunClick = False
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(cr + enter)" & vbNewLine & "click item to run: " & LCase(My.Settings.SettingRunClick), vbInformation, "dna.exe.config: SettingRunClick")
                Case "sh" 'stay hidden after middle click item run
                    If My.Settings.SettingRunStayHidden = False Then
                        My.Settings.SettingRunStayHidden = True
                    Else
                        My.Settings.SettingRunStayHidden = False
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(sh + enter)" & vbNewLine & "stay hidden after middle click item run: " & LCase(My.Settings.SettingRunStayHidden), vbInformation, "dna.exe.config: SettingRunStayHidden")
                Case "ex" 'drag to extended screen
                    If My.Settings.SettingChkDragToExtendedScreen = False Then
                        My.Settings.SettingChkDragToExtendedScreen = True
                    Else
                        My.Settings.SettingChkDragToExtendedScreen = False
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("drag to extended screen: " & LCase(My.Settings.SettingChkDragToExtendedScreen.ToString), vbInformation, "dna.exe.config: SettingChkDragToExtendedScreen")
                Case "w7" 'Windows7
                    If My.Settings.SettingChkW7 = True Then
                        My.Settings.SettingChkW7 = False
                    Else
                        My.Settings.SettingChkW7 = True
                    End If
                    If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(w7 + enter)" & vbNewLine & "Windows 7 mode: " & (My.Settings.SettingChkW7.ToString), vbInformation, "dna.exe.config: SettingChkW7")

            End Select
        End If
    End Sub

    Private Sub txtString_DoubleClick(sender As Object, e As EventArgs) Handles txtString.DoubleClick
        If txtString.TextLength > 8 Then 'clear "?" txt
            If Microsoft.VisualBasic.Left(txtString.Text, 4) = "make" And Microsoft.VisualBasic.Right(txtString.Text, 4) = "====" Then
                clearTxtString()
                If My.Settings.SettingWordWrap = False Then WordWrapToolStripMenuItem.PerformClick()
                resizeSplitter()
            End If
        End If

        If chk_tips.CheckState = CheckState.Checked And txtString.Text = "" And Len(TextBox1.Text) < My.Settings.SettingTxtCodeLength Then  'if  "" 
            Dim s As String = "turn on engine first " & vbNewLine

            If chk_timer1_on_val.Checked = True Then
                s = ""
            End If

            MsgBox(s & "1) press " & txtLength.Text & " keys on your keyboard right now (example: test)" & vbNewLine & "    this will be your " & txtLength.Text & " button keyboard shortcut" & vbNewLine & "2) click ok button below then" & vbNewLine & "3) double click blank text box in db tab to" & vbNewLine & "    get your " & txtLength.Text & " button keyboard shortcut code" & vbNewLine & "4) type out a custom «algorithm» (example: «bs*4»123)" & vbNewLine & "    tip: right click text box and pick from menu" & vbNewLine & "5) press ctrl + s when finished to save to db" & vbNewLine & "6) type " & txtLength.Text & " keys from above in a text box" & vbNewLine & vbNewLine & "example: test  «bs*4»123" & vbNewLine & vbNewLine & "(main tab, length must be set to 4" & vbNewLine & "engine tab, on must be checked" & vbNewLine & "ignore tab, a-z must be unchecked)", vbInformation, "to make a " & txtLength.Text & " button keyboard shortcut")
            Exit Sub
        End If

        If txtString.Text = "" And Len(TextBox1.Text) = My.Settings.SettingTxtCodeLength Then 'if > "" 
            txtString.Text = TextBox1.Text & vbTab 'grab code
            txtString.SelectionStart = Len(txtString.Text)
            Exit Sub
        End If

        If txtString.SelectedText = "" Then
            SendKeys.Send("«»{left}") '«»
            Exit Sub
        End If
    End Sub

    Private Sub txtString_KeyDown(sender As Object, e As KeyEventArgs) Handles txtString.KeyDown
        If GetAsyncKeyState(Keys.Oemcomma) Then '«,» move right one
            If GetAsyncKeyState(Keys.LShiftKey) Then Exit Sub
            If GetAsyncKeyState(Keys.RShiftKey) Then Exit Sub
            Try
                If GetChar(txtString.Text, txtString.SelectionStart) = "«" And GetChar(txtString.Text, txtString.SelectionStart + 1) = "»" Then
                    'keyRelease(Keys.Oemcomma)
                    key(Keys.Right)
                    retap = -1
                    Exit Sub
                End If
            Catch ex As Exception
            End Try
        End If

        If GetAsyncKeyState(Keys.F8) Then 'oXY
            keyRelease(Keys.F8)
            keyClear(Keys.F8)
            runMousePosition()
            reStyle()
            Exit Sub
        End If
        'move cursor home or end
        If GetAsyncKeyState(Keys.Down) Then 'down {end}
            If txtString.Text > "" Then
                Dim getLastLineNumb = txtString.GetLineFromCharIndex(txtString.SelectionStart + txtString.TextLength) + 1
                Dim getLineNumb As Integer = txtString.GetLineFromCharIndex(txtString.SelectionStart) + 1
                If getLineNumb = getLastLineNumb Then txtString.SelectionStart = txtString.TextLength : Exit Sub 'Bottom
            End If
        End If
        If GetAsyncKeyState(Keys.Up) Then 'up {home}
            If txtString.Text > "" Then
                Dim getLineNumb As Integer = txtString.GetLineFromCharIndex(txtString.SelectionStart) + 1
                If getLineNumb = 1 Then txtString.SelectionStart = 0 : Exit Sub 'Bottom
            End If
        End If

        If GetAsyncKeyState(Keys.ControlKey) And GetAsyncKeyState(Keys.Space) Then 'ctrl + space : open menu
            key(Keys.ControlKey)
            key(Keys.Back)
            ttAdjust()
            Me.ContextMenuStripString.Show(MousePosition)  'key(93)
        End If


        If My.Computer.Keyboard.CtrlKeyDown And GetAsyncKeyState(Keys.V) Then 'ctrl v cb length

            If txtString.Text.StartsWith(":1") Then 'replace line feed with <<space>>
                Clipboard.SetText(Clipboard.GetText.Replace(vbLf, ""))
                Clipboard.SetText(Clipboard.GetText.Replace(Chr(13), "«space»"))
            End If
            If txtString.Text.StartsWith(":2") Then 'replace line feed with <<enter>>
                Clipboard.SetText(Clipboard.GetText.Replace(vbLf, ""))
                Clipboard.SetText(Clipboard.GetText.Replace(Chr(13), "«enter»"))
            End If

            pasteCBLength()


        End If

#Region "tab"


        If GetAsyncKeyState(Keys.Tab) Then 'tab complete
            If GetAsyncKeyState(Keys.LShiftKey) Then Exit Sub

            Select Case txtString.Text
                Case "a-z"
                    ContextMenuStripChkAtoZ.Show(MousePosition)
                    'txtStringClear()
                    Exit Sub
                Case "0-9"
                    ContextMenuStripChk0to9.Show(MousePosition)
                    Exit Sub
                Case "f1-f12"
                    ContextMenuStripf1tof12.Show(MousePosition)
                    Exit Sub
                Case "number pad"
                    ContextMenuStripNumPad.Show(MousePosition)
                    Exit Sub
                Case "misc"
                    ContextMenuStripChkMisc.Show(MousePosition)
                    Exit Sub
                Case "arrows"
                    ContextMenuStripArrows.Show(MousePosition)
                    Exit Sub
                Case "media"
                    ContextMenuStripChkMedia.Show(MousePosition)
                    Exit Sub
                Case "other"
                    RightCtrllToolStripMenuItem.Text = "right ctrl=" & My.Settings.SettingRctrleqMod
                    ContextMenuStripChkOther.Show(MousePosition)
                    Exit Sub
                Case "tips"
                    ContextMenuStripChkTips.Show(MousePosition)
                    txtStringClear()
                    Exit Sub
                Case "on"
                    ContextMenuStripChkOn.Show(MousePosition)
                    txtStringClear()
                    Exit Sub
                Case "db"
                    mnuItemsShow(True)
                    ContextMenuStripDb.Show(MousePosition)
                    txtStringClear()
                    Exit Sub

                Case "dt"
                    tabCompleteTxt("dt", "dbtip")
                    Exit Sub
                Case "ex"
                    tabCompleteTxt("ex", "export")
                    Exit Sub
                Case "sw"
                    tabCompleteTxt("sw", "swipe")
                    Exit Sub
                Case "al"
                    tabCompleteTxt("al", "algorithm")
                    Exit Sub
                Case "b"
                    tabCompleteTxt("b", "browser")
                    Exit Sub
                Case "da"
                    tabCompleteTxt("da", "delete all")
                    Exit Sub
                Case "ed"
                    tabCompleteTxt("ed", "edit")
                    Exit Sub
                Case "d"
                    tabCompleteTxt("d", "db")
                    Exit Sub
                Case "h"
                    tabCompleteTxt("h", "hide")
                    Exit Sub
                Case "l"
                    tabCompleteTxt("l", "length")
                    Exit Sub
                Case "i"
                    tabCompleteTxt("i", "interval")
                    Exit Sub
                Case "r"
                    tabCompleteTxt("r", "restart") 'auto complete wo tagwrap
                    Exit Sub
                Case "e"
                    tabCompleteTxt("e", "exit")
                    Exit Sub
                Case "re"
                    tabCompleteTxt("re", "reload")
                    Exit Sub
                Case "v"
                    tabCompleteTxt("v", "virtual store")
                    Exit Sub
                Case "s"
                    tabCompleteTxt("s", "startup")
                    Exit Sub
                Case "a"
                    tabCompleteTxt("a", "a-z")
                    Exit Sub
                Case "0"
                    tabCompleteTxt("0", "0-9")
                    Exit Sub
                Case "f"
                    tabCompleteTxt("f", "f1-f12")
                    Exit Sub
                Case "fo"
                    tabCompleteTxt("fo", "font")
                    Exit Sub
                Case "n"
                    tabCompleteTxt("n", "number pad")
                    Exit Sub
                Case "m"
                    tabCompleteTxt("m", "misc")
                    Exit Sub
                Case "ar"
                    tabCompleteTxt("ar", "arrows")
                    Exit Sub
                Case "me"
                    tabCompleteTxt("me", "media")
                    Exit Sub
                Case "ot"
                    tabCompleteTxt("ot", "other")
                    Exit Sub
                Case "o"
                    tabCompleteTxt("o", "on")
                    Exit Sub
                Case "of"
                    tabCompleteTxt("of", "off")
                    Exit Sub
                Case "t"
                    tabCompleteTxt("t", "top")
                    Exit Sub
                Case "ti"
                    tabCompleteTxt("ti", "tips")
                    Exit Sub
                Case "h"
                    tabCompleteTxt("h", "help")
                    Exit Sub
                Case "c"
                    tabCompleteTxt("c", "clear")
                    Exit Sub
                Case "co"
                    tabCompleteTxt("co", "config")
                    Exit Sub
                Case "sk"
                    tabCompleteTxt("sk", "skin")
                    Exit Sub

            End Select

            If retap = -1 Then '2nd tab
                retap = 0
                keyRelease(Keys.Tab)
                key(Keys.Back)
                w7(33)

                If txtString.SelectionStart > 1 Then
                    If GetChar(txtString.Text, txtString.SelectionStart) = "," Then
                        shiftHold()
                        key(Keys.OemSemicolon)
                        shiftRelease()
                        Exit Sub
                    End If

                    If IsNumeric(GetChar(txtString.Text, txtString.SelectionStart)) And txtString.Text.IndexOf("»", txtString.SelectionStart) = txtString.SelectionStart Then 'if *# move right
                        key(Keys.Right)
                        Exit Sub
                    End If
                End If


                SendKeys.Send("«»{left}")
                keyClear(Keys.Left)
                retap = 0
                Exit Sub
            End If

            Dim ss = txtString.SelectionStart

            If txtString.Text = "" Or txtString.SelectedText.Length = txtString.Text.Length Or txtString.SelectionStart = 0 Or
                txtString.SelectionStart = txtString.Text.Length Then 'And txtString.Text.Length > 2 Then
                If txtString.SelectionStart >= 1 Then
                    If GetChar(txtString.Text, txtString.SelectionStart) <> "»" Then '>end
                        keyRelease(Keys.Tab)
                        key(Keys.Back)
                        w7(33)
                        SendKeys.Send("«»{left}")
                        keyClear(Keys.Left)
                        Exit Sub
                    Else
                    End If
                Else
                    keyRelease(Keys.Tab)
                    key(Keys.Back)
                    w7(33)
                    SendKeys.Send("«»{left}")
                    keyClear(Keys.Left)
                    Exit Sub
                End If
            End If

            If txtString.SelectionStart >= 1 Then

                If txtString.SelectionStart = txtString.Text.Length And GetChar(txtString.Text, txtString.SelectionStart) = Chr(9) Then 'end
                    keyRelease(Keys.Tab)
                    key(Keys.Back)
                    w7(33)
                    SendKeys.Send("«»{left}")
                    Exit Sub
                End If

                If txtString.SelectionStart < txtString.TextLength Then 'print tab
                    If GetChar(txtString.Text, txtString.SelectionStart) = "«" And
                            GetChar(txtString.Text, txtString.SelectionStart + 1) = "»" Then
                        keyRelease(Keys.Tab)
                        key(Keys.Back)
                        key(Keys.Back)
                        w7(33)
                        key(Keys.Delete)
                        w7(33)
                        key(Keys.Tab)
                        w7(33)
                        Exit Sub
                    End If
                End If


                If GetChar(txtString.Text, txtString.SelectionStart) = "*" Or GetChar(txtString.Text, txtString.SelectionStart) = "" Then 'clear *, move right
                    keyRelease(Keys.Tab)
                    key(Keys.Back)
                    key(Keys.Back)
                    w7(33)
                    key(Keys.Right)
                    w7(33)
                    Exit Sub
                End If

                If IsNumeric(GetChar(txtString.Text, txtString.SelectionStart)) And txtString.Text.IndexOf("»", txtString.SelectionStart) = txtString.SelectionStart Then 'if *# move right
                    timeout2(222)
                    key(Keys.Back)
                    w7(33)
                    key(Keys.Right)
                    Exit Sub
                End If

                If txtString.Text.EndsWith("»") And txtString.SelectionStart = txtString.TextLength Then 'move left one if «» for *or:
                    keyRelease(Keys.Tab)
                    timeout2(222)
                    key(Keys.Back)

                    If txtString.Text.EndsWith("-»" & vbTab) Then
                        key(Keys.Right)
                        w7(33)
                        SendKeys.Send("«»{left}")
                        retap = 0
                        Exit Sub
                    End If

                    If IsNumeric(GetChar(txtString.Text, txtString.SelectionStart - 2)) Or
                           txtString.Text.EndsWith("shift»" & vbTab) Or
                           txtString.Text.EndsWith("alt»" & vbTab) Or
                           txtString.Text.EndsWith("win»" & vbTab) Or
                           txtString.Text.EndsWith("ctrl»" & vbTab) Then
                        key(Keys.Right)
                        key(Keys.Right)
                        w7(33)
                        SendKeys.Send("«»{left}")
                        Exit Sub
                    End If

                    w7(44)
                    SendKeys.Send("«»{left}")
                    keyClear(Keys.Left)

                    Exit Sub
                End If


            End If


            If txtString.TextLength >= 1 And txtString.SelectionStart <> txtString.TextLength And txtString.SelectionStart() > 0 Then
                txtFinish("a", "", False)
                txtFinish("t", "tab", True)
                txtFinish("s", "", False)
                txtFinish("l", "left", True)
                txtFinish("c", "", False)
                txtFinish("r", "right", True)
                txtFinish("u", "up", True)
                txtFinish("d", "down", True)
                txtFinish("n", "note", False)
                txtFinish("e", "enter", True)
                txtFinish("b", "bs", True)
                txtFinish("i", "insert", True)
                txtFinish("p", "pause", True)
                txtFinish("v", "volume-", False)
                txtFinish("w", "", False)
                txtFinish("x", "", True)
                txtFinish("y", "yesno:", False)
            End If

            If txtString.TextLength >= 2 And txtString.SelectionStart <> txtString.TextLength And txtString.SelectionStart() > 0 Then
                txtFinish("wr", "", False)
                txtFinish("ww", "", False)
                txtFinish("mo", "mouse-", False)
                txtFinish("hi", "hide", True)
                txtFinish("al", "", False)
                txtFinish("of", "off", True)
                txtFinish("sh", "", False)
                txtFinish("co", "", False)
                txtFinish("ct", "", False)
                txtFinish("sk", "sendkeys:", False)
                txtFinish("lc", "left-click", True)
                txtFinish("lh", "left-hold", True)
                txtFinish("lr", "left-release", True)
                txtFinish("rc", "right-click", True)
                txtFinish("rh", "right-hold", True)
                txtFinish("rr", "right-release", True)
                txtFinish("mc", "middle-click", True)
                txtFinish("mh", "middle-hold", True)
                txtFinish("mr", "middle-release", True)
                txtFinish("rm", "return-mouse", True)
                txtFinish("nu", "num", True)
                txtFinish("ca", "caps", True)
                txtFinish("sc", "scroll", True)
                txtFinish("ba", "back-space", True)
                txtFinish("ta", "tab", True)
                txtFinish("es", "escape", True)
                txtFinish("in", "insert", True)
                txtFinish("br", "break", True)
                txtFinish("ho", "home", True)
                txtFinish("en", "end", True)
                txtFinish("pd", "page-down", True)
                txtFinish("pu", "page-up", True)
                txtFinish("me", "menu", True)
                txtFinish("de", "delete", True)
                txtFinish("mu", "mute", True)
                txtFinish("vu", "volume-up", True)
                txtFinish("vd", "volume-down", True)
                txtFinish("ps", "print-screen", True)
                txtFinish("cb", "clipboard:", False)
                txtFinish("sa", "stop-audio", True)
                txtFinish("ap", "app:", False)
                txtFinish("we", "web:", False)
                txtFinish("no", "note:", False)
                txtFinish("vo", "volume-", False)
                txtFinish("ig", "ignore-", False)
                txtFinish("ch", "check", True)
                txtFinish("uc", "uncheck", True)
                txtFinish("un", "uncheck", True)
                txtFinish("ti", "time", True)
                txtFinish("da", "date", True)
                txtFinish("dn", "dna", True)
                txtFinish("re", "restart", True)
                txtFinish("ex", "exit", True)
                txtFinish("se", "seconds:", False)
                txtFinish("ms", "milliseconds:", False)
                txtFinish("mi", "milliseconds:", False)
                txtFinish("sl", "sleep:", False)
                txtFinish("to", "timeout:", False)
                txtFinish("wa", "wait:", False)
                txtFinish("pa", "pause:", False)
                txtFinish("mt", "manual-timeout:", False)
                txtFinish("pl", "play-pause", True)
                txtFinish("ri", "right", True)
                txtFinish("le", "left", True)
                txtFinish("pr", "print-screen", True)
                txtFinish("rn", "#", True)
                txtFinish("rnn", "#:-«left»", False)
                txtFinish("rl", "x", True)
                txtFinish("sp", "space", True)
                txtFinish("ls", "", False)
                txtFinish("rs", "", False)
                txtFinish("lw", "", False)
                txtFinish("rw", "", False)
                txtFinish("la", "", False)
                txtFinish("ra", "", False)
                txtFinish("im", "import", False)
                txtFinish("ad", "add", False)
                txtFinish("ed", "edit", False)

                If txtString.SelectionStart() > txtString.TextLength Then 'w7 chk
                    If GetChar(txtString.Text, txtString.SelectionStart()) = "«" And GetChar(txtString.Text, txtString.SelectionStart() + 1) = "»" Then
                        Dim s As Integer = txtString.SelectionStart
                        key(Keys.Back)
                        timeout2(111)
                        Dim t As String = txtString.Text.Remove(txtString.SelectionStart() - 1, 2)
                        txtStringClear()
                        txtString.AppendText(t)
                        timeout2(111)
                        Me.ContextMenuStripString.Show(MousePosition)
                        txtString.SelectionStart = s - 1
                    End If
                End If

                txtFinish("au", "", True)
                txtFinish("ur", "", True)
                txtFinish("xy", "", True)

                txtFinish("ws", "", False)
                txtFinish("wsp", "", False)

            End If

            If txtString.TextLength >= 3 And txtString.SelectionStart <> txtString.TextLength And txtString.SelectionStart() > 0 Then
                txtFinish("cak", "clearallkeys", True)
                txtFinish("uca", "ucase", True)
                txtFinish("lca", "lcase", True)
                txtFinish("url", "", True)
                txtFinish("imc", "ignore-mouse-check", True)
                txtFinish("imu", "ignore-mouse-uncheck", True)
                txtFinish("tim", "time", False)
                txtFinish("dat", "date", False)
                txtFinish("dna", "dna", True)
                txtFinish("bro", "browser", False)
                txtFinish("med", "media-", False)
                txtFinish("mpp", "media-play-pause", True)
                txtFinish("mpt", "media-previous-track", True)
                txtFinish("mnt", "media-next-track", True)
                txtFinish("rep", "replace:|", False)
                txtFinish("ent", "enter", True)
                txtFinish("rel", "release", True)
                txtFinish("sav", "save", True)
                txtFinish("sho", "show", True)
                txtFinish("pau", "pause:", True)
                txtFinish("lco", "", False)
                txtFinish("rco", "", False)
                txtFinish("rct", "", False)
                txtFinish("lct", "", False)
            End If

            Application.DoEvents()
            sleep(1)

            'timeout2(222) 'move right if no case
            If ss = txtString.SelectionStart - 1 Then
                If txtString.TextLength >= 3 Then
                    If txtString.SelectionStart = 1 Then 'tab"txtStrign.text" 
                        If GetChar(txtString.Text, txtString.SelectionStart + 1) = "«" Then
                            keyRelease(Keys.Tab)
                            key(Keys.Back)
                            w7(33)
                            SendKeys.Send("«»{left}")
                        End If
                        Exit Sub
                    End If '
                    If GetChar(txtString.Text, txtString.SelectionStart - 1) = Chr(9) Then
                        key(Keys.Back)
                        sleep(44)
                        SendKeys.Send("«»{left}") '»«
                        keyClear(Keys.Left)
                        keyRelease(Keys.Tab)
                        Exit Sub
                    End If
                    If txtString.SelectionStart = txtString.TextLength Then Exit Sub

                    keyRelease(Keys.Tab)
                    If clear_skmg1 = True Then
                        If GetChar(txtString.Text, txtString.SelectionStart) = vbTab Then key(Keys.Back)
                        clear_skmg1 = False
                    Else
                        key(Keys.Back)
                    End If
                    w7(33)

                    If GetChar(txtString.Text, txtString.SelectionStart - 1) = "»" And GetChar(txtString.Text, txtString.SelectionStart + 1) = "«" Then
                        SendKeys.Send("«»{left}") '»«
                    Else
                        If GetChar(txtString.Text, txtString.SelectionStart - 1) <> "«" And GetChar(txtString.Text, txtString.SelectionStart + 1) = "»" Then
                            If GetChar(txtString.Text, txtString.SelectionStart - 1) = "m" Then
                                SendKeys.Send("+;") ':
                            Else

                                If txtString.Text.EndsWith("-" & vbTab & "»") Then
                                    key(Keys.Right)
                                    retap = -1
                                    Exit Sub
                                End If

                                If txtString.SelectionStart > 1 Then
                                    If GetChar(txtString.Text, txtString.SelectionStart) = vbTab And GetChar(txtString.Text, txtString.SelectionStart - 1) = "," Then
                                        shiftHold()
                                        key(Keys.OemSemicolon)
                                        shiftRelease()
                                        Exit Sub
                                    End If

                                End If
                                shiftHold()
                                key(Keys.D8)
                                shiftRelease()
                            End If
                        Else

                            If clear_skmg1 = True Then
                                clear_skmg1 = False
                            Else
                                If GetChar(txtString.Text, txtString.SelectionStart) = "»" And GetChar(txtString.Text, txtString.SelectionStart + 1) = "«" Then Exit Sub 'ws
                                SendKeys.Send("«»{left}")
                                keyClear(Keys.Left)
                                Exit Sub
                            End If

                        End If
                    End If
                End If
            End If


        End If
#End Region

    End Sub

    Sub pasteCBLength()
        If txtString.SelectionStart = 0 Or txtString.SelectionStart = txtString.TextLength Then Exit Sub
        If GetChar(txtString.Text, txtString.SelectionStart) = "*" And GetChar(txtString.Text, txtString.SelectionStart + 1) = "»" Then 'paste clipboard length
            keyRelease(Keys.ControlKey)
            keyRelease(Keys.V)
            print(Clipboard.GetText.ToString.Length.ToString, False)
            timeout2(44)
            key(Keys.Right)
        End If
    End Sub

    Sub keyBackDel()
        key(Keys.Back)
        key(Keys.Delete)
    End Sub

    Sub txtFinish(shortString As String, longString As String, moveRight As Boolean)
        If txtString.TextLength >= shortString.Length + 2 And txtString.SelectionStart() > shortString.Length Then

            If txtString.TextLength = txtString.SelectionStart Then Exit Sub

            If GetChar(txtString.Text, txtString.SelectionStart() - shortString.Length) = "«" And Microsoft.VisualBasic.Mid(txtString.Text, txtString.SelectionStart() - shortString.Length + 1, shortString.Length) = shortString And GetChar(txtString.Text, txtString.SelectionStart() + 1) = "»" Or
                GetChar(txtString.Text, txtString.SelectionStart() - shortString.Length) = "-" And Microsoft.VisualBasic.Mid(txtString.Text, txtString.SelectionStart() - shortString.Length + 1, shortString.Length) = shortString And GetChar(txtString.Text, txtString.SelectionStart() + 1) = "»" Then

                TextBox1.Text = "'"
                keyRelease(Keys.Tab)
                txtString.Focus()

                w7(33) 'w7 fix

                For i = 0 To shortString.Length
                    key(Keys.Back)
                    w7(33)
                Next

                Select Case shortString
                    Case "rep"
                        keyBackDel
                        print("«replace:|»", False)
                        keyz(Keys.Left, 2)
                    Case "wr"
                        keyBackDel()
                        print("«win»r«-win»«app:run»«enter»", False)
                        keyz(Keys.Left, 7)
                        'l7
                    Case "ww"
                        keyBackDel()
                        print("«win»r«-win»«app:run»", False)
                    Case "c"
                        keyBackDel()
                        CtrlToolStripMenuItem.PerformClick()
                    Case "lco"
                        keyBackDel()
                        skMenuGet1("«lctrl»«-lctrl»{left 8}", "lctrl", "-")
                    Case "rco"
                        keyBackDel()
                        skMenuGet1("«rctrl»«-rctrl»{left 8}", "rctrl", "-")
                    Case "rct"
                        keyBackDel()
                        skMenuGet1("«rctrl»«-rctrl»{left 8}", "rctrl", "-")
                    Case "lct"
                        keyBackDel()
                        skMenuGet1("«lctrl»«-lctrl»{left 8}", "lctrl", "-")
                    Case "lw"
                        keyBackDel()
                        skMenuGet1("«lwin»«-lwin»{left 7}", "lwin", "-")
                    Case "rw"
                        keyBackDel()
                        skMenuGet1("«rwin»«-rwin»{left 7}", "rwin", "-")
                    Case "rs"
                        keyBackDel()
                        skMenuGet1("«rshift»«-rshift»{left 9}", "rshift", "-")
                    Case "ls"
                        keyBackDel()
                        skMenuGet1("«lshift»«-lshift»{left 9}", "lshift", "-")
                    Case "la"
                        keyBackDel()
                        skMenuGet1("«lalt»«-lalt»{left 7}", "lalt", "-")
                    Case "ra"
                        keyBackDel()
                        skMenuGet1("«ralt»«-ralt»{left 7}", "ralt", "-")
                    Case "ws"
                        keyBackDel()
                        skMenuGet1("«ws»«-ws»{left 5}", "ws", "-")
                        clear_skmg1 = True
                    Case "wsp"
                        keyBackDel()
                        print("‹›", False)
                        key(Keys.Left)
                        clear_skmg1 = True
                        retap = -1
                    Case "co"
                        keyBackDel()
                        CtrlToolStripMenuItem.PerformClick()
                    Case "ct"
                        keyBackDel()
                        CtrlToolStripMenuItem.PerformClick()
                    Case "w"
                        keyBackDel()
                        WinToolStripMenuItem.PerformClick()
                    Case "s"
                        keyBackDel()
                        ShiftToolStripMenuItem.PerformClick()
                    Case "sh"
                        keyBackDel()
                        ShiftToolStripMenuItem.PerformClick()
                    Case "a"
                        keyBackDel()
                        AltToolStripMenuItem.PerformClick()
                    Case "al"
                        keyBackDel()
                        AltToolStripMenuItem.PerformClick()
                    Case "alt"
                        keyBackDel()
                        AltToolStripMenuItem.PerformClick()
                    Case "ur"
                        keyBackDel()
                        timeout2(111)
                        UrlToolStripMenuItem.PerformClick()
                    Case "url"
                        keyBackDel()
                        timeout2(111)
                        UrlToolStripMenuItem.PerformClick()
                    Case "x"
                        keyBackDel()
                        timeout2(111)
                        runMousePosition() 'xy
                        reStyle()
                    Case "xy"
                        keyBackDel()
                        timeout2(111)
                        runMousePosition() 'xy
                        reStyle()
                    Case "au"
                        keyBackDel()
                        timeout2(111)
                        AudioToolStripMenuItem.PerformClick()
                    Case Else
                        apisk("x" & longString) '
                        w7(33)
                End Select

                If moveRight = True Then
                    key(Keys.Right)
                    w7(33)
                End If
                clearAllKeys()
                emode()

                Return
            End If
        End If
    End Sub

    Sub txtFinish1(shortString As String, longString As String)

        If txtString.Text = shortString Then
            TextBox1.Text = "'"
            keyRelease(Keys.Tab)
            txtString.Focus()
            w7(33)

            For i = 0 To shortString.Length
                key(Keys.Back)
                w7(33)
            Next

            Select Case shortString
                Case shortString
                    print(longString, False)
            End Select

            clearAllKeys()
            TextBox1.Text = ""

            Return
        End If
        emode()
    End Sub

    Sub txtStringClear()
        txtString.SelectAll() : txtString.SelectedText = ""
    End Sub
    Sub keyClear(key As Keys)
        GetAsyncKeyState(key)
    End Sub
    Sub keyRelease(key As Keys)
        keybd_event(key, 0, &H2, 0)
    End Sub
    Sub keyHold(key As Keys)
        keybd_event(key, 0, 0, 0)
    End Sub

    Sub keyClear(brick As String)
        For i = 1 To brick.Length
            Dim b As String = GetChar(brick, i)
        Next
    End Sub

    Sub import1()
        Call GetAsyncKeyState(Keys.LControlKey) 'clear ctrl key
        keybd_event(Keys.LControlKey, 0, &H2, 0) '

        If txtString.Text = "" Then
            If chk_tips.CheckState = CheckState.Checked Then MsgBox("nothing to import from text box" & vbNewLine & " example:" & vbNewLine & "test  Hi!" & vbNewLine & "tes2  Hello" & vbNewLine & "tes3  Peace", vbInformation)
            txtString.Focus()
            Exit Sub
        End If

        Me.Text = "dna > importing" 'caption

        Dim a As String = txtString.Text 'count 
        Dim pattern As String = My.Settings.SettingExportNewLine
        Dim ex As New System.Text.RegularExpressions.Regex(pattern)
        Dim m As System.Text.RegularExpressions.MatchCollection
        m = ex.Matches(a)

        If m.Count = 0 And txtString.Text > "" Then
            'import lines
            For i = 0 To txtString.Lines.Length - 1 'get num of lines in txtbox
                If GetAsyncKeyState(Keys.Escape) Then Exit For
                Dim x As String = txtString.Lines(i)
                Me.Text += "." 'animate me.txt 
                If Len(Me.Text) > 60 Then Me.Text = "dna > importing" 'caption reset
                ListBox1.Items.Add(x) 'add item to listbox
                My.Settings.Settingdb.Add(x) 'add item to settingsDb
            Next
        ElseIf m.Count > 0 And txtString.Text > "" Then
            For i = 1 To m.Count  'import
                If GetAsyncKeyState(Keys.Escape) Then Exit For
                Dim charRange = My.Settings.SettingExportNewLine
                Dim startIndex As Integer = txtString.Text.IndexOf(charRange)
                Dim itm As String = (Microsoft.VisualBasic.Left(txtString.Text, startIndex))
                Me.Text += "." 'animate me.txt 
                If Len(Me.Text) > 60 Then Me.Text = "dna > importing" 'caption reset
                ListBox1.Items.Add(itm)
                My.Settings.Settingdb.Add(itm)
                txtString.Text = txtString.Text.Replace((itm & My.Settings.SettingExportNewLine & vbLf), "")
            Next

        End If

        dnaTxt()
        txtStringClear() 'clear

        'reloadDb()
        ListBox1.SelectedItem() = ListBox1.Items.Item(ListBox1.Items.Count - 1) 'select last item

        reStyle()
        emode()
    End Sub
    Sub trimLastLf()
        If txtString.Text.EndsWith(vbLf) Then txtString.Text = Microsoft.VisualBasic.Left(txtString.Text, txtString.TextLength - 1)
    End Sub

    Sub import2(textbox As RichTextBox, listbox As ListBox)
        trimLastLf()
        import1()
    End Sub
    Private Sub txtString_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtString.KeyPress
        'If Me.Height <= 39 Then Exit Sub
        'Clipboard.SetText(e.KeyChar) 'copy ctrl s etc..
        If GetAsyncKeyState(Keys.LShiftKey) Then 'chkon toggle
            If GetAsyncKeyState(Keys.Escape) Then
                keyRelease(Keys.LShiftKey)
                keyRelease(Keys.Escape)
                If chk_timer1_on_val.Checked = False Then
                    tc = TabPage3.BackColor
                    TabPage3.BackColor = Color.Lime
                    chk_timer1_on_val.Checked = True
                    timeout1(1)
                    TabPage3.BackColor = tc
                    emode()
                    Exit Sub
                End If
                If chk_timer1_on_val.Checked = True Then
                    chk_timer1_on_val.Checked = False
                    If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable And Me.ControlBox = False Then 'appearance 7
                        sizeable()
                        My.Settings.SettingMoveBar = True 'reshow move bar
                        moveable()
                    End If
                    txtString.Focus()

                    tc = TabPage3.BackColor
                    If chk_timer1_on_val.Checked = True Then
                        TabPage3.BackColor = Color.Lime
                    Else
                        TabPage3.BackColor = Color.Red
                    End If
                    timeout1(1)
                    TabPage3.BackColor = tc

                    emode()
                    If chk_timer1_on_val.Checked = False Then dnaTxt()
                    Exit Sub
                End If
            End If
        End If

        'changeInterval
        If GetAsyncKeyState(Keys.Escape) And GetAsyncKeyState(Keys.I) Then
            keyRelease(Keys.Escape)
            keyRelease(Keys.I)
            changeInterval()
            key(Keys.Back)
        End If

        If NoLengthToolStripMenuItem.Checked = False Or TextBox1.Text.StartsWith("«") Then
            'timer1 #added  And txtString.ContainsFocus = False 
            If GetAsyncKeyState(Keys.I) And IToolStripMenuItem.Checked = False And chkAz.Checked = False Then TextBox1.Text += "i"
            If GetAsyncKeyState(Keys.L) And LToolStripMenuItem.Checked = False And chkAz.Checked = False Then TextBox1.Text += "l"
            If GetAsyncKeyState(Keys.P) And PToolStripMenuItem.Checked = False And chkAz.Checked = False Then TextBox1.Text += "p"

            'If GetAsyncKeyState(Keys.F1) And F1ToolStripMenuItem.Checked = False And chkF1f12.Checked = False Then TextBox1.Text += "!"

            If GetAsyncKeyState(Keys.D0) And ToolStripMenuItem14.Checked = False And chk09.Checked = False Then TextBox1.Text += "0"
            If GetAsyncKeyState(Keys.D1) And ToolStripMenuItem15.Checked = False And chk09.Checked = False Then TextBox1.Text += "1"
            If GetAsyncKeyState(Keys.D2) And ToolStripMenuItem16.Checked = False And chk09.Checked = False Then TextBox1.Text += "2"
            If GetAsyncKeyState(Keys.D3) And ToolStripMenuItem17.Checked = False And chk09.Checked = False Then TextBox1.Text += "3"
            If GetAsyncKeyState(Keys.D4) And ToolStripMenuItem18.Checked = False And chk09.Checked = False Then TextBox1.Text += "4"
            If GetAsyncKeyState(Keys.D5) And ToolStripMenuItem19.Checked = False And chk09.Checked = False Then TextBox1.Text += "5"
            If GetAsyncKeyState(Keys.D6) And ToolStripMenuItem20.Checked = False And chk09.Checked = False Then TextBox1.Text += "6"
            If GetAsyncKeyState(Keys.D7) And ToolStripMenuItem21.Checked = False And chk09.Checked = False Then TextBox1.Text += "7"
            If GetAsyncKeyState(Keys.D8) And ToolStripMenuItem22.Checked = False And chk09.Checked = False Then TextBox1.Text += "8"
            If GetAsyncKeyState(Keys.D9) And ToolStripMenuItem23.Checked = False And chk09.Checked = False Then TextBox1.Text += "9"

            If GetAsyncKeyState(Keys.Space) And SpaceToolStripMenuItem.Checked = False And chkOther.Checked = False Then TextBox1.Text += " "
        End If

        'change length sk
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D1) Then
            txtLength.Text = "1"
            keyRelease(Keys.L)
            keyRelease(Keys.D1)
            key(Keys.Back)
            key(Keys.Back)
        End If
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D2) Then
            txtLength.Text = "2"
            keyRelease(Keys.D2)
            key(Keys.Back)
            key(Keys.Back)
        End If
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D3) Then
            txtLength.Text = "3"
            keyRelease(Keys.D3)
            key(Keys.Back)
            key(Keys.Back)
        End If
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D4) Then
            txtLength.Text = "4"
            keyRelease(Keys.D4)
            key(Keys.Back)
            key(Keys.Back)
        End If
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D5) Then
            txtLength.Text = "5"
            keyRelease(Keys.D5)
            key(Keys.Back)
            key(Keys.Back)
        End If
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D6) Then
            txtLength.Text = "6"
            keyRelease(Keys.D6)
            key(Keys.Back)
            key(Keys.Back)
        End If
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D7) Then
            txtLength.Text = "7"
            keyRelease(Keys.D7)
            key(Keys.Back)
            key(Keys.Back)
        End If
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D8) Then
            txtLength.Text = "8"
            keyRelease(Keys.D8)
            key(Keys.Back)
            key(Keys.Back)
        End If
        If GetAsyncKeyState(Keys.L) And GetAsyncKeyState(Keys.D9) Then
            txtLength.Text = "9"
            keyRelease(Keys.D9)
            key(Keys.Back)
            key(Keys.Back)
        End If

        'browser address box
        If GetAsyncKeyState(Keys.Return) Then
            If TabPage3.Text = "browser" Or GetAsyncKeyState(Keys.LControlKey) And WebBrowser1.Visible = True Then
                keyRelease(Keys.Return)
                apisk(" " & "«bs»«m»")
                Try
                    WebBrowser1.Navigate(txtString.Text)
                    dnaTxt()
                Catch ex As Exception
                End Try
                Exit Sub
            End If
        End If

        'print mouse x y coordinates
        If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.P) Then
            keybd_event(Keys.LControlKey, 0, &H2, 0)
            keybd_event(Keys.P, 0, &H2, 0)
            txtString.Focus()
            If My.Settings.SettingChkW7 = True Then 'w7
                print("«", False)
                w7(33)
                print("xy:" & lblX.Text & "-" & lblY.Text & "»", False)
            Else
                print("«xy:" & lblX.Text & "-" & lblY.Text & "»", False)
            End If
        End If 'print mouse x y coordinates
        If GetAsyncKeyState(Keys.RControlKey) And GetAsyncKeyState(Keys.P) Then
            keybd_event(Keys.RControlKey, 0, &H2, 0)
            keybd_event(Keys.P, 0, &H2, 0)
            txtString.Focus()
            print("«", False)
            timeout1(1)
            print("xy:" & lblX.Text & "-" & lblY.Text & "»", False)
        End If

        'print <<>>
        If My.Computer.Keyboard.CtrlKeyDown Then
            If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.Enter) Then
                keyRelease(Keys.LControlKey)
                keyRelease(Keys.Enter)
                SendKeys.Send("{bs}")

                w7(33) 'w7 fix

                Dim cl As Integer = txtString.SelectionStart 'get text cursor location
                If txtString.TextLength >= 2 And cl + 1 <= txtString.TextLength And cl >= 2 Then 'print<<*>> *
                    If GetChar(txtString.Text, cl + 1) = "»" And GetChar(txtString.Text, cl) = vbLf And Not GetChar(txtString.Text, cl - 1) = "*" And Not GetChar(txtString.Text, cl - 1) = ":" Then
                        SendKeys.Send("+8") '*
                        Exit Sub
                    End If
                    If GetChar(txtString.Text, cl + 1) = "»" And GetChar(txtString.Text, cl) = vbLf And GetChar(txtString.Text, cl - 1) = "*" Then
                        SendKeys.Send("{bs}+;") '*
                        Exit Sub
                    End If

                    If cl >= 7 Then 'print strand #
                        If cl - 7 = 0 Then '7.11.14
                            SendKeys.Send("«»{left}")
                            Exit Sub
                        End If '

                    End If

                    If GetChar(txtString.Text, cl + 1) = "»" And GetChar(txtString.Text, cl) = vbLf And GetChar(txtString.Text, cl - 1) = ":" Then
                        SendKeys.Send("{bs}+8") ':
                        Exit Sub
                    End If

                End If

                SendKeys.Send("«»{left}") 'print <<>>
                'w7(33) 'w7 fix

            End If
        End If

        If GetAsyncKeyState(Keys.RControlKey) And GetAsyncKeyState(Keys.Enter) Then 'run code
            keybd_event(Keys.RControlKey, 0, &H2, 0)
            keybd_event(Keys.Enter, 0, &H2, 0)
            key(Keys.Back)
            timeout2(33)
            If My.Settings.SettingChkRCtrlEnterRun = True Then
                runCode()
            Else
                SendKeys.Send("«»") 'print <<>>
                timeout2(222)
                key(Keys.Left)
            End If
            Exit Sub
        End If

        If Microsoft.VisualBasic.Right(txtString.Text, 5) = ".add" & vbLf Then 'temp add
            ListBox1.Items.Add(Microsoft.VisualBasic.Left(txtString.Text, txtString.TextLength - 5))
            txtStringClear()
            selectBottomItem()
            Exit Sub
        End If
        If Microsoft.VisualBasic.Right(txtString.Text, 8) = ".import" & vbLf Then 'temp import
            txtString.Text = Microsoft.VisualBasic.Left(txtString.Text, txtString.TextLength - 8) 'text w/o .import
            If txtString.Text = ".import" & vbLf Then Exit Sub
            import2(txtString, ListBox1)
            txtStringClear()
            selectBottomItem()
            Exit Sub
        End If


        If GetAsyncKeyState(Keys.Enter) And txtString.TextLength <= 14 Then
            '!temp db. with «tag»
            dbCode("import") 'txtString.text«import(enter)»
            dbCode("i")
            dbCode("add")
            dbCode("a")
            dbCode("edit")
            dbCode("e")
            dbCode("update")
            dbCode("u")

            'no tag
            dbCode1("ato") 'app timed out
            dbCode1("si") 'sizeable
            dbCode1("dbtip") 'db tab dna > tip
            dbCode1("op") 'opacity
            dbCode1("export")
            dbCode1("rf") 'resetfont changefont(f
            dbCode1("s") 'shrink
            dbCode1("ic") 'settingicon
            dbCode1("cc") 'settingchangecolor
            dbCode1("v") 'changeview
            dbCode1("cv") 'changeview
            dbCode1("o") 'open
            dbCode1("c") 'close
            dbCode1("font")
            dbCode1("x") 'exit
            dbCode1("sl") 'SettingScrollLockRun
            dbCode1("ml") 'dna > SettingMaxKeyLen
            dbCode1("od") 'onedrive
            dbCode1("odd") 'onedrivedir
            dbCode1("ar") 'auto retry app:
            dbCode1("dna") 'user config
            dbCode1("bg") 'bg img
            dbCode1("ws") 'ignore white space
            dbCode1("nl") 'no length run mode
            dbCode1("swipe") 'swipe menu hide/show
            dbCode1("algorithm") 'algorithm menu hide/show
            dbCode1("al") 'auto lock (dna > »)
            dbCode1("cb") 'remember clipboard
            dbCode1("h") 'hide tabs
            dbCode1("ht") 'hide tabs
            dbCode1("browser")
            dbCode1("wb") '.html , .htm main gb img effect
            dbCode1("osk") 'ignore > other > osk 
            dbCode1("gc") 'gcollect
            dbCode1("gc:") 'gcollect toggle
            dbCode1("?")
            dbCode1("delete all")
            dbCode1("e") 'edit
            dbCode1("ed")
            dbCode1("edit")
            dbCode1("t") 'select top item in db
            dbCode1("b") 'select bottom item
            dbCode1("d") 'select db
            dbCode1("db") 'select db
            dbCode1("tips")
            dbCode1("interval")
            dbCode1("length")
            dbCode1("virtual store")
            dbCode1("config")
            dbCode1("a-z") 'toggle ignore options
            dbCode1("0-9")
            dbCode1("f1-f12")
            dbCode1("number pad")
            dbCode1("np")
            dbCode1("misc")
            dbCode1("arrows")
            dbCode1("media")
            dbCode1("other") '
            dbCode1("hide") 'stay hidden after run
            dbCode1("clear") 'clear db
            dbCode1("reload") 'reloadDb
            dbCode1("startup") 'setup startup path
            dbCode1("help") 'legend
            dbCode1("skin") 'customize
            dbCode1("w7") 'Windows7
            dbCode1("ex") 'drag to extended screen
            dbCode1("sh") 'stay hidden after middle click item run
            dbCode1("cr") 'click run
            dbCode1("re") 'rctrl + enter run
            dbCode1("ep") 'esc + period run
            dbCode1("ei") 'esc + insert run
            dbCode1("rr") 'rclick run
            dbCode1("dc") 'dbl click run
            dbCode1("mc") 'middle click run
            dbCode1("sw") 'swipe run
            dbCode1("close") 'close
            dbCode1("exit") 'close
            dbCode1("top") 'toggle top
            dbCode1("off") 'engine off
            dbCode1("on") 'engine on
            dbCode1("ww") 'wordwrap
            dbCode1("ts") 'touch screen
            dbCode1("restart") 'restart program

        End If

        If e.KeyChar = ChrW(22) Then  'ctrl + v   cb
            If Not Clipboard.ContainsText Then Exit Sub
            Try
                If GetChar(txtString.Text, txtString.SelectionStart) = "*" And GetChar(txtString.Text, txtString.SelectionStart + 1) = "»" Then Exit Sub 'keydown -> paste cb len
            Catch ex As Exception
            End Try

            If Clipboard.GetText > "" Then Clipboard.SetText(Clipboard.GetText.ToString) 'double paste, raw txt
            txtString.Undo()
            txtString.Paste()
            txtString.Font = ListBox1.Font 'color refresh
        End If

        If e.KeyChar = ChrW(21) Then   'ctrl + u   update / save
            dbToUpdate()
        End If
        If e.KeyChar = ChrW(19) Then 'ctrl + s   save / add
            addDbItm()
        End If
        If e.KeyChar = ChrW(5) Then 'ctrl + e   edit
            'SendKeys.Send("^z")
            keybd_event(Keys.LControlKey, 0, 0, 0)
            keybd_event(Keys.Z, 0, 0, 0)
            keybd_event(Keys.Z, 0, &H2, 0)
            keybd_event(Keys.LControlKey, 0, &H2, 0)
            timeout1(0.1)
            editDbItm()
            txtString.Font = ListBox1.Font 'color refresh
        End If

        If e.KeyChar = ChrW(6) Then  'ctrl + f   'search
            If txtString.Text = "" Then Exit Sub

            If ListBox1.Items.Count = 0 Or SplitContainer1.SplitterDistance <= 1 Then 'search text
                If txtString.SelectionStart = txtString.Text.Length Then txtString.SelectionStart = 0
                If txtString.SelectedText > "" Then x = txtString.SelectedText Else x = InputBox("find text:", "ctrl + f", txtString.SelectedText)
                If x > "" Then
                    Dim pattern As String = x
                    Dim ex As New System.Text.RegularExpressions.Regex(pattern)
                    Dim m As System.Text.RegularExpressions.MatchCollection
                    y = (Microsoft.VisualBasic.Right(txtString.Text, txtString.Text.Length - txtString.SelectionStart))
                    m = ex.Matches(y)
                    If m.Count = 1 And txtString.SelectedText > "" Then
                        txtString.SelectionStart = 0 'on last go to start
                    Else
                        If m.Count = 0 Then Exit Sub ''
                        txtString.SelectionStart += pattern.Length 'next match
                        txtString.SelectionLength = 0
                    End If
                    y = (Microsoft.VisualBasic.Right(txtString.Text, txtString.TextLength - txtString.SelectionStart)) 'update ->
                    txtString.SelectionStart += y.IndexOf(pattern) 'select match
                    txtString.SelectionLength = pattern.Length
                End If
                Exit Sub
            End If
            'search list
            If txtString.SelectedText > "" Then re = txtString.SelectedText Else re = txtString.Text
            If ListBox1.SelectedItem = Nothing And ListBox1.Items.Count > 0 Then ListBox1.SelectedIndex = 0
            If ListBox1.SelectedItem.Contains(re) And ListBox1.SelectedIndex <> ListBox1.Items.Count - 1 Then
                ListBox1.SelectedIndex += 1 'next
                searchResults()
            Else
                ListBox1.SelectedIndex = 0 'begin
                searchResults()
            End If
            If ListBox1.SelectedIndex = 0 And ListBox1.SelectedItem.ToString().Contains(re) Then
                searchResults() 'if in 1
                Exit Sub
            End If
            For q = ListBox1.SelectedIndex To ListBox1.Items.Count - 1 'scan
                If GetAsyncKeyState(Keys.Pause) Or GetAsyncKeyState(Keys.Escape) Then Exit Sub
                If ListBox1.Items.Item(q).Contains(re) Then
                    ListBox1.SelectedIndex = q
                    searchResults()
                    Exit For
                End If
            Next
        End If '/search
    End Sub
    Sub searchResults()
        If txtString.SelectedText > "" Then re = txtString.SelectedText Else re = txtString.Text
        If ListBox1.SelectedItem.ToString().Contains(re) Then
            tc = TabPage3.BackColor
            TabPage3.BackColor = Color.Lime
            Application.DoEvents()
            System.Threading.Thread.Sleep(701)
            TabPage3.BackColor = tc
        Else
            tc = TabPage3.BackColor
            TabPage3.BackColor = Color.Red
            Application.DoEvents()

            System.Threading.Thread.Sleep(701)
            TabPage3.BackColor = tc
        End If
    End Sub

    Sub dbToUpdate()
        keyRelease(Keys.U)
        keyRelease(Keys.LControlKey)
        If ListBox1.SelectedIndex < 0 Or txtString.Text = "" Then Exit Sub 'nothing slected; exit
        Dim r As Integer = txtString.SelectionStart
        Dim ls As String = txtString.Text
        Dim dbR As Integer = ListBox1.SelectedIndex
        txtStringClear()
        txtString.AppendText(ListBox1.Text)
        txtStringClear()
        txtString.AppendText(ls)

        If ListBox1.Items.Count <> My.Settings.Settingdb.Count Or DeleteAllToolStripMenuItem.Text = "reload" Then 'in clear mode / db was cleared
            ListBox1.Items.RemoveAt(dbR) 'temp update
            ListBox1.Items.Insert(dbR, ls)
        Else
            ListBox1.Items.RemoveAt(dbR) 'master update
            My.Settings.Settingdb.RemoveAt(dbR)
            ListBox1.Items.Insert(dbR, ls)
            My.Settings.Settingdb.Insert(dbR, ls)
        End If

        ListBox1.SelectedItem() = ListBox1.Items.Item(dbR) 're select item
        txtString.SelectionStart = r
    End Sub
    Dim ttop As Integer

    Private Sub ListBox1_Click(sender As Object, e As EventArgs) Handles ListBox1.Click
        My.Settings.SettingLastListIndex = ListBox1.SelectedIndex '$

        If txtString.Text = "d" Then
            If tipsDeleteToolStripMenuItem2.Checked = True Then tipsDeleteToolStripMenuItem2.Checked = False
            deleteDbItm()
            tipsDeleteToolStripMenuItem2.Checked = My.Settings.SettingTipsDelete
            Exit Sub
        End If

        'click to run
        If My.Settings.SettingRunClick = True Then
            If My.Settings.SettingMulti = False Then leftrelease()
            runList()
            Exit Sub
        End If '


        c = 0
        If Microsoft.VisualBasic.Right(txtString.Text, 7) = ".delete" Then 'temp delete
            ListBox1.Items.RemoveAt(ListBox1.SelectedIndex)
            txtStringClear()
            Exit Sub
        End If
        If Microsoft.VisualBasic.Left(txtString.Text, 1) = ">" Then 'extract list item to txt
            If ListBox1.Text > "" Then txtString.Text += ListBox1.Text
            txtString.SelectionStart = txtString.TextLength
            txtString.Focus()
            Exit Sub
        End If

        If Microsoft.VisualBasic.Left(txtString.Text, 1) = "<" And txtString.TextLength > 1 Then 'insert txt to list
            If txtString.Text = "" Then Exit Sub
            txtString.Text = Microsoft.VisualBasic.Right(Microsoft.VisualBasic.Right(txtString.Text, txtString.TextLength - 1), txtString.TextLength) 'text w/o .add

            If ListBox1.SelectedIndex = ListBox1.Items.Count - 1 And ListBox1.Items.Count <> 0 Then
                ListBox1.Items.Insert(ListBox1.Items.Count - 1, txtString.Text)
                My.Settings.Settingdb.Insert(ListBox1.Items.Count - 2, txtString.Text)
                txtStringClear()
                Exit Sub
            End If

            If ListBox1.SelectedIndex = ListBox1.Items.Count - 1 Or ListBox1.SelectedIndex = -1 Then
                ListBox1.Items.Add(txtString.Text)
                My.Settings.Settingdb.Add(txtString.Text)
                txtStringClear()
                Exit Sub
            End If
            If ListBox1.SelectedIndex <= 0 Then
                ListBox1.Items.Insert(ListBox1.SelectedIndex, txtString.Text)
                My.Settings.Settingdb.Insert(ListBox1.SelectedIndex - 1, txtString.Text)
                txtStringClear()
                Exit Sub
            End If
            ListBox1.Items.Insert(ListBox1.SelectedIndex, txtString.Text)
            My.Settings.Settingdb.Insert(ListBox1.SelectedIndex - 1, txtString.Text)
            txtStringClear()
            Exit Sub
        End If

        If Microsoft.VisualBasic.Right(txtString.Text, 5) = ".edit" Then 'temp edit
            txtStringClear()
            txtString.Text = ListBox1.SelectedItem
            ListBox1.Items.RemoveAt(ListBox1.SelectedIndex)
            Exit Sub
        End If
        If Microsoft.VisualBasic.Right(txtString.Text, 4) = ".add" Then 'temp add
            txtString.Text = Microsoft.VisualBasic.Left(txtString.Text, txtString.TextLength - 4) 'text w/o .add
            If ListBox1.SelectedIndex = ListBox1.Items.Count - 1 Or ListBox1.SelectedIndex = -1 Then
                ListBox1.Items.Add(txtString.Text)
                txtStringClear()
                Exit Sub
            End If
            If ListBox1.SelectedIndex <= 0 Then
                ListBox1.Items.Insert(ListBox1.SelectedIndex, txtString.Text)
                txtStringClear()
                Exit Sub
            End If
            ListBox1.Items.Insert(ListBox1.SelectedIndex, txtString.Text)
            txtStringClear()
            Exit Sub
        End If

    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        c = 0

        ttAdjust()
        mnuItemsShow(False) 'hide import xport mnu items

        If GetAsyncKeyState(Keys.LControlKey) Then
            mnuItemsShow(True) 'show import export mnu items
            Me.ContextMenuStripDb.Show(MousePosition) 'popupmenu db    
            Exit Sub
        End If

        'dblclick to run item
        If My.Settings.SettingRunDblClick = True Then runList()
    End Sub

    Private Sub ListBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles ListBox1.KeyPress
        If GetAsyncKeyState(Keys.Enter) Then 'run
            runList()
            ListBox1.Focus()
            Exit Sub
        End If

        If GetAsyncKeyState(Keys.LControlKey) Then
            Dim s = ListBox1.SelectedIndex 'reselect
            If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.C) Then 'copy
                Clipboard.SetText(ListBox1.SelectedItem)
                timeout1(1)
                ListBox1.SelectedIndex = s 'reselect
            End If
            If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.E) Then 'db item to text 
                txtString.SelectAll()
                If ListBox1.Items.Count >= 1 Then txtString.SelectedText = (ListBox1.SelectedItem)
                If SplitContainer1.Height - SplitContainer1.SplitterDistance <= 40 Then SplitContainer1.SplitterDistance = SplitContainer1.Height / 2 'show if can't see
                timeout1(1)
                ListBox1.SelectedIndex = s 'reselect
            End If

            If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.U) Then 'db update item to text 
                dbToUpdate()
                timeout1(1)
                ListBox1.SelectedIndex = s 'reselect
            End If


            If GetAsyncKeyState(Keys.LControlKey) And GetAsyncKeyState(Keys.Space) Then 'db menu
                mnuItemsShow(False)
                Me.ContextMenuStripDb.Show(MousePosition) 'popupmenu db    
            End If
        End If
    End Sub

    Private Sub ListBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles ListBox1.KeyUp
        If GetAsyncKeyState(Keys.F1) Then 'f1 tt
            f1tt()
            Exit Sub
        End If

        If GetAsyncKeyState(Keys.Down) Then '$
            My.Settings.SettingLastListIndex = ListBox1.SelectedIndex
            Exit Sub
        End If
        If GetAsyncKeyState(Keys.Up) Then
            My.Settings.SettingLastListIndex = ListBox1.SelectedIndex
            Exit Sub
        End If

        If GetAsyncKeyState(93) Then ttAdjust() 'mnu btn pressed then tool tip adjust

        If (e.KeyCode) = 46 Then deleteDbItm()

        If GetAsyncKeyState(Keys.F4) Then 'f4 clear txt
            txtStringClear()
        End If
        If GetAsyncKeyState(Keys.F5) Then 'f5 run
            keyRelease(Keys.F5)
            System.Threading.Thread.Sleep(101)
            runList()
        End If

        If GetAsyncKeyState(Keys.Right) Then
            My.Settings.SettingLastListIndex = ListBox1.SelectedIndex
            If GetAsyncKeyState(Keys.LWin) Then Exit Sub
            selectBottomItem()
            Exit Sub
        End If
        If GetAsyncKeyState(Keys.Left) Then
            My.Settings.SettingLastListIndex = ListBox1.SelectedIndex '$
            If GetAsyncKeyState(Keys.LWin) Then Exit Sub
            selectTopItem()
            Exit Sub
        End If
    End Sub

    Private Sub ListBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Middle And My.Settings.SettingRunMiddleClickInit = True Then 'middle click run
            keyRelease(Keys.MButton)
            leftclick()
            timeout2(44)
            runList()
            If My.Settings.SettingRunStayHidden = True Then Me.Hide()
            Exit Sub
        End If
        mnuItemsShow(False)
        GetAsyncKeyState(Keys.LControlKey) 'clear
        If MouseButtons = Windows.Forms.MouseButtons.Right Then 'if button = 2 then
            'rclick run
            If My.Settings.SettingRunRClick = True Then
                leftclick()
                timeout2(44)
                runList()
                Exit Sub
            End If '
            Dim rz = txtString.ZoomFactor ' = rz 're-zoom
            If GetAsyncKeyState(Keys.LControlKey) And txtString.Text > "" Then
                txtString.SelectAll()
                txtString.SelectedText = ""
            End If
            ttAdjust()
            txtString.ZoomFactor = rz 're-zoom
            Me.ContextMenuStripDb.Show(MousePosition) 'popupmenu db    
        End If
    End Sub


    Private Sub txtString_KeyUp(sender As Object, e As KeyEventArgs) Handles txtString.KeyUp
        If txtString.Text.StartsWith(">sk") Then ' >sk special key
            If txtString.Text.Length = 3 Then
                If GetAsyncKeyState(Keys.LControlKey) Or GetAsyncKeyState(Keys.RControlKey) Or
                   Not GetAsyncKeyState(Keys.LControlKey) Or Not GetAsyncKeyState(Keys.RControlKey) Then txtString.AppendText(":")
            Else
                Select Case e.KeyValue
                    Case 17
                        My.Settings.SettingSpecialKey = Keys.RControlKey
                    Case Else
                        If IsNumeric(e.KeyValue) Then My.Settings.SettingSpecialKey = e.KeyValue
                End Select
                clearTxtString()
            End If
            Exit Sub
        End If

        If GetAsyncKeyState(93) Then ttAdjust() 'mnu btn pressed then tool tip adjust

        'f5 run
        If GetAsyncKeyState(Keys.F5) Then
            keyRelease(Keys.F5)
            timeout2(33)
            runCode()
        End If


        If GetAsyncKeyState(Keys.F1) Then 'copy txt
            pasteCBLength()
        End If
        If GetAsyncKeyState(Keys.F2) Then 'copy txt
            If txtString.Text > "" Then If txtString.TextLength > 0 And txtString.SelectedText.Length > 0 Then Clipboard.SetText(txtString.SelectedText) Else Clipboard.SetText(txtString.Text)
        End If
        If GetAsyncKeyState(Keys.F3) Then 'paste txt
            txtString.Paste()
        End If
        If GetAsyncKeyState(Keys.F4) Then 'clear txt
            txtStringClear()
        End If

        'mod
        If GetAsyncKeyState(Keys.RControlKey) And RightCtrllToolStripMenuItem.Checked = True Then
            keybd_event(Keys.RControlKey, 0, &H2, 0)
            If My.Settings.SettingRctrleqMod = "«" Then
                TextBox1.Clear() 'if v2
                Me.TextBox1.Text = My.Settings.SettingRctrleqMod.ToString
            Else
                Me.TextBox1.Text += My.Settings.SettingRctrleqMod.ToString
            End If
        End If

    End Sub

    Private Sub txtString_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles txtString.LinkClicked
        leftrelease()
        apisk("x«win»r«-win»«sleep:555»" + e.LinkText.ToString + "«enter»")
        clearAllKeys()
        emode()
    End Sub

    Private Sub txtString_MouseDown1(sender As Object, e As MouseEventArgs) Handles txtString.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then 'if button = 2 then
            dz = 0
            ttAdjust()
            txtString.Focus()
            If GetAsyncKeyState(Keys.LControlKey) Then
                mnuItemsShow(False)
                Me.ContextMenuStripDb.Show(MousePosition)
                Exit Sub
            End If
            Me.ContextMenuStripString.Show(MousePosition) 'popupmenu db
        End If


        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            '< or > remove
            If txtString.Text > "" Then
                If txtString.SelectionStart = 0 Or txtString.SelectionStart = 1 Then
                    If txtString.Text = "d" Or txtString.Text.StartsWith("<") Or txtString.Text.StartsWith(">") Or txtString.Text.StartsWith(":1") Or txtString.Text.StartsWith(":2") Then
                        If txtString.Text = "" Then Exit Sub

                        Dim p As Integer = txtString.SelectionStart
                        Dim t As String = txtString.Text
                        Dim x As Boolean = False, y As Boolean = False

                        'If t = "" Then Exit Sub
                        Select Case GetChar(t, 1)
                            Case "d"
                                t = Replace(t, "d", "", 1, 1)
                                y = True
                            Case ">"
                                t = Replace(t, ">", "", 1, 1)
                                y = True
                            Case "<"
                                t = Replace(t, "<", "", 1, 1)
                                y = True
                            Case Else
                                y = False
                        End Select


                        Select Case Microsoft.VisualBasic.Left(t, 2)
                            Case ":1"
                                t = Replace(t, ":1", "", 1, 1)
                                x = True
                            Case ":2"
                                t = Replace(t, ":2", "", 1, 1)
                                x = True
                            Case Else
                                x = False
                        End Select

                        If x = True Or y = True Then
                            txtStringClear()
                            txtString.AppendText(t)
                            txtString.SelectionStart = p
                            Exit Sub
                        End If
                    End If
                End If
            End If
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Left And My.Computer.Keyboard.CtrlKeyDown Then  'quick repeat ctrl + doubleclick
            If Microsoft.VisualBasic.Left(NullToolStripMenuItem1.Text, 3) = "«xy" Then 'repeat xy
                leftrelease()
                keybd_event(Keys.LControlKey, 0, &H2, 0)
                print("«xy:" & lblX.Text & "-" & lblY.Text & "»", False)
                NullToolStripMenuItem1.Text = "«xy"
                timeout2(111)
                Exit Sub
            End If
            keybd_event(Keys.LControlKey, 0, &H2, 0)
            leftrelease()

            If NullToolStripMenuItem1.Text = "audio" Then 'do this instead
                AudioToolStripMenuItem.PerformClick()
                Exit Sub
            End If
            If NullToolStripMenuItem1.Text = "url" Then '
                generateFromDialog("all files|*.*", "url")
                Exit Sub
            End If
            If NullToolStripMenuItem1.Text = "-url" Then '
                generateFromDialog("all files|*.*", "win")
                Exit Sub
            End If

            Dim x As String = "" ' repeat v2
            Dim a As Integer = 0
            Dim b As Integer = 0
            If GetChar(NullToolStripMenuItem1.Text, Len(NullToolStripMenuItem1.Text)) = "}" Then
                For i = 1 To Len(NullToolStripMenuItem1.Text) 'look for sk {}
                    If GetChar(NullToolStripMenuItem1.Text, i) = "{" Then a = i
                    If GetChar(NullToolStripMenuItem1.Text, i) = "}" Then b = i
                Next i
                x = Microsoft.VisualBasic.Mid(NullToolStripMenuItem1.Text, a, b) 'sk
            End If

            If a > 0 Then
                print(Microsoft.VisualBasic.Mid(NullToolStripMenuItem1.Text, 1, a - 1), False) 'print 
            Else

                If NullToolStripMenuItem1.Text = "null" Then
                Else
                    print(NullToolStripMenuItem1.Text, False) 'print regular
                End If
            End If

            If x > "" Then 'contains sk {x #}, convert and run
                x = Replace(x, "{", "«")
                x = Replace(x, "}", "»")
                x = Replace(x, " ", "*")
                apisk(" " + x + "")

            End If
            'timeout1(1)
            System.Threading.Thread.Sleep(1)

            Exit Sub
            keyRelease(Keys.LControlKey)
            GetAsyncKeyState(Keys.LControlKey)

        End If

        If MouseButtons = Windows.Forms.MouseButtons.Middle Then SendKeys.Send("«»{left}") 'sss
    End Sub

    Private Sub LongTagsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LongTagsToolStripMenuItem.Click
        chkItem(LongTagsToolStripMenuItem) 'long tags
        My.Settings.SettingChkTags = LongTagsToolStripMenuItem.CheckState
    End Sub

    Private Sub ListBox1_MouseLeave1(sender As Object, e As EventArgs) Handles ListBox1.MouseLeave
        If chk_tips.Checked = False Then ToolTip1.Active = False
        ToolTip1.Hide(Me.ListBox1)
        ToolTip1.SetToolTip(ListBox1, ttl)
        Me.ToolTip1.ToolTipTitle = ""
    End Sub

    Sub runList()
        If TextBox1.Text = "«st" Or TextBox1.Text = "«stE" Then Exit Sub 'show tabs x

        If Me.ListBox1.Items.Count <= 0 Then Exit Sub

        If ListBox1.SelectedItem.ToString.StartsWith("«") And ListBox1.SelectedItem.ToString.Contains("»") Then 'add to al
            ar.Clear()
            Dim lr_ar As String
            lr_ar = Microsoft.VisualBasic.Left(ListBox1.SelectedItem, ListBox1.SelectedItem.ToString.IndexOf("»") + 1)
            If lr_ar.Contains("*") Then lr_ar = Microsoft.VisualBasic.Left(ListBox1.SelectedItem, ListBox1.SelectedItem.ToString.IndexOf("*")) & "»" ' lr_ar.Contains("*:#") Or lr_ar.Contains("*:r")

            ii91 = Microsoft.VisualBasic.Right(ListBox1.SelectedItem.ToString, ListBox1.SelectedItem.ToString.Length - lr_ar.Length)
            If ii91.Contains(lr_ar) Then
                middle = lr_ar
                infiniteLoop(middle)
                Exit Sub
            End If

            ar.Add(lr_ar) 'connect1
        End If

        If ListBox1.Text.StartsWith("http") Then

            If connect = False Or strandComplete = False Then
                apisk("x«win»r«-win»«sleep:555»" + ListBox1.Text.ToString + "«enter»")
            Else
                strandComplete = True
                connect = True
                apisk("x«win»r«-win»«sleep:555»" + ListBox1.Text.ToString + "«enter»")
            End If

            clearAllKeys()
            emode()
            Exit Sub
        End If


        Me.Visible = False
        If ListBox1.Text.Length > Val(txtLength.Text) + 1 Then
            If GetChar(ListBox1.SelectedItem.ToString, Val(txtLength.Text) + 1) = Chr(9) Then 'run with tab
                Dim f1 As String = Mid(ListBox1.SelectedItem.ToString, 1, Val(txtLength.Text)) 'code
                Dim f2 = Microsoft.VisualBasic.Right(ListBox1.SelectedItem.ToString, Len(ListBox1.SelectedItem.ToString) - Val(txtLength.Text) - 1) 'message

                TextBox1.Text = "'"

                strandComplete = False
                connect = False
                Dim cb = Clipboard.GetText
                apisk("x" + Replace(f1, ".", "") + f2) 'add extra char, remove esc/.'s 
                If Not Clipboard.GetText = cb Then If g_remcb Then Clipboard.SetText(cb)
                strandComplete = True
                connect = True

                clearAllKeys()

                spacer()
                Me.Visible = True
                c = 0
                'TextBox1.Text = ""
                emode()
                Exit Sub
            End If
        End If

        Dim f3 As String = ListBox1.SelectedItem.ToString 'run without tab
        '
        If ListBox1.SelectedItem.ToString.Length > 0 Then
            TextBox1.Text = "'"
            If GetChar(f3, 1) = "'" Then
                f3 = Microsoft.VisualBasic.Mid(f3, 1, Len(f3))

                If connect = False Or strandComplete = False Then
                    apisk(f3) 'run new 'code without '
                Else
                    strandComplete = True
                    connect = True

                    apisk(f3) 'run new 'code without '
                End If

            Else 'run reg

                If connect = False Or strandComplete = False Then
                    strandComplete = True
                    connect = True

                    apisk("x" + f3)
                Else
                    strandComplete = True
                    connect = True
                    apisk("x" + f3)
                End If

            End If
            emode()
        End If
        '
        clearAllKeys()
        spacer()
        Me.Visible = True
        c = 0 'clear slide
    End Sub

    Sub spacer()
        System.Threading.Thread.Sleep(My.Settings.SettingSpacer)
    End Sub

    Dim c As Integer = 0 'tablet right click /slide
    Private Sub ListBox1_MouseMove1(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseMove
        showCursor()
        If chk_tips.Checked = True Then
            If Len(ToolTip1.GetToolTip(ListBox1)) < Len(ttl) Then 'remove database title when f1
                If ToolTip1.ToolTipTitle = "" Then
                Else
                    ToolTip1.ToolTipTitle = ""
                End If
            Else
                ToolTip1.ToolTipTitle = "database"
            End If
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            c += 1
            If c >= My.Settings.SettingZone Then
                If TabletToolStripMenuItem.Checked = False Then
                    ttAdjust()
                    mnuItemsShow(True)
                    If GetAsyncKeyState(Keys.LControlKey) Then txtString.SelectAll() : txtString.SelectedText = ""


                    If ListBox1.Items.Count > 0 Then
                        If ListBox1.Items.Count <> My.Settings.Settingdb.Count Or
                            ListBox1.Items.Item(0).ToString <> My.Settings.Settingdb.Item(0).ToString Or
                            ListBox1.Items.Item(0).ToString.Length <> My.Settings.Settingdb.Item(0).ToString.Length Then DeleteAllToolStripMenuItem.Text = "reload"  'in temp db
                    End If

                    Me.ContextMenuStripDb.Show(MousePosition) 'popupmenu db   

                Else
                    If My.Settings.SettingMulti = False Then leftrelease()
                    runList() 'swipe
                End If
                c = 0 'clear slide/swipe
            End If
        End If
    End Sub

    Private Sub ListBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseUp
        c = 0 'clear slide
    End Sub

    Private Sub ListBox1_MouseWheel1(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseWheel
        'ctrl + mouse wheel adjust listbox font size
        If GetAsyncKeyState(Keys.LControlKey) Or GetAsyncKeyState(Keys.RControlKey) Then
            If e.Delta > 1 Then
                Me.ListBox1.Font = New System.Drawing.Font(txtString.Font.Name, ListBox1.Font.Size + +1) '+ font size
            End If
            If e.Delta < 1 Then
                If ListBox1.Font.Size <= 1 Then Me.ListBox1.Font = New System.Drawing.Font(ListBox1.Font.Size, 8.25) 'reset
                Me.ListBox1.Font = New System.Drawing.Font(txtString.Font.Name, ListBox1.Font.Size + -1) '- f size
            End If
            My.Settings.SettingLstFontSize = ListBox1.Font.Size
        End If
    End Sub

    Private Sub txtString_MouseLeave1(sender As Object, e As EventArgs) Handles txtString.MouseLeave
        Me.ToolTip1.ToolTipTitle = ""
        If TabPage3.Text = "browser" And chk_tips.Checked = True Then ToolTip1.Active = True
    End Sub

    Private Sub txtString_MouseMove1(sender As Object, e As MouseEventArgs) Handles txtString.MouseMove
        showCursor()
        If TabPage3.Text = "browser" Then ToolTip1.Active = False
        If chk_tips.Checked = False Then Exit Sub
        If Me.ToolTip1.ToolTipTitle = "algorithm editor" Then
        Else
            Me.ToolTip1.ToolTipTitle = "algorithm editor"
        End If
    End Sub

    Private Sub MiddleClickToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MiddleClickToolStripMenuItem.Click
        skMenuGet1("¶", "middle-click", "")
    End Sub

    Private Sub ShowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowToolStripMenuItem.Click
        skMenuGet1("¾", "show", "")
    End Sub

    Private Sub HideToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles HideToolStripMenuItem1.Click
        skMenuGet1("ð", "hide", "")
    End Sub

    Private Sub RightAltToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RightAltToolStripMenuItem.Click
        chkItem(RightAltToolStripMenuItem)
        My.Settings.SettingChkOtherRightAlt = RightAltToolStripMenuItem.CheckState
    End Sub

    Private Sub LeftAltToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LeftAltToolStripMenuItem.Click
        chkItem(LeftAltToolStripMenuItem)
        My.Settings.SettingChkOtherLeftAlt = LeftAltToolStripMenuItem.CheckState
    End Sub

    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        If txtString.SelectedText > "" Then 'clear selected text only
            txtString.SelectedText = ""
            Exit Sub
        End If
        txtString.SelectAll() 'clear all text
        txtString.SelectedText = ""
    End Sub

    Private Sub PasteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteToolStripMenuItem.Click
        If Clipboard.GetText > "" Then Clipboard.SetText(Clipboard.GetText.ToString)
        txtString.Paste()
    End Sub

    Private Sub PauseToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles PauseToolStripMenuItem1.Click
        If LongTagsToolStripMenuItem.Checked = True Then
            If chk_tips.Checked = True Then
                skMenuGet("«milliseconds:»{left}")
            Else
                skMenuGet("«m:»{left}")
            End If
        Else
            skMenuGet("«m:»{left}")
        End If
    End Sub

    Sub showMoveBar()
        lblMove.Visible = True
        lblMoveTop.Visible = True
        lblMove.Left = Me.Width - lblMove.Width + 5
    End Sub

    Sub showCursor()
        If cursorshow = False Then
            cursorshow = True
            Cursor.Show()
        End If
    End Sub

    Private Sub dna_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            dragForm()
        End If
        If lblMove.Visible = True Then Exit Sub
        If MouseButtons = Windows.Forms.MouseButtons.Left Then 'ts
            'dragForm()
            If TabControl1.Visible = False And lblMove.Visible = False Then showMoveBar()
        End If
        If TabControl1.Visible = False And lblMove.Visible = False Then showMoveBar()
    End Sub

    Private Sub dna_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        drag = False
        showCursor()
    End Sub

    Private Sub dna_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        ListBox1.Width = txtString.Width
        If Me.Width <= 136 And Me.MaximizeBox = False And Cursor.Current = Cursors.SizeWE Then invertIcon()
    End Sub

    Sub invertIcon()
        If Me.ShowIcon = False Then
            Me.ShowIcon = True
        Else
            If Me.ControlBox = True Then Me.ShowIcon = False
        End If
        My.Settings.SettingShowIcon = Me.ShowIcon()
    End Sub

    Dim sp As Boolean = False 'sp
    Private Sub dna_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        spDown()
    End Sub

    Sub spDown() 'Resize bottom container height get value
        If ListBox1.Font.Size >= 8 And ListBox1.Font.Size < 9 Then
            If SplitContainer1.SplitterDistance >= SplitContainer1.Height - SplitContainer1.SplitterWidth - 10 Then sp = True ': fixSCHeight()
        Else
            If SplitContainer1.SplitterDistance >= SplitContainer1.Height - SplitContainer1.SplitterWidth - 15 Then sp = True
        End If
    End Sub
    Sub spUp() 'Resize bottom container height
        If sp = True Then
            If SplitContainer1.Height > 0 Then
                If Me.Height = SplitContainer1.Height + 20 Then
                    SplitContainer1.SplitterDistance = SplitContainer1.Height - 10
                Else
                    If SplitContainer1.SplitterDistance = 0 Or SplitContainer1.Panel1.Height <= 3 Or SplitContainer1.Panel2.Height <= 3 Then Exit Sub
                    SplitContainer1.SplitterDistance = SplitContainer1.Height
                End If
            End If
            sp = False
        End If
    End Sub

    Sub fixSCHeight() 'splitcontainer
        If ListBox1.Font.Size < 9 Then
            If SplitContainer1.Height <= Me.Height Or SplitContainer1.Height <= Me.Height - txtLength.Height - SplitContainer1.SplitterWidth Then
                If My.Settings.SettingMoveBar = True Then Exit Sub
                SplitContainer1.Height = Me.Height - txtLength.Height - 10
            End If
        End If
    End Sub

    Private Sub dna_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        If Me.Height > 39 And TabControl1.Visible = True And txtString.Visible = False Then
            txtString.Visible = True
            txtString.Focus()
        End If
        If Me.Height <= 39 Or TabControl1.Visible = False Then
            txtString.Visible = False
        End If

        Me.Refresh()

        If SplitContainer1.BorderStyle = BorderStyle.FixedSingle Then
            ListBox1.Width = SplitContainer1.Width - 2 '20
        Else
            ListBox1.Width = txtString.Width
        End If

        txtString.Width = ListBox1.Width
        If My.Settings.SettingScrollBar = False Then showScrollBar(False)

        lblMove.Left = Me.Width - lblMove.Width + 5

        If Me.Width <= 136 Then
            Me.MaximizeBox = False
            Me.MinimizeBox = False
        End If
        If Me.Width > 236 And Me.Text > "" And Me.ControlBox = True Then
            Me.MaximizeBox = True
            Me.MinimizeBox = True
        End If
        Try
            spUp()
        Catch ex As Exception
        End Try
    End Sub

    Sub showScrollBar(tf As Boolean)
        If tf = False Then
            nsb = SplitContainer1.Height - SplitContainer1.Panel1.Height - SplitContainer1.SplitterWidth + 17 'wordwrap
            nsbp = SplitContainer1.Height - SplitContainer1.Panel1.Height - SplitContainer1.SplitterWidth '- 1

            If SplitContainer1.BorderStyle = BorderStyle.None Then

                SplitContainer1.Left = 3

                ListBox1.Width = SplitContainer1.Width - 3 + 20 'hide lstbox scrlbar
                If Me.ControlBox = True And Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable Then txtString.Height = nsbp ' hide ww sb, resize height

                If WordWrapToolStripMenuItem.Checked = True Then
                    If ListBox1.BorderStyle = BorderStyle.FixedSingle Then nsb += 1
                    txtString.Height = nsb
                    txtString.Width = ListBox1.Width '- 1
                    If Me.ControlBox = False And Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable Then txtString.Width = ListBox1.Width '- 1
                    If Me.ControlBox = False And Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None Then txtString.Width = ListBox1.Width '- 1
                Else
                    txtString.Width = ListBox1.Width + 1
                    txtString.Height = nsbp
                End If

                Refresh()
            End If

            If SplitContainer1.BorderStyle = BorderStyle.FixedSingle Then
                SplitContainer1.Left = 3
                ListBox1.Width = SplitContainer1.Width - 5 + 20
                txtString.Height = nsbp
                txtString.Width = ListBox1.Width
            End If

            If ListBox1.BorderStyle = BorderStyle.FixedSingle Then 'border
                SplitContainer1.Left = 3
                If Me.ControlBox = False And Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable Then SplitContainer1.Left = 4
                ListBox1.Width = SplitContainer1.Width - 3 + 20
            End If

            If My.Settings.SettingMultiColumn = True And My.Settings.SettingViewMultiScrollBar = False Then ListBox1.Height = SplitContainer1.Panel1.Height + 33 'multi sb
        End If


        If tf = True Then
            If SplitContainer1.BorderStyle = BorderStyle.FixedSingle Then 'border
                SplitContainer1.Left = 4
            End If
        End If

        If My.Settings.SettingViewMultiScrollBar = False And My.Settings.SettingMultiColumn = True Then ListBox1.Height = SplitContainer1.Panel1.Height + 33 'multi sb
    End Sub

    Dim oneXY As Boolean = True
    Sub runMousePosition()
        If My.Settings.SettingDnaX = True Then tipsDnaToolStripMenuItem.Checked = False

        If oneXY = True Then
            oneXY = False
            dz = 0
            NullToolStripMenuItem1.Text = "«xy"
            Dim rz = txtString.ZoomFactor  're-zoom

            Dim g = TabPage3.BackColor 'redbg
            If TabPage3.BackColor = Color.Red Then TabPage3.BackColor = Color.White Else TabPage3.BackColor = Color.Red '

            Cursor.Show()
            Me.Text = "3" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "3" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "3" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "2" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "2" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "2" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "1" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "1" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "1" & " «xy:" & MousePosition.X & "-" & MousePosition.Y & "»"
            timeout2(333)
            Me.Text = "dna"
            leftrelease()
            AppActivate("dna")
            txtString.Focus()
            timeout2(333)

            print("«xy:" & MousePosition.X & "-" & MousePosition.Y & "»", False)
            timeout2(333)
            print("«xy:" & MousePosition.X & "-" & MousePosition.Y & "»", True)

            TabPage3.BackColor = g 'redbg

            If My.Settings.SettingScrollBar = False Then showScrollBar(False)
            txtString.ZoomFactor = rz 're-zoom
            oneXY = True
            tipsDnaToolStripMenuItem.Checked = My.Settings.SettingDnaX
        End If
    End Sub

    Sub printmp()
        Dim g = TabPage3.BackColor
        If TabPage3.BackColor = Color.Red Then TabPage3.BackColor = Color.White Else TabPage3.BackColor = Color.Red
        timeout1(3)
        txtString.Focus()
        print("«xy:" & MousePosition.X & "-" & MousePosition.Y & "»", False)
        timeout2(333)
        print("«xy:" & MousePosition.X & "-" & MousePosition.Y & "»", True)
        TabPage3.BackColor = g
    End Sub

    Private Sub TabPage3_DoubleClick(sender As Object, e As EventArgs) Handles TabPage3.DoubleClick
        If Me.Text > "" Then runMousePosition() Else printmp()
    End Sub

    Private Sub RedoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RedoToolStripMenuItem.Click
        SendKeys.Send("^y")
        keyClear(Keys.Y)
    End Sub

    Private Sub UndoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UndoToolStripMenuItem.Click
        SendKeys.Send("^z")
        keyClear(Keys.Z)
    End Sub

    Private Sub WordWrapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WordWrapToolStripMenuItem.Click
        Dim z = txtString.ZoomFactor
        chkItem(WordWrapToolStripMenuItem) 'word wrap option

        If WordWrapToolStripMenuItem.Checked = True Then
            txtString.WordWrap = False
        Else
            txtString.WordWrap = True
        End If
        My.Settings.SettingWordWrap = WordWrapToolStripMenuItem.CheckState

        showScrollBar(My.Settings.SettingScrollBar)

        txtString.ZoomFactor = z
    End Sub

    Private Sub MultiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MultiToolStripMenuItem.Click
        chkItem(MultiToolStripMenuItem)
        My.Settings.SettingMulti = MultiToolStripMenuItem.CheckState
    End Sub

    Private Sub ZoneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ZoneToolStripMenuItem.Click
        Dim z = InputBox("1-99", "click swipe zone: " & My.Settings.SettingZone, My.Settings.SettingZone)
        If IsNumeric(z) Then
            If z < 100 And z > 0 Then
                My.Settings.SettingZone = z
            End If
        End If
    End Sub

    Sub showBrowserTab()
        Clipboard.SetText(WebBrowser1.Parent.ToString)
        WebBrowser1.Parent = SplitContainer1.Panel1
        WebBrowser1.BringToFront()

        WebBrowser1.Visible = True
        WebBrowser1.Top = 0
        WebBrowser1.Left = 0
        WebBrowser1.Width = ListBox1.Width
        WebBrowser1.Height = Me.Height
        TabPage3.Text = "browser"
        SplitContainer1.SplitterDistance = SplitContainer1.Height - 50
        txtString.Focus()
        txtString.SelectAll()
    End Sub

    Private Sub PlayPauseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayPauseToolStripMenuItem.Click
        chkItem(PlayPauseToolStripMenuItem)
        My.Settings.SettingIgnoreMediaPlayPause = PlayPauseToolStripMenuItem.CheckState
    End Sub

    Private Sub VolumeUpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VolumeUpToolStripMenuItem.Click
        chkItem(VolumeUpToolStripMenuItem)
        My.Settings.SettingIgnoreVoluemUp = VolumeUpToolStripMenuItem.CheckState
    End Sub

    Private Sub VolumeDownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VolumeDownToolStripMenuItem.Click
        chkItem(VolumeDownToolStripMenuItem)
        My.Settings.SettingIgnoreVolumeDown = VolumeDownToolStripMenuItem.CheckState
    End Sub

    Private Sub VolumeMuteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VolumeMuteToolStripMenuItem.Click
        chkItem(VolumeMuteToolStripMenuItem)
        My.Settings.SettingIgnoreVolumeMute = VolumeMuteToolStripMenuItem.CheckState
    End Sub

    Private Sub PrintScreenToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles PrintScreenToolStripMenuItem2.Click
        chkItem(PrintScreenToolStripMenuItem2)
        My.Settings.SettingIgnorePrintScreen = PrintScreenToolStripMenuItem2.CheckState
    End Sub

    Private Sub PageUpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PageUpToolStripMenuItem.Click
        chkItem(PageUpToolStripMenuItem)
        My.Settings.SettingIgnorePageUp = PageUpToolStripMenuItem.CheckState
    End Sub

    Private Sub PageDownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PageDownToolStripMenuItem.Click
        chkItem(PageDownToolStripMenuItem)
        My.Settings.SettingIgnorePageDown = PageDownToolStripMenuItem.CheckState
    End Sub

    Private Sub HomeToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles HomeToolStripMenuItem1.Click
        chkItem(HomeToolStripMenuItem1)
        My.Settings.SettingIgnoreHome = HomeToolStripMenuItem1.CheckState
    End Sub

    Private Sub EndToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EndToolStripMenuItem1.Click
        chkItem(EndToolStripMenuItem1)
        My.Settings.SettingIgnoreEnd = EndToolStripMenuItem1.CheckState
    End Sub

    Private Sub chkWedgee_CheckedChanged(sender As Object, e As EventArgs) Handles chkWedgee.CheckedChanged
        My.Settings.SettingIgnoreWedgee = chkWedgee.CheckState
    End Sub

    Private Sub chkWedgee_MouseDown(sender As Object, e As MouseEventArgs) Handles chkWedgee.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            ttAdjust()
            ContextMenuStripChkMedia.Show(MousePosition)
        End If
    End Sub

    Private Sub TabletToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TabletToolStripMenuItem.Click
        chkItem(TabletToolStripMenuItem)
        My.Settings.SettingTabletSwipe = TabletToolStripMenuItem.CheckState
    End Sub

    Private Sub SpacerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpacerToolStripMenuItem.Click
        Dim z = InputBox("milliseconds", "spacer", My.Settings.SettingSpacer)
        If IsNumeric(z) Then
            If z <= 9999 And z > 0 Then
                My.Settings.SettingSpacer = z
            End If
        End If
    End Sub

    Private Sub ZToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ZToolStripMenuItem.Click
        chkItem(ZToolStripMenuItem)
        My.Settings.SettingIgnoreZ = ZToolStripMenuItem.CheckState
    End Sub

    Private Sub YToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles YToolStripMenuItem.Click
        chkItem(YToolStripMenuItem)
        My.Settings.SettingIgnoreY = YToolStripMenuItem.CheckState
    End Sub

    Private Sub XToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XToolStripMenuItem.Click
        chkItem(XToolStripMenuItem)
        My.Settings.SettingIgnoreX = XToolStripMenuItem.CheckState
    End Sub

    Private Sub WToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WToolStripMenuItem.Click
        chkItem(WToolStripMenuItem)
        My.Settings.SettingIgnoreW = WToolStripMenuItem.CheckState
    End Sub

    Private Sub VToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VToolStripMenuItem.Click
        chkItem(VToolStripMenuItem)
        My.Settings.SettingIgnoreV = VToolStripMenuItem.CheckState
    End Sub

    Private Sub UToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UToolStripMenuItem.Click
        chkItem(UToolStripMenuItem)
        My.Settings.SettingIgnoreU = UToolStripMenuItem.CheckState
    End Sub

    Private Sub TToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TToolStripMenuItem.Click
        chkItem(TToolStripMenuItem)
        My.Settings.SettingIgnoreT = TToolStripMenuItem.CheckState
    End Sub

    Private Sub SToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SToolStripMenuItem.Click
        chkItem(SToolStripMenuItem)
        My.Settings.SettingIgnoreS = SToolStripMenuItem.CheckState
    End Sub

    Private Sub RToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RToolStripMenuItem.Click
        chkItem(RToolStripMenuItem)
        My.Settings.SettingIgnoreR = RToolStripMenuItem.CheckState
    End Sub

    Private Sub QToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QToolStripMenuItem.Click
        chkItem(QToolStripMenuItem)
        My.Settings.SettingIgnoreQ = QToolStripMenuItem.CheckState
    End Sub

    Private Sub PToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PToolStripMenuItem.Click
        chkItem(PToolStripMenuItem)
        My.Settings.SettingIgnoreP = PToolStripMenuItem.CheckState
    End Sub

    Private Sub OToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OToolStripMenuItem.Click
        chkItem(OToolStripMenuItem)
        My.Settings.SettingIgnoreO = OToolStripMenuItem.CheckState
    End Sub

    Private Sub NToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NToolStripMenuItem.Click
        chkItem(NToolStripMenuItem)
        My.Settings.SettingIgnoreN = NToolStripMenuItem.CheckState
    End Sub

    Private Sub MToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MToolStripMenuItem.Click
        chkItem(MToolStripMenuItem)
        My.Settings.SettingIgnoreM = MToolStripMenuItem.CheckState
    End Sub

    Private Sub LToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LToolStripMenuItem.Click
        chkItem(LToolStripMenuItem)
        My.Settings.SettingIgnoreL = LToolStripMenuItem.CheckState
    End Sub

    Private Sub KToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KToolStripMenuItem.Click
        chkItem(KToolStripMenuItem)
        My.Settings.SettingIgnoreK = KToolStripMenuItem.CheckState
    End Sub

    Private Sub JToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JToolStripMenuItem.Click
        chkItem(JToolStripMenuItem)
        My.Settings.SettingIgnoreJ = JToolStripMenuItem.CheckState
    End Sub

    Private Sub IToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IToolStripMenuItem.Click
        chkItem(IToolStripMenuItem)
        My.Settings.SettingIgnoreI = IToolStripMenuItem.CheckState
    End Sub

    Private Sub HToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HToolStripMenuItem.Click
        chkItem(HToolStripMenuItem)
        My.Settings.SettingIgnoreH = HToolStripMenuItem.CheckState
    End Sub

    Private Sub GToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GToolStripMenuItem.Click
        chkItem(GToolStripMenuItem)
        My.Settings.SettingIgnoreF = GToolStripMenuItem.CheckState
    End Sub

    Private Sub FToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FToolStripMenuItem1.Click
        chkItem(FToolStripMenuItem1)
        My.Settings.SettingIgnoreF = FToolStripMenuItem1.CheckState
    End Sub

    Private Sub EToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EToolStripMenuItem.Click
        chkItem(EToolStripMenuItem)
        My.Settings.SettingIgnoreE = EToolStripMenuItem.CheckState
    End Sub

    Private Sub DToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DToolStripMenuItem.Click
        chkItem(DToolStripMenuItem)
        My.Settings.SettingIgnoreD = DToolStripMenuItem.CheckState
    End Sub

    Private Sub CToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CToolStripMenuItem.Click
        chkItem(CToolStripMenuItem)
        My.Settings.SettingIgnoreV = CToolStripMenuItem.CheckState
    End Sub

    Private Sub BToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BToolStripMenuItem.Click
        chkItem(BToolStripMenuItem)
        My.Settings.SettingIgnoreB = BToolStripMenuItem.CheckState
    End Sub

    Private Sub AToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AToolStripMenuItem.Click
        chkItem(AToolStripMenuItem)
        My.Settings.SettingIgnoreA = AToolStripMenuItem.CheckState
    End Sub

    Private Sub chkAz_MouseDown(sender As Object, e As MouseEventArgs) Handles chkAz.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then ContextMenuStripChkAtoZ.Show(MousePosition)
    End Sub

    Private Sub chk09_MouseDown(sender As Object, e As MouseEventArgs) Handles chk09.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then ContextMenuStripChk0to9.Show(MousePosition)
    End Sub

    Private Sub chkF1f12_MouseDown(sender As Object, e As MouseEventArgs) Handles chkF1f12.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then ContextMenuStripf1tof12.Show(MousePosition)
    End Sub

    Private Sub chkNumPad_MouseDown(sender As Object, e As MouseEventArgs) Handles chkNumPad.MouseDown
        If GetAsyncKeyState(Keys.LControlKey) Or GetAsyncKeyState(Keys.RControlKey) And MouseButtons = Windows.Forms.MouseButtons.Right Then
            If My.Settings.SettingUseNumPad = False Then My.Settings.SettingUseNumPad = True Else My.Settings.SettingUseNumPad = False
            MsgBox("use num-pad keys instead of number keys when printing numbers 0-9" & vbNewLine & My.Settings.SettingUseNumPad.ToString, vbInformation, "setting changed")
            Exit Sub
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right Then ContextMenuStripNumPad.Show(MousePosition)
    End Sub

    Private Sub chkArrows_MouseDown(sender As Object, e As MouseEventArgs) Handles chkArrows.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then ContextMenuStripArrows.Show(MousePosition)
    End Sub

    Private Sub F1ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F1ToolStripMenuItem1.Click
        chkItem(F1ToolStripMenuItem1)
        My.Settings.SettingIgnoref1 = F1ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F2ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F2ToolStripMenuItem1.Click
        chkItem(F2ToolStripMenuItem1)
        My.Settings.SettingIgnoref2 = F2ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F3ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F3ToolStripMenuItem1.Click
        chkItem(F3ToolStripMenuItem1)
        My.Settings.SettingIgnoref3 = F3ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F4ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles F4ToolStripMenuItem.Click
        chkItem(F4ToolStripMenuItem)
        My.Settings.SettingIgnoref4 = F4ToolStripMenuItem.CheckState
    End Sub

    Private Sub F5ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F5ToolStripMenuItem1.Click
        chkItem(F5ToolStripMenuItem1)
        My.Settings.SettingIgnoref5 = F5ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F6ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F6ToolStripMenuItem1.Click
        chkItem(F6ToolStripMenuItem1)
        My.Settings.SettingIgnoref6 = F6ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F7ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F7ToolStripMenuItem1.Click
        chkItem(F7ToolStripMenuItem1)
        My.Settings.SettingIgnoref7 = F7ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F8ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F8ToolStripMenuItem1.Click
        chkItem(F8ToolStripMenuItem1)
        My.Settings.SettingIgnoref8 = F8ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F9ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F9ToolStripMenuItem1.Click
        chkItem(F9ToolStripMenuItem1)
        My.Settings.SettingIgnoref9 = F9ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F10ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F10ToolStripMenuItem1.Click
        chkItem(F10ToolStripMenuItem1)
        My.Settings.SettingIgnoref10 = F10ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F11ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F11ToolStripMenuItem1.Click
        chkItem(F11ToolStripMenuItem1)
        My.Settings.SettingIgnoref11 = F11ToolStripMenuItem1.CheckState
    End Sub

    Private Sub F12ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles F12ToolStripMenuItem1.Click
        chkItem(F12ToolStripMenuItem1)
        My.Settings.SettingIgnoref12 = F12ToolStripMenuItem1.CheckState
    End Sub

    Private Sub UpToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles UpToolStripMenuItem1.Click
        chkItem(UpToolStripMenuItem1)
        My.Settings.SettingIgnoreup = UpToolStripMenuItem1.CheckState
    End Sub

    Private Sub DownToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles DownToolStripMenuItem1.Click
        chkItem(DownToolStripMenuItem1)
        My.Settings.SettingIgnoredown = DownToolStripMenuItem1.CheckState
    End Sub

    Private Sub LeftToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles LeftToolStripMenuItem1.Click
        chkItem(LeftToolStripMenuItem1)
        My.Settings.SettingIgnoreleft = LeftToolStripMenuItem1.CheckState
    End Sub

    Private Sub RightToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles RightToolStripMenuItem1.Click
        chkItem(RightToolStripMenuItem1)
        My.Settings.SettingIgnoreright = RightToolStripMenuItem1.CheckState
    End Sub

    Private Sub ToolStripMenuItem14_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem14.Click
        chkItem(ToolStripMenuItem14)
        My.Settings.SettingIgnore0 = ToolStripMenuItem14.CheckState
    End Sub

    Private Sub ToolStripMenuItem15_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem15.Click
        chkItem(ToolStripMenuItem15)
        My.Settings.SettingIgnore1 = ToolStripMenuItem15.CheckState
    End Sub

    Private Sub ToolStripMenuItem16_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem16.Click
        chkItem(ToolStripMenuItem16)
        My.Settings.SettingIgnore2 = ToolStripMenuItem16.CheckState
    End Sub

    Private Sub ToolStripMenuItem17_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem17.Click
        chkItem(ToolStripMenuItem17)
        My.Settings.SettingIgnore3 = ToolStripMenuItem17.CheckState
    End Sub

    Private Sub ToolStripMenuItem18_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem18.Click
        chkItem(ToolStripMenuItem18)
        My.Settings.SettingIgnore4 = ToolStripMenuItem18.CheckState
    End Sub

    Private Sub ToolStripMenuItem19_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem19.Click
        chkItem(ToolStripMenuItem19)
        My.Settings.SettingIgnore5 = ToolStripMenuItem19.CheckState
    End Sub

    Private Sub ToolStripMenuItem20_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem20.Click
        chkItem(ToolStripMenuItem20)
        My.Settings.SettingIgnore6 = ToolStripMenuItem20.CheckState
    End Sub

    Private Sub ToolStripMenuItem21_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem21.Click
        chkItem(ToolStripMenuItem21)
        My.Settings.SettingIgnore7 = ToolStripMenuItem21.CheckState
    End Sub

    Private Sub ToolStripMenuItem22_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem22.Click
        chkItem(ToolStripMenuItem22)
        My.Settings.SettingIgnore8 = ToolStripMenuItem22.CheckState
    End Sub

    Private Sub ToolStripMenuItem23_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem23.Click
        chkItem(ToolStripMenuItem23)
        My.Settings.SettingIgnore9 = ToolStripMenuItem23.CheckState
    End Sub

    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        chkItem(ToolStripMenuItem4)
        My.Settings.SettingIgnoren0 = ToolStripMenuItem4.CheckState
    End Sub

    Private Sub ToolStripMenuItem5_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem5.Click
        chkItem(ToolStripMenuItem5)
        My.Settings.SettingIgnoren1 = ToolStripMenuItem5.CheckState
    End Sub

    Private Sub ToolStripMenuItem6_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem6.Click
        chkItem(ToolStripMenuItem6)
        My.Settings.SettingIgnoren2 = ToolStripMenuItem6.CheckState
    End Sub

    Private Sub ToolStripMenuItem7_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem7.Click
        chkItem(ToolStripMenuItem7)
        My.Settings.SettingIgnoren3 = ToolStripMenuItem7.CheckState
    End Sub

    Private Sub ToolStripMenuItem8_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem8.Click
        chkItem(ToolStripMenuItem8)
        My.Settings.SettingIgnoren4 = ToolStripMenuItem8.CheckState
    End Sub

    Private Sub ToolStripMenuItem9_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem9.Click
        chkItem(ToolStripMenuItem9)
        My.Settings.SettingIgnoren5 = ToolStripMenuItem9.CheckState
    End Sub

    Private Sub ToolStripMenuItem10_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem10.Click
        chkItem(ToolStripMenuItem10)
        My.Settings.SettingIgnoren6 = ToolStripMenuItem10.CheckState
    End Sub

    Private Sub ToolStripMenuItem11_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem11.Click
        chkItem(ToolStripMenuItem11)
        My.Settings.SettingIgnoren7 = ToolStripMenuItem11.CheckState
    End Sub

    Private Sub ToolStripMenuItem12_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem12.Click
        chkItem(ToolStripMenuItem12)
        My.Settings.SettingIgnoren8 = ToolStripMenuItem12.CheckState
    End Sub

    Private Sub ToolStripMenuItem13_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem13.Click
        chkItem(ToolStripMenuItem13)
        My.Settings.SettingIgnoren9 = ToolStripMenuItem13.CheckState
    End Sub

    Private Sub LeftCtrlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RightCtrllToolStripMenuItem.Click
        If RightCtrllToolStripMenuItem.Checked = False Then 'lctrl , custom += timer1
            Dim q = InputBox("right ctrl = " & My.Settings.SettingRctrleqMod, "change right ctrl = ", My.Settings.SettingRctrleqMod)
            If q.Length > 0 Then
                My.Settings.SettingRctrleqMod = GetChar(q, 1)
            End If
        End If
        chkItem(RightCtrllToolStripMenuItem)
        My.Settings.SettingRctrleqdot = RightCtrllToolStripMenuItem.CheckState

        If My.Settings.SettingRctrleqMod = "»" And RightCtrllToolStripMenuItem.Checked = True And My.Settings.SettingAutoLockEmode = True Then 'freeze 
            TextBox1.Text = "»"
            dnaTxt()
        Else
            TextBox1.Clear()
        End If
    End Sub

    Sub selectBottomItem()
        If ListBox1.Items.Count >= 0 Then ListBox1.SelectedIndex = ListBox1.Items.Count - 1
        My.Settings.SettingLastListIndex = ListBox1.SelectedIndex '$
    End Sub

    Sub selectTopItem()
        If ListBox1.Items.Count = Nothing Then Exit Sub
        If ListBox1.Items.Count >= 0 Then ListBox1.SelectedItem() = ListBox1.Items.Item(0)
        My.Settings.SettingLastListIndex = ListBox1.SelectedIndex '$
    End Sub

    Private Sub LeftClickToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles LeftClickToolStripMenuItem1.Click
        chkItem(LeftClickToolStripMenuItem1)
        My.Settings.SettingMouseLeft = LeftClickToolStripMenuItem1.CheckState
    End Sub

    Private Sub RightClickToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles RightClickToolStripMenuItem1.Click
        chkItem(RightClickToolStripMenuItem1)
        My.Settings.SettingMouseRight = RightClickToolStripMenuItem1.CheckState
    End Sub

    Private Sub MiddleClickToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles MiddleClickToolStripMenuItem1.Click
        chkItem(MiddleClickToolStripMenuItem1)
        My.Settings.SettingMouseM = MiddleClickToolStripMenuItem1.CheckState
    End Sub

    Private Sub ChkMouse_CheckStateChanged(sender As Object, e As EventArgs) Handles ChkMouse.CheckStateChanged
        My.Settings.SettingMouse = ChkMouse.CheckState
    End Sub

    Private Sub ChkMouse_MouseDown(sender As Object, e As MouseEventArgs) Handles ChkMouse.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then ContextMenuStripMouse.Show(MousePosition)
    End Sub

    Private Sub OskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OskToolStripMenuItem.Click
        chkItem(OskToolStripMenuItem)
        My.Settings.SettingOsk = OskToolStripMenuItem.CheckState
    End Sub

    Private Sub AddToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles AddToolStripMenuItem.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Left Then 'add  to db list
            addDbItm()
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right Then 'temporarily add to db list
            ListBox1.Items.Add(txtString.Text)
            txtStringClear()
            selectBottomItem()
        End If
    End Sub

    Sub sizeable()
        Me.BackColor = Color.GhostWhite

        TabControl1.Top = -txtLength.Height - 1
        TabControl1.SizeMode = TabSizeMode.Fixed
        TabControl1.Appearance = TabAppearance.FlatButtons

        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        If Me.tipsDnaToolStripMenuItem.Checked = True Then Me.tipsDnaToolStripMenuItem.Checked = False
        Me.Text = ""
        Me.ControlBox = False
        TabControl1.Left = 0
        TabControl1.Width = Me.Width - 16 '- SplitContainer1.SplitterWidth

        moveable()
        Me.BackColor = My.Settings.SettingMainBgColor
        showMoveBar()

        fixSCHeight()
    End Sub

    Sub changeView()

        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None And Me.ControlBox = False Then 'if ctrl+dblclick view, back to norm
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
            Me.MinimizeBox = True
            Me.MaximizeBox = True
            Me.tipsDnaToolStripMenuItem.Checked = My.Settings.SettingDnaX
            Me.ControlBox = True
            lblMove.Visible = False
            lblMoveTop.Visible = False
            Me.ShowIcon = True
            Me.BackColor = My.Settings.SettingBgColor

            reStack()
            If My.Settings.SettingScrollBar = False Then showScrollBar(False)
            Exit Sub
        End If

        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None And TabControl1.Visible = False Then
            TabControl1.Top = ttop
            tabStyleAppearance()
            Me.Height += lblLength.Height 'solid view
            TabControl1.Visible = True
            Me.BackColor = Color.GhostWhite

            showMoveBar()
            If My.Settings.SettingScrollBar = False Then showScrollBar(False)

            Exit Sub
        End If

        If TabControl1.Visible = True And Not Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None Then
            TabControl1.Visible = False
            Me.BackColor = My.Settings.SettingMainBgColor
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
            If My.Settings.SettingScrollBar = False Then showScrollBar(False)
        Else

            If Me.BackColor = Color.GhostWhite Then 'hide tabs
                If TabControl1.Top <= -txtLength.Height Then
                    GoTo mainstyle
                    Exit Sub
                End If
                TabControl1.Top = -txtLength.Height - 1
                TabControl1.SizeMode = TabSizeMode.Fixed
                TabControl1.Appearance = TabAppearance.FlatButtons

                If GetAsyncKeyState(Keys.LControlKey) Or GetAsyncKeyState(Keys.RControlKey) Then preSizeable()

                If chk_tips.Checked = True Then MsgBox("ctrl + tab: toggle tabs")

                If My.Settings.SettingScrollBar = False Then showScrollBar(False)
                'img
                Exit Sub
            End If

mainstyle:
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
            lblMove.Visible = False
            lblMoveTop.Visible = False
            TabControl1.Visible = True
            txtString.Visible = True
            txtString.Focus()
            tabStyleAppearance()
            Me.BackColor = My.Settings.SettingMainBgColor
            TabControl1.Top = ttop
            reStack()
            TabControl1.SizeMode = TabSizeMode.Normal
            TabControl1.Appearance = TabAppearance.Normal

        End If
        Me.Refresh()
        If My.Settings.SettingScrollBar = False Then showScrollBar(False)

        If TabControl1.Visible = False Then 'drag frm
            System.Threading.Thread.Sleep(333)
            key(Keys.LControlKey)
        End If
    End Sub

    Sub preSizeable()
        Dim ts = txtString.ZoomFactor
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        If Me.tipsDnaToolStripMenuItem.Checked = True Then Me.tipsDnaToolStripMenuItem.Checked = False
        Me.Text = ""
        Me.ControlBox = False
        TabControl1.Left = 0
        TabControl1.Width = Me.Width - 16 '- SplitContainer1.SplitterWidth
        TabControl1.Height = Me.Height + txtLength.Height - 16 - SplitContainer1.SplitterWidth
        Me.BackColor = My.Settings.SettingMainBgColor
        showMoveBar()
        txtString.ZoomFactor = ts
    End Sub

    Private Sub lblMove_DoubleClick(sender As Object, e As EventArgs) Handles lblMove.DoubleClick
        changeView()
    End Sub

    Sub safeMove()
        If My.Settings.SettingChkDragToExtendedScreen = True Then Exit Sub
        If Me.Left < -Me.Width + 10 Then 'left
            Me.Left = 0
            leftrelease()
        End If
        If Me.Top < -Me.Height + 45 Then 'top
            Me.Top = 0
            leftrelease()
        End If

        If Me.Top > Screen.PrimaryScreen.Bounds.Height - 20 Then 'bottom
            Me.Top = Screen.PrimaryScreen.Bounds.Height - Me.Height
            leftrelease()
        End If
        If Me.Left > Screen.PrimaryScreen.Bounds.Width - 20 Then 'right
            Me.Left = Screen.PrimaryScreen.Bounds.Width - Me.Width
            leftrelease()
        End If
    End Sub

    Sub dragFormTop()
        If cursorshow = True Then
            cursorshow = False
            Cursor.Hide()
        End If
        Dim xx As Integer = MousePosition.X 'grab form 
        Dim yy As Integer = MousePosition.Y
        Me.Left = xx
        Me.Top = yy
        safeMove()
    End Sub
    Dim cursorshow = True
    Sub dragForm()
        If cursorshow = True Then
            cursorshow = False
            Cursor.Hide()
        End If
        If drag = True Then
            Me.Top = Cursor.Position.Y - mousey
            Me.Left = Cursor.Position.X - mousex
        End If
        safeMove()
    End Sub

    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer
    Sub dragfrm()
        drag = True
        mousex = Cursor.Position.X - Me.Left
        mousey = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub lblMove_MouseMove(sender As Object, e As MouseEventArgs) Handles lblMove.MouseMove
        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            If GetAsyncKeyState(Keys.LControlKey) Then Exit Sub
            dragForm()
        End If
    End Sub

    Private Sub lblMove_MouseUp(sender As Object, e As MouseEventArgs) Handles lblMove.MouseUp
        drag = False
        showCursor()
    End Sub

    Private Sub lblMoveTop_DoubleClick(sender As Object, e As EventArgs) Handles lblMoveTop.DoubleClick
        changeView()
    End Sub

    Sub showOptionsMenu()
        ttAdjust()
        OnToolStripMenuItem.Checked = My.Settings.SettingTimer1_chk_on_val
        HideToolStripMenuItem.Checked = My.Settings.SettingHidden
        ContextMenuStripOptions.Show(MousePosition)
        If My.Settings.SettingHidden = True Then
            HideToolStripMenuItem.Checked = True
            Me.Show()
        End If
        HideToolStripMenuItem.Checked = False
    End Sub

    Private Sub lblMoveTop_MouseMove(sender As Object, e As MouseEventArgs) Handles lblMoveTop.MouseMove
        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            If GetAsyncKeyState(Keys.LControlKey) Then Exit Sub
            dragForm()
        End If
    End Sub

    Private Sub lblMoveTop_MouseUp(sender As Object, e As MouseEventArgs) Handles lblMoveTop.MouseUp
        drag = False
        showCursor()
    End Sub

    Private Sub DeleteToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles DeleteToolStripMenuItem.MouseDown
        ContextMenuStripDb.Hide()

        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            If ListBox1.SelectedIndex = -1 Then Exit Sub
            Dim msg As Integer
            msg = MsgBox("[" & Microsoft.VisualBasic.Left(ListBox1.SelectedItem.ToString, Val(txtLength.Text)) & "]", vbYesNo, "temporarily delete item?")

            If msg = vbYes Then
                ListBox1.Items.RemoveAt(ListBox1.SelectedIndex)
                DeleteAllToolStripMenuItem.Text = "reload"
            End If

            Exit Sub

        End If

        If DeleteAllToolStripMenuItem.Text = "reload" Then 'temp delete
            If ListBox1.Items.Count > 0 Then ListBox1.Items.RemoveAt(ListBox1.SelectedIndex)
            Exit Sub
        End If

        deleteDbItm()
    End Sub

    Private Sub DeleteAllToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles DeleteAllToolStripMenuItem.MouseDown
        ContextMenuStripDb.Hide()

        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            If ListBox1.SelectedIndex = -1 Then Exit Sub
            Dim msg As Integer
            msg = MsgBox(ListBox1.Items.Count, vbYesNo, "temporarily delete all?")

            If msg = vbYes Then
                ListBox1.Items.Clear()
                DeleteAllToolStripMenuItem.Text = "reload"
            End If

            Exit Sub

        End If

        deleteDbItmAll()
    End Sub

    Sub skin()
        mainColorPick()
        Me.Visible = True
        timeout2(111)
        TabControl1.Visible = False
        reStack()
        showTab(3)
        TabControl1.Visible = True
        ListBox1.Width = txtString.Width

        If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable And Me.ControlBox = False Then sizeable()
        If My.Settings.SettingScrollBar = False Then showScrollBar(False)

        If My.Settings.SettingGCCollect = True Then GC.Collect()
    End Sub

    Private Sub ExportToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles ExportToolStripMenuItem.MouseDown
        ContextMenuStripDb.Hide()

        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            If ListBox1.Items.Count = 0 Then Exit Sub
            Dim q1 = MsgBox("db: " & ListBox1.Items.Count & " items" & vbNewLine & "escape: cancel, backup option", vbYesNo, "database to text?")
            If q1 = vbYes Then
                Me.Text = "dna > exporting" 'caption

                If ListBox1.Items.Count <> My.Settings.Settingdb.Count Or DeleteAllToolStripMenuItem.Text = "reload" Or
                    ListBox1.Items.Item(0).ToString <> My.Settings.Settingdb.Item(0).ToString Or
                        ListBox1.Items.Item(0).ToString.Length <> My.Settings.Settingdb.Item(0).ToString.Length Then
                Else
                    reloadDb() 'ws
                End If

                txtStringClear()

                Dim enl As String = My.Settings.SettingExportNewLine.ToString

                wsScan()
                If containsws_g = True Or My.Settings.SettingIgnoreWhiteSpace = True Then enl = My.Settings.SettingExportNewLine.ToString Else enl = ""

                For i = 0 To ListBox1.Items.Count - 1
                    If GetAsyncKeyState(Keys.Escape) Then Exit For
                    If i = ListBox1.Items.Count - 1 Then
                        If ListBox1.Items.Item(i).ToString > "" Then txtString.Text += ListBox1.Items(i) & enl  'add last item to txtbox
                    Else
                        If ListBox1.Items.Item(i).ToString > "" Then txtString.Text += ListBox1.Items(i) & enl & vbNewLine 'add item to txtbox
                    End If
                    Me.Text += "." 'animate me.text exporting..
                    If Len(Me.Text) > 60 Then Me.Text = "dna > exporting" 'caption reset
                Next

                dnaTxt() 'caption 

                'backup option
                If Me.ListBox1.Items.Count <= 0 Then Exit Sub
                Dim q = MsgBox("make a backup also?" & vbNewLine, vbYesNo)
                If q = MsgBoxResult.Yes Then exportListToTxt1()
                txtString.Focus()
                txtString.SelectAll()
            End If
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            export_mr()
        End If
    End Sub

    Sub export_mr()
        If ListBox1.Items.Count = 0 Then Exit Sub
        Dim path = VirtualStore(False, True)
        Dim q = MsgBox(path, vbYesNo, "export database to .txt file?")
        If q = MsgBoxResult.Yes Then exportListToTxt1()
    End Sub

    Sub checkIfOn()
        If chk_timer1_on_val.Checked = True And Timer1.Interval > 0 Then
            Me.Hide()
        Else
            MsgBox("engine must be on...", vbExclamation)
            Exit Sub
        End If
    End Sub

    Private Sub HideToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles HideToolStripMenuItem.MouseDown
        ContextMenuStripOptions.Close()

        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            checkIfOn() 'hide
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right Then 'start hidden
            chkItem(HideToolStripMenuItem)
            If My.Settings.SettingHidden = True Then
                My.Settings.SettingHidden = False
                HideToolStripMenuItem.Checked = False
                gSettingHidden = False
            Else
                My.Settings.SettingHidden = True
                gSettingHidden = True
                HideToolStripMenuItem.Checked = False
            End If
            Me.Visible = True
        End If
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        If txtString.Text = "" Then Exit Sub
        If txtString.SelectedText = "" Then Clipboard.SetText(txtString.Text) Else Clipboard.SetText(txtString.SelectedText) 'copy all or selected text
    End Sub

    Private Sub CopyToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles CopyToolStripMenuItem.MouseDown

        ContextMenuStripString.Close()

        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            If txtString.Text = "" Then Exit Sub
            If txtString.SelectedText = "" Then Clipboard.SetText(txtString.Text) Else Clipboard.SetText(txtString.SelectedText) 'copy all or selected text
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right Then 'print < or >
            If txtString.Text = "" Then
                txtString.Text = ">"
                txtString.SelectionStart = 1
                Exit Sub
            End If
            If txtString.Text > "" Then
                Dim p As Integer = txtString.SelectionStart
                Dim t As String

                If GetChar(txtString.Text, 1) = ">" Then '<
                    t = Replace(txtString.Text, ">", "<", 1, 1)
                    txtStringClear()
                    txtString.AppendText(t)
                    txtString.SelectionStart = p
                    Exit Sub
                End If
                If GetChar(txtString.Text, 1) = "<" Then '>
                    t = Replace(txtString.Text, "<", ">", 1, 1)
                    txtStringClear()
                    txtString.AppendText(t)
                    txtString.SelectionStart = p
                    Exit Sub
                End If

                t = txtString.Text '
                txtStringClear()
                txtString.AppendText(">" + t)
                txtString.SelectionStart = p + 1
            End If

        End If
    End Sub

    Sub printXY()
        Dim rz = txtString.ZoomFactor  're-zoom
        If My.Settings.SettingDnaX = True Then tipsDnaToolStripMenuItem.Checked = False

        NullToolStripMenuItem1.Text = "«xy"
        leftrelease()
        timeout2(333)

        print("«", False)
        w7(33)
        print("xy:" & lblX.Text & "-" & lblY.Text, False)
        w7(33)
        print("»", False)
        w7(33)

        timeout2(333)
        print("«xy:" & lblX.Text & "-" & lblY.Text & "»", True)

        If My.Settings.SettingScrollBar = False Then showScrollBar(False)

        tipsDnaToolStripMenuItem.Checked = My.Settings.SettingDnaX

        txtString.ZoomFactor = rz 're-zoom
    End Sub

    Private Sub XyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XyToolStripMenuItem.Click
        print("«", False)
        w7(33)
        print("xy:" & lblX.Text & "-" & lblY.Text, False)
        w7(33)
        print("»", False)
        w7(33)

        w7(33)
        print("«", True)
        w7(33)
        print("xy:" & lblX.Text & "-" & lblY.Text & "»", True)

    End Sub

    Private Sub XyToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles XyToolStripMenuItem.MouseDown
        Dim br = My.Settings.SettingMoveBar 'get movebar stat

        ContextMenuStripString.Hide()
        Dim ft = txtString.ZoomFactor
        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            Me.Text = "dna"
            timeout1(1)
            AppActivate("dna")
            txtString.Focus()
            printXY()
            reStyle()
        End If
        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            Me.Text = "dna"
            timeout1(1)
            AppActivate("dna")
            txtString.Focus()
            runMousePosition()
            reStyle()
        End If

        If br = True Then moveBarRe() 'reset mbar stat
        txtString.ZoomFactor = ft
    End Sub

    Private Sub ToolStripMenuItem28_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem28.Click
        printShortMenu("«+:»", True, False)
    End Sub

    Private Sub ToolStripMenuItem29_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem29.Click
        printShortMenu("«-:»", True, False)
    End Sub

    Private Sub ToolStripMenuItem30_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem30.Click
        printShortMenu("«++»", False, False)
    End Sub

    Private Sub ToolStripMenuItem31_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem31.Click
        printShortMenu("«--»", False, False)
    End Sub

    Private Sub CbToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CbToolStripMenuItem.Click
        If chk_tips.Checked = True Then
            skMenuGet("«clipboard:»{left}")
        Else
            skMenuGet("«cb:»{left}")
        End If
    End Sub

    Private Sub SkinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SkinToolStripMenuItem.Click
        skin()
    End Sub

    Private Sub SkinToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles SkinToolStripMenuItem.MouseDown
        ContextMenuStripDb.Hide()
        If MouseButtons = Windows.Forms.MouseButtons.Left Then skin()
        If MouseButtons = Windows.Forms.MouseButtons.Right Then changeFont(True)

        If SplitContainer1.BorderStyle = BorderStyle.FixedSingle And My.Settings.SettingScrollBar = True Then 'border
            SplitContainer1.Left = 4
        End If
    End Sub

    Private Sub PauseToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles PauseToolStripMenuItem2.Click
        chkItem(PauseToolStripMenuItem2)
        My.Settings.SettingIgnorePause = PauseToolStripMenuItem2.CheckState
    End Sub

    Private Sub ExitToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles ExitToolStripMenuItem.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            saveSettings()
            Application.Restart()
        End If
        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            ContextMenuStripOptions.Hide()
            Close()
        End If
    End Sub

    Private Sub OnToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles OnToolStripMenuItem.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            chkItem(chk_timer1_on_val) 'tog on
            My.Settings.SettingTimer1_chk_on_val = chk_timer1_on_val.CheckState
            If chk_timer1_on_val.Checked = True Then emode()
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            If TabControl1.Visible = False Then 'toggle listbox
                TabControl1.Visible = True
                txtString.Visible = True
            Else
                If BackColor = Color.GhostWhite And My.Settings.SettingBgImgMain = "" Then
                    If chk_tips.Checked = True Then MsgBox("«st + enter: toggle tab")
                    ContextMenuStripOptions.Hide()
                    Exit Sub
                End If
                TabControl1.Visible = False
            End If
        End If
    End Sub

    Sub tabCompleteTxt(shortTxt As String, fullTxt As String) 'auto complete in beginning of txtstring', outsideBeginnig As Boolean
        If txtString.TextLength = shortTxt.Length Then 'print in beginning only
            txtFinish1(shortTxt, fullTxt)
            Exit Sub
        End If
    End Sub

    Private Sub lblLength_DoubleClick(sender As Object, e As EventArgs) Handles lblLength.DoubleClick
        If txtLength.Visible = False Then
            ContextMenuStripNull.Show(MousePosition)
        Else
            LengthToolStripMenuItem.PerformClick()
        End If
    End Sub

    Private Sub HideToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles HideToolStripMenuItem.MouseLeave
        If chk_tips.Checked = True Then
            ToolTip1.Active = True
        Else
            ToolTip1.Active = False
            HideToolStripMenuItem.ToolTipText = ""
        End If
    End Sub

    Private Sub HideToolStripMenuItem_MouseMove(sender As Object, e As MouseEventArgs) Handles HideToolStripMenuItem.MouseMove
        If My.Settings.SettingHidden = True Then 'tt
            ToolTip1.Active = True
            If chk_tips.Checked = True Then
                HideToolStripMenuItem.ToolTipText = "start hidden: true" & vbNewLine & "escape + h: hide/show" & vbNewLine & "right click: toggle start hidden"
            Else
                If My.Settings.SettingOptionsHideTipShow = True Then HideToolStripMenuItem.ToolTipText = "start hidden: true" '& vbNewLine & "right click: toggle start hidden"
            End If
        End If
    End Sub

    Private Sub CopyToolStripMenuItem1_MouseDown(sender As Object, e As MouseEventArgs) Handles CopyToolStripMenuItem1.MouseDown
        If MouseButtons = Windows.Forms.MouseButtons.Left Then
            If ListBox1.SelectedItem = Nothing Then Else If ListBox1.SelectedItem() > "" Then Clipboard.SetText(ListBox1.SelectedItem()) 'copy item
        End If

        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            If Clipboard.GetText = "" Then Exit Sub
            Dim c As String = Clipboard.GetText
            If Clipboard.GetText.Length + 1 > txtLength.Text Then 'filter / add chr(9) to cb
                If Not Microsoft.VisualBasic.Mid(Clipboard.GetText, My.Settings.SettingTxtCodeLength + 1, 1) = Chr(9) And Clipboard.GetText.Length > My.Settings.SettingTxtCodeLength Then If Clipboard.GetText.StartsWith("http") Or Clipboard.GetText.StartsWith("«") Or Clipboard.GetText.StartsWith("'") Or Clipboard.GetText.StartsWith("//") Or Clipboard.GetText.EndsWith("//") Then Else Clipboard.SetText(Microsoft.VisualBasic.Left(Clipboard.GetText, My.Settings.SettingTxtCodeLength) & Chr(9) & Microsoft.VisualBasic.Right(Clipboard.GetText, Clipboard.GetText.Length - My.Settings.SettingTxtCodeLength)) 'if missing tab, reinsert
                timeout2(222)
            End If

            'clipboard to db
            If ListBox1.SelectedIndex = ListBox1.Items.Count - 1 And ListBox1.Items.Count <> 0 Then
                ListBox1.Items.Insert(ListBox1.Items.Count - 1, Clipboard.GetText)
                My.Settings.Settingdb.Insert(ListBox1.Items.Count - 2, Clipboard.GetText)
                Clipboard.SetText(c)
                ListBox1.SelectedIndex = ListBox1.Items.Count - 2
                Exit Sub 'last
            End If

            If ListBox1.SelectedIndex = ListBox1.Items.Count - 1 Or ListBox1.SelectedIndex = -1 Then
                ListBox1.Items.Add(Clipboard.GetText)
                My.Settings.Settingdb.Add(Clipboard.GetText)
                Clipboard.SetText(c)
                selectTopItem()
                Exit Sub
            End If
            If ListBox1.SelectedIndex <= 0 Then
                ListBox1.Items.Insert(ListBox1.SelectedIndex, Clipboard.GetText)
                My.Settings.Settingdb.Insert(ListBox1.SelectedIndex - 1, Clipboard.GetText)
                Clipboard.SetText(c)
                selectTopItem()
                Exit Sub '1st
            End If
            ListBox1.Items.Insert(ListBox1.SelectedIndex, Clipboard.GetText)
            My.Settings.Settingdb.Insert(ListBox1.SelectedIndex - 1, Clipboard.GetText)
            Clipboard.SetText(c)
            ListBox1.SelectedIndex = ListBox1.SelectedIndex - 1
            Exit Sub 'mid
            '
        End If
    End Sub

    Sub dnauserconfig()
        Me.Opacity = 0
        chk_tips.Checked = False
        txtLength.Text = 4

        sc = My.Settings.SettingChangeColor

        My.Settings.SettingForeColor = sc
        Me.ListBox1.ForeColor = sc
        Me.txtString.ForeColor = sc
        Me.txtLength.ForeColor = sc
        Me.ForeColor = sc
        TabPage1.ForeColor = sc
        TabPage2.ForeColor = sc
        TabPage4.ForeColor = sc

        My.Settings.SettingBgColor = Color.Black
        Me.ListBox1.BackColor = Color.Black
        Me.txtString.BackColor = Color.Black
        Me.txtLength.BackColor = Color.Black

        My.Settings.SettingTabColor = Color.Black
        Me.TabPage3.BackColor = Color.Black
        Me.TabPage2.BackColor = Color.Black
        Me.TabPage1.BackColor = Color.Black
        Me.TabPage4.BackColor = Color.Black

        My.Settings.SettingMainBgColor = Color.Black
        Me.BackColor = Color.Black

        My.Settings.SettingBorder = False
        My.Settings.SettingBorder2 = True
        My.Settings.SettingSplitterWidth = 33
        txtString.BorderStyle = BorderStyle.None
        ListBox1.BorderStyle = BorderStyle.None
        SplitContainer1.BorderStyle = BorderStyle.FixedSingle
        SplitContainer1.SplitterWidth = 33

        My.Settings.SettingScrollBar = False
        showScrollBar(False)

        My.Settings.SettingExportToOneDrive = True
        My.Settings.SettingAutoRetryAppError = True

        chkMisc.Checked = False
        My.Settings.SettingChkMiscComma = False
        ToolStripMenuItemChkMiscComma.Checked = False

        chkAz.Checked = False
        chk09.Checked = False
        chkOther.Checked = False
        My.Settings.SettingRctrleqdot = True
        My.Settings.SettingRctrleqMod = "«"
        RightCtrllToolStripMenuItem.Checked = True
        lbl_timer1_interval_val.Text = 150
        chk_timer1_on_val.Checked = True
        SplitContainer1.SplitterWidth = 18

        path = VirtualStore(False, True) 'bg
        Dim charRange = My.Settings.SettingExportToOneDriveDir
        Dim startIndex As Integer = path.ToString.IndexOf(charRange)
        path = path.ToString.Replace(Microsoft.VisualBasic.Right(path, startIndex), "")
        Dim itm As String = (Microsoft.VisualBasic.Left(path, startIndex))
        Try
            Me.BackgroundImage = Image.FromFile(itm + My.Settings.SettingExportToOneDriveDir + "\dna.png")
            My.Settings.SettingBgImgMain = itm + My.Settings.SettingExportToOneDriveDir + "\dna.png"
        Catch ex As Exception
            editMainBgImg()
        End Try

        If GetAsyncKeyState(Keys.Escape) Then
            Try
                txtString.Font = New System.Drawing.Font("Impact", 15.75)
                ListBox1.Font = New System.Drawing.Font("Impact", 15.75)
                Me.Font = New System.Drawing.Font("Impact", 15.75)
                My.Settings.SettingFont = New System.Drawing.Font("Impact", 15.75)
                txtString.ZoomFactor = 1
                tabOnly()
                SplitContainer1.SplitterWidth = 33
                My.Settings.SettingOpacity = 0.7
                My.Settings.SettingDbTip = True
            Catch ex As Exception
                txtString.Font = Nothing
                ListBox1.Font = Nothing
                Me.Font = Nothing
                My.Settings.SettingFont = Nothing
                My.Settings.SettingLstFontSize = Nothing
                txtString.ZoomFactor = 1
            End Try
        End If

        changeView()
        tabOnly()

        Try
            Me.Width = Me.BackgroundImage.Width
            Me.Height = Me.BackgroundImage.Height
            If GetAsyncKeyState(Keys.Escape) And ListBox1.Font.Size = 15.75 And ListBox1.Font.Name = "Impact" Then Me.SplitContainer1.Height = TabControl1.Height - 48
        Catch ex As Exception
        End Try

        My.Settings.SettingScrollLockRun = True
        g_scroll = True


        SplitContainer1.SplitterDistance = 28
        dbfocus()

        tipsDnaToolStripMenuItem.Checked = False
        My.Settings.SettingDnaX = False
        chkWedgee.Checked = False
        VolumeUpToolStripMenuItem.Checked = False
        My.Settings.SettingIgnoreVoluemUp = False
        VolumeDownToolStripMenuItem.Checked = False
        My.Settings.SettingIgnoreVolumeDown = False
        My.Settings.SettingChkDragToExtendedScreen = True

        Me.CenterToScreen()
        My.Settings.SettingShowIcon = True
        Me.ShowIcon = True
        If My.Settings.SettingOpacity <= 0.1 Then My.Settings.SettingOpacity = 1
        Me.Opacity = My.Settings.SettingOpacity
    End Sub
    Sub dbfocus()
        TabControl1.Focus()
        key(Keys.Left)
        keyClear(Keys.Left)
        key(Keys.Right)
        keyClear(Keys.Right)
        timeout2(1)
        txtString.Focus()
    End Sub
    Sub editMainBgImg()
mainimg:
        Dim img1 = MsgBox("main background image: " & LCase(My.Settings.SettingBgImgMain.ToString) & vbNewLine &
                         "main background color: " & LCase(My.Settings.SettingMainBgColor.ToString) & vbNewLine &
                         "escape: change font (impact/15)", vbYesNoCancel, "change main background image or color?") ' & vbNewLine & vbNewLine & "tip: main background image can also be set to a .htm or .html file"
        If img1 = MsgBoxResult.Cancel Then Exit Sub
        If img1 = MsgBoxResult.Yes Then
            If (OpenFileDialog1.ShowDialog() = DialogResult.OK) Then
                If OpenFileDialog1.FileName.EndsWith(".htm") Or OpenFileDialog1.FileName.EndsWith(".html") Then 'html bg
                    webBg(OpenFileDialog1.FileName, True, 2)
                    My.Settings.SettingHtmlBg = OpenFileDialog1.FileName.ToString 'settings html bg
                Else
                    Try
                        Me.BackgroundImage = Image.FromFile(OpenFileDialog1.FileName) 'img bg
                        My.Settings.SettingBgImgMain = OpenFileDialog1.FileName
                    Catch ex As Exception
                        MsgBox("error")
                        GoTo mainimg
                    End Try
                End If
            ElseIf DialogResult.Cancel Then
                If WebBrowser1.Visible = True And TabPage3.Text = "db" Then WebBrowser1.Visible = False
                If My.Settings.SettingHtmlTab > "" Then WebBrowser1.Visible = True
                My.Settings.SettingBgImgMain = ""
                Me.BackgroundImage = Nothing
                My.Settings.SettingHtmlBg = "" 'settings html bg

                'cancel goto main bg img color
                If My.Settings.SettingBgImgMain = "" Then
                    If ColorDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Me.BackColor = ColorDialog1.Color
                        My.Settings.SettingMainBgColor = ColorDialog1.Color
                    ElseIf DialogResult.Cancel Then
                        My.Settings.SettingMainBgColor = Nothing
                        Me.BackColor = Nothing
                    End If
                End If

            End If
        End If

    End Sub

    Sub ignoreWhiteSpacef()
        If My.Settings.SettingIgnoreWhiteSpace = False Then
            My.Settings.SettingIgnoreWhiteSpace = True
        Else
            My.Settings.SettingIgnoreWhiteSpace = False
        End If
        ignoreWhiteSpace_g = My.Settings.SettingIgnoreWhiteSpace
        If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(ws + enter)" & vbNewLine & "ignore white space: " & LCase(My.Settings.SettingIgnoreWhiteSpace), vbInformation, "dna.exe.config: SettingIgnoreWhiteSpace")
        masterClear()
    End Sub
    Sub masterClear()
        TextBox1.Clear()
        clearAllKeys()
        emode()
        ToolTip1.Hide(Me)
    End Sub
    Sub noLengthMode()
        If My.Settings.SettingNoLengthMode = False Then
            My.Settings.SettingNoLengthMode = True
            txtLength.Visible = False
        Else
            My.Settings.SettingNoLengthMode = False
            txtLength.Visible = True
        End If
        NoLengthToolStripMenuItem.Checked = My.Settings.SettingNoLengthMode
        If chk_tips.Checked = True Or My.Settings.SettingShowSettingsTips = True Then MsgBox("(nl + enter)" & vbNewLine & "no length run mode: " & LCase(My.Settings.SettingNoLengthMode), vbInformation, "dna.exe.config: SettingNoLengthMode (dna > «)")
        masterClear()
    End Sub
    Private Sub NoLengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NoLengthToolStripMenuItem.Click
        chkItem(NoLengthToolStripMenuItem)
        noLengthMode()
    End Sub

    Private Sub lblLength_MouseDown(sender As Object, e As MouseEventArgs) Handles lblLength.MouseDown
        If MouseButtons = MouseButtons.Right Then
            ttAdjust()
            ContextMenuStripNull.Show(MousePosition)
        End If
    End Sub

    Private Sub PSToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles PSToolStripMenuItem3.Click
        chkItem(PSToolStripMenuItem3) 'print screen
        My.Settings.SettingIgnorePS = PSToolStripMenuItem3.CheckState
    End Sub

    Private Sub OnToolStripMenuItem_MouseMove(sender As Object, e As MouseEventArgs) Handles OnToolStripMenuItem.MouseMove
        If Me.FormBorderStyle = FormBorderStyle.None And chk_tips.Checked = False And chk_timer1_on_val.Checked = True And My.Settings.SettingOptionsOnTipShow = True Then OnToolStripMenuItem.ToolTipText = "dna > " & TextBox1.Text
    End Sub

    Private Sub TabPage1_MouseMove(sender As Object, e As MouseEventArgs) Handles TabPage1.MouseMove, TabPage2.MouseMove, TabPage3.MouseMove, TabPage4.MouseMove
        showCursor()
    End Sub

    Private Sub PauseBreakToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PauseBreakToolStripMenuItem.Click
        chkItem(PauseBreakToolStripMenuItem)
        My.Settings.SettingIgnorePauseBreak = PauseBreakToolStripMenuItem.CheckState
    End Sub

    Private Sub LscrollToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LscrollToolStripMenuItem.Click
        chkItem(LscrollToolStripMenuItem)
        My.Settings.SettingLscroll = LscrollToolStripMenuItem.CheckState
    End Sub

    Private Sub RscrollToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RscrollToolStripMenuItem.Click
        chkItem(RscrollToolStripMenuItem)
        My.Settings.SettingRscroll = RscrollToolStripMenuItem.CheckState
    End Sub

    Private Sub LengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LengthToolStripMenuItem.Click
        Dim ctop As Boolean = chk_top.CheckState 'invert top
        If ctop = True Then chk_top.Checked = False
        t1 = InputBox("dna > 'key length'", "change length?", My.Settings.SettingTxtCodeLength)
        If ctop = True Then chk_top.Checked = True
        If IsNumeric(t1) Then
            My.Settings.SettingTxtCodeLength = t1
            txtLength.Text = t1
        End If
    End Sub

    Private Sub LengthToolStripMenuItem_MouseDown(sender As Object, e As MouseEventArgs) Handles LengthToolStripMenuItem.MouseDown
        If MouseButtons = MouseButtons.Right Then
            ContextMenuStripDb.Hide()
            NoLengthToolStripMenuItem.PerformClick()
        End If
    End Sub
End Class
