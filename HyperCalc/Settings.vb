Imports HyperLib

Public Class Settings

    Private NiterationsNewt%, NiterationsSimple%, divPrecQ%, divPrecSimple%, divPrecNewton%, divPrecLog%
    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadS()
        ToolTip1.ShowAlways = True
        ToolTip1.SetToolTip(Me.LblOVLprec, hintOVLprec)
        ToolTip1.SetToolTip(Me.lblBaseDivPrec64bit, hintQuotPrec)
        ToolTip1.SetToolTip(Me.Label4, hintDIV1prec)
        ToolTip1.SetToolTip(Me.Label2, hintDIV2prec)


    End Sub

    Const hintOVLprec$ = ""
    Const hintQuotPrec = "Precision used at dividing by a 64-bit integer"
    Const hintDIV2prec$ = "Precision used at dividing by large integers and by non-integers"
    Const hintDIV1prec$ = "Number of iterations performed for getting closer to the result."
    Const hintConvDecPrec$ = "Precision used at converting from decimal input"



    Private Sub OkBtn_Click(sender As Object, e As EventArgs) Handles OkBtn.Click
        save()

    End Sub

    Private Sub save()


        My.Settings.OverallPrecision = txOVLprec.Value
        My.Settings.QuotientPrec = NumericUpDown2.Value
        My.Settings.ConversionDecPrec = NumericUpDown5.Value
        My.Settings.DisplayDigitsDec = NumericUpDown6.Value
        My.Settings.nIterationsRoots = txIterationsSQR.Value
        My.Settings.RootPrec = txRootPrec.Value

        My.Settings.nIterations = NiterationsSimple
        My.Settings.nIterationsDivNewton = NiterationsNewt
        My.Settings.DivPrecQuick = divPrecQ
        My.Settings.DivPrecNewton = divPrecNewton
        My.Settings.DivPrecXL = divPrecSimple
        My.Settings.DivMethodType = ComboBox1.SelectedIndex
        My.Settings.SqrMethodType = txRootMethod.SelectedIndex
        My.Settings.DivPrecLog = divPrecLog

        My.Settings.Save()

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            txOVLprec.Enabled = True
        Else
            txOVLprec.Enabled = False

        End If
    End Sub

    Private Sub loadS()
        CheckBox1.Checked = My.Settings.ApplyOverallPrec
        txOVLprec.Value = My.Settings.OverallPrecision
        NumericUpDown2.Value = My.Settings.QuotientPrec
        NumericUpDown5.Value = My.Settings.ConversionDecPrec
        NumericUpDown6.Value = My.Settings.DisplayDigitsDec
        txIterationsSQR.Value = My.Settings.nIterationsRoots
        txRootPrec.Value = My.Settings.RootPrec
        ComboBox1.SelectedIndex = My.Settings.DivMethodType
        NiterationsSimple = My.Settings.nIterations
        NiterationsNewt = My.Settings.nIterationsDivNewton
        divPrecQ = My.Settings.DivPrecQuick
        divPrecNewton = My.Settings.DivPrecNewton
        divPrecSimple = My.Settings.DivPrecXL
        divPrecLog = My.Settings.DivPrecLog
        If ComboBox1.SelectedIndex = 1 Then
            txNiterations.Value = My.Settings.nIterations
            txDivXLprec.Value = divPrecSimple
        ElseIf ComboBox1.SelectedIndex = 2 Then
            txDivXLprec.Value = divPrecNewton
            txNiterations.Value = My.Settings.nIterationsDivNewton
        ElseIf ComboBox1.SelectedIndex = 3 Then
            txDivXLprec.Value = divPrecLog
            txNiterations.Value = 0
            txNiterations.Enabled = False

        Else
            txNiterations.Enabled = False
            txNiterations.Value = 0
            txDivXLprec.Value = divPrecQ

        End If
        txRootMethod.SelectedIndex = My.Settings.SqrMethodType
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If MsgBox("Reset settings to defaults?", vbYesNo) = MsgBoxResult.Yes Then

            My.Settings.Reset()
            loadS()
        End If
    End Sub



    Private Sub txNiterations_ValueChanged(sender As Object, e As EventArgs) Handles txNiterations.ValueChanged
        If ComboBox1.SelectedIndex = 1 Then
            NiterationsSimple = txNiterations.Value
        ElseIf ComboBox1.SelectedIndex = 2 Then
            NiterationsNewt = txNiterations.Value
        End If

    End Sub

    Private Sub NumericUpDown4_ValueChanged(sender As Object, e As EventArgs) Handles txDivXLprec.ValueChanged
        If ComboBox1.SelectedIndex = 1 Then
            divPrecSimple = txDivXLprec.Value

        ElseIf ComboBox1.SelectedIndex = 2 Then
            divPrecNewton = txDivXLprec.Value
        ElseIf ComboBox1.SelectedIndex = 0 Then
            divPrecQ = txDivXLprec.Value
        Else
            divPrecLog = txDivXLprec.Value

        End If

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = 0 Then
            txDivXLprec.Value = divPrecQ
            txNiterations.Value = 0
            txNiterations.Enabled = False
        Else
            txNiterations.Enabled = True
            If ComboBox1.SelectedIndex = 1 Then
                txNiterations.Value = My.Settings.nIterations

                txDivXLprec.Value = divPrecSimple
            ElseIf ComboBox1.SelectedIndex = 2 Then
                txDivXLprec.Value = divPrecNewton
                txNiterations.Value = My.Settings.nIterationsDivNewton
            Else
                txNiterations.Enabled = False
                txDivXLprec.Value = divPrecLog

            End If
        End If
    End Sub

    Private Sub Settings_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                CancelBtn.PerformClick()
            Case Keys.Enter
                If CancelBtn.Focused <> True AndAlso Button1.Focused <> True Then
                    OkBtn.PerformClick()
                End If
        End Select
    End Sub


End Class