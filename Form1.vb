﻿Option Strict Off
Option Explicit On
Public Class Form1
    Inherits System.Windows.Forms.Form
    Public WithEvents hSherlock As IpeEngCtrlLib.Engine
    Public nErr As IpeEngCtrlLib.I_ENG_ERROR
    Public proname() As String
    Public ROIName() As String
    Public roiState, annoState, setVarState, modifyState As Boolean
    Public temp As Double
    Public variables()
    Public textBoxes() As MaskedTextBox
    Public sNowSolutionPath As String
    Public SizeHeightEnalbe, SizeWidthEnalbe, ScreenRotateEnalbe, ScreenShiftEnalbe, Rotate90Enable, EdgeInspectEnalbe, CornerInspectEnalbe As Double
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        StartScreen.Show()
        roiState = False
        annoState = False
        setVarState = False
        modifyState = False
        'create engine object
        hSherlock = CreateObject("IpeEngCtrl.EngineNg")
        'initialize object
        nErr = hSherlock.EngInitialize
        'connect display to engine
        AxIpeDspCtrl1.ConnectEngine(hSherlock.GetEngineObj)
        sNowSolutionPath = GetIniStr("Config", "SolutionPath", My.Application.Info.DirectoryPath & "\config.ini")
        If sNowSolutionPath = "" Or Dir(sNowSolutionPath) = "" Then
            MsgBox("Error : 方案路径错误,重新加载", vbOKOnly, "提示")
        Else
            Call Load_Pro()
        End If
        StartScreen.Close()
        Timer3.Interval = 1000
        Timer3.Start()
    End Sub
    'Dim sNowSolutionPath As String
    Private Sub BtnSolutoon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSolutoon.Click
        OpenFileDialog1.Filter = "Sherlock Files" & "(*.ivx;*.ivs)|*.ivx;*.ivs"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.ShowDialog()
        If OpenFileDialog1.FileName <> "" Then
            sNowSolutionPath = Me.OpenFileDialog1.FileName
            Call WriteIniStr("Config", "SolutionPath", CStr(sNowSolutionPath), My.Application.Info.DirectoryPath & "\config.ini")
            Call Load_Pro()
        End If
    End Sub
    Dim Flag_Prog As Boolean
    Private Sub Load_Pro()
        BtnShowRoi.Text = "显示框"
        BtnAnnotations.Text = "显示点"
        GroupBoxSetVariable.Hide()
        nErr = hSherlock.InvLoad(sNowSolutionPath)
        AxIpeDspCtrl1.ConnectImgWindow("检测窗口")
        AxIpeDspCtrl1.SetZoom(-1)
        variables = {"DETECTED_CELL_SIZE_LEVEL_MM", "DETECTED_CELL_HEIGHT_LEVEL_MM", "DETECTED_CELL_WIDTH_LEVEL_MM",
            "SCREEN_ROTATE_ERROR_LEVEL", "SCREEN_SHIFT_ERROR_LEVEL_MM", "EDGE_ERROR_LEVEL_PIX", "CORNER_ERROR_LEVEL_PIX", "EDGE_BREAKAGE_LEVEL_PIX",
            "INSPECT_ROI_DIAGONLA_WIDTH_PIX", "CORNER_BREAKAGE_LEVEL_PIX", "INSPECT_INNER_ERROR_THRESHOLD"}
        textBoxes = {Text0, Text1, Text2, Text3, Text4, Text5, Text6, Text7, Text8, Text9, Text10}
        hSherlock.VarGetDouble("DETECTED_CELL_HEIGHT_SKIP", SizeHeightEnalbe)
        hSherlock.VarGetDouble("DETECTED_CELL_WIDTH_SKIP", SizeWidthEnalbe)
        hSherlock.VarGetDouble("SCREEN_ROTATE_ERROR_SKIP", ScreenRotateEnalbe)
        hSherlock.VarGetDouble("SCREEN_SHIFT_ERROR_SKIP", ScreenShiftEnalbe)
        hSherlock.VarGetDouble("ROTATE90_ERROR_SKIP", Rotate90Enable)
        hSherlock.VarGetDouble("EDGE_ERROR_SKIP", EdgeInspectEnalbe)
        hSherlock.VarGetDouble("CORNER_ERROR_SKIP", CornerInspectEnalbe)
        If SizeHeightEnalbe = 0 Then
            Button1.BackColor = Color.Lime
        Else
            Button1.BackColor = Color.Red
        End If

        If SizeWidthEnalbe = 0 Then
            Button2.BackColor = Color.Lime
        Else
            Button2.BackColor = Color.Red
        End If

        If ScreenRotateEnalbe = 0 Then
            Button3.BackColor = Color.Lime
        Else
            Button3.BackColor = Color.Red
        End If

        If ScreenShiftEnalbe = 0 Then
            Button4.BackColor = Color.Lime
        Else
            Button4.BackColor = Color.Red
        End If

        If Rotate90Enable = 0 Then
            Button7.BackColor = Color.Lime
        Else
            Button7.BackColor = Color.Red
        End If

        If EdgeInspectEnalbe = 0 Then
            Button5.BackColor = Color.Lime
        Else
            Button5.BackColor = Color.Red
        End If

        If CornerInspectEnalbe = 0 Then
            Button6.BackColor = Color.Lime
        Else
            Button6.BackColor = Color.Red
        End If

        If nErr <> IpeEngCtrlLib.I_ENG_ERROR.I_OK Then
            nErr = hSherlock.EngTerminate
            hSherlock = Nothing
            Flag_Prog = False
        Else
            Flag_Prog = True
        End If
        If Flag_Prog = True Then
        End If
    End Sub
    Private Sub BtnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStart.Click
        nErr = hSherlock.InvModeSet(IpeEngCtrlLib.I_MODE.I_EXE_MODE_CONT)
        AxIpeDspCtrl1.ConnectImgWindow("检测窗口")
        BtnStop.Enabled = True
        BtnStart.Enabled = False
        BtnEscInter.Enabled = False
        Btnparameter.Enabled = False
        GroupBox1.Enabled = False
        ChkALive.CheckState = CheckState.Unchecked
        ChkALive.Enabled = False
        BtnSetVariable.Enabled = False
    End Sub
    Private Sub BtnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStop.Click
        nErr = hSherlock.InvModeSet(IpeEngCtrlLib.I_MODE.I_EXE_MODE_HALT_AFTER_ITERATION)
        'allow engine to halt and process any messages from the engine while it stops
        Dim m As IpeEngCtrlLib.I_MODE
        Do
            System.Windows.Forms.Application.DoEvents()
            hSherlock.InvModeGet(m)
        Loop Until m = IpeEngCtrlLib.I_MODE.I_EXE_MODE_HALT
        BtnStop.Enabled = False
        BtnStart.Enabled = True
        BtnEscInter.Enabled = True
        Btnparameter.Enabled = True
        ChkALive.Enabled = True
        BtnSetVariable.Enabled = True
    End Sub

    Private Sub BtnEscInter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEscInter.Click
        nErr = hSherlock.InvModeSet(IpeEngCtrlLib.I_MODE.I_EXE_MODE_HALT_AFTER_ITERATION)
        Dim m As IpeEngCtrlLib.I_MODE
        Do
            System.Windows.Forms.Application.DoEvents()
            hSherlock.InvModeGet(m)
        Loop Until m = IpeEngCtrlLib.I_MODE.I_EXE_MODE_HALT
        AxIpeDspCtrl1.DisconnectImgWindow()
        AxIpeDspCtrl1.DisconnectEngine()
        nErr = hSherlock.EngTerminate
        hSherlock = Nothing
        Me.Close()
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        hSherlock.RoiMove(CmbSelRoi.Text, MouseCurX - MouseIniX, MouseCurY - MouseIniY)
        MouseIniX = MouseCurX
        MouseIniY = MouseCurY
        AxIpeDspCtrl1.UpdateDisplay()
        AxIpeDspCtrl1.UseWaitCursor = False
        Timer1.Enabled = False
        Me.UseWaitCursor = False
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        hSherlock.RoiCoordArraySet(CmbSelRoi.Text, ROIPtsXArray, ROIPtsYArray)
        AxIpeDspCtrl1.UpdateDisplay()
        AxIpeDspCtrl1.UseWaitCursor = False
        Me.Cursor = Cursors.Default
        Me.UseWaitCursor = False
        Timer2.Enabled = False
    End Sub
    Private Sub BtnSave1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSave1.Click
        nErr = hSherlock.InvSave(sNowSolutionPath)
    End Sub
    Private Sub Btnparameter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btnparameter.Click
        GroupBox1.Enabled = True
        nErr = hSherlock.VarGetDouble("ExposureTime", vbExposureTime)
        CamShutter.Text = CStr(vbExposureTime)
        CmbImg.Items.Clear()
        CmbImg.Items.Insert(0, "检测工位")
        CmbImg.Items.Insert(1, "校准工位")
        CmbImg.SelectedIndex = 0
    End Sub
    Dim ROINames(), ImgFilter As String
    Private Sub CmbImg_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbImg.SelectedIndexChanged
        Select Case CmbImg.SelectedIndex
            Case 0
                ImgFilter = "检测窗口"
                AxIpeDspCtrl1.ConnectImgWindow("检测窗口")
            Case 1
                ImgFilter = "imgB"
                AxIpeDspCtrl1.ConnectImgWindow("imgB")

        End Select
        nErr = hSherlock.RoiGetNames(ImgFilter, ROINames)
        CmbSelRoi.Items.Clear()
        For J = 0 To UBound(ROINames)
            CmbSelRoi.Items.Insert(J, ROINames(J))
        Next (J)
        If CmbSelRoi.Items.Count > 0 Then
            CmbSelRoi.SelectedIndex = 0
        End If
    End Sub
    Dim ROISelected As String
    Private Sub cmbSelRoi_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSelRoi.SelectedIndexChanged
        Dim I As Integer, j As Integer
        Dim ROIProc(I) As String
        'roiState = False
        'annoState = False
        'modifyState = False
        'Dim n As Integer
        hSherlock.RoiShowOutlineGet(CmbSelRoi.Text, roiState)
        'roiState = Convert.ToBoolean(n)
        hSherlock.RoiFlagsGet(CmbSelRoi.Text, modifyState)
        'modifyState = Convert.ToBoolean(n)
        hSherlock.RoiShowAnnotGet(CmbSelRoi.Text, annoState)
        'annoState = Convert.ToBoolean(n)
        If roiState = True Then
            BtnShowRoi.Text = "隐藏框"
        Else
            BtnShowRoi.Text = "显示框"
        End If

        If modifyState = True Then
            BtnModify.Text = "隐藏处理"
        Else
            BtnModify.Text = "处理图形"
        End If

        If annoState = True Then
            BtnAnnotations.Text = "隐藏点"
        Else
            BtnAnnotations.Text = "显示点"
        End If

        'BtnShowRoi.Text = "显示框"
        'BtnAnnotations.Text = "显示点"
        '读取ROI内的处理程序
        hSherlock.RoiGetProc(CmbSelRoi.Text, ROIProc)
        ROIProcess.Columns.Clear()
        ROIProcess.Items.Clear()
        ROIProcess.Columns.Add("编号", 50)
        ROIProcess.Columns.Add("ROI工具", 140)
        ' lvProcess.Items.Clear()
        j = 0
        For I = 0 To UBound(ROIProc)
            ROIProcess.Items.Add(CStr(I))
            If ROIProcess.Items(Me.ROIProcess.Items.Count - 1).SubItems.Count > 1 Then
                ROIProcess.Items(Me.ROIProcess.Items.Count - 1).SubItems(0).Text = CStr(j)
            Else
                ROIProcess.Items(Me.ROIProcess.Items.Count - 1).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, ROIProc(I)))
            End If
        Next I
    End Sub
    Private Sub ROIProcess_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ROIProcess.SelectedIndexChanged
        Dim Item2 As String
        Dim tempStr As String
        If ROIProcess.SelectedItems IsNot Nothing AndAlso ROIProcess.SelectedItems.Count > 0 Then '判断是否已选定了某行，如果未选定则不执行代码
            Item2 = ROIProcess.SelectedItems(0).SubItems(1).Text
            If Item2 <> "" Then
                tempStr = CmbSelRoi.Text + "." + Item2
                nErr = hSherlock.InstrCommand(tempStr, IpeEngCtrlLib.I_INSTR_COMMANDS.INSTR_SHOW_PARMS_DLG, 0)
            End If
        End If
    End Sub
    Private Sub AxIpeDspCtrl1_LButtonDown(ByVal sender As Object, ByVal e As AxIpeDspCtrlLib._DIpeDspCtrlEvents_LButtonDownEvent) Handles AxIpeDspCtrl1.LButtonDown
        nErr = hSherlock.RoiRotationGet(CmbSelRoi.Text, ROIAngle)
        nErr = hSherlock.RoiRotationGet(CmbSelRoi.Text, ROICurAngle)
        hSherlock.RoiCoordArrayGet(CmbSelRoi.Text, ROIPtsXArray, ROIPtsYArray)
        hSherlock.RoiTypeGet(CmbSelRoi.Text, ROItype)
        hSherlock.RoiGetNames("imgA", ROIName)
        If (ROItype = "RoiRect" Or ROItype = "RoiRake") Then
            Call Rect_LButtonClick(e.x, e.y)
            If (ROILTFlag = True Or ROIRDFlag = True Or ROIRTFlag = True Or ROILDFlag = True Or ROIMoveFlag = True Or ROIRotateFlag = True) Then
                If (UBound(ROIName) > 0) Then
                    For i = 0 To UBound(ROIName) / 2 Step 1
                        hSherlock.RoiSelectedSet(CmbSelRoi.Items(i), 0)
                        AxIpeDspCtrl1.UpdateDisplay()
                    Next i
                End If
                hSherlock.RoiSelectedSet(CmbSelRoi.Text, 1)
            Else
                hSherlock.RoiSelectedSet(CmbSelRoi.Text, 0)
            End If
        End If

        AxIpeDspCtrl1.UpdateDisplay()
    End Sub

    Private Sub AxIpeDspCtrl1_LButtonUp(ByVal sender As Object, ByVal e As AxIpeDspCtrlLib._DIpeDspCtrlEvents_LButtonUpEvent) Handles AxIpeDspCtrl1.LButtonUp
        ROIMoveFlag = False
        ROILTFlag = False
        Timer1.Enabled = False
        ROIRDFlag = False
        ROIRotateFlag = False
        ROIRTFlag = False
        ROILDFlag = False
        Timer2.Enabled = False
    End Sub
    Private Sub AxIpeDspCtrl1_MouseMoveEvent(ByVal sender As Object, ByVal e As AxIpeDspCtrlLib._DIpeDspCtrlEvents_MouseMoveEvent) Handles AxIpeDspCtrl1.MouseMoveEvent
        nErr = hSherlock.RoiRotationGet(CmbSelRoi.Text, tempROIAngle)
        hSherlock.RoiCoordArrayGet(CmbSelRoi.Text, ROIPtsXArray, ROIPtsYArray)
        hSherlock.RoiTypeGet(CmbSelRoi.Text, ROItype)
        If (ROItype = "RoiRect" Or ROItype = "RoiRake") Then
            Call Rect_MouseMove(e.x, e.y)
            If (ROIMoveFlag = True) Then
                AxIpeDspCtrl1.UseWaitCursor = True
                Me.Cursor = Cursors.SizeAll
                Timer1.Enabled = True
            End If
            If (ROILTFlag = True Or ROIRDFlag = True Or ROIRTFlag = True Or ROILDFlag = True) Then
                Timer2.Enabled = True
                AxIpeDspCtrl1.UseWaitCursor = True
                Me.Cursor = Cursors.SizeNWSE
            End If
            If (ROIRotateFlag = True) Then
                Call Rect_MouseMove(e.x, e.y)
                hSherlock.RoiRotationSet(CmbSelRoi.Text, ROIAngle + tempIniRotateAngle)
                AxIpeDspCtrl1.UpdateDisplay()
            End If
        End If
    End Sub
    Private Sub ChkALive_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkALive.CheckedChanged
        If (ChkALive.CheckState = CheckState.Checked) Then
            AxIpeDspCtrl1.ConnectImgWindow("imgA")
            nErr = hSherlock.SoLiveSet("imgA", 1)
        End If
        If (ChkALive.CheckState = CheckState.Unchecked) Then
            AxIpeDspCtrl1.ConnectImgWindow("检测窗口")
            nErr = hSherlock.SoLiveSet("imgA", 0)
        End If
    End Sub
    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        Dim i As Double
        Dim path As String
        For i = 5 To 100
            path = "D:\ImageSave\" + Format(Now.AddDays(-i), "yyyyMMdd")
            If My.Computer.FileSystem.DirectoryExists(path) Then
                My.Computer.FileSystem.DeleteDirectory(path, FileIO.DeleteDirectoryOption.DeleteAllContents)
            End If
        Next i
    End Sub
    Private Sub Timer4_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer4.Tick
        nErr = hSherlock.InvModeSet(IpeEngCtrlLib.I_MODE.I_EXE_MODE_HALT_AFTER_ITERATION)
        'allow engine to halt and process any messages from the engine while it stops
        Dim m As IpeEngCtrlLib.I_MODE
        Do
            System.Windows.Forms.Application.DoEvents()
            hSherlock.InvModeGet(m)
        Loop Until m = IpeEngCtrlLib.I_MODE.I_EXE_MODE_HALT
        Timer4.Enabled = False
    End Sub
    Dim vbExposureTime As Double

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtnShowRoi.Click
        'hSherlock.RoiShowOutlineSet(CmbSelRoi.Text, True)
        If roiState = True Then
            hSherlock.RoiShowOutlineSet(CmbSelRoi.Text, False)
            roiState = False
            BtnShowRoi.Text = "显示框"
        Else
            hSherlock.RoiShowOutlineSet(CmbSelRoi.Text, True)
            roiState = True
            BtnShowRoi.Text = "隐藏框"
        End If

        AxIpeDspCtrl1.Refresh()
    End Sub

    Private Sub BtnModify_Click(sender As Object, e As EventArgs) Handles BtnModify.Click
        If modifyState = True Then
            hSherlock.RoiFlagsSet(CmbSelRoi.Text, False)
            modifyState = False
            BtnModify.Text = "处理图形"
        Else
            hSherlock.RoiFlagsSet(CmbSelRoi.Text, True)
            modifyState = True
            BtnModify.Text = "隐藏处理"
        End If
        hSherlock.EngExecuteSub("Inspect")
        hSherlock.RoiShowOutlineSet(CmbSelRoi.Text, True)
        hSherlock.RoiSelectedSet(CmbSelRoi.Text, True)
        AxIpeDspCtrl1.UpdateDisplay()
        'AxIpeDspCtrl1.Refresh()
    End Sub

    Private Sub DETECTED_CELL_SIZE_LEVEL_MM_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text0.KeyPress
        Label16.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("DETECTED_CELL_SIZE_LEVEL_MM", Val(Text0.Text))
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Label16.Hide()
        Label17.Hide()
        Label18.Hide()
        Label19.Hide()
        Label20.Hide()
        Label21.Hide()
        Label22.Hide()
        Label23.Hide()
        Label24.Hide()
        Label25.Hide()
        Label26.Hide()
        nErr = hSherlock.InvSave(sNowSolutionPath)
    End Sub

    Private Sub DETECTED_CELL_HEIGHT_LEVEL_MM_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text1.KeyPress
        Label17.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("DETECTED_CELL_HEIGHT_LEVEL_MM", Val(Text1.Text))
        End If
    End Sub

    Private Sub DETECTED_CELL_WIDTH_LEVEL_MM_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text2.KeyPress
        Label18.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("DETECTED_CELL_WIDTH_LEVEL_MM", Val(Text2.Text))
        End If
    End Sub

    Private Sub SCREEN_ROTATE_ERROR_LEVEL_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text3.KeyPress
        Label19.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("SCREEN_ROTATE_ERROR_LEVEL", Val(Text3.Text))
        End If
    End Sub

    Private Sub SCREEN_SHIFT_ERROR_LEVEL_MM_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text4.KeyPress
        Label20.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("SCREEN_SHIFT_ERROR_LEVEL_MM", Val(Text4.Text))
        End If
    End Sub

    Private Sub EDGE_ERROR_LEVEL_PIX_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text5.KeyPress
        Label21.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("EDGE_ERROR_LEVEL_PIX", Val(Text5.Text))
        End If
    End Sub

    Private Sub CORNER_ERROR_LEVEL_PIX_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text6.KeyPress
        Label22.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("CORNER_ERROR_LEVEL_PIX", Val(Text6.Text))
        End If
    End Sub

    Private Sub EDGE_BREAKAGE_LEVEL_PIX_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text7.KeyPress
        Label23.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("EDGE_BREAKAGE_LEVEL_PIX", Val(Text7.Text))
        End If
    End Sub

    Private Sub INSPECT_ROI_DIAGONLA_WIDTH_PIX_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text8.KeyPress
        Label24.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("INSPECT_ROI_DIAGONLA_WIDTH_PIX", Val(Text8.Text))
        End If
    End Sub

    Private Sub CORNER_BREAKAGE_LEVEL_PIX_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text9.KeyPress
        Label25.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("CORNER_BREAKAGE_LEVEL_PIX", Val(Text9.Text))
        End If
    End Sub

    Private Sub INSPECT_INNER_ERROR_THRESHOLD_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Text10.KeyPress
        Label26.Show()
        If e.KeyChar = Chr(Keys.Enter) Then
            nErr = hSherlock.VarSetDouble("INSPECT_INNER_ERROR_THRESHOLD", Val(Text10.Text))
        End If
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        If SizeHeightEnalbe = 0 Then
            hSherlock.VarSetDouble("DETECTED_CELL_HEIGHT_SKIP", 1)
            SizeHeightEnalbe = 1
            Button1.BackColor = Color.Red
        Else
            hSherlock.VarSetDouble("DETECTED_CELL_HEIGHT_SKIP", 0)
            SizeHeightEnalbe = 0
            Button1.BackColor = Color.Lime
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If SizeWidthEnalbe = 0 Then
            hSherlock.VarSetDouble("DETECTED_CELL_WIDTH_SKIP", 1)
            SizeWidthEnalbe = 1
            Button2.BackColor = Color.Red
        Else
            hSherlock.VarSetDouble("DETECTED_CELL_WIDTH_SKIP", 0)
            SizeWidthEnalbe = 0
            Button2.BackColor = Color.Lime
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If ScreenRotateEnalbe = 0 Then
            hSherlock.VarSetDouble("SCREEN_ROTATE_ERROR_SKIP", 1)
            ScreenRotateEnalbe = 1
            Button3.BackColor = Color.Red
        Else
            hSherlock.VarSetDouble("SCREEN_ROTATE_ERROR_SKIP", 0)
            ScreenRotateEnalbe = 0
            Button3.BackColor = Color.Lime
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If ScreenShiftEnalbe = 0 Then
            hSherlock.VarSetDouble("SCREEN_SHIFT_ERROR_SKIP", 1)
            ScreenShiftEnalbe = 1
            Button4.BackColor = Color.Red
        Else
            hSherlock.VarSetDouble("SCREEN_SHIFT_ERROR_SKIP", 0)
            ScreenShiftEnalbe = 0
            Button4.BackColor = Color.Lime
        End If
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If Rotate90Enable = 0 Then
            hSherlock.VarSetDouble("ROTATE90_ERROR_SKIP", 1)
            Rotate90Enable = 1
            Button7.BackColor = Color.Red
        Else
            hSherlock.VarSetDouble("ROTATE90_ERROR_SKIP", 0)
            Rotate90Enable = 0
            Button7.BackColor = Color.Lime
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If EdgeInspectEnalbe = 0 Then
            hSherlock.VarSetDouble("EDGE_ERROR_SKIP", 1)
            EdgeInspectEnalbe = 1
            Button5.BackColor = Color.Red
        Else
            hSherlock.VarSetDouble("EDGE_ERROR_SKIP", 0)
            EdgeInspectEnalbe = 0
            Button5.BackColor = Color.Lime
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If CornerInspectEnalbe = 0 Then
            hSherlock.VarSetDouble("CORNER_ERROR_SKIP", 1)
            CornerInspectEnalbe = 1
            Button6.BackColor = Color.Red
        Else
            hSherlock.VarSetDouble("CORNER_ERROR_SKIP", 0)
            CornerInspectEnalbe = 0
            Button6.BackColor = Color.Lime
        End If
    End Sub

    Private Sub BtnAnnotations_Click(sender As Object, e As EventArgs) Handles BtnAnnotations.Click
        If annoState = True Then
            hSherlock.RoiShowAnnotSet(CmbSelRoi.Text, False)
            annoState = False
            BtnAnnotations.Text = "显示点"
        Else
            hSherlock.RoiShowAnnotSet(CmbSelRoi.Text, True)
            annoState = True
            BtnAnnotations.Text = "隐藏点"
        End If
        'hSherlock.RoiShowAnnotSet(CmbSelRoi.Text, True)
        hSherlock.EngExecuteSub("Inspect")
        AxIpeDspCtrl1.Refresh()
    End Sub

    Private Sub BtnSetVariable_Click(sender As Object, e As EventArgs) Handles BtnSetVariable.Click
        If setVarState = False Then
            AxIpeDspCtrl1.Hide()
            ChkALive.Hide()
            GroupBoxSetVariable.Show()
            GroupBox1.Enabled = False
            BtnSolutoon.Enabled = False
            BtnStart.Enabled = False
            BtnStop.Enabled = False
            Btnparameter.Enabled = False
            BtnSetVariable.Text = "返回主页"
            setVarState = True
            '
            'Dim n As Integer
            For i = 0 To 10
                nErr = hSherlock.VarGetDouble(variables(i), temp)
                textBoxes(i).Text = temp.ToString
            Next
        Else
            AxIpeDspCtrl1.Show()
            ChkALive.Show()
            GroupBoxSetVariable.Hide()
            GroupBox1.Enabled = True
            BtnSolutoon.Enabled = True
            BtnStart.Enabled = True
            BtnStop.Enabled = True
            Btnparameter.Enabled = True
            BtnSetVariable.Text = "参数设定"
            setVarState = False
        End If

    End Sub

    Private Sub ButShutter_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButShutter.Click
        ButShutter.Enabled = False
        vbExposureTime = CDbl(CamShutter.Text)
        nErr = hSherlock.VarSetDouble("ExposureTime", vbExposureTime)
        nErr = hSherlock.EngExecuteSub("Ini")
        ChkALive.CheckState = CheckState.Checked
        AxIpeDspCtrl1.UpdateDisplay()
        ButShutter.Enabled = True
    End Sub
End Class