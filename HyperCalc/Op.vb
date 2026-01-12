Option Explicit Off
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports HyperLib


Module Op

    Friend LeadingZerosTolerance% = 20
    Friend PrecOnDivByInt64% = 900
    Friend nIterationsAtDiv% = 1444
    Friend XLdivPrec% = 44

    Sub New()
        '        Dim j As Divide = AddressOf DivideXL
        '       j = AddressOf QuickDivide
    End Sub

    Friend Sub AssignDivMethod(method As DivisionMethodDef)
        Select Case method
            Case DivisionMethodDef.Quick
                DivisionXL = AddressOf QuickDivide
                XLdivPrec% = My.Settings.DivPrecQuick
            Case DivisionMethodDef.Simple
                DivisionXL = AddressOf DivideXL
                XLdivPrec% = My.Settings.DivPrecXL

            Case DivisionMethodDef.NewtonRaphson
                DivisionXL = AddressOf DivideXLnwt
                XLdivPrec% = My.Settings.DivPrecNewton
            Case Else
                XLdivPrec% = My.Settings.DivPrecLog
                DivisionXL = AddressOf DivideXL_log



        End Select
    End Sub

    Delegate Function DivideDlg(ByRef r1 As Hyper, ByRef d As Hyper, precision%) As Hyper
    Friend DivisionXL As DivideDlg

    Friend Enum DivisionMethodDef
        Quick
        Simple
        NewtonRaphson
        Logarithmic
    End Enum

    Friend DivisionMethod As DivisionMethodDef = 1




    Friend Function DivideXL(ByRef h As Hyper, ByRef d As Hyper, precision%) As Hyper
        'Friend Function DivideXL(ByRef r1 As Hyper, ByRef d As Hyper, precision%, Optional nIterations% = 5500) As Hyper
        nIterations% = nIterationsAtDiv

        Dim r As New Hyper(0, 0)
        'Dim r1 As New Hyper(0, 0)
        hiXp% = d.FindHighExponent()
        z& = d(hiXp)
        Dim rest As New Hyper(d)
        rest.StripZeros()
        rest(hiXp) = 0
        rest.PartSize = rest.BufferSize - 1
        'If rest.IsNegative Xor (z < 0) Then
        'rest.Negate()
        ' neg = 0 = 0
        'End If


        Dim r1 As New Hyper(h)
        If Not rest.IsNotZero Then

            r = r1.Clone
            r.Divide(z, precision)
            r.PartSize += hiXp
            Return r

        End If

        one% = 1

        For ii% = 0 To nIterations

            If z <> 1 Then
                r1.Divide(z, precision)
            End If

            '  r.Add(r1) 'Else r.Add(r1)
            'r.Subtract(r1) 'Else r.Add(r1)
            'If (ii And one) = 0 Then r.Subtract(r1) Else r.Add(r1)
            If (ii And one) Then r.Subtract(r1) Else r.Add(r1)

            r1 *= rest
            If r1.GetTopExp - r1.FindHighExponent > LeadingZerosTolerance Then r1.StripZeros()
            If r1.GetBottExp + 20 < -precision Then r1.Round(precision)


        Next
        'r.StripZeros()
        r.PartSize += hiXp '+ neg
        'If neg Then r.Negate()


        'r1 = r.Clone
        Return r
    End Function


    Friend Function DivideXLnwt(ByRef r1 As Hyper, ByRef d As Hyper, prec%) As Hyper


        Return r1 * ReciprocalNwt(d, prec) ', nIterationsAtDiv)


    End Function


    Friend Function ReciprocalXL(ByRef d As Hyper, prec%) As Hyper
        Select Case My.Settings.DivMethodType
            Case DivisionMethodDef.Quick
                Return ReciprocalValQ(d, My.Settings.DivPrecQuick)

            Case DivisionMethodDef.Simple

                tmp = New Hyper(0, 0)
                tmp(0) = 1
                'DivideXL(tmp, r, prec) ', nIterationsAtDiv)
                Return DivisionXL(tmp, d, prec) ', nIterationsAtDiv)

            Case DivisionMethodDef.Logarithmic

                Return Reciprocal_log(d, prec)

            Case Else

                Return ReciprocalNwt(d, My.Settings.DivPrecNewton) ', My.Settings.nIterationsDivNewton)

        End Select

    End Function
    Friend Function ReciprocalNwt(ByRef d As Hyper, prec%) As Hyper
        'r(new) = r(2- h*r)
        nIterations% = My.Settings.nIterationsDivNewton
        Dim two As New Hyper(0, 0)
        two(0) = 2
        Dim r As New Hyper(0, 0)
        e = d.FindHighExponent

        'If True Then
        If False Then

            r(0) = 1
            r.Divide(d(e), prec)
            r.PartSize += e
            r.StripZeros()
            ' Debug.WriteLine(r.ToString)
        Else

            r = ReciprocalValQ(d, My.Settings.DivPrecQuick)
        End If

        For i = 1 To nIterations
            r *= (two - d * r)
            If r.GetBottExp + 20 < -prec Then r.Round(prec)
            If r.GetTopExp - r.FindHighExponent > LeadingZerosTolerance Then r.StripZeros()
        Next

        Return r
    End Function


    Private Function QuickDivide(ByRef dividend As Hyper, ByRef d As Hyper, precision%) As Hyper
        Return dividend * ReciprocalQ(d, precision)
    End Function


    Friend Function ReciprocalQ(d As Hyper, precision%) As Hyper


        Dim r As New Hyper(precision, 0) ' New Hyper(0, 0)
        Dim bp, r1 As Hyper

        lowVal& = 0 ' d(lowExp)

        hiExp% = d.FindHighExponent
        lowExp% = d.FindLowExponent '000000000000000000000000000000000000000000000 d.PartSize
        '        precision += hiExp - lowExp
        endpos% = precision ' + lowExp

        'If lowExp < n Then
        If precision < hiExp - lowExp Then
            lowExp = hiExp - precision
            d(lowExp) = d(lowExp) Or 1
            lowVal& = d(lowExp)
        Else
            '        d(lowestNZ)
            lowVal& = d(lowExp)
            If lowVal And 1 = 0 Then
                ' Debug.WriteLine("even nr")
                'if the least significant bit is 0, then we can help ourselves with dividing/multiplying the dividend, and then the result, by 2^n.

                'lowVal = 1 Or lowVal
                d(lowExp - 1) = 1
                lowVal = 1
                lowExp -= 1
            End If
        End If


        mq& = GetMagicNr(lowVal)

        pos% = lowExp '
        d.Round(-pos)
        de% = pos
        d.PartSize = 0
        'bp = New Hyper("1")
        bp = New Hyper(0, 0)
        bp(0) = 1
        pos1% = 0
mainloop:
        ' get the sequence which, when multiplied by divisor, nullifies itself

        r1 = New Hyper(pos1, pos1)
        r1(pos1) = bp(pos1) * mq
        r(pos1) = r1(pos1)

        bp -= d * r1
        bp.Negate()




        pos1 += 1
        pos += 1

        If pos > endpos Then GoTo nx
        'reciprocal values of large numbers tend to repeat at very large intervals, so we'll be satisfied with our precision                 

        GoTo mainloop


nx:




        r1 = r * d

        'l% = (hiExp - lowExp)
        'pos1 = r1.FindHighExponent - l - 2
        'pos1 = r.FindHighExponent

        'For i% = pos1 To r1.FindHighExponent
        'r1(i) = 0
        'Next


        hi% = r1.FindHighExponent
        ' r.Divide(r1(hi) * mq, 444) ' - r1.PartSize), 22)
        r.Divide(r1(hi), 0) ' precision2) ' - r1.PartSize), 22)
        r.PartSize = hi + r1.PartSize + r.PartSize + de '.PartSize
        d.PartSize = -de
        Return r
    End Function

    Friend Function ReciprocalValQ(d As Hyper, precision%) As Hyper


        ' precision% = XLdivPrec         'number of 64-bit digits to extract. Must be larger than exponent range
        'precision2% = 0 ' XLdivPrec2 'may be zero


        Dim r As New Hyper(precision, 0) ' New Hyper(0, 0)
        Dim bp, r1 As Hyper

        lowVal& = 0 ' d(lowExp)

        hiExp% = d.FindHighExponent
        lowExp% = d.FindLowExponent '000000000000000000000000000000000000000000000 d.PartSize
        endpos% = precision ' + lowExp

        If lowExp < -precision Then

            lowExp = -precision
            lowVal& = d(lowExp)
            d(lowExp) = d(lowExp) Or 1
        Else
            '        d(lowestNZ)
            lowVal& = d(lowExp)
            If lowVal And 1 = 0 Then
                ' Debug.WriteLine("even nr")
                'if the least significant bit is 0, then we can help ourselves with dividing/multiplying the dividend, and then the result, by 2^n.

                'lowVal = 1 Or lowVal
                d(lowExp - 1) = 1
                lowVal = 1
                lowExp -= 1
            End If
        End If


        mq& = GetMagicNr(lowVal)

            pos% = lowExp '
        d.Round(-pos)
        de% = pos
        d.PartSize = 0
        'bp = New Hyper("1")
        bp = New Hyper(0, 0)
        bp(0) = 1
        pos1% = 0
mainloop:
        ' get the sequence which, when multiplied by divisor, nullifies itself

        r1 = New Hyper(pos1, pos1)
        r1(pos1) = bp(pos1) * mq
        r(pos1) = r1(pos1)

        bp -= d * r1
        bp.Negate()




        pos1 += 1
        pos += 1

        If pos > endpos Then GoTo nx
        'reciprocal values of large numbers tend to repeat at very large intervals, so we'll be satisfied with our precision                 

        GoTo mainloop


nx:




        r1 = r * d

        'l% = (hiExp - lowExp)
        'pos1 = r1.FindHighExponent - l - 2
        'pos1 = r.FindHighExponent

        'For i% = pos1 To r1.FindHighExponent
        'r1(i) = 0
        'Next


        hi% = r1.FindHighExponent
        ' r.Divide(r1(hi) * mq, 444) ' - r1.PartSize), 22)
        r.Divide(r1(hi), 0) ' precision2) ' - r1.PartSize), 22)
        r.PartSize = hi + r1.PartSize + r.PartSize + de '.PartSize
        d.PartSize = -de
        Return r
    End Function

    Private Function GetMagicNr&(a&)


        ' Magic number or "reciprocal integer" - GET THE 64-BIT NUMBER WHICH, when multiplied by the lowest digit, gives 1 as the remainder of 2^64               
        ' only for odd numbers

        bt& = 1 'bit tester
        d& = a 'bit mask

        r& = 0 : i% = 0 : r0& = 0



        For i = 0 To 63

            If bt And r Then GoTo skip

            r += d
            r0 = r0 Or bt
skip:
            bt <<= 1 : d <<= 1
        Next

        Return r0


    End Function



    Friend Function DivideXL_log(ByRef h As Hyper, ByRef d As Hyper, precision%) As Hyper
        Return h * Reciprocal_log(d, precision)
    End Function
    Friend Function Reciprocal_log(ByRef h As Hyper, precision%) As Hyper

        x# = 0

        Dim r, r2, one, n As Hyper
        lim% = Math.Log(precision * 0.0625) * 1.5

        n = h
        e% = n.FindHighExponent
        n.PartSize -= e + 1
        n.ExtractDouble(x)
        'ex% = (64 * e * Math.Log(2)) / Math.Log(10)

        'ex = -ex
        x = 1 / x
        r = New Hyper(x.ToString)
        r.PartSize -= e + 1
        one = New Hyper("1")
        'r2 = r
        prec% = 8
        '        n = h

        For i% = 1 To lim

            r2 = one - h * r
            prec = prec * 2 - 1
            r2.PartSize += prec
            'r2.PartSize += prec * 2 - 1
            'r2 = one - r2
            r2 *= r
            'If r2.GetBottExp + 20 < -prec Then r2.Round(prec)
            If r2.GetBottExp + 20 < -XLdivPrec Then r2.Round(XLdivPrec)
            If r2.GetTopExp - r2.FindHighExponent > LeadingZerosTolerance Then r2.StripZeros()
            r.Add(r2)

        Next

        Return r
    End Function


End Module
