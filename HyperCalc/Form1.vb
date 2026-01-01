Option Explicit Off

'Imports System
Imports System.ComponentModel
Imports System.Threading
Imports HyperLib



Public Class Form1


    Shared a, b As Hyper
    Shared XLmulPrec% = 900
    Shared bbuff&()
    Dim OALprec% = 900
    Dim PrecOnDivByInt64% = 900
    Dim nIterationsAtDiv% = 1444
    Dim nIterationsAtSqr% = 144
    Dim XLdivPrec% = 44
    Dim lastTextInp$ = ""
    Shared NthRootPrec% = 80

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


        ' Dim one As New Hyper(".5")
        'Dim d1 As New Hyper("0.5")
        'one(0) = 1
        'one *= d1 ' * one
        ' d1 *= one
        'MsgBox(d1.ToString)


        Try
            If IsNothing(a) Then GoTo npr
            LdInput()
            't = DateAndTime.Timer

            'napr:


            Select Case ComboBox1.SelectedIndex
                Case 0
                    a.Add(b)
                Case 1
                    a.Subtract(b)
                Case 2
                    'a.StripZeros()



                    a *= b

                   ' a.StripZeros()

                Case 3

                    If chkINT(b) Then
                        a.Divide(b(0), PrecOnDivByInt64)
                    Else
                        'a = BigDivide(a, b)

                        ' Dim test As New Hyper(a)
                        'DivideXLnwt(test, b, XLdivPrec, nIterationsAtDiv)
                        'DivideXLnwt(a, b, XLdivPrec, nIterationsAtDiv)
                        DivideXL(a, b, XLdivPrec, nIterationsAtDiv)

                        '                        a.StripZeros()
                        'isApprox = True
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

                    a = NthPower(a, b(0), OALprec)
                Case 6

                    a = NthRoot(a, b(0))
                    'a.StripZeros()
                    ' a.Round(OALprec)
            End Select
            ' t = DateAndTime.Timer - t

            'comment out the following line to display Float64 in TextBox3
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

            'If -a.GetBottExp > OALprec Then
            'a.Round(OALprec)
            'End If


            s2$ = ""

            a.StripZeros()
            prepareA()
            s2$ = olepšaj(a.ToString)

            If CheckBox1.Checked Then

                If TextBox2.TextLength Then lastTextInp = TextBox2.Text
                If ComboBox1.SelectedIndex = 6 Then

                    rtx.SelectionStart = rtx.TextLength
                    rtx.SelectedText = vbCrLf
                    rtx.SelectionCharOffset = 10
                    ' rtx.AppendText(b(0))
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


            'hi% = a.FindHighExponent
            'Hyper.displayMode = Hyper.displayModeType.inHex


            'txMonitor.Text = a.ToString
            'txMonitor.Text = Hex(a(hi)) & vbCrLf & hi
            Hyper.displayMode = Hyper.displayModeType.inTrueDecimal

            izpis()
            'Label3.Text = "Operation took (s): " & t
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
                'rtx.AppendText(vbCrLf & s4)
                'rtx.SelectionStart = rtx.TextLength
                'rtx.ScrollToCaret()

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


    Sub DivideXL(ByRef r1 As Hyper, ByRef d As Hyper, precision%, Optional nIterations% = 5500) ' As Hyper

        ' precision \ 2
        '        Hyper.QuotientPrecision = precision

        Dim r As New Hyper(0, 0)
        'Dim r1 As New Hyper(0, 0)
        hiXp% = d.FindHighExponent()
        z& = d(hiXp)
        Dim rest As New Hyper(d)
        rest.StripZeros()
        rest(hiXp) = 0
        rest.PartSize = rest.BufferSize - 1

        'rest.Multiply(2)
        'r1.Multiply(2)

        'If 1 = 0 Then

        If Not rest.IsNotZero Then
            r1.Divide(z, precision)
            r1.PartSize += hiXp
            Exit Sub
        End If
        '        r(0) = 0
        'ix% = -hiXp

        one% = 1

        For ii% = 0 To nIterations

            If z <> 1 Then
                r1.Divide(z, precision)
            End If

            'r.Add(r1) 'Else r.Add(r1)
            If (ii And one) Then r.Subtract(r1) Else r.Add(r1)

            r1 *= rest
            r1.StripZeros()
            If r1.GetBottExp - 20 < -precision Then r1.Round(precision)


        Next
        'r.StripZeros()
        r.PartSize += hiXp

        r1 = r.Clone
        'Return r

    End Sub

    Sub DivideXLnwt(ByRef r1 As Hyper, ByRef d As Hyper, prec%, Optional nIterations% = 5)


        r1 *= Reciprocal(d, XLdivPrec, nIterationsAtDiv)


    End Sub


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
        '     a = New Hyper("35")
        '  ReDim bbuff(1)
        '   b = New Hyper(bbuff, 0)
        '   Hyper.AddX(b,)
        '    b.DefineFromDecimal(44, "0")
        'b(0) += 4
        'Dim q As New HyperM

    End Sub

    Private Sub loadSettings()
        Hyper.ExtraDigitsForConversionFromDecimal = My.Settings.ConversionDecPrec
        Hyper.maxDigitsInString = My.Settings.DisplayDigitsDec
        Hyper.QuotientPrecision = My.Settings.QuotientPrec
        nIterationsAtSqr = My.Settings.nIterationsSQR

        If My.Settings.ApplyOverallPrec Then
            OALprec% = My.Settings.OverallPrecision
        Else
            OALprec = 2000000
        End If

        PrecOnDivByInt64% = My.Settings.QuotientPrec
        nIterationsAtDiv% = My.Settings.nIterations
        XLdivPrec% = My.Settings.DivPrecXL
        NthRootPrec = My.Settings.NthRootPrec


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

            Case Keys.Escape
                If unclear Then
                    TextBox2.Text = ""
                    TextBox2.Focus()

                Else
                    TextBox1.Clear()
                    a = Nothing
                    'b = Nothing
                    bbuff = Nothing
                    izpis()
                    TextBox2.Focus()
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


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Settings.ShowDialog() = vbOK Then
            My.Settings.Reload()
            loadSettings()
        End If
    End Sub


    Private Sub MSbtn_Click(sender As Object, e As EventArgs) Handles MSbtn.Click
        ms(txMems.SelectedIndex) = a.Clone
        If CheckBox1.Checked Then

            'If TextBox2.TextLength Then lastTextInp = TextBox2.Text
            '   If Strings.Left(rtx.Lines(UBound(rtx.Lines)), 4) <> " = M" Then
            '& txMems.SelectedIndex Then
            rtx.AppendText(vbCrLf & " = M" & txMems.SelectedIndex)
            'lastTextInp & " = " & vbCrLf & s2)

            rtx.SelectionStart = rtx.TextLength
            rtx.ScrollToCaret()

            '  Else
            ' rtx.Lines(UBound(rtx.Lines)) = " = M" & txMems.SelectedIndex

            '        End If
        End If

        TextBox2.Focus()
    End Sub

    Private Sub MRbtn_Click(sender As Object, e As EventArgs) Handles MRbtn.Click



        TextBox2.Text = txMems.Text

        TextBox2.Focus()
    End Sub

    Private Sub MCbtn_Click(sender As Object, e As EventArgs) Handles MCbtn.Click
        ms(txMems.SelectedIndex) = New Hyper(0, 0)
        TextBox2.Focus()
    End Sub



    'Class GfG



    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        LdInput()
        a = NthRoot(b, 2)

        finishUp()

    End Sub

    Private Function Reciprocal(ByRef d As Hyper, prec%, nIterations%) As Hyper
        'r(new) = r(2- h*r)

        Dim two As New Hyper(0, 0)
        two(0) = 2
        Dim r As New Hyper(0, 0)
        e = d.FindHighExponent
        r(0) = 1
        r.Divide(d(e), PrecOnDivByInt64)
        r.PartSize += e


        For i = 1 To nIterations
            r *= (two - d * r)
            If r.GetBottExp < -prec Then r.Round(prec)
            r.StripZeros()
        Next

        Return r
    End Function

    Private Function NthRoot(ByRef h As Hyper, n%) As Hyper
        Dim r As Hyper
        '  Dim r As New Hyper(0, 0)
        'Dim rcpN As New Hyper(Reciprocal(h))
        hiExp0% = h.FindHighExponent
        prec% = NthRootPrec '- (hiExp0 \ 2)
        If hiExp0 < 0 Then prec -= hiExp0
        'prec% = NthRootPrec - (h.GetBottExp \ 2)

        i% = 0
        Dim rcpN As New Hyper("1")

        'rcpN.Divide(n, PrecOnDivByInt64)
        rcpN.Divide(n, prec)
        nMinus1 = n - 1

        Dim tmp As Hyper
        Dim tmp1 As Hyper

        hiExp% = hiExp0
        'r(0) = 
        If hiExp0 >= 0 Then
            hiExp \= n

            'hiExp += 1
        Else 'if hiexp
            hiExp += 1
            hiExp \= n
            hiExp -= 1

        End If

        oddity% = hiExp0 Mod n


        r = New Hyper(hiExp, hiExp)



        '        If (hiExp0 Mod n) = 0 Then
        If oddity = 0 Then
            'r(hiExp) =
            rut# = h(hiExp0) ^ (1 / n)
            r(hiExp) = rut

        Else
            If oddity < 0 Then oddity += n


            '            Else
            '=            rut# = (CDbl(h(hiExp0)) * (2 ^ 64) * oddity) ^ (1 / n)
            tmp = New Hyper(oddity, oddity)
                tmp(oddity) = h(hiExp0)
                tmp(oddity - 1) = h(hiExp0 - 1)
                rut# = 0
                tmp.ExtractDouble(rut)

                rut ^= (1 / n)
                '            rut# = h(hiExp0) ^ (1 / n)

                '            tmp = New Hyper((1 / rut).ToString) ' * (2 ^ 63)
                'r(hiExp ) = tmp(-1)

                ' If (oddity And 1) Then rut = 1 / rut

                r(hiExp) = rut
                'r = New Hyper(rut.ToString)
                'r.PartSize -= (hiExp)
            End If

        '       End If


        '        r(hiExp) = rut


        For i = 1 To nIterationsAtSqr
            'tmp = New Hyper(r)
            tmp1 = New Hyper(r)

            tmp = NthPower(r, nMinus1, prec)
            'For i2% = 2 To nMinus1
            'tmp *= tmp1
            'Next

            r = h.Clone

            DivideXL(r, tmp, prec, nIterationsAtDiv)
            'DivideXL(r, tmp, XLdivPrec, nIterationsAtDiv)
            'DivideXLnwt(r, tmp, XLdivPrec, nIterationsAtDiv)
            tmp1.Multiply(nMinus1)
            tmp1.Add(r)

            r = tmp1 * rcpN
            'r = tmp
            ' r.Divide(n, PrecOnDivByInt64)
            If r.GetBottExp + 20 < -prec Then  r.Round(prec)
            r.StripZeros()


        Next

        Return r
        'r(new)= (1/n) * ((n-1)r + h / r^n-1

    End Function


    Private Function NthPower(ByRef h As Hyper, n%, prec%) As Hyper

        i2% = 1
        Dim tmp As New Hyper(h)
        'Dim tmp As New Hyper("1")

        Dim r As New Hyper(0, 0)
        r(0) = 1

        'binary

        'For i% = 0 To 30

        While i2 <= n
            If i2 And n Then
                r *= tmp
            End If

            i2 <<= 1
            tmp *= tmp
            tmp.StripZeros()
            If tmp.GetBottExp + 20 < prec Then tmp.Round(prec)

        End While

        Return r
        'Exit Sub

    End Function


    Function neki()



        '        While n >= 2 ^ i2
        While n >= i2 ' (1 >> i2)
            i2 <<= 1
            tmp *= tmp

            'i2 += 1
        End While

        i% = n - i2
        tmp = h

        While i
            i2 >>= 1

        End While




        If n >= 2 Then
            tmp *= h
            If n >= 4 Then
                tmp *= tmp
                i2 = 4

            End If
            For i% = i2 To n
                tmp *= h
            Next
        End If

    End Function


    Private Function SqrXLnwt(ByRef x As Hyper, Optional prec% = 20) As Hyper


        'x = New Hyper(-2, 0) : x(1) = 2

        prec = XLdivPrec
        Dim r As New Hyper(0, 0)
        Dim half As New Hyper(0, -1)
        half(0) = 1
        half(-1) = &H8000000000000000
        Dim tri As New Hyper(0, -prec)
        tri(0) = 3


        Dim tmp As Hyper
        'xt# = 0
        '    x.ExtractDouble(xt)
        's$ = half.ToString
        If Not x.IsNotZero Then
            Return r
        End If

        hiExp0% = x.FindHighExponent

        'hiExp% = -3 \ 2
        hiExp% = hiExp0

        '        If (hiExp0 And 1) Then
        'hiExp /= 2

        'odd% = hiExp0 And 1

        If hiExp0 >= 0 Then
            hiExp >>= 1
            'hiExp += 1
        Else 'if hiexp
            hiExp += 1
            hiExp \= 2
            hiExp -= 1

        End If

        If x(hiExp0) = 1 Then
            r(hiExp) = 1

            GoTo skipp

        End If
        rut# = Math.Sqrt(x(hiExp0))

        'hiExp = -hiExp - 1

        If (hiExp0 And 1) Then
            'r(hiExp) =
            tmp = New Hyper((1 / rut).ToString) ' * (2 ^ 63)
            r(hiExp) = tmp(-1)
            'r(hiExp) = rut
        Else
            r(hiExp) = rut
        End If

skipp:
        Dim one As New Hyper("1")

        DivideXLnwt(one, r, XLdivPrec, nIterationsAtDiv)

        r = one
        r.StripZeros() : r.Round(prec)




        For i = 1 To nIterationsAtSqr
            r = r * (tri - x * r * r) * half
            'tmp = r
            'r *= r
            'r.StripZeros() : r.Round(prec)
            'r *= x
            'r.StripZeros() : r.Round(prec)
            'r = tri - r
            'r *= tmp * half
            r.StripZeros() : r.Round(prec)
        Next

        r *= x
        'r.StripZeros()

        Return r

    End Function
    Private Function SqrXL(ByRef x As Hyper) As Hyper
        Dim r As New Hyper(0, 0)
        Dim half As New Hyper(0, -1)
        half(0) = 1
        half(-1) = &H8000000000000000
        Dim tmp As Hyper
        'xt# = 0
        '    x.ExtractDouble(xt)
        's$ = half.ToString
        If Not x.IsNotZero Then
            Return r
        End If

        hiExp0% = x.FindHighExponent

        'hiExp% = -3 \ 2
        hiExp% = hiExp0

        '        If (hiExp0 And 1) Then
        'hiExp /= 2

        'odd% = hiExp0 And 1

        If hiExp0 >= 0 Then
            hiExp >>= 1
            'hiExp += 1
        Else 'if hiexp
            hiExp += 1
            hiExp \= 2
            hiExp -= 1

        End If

        If x(hiExp0) = 1 Then
            r(hiExp) = 1

            GoTo skipp

        End If
        rut# = Math.Sqrt(x(hiExp0))

        If (hiExp0 And 1) Then
            'r(hiExp) =
            tmp = New Hyper((1 / rut).ToString) ' * (2 ^ 63)
            r(hiExp) = tmp(-1)
        Else
            r(hiExp) = rut
        End If
        '       End If
        GoTo skipp

        '319508394304018648
        '247970052451






        If hiExp0 >= 0 Then
            If hiExp0 > 1 Then
                hiExp += ((hiExp0 + 1) And 1)

            End If
            r = New Hyper(hiExp, hiExp)
            r(hiExp) = Math.Sqrt(x(hiExp0))
        Else

            '            If hiExp0 < -1 Then
            odd% = hiExp0 And 1

            r = New Hyper(hiExp, hiExp)
            'r(hiExp) = (x(hiExp0) ^ 2)
            '            r(hiExp) = Math.Sqrt(
            If hiExp0 < -1 Then

                If odd Then

                    hiExp -= 1

                    r(hiExp) = x(hiExp0)
                Else
                    'r(hiExp) = x(hiExp0)
                    r(hiExp) = Math.Sqrt(x(hiExp0))

                End If

            Else

                t& = x(hiExp0)
                tester& = &H400000000000000
                '            i% = 63
                i% = 0
                While (t And tester) = 0
                    tester >>= 1
                    i += 1
                End While

                i >>= 1

                r(hiExp) = t << i

            End If
            'ElseIf hiExp = 1 Then
            '   hiExp = 0
            'hiExp += 1
            '           Else 'If hiexp0 = -1 Then
            '          hiExp = -1
            '        'hiExp += 1
            '     End If


        End If
        '        End If

        '90 000
        '     300
        '2500
        '    50
        '0000001
        '     





        'If True Then
        '        If x > New Hyper("1") Then
        'r.Divide(2, PrecOnDivByInt64)
        '       r(0) = 1
        'r *= half
        '      Else
        '     r = x.Clone
        '    r.Multiply(2)

        '        End If
        'r(0) = 1
        'r = x.Clone

        '--r(0) = 1

        'rt# = 1

skipp:


        'Dim chk As Hyper
        ' Dim chk1 As Hyper

        For i = 0 To nIterationsAtSqr

            tmp = New Hyper(x)
            'MsgBox(s & vbCrLf & xt)

            'chk1 = New Hyper(tmp)
            '   Dim chk3 As New Hyper(chk1)
            DivideXL(tmp, r, XLdivPrec, nIterationsAtDiv)
            's$ = olepšaj(tmp.ToString)
            'r(0) = 0
            ' s1$ = olepšaj(r.ToString)

            'ttrt = xt / rt
            's$ = olepšaj(tmp.ToString)
            'MsgBox(s & vbCrLf & rt)

            'chk = New Hyper(tmp * r)

            'chk1.Subtract(chk)
            ' chk1.StripZeros()
            'If chk1 > New Hyper("0.00001") Then MsgBox(olepšaj(chk1.ToString))

            tmp.Add(r)
            tmp *= half
            ' tmp.Divide(2, PrecOnDivByInt64)
            r = tmp.Clone
            r.StripZeros()


            'r(New) = 0.5 * (r + (x / r))

            'tt rt = 0.5 * (rt + (xt / rt))
            's$ = olepšaj(r.ToString)
            ' MsgBox(s & vbCrLf & rt)


            'If r.GetBottExp < -OALprec Then r.Round(OALprec)
            If r.GetBottExp + 15 < -XLdivPrec Then r.Round(XLdivPrec)
        Next

        Return r

    End Function


    Dim nIterationsPi% = 61

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        TextBox1.Text = CalculatePi().ToString
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        ms(txMems.SelectedIndex) += a

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ms(txMems.SelectedIndex) -= a

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click


        Try
            'If IsNothing(a) Then GoTo npr
            txt$ = TextBox2.Text

            s$ = txt.Replace(" ", "")

            If UCase(Strings.Left(s, 1)) = "M" Then
                txt = Mid(s, 2)
                memNr% = txt
                b = ms(memNr)
                GoTo napr
            End If

            If InStr(s, ",") Then
                txt = s.Replace(",", ".")
            End If
            If s.Length Then
                b = New Hyper(s)
                b.StripZeros()
                'ElseIf Not IsNothing(b) Then

            Else
                b = a
                '                Exit Sub
            End If


            'Dim tolr As New Hyper(-precAtSQR, -precAtSQR)
            'tolr(-precAtSQR) = 1

