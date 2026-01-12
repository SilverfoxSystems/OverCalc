Option Explicit Off
Imports HyperLib
Imports HyperCalc.Op

Imports System.Security.Cryptography

Module OpEx


    Friend nIterationsAtSqr% = 144
    Friend NthRootPrec% = 80
    'Friend currPrecision%
    Delegate Function SQRdlg(ByRef a As Hyper) As Hyper  ', prec%)

    Friend SqrXL As SQRdlg

    Friend Sub AssignSQRmethod(method As DivisionMethodDef)
        Select Case method
            Case DivisionMethodDef.Quick
                SqrXL = AddressOf sqrxlnth
            Case DivisionMethodDef.Simple
                SqrXL = AddressOf SqrXLsimple

            Case DivisionMethodDef.NewtonRaphson
                SqrXL = AddressOf SqrXLnwt

        End Select
    End Sub



    Friend Function SqrXLnwt(ByRef x As Hyper) As Hyper


        'x = New Hyper(-2, 0) : x(1) = 2

        prec = NthRootPrec ' My.Settings.RootPrec ' XLdivPrec
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
        'Dim one As New Hyper("1")
        'r = DivisionXL(one, r, XLdivPrec) ', nIterationsAtDiv)
        r = ReciprocalXL(r, prec)
        'Division(one, r, XLdivPrec) ', nIterationsAtDiv)

        r.StripZeros() : r.Round(prec)




        For i = 1 To nIterationsAtSqr
            'r = r * (tri - x * r * r) * half

            tmp = x * r * r
            tmp.StripZeros()
            If tmp.GetBottExp + 20 < -prec Then tmp.Round(prec)
            tmp.Subtract(tri)
            tmp.Negate()
            tmp *= half
            r *= tmp
            If r.GetTopExp - r.FindHighExponent > LeadingZerosTolerance Then r.StripZeros()
            If r.GetBottExp + 20 < -prec Then r.Round(prec)
        Next

        r *= x
        'r.StripZeros()

        Return r

    End Function
    Friend Function SqrXLsimple(ByRef x As Hyper) As Hyper
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

            '-tmp = New Hyper(x)
            'MsgBox(s & vbCrLf & xt)

            'chk1 = New Hyper(tmp)
            '   Dim chk3 As New Hyper(chk1)
            '--Division(tmp, r, XLdivPrec) ', nIterationsAtDiv)
            tmp = DivisionXL(x, r, XLdivPrec) ', nIterationsAtDiv)

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
            If r.GetTopExp - r.FindHighExponent > LeadingZerosTolerance Then r.StripZeros()


            'r(New) = 0.5 * (r + (x / r))

            'tt rt = 0.5 * (rt + (xt / rt))
            's$ = olepšaj(r.ToString)
            ' MsgBox(s & vbCrLf & rt)


            'If r.GetBottExp < -OALprec Then r.Round(OALprec)
            If r.GetBottExp + 15 < -NthRootPrec Then r.Round(NthRootPrec)
        Next

        Return r


    End Function



    Friend Function NthRoot(ByRef h As Hyper, n%) As Hyper
        Dim r As Hyper
        '  Dim r As New Hyper(0, 0)
        'Dim rcpN As New Hyper(Reciprocal(h))
        hiExp0% = h.FindHighExponent
        prec% = NthRootPrec '- (hiExp0 \ 2)
        If hiExp0 < 0 Then prec -= hiExp0
        'prec% = NthRootPrec - (h.GetBottExp \ 2)

        i% = 0
        Dim rcpN As New Hyper("1")

        rcpN.Divide(n, prec)
        nMinus1 = n - 1

        Dim tmp As Hyper
        Dim tmp1 As Hyper

        hiExp% = hiExp0
        If hiExp0 >= 0 Then
            hiExp \= n

        Else
            hiExp += 1
            hiExp \= n
            hiExp -= 1

        End If

        oddity% = hiExp0 Mod n


        r = New Hyper(hiExp, hiExp)



        If oddity = 0 Then
            rut# = h(hiExp0) ^ (1 / n)
            r(hiExp) = rut

        Else
            If oddity < 0 Then oddity += n


            '   rut# = (CDbl(h(hiExp0)) * (2 ^ 64) * oddity) ^ (1 / n)
            tmp = New Hyper(oddity, oddity - 1)
            tmp(oddity) = h(hiExp0)
            tmp(oddity - 1) = h(hiExp0 - 1)
            rut# = 0
            tmp.ExtractDouble(rut)

            rut ^= (1 / n)

            r(hiExp) = rut
        End If



        For i = 1 To nIterationsAtSqr
            tmp1 = New Hyper(r)

            tmp = NthPower(r, nMinus1, prec)
            tmp1.Multiply(nMinus1)
            tmp1.Add(DivisionXL(h, tmp, prec))

            r = tmp1 * rcpN
            If r.GetBottExp + 20 < -prec Then r.Round(prec)
            If r.GetTopExp - r.FindHighExponent > LeadingZerosTolerance Then  r.StripZeros()

        Next

        Return r
        'r(new)= (1/n) * ((n-1)r + h / r^n-1

    End Function


    Friend Function NthPower(ByRef h As Hyper, n%, prec%) As Hyper

        i2% = 1
        Dim tmp As New Hyper(h)
        Dim nIsNegative As Boolean
        'Dim tmp As New Hyper("1")

        Dim r As New Hyper(0, 0)
        r(0) = 1

        'binary
        If n < 0 Then nIsNegative = True : n = -n

        'For i% = 0 To 30

        While i2 <= n
            If i2 And n Then
                r *= tmp
            End If

            i2 <<= 1
            tmp *= tmp
            If tmp.GetTopExp - tmp.FindHighExponent > LeadingZerosTolerance Then     tmp.StripZeros()
            If tmp.GetBottExp + 20 < -prec Then tmp.Round(prec)

        End While

        If nIsNegative Then

            Return ReciprocalXL(r, prec)

            'Return Reciprocal(r,prec ,)
        Else
            Return r
        End If
        'Exit Sub

    End Function

    Function RthPower(ByRef h As Hyper, ByRef exp As Hyper, prec%) As Hyper


        i2% = 1
        Dim tmp As New Hyper(h)
        Dim nIsNegative As Boolean
        'Dim tmp As New Hyper("1")

        Dim r As New Hyper(0, 0)
        r(0) = 1

        'binary
        If exp.IsNegative Then nIsNegative = True : exp.Negate()


        'For i% = 0 To 30
        n& = exp(0)
        While i2 <= n
            If i2 And n Then
                r *= tmp
            End If

            i2 <<= 1
            tmp *= tmp
            tmp.StripZeros()
            If tmp.GetBottExp + 20 < -prec Then tmp.Round(prec)

        End While

        n& = exp(-1)
        i2 = &H8000000000000000

        tmp = h.Clone
        tmp = New Hyper(0, 0)
        tmp(0) = 1
        'DivideXL(tmp, r, prec) ', nIterationsAtDiv)
        tmp = DivisionXL(tmp, r, prec) ', nIterationsAtDiv)
        If i2 And n Then
            r *= tmp
        End If
        'i2 >>= 1
        i2 = &H4000000000000000



        '        While i2 <= 64
        For i = 0 To 63

            If i2 And n Then
                r *= tmp
            End If

            i2 >>= 1
            tmp *= tmp
            tmp.StripZeros()
            If tmp.GetBottExp + 20 < -prec Then tmp.Round(prec)

        Next
        '       End While



        If nIsNegative Then

            tmp = New Hyper(0, 0)
            tmp(0) = 1
            Return DivisionXL(tmp, r, prec) '), nIterationsAtDiv)
        Else
            Return r
        End If
    End Function

    Friend Function sqrXLnth(ByRef h As Hyper) As Hyper  ', prec%)

        Return NthRoot(h, 2)
    End Function


End Module
