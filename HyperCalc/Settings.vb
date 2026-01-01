Imports HyperLib

Public Class Settings
    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        load()
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
        My.Settings.DivPrecXL = NumericUpDown4.Value
        My.Settings.nIterations = NumericUpDown3.Value
        My.Settings.ConversionDecPrec = NumericUpDown5.Value
        My.Settings.DisplayDigitsDec = NumericUpDown6.Value
        My.Settings.nIterationsSQR = txIterationsSQR.Value
        My.Settings.NthRootPrec = txRut.Value

        My.Settings.Save()

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            txOVLprec.Enabled = True
        Else
            txOVLprec.Enabled = False

        End If
    End Sub

    Private Sub load()
        CheckBox1.Checked = My.Settings.ApplyOverallPrec
        txOVLprec.Value = My.Settings.OverallPrecision
        NumericUpDown2.Value = My.Settings.QuotientPrec
        NumericUpDown4.Value = My.Settings.DivPrecXL
        NumericUpDown3.Value = My.Settings.nIterations
        NumericUpDown5.Value = My.Settings.ConversionDecPrec
        NumericUpDown6.Value = My.Settings.DisplayDigitsDec
        txIterationsSQR.Value = My.Settings.nIterationsSQR
        txRut.Value = My.Settings.NthRootPrec
        '        NumericUpDown6.Value = My.Settings.DisplayDigitsDec

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If MsgBox("Reset settings to defaults?", vbYesNo) = MsgBoxResult.Yes Then

            My.Settings.Reset()
            load()
        End If
    End Sub
End Class