Option Explicit Off

Imports System.ComponentModel
Imports System.Threading
Imports HyperLib



Public Class Form1


    Shared a, b As Hyper
    Friend Shared OALprec% = 900
    Dim lastTextInp$ = ""

    Function olepšaj(s$) As String

        s1$ = ""
        If Strings.Left(s, 1) <> "-" Then
            If InStr(s, "e") Then
                s1 = s.TrimStart("0")
            ElseIf InStr(s, ".") Then
                s1 = s.Trim("0")
            Else
                s1 = s.TrimStart("0")
            End If

            If Strings.Left(s1, 1) = "." Then s1 = "0" + s1

        Else
            s1$ = s.Remove(0, 1)
            If InStr(s, "e") Then
                s1 = s1.TrimStart("0")
            ElseIf InStr(s, ".") Then
                s1 = s1.Trim("0")
            Else
                s1 = s1.TrimStart("0")
            End If
            If Strings.Left(s1, 1) = "." Then
                s1 = "-0" + s1
            Else
                s1 = "-" + s1

            End If
        End If

        If Strings.Right(s1, 1) = "." Then
            Return Strings.Left(s1, Len(s1) - 1)
        Else
            Return s1
        End If
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


        Try
            If IsNothing(a) Then GoTo npr
            LdInput()


            Select Case ComboBox1.SelectedIndex
                Case 0
                    a.Add(b)
                Case 1
                    a.Subtract(b)
                Case 2

                    a *= b


                Case 3

                    If chkINT(b) Then
                        a.Divide(b(0), PrecOnDivByInt64)
                    Else

                        a = DivisionXL(a, b, XLdivPrec) ', nIterationsAtDiv)
                    End If

                Case 4
                    If chkINT(a) Then
                        x& = a.Divide(b(0), PrecOnDivByInt64)
                        a = New Hyper(0, 0)
                        a(0) = x
                    Else
                        MsgBox("The second operand is not an Int64.")
                    End If

                Case 5

                    a = OpEx.NthPower(a, b(0), OALprec)
                Case 6

                    a = OpEx.NthRoot(a, b(0))
                Case -1
                    a = b

            End Select
            GoTo skipDbl

            d# = 0
            s3$ = ""
            Try
                a.ExtractDouble(d)
                s3$ = d
            Catch ex As Exception
                s3$ = "out of rng"
            End Try
            txMonitor.Text = s3

skipDbl:


            s2$ = ""

            a.StripZeros()
            prepareA()
            s2$ = olepšaj(a.ToString)

            If CheckBox1.Checked Then


                If TextBox2.TextLength >= 3 Then

                    If UCase(Strings.Left(TextBox2.Text, 2)) = "MC" Then

                        printout(vbCrLf & "M" & txMems.SelectedIndex & " = 0")
                        Exit Sub
                    ElseIf UCase(Strings.Left(TextBox2.Text, 2)) = "MR" Then
                        printout("= " & ComboBox1.SelectedText & " M" & txMems.SelectedIndex)
                        Exit Sub

                    End If

                End If



                If TextBox2.TextLength Then lastTextInp = TextBox2.Text
                If ComboBox1.SelectedIndex = 6 Then

                    rtx.SelectionStart = rtx.TextLength
                    rtx.SelectedText = vbCrLf
                    rtx.SelectionCharOffset = 10
                    rtx.SelectedText = b(0)
                    rtx.SelectionStart = rtx.TextLength
                    rtx.SelectionCharOffset = 0
                    rtx.AppendText("√" + stringOut + " = " & vbCrLf &
s2)
                    rtx.SelectionCharOffset = 10
                    rtx.ScrollToCaret()

                Else
                    rtx.AppendText(stringOut & " " & ComboBox1.Text & vbCrLf &
                        lastTextInp & " = " & vbCrLf & s2)

                    End If
                    stringOut = ""
                    rtx.SelectionStart = rtx.TextLength
                    rtx.ScrollToCaret()
                End If

                TextBox1.Text = s2
            TextBox2.Text = ""
            TextBox2.Focus()

            izpis()
            Exit Sub