napr:
            a = b * b
            a.StripZeros()
            prepareA()
            '        a = floorSqrt(b)
            s2$ = olepšaj(a.ToString)
            TextBox1.Text = s2
            TextBox2.Clear()

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

    Function CalculatePi() As Hyper

        Dim k As New Hyper("-2")
        Dim two As New Hyper("2")
        'Dim tmp As Hyper
        n% = nIterationsPi
        'n = 42
        'tmp = New Hyper(k)
        Dim l As New Hyper(0, 0)

        For i% = 0 To n - 1
            'If k.GetBottExp < -OALprec Then k.Round(OALprec)



            'l = New Hyper(two - k)
            'If i = 34 Then            i = i

            'l = two - k
            l = SqrXL(New Hyper(two - k))
            l.Multiply(2 ^ (i))
            ''l.PartSize -= 1
            'If 2 * l < two Then brejk% = 0
            '            End If
            l.StripZeros()
            If l.GetTopExp < 0 Then l(0) = 0
            'l(l.GetTopExp + 2) = 0
            TextBox1.Text = "(" & i & ") " & olepšaj(l.ToString)
            TextBox1.Refresh()


            k.Add(two)
            k = SqrXL(k) 'else

        Next

        l = SqrXL(New Hyper(two - k))
        ''l = SqrXL(l)
        l.Multiply(2 ^ (n))

        'l.Multiply(2 ^ (n - 1))
        'l.PartSize -= 1
        Return l

    End Function

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click

        n% = 3
        LdInput()

        a = b.Clone

        For i = 2 To n
            a *= b
        Next

        a.StripZeros()
        prepareA()
        '        a = floorSqrt(b)
        s2$ = olepšaj(a.ToString)
        TextBox1.Text = s2
        TextBox2.Clear()

    End Sub

    Dim stringOut$ = ""

    Sub outputA(final As Boolean)


        prepareA()
        s2$ = olepšaj(a.ToString)
        TextBox1.Text = s2
        TextBox2.Clear()

        If CheckBox1.Checked Then
            '            If final Then

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
        '        End If

    End Sub
    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        '      If TextBox2.SelectionStart Then
        Button1.PerformClick()
        'LdInput()
        ' a = b
        ComboBox1.SelectedIndex = 6

        '       outputA(False)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        'If TextBox2.SelectionStart Then
        ComboBox1.SelectedIndex = -1
        Button1.PerformClick()
        'LdInput()
        ' a = b
        ComboBox1.SelectedIndex = 5
        finishUp()

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        LdInput()
        a = NthRoot(b, 3)
    End Sub


    Sub finishUp()
        a.StripZeros()
        prepareA()
        '        a = floorSqrt(b)
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
                memNr% = txt
                b = ms(memNr).Clone
                '                GoTo napr
                Exit Sub
            End If

            'If InStr(s, ",") Then
            'txt = s.Replace(",", ".")

            'End If
            If s.Length Then
                b = New Hyper(s)
                'b.StripZeros()
                'Dim tmp As New Hyper(s)
                'ReDim bbuff(0)
                'b.PartSize = 0
                'b.Add(tmp)
                'b += tmp
                'ElseIf Not IsNothing(b) Then
            Else
                b = a.Clone

            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)


        End Try


    End Sub

End Class