npr:

            a = New Hyper(TextBox2.Text)
            TextBox2.Text = ""
            izpis()
            prepareA()
            s4$ = olepšaj(a.ToString)
            TextBox1.Text = s4
            ComboBox1.SelectedItem = Nothing

            If CheckBox1.Checked Then

                stringOut = s4

            End If
            TextBox2.Focus()


        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
        End Try

    End Sub

    Sub izpis()
        If IsNothing(a) Then
            Label1.Text = ""
            Label2.Text = ""
        Else
            Label1.Text = "Low Exp: " & a.GetBottExp & "        Buffer size: " & a.BufferSize
            Label2.Text = "High Exp: " & a.GetTopExp
        End If

    End Sub

    Function chkINT(h As Hyper) As Boolean
        If h.GetBottExp Or h.GetTopExp Then
            Return False
        Else
            Return True

        End If
    End Function



    Private ms As New List(Of Hyper)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Hyper.displayMode = Hyper.displayModeType.inTrueDecimal
        Hyper.ExtraDigitsForConversionFromDecimal = 22
        Hyper.QuotientPrecision = 3333

        txMems.SelectedIndex = 0
        loadSettings()
        For i% = 0 To 5
            ms.Add(New Hyper(0, 0))
        Next

    End Sub

    Private Sub loadSettings()
        Hyper.ExtraDigitsForConversionFromDecimal = My.Settings.ConversionDecPrec
        Hyper.maxDigitsInString = My.Settings.DisplayDigitsDec
        Hyper.QuotientPrecision = My.Settings.QuotientPrec
        nIterationsAtSqr = My.Settings.nIterationsRoots

        If My.Settings.ApplyOverallPrec Then
            OALprec% = My.Settings.OverallPrecision
        Else
            OALprec = 2000000
        End If

        PrecOnDivByInt64% = My.Settings.QuotientPrec
        nIterationsAtDiv% = My.Settings.nIterations
        NthRootPrec = My.Settings.RootPrec

        Op.AssignDivMethod(My.Settings.DivMethodType)
        OpEx.AssignSQRmethod(My.Settings.SqrMethodType)

    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If e.Alt OrElse e.Control Then Exit Sub

        Dim unclear As Boolean = TextBox2.TextLength
        e.Handled = True
        e.SuppressKeyPress = True

        Select Case e.KeyCode
            Case Keys.Oemplus, Keys.Add
                If TextBox2.SelectionStart Then Button1.PerformClick()
                ComboBox1.SelectedIndex = 0
            Case Keys.Subtract, Keys.OemMinus

                With TextBox2

                    txt$ = TextBox2.Text

                    If .SelectionStart Then
                        If UCase(Mid(txt, .SelectionStart, 1)) = "E" Then
                            e.Handled = False
                            e.SuppressKeyPress = False
                            Exit Select
                        End If

                        Button1.PerformClick()
                        ComboBox1.SelectedIndex = 1
                    Else
                        e.Handled = False
                        e.SuppressKeyPress = False

                    End If
                End With

            Case Keys.Multiply, e.Shift And Keys.D8
                If TextBox2.SelectionStart Then Button1.PerformClick()
                ComboBox1.SelectedIndex = 2
            Case Keys.Divide, 191

                If TextBox2.SelectionStart Then Button1.PerformClick()
                ComboBox1.SelectedIndex = 3

            Case e.Shift And Keys.D6
                If TextBox2.SelectionStart Then Button1.PerformClick()
                ComboBox1.SelectedIndex = 5


            Case Keys.M
                If TextBox2.SelectionStart Then
                    Button1.PerformClick()
                    ComboBox1.SelectedIndex = 4

                Else
                    e.Handled = False
                    e.SuppressKeyPress = False
                    Exit Select

                End If

            Case Keys.S, Keys.C, Keys.R
                If TextBox2.SelectionStart = 1 AndAlso
UCase(Strings.Left(TextBox2.Text, 1)) = "M" Then
                    e.Handled = False
                    e.SuppressKeyPress = False

                End If


            Case Keys.Escape
                If unclear Then
                    TextBox2.Text = ""
                    TextBox2.Focus()

                Else
                    TextBox1.Clear()
                    a = Nothing

                    izpis()
                    TextBox2.Focus()
                    ComboBox1.SelectedIndex = -1
                End If
            Case Keys.Enter

                Button1.PerformClick()

                Exit Sub

            Case Keys.D0 To Keys.D9, Keys.NumPad0 To Keys.NumPad9, Keys.OemPeriod, Keys.Space

                If e.Shift Then Exit Sub
                e.Handled = False
                e.SuppressKeyPress = False
                TextBox2.Focus()


            Case Keys.Back, Keys.Delete, Keys.Up, Keys.Down, Keys.Left, Keys.Right,
Keys.Home, Keys.End, Keys.E

                e.Handled = False
                e.SuppressKeyPress = False

            Case Keys.Oemcomma
                SendKeys.Send(".")
                Exit Sub



        End Select


    End Sub

    Sub printout(s$)

        If CheckBox1.Checked Then

            rtx.AppendText(s)

            rtx.SelectionStart = rtx.TextLength
            rtx.ScrollToCaret()
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Settings.ShowDialog() = vbOK Then
            My.Settings.Reload()
            loadSettings()
        End If
    End Sub


    Private Sub MSbtn_Click(sender As Object, e As EventArgs) Handles MSbtn.Click

        If UCase(Strings.Left(TextBox2.Text, 1)) = "M" Then
            TextBox2.Text = "MS" & txMems.SelectedIndex
            apply()



        ElseIf TextBox2.TextLength AndAlso IsNothing(a) Then

            LdInput()
            a = b
            ms(txMems.SelectedIndex) = b.Clone
            TextBox2.SelectAll()
            TextBox2.Focus()


        ElseIf TextBox2.TextLength Then
            LdInput()
            ms(txMems.SelectedIndex) = b.Clone
            TextBox2.SelectAll()
            TextBox2.Focus()

        Else
            TextBox2.Text = "MS" & txMems.SelectedIndex
            apply()
        End If

    End Sub

    Private Sub MRbtn_Click(sender As Object, e As EventArgs) Handles MRbtn.Click



        TextBox2.Text = "MR" & txMems.SelectedIndex
        apply()
    End Sub

    Private Sub apply()
        TextBox2.SelectionStart = TextBox2.TextLength
        TextBox2.Focus()

    End Sub
    Private Sub MCbtn_Click(sender As Object, e As EventArgs) Handles MCbtn.Click

        TextBox2.Text = "MC" + txMems.SelectedText.ToString
        apply()
    End Sub




    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        LdInput()
        a = SqrXL(b)
        finishUp()

    End Sub



    Dim nIterationsPi% = 61


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        ms(txMems.SelectedIndex) += a

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ms(txMems.SelectedIndex) -= a

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click


        Try
            LdInput()

napr:
            a = b * b
            finishUp()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)

        End Try

    End Sub

    Function prepareA$()
        hiExp% = a.GetTopExp
        If hiExp < 0 Then
            If a(hiExp) Then

                a(hiExp + 1) = 0
            End If
        End If

    End Function

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        LdInput()
        a = NthRoot(b, 3)
        prepareA()
        TextBox1.Text = a.ToString
        TextBox2.Clear()
    End Sub


    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click

        n% = 3
        LdInput()

        a = b.Clone

        For i = 2 To n
            a *= b
        Next

        finishUp()

    End Sub

    Dim stringOut$ = ""

    Sub outputA(final As Boolean)


        prepareA()
        s2$ = olepšaj(a.ToString)
        TextBox1.Text = s2
        TextBox2.Clear()

        If CheckBox1.Checked Then

            If TextBox2.TextLength Then lastTextInp = TextBox2.Text

            If ComboBox1.SelectedIndex = 6 Then

                rtx.SelectionCharOffset = 10
                rtx.SelectedText = b(0)

            Else
                rtx.AppendText(" " & ComboBox1.Text & vbCrLf &
            lastTextInp & " = " & vbCrLf & s2)

                rtx.SelectionStart = rtx.TextLength
                rtx.ScrollToCaret()
            End If


        Else

        End If

    End Sub
    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        Button1.PerformClick()
        ComboBox1.SelectedIndex = 6

    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        ComboBox1.SelectedIndex = -1
        Button1.PerformClick()
        ComboBox1.SelectedIndex = 5
        finishUp()

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        LdInput()
        a = NthRoot(b, 3)
        finishUp()

    End Sub


    Sub finishUp()
        a.StripZeros()
        prepareA()

        s2$ = olepšaj(a.ToString)
        TextBox1.Text = s2
        TextBox2.Clear()
        TextBox2.Focus()

    End Sub

    Private Sub LdInput()

        Try
            'If IsNothing(a) Then GoTo npr
            txt$ = TextBox2.Text

            s$ = txt.Replace(" ", "")

            If UCase(Strings.Left(s, 1)) = "M" Then
                txt = Mid(s, 2)
                Select Case UCase(Strings.Left(txt, 1))
                    Case "S"
                        txt = Mid(s, 3)
                        ms(txt) = a.Clone
                        '-printout(vbCrLf & " = M" & txMems.SelectedIndex)

                    Case "C"
                        txt = Mid(s, 3) ' : memNr% = txt
                        ms(txt) = New Hyper(0, 0)
                        '-printout(vbCrLf & "M" & txMems.SelectedIndex & " = 0")

                    Case "R"
                        txt = Mid(s, 3) ' : memNr% = txt
                        b = ms(txt).Clone
                End Select
                Exit Sub
            End If

            If s.Length Then
                b = New Hyper(s)

            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)


        End Try


    End Sub

End Class
