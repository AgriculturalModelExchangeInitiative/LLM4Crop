Sub iCrops()
'----------------- A simple crop model for wheat, maize and grain legumes (soybean, chickpea et al)
'AS at NCSU & GUASNR, Nov. 2010 - Aug. 2011
'AS at GUASNR, March 2016 - August 2017
'As at Montpellier SupAGro, March-August 2018 (GW thesis)
'Latest modifications:
    'An error in response to VPD corrected, July 2016
    '"Yr = Pyear And" was inactivated in "FindSimSowDate" because the model stopped when simulation and sowing doy are not in the same year, Jan 2017
    'Several sowing algorithms were included, 12 June 2017
    'StopDap was addedd which is DAP when the crop must be removed from the field, 19 June 2017 --> modified oct 31st by HM (DOY into DAP)
    'MATYP in the outout indicates type of manurity: (1) normal, (2) pre-mature (LAI<0.05), (3) planting not possible in the sowing window, (4) StopDap is reached, (5) Flood kill, 19 June 2017
    'N fertilization based on DOY is possible now, 19 June 2017
    'Variable grain N simulation included, July 2017
    'Maize crop added, 12 August 2017
    'NO CO2 effects on RUE and TEC (removed from version delivered on 13 August 2017)
    'DENS effect on PLAPOW
    'phenology as follow :
    '   Wheat : tempfun follows a beta function, ppfun follows a quadratic function (1-ppsen(pp-cp)^2)
    '           vernfun: uses parameters from the crops excel sheet
    '   Legumes: tempfun follows a dent like function, ppfun follows a linear function (1-ppsen(pp-cp)^2)
    '          BRP, TRP, TLM, TLP,stages are calculated from R1,R3,R5,R7 in the code, not in the crop parameter sheet
    '   Maize: tempfun follows a dentlike function,
    '          ppfun followsa weird hyperbolic function (1/(1+ppsen/4(pp-cpp))
    '          development rate is affected by drought during the seed growing phase only
    '          many phenological stages calcaluated or coded in hard. Card temperature in hard in the code removed
    'Heat, Frost and Flood effect included for all crops :  effect on LAI, but no effect on harvest index
    '           Heat: modif: replace DLAI by LAI in the algoritm : heat efect is possible even when no senscence already
    'survival model revised, using EPCOND and LTWCT introduced for all crops.LAI reduction as corrected on august 14th 2014
    '               LAI reduction (LAI<0.05)   due to leaf dryness causes plant death only if FTSW<0.05 too
    'N accumulated in grain can vary between GNCmin adn GCmax as entered in crop parameters
    'BNF estimated for legumes
    'vpd response available
    'Ritchie 2-Stage soil evap included, included and activabl using parameters semethod in soil excel sheet1 Feb 2012 (select semethod=2)
    'semethod = 1=--> simplified soil evaporation ; semethod = 2 --> Ritchie 2 stages evap
    'Mulching effect on evaporation as corrected in 2017 (Ln instead of Log)
    'effect of water stress on N fixation for legumes has been re-added (may 12th 2020) HM
    'Sowing window is specified by Fpdoy and SearchDur, (was Lpdoy since 19 June 2017) HM

    

'////////////////////////////////////////////////// MAIN PROGRAM
 Application.ScreenUpdating = False
 
 Dim scnName As String
 Dim locName As String
 Dim mngName As String
 Dim solName As String
 Dim crpName As String
 Dim wthFile As String
 
 FrstScnRow = ThisWorkbook.Worksheets(1).Cells(5, 2)
 FrstScnRow = FrstScnRow - 1
 scnNo = ThisWorkbook.Worksheets(1).Cells(6, 2)
 scyrCntr = 1
 dayCntr = 0

For scnCntr = 1 To scnNo
'If ThisWorkbook.Worksheets("Run").Cells(4, 5).Value = "yes" Then dayCntr = 0
dayCntr = 0
    LocRowNo = ThisWorkbook.Worksheets("Run").Cells(scnCntr + FrstScnRow, 2)
    MangRowNo = ThisWorkbook.Worksheets("Run").Cells(scnCntr + FrstScnRow, 3)
    SoilRowNo = ThisWorkbook.Worksheets("Run").Cells(scnCntr + FrstScnRow, 4)
    CropColNo = ThisWorkbook.Worksheets("Run").Cells(scnCntr + FrstScnRow, 5)
    
    scnName = ThisWorkbook.Worksheets("Run").Cells(scnCntr + FrstScnRow, 1)
    locName = ThisWorkbook.Worksheets("Location").Cells(LocRowNo + 2, 1)
    wthFile = ThisWorkbook.Worksheets("Location").Cells(LocRowNo + 2, 4)
    mngName = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 2, 1)
    solName = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 2, 1)
    crpName = ThisWorkbook.Worksheets("Crop").Cells(5, CropColNo)

    Set wb = Workbooks.Open(wthFile, True, True)

    ReDim DLYER(10), MAI(10), DRAINF(10)
    ReDim WLAD(10), WLLL(10), WLUL(10), WLST(10), WL(10)
    ReDim ATSW(10), TTSW(10), FTSW(10), FLIN(10), FLOUT(10)
    ReDim RT(10), WU(10), SE(10), RLYER(10), NFA01(10)
    ReDim DAPNF(10), NFERTI(10), VOLFI(10), MNORG(10), NSOL(10), NCON(10)
    ReDim NOUT(10), NMIN(10), NDNIT(10), NAVL(10), NU(10), NUDF(10)
    ReDim DAPIR(20), IRGWI(20), WAT01(20), iATSW(20), iFTSW(20), INSOL(20)

    Pyear = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 2)
    yrno = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 3)
    wthRow = ThisWorkbook.Worksheets("Location").Cells(LocRowNo + 2, 5)
    water = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 6, 6)
    nitrogen = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 6, 2)

    Call Headers
    For yrCntr = 1 To yrno
        GoSub LocManagInputs
        GoSub FindSimSowDate
        Do Until MAT = 1
           GoSub Weather
           GoSub PhenologyBD
           If nitrogen = 0 Or water = 0 Then GoSub CropLAI
           If nitrogen = 2 Then GoSub CropLAIN
           GoSub DMProduction
           GoSub DMDistribution
           If nitrogen = 2 And water > 0 Then GoSub PlantN
           If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
           If nitrogen = 2 And water > 0 Then GoSub SoilN
           GoSub DailyPrintOut
        Loop
        GoSub SummaryPrintOut
        Pyear = Pyear + 1
        scyrCntr = scyrCntr + 1
    Next yrCntr
    wb.Close False
    Set wb = Nothing
Next scnCntr

Application.ScreenUpdating = True
Exit Sub
'////////////////////////////////////////////////// END OF MAIN PROGRAM




LocManagInputs: '////////////////////////////////// LOC/MANAG/INPUTS
  LAT = ThisWorkbook.Worksheets("Location").Cells(LocRowNo + 2, 2)
  VPDF = ThisWorkbook.Worksheets("Location").Cells(LocRowNo + 2, 3)
  'tchng = ThisWorkbook.Worksheets("Location").Cells(LocRowNo + 2, 5)
  'pchng = ThisWorkbook.Worksheets("Location").Cells(LocRowNo + 2, 6)
  'CO2 = ThisWorkbook.Worksheets("Location").Cells(LocRowNo + 2, 7)
  'CO2REF = 350
  
  FixFind = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 1)
  SimDoy = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 4)
  Pdoy = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 5)
  If SimDoy = Pdoy Then SimDoy = Pdoy - 1
  SearchDur = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 6)
  StopDap = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 7)
  SowTmp = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 8)
  SowWat = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 9)
  PDEN = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 10)
  
  STBLW = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 4, 11)
  
  water = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 6, 6)
  IRGLVL = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 7, 7)

  If water = 3 Or water = 4 Then
     IRNUM = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 7, 6)         'Number of irrg. application
     WATiT = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 8, 6)         '1=DAP-based, 2=CBD-based, 3=DOY-based
     For N = 1 To IRNUM
         DAPIR(N) = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 10 + N, 5)   'Time of irrig. application
         IRGWI(N) = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 10 + N, 6)   'Irrig. water (mm)
         WAT01(N) = 0
     Next N
  End If
    
  nitrogen = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 6, 2)
  If nitrogen = 2 Then
     FN = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 7, 2)         'Number of N application
     NATiT = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 8, 2)      '1=DAP-based, 2=CBD-based,  3=DOY-based
     For N = 1 To FN
         DAPNF(N) = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 10 + N, 2)   'Time of N application as top-dressing
         NFERTI(N) = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 10 + N, 3)  'N fertilizer amount at the top-dressing (gN.m-2)
         VOLFI(N) = ThisWorkbook.Worksheets("Manag").Cells(MangRowNo + 10 + N, 4)   'Fraction volatilization
         NFA01(N) = 0
     Next N
  End If
  CUMBNF = 0
  
  '------------------------------- Initials
   MAT = 0
   iniPheno = 0
   iniLAI = 0
   iniDMP = 0
   iniDMD = 0
   iniSW = 0
   iniPNB = 0
   iniSNB = 0
   MATYP = 0 ' GW correction MATYP
Return





Weather:    '////////////////////////////////////// WEATHER
   Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
   DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
   SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
   TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
   TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
   RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)

   TMIN = TMIN '+ tchng
   TMAX = TMAX '+ tchng
   RAIN = RAIN '* pchng

   TMAXB = wb.Worksheets("Data").Cells(wthRow - 1, 4)       'wb.Worksheets("Sheet1").Cells(wthRow - 1, 4)
   TMINA = wb.Worksheets("Data").Cells(wthRow + 1, 5)       'wb.Worksheets("Sheet1").Cells(wthRow + 1, 5)

   TMP = (TMAX + TMIN) / 2
   wthRow = wthRow + 1
Return





PhenologyBD:    '////////////////////////////////// PHENOLOGY
crpName = UCase(crpName)
Select Case crpName
Case "WHEAT" '------------------------------------------------------WHEAT PHENOLOGY
If iniPheno = 0 Then
   'Cardinal temperature
   TBD = ThisWorkbook.Worksheets("Crop").Cells(69, CropColNo)
   TP1D = ThisWorkbook.Worksheets("Crop").Cells(70, CropColNo)
   TP2D = ThisWorkbook.Worksheets("Crop").Cells(71, CropColNo)
   TCD = ThisWorkbook.Worksheets("Crop").Cells(72, CropColNo)

   TBVER = ThisWorkbook.Worksheets("Crop").Cells(38, CropColNo)
   TP1VER = ThisWorkbook.Worksheets("Crop").Cells(39, CropColNo)
   TP2VER = ThisWorkbook.Worksheets("Crop").Cells(40, CropColNo)
   TCVER = ThisWorkbook.Worksheets("Crop").Cells(41, CropColNo)
   VDSAT = ThisWorkbook.Worksheets("Crop").Cells(42, CropColNo)
   vsen = ThisWorkbook.Worksheets("Crop").Cells(74, CropColNo)

   cpp = ThisWorkbook.Worksheets("Crop").Cells(76, CropColNo)
   ppsen = ThisWorkbook.Worksheets("Crop").Cells(77, CropColNo)

   '>>> Biodays to phenological stages
   bdSOWEMR = ThisWorkbook.Worksheets("Crop").Cells(79, CropColNo)
   bdEMRTIL = ThisWorkbook.Worksheets("Crop").Cells(80, CropColNo)
   bdTILSEL = ThisWorkbook.Worksheets("Crop").Cells(81, CropColNo)
   bdSELBOT = ThisWorkbook.Worksheets("Crop").Cells(82, CropColNo)
   bdBOTEAR = ThisWorkbook.Worksheets("Crop").Cells(83, CropColNo)
   bdEARANT = ThisWorkbook.Worksheets("Crop").Cells(84, CropColNo)
   bdANTPM = ThisWorkbook.Worksheets("Crop").Cells(85, CropColNo)
   bdPMHM = ThisWorkbook.Worksheets("Crop").Cells(86, CropColNo)
   
   bdEMR = bdSOWEMR
   bdTIL = bdEMR + bdEMRTIL
   bdSEL = bdTIL + bdTILSEL
   bdBOT = bdSEL + bdSELBOT
   bdEAR = bdBOT + bdBOTEAR
   bdANT = bdEAR + bdEARANT
   bdPM = bdANT + bdANTPM
   bdMAT = bdPM + bdPMHM
   
   '>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Physiological important biodays (stages)
   bdBRV = bdEMR                'Beg. response to vernalization
   bdTRV = bdSEL                'End of response to vernalization
      
   bdBRP = bdEMR                'Beg. responsr to photoperiod (emergence)
   bdTRP = bdSEL                'Termination response to photoperiod (stem elongation)

   bdBSG = bdANT + 5            'bdANT + 5  'BSG or beginning linear HI 5 days after anthesis
   bdTSG = bdPM - 1.5           'bdPM - 1.5 'TSG or termination of linear HI 1.5 days before physiological maturity

   bdBLG = bdEMR                'Leaf production starts after emergence
   bdTLM = bdBOT                'Leaf production on mainstem terminates at booting
   bdTLP = bdBSG                'Leaf production on plant terminates at begning seed growth
   bdBLS = bdBSG

   'bdFLW = bdANT                'Flowering
   'bdBXTF = bdANT - 5           'Beg. flowering window
   'bdTXTF = bdANT + 12          'End of flowering window

   bdBRG = bdEMR                'Beg. root depth grwoth
   bdTRG = bdBSG                'End of root growth
   bdBNF = 999                  'Beg. N fixation: a very high value that means no N fixation
   '<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
   
   SNOW = 0:    CUMVER = 0:  CBD = 0:     DAP = 0:     FTSW(1) = 0.5:  WSFD = 1:
   DAYT = 0:    SRAINT = 0:  STMINT = 0:  STMAXT = 0:  SSRADT = 0:     SUMETT = 0:
   DAY3 = 0:    SRAIN3 = 0:  STMIN3 = 0:  STMAX3 = 0:  SSRAD3 = 0:     SUMET3 = 0:
   DAY2 = 0:    SRAIN2 = 0:  STMIN2 = 0:  STMAX2 = 0:  SSRAD2 = 0:     SUMET2 = 0:
   iniPheno = 1
End If
  
'----------------------------- Vernalization function
     '--- Snow Cover and Melt
     SNOMLT = 0
     If TMAX <= 1 Then
        SNOW = SNOW + RAIN
        RAIN = 0
     ElseIf SNOW > 0 Then
        SNOMLT = TMAX + RAIN * 0.4
        If SNOMLT > SNOW Then SNOMLT = SNOW
        SNOW = SNOW - SNOMLT:     RAIN = RAIN + SNOMLT
     End If

     '--- Crown Temperature
     TMINCR = TMIN
     TMAXCR = TMAX
     XS = SNOW
     If XS > 15 Then XS = 15
     If TMIN < 0 And XS > 0 Then TMINCR = 2 + TMIN * (0.4 + 0.0018 * (XS - 15) ^ 2)
     If TMAX < 0 And XS > 0 Then TMAXCR = 2 + TMAX * (0.4 + 0.0018 * (XS - 15) ^ 2)
     TMPCR = (TMAXCR + TMINCR) / 2

     '--- Vernalization day per calendar day
     VERDAY = 0
     If TMPCR <= TBVER Or TMPCR >= TCVER Then
        VERDAY = 0
     ElseIf TMPCR > TBVER And TMPCR < TP1VER Then
        VERDAY = (TMPCR - TBVER) / (TP1VER - TBVER)
     ElseIf TMPCR > TP2VER And TMPCR < TCVER Then
        VERDAY = ((TCVER - TMPCR) / (TCVER - TP2VER))
     ElseIf TMPCR >= TP1VER And TMPCR <= TP2VER Then
        VERDAY = 1
     End If

     '--- Cumulative vernalization day
     CUMVER = CUMVER + VERDAY
     If CUMVER < 10 And TMAX > 30 Then CUMVER = CUMVER - 0.5 * (TMAX - 30)
     If CUMVER < 0 Then CUMVER = 0
      
     If CBD >= bdBRV And CBD <= bdTRV Then
        verfun = 1 - vsen * (VDSAT - CUMVER)
        If verfun < 0 Then verfun = 0
        If verfun > 1 Then verfun = 1
     Else
         verfun = 1
     End If

'----------------------------- Temperature function
' beta function
  If TMP <= TBD Or TMP >= TCD Then
     tempfun = 0
  Else
     part1 = (TCD - TMP) / (TCD - TP1D)
     part2 = (TMP - TBD) / (TP1D - TBD)
     pow = (TP1D - TBD) / (TCD - TP1D)
     tempfun = (part1 * part2 ^ pow)
  End If
  
  ' dent-like function
'  If TMP <= TBD Or TMP >= TCD Then
'     tempfun = 0
'  ElseIf TMP > TBD And TMP < TP1D Then
'     tempfun = (TMP - TBD) / (TP1D - TBD)
'  ElseIf TMP > TP2D And TMP < TCD Then
'     tempfun = ((TCD - TMP) / (TCD - TP2))
'  ElseIf TMP >= TP1D And TMP <= TP2D Then
'     tempfun = 1
'  End If


'----------------------------- Photoperiod function
  Pi = 3.141592654:      RDN = Pi / 180
  DEC = Sin(23.45 * RDN) * Cos(2 * Pi * (DOY + 10) / 365)
  DEC = Atn(DEC / Sqr(1 - DEC ^ 2)) * -1
  DECL = DEC * 57.29578
  SINLD = Sin(RDN * LAT) * Sin(DEC)
  COSLD = Cos(RDN * LAT) * Cos(DEC)
  AOB = SINLD / COSLD
  AOB2 = Atn(AOB / Sqr(1 - AOB ^ 2))
  DAYL = 12 * (1 + 2 * AOB2 / Pi)
  pp = DAYL + 0.9
  
  If CBD >= bdBRP And CBD <= bdTRP Then
     If pp < cpp Then
        ppfun = 1 - ppsen * (cpp - pp) ^ 2
        If ppfun < 0 Then ppfun = 0
     Else
        ppfun = 1
     End If
  Else
     ppfun = 1
  End If

'----------------------------- Biological day
  If CBD <= bdEMR Then
     DTU = (TP1D - TBD) * tempfun          'daily temperature unit
     bd = verfun * tempfun * ppfun
     If FTSW(1) <= 0 Then bd = 0
  ElseIf CBD > bdEMR Then
     DTU = (TP1D - TBD) * tempfun * WSFD   'daily temperature unit
     bd = verfun * tempfun * ppfun * WSFD
  End If

  CBD = CBD + bd
  
  DAP = DAP + 1

'----------------------------- Days to stages
  If CBD < bdEMR Then dtEMR = DAP + 1
  If CBD < bdTIL Then dtTIL = DAP + 1: dtR0 = dtTIL
  If CBD < bdSEL Then dtSEL = DAP + 1: dtR1 = dtSEL
  If CBD < bdBOT Then dtBOT = DAP + 1: dtR2 = dtBOT
  If CBD < bdEAR Then dtEAR = DAP + 1: dtR3 = dtEAR
  If CBD < bdANT Then dtANT = DAP + 1: dtR4 = dtANT
  If CBD < bdBSG Then dtBSG = DAP + 1: dtR5 = dtBSG
  If CBD < bdTSG Then dtTSG = DAP + 1: dtR6 = dtTSG
  If CBD < bdPM Then dtPM = DAP + 1: dtR7 = dtPM
  If CBD < bdMAT Then dtMAT = DAP + 1: dtR8 = dtMAT
  
  If CBD > bdMAT Then
     MAT = 1                            ' GW & HM Correction 17/07/2018 : MATYP = 1
     If MATYP <= 1 Then MATYP = 1
  End If
  If DOY = StopDap Then
     MAT = 1: MATYP = 4
  End If
  
  

Case "MAIZE" '------------------------------------------------------MAIZE PHENOLOGY
  If iniPheno = 0 Then
     TBD = ThisWorkbook.Worksheets("Crop").Cells(69, CropColNo)  'Sheet5.[b49]
     TP1D = ThisWorkbook.Worksheets("Crop").Cells(70, CropColNo)
     TP2D = ThisWorkbook.Worksheets("Crop").Cells(71, CropColNo)
     TCD = ThisWorkbook.Worksheets("Crop").Cells(72, CropColNo)

     cpp = ThisWorkbook.Worksheets("Crop").Cells(76, CropColNo)
     ppsen = ThisWorkbook.Worksheets("Crop").Cells(77, CropColNo)

     bdSOWEMR = ThisWorkbook.Worksheets("Crop").Cells(79, CropColNo)
     bdEMREJU = ThisWorkbook.Worksheets("Crop").Cells(80, CropColNo)
     bdSILPM = ThisWorkbook.Worksheets("Crop").Cells(81, CropColNo)
     PHYL = ThisWorkbook.Worksheets("Crop").Cells(7, CropColNo)
     
     bdEMR = bdSOWEMR                    'Emergence
     bdEJU = bdEMR + bdEMREJU            'End of juvenile
     bdEJUTSI = 4                        'End of juvenile to tassel initiation
     bdTSI = bdEJU + bdEJUTSI
     bdTSISIL = ((15 + 0.5) * PHYL) / (TP1D - TBD) - (bdTSI - bdEMR)  'Tasseling occurs when the plant has 15.5 leaves on the main stem
     bdSIL = bdTSI + bdTSISIL            'Silking
     bdTSE = bdSIL - 76 / (TP1D - TBD)   'TSE occures 76 oC before silking
     bdTSG = bdSIL + 0.95 * bdSILPM      'Termination seed growth
     bdPM = bdTSG + 0.05 * bdSILPM       'Physiological maturity
     bdMAT = bdPM + 4                    'Harvest maturity
     
     bdBRP = bdEJU                       'Beg. response to photoperiod
     bdTRP = bdTSI                       'End of response to photoperiod
     bdBSG = bdSIL + 170 / (TP1D - TBD)  'BSG 170 oC after silking
     bdTSG = bdSIL + 0.95 * bdSILPM
     bdBLG = bdEMR
     bdTLM = bdSIL                       'Leaf production on mainstem terminates
     bdTLP = bdSIL
     bdBLS = bdBSG
     'bdFLW = bdSIL              'Biodays to flowering
     'bdBXTF = bdSIL - 5         'Beginning flowering window
     'bdTXTF = bdSIL + 10        'Termination flowering window
     bdBRG = bdEMR
     bdTRG = bdBSG
     bdBNF = 999                'A very high value that means no N fixation
          
     CBD = 0:    DAP = 0:   WSFD = 1:   tuEMRTSI = 0:   tuTSI = 0:  xTSI = 0:
     DAYT = 0:    SRAINT = 0:  STMINT = 0:  STMAXT = 0:  SSRADT = 0:     SUMETT = 0:
     DAY3 = 0:    SRAIN3 = 0:  STMIN3 = 0:  STMAX3 = 0:  SSRAD3 = 0:     SUMET3 = 0:
     DAY2 = 0:    SRAIN2 = 0:  STMIN2 = 0:  STMAX2 = 0:  SSRAD2 = 0:     SUMET2 = 0:
     
     iniPheno = 1
  End If

'----------------------------- Temperature function
' dent-like function
  If TMP <= TBD Or TMP >= TCD Then
     tempfun = 0
  ElseIf TMP > TBD And TMP < TP1D Then
     tempfun = (TMP - TBD) / (TP1D - TBD)
  ElseIf TMP > TP2D And TMP < TCD Then
     tempfun = ((TCD - TMP) / (TCD - TP2))
  ElseIf TMP >= TP1D And TMP <= TP2D Then
     tempfun = 1
  End If

'----------------------------- Photoperiod function
  If CBD >= bdBRP And CBD <= bdTRP Then
     Pi = 3.141592654:      RDN = Pi / 180
     DEC = Sin(23.45 * RDN) * Cos(2 * Pi * (DOY + 10) / 365)
     DEC = Atn(DEC / Sqr(1 - DEC ^ 2)) * -1
     DECL = DEC * 57.29578
     SINLD = Sin(RDN * LAT) * Sin(DEC)
     COSLD = Cos(RDN * LAT) * Cos(DEC)
     AOB = SINLD / COSLD
     AOB2 = Atn(AOB / Sqr(1 - AOB ^ 2))
     DAYL = 12 * (1 + 2 * AOB2 / Pi)
     pp = DAYL + 0.9
     
     If pp > cpp Then
        RATEIN = 1 / (bdEJUTSI + ppsen * (pp - cpp))
        rmax = 1 / bdEJUTSI
        ppfun = RATEIN / rmax
        If ppfun < 0 Then ppfun = 0
     Else
         ppfun = 1
     End If
  Else
     ppfun = 1
  End If
          
'----------------------------- Biological day
  DTU = (TP1D - TBD) * tempfun          'daily temperature unit
  If CBD > bdBSG Then DTU = DTU * WSFD

  If CBD >= bdBRP And CBD <= bdTRP Then
     bd = ppfun
  Else
     bd = tempfun
  End If
  If CBD > bdBSG Then bd = bd * WSFD    'water deficit only accelerate grain filling period
  
  CBD = CBD + bd
  DAP = DAP + 1
'----------------------------- Re-calculate CBDs to stages
  If CBD >= bdEMR And CBD <= bdTSI Then tuEMRTSI = tuEMRTSI + DTU
  If CBD > bdTSI And xTSI = 0 Then
     TLNO = tuEMRTSI / (PHYL * 0.5) + 5
     bdTSISIL = (((TLNO + 0.5) * PHYL) - tuEMRTSI) / (TP1D - TBD)
     xTSI = 1
        
     bdSIL = bdTSI + bdTSISIL
     bdTSE = bdSIL - 76 / (TP1D - TBD)            'TSE occures 76 oC before silking
     bdBSG = bdSIL + 170 / (TP1D - TBD)           'BSG 170 oC after silking
     bdTSG = bdSIL + 0.95 * bdSILPM
     bdPM = bdTSG + 0.05 * bdSILPM
     bdMAT = bdPM + 4
     bdTLM = bdSIL           'Leaf production on mainstem terminates
     
     bdBRP = bdEJU
     bdTRP = bdTSI
     bdBSG = bdSIL + 170 / (TP1D - TBD)           'BSG 170 oC after silking
     bdTSG = bdSIL + 0.95 * bdSILPM
     bdBLG = bdEMR
     bdTLM = bdSIL        'Leaf production on mainstem terminates
     bdTLP = bdSIL
     bdBLS = bdBSG
     'bdFLW = bdSIL
     'bdBXTF = bdSIL - 5
     'bdTXTF = bdSIL + 10
     bdBRG = bdEMR
     bdTRG = bdBSG
  End If


'----------------------------- Days to stages
  If CBD < bdEMR Then dtEMR = DAP + 1       'emergence
  If CBD < bdEJU Then dtEJU = DAP + 1       'end of juvenile
  If CBD < bdTSI Then dtTSI = DAP + 1       'tassel initiation
  If CBD < bdTSE Then dtTSE = DAP + 1       'tassel emergence
  If CBD < bdSIL Then dtSIL = DAP + 1       'silking
  If CBD < bdBSG Then dtBSG = DAP + 1       'beginning seed growth
  If CBD < bdTSG Then dtTSG = DAP + 1       'termination seed growth
  If CBD < bdPM Then dtPM = DAP + 1         'physiological maturity
  If CBD < bdMAT Then dtMAT = DAP + 1       'harvest maturity
  
  dtR0 = dtTSE
  dtR1 = dtSIL
  dtR7 = dtPM
  dtR8 = dtMAT
  dtR2 = -9:    dtR3 = -9:    dtR4 = -9:     dtR5 = -9:    dtR6 = -9

'------------------------------- Maturity?
  If CBD > bdMAT Then
     MAT = 1                ' GW & HM Correction 17/07/2018 : MATYP = 1
     If MATYP <= 1 Then MATYP = 1
  End If
  If DAP = StopDap Then
     MAT = 1: MATYP = 4
  End If




Case Else   '------------------------------------ LEGUME PHENOLOGY"
 If iniPheno = 0 Then
    TBD = ThisWorkbook.Worksheets("Crop").Cells(69, CropColNo)  'Sheet5.[b49]
    TP1D = ThisWorkbook.Worksheets("Crop").Cells(70, CropColNo) 'Sheet5.[b50]
    TP2D = ThisWorkbook.Worksheets("Crop").Cells(71, CropColNo) 'Sheet5.[b51]
    TCD = ThisWorkbook.Worksheets("Crop").Cells(72, CropColNo)  'Sheet5.[b52]
    
    cpp = ThisWorkbook.Worksheets("Crop").Cells(76, CropColNo)  'Sheet5.[b54]
    ppsen = ThisWorkbook.Worksheets("Crop").Cells(77, CropColNo) 'Sheet5.[b55]
    
    bdSOWEMR = ThisWorkbook.Worksheets("Crop").Cells(79, CropColNo)  'Sheet5.[b57]
    bdEMRR1 = ThisWorkbook.Worksheets("Crop").Cells(80, CropColNo)  'Sheet5.[b58]
    bdR1R3 = ThisWorkbook.Worksheets("Crop").Cells(81, CropColNo)  'Sheet5.[b59]
    bdR3R5 = ThisWorkbook.Worksheets("Crop").Cells(82, CropColNo)  'Sheet5.[b60]
    bdR5R7 = ThisWorkbook.Worksheets("Crop").Cells(83, CropColNo)  'Sheet5.[b61]
    bdR7R8 = ThisWorkbook.Worksheets("Crop").Cells(84, CropColNo)  'Sheet5.[b62]
    
    bdEMR = bdSOWEMR
    bdR1 = bdEMR + bdEMRR1
    bdR3 = bdR1 + bdR1R3
    bdR5 = bdR3 + bdR3R5
    bdR7 = bdR5 + bdR5R7
    bdR8 = bdR7 + bdR7R8
    bdMAT = bdR8
   '>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    bdBRP = bdEMR 'ThisWorkbook.Worksheets("Crop").Cells(88, CropColNo)
    bdTRP = bdR5 'ThisWorkbook.Worksheets("Crop").Cells(89, CropColNo)

    bdBSG = bdR3 'ThisWorkbook.Worksheets("Crop").Cells(90, CropColNo)
    bdTSG = bdR7 'ThisWorkbook.Worksheets("Crop").Cells(91, CropColNo)

    bdBLG = bdEMR
    bdTLM = bdR3 'ThisWorkbook.Worksheets("Crop").Cells(92, CropColNo)
    bdTLP = bdR5 'ThisWorkbook.Worksheets("Crop").Cells(93, CropColNo)
    bdBLS = bdR3 'ThisWorkbook.Worksheets("Crop").Cells(94, CropColNo)

    bdFLW = bdR1
    bdBXTF = bdR1 - 5
    bdTXTF = bdR1 + 10

    bdBRG = bdEMR
    bdTRG = bdBSG
    bdBNF = ThisWorkbook.Worksheets("Crop").Cells(85, CropColNo)
   '<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
          
    DAP = 0!:    CBD = 0!:    WSFD = 1!:   FTSW(1) = 0.5:
    DAYT = 0:    SRAINT = 0:  STMINT = 0:  STMAXT = 0:  SSRADT = 0:     SUMETT = 0:
    DAY3 = 0:    SRAIN3 = 0:  STMIN3 = 0:  STMAX3 = 0:  SSRAD3 = 0:     SUMET3 = 0:
    DAY2 = 0:    SRAIN2 = 0:  STMIN2 = 0:  STMAX2 = 0:  SSRAD2 = 0:     SUMET2 = 0:
    iniPheno = 1
 End If

'------------------------------- Thermal time calculation
  If TMP <= TBD Or TMP >= TCD Then
     tempfun = 0
  ElseIf TMP > TBD And TMP < TP1D Then
     tempfun = (TMP - TBD) / (TP1D - TBD)
  ElseIf TMP > TP2D And TMP < TCD Then
     tempfun = (TCD - TMP) / (TCD - TP2D)
  ElseIf TMP >= TP1D And TMP <= TP2D Then
     tempfun = 1
  End If

'----------------------------- Photoperiod function
  Pi = 3.141592654:      RDN = Pi / 180
  DEC = Sin(23.45 * RDN) * Cos(2 * Pi * (DOY + 10) / 365)
  DEC = Atn(DEC / Sqr(1 - DEC ^ 2)) * -1
  DECL = DEC * 57.29578
  SINLD = Sin(RDN * LAT) * Sin(DEC)
  COSLD = Cos(RDN * LAT) * Cos(DEC)
  AOB = SINLD / COSLD
  AOB2 = Atn(AOB / Sqr(1 - AOB ^ 2))
  DAYL = 12 * (1 + 2 * AOB2 / Pi)
  pp = DAYL + 0.9

  If ppsen >= 0 Then
     ppfun = 1 - ppsen * (cpp - pp)
  ElseIf ppsen < 0 Then
     ppfun = 1 - (-ppsen) * (pp - cpp)
  End If
  If ppfun > 1 Then ppfun = 1
  If ppfun < 0 Then ppfun = 0
   
  If CBD < bdBRP Then ppfun = 1
  If CBD > bdTRP Then ppfun = 1

'----------------------------- Biological day
  If CBD <= bdEMR Then
     DTU = (TP1D - TBD) * tempfun          'daily temperature unit
     bd = tempfun * ppfun
     If FTSW(1) <= 0 Then bd = 0
  ElseIf CBD > bdEMR Then
     DTU = (TP1D - TBD) * tempfun * WSFD   'daily temperature unit
     bd = tempfun * ppfun * WSFD
  End If

  CBD = CBD + bd
  DAP = DAP + 1
  
'----------------------------- Days to stages
  If CBD < bdEMR Then dtEMR = DAP + 1  'Saving days to EMR
  If CBD < bdR1 Then dtR1 = DAP + 1    'Saving days to R1
  If CBD < bdR3 Then dtR3 = DAP + 1    'Saving days to R3
  If CBD < bdR5 Then dtR5 = DAP + 1    'Saving days to R5
  If CBD < bdR7 Then dtR7 = DAP + 1    'Saving days to R7
  If CBD < bdR8 Then dtR8 = DAP + 1    'Saving days to R8
  'dtR0 = -9
  
'------------------------------- Maturity?
  If CBD > bdMAT Then
     MAT = 1                ' GW & HM Correction 17/07/2018 : MATYP = 1
     If MATYP <= 1 Then MATYP = 1
  End If
  If DAP = StopDap Then
     MAT = 1: MATYP = 4
  End If
End Select
  
'----------------------------------------------- ENVIRONMENTAL
  If CBD <= bdMAT Then  '--sowing to MAT
     DAYT = DAYT + 1
     SRAINT = SRAINT + RAIN
     STMINT = STMINT + TMIN
     STMAXT = STMAXT + TMAX
     SSRADT = SSRADT + SRAD
     SUMETT = SUMETT + SEVP + TR
     MTMINT = STMINT / DAYT
     MTMAXT = STMAXT / DAYT
  End If
  
  If CBD <= bdBSG Then  '--sowing to BSG
     DAY2 = DAY2 + 1
     SRAIN2 = SRAIN2 + RAIN
     STMIN2 = STMIN2 + TMIN
     STMAX2 = STMAX2 + TMAX
     SSRAD2 = SSRAD2 + SRAD
     SUMET2 = SUMET2 + SEVP + TR
     MTMIN2 = STMIN2 / DAY2
     MTMAX2 = STMAX2 / DAY2
  End If
  
  If CBD > bdBSG And CBD <= bdMAT Then   '--BSG to MAT
     DAY3 = DAY3 + 1
     SRAIN3 = SRAIN3 + RAIN
     STMIN3 = STMIN3 + TMIN
     STMAX3 = STMAX3 + TMAX
     SSRAD3 = SSRAD3 + SRAD
     SUMET3 = SUMET3 + SEVP + TR
     MTMIN3 = STMIN3 / DAY3
     MTMAX3 = STMAX3 / DAY3
  End If

Return
  




CropLAI:    '////////////////////////////////// CROP LAI NO N
'------------------------------- LAI initials and pars
If iniLAI = 0 Then
    PHYL = ThisWorkbook.Worksheets("Crop").Cells(7, CropColNo)
    PLACON = ThisWorkbook.Worksheets("Crop").Cells(8, CropColNo)
    PLAPOW = ThisWorkbook.Worksheets("Crop").Cells(9, CropColNo)
    'a_pla_den = ThisWorkbook.Worksheets("Crop").Cells(10, CropColNo)   'a=1 if there is no estimate
    'b_pla_den = ThisWorkbook.Worksheets("Crop").Cells(11, CropColNo)   'b=0 if there is no estimate
    SLA = ThisWorkbook.Worksheets("Crop").Cells(12, CropColNo)
    FRZTKIL = ThisWorkbook.Worksheets("Crop").Cells(13, CropColNo)
    FRZLDR = ThisWorkbook.Worksheets("Crop").Cells(14, CropColNo)
    HtLTH = ThisWorkbook.Worksheets("Crop").Cells(15, CropColNo)
    HtLDR = ThisWorkbook.Worksheets("Crop").Cells(16, CropColNo)

    'PLAPOW = PLAPOW * (b_pla_den * PDEN + a_pla_den)
    MSNN = 1:      PLA2 = 0:     PLA1 = 0:    LAI = 0:
    R5ANTLAI = 0:  MXLAI = 0:    WSFL = 1:    iniLAI = 1
 End If

'------------------------------- Yesterday LAI to intercept PAR today
 LAI = LAI + GLAI - DLAI
 If CBD > bdBSG And CBD < bdTSG And LAI < 0.05 Then
    CBD = bdTSG
    MAT = 1                 ' GW & HM Correction 17/07/2018
    MATYP = 2  'pre-mature due to low LAI
 End If
 If LAI < 0 Then LAI = 0
 If LAI > MXLAI Then MXLAI = LAI   'Saving maximum LAI
   
'------------------------------- Daily increase and decrease in LAI today
 If CBD <= bdBLG Then
    GLAI = 0
 ElseIf CBD > bdBLG And CBD <= bdTLM Then
    INODE = DTU / PHYL
    MSNN = MSNN + INODE
    PLA2 = PLACON * MSNN ^ PLAPOW
    GLAI = ((PLA2 - PLA1) * PDEN / 10000) * WSFL
    PLA1 = PLA2
    DLAI = 0
 ElseIf CBD > bdTLM And CBD <= bdTLP Then
    GLAI = GLF * SLA

 ElseIf CBD > bdTLP Then
    GLAI = 0
 End If
 
 If CBD < bdBLS Then
    DLAI = 0
    BLSLAI = LAI             'Saving LAI at BLS
 ElseIf CBD >= bdBLS Then
    DLAI = bd / (bdMAT - bdBLS) * BLSLAI
 End If
 
If crpName = "WHEAT" And CBD <= bdANT Then
   R5ANTLAI = LAI     'for wheat
ElseIf crpName = "MAIZE" And CBD <= bdSIL Then
   R5ANTLAI = LAI     'for maize
ElseIf crpName <> "WHEAT" And crpName <> "MAIZE" And CBD <= bdR5 Then
   R5ANTLAI = LAI     'for legumes
End If
 
'------------------------------- Frost & Heat
  DLAIF = 0
  If CBD > bdBLG And TMIN < FRZTKIL Then
     frstf = Abs(TMIN - FRZTKIL) * FRZLDR
     If frstf < 0 Then frstf = 0
     If frstf > 1 Then frstf = 1
     DLAIF = LAI * frstf
  End If
  If DLAI < DLAIF Then DLAI = DLAIF
  
  DLAIH = 0
  If CBD > bdBLG And TMAX > HtLTH Then
     'heatf = 4 - (1 - (TMAX - 34) / 2)              'Asseng-APSIM
     heatf = 1 + (TMAX - HtLTH) * HtLDR              'Semenov-Sirius
     If heatf < 1 Then heatf = 1
     DLAIH = DLAI * heatf
  End If

  If DLAI < DLAIF Then DLAI = DLAIF
  If DLAI < DLAIH Then DLAI = DLAIH
Return





CropLAIN:   '////////////////////////////////// CROP LAI WITH N
'------------------------------- LAI initials and pars
If iniLAI = 0 Then
   PHYL = ThisWorkbook.Worksheets("Crop").Cells(7, CropColNo)
   PLACON = ThisWorkbook.Worksheets("Crop").Cells(8, CropColNo)
   PLAPOW = ThisWorkbook.Worksheets("Crop").Cells(9, CropColNo)
   'a_pla_den = ThisWorkbook.Worksheets("Crop").Cells(10, CropColNo)   'a=1 if there is no estimate
   'b_pla_den = ThisWorkbook.Worksheets("Crop").Cells(11, CropColNo)   'b=0 if there is no estimate
   SLA = ThisWorkbook.Worksheets("Crop").Cells(12, CropColNo)
   FRZTKIL = ThisWorkbook.Worksheets("Crop").Cells(13, CropColNo)
   FRZLDR = ThisWorkbook.Worksheets("Crop").Cells(14, CropColNo)
   HtLTH = ThisWorkbook.Worksheets("Crop").Cells(15, CropColNo)
   HtLDR = ThisWorkbook.Worksheets("Crop").Cells(16, CropColNo)

   'PLAPOW = PLAPOW * (b_pla_den * PDEN + a_pla_den)
   MSNN = 1:      PLA2 = 0:     PLA1 = 0:    LAI = 0:
   R5ANTLAI = 0:  MXLAI = 0:    WSFL = 1:    SLNG = 2:
   iniLAI = 1
End If

'------------------------------- Yesterday LAI to intercept PAR today
If GLAI > (INLF / SLNG) Then GLAI = (INLF / SLNG)
LAI = LAI + GLAI - DLAI
If CBD > bdBSG And CBD < bdTSG And LAI < 0.05 Then
   CBD = bdTSG
   'MAT = 1          ' GW & HM Correction 17/07/2018
   MATYP = 2        'pre-mature due to low LAI
End If
If LAI < 0 Then LAI = 0
If LAI > MXLAI Then MXLAI = LAI   'Saving maximum LAI
  
'------------------------------- Daily increase and decrease in LAI today
If CBD <= bdBLG Then
   GLAI = 0:
ElseIf CBD > bdBLG And CBD <= bdTLM Then
   INODE = DTU / PHYL
   MSNN = MSNN + INODE
   PLA2 = PLACON * MSNN ^ PLAPOW
   GLAI = ((PLA2 - PLA1) * PDEN / 10000) * WSFL
   PLA1 = PLA2
ElseIf CBD > bdTLM And CBD <= bdTLP Then
   GLAI = GLF * SLA
   BLSLAI = LAI             'Saving LAI at TLP
ElseIf CBD > bdTLP Then
   GLAI = 0
End If
DLAI = XNLF / (SLNG - SLNS)

If crpName = "WHEAT" And CBD <= bdANT Then
   R5ANTLAI = LAI     'for wheat
ElseIf crpName = "MAIZE" And CBD <= bdSIL Then
   R5ANTLAI = LAI     'for wheat
ElseIf crpName <> "WHEAT" And crpName <> "MAIZE" And CBD <= bdR5 Then
   R5ANTLAI = LAI     'for legumes
End If
  
'------------------------------- Frost & Heat effect
DLAIF = 0
If CBD > bdBLG And TMIN < FRZTKIL Then
   frstf = Abs(TMIN - FRZTKIL) * FRZLDR
   If frstf < 0 Then frstf = 0
   If frstf > 1 Then frstf = 1
   DLAIF = LAI * frstf
End If
If DLAI < DLAIF Then DLAI = DLAIF

DLAIH = 0
If CBD > bdBLG And TMAX > HtLTH Then
   'heatf = 4 - (1 - (TMAX - 34) / 2)              'Asseng-APSIM
   heatf = 1 + (TMAX - HtLTH) * HtLDR              'Semenov-Sirius
   If heatf < 1 Then heatf = 1
   DLAIH = DLAI * heatf
End If

If DLAI < DLAIF Then DLAI = DLAIF
If DLAI < DLAIH Then DLAI = DLAIH
Return





DMProduction:   '////////////////////////////////// DM PRODUCTION
'------------------------------- Parameters and Initials
If iniDMP = 0 Then
   TBRUE = ThisWorkbook.Worksheets("Crop").Cells(18, CropColNo)
   TP1RUE = ThisWorkbook.Worksheets("Crop").Cells(19, CropColNo)
   TP2RUE = ThisWorkbook.Worksheets("Crop").Cells(20, CropColNo)
   TCRUE = ThisWorkbook.Worksheets("Crop").Cells(21, CropColNo)
   KPAR = ThisWorkbook.Worksheets("Crop").Cells(22, CropColNo)
   IRUE1 = ThisWorkbook.Worksheets("Crop").Cells(23, CropColNo)
   IRUE2 = ThisWorkbook.Worksheets("Crop").Cells(24, CropColNo)

   TEC = ThisWorkbook.Worksheets("Crop").Cells(48, CropColNo)
   
   'IRUE = IRUE '
      
   WSFG = 1:    iniDMP = 1
End If
  
'------------------------------- Adjustment of RUE
If TMP <= TBRUE Or TMP >= TCRUE Then
   TCFRUE = 0
ElseIf TMP > TBRUE And TMP < TP1RUE Then
   TCFRUE = (TMP - TBRUE) / (TP1RUE - TBRUE)
ElseIf TMP > TP2RUE And TMP < TCRUE Then
   TCFRUE = (TCRUE - TMP) / (TCRUE - TP2RUE)
ElseIf TMP >= TP1RUE And TMP <= TP2RUE Then
   TCFRUE = 1
End If

'RUE = IRUE * TCFRUE * WSFG 'icicici
If bd < bdBSG Then RUE = IRUE1 * TCFRUE * WSFG Else RUE = IRUE1 * TCFRUE * WSFG



If CBD < bdEMR Or CBD > bdTSG Then RUE = 0

'------------------------------ CODES FOR RESPONSE TO VPD
vpdtp = ThisWorkbook.Worksheets("Crop").Cells(55, CropColNo) 'if vpdtp=1 --> daily calculation, no breakpoint in the response of T to VPD
VPDcr = ThisWorkbook.Worksheets("Crop").Cells(56, CropColNo) 'if vpdt=2 --> hourly calculation. vpdcr is the breakpoint for the response of T to vpd

If vpdtp = 2 Then
'__________________________ Hourly calcs ___________________________
  Pi = 3.141592654:      RDN = Pi / 180
  DEC = Sin(23.45 * RDN) * Cos(2 * Pi * (DOY + 10) / 365)
  DEC = Atn(DEC / Sqr(1 - DEC ^ 2)) * -1
  DECL = DEC * 57.29578
  SINLD = Sin(RDN * LAT) * Sin(DEC)
  COSLD = Cos(RDN * LAT) * Cos(DEC)
  AOB = SINLD / COSLD
  AOB2 = Atn(AOB / Sqr(1 - AOB ^ 2))
  DAYL = 12 * (1 + 2 * AOB2 / Pi)
  DSINB = 3600 * (DAYL * SINLD + 24 * COSLD * Sqr(1 - AOB * AOB) / Pi)
  DSINBE = 3600 * (DAYL * (SINLD + 0.4 * (SINLD * SINLD + COSLD * COSLD * 0.5)) + 12 * COSLD * (2 + 3 * 0.4 * SINLD) * Sqr(1 - AOB * AOB) / Pi)
  SC = 1370 * (1 + 0.033 * Cos(2 * Pi * DOY / 365))
  DSO = SC * DSINB
  Bnoon = 90 - (LAT - DECL)
  DTR = SRAD * 1000000     'from MJ m-2 d-1 to J m-2 d-1
  TC = 4:                              P = 1.5
  SUNRIS = 12 - 0.5 * DAYL:            SUNSET = 12 + 0.5 * DAYL

  If vpdtp = 2 Then
    VPTMIN = 0.6108 * Exp(17.27 * TMIN / (237.3 + TMIN))
    TR = 0
  End If
  DDMP = 0:
  
  For H = 1 To 24
    If H > SUNRIS And H < SUNSET Then
       If H < SUNRIS Then
          TSUNST = TMIN + (TMAXB - TMIN) * Sin(Pi * (DAYL / (DAYL + 2 * P)))
          NIGHTL = 24 - DAYL
          TEMP1 = (TMIN - TSUNST * Exp(-NIGHTL / TC) + (TSUNST - TMIN) * Exp(-(H + 24 - SUNSET) / TC)) / (1 - Exp(-NIGHTL / TC))
       ElseIf H < 13.5 Then
          TEMP1 = TMIN + (TMAX - TMIN) * Sin(Pi * (H - SUNRIS) / (DAYL + 2 * P))
       ElseIf H < SUNSET Then
          TEMP1 = TMINA + (TMAX - TMINA) * Sin(Pi * (H - SUNRIS) / (DAYL + 2 * P))
       Else
          TSUNST = TMINA + (TMAX - TMINA) * Sin(Pi * (DAYL / (DAYL + 2 * P)))
          NIGHTL = 24 - DAYL
          TEMP1 = (TMINA - TSUNST * Exp(-NIGHTL / TC) + (TSUNST - TMINA) * Exp(-(H - SUNSET) / TC)) / (1 - Exp(-NIGHTL / TC))
       End If
 
       SINB = SINLD + COSLD * Cos(2 * Pi * (H + 12) / 24)
       If SINB < 0 Then SINB = 0
       BET = Atn(SINB / Sqr(1 - SINB ^ 2))             'BETA IN RADIAN
       BETA = BET * 57.29578                           'BETA IN DEGREE
       SRAD1 = DTR * SINB * (1 + 0.4 * SINB) / DSINBE  'J m-2 s-1
       SRAD1 = SRAD1 * 3600 / 1000000                  'to J m-2 h-1; then MJ m-2 h-1
       If SRAD1 < 0 Then SRAD1 = 0

       FINT = 1 - Exp(-KPAR * LAI)
       DDMP1 = SRAD1 * 0.48 * FINT * RUE

 If vpdtp = 2 Then
       VPTEMP1 = 0.6108 * Exp(17.27 * TEMP1 / (237.3 + TEMP1))
       VPD1 = (VPTEMP1 - VPTMIN) * (VPDF / 0.75)        '(VPDF/0.75) is a correction factor when VPDF is different from 0.75
       TR1 = DDMP1 * VPD1 / TEC   'VPD in kPa, TEC in Pa

       If VPD1 > VPDcr Then                            'correction of TR and DBP for VDP>VPDcr
          TR1 = DDMP1 * VPDcr / TEC
          DDMP1 = TR1 * TEC / VPDcr
          TR1 = DDMP1 * VPDcr / TEC
          DDMP1 = TR1 * TEC / VPDcr
       End If
       TR = TR + TR1
       
  End If
       DDMP = DDMP + DDMP1
    End If
  Next H
ElseIf vpdtp = 1 Then
'_________________________daily______________
  FINT = 1 - Exp(-KPAR * LAI)
  DDMP = SRAD * 0.48 * FINT * RUE
End If
  
  If CBD < bdEM Or CBD > bdTSG Then DDMP = 0
Return






DMDistribution: '////////////////////////////////// DM DISTRIBUTION
'------------------------------- Parameters and Initials
If iniDMD = 0 Then
   FLF1A = ThisWorkbook.Worksheets("Crop").Cells(26, CropColNo)
   FLF1B = ThisWorkbook.Worksheets("Crop").Cells(27, CropColNo)
   WTOPL = ThisWorkbook.Worksheets("Crop").Cells(28, CropColNo)
   FLF2 = ThisWorkbook.Worksheets("Crop").Cells(29, CropColNo)
   FRTRL = ThisWorkbook.Worksheets("Crop").Cells(30, CropColNo)
   GCC = ThisWorkbook.Worksheets("Crop").Cells(31, CropColNo)
   PDHI = ThisWorkbook.Worksheets("Crop").Cells(32, CropColNo)
   WDHI1 = ThisWorkbook.Worksheets("Crop").Cells(33, CropColNo)
   WDHI2 = ThisWorkbook.Worksheets("Crop").Cells(34, CropColNo)
   WDHI3 = ThisWorkbook.Worksheets("Crop").Cells(35, CropColNo)
   WDHI4 = ThisWorkbook.Worksheets("Crop").Cells(36, CropColNo)
  
   WLF = 0.5:     WST = 0.5:      WVEG = WLF + WST:
   R5ANTDM = 0:   WGRN = 0:       FrHtDHI = 1
   iniDMD = 1:
End If

'------------------------------- Seed dry matter growth
If CBD <= bdBSG Then
    TRANSL = 0:  SGR = 0:
    BSGDM = WTOP                'Saving WTOP at BSG
    If BSGDM <= WDHI1 Or BSGDM >= WDHI4 Then
       DHIDMF = 0
    ElseIf BSGDM > WDHI1 And BSGDM < WDHI2 Then
       DHIDMF = (BSGDM - WDHI1) / (WDHI2 - WDHI1)
    ElseIf BSGDM > WDHI3 And BSGDM < WDHI4 Then
       DHIDMF = (WDHI4 - BSGDM) / (WDHI4 - WDHI3)
    ElseIf BSGDM >= WDHI2 And BSGDM <= WDHI3 Then
       DHIDMF = 1
    End If
    DHI = PDHI * DHIDMF
    TRLDM = BSGDM * FRTRL

ElseIf CBD > bdBSG And CBD <= bdTSG Then
    SGR = (DHI * FrHtDHI) * (WTOP + DDMP) + DDMP * HI
    If LAI = 0 And NST <= (WST * SNCS) Then SGR = 0   'There is no N for seed filling
    If SGR < 0 Then SGR = 0
    
    If (SGR / GCC) > DDMP Then
       TRANSL = (SGR / GCC) - DDMP
       If TRANSL > TRLDM Then TRANSL = TRLDM
    ElseIf (SGR / GCC) <= DDMP Then
       TRANSL = 0
    End If
    
    TRLDM = TRLDM - TRANSL
    If SGR > (DDMP + TRANSL) * GCC Then SGR = (DDMP + TRANSL) * GCC

ElseIf CBD > bdTSG Then
    TRANSL = 0: SGR = 0:
End If
  
'------------------------------- DM avail. for Leaf & stem
  DDMP2 = DDMP - SGR * GCC
  If DDMP2 < 0 Then DDMP2 = 0

'------------------------------- Leaf dry matter growth
  If CBD <= bdBLG Or CBD > bdTLP Then
     GLF = 0:
  ElseIf CBD > bdBLG And CBD <= bdTLM Then
     If WTOP < WTOPL Then FLF1 = FLF1A Else FLF1 = FLF1B
     GLF = FLF1 * DDMP2
  ElseIf CBD > bdTLM And CBD <= bdTLP Then
     GLF = FLF2 * DDMP2
  End If
       
'------------------------------- Stem dry matter growth
  GST = DDMP2 - GLF

'------------------------------- Organs accumulated mass
  WLF = WLF + GLF
  WST = WST + GST
  WGRN = WGRN + SGR
  WVEG = WVEG + DDMP - (SGR / GCC)
  WTOP = WVEG + WGRN
  HI = WGRN / WTOP
  
If crpName = "WHEAT" And CBD <= bdANT Then
   R5ANTDM = WTOP     'for wheat
ElseIf crpName = "MAIZE" And CBD <= bdSIL Then
   R5ANTDM = WTOP     'for wheat
ElseIf crpName <> "WHEAT" And crpName <> "MAIZE" And CBD <= bdR5 Then
   R5ANTDM = WTOP     'for legumes
End If
Return





SoilWater:  '////////////////////////////////// SOIL WATER
'------------------------------- Parameters and Initials
If iniSW = 0 Then
    DEPORT = ThisWorkbook.Worksheets("Crop").Cells(45, CropColNo)     'mm
    MEED = ThisWorkbook.Worksheets("Crop").Cells(46, CropColNo)       'mm
    GRTDP = ThisWorkbook.Worksheets("Crop").Cells(47, CropColNo)      'mm/bd
    TEC = ThisWorkbook.Worksheets("Crop").Cells(48, CropColNo)
    WSSG = ThisWorkbook.Worksheets("Crop").Cells(50, CropColNo)
    WSSL = ThisWorkbook.Worksheets("Crop").Cells(51, CropColNo)
    WSSD = ThisWorkbook.Worksheets("Crop").Cells(52, CropColNo)
    WSSN = ThisWorkbook.Worksheets("Crop").Cells(53, CropColNo)
    FLDKIL = ThisWorkbook.Worksheets("Crop").Cells(54, CropColNo)
    vpdtp = ThisWorkbook.Worksheets("Crop").Cells(55, CropColNo)
    VPDcr = ThisWorkbook.Worksheets("Crop").Cells(56, CropColNo)
    'surviv = ThisWorkbook.Worksheets("Crop").Cells(57, CropColNo)
    EPCOND = ThisWorkbook.Worksheets("Crop").Cells(58, CropColNo)
    LTLRWC = ThisWorkbook.Worksheets("Crop").Cells(59, CropColNo)
    LAI = 0 'HM 04/07/2018
    BLSLAI = 0 'HM 04/07/2018
    
    bdBRG = 0
       
    NLYER = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 4, 1)
    LDRAIN = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 4, 2)
    SALB = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 4, 3)
    CN = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 4, 5)
    U = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 4, 4)
    semethod = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 4, 6)
    
    
    EOSMIN = 1.5:    WETWAT = 10:     KET = 0.5:       CALB = 0.23:
    
    DYSE = 1:        CTR = 0:         CE = 0:          CRAIN = 0:
    CRUNOF = 0:      CIRGW = 0:       IRGNO = 0:       CDRAIN = 0:
    DPTOP = 0:       RTLN = 1:        WSOL = 0:        WDUL = 0:
    WSAT = 0:        SOLDEP = 0:      ISOLWAT = 0:     AROOT = 1:
    FLDUR = 0:

    For L = 1 To NLYER
        DLYER(L) = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 2)      'mm
        SAT = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 3)           'mm/mm
        DUL = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 4)           'mm/mm
        CLL = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 5)           'mm/mm
        EXTR = DUL - CLL                          'mm/mm
       'ADRY = CLL / 3                            'mm/mm
        ADRY = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 6)          'mm/mm
        WL(L) = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 7)         '0=cll, 1=dul
        WL(L) = WL(L) * DLYER(L)
        DRAINF(L) = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 8)
    
        WLAD(L) = ADRY * DLYER(L)
        WLLL(L) = CLL * DLYER(L)
        WLUL(L) = DUL * DLYER(L)
        WLST(L) = SAT * DLYER(L)
        ATSW(L) = WL(L) - WLLL(L)
        TTSW(L) = WLUL(L) - WLLL(L)
        FTSW(L) = ATSW(L) / TTSW(L)
        
        iATSW(L) = ATSW(L)
        iFTSW(L) = FTSW(L)
        
        WSOL = WSOL + WL(L)
        WDUL = WDUL + WLUL(L)
        WSAT = WSAT + WLST(L)
     
        ISOLWAT = ISOLWAT + ATSW(L)
        SOLDEP = SOLDEP + DLYER(L)
    Next L
    
    
    SE2C = 3.5:    SE1MX = U:    DSR = 1:     SSE1 = U:      SSE = U + SE2C: '<--- Inputs for Ritchie 2-Stage soil evap.

    iniSW = 1
End If

'------------------------------- Irrigation based on DAP, CBD or DOY
If water = 1 Then
    If FTSWRZ <= IRGLVL And CBD < bdTSG Then
       IRGW = TTSWRZ - ATSWRZ    'GW & HM 08/08/2018 Irigation = 30mm (we don't wan't the soil to be completely filled when irrigated
       IRGNO = IRGNO + 1
    Else
       IRGW = 0
    End If

ElseIf water = 2 Then
    'nothing : rainfed
ElseIf water = 3 Then
    IRGW = 0:
    If WATiT = 1 Then             'Irrig. appl. based on DAP
       For N = 1 To IRNUM
           If DAP = DAPIR(N) Then
              IRGW = IRGWI(N)             'mm
              IRGNO = IRGNO + 1
           End If
       Next N
    ElseIf WATiT = 2 Then         'Irrgig. appl. based on CBD with user-specified water
       For N = 1 To IRNUM
           If CBD >= DAPIR(N) And WAT01(N) = 0 Then
              IRGW = IRGWI(N)             'mm
              IRGNO = IRGNO + 1
              WAT01(N) = 1
           End If
       Next N
    ElseIf WATiT = 3 Then         'Irrgig. appl. based on DOY
       For N = 1 To IRNUM
           If DOY = DAPIR(N) Then
              IRGW = IRGWI(N)             'mm
              IRGNO = IRGNO + 1
           End If
       Next N
     End If
     
 ElseIf water = 4 Then  ' replenish root zone on fixed dates
    IRGW = 0:
    If WATiT = 1 Then             'Irrig. appl. based on DAP
       For N = 1 To IRNUM
           If DAP = DAPIR(N) Then
              IRGW = (TTSWRZ - ATSWRZ)
              If IRGW < 0 Then IRGW = 0 'mm
              IRGNO = IRGNO + 1
           End If
       Next N
    ElseIf WATiT = 2 Then         'Irrgig. appl. based on CBD with user-specified water
       For N = 1 To IRNUM
           If CBD >= DAPIR(N) And WAT01(N) = 0 Then
              IRGW = (TTSWRZ - ATSWRZ)
              If IRGW < 0 Then IRGW = 0 'mm              'mm
              IRGNO = IRGNO + 1
              WAT01(N) = 1
           End If
       Next N
    ElseIf WATiT = 3 Then         'Irrgig. appl. based on DOY
       For N = 1 To IRNUM
           If DOY = DAPIR(N) Then
              IRGW = (TTSWRZ - ATSWRZ)
              If IRGW < 0 Then IRGW = 0 'mm      'mm
              IRGNO = IRGNO + 1
           End If
       Next N
     End If
     
End If

CIRGW = CIRGW + IRGW
    
'------------------------------- Drainage
DRAIN = FLOUT(LDRAIN)
CDRAIN = CDRAIN + DRAIN

'------------------------------- Root depth increase
GRTD = GRTDP * bd
If CBD < bdBRG Then GRTD = 0
If CBD > bdTRG Then GRTD = 0
If DDMP = 0 Then GRTD = 0
If DEPORT >= SOLDEP Then GRTD = 0
If DEPORT >= MEED Then GRTD = 0
If ATSW(RTLN) = 0 Then GRTD = 0
DEPORT = DEPORT + GRTD

DPTOP = 0:    RTLN = 0
For L = 1 To NLYER            'Loop to find if there is roots in each layer
    RLYER(L) = DEPORT - DPTOP
    If RLYER(L) > DLYER(L) Then RLYER(L) = DLYER(L)
    If RLYER(L) < 0 Then RLYER(L) = 0
    If RLYER(L) > 0 Then RTLN = L
    DPTOP = DPTOP + DLYER(L)
Next L

'------------------------------- Runoff
RUNOF = 0
If RAIN > 0.01 Then   'water = 2 And        'HM 18/07/2018 No reason not to have runoff in irrgated simulations
    'If DLYER(1) >= 250 Then                'HM 22/06/2018 NO INFILTRARTION FOR SHALLOW SOILS
        'from the first layer only

        S = 254 * (100 / CN - 1)
        SWER = 0.15 * ((WLST(1) - WL(1)) / (WLST(1) - WLLL(1)))
        If SWER < 0 Then SWER = 0
        If (RAIN - SWER * S) > 0 Then
           RUNOF = (RAIN - SWER * S) ^ 2 / (RAIN + (1 - SWER) * S)
        Else
           RUNOF = 0
        End If
    'ElseIf DLYER(1) < 250 Then
        'from layer 1+2
        'S = 254 * (100 / CN - 1)
        'SWER = 0.15 * (((WLST(1) - WL(1)) / (WLST(1) - WLLL(1))) * 0.5 + ((WLST(2) - WL(2)) / (WLST(2) - WLLL(2))) * 0.5)
        'If SWER < 0 Then SWER = 0
        'If (RAIN - SWER * S) > 0 Then
           'RUNOF = (RAIN - SWER * S) ^ 2 / (RAIN + (1 - SWER) * S)
        'Else
           'RUNOF = 0
        'End If
    'End If
End If

CRAIN = CRAIN + RAIN
CRUNOF = CRUNOF + RUNOF

'------------------------------- LAI for soil evaporation
If CBD <= bdBSG Then ETLAI = LAI Else ETLAI = BLSLAI

'------------------------------- Potential ET
TD = 0.6 * TMAX + 0.4 * TMIN
ALBEDO = CALB * (1 - Exp(-KET * ETLAI)) + SALB * Exp(-KET * ETLAI)
EEQ = SRAD * (0.004876 - 0.004374 * ALBEDO) * (TD + 29)
PET = EEQ * 1.1
If TMAX > 34 Then PET = EEQ * ((TMAX - 34) * 0.05 + 1.1)
If TMAX < 5 Then PET = EEQ * 0.01 * Exp(0.18 * (TMAX + 20))
  
'------------------------------- Soil evaporation
EOS = PET * Exp(-KET * ETLAI)
If PET > EOSMIN And EOS < EOSMIN Then EOS = EOSMIN
If STBLW > 0 Then
EOS = EOS * (1.5 - 0.2 * Application.WorksheetFunction.Ln(100 * STBLW))      'Mulch effect HM 31/10/2017        'GW & HM Correction 09/08/2018 logarithme neperien ‡ la place de Log10 : Log(100 * STBLW))
End If
SEVP = EOS
If (RAIN + IRGW) > WETWAT Then DYSE = 1
If DYSE > 1 Or FTSWRZ < 0.5 Or ATSW(1) <= 1 Then
   SEVP = EOS * ((DYSE + 1) ^ 0.5 - DYSE ^ 0.5)
   DYSE = DYSE + 1
End If

'------------------------------------------ A modified Ritchie 2-stage soil evp
If semethod = 2 Then
  If ATSW(1) <= WLAD(1) Then
     SEVP = 0
  Else
     If SSE1 < SE1MX Then
       'Stage I evaporation
        SEVP = EOS
        If SEVP > (SE1MX - SSE1) Then SEVP = SE1MX - SSE1
        SSE1 = SSE1 + SEVP
        SSE = SSE + SEVP
        If SSE1 >= SE1MX Then
          'Transition from Stage I to Stage II
           SEVP = SEVP + SE2C * (DSR ^ 0.5 - (DSR - 1) ^ 0.5) * (1 - (SEVP / EOS))
           DSR = DSR + 1 - SEVP / EOS
        End If
     Else
       'Stage II evaporation
        SEVP = SE2C * (DSR ^ 0.5 - (DSR - 1) ^ 0.5)
        If SEVP > EOS Then SEVP = EOS
        DSR = DSR + 1
        SSE = SSE + SEVP
     End If

     FLUX1 = RAIN + IRGW - RUNOF
     If FLUX1 >= SSE1 Then
        SSE = SSE - FLUX1
        If SSE < 0 Then SSE = 0
        SSE1 = 0
        DSR = 1 + (SSE / SE2C) ^ 0.5
     Else
        SSE = SSE - FLUX1
        SSE1 = SSE1 - FLUX1
        DSR = 1 + (SSE / SE2C) ^ 0.5
     End If
  End If
End If
'---------------------------------------------------------------------------------


CE = CE + SEVP

'------------------------------- Plant transpiration
If vpdtp = 1 Then
     VPTMIN = 0.6108 * Exp(17.27 * TMIN / (TMIN + 237.3))
     VPTMAX = 0.6108 * Exp(17.27 * TMAX / (TMAX + 237.3))
     VPD = VPDF * (VPTMAX - VPTMIN)
     TR = DDMP * VPD / TEC         'VPD in kPa, TEC in Pa
ElseIf vpdtp = 2 Then
     'Nothing
End If

'>>>>>>>>>>>>>> Survival calc >>>>>>>>>>>>>>>>>>>>>>>>>         <--------------------*** 'GW Ajout Survival subodel
'EPCOND = 0.3
'LTLRWC = 0.55

bdDKill = bdANT

TRSUR = LAI * EPCOND * VPD * DAYL * 3600 * 0.0022 / (TMP + 273)
'If CBD > bdDKill And TRSUR > TR Then
If (TRSUR > TR) And (FTSWRZ < 0.05) Then
     TR = TRSUR
     LAI = LAI - TR / (9 * (1 - LTLRWC)) * SLA * 1000#
End If
'<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

CTR = CTR + TR

'------------------------------- Updating
  If AROOT = 0 Then WUUR = 0 Else WUUR = TR / AROOT    'addition when using SDOY
  TSE = SEVP:
For L = 1 To NLYER
    WU(L) = RLYER(L) * RT(L) * WUUR

    SE(L) = TSE
    If SE(L) > ((WL(L) - WLAD(L)) * DRAINF(L)) Then SE(L) = ((WL(L) - WLAD(L)) * DRAINF(L))
    If WL(L) <= WLAD(L) Then SE(L) = 0
    TSE = TSE - SE(L)
    If TSE < 0 Then TSE = 0
Next L

WRZ = 0:      WRZUL = 0:     WRZST = 0:      ATSWRZ = 0:    SWL = 0:
TTSWRZ = 0:   WSOL = 0:      ATSWSL = 0:     TTSWSL = 0:    AROOT = 0:
For L = 1 To NLYER
    If L = 1 Then
       FLIN(L) = RAIN + IRGW - RUNOF          'today recharge
    Else
       FLIN(L) = FLOUT(L - 1)
    End If
    If FLIN(L) < 0 Then FLIN(L) = 0

    WL(L) = WL(L) + FLIN(L) - WU(L) - SE(L)
    FLOUT(L) = (WL(L) - WLUL(L)) * DRAINF(L)
    If FLOUT(L) < 0 Then FLOUT(L) = 0
    WL(L) = WL(L) - FLOUT(L)

    ATSW(L) = WL(L) - WLLL(L)
    If ATSW(L) < 0 Then ATSW(L) = 0
    TTSW(L) = WLUL(L) - WLLL(L)
    FTSW(L) = ATSW(L) / TTSW(L)
    If FTSW(L) > WSSG Then RT(L) = 1 Else RT(L) = FTSW(L) / WSSG
    If RT(L) < 0 Then RT(L) = 0
    
    AROOT = AROOT + RLYER(L) * RT(L)
    WRZ = WRZ + WL(L) * (RLYER(L) / DLYER(L))
    WRZUL = WRZUL + WLUL(L) * (RLYER(L) / DLYER(L))
    WRZST = WRZST + WLST(L) * (RLYER(L) / DLYER(L))
    ATSWRZ = ATSWRZ + ATSW(L) * (RLYER(L) / DLYER(L))
    TTSWRZ = TTSWRZ + TTSW(L) * (RLYER(L) / DLYER(L))
    FTSWRZ = ATSWRZ / TTSWRZ
  
    WSOL = WSOL + WL(L)
    ATSWSL = ATSWSL + ATSW(L)
    TTSWSL = TTSWSL + TTSW(L)
    FTSWSL = ATSWSL / TTSWSL
    
    SWL = SWL + WL(L)
Next L



'------------------------------- Water-stress-effect factors
If FTSWRZ > WSSL Then WSFL = 1 Else WSFL = FTSWRZ / WSSL
If FTSWRZ > WSSG Then WSFG = 1 Else WSFG = FTSWRZ / WSSG
If UCase(crpName) <> "MAIZE" And UCase(crpName) <> "WHEAT" Then
    If FTSWRZ > WSSN Then WSFN = 1 Else WSFN = FTSWRZ / WSSN
End If
WSFD = (1 - WSFG) * WSSD + 1
If WRZ <= WRZUL Then WSXF = 1 Else WSXF = ((WRZST - WRZ) / (WRZST - WRZUL))
If WSXF < 0 Then WSXF = 0
'If FTSWRZ > 1 Then
    'icicicici
 '    WSFG = WSXF:      WSFN = WSXF: WSFL = WSXF: 'WSFD = WSXF:
'End If
'If WRZ > (WRZST - WRZUL) / 4 + WRZUL Then WSFD = 0

If WL(1) <= WLUL(1) Then WSXF = 1 Else WSXF = ((WLST(1) - WL(1)) / (WLST(1) - WLUL(1)))
If WSXF < 0 Then WSXF = 0
If FTSW(1) > 1 Then WSFD = WSXF * WSXF * WSXF: WSFL = WSXF: WSFG = WSXF: WSFN = WSXF
'If FTSW(1) > 1 Then WSFD = WSSD * WSSD: WSFL = WSXF: WSFG = WSXF: WSFN = WSXF
'If WRZ >= WRZST * 0.8 Then WSFD = 0
If WL(1) >= WLST(1) Then WSFD = 0
'If FTSW(1) > 1 Then WSFD = 0.1

If WSXF <= 0.02 Then
     FLDUR = (FLDUR + 1)
Else
    FLDUR = 0
End If
If FLDUR > FLDKIL Then CBD = bdTSG: MATYP = 5

Return





PlantN: '////////////////////////////////// PALNT N
  If iniPNB = 0 Then
     SLNG = ThisWorkbook.Worksheets("Crop").Cells(61, CropColNo)
     SLNS = ThisWorkbook.Worksheets("Crop").Cells(62, CropColNo)
     SNCG = ThisWorkbook.Worksheets("Crop").Cells(63, CropColNo)
     SNCS = ThisWorkbook.Worksheets("Crop").Cells(64, CropColNo)
     GNCmin = ThisWorkbook.Worksheets("Crop").Cells(65, CropColNo)  'min GNC
     GNCmax = ThisWorkbook.Worksheets("Crop").Cells(66, CropColNo)  'max GNC
     MXNUP = ThisWorkbook.Worksheets("Crop").Cells(67, CropColNo)
     'bdBNF = ThisWorkbook.Worksheets("Crop").Cells(95, CropColNo)
     
     NST = WST * SNCG:     NLF = LAI * SLNG:   WSFN = 1:
     CNUP = NST + NLF:     NGRN = 0:           CUMBNF = 0:
     iniPNB = 1:
  End If
  
  If CBD <= bdEMR Or CBD > bdTSG Then
       NUP = 0:    XNLF = 0:   XNST = 0:   INLF = 0:   INST = 0:   INGRN = 0: NFC = 0: 'icicci initialisation NFC
  ElseIf CBD > bdEMR And CBD < bdBSG Then
       INGRN = 0
       NSTDF = (WST * SNCG) - NST
       If NSTDF < 0 Then NSTDF = 0
       NUP = (GST * SNCG) + (GLAI * SLNG) + NSTDF
       If NUP > MXNUP Then NUP = MXNUP
       NFC = NFC * 3 / 4 + NUP / WVEG * (1 / 4)   'from Sinclair et al. 2003       'legume
       NUP = NUP * WSFN                                                            'legume
       If NUP < 0 Then NUP = 0
       If FTSWRZ > 1 Then NUP = NUP * WSXF
       If DDMP = 0 Then NUP = 0
       If bdBNF >= bdBSG And NUP > SNAVL Then NUP = SNAVL      'for non-legume
       If CBD < bdBNF And NUP > SNAVL Then NUP = SNAVL         'legume
       If NUP > SNAVL Then BNF = NUP - SNAVL                   'legume
       If BNF < 0 Then BNF = 0
       CUMBNF = CUMBNF + BNF                                                       'legume

       If NST <= (WST * SNCS) Then
            INST = WST * SNCS - NST
            XNST = 0
            If INST >= NUP Then
                INLF = 0
                XNLF = INST - NUP
            ElseIf INST < NUP Then
                INLF = GLAI * SLNG
                If INLF > (NUP - INST) Then INLF = NUP - INST
                INST = NUP - INLF
                XNLF = 0
            End If
       ElseIf NST > (WST * SNCS) Then
            INLF = GLAI * SLNG
            XNLF = 0
            If INLF >= NUP Then
                INST = 0
                XNST = INLF - NUP
                If XNST > (NST - WST * SNCS) Then XNST = NST - WST * SNCS
                INLF = NUP + XNST
            ElseIf INLF < NUP Then
                INST = NUP - INLF
                XNST = 0
            End If
       End If
       TRLNBSG = LAI * (SLNG - SLNS) + (NST - WST * SNCS)
  
  ElseIf CBD >= bdBSG And CBD <= bdTSG Then
       INGRN = SGR * GNCmax                        'first estimate using maximum GNC
       NUP = INGRN + (GST * SNCG) + (GLAI * SLNG)
       nup_fr_grn = INGRN / (NUP + 1E-08)
       If NUP < 0 Then NUP = 0
       If NUP > MXNUP Then NUP = MXNUP

       If CBD < bdBNF Then
           DNF = 0                                  'before BNF activation OR for non-legum crops
       ElseIf CBD >= bdBNF Then
           PDNF = NFC * WVEG                        'legume
           If PDNF > NUP Then PDNF = NUP            'legume
           DNF = PDNF * WSFN                        'legume
           If DDMP <= (SGR * GCC) Then DNF = 0      'legume
           If DDMP = 0 Then DNF = 0                 'legume
       End If

       If FTSWRZ > 1 Then NUP = NUP * WSXF
       If DDMP < (SGR / GCC) Then NUP = 0
       If DDMP = 0 Then NUP = 0
       If bdBNF >= bdBSG And NUP > SNAVL Then NUP = SNAVL      'for non-legume
       If NUP > (SNAVL + DNF) Then NUP = (SNAVL + DNF)
       BNF = NUP - SNAVL                                       'legume
       If BNF < 0 Then BNF = 0
       CUMBNF = CUMBNF + BNF                                   'legume

       XNVEG = TRLNBSG / (bdTSG - bdBSG) * bd                  'An estimate of supply from veg
       If SGR = 0 Then XNVEG = 0
       INGRN = XNVEG + NUP * nup_fr_grn
       
       If INGRN < (SGR * GNCmin) Then
          INGRN = SGR * GNCmin
          XNVEG = INGRN - NUP
       ElseIf INGRN >= (SGR * GNCmax) Then
          INGRN = SGR * GNCmax
          XNVEG = INGRN - NUP
       End If
       
       If NUP > INGRN Then
           'N is excess of seed needs
            NUP2 = NUP - INGRN
            If NST <= (WST * SNCS) Then
               INST = WST * SNCS - NST
               XNST = 0
               If INST >= NUP2 Then
                   INLF = 0
                   XNLF = INST - NUP2
               ElseIf INST < NUP2 Then
                   INLF = GLAI * SLNG
                   If INLF > (NUP2 - INST) Then INLF = NUP2 - INST
                   INST = NUP2 - INLF
                   XNLF = 0
               End If
            ElseIf NST > (WST * SNCS) Then
               INLF = GLAI * SLNG
               XNLF = 0
               If INLF >= NUP2 Then
                   INST = 0
                   XNST = INLF - NUP2
                   If XNST > (NST - WST * SNCS) Then XNST = NST - WST * SNCS
                   INLF = NUP2 + XNST
               ElseIf INLF < NUP2 Then
                   INST = NUP2 - INLF
                   XNST = 0
               End If
            End If
           
       ElseIf NUP <= INGRN Then
           'Need to transfer N from vegetative tissue
            INLF = 0
            INST = 0
            XNLF = (INGRN - NUP) * FXLF
            XNST = (INGRN - NUP) * (1 - FXLF)
       End If
  End If
  
  NST = NST + INST - XNST
  NLF = NLF + INLF - XNLF
  NVEG = NLF + NST
  NGRN = NGRN + INGRN
  CNUP = CNUP + NUP
  
  TRLN = LAI * (SLNG - SLNS) + (NST - WST * SNCS)
  FXLF = LAI * (SLNG - SLNS) / (TRLN + 1E-12)
  If FXLF > 1 Then FXLF = 1
  If FXLF < 0 Then FXLF = 0
Return





SoilN:  '////////////////////////////////// SOIL N
'------------------------------- Parameters and initials
If iniSNB = 0 Then
   CNSOL = 0
   For L = 1 To NLYER
       FG = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 9)          'Fraction soil >2mm
       BDL = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 10)        'Soil bulk density (g.cm-3)
       NORGP = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 11)      'Organic N (%)
       FMIN = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 12)       'Fraction Org N avail. for mineralization
       NO3L = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 13)       'ppm = mg.kg-1
       NH4L = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 14)       'ppm = mg.kg-1
       
       SOILML = DLYER(L) * BDL * (1 - FG) * 1000    'g.m-2
       NORG = NORGP * 0.01 * SOILML                 'g.m-2
       MNORG(L) = NORG * FMIN                       'gN.m-2 org. N avail. for mineralization
       
       NO3L = NO3L * (14 / 62) * 1E-06 * SOILML     'from ppm to gN.m-2
       NH4L = NH4L * (14 / 18) * 1E-06 * SOILML     'from ppm to gn.m-2
       
       NSOL(L) = (NO3L + NH4L)                      'gN.m-2
       INSOL(L) = NSOL(L)
       
       NCON(L) = NSOL(L) / (WL(L) * 1000)           'gN.g-1 H2O
       CNSOL = CNSOL + NSOL(L)
   Next L

   ISOLN = CNSOL:  CNFERT = 0:   CNVOL = 0:
   CNLEACH = 0:    CNMIN = 0:    CNDNIT = 0:   iniSNB = 1
End If

'------------------------------- N net mineralization
TMPS = TMP
If TMPS > 35 Then TMPS = 35
SNMIN = 0
For L = 1 To NLYER
    KN = 24 * Exp(17.753 - 6350.5 / (TMPS + 273)) / 168
    If FTSW(L) < 0.9 Then RN = 1.111 * FTSW(L)
    If FTSW(L) >= 0.9 Then RN = 10 - 10 * FTSW(L)
    If RN < 0 Then RN = 0
    NMIN(L) = MNORG(L) * RN * (1 - Exp(-KN))
    NMIN(L) = NMIN(L) * (0.0002 - NCON(L)) / 0.0002     'threshold = 200 mgN.L-1
    If NMIN(L) < 0 Then NMIN(L) = 0
    MNORG(L) = MNORG(L) - NMIN(L)

    SNMIN = SNMIN + NMIN(L)
Next L
CNMIN = CNMIN + SNMIN

'------------------------------- N application & volatilization
NFERT = 0:   NVOL = 0
If NATiT = 1 Then             'N appl. based on DAP
   For N = 1 To FN
       If DAP = DAPNF(N) Then
          NFERT = NFERTI(N)             'gN.m-2
          VOLF = VOLFI(N) / 100
          NVOL = VOLF * NFERT           'gN.m-2
       End If
   Next N
ElseIf NATiT = 2 Then         'N appl. based on CBD
   For N = 1 To FN
       If CBD >= DAPNF(N) And NFA01(N) = 0 Then
          NFERT = NFERTI(N)             'gN.m-2
          VOLF = VOLFI(N) / 100
          NVOL = VOLF * NFERT           'gN.m-2
          NFA01(N) = 1
       End If
   Next N
ElseIf NATiT = 3 Then         'N appl. based on DOY

   For N = 1 To FN
       If DOY = DAPNF(N) Then
          NFERT = NFERTI(N)             'gN.m-2
          VOLF = VOLFI(N) / 100
          NVOL = VOLF * NFERT           'gN.m-2
       End If
   Next N
   
End If

CNFERT = CNFERT + NFERT
CNVOL = CNVOL + NVOL

'------------------------------- N downward movement
For L = 1 To NLYER
    NOUT(L) = NSOL(L) * (FLOUT(L) / (WL(L) + FLOUT(L)))    'gN.m-2
    If NCON(L) <= 1E-06 Then NOUT(L) = 0                   'threshold = 1 mgN.L-1
Next L

NLEACH = NOUT(NLYER)
CNLEACH = CNLEACH + NLEACH

'------------------------------- N denitification
SNDNIT = 0
For L = 1 To NLYER
    If L = 1 Or L = 2 Then           'Dnit from layers 1 & 2 unless layer 1 is thick enough
       If FTSW(L) > 1 Then
          XNCON = NCON(L)
          If XNCON > 0.0004 Then XNCON = 0.0004      'threshold = 400 mgN.L-1
          KDNIT = 6 * Exp(0.07735 * TMPS - 6.593)    'rate coef.
          NDNIT(L) = XNCON * (1 - Exp(-KDNIT))       'gN.g-1 H2O
          NDNIT(L) = NDNIT(L) * WL(L) * 1000         'gN.m-2
          'WFNDNIT = (WL(L) - WLUL(L)) / (WLST(L) - WLUL(L))
          'If WFNDNIT > 1 Then WFNDNIT = 1
          'NDNIT(L) = NDNIT(L) * WFNDNIT
          If DLYER(1) > 250 Then NDNIT(2) = 0        'for conditions that the first layer is thick
       Else
          NDNIT(L) = 0
       End If
    Else
       NDNIT(L) = 0
    End If
    SNDNIT = SNDNIT + NDNIT(L)
Next L
CNDNIT = CNDNIT + SNDNIT

'------------------------------ N removal by plant uptake
For L = 1 To NLYER
    NU(L) = (NAVL(L) / (SNAVL + 1E-06)) * (NUP - BNF)
Next L
   
'------------------------------ Updating
SNAVL = 0:    CNSOL = 0:
For L = 1 To NLYER
    If L = 1 Then
       NSOL(L) = NSOL(L) + NMIN(L) + NFERT - NVOL - NOUT(L) - NDNIT(L) - NU(L)
    Else
       NSOL(L) = NSOL(L) + NOUT(L - 1) + NMIN(L) - NOUT(L) - NDNIT(L) - NU(L)
    End If
    NCON(L) = NSOL(L) / (WL(L) * 1000)

    NAVL(L) = (NCON(L) - 1E-06) * ATSW(L) * 1000 * (RLYER(L) / DLYER(L))      'threshold = 1 mgN.L-1
    If NAVL(L) < 0 Then NAVL(L) = 0

    SNAVL = SNAVL + NAVL(L)
    CNSOL = CNSOL + NSOL(L)
Next L
Return





FindSimSowDate: '////////////////////////////////// FIND SIMULATION AND SOWING DATE
Do    '---- this finds start simulation date
    Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
    DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
    SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
    TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
    TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
    RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)
    wthRow = wthRow + 1

Loop Until Yr = Pyear And DOY = SimDoy


Do    '---- this finds sowing date (FixFind=0) or first date in the sowing window (for FixFin =1, 2 or 3)
    Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
    DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
    SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
    TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
    TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
    RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)
    TMIN = TMIN '+ tchng
    TMAX = TMAX '+ tchng
    RAIN = RAIN '* pchng
    TMP = (TMAX + TMIN) / 2
    wthRow = wthRow + 1
    CBD = 0
    DAP = 0
    If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
    If nitrogen = 2 And water > 0 Then GoSub SoilN

GoSub DailyPrintOut   ' to print out water and N balance before sowing

Loop Until DOY = Pdoy    'And Yr = Pyear <--- was removed 25 Jan 2017
    
If FixFind = 0 Then
    '------- Fixed sowing date
    '------- nothing to do
ElseIf FixFind = 1 Then
    '------- Sow in the 5th day of a 5-day rainfree period
     R1 = 99: R2 = 99: R3 = 99: R4 = 99: R5 = 99: RfreeP = 5:
     Nfixfind = 0: SUMRAIN = 99:  cumfind = 0
     Do
        Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
        DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
        SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
        TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
        TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
        RAIN = wb.Worksheets("Data").Cells(wthRow, 6)      '     wb.Worksheets("Sheet1").Cells(wthRow, 6)
        TMIN = TMIN '+ tchng
        TMAX = TMAX '+ tchng
        RAIN = RAIN '* pchng
        TMP = (TMAX + TMIN) / 2
        wthRow = wthRow + 1
    
        R5 = R4: R4 = R3: R3 = R2: R2 = R1: R1 = RAIN
        SUMRAIN = R1 + R2 + R3 + R4 + R5
        Nfixfind = Nfixfind + 1
        cumfind = cumfind + 1
    
        If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
        If nitrogen = 2 And water > 0 Then GoSub SoilN
        
        DAP = 0
        GoSub DailyPrintOut   ' to print out water and N balance before sowing

        
        If cumfind > SearchDur Then MAT = 1: MATYP = 3
        If MAT = 1 Then Exit Do
     Loop Until SUMRAIN = 0 And Nfixfind >= RfreeP
     Pdoy = DOY
     
ElseIf FixFind = 2 Then
    '-------- Sow in the 5th day of a 5-day rainfree period + temp> x oC
     T1 = 0:    T2 = 0:     T3 = 0:     T4 = 0:     T5 = 0
     R1 = 99:   R2 = 99:    R3 = 99:    R4 = 99:    R5 = 99:   RfreeP = 5:
     Nfixfind = 0:          SUMRAIN = 99:     cumfind = 0
     Do
        Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
        DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
        SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
        TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
        TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
        RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)

        TMIN = TMIN '+ tchng
        TMAX = TMAX '+ tchng
        RAIN = RAIN '* pchng
        TMP = (TMAX + TMIN) / 2
        wthRow = wthRow + 1
    
        T5 = T4: R5 = R4:  T4 = T3: R4 = R3:  T3 = T2: R3 = R2:  T2 = T1: R2 = R1
        T1 = TMP: R1 = RAIN
        MVMTMP = (T1 + T2 + T3 + T4 + T5) / 5
        SUMRAIN = R1 + R2 + R3 + R4 + R5
        Nfixfind = Nfixfind + 1
        cumfind = cumfind + 1
    
        If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
        If nitrogen = 2 And water > 0 Then GoSub SoilN
        
        DAP = 0
        GoSub DailyPrintOut   ' to print out water and N balance before sowing
       
        
        If cumfind > SearchDur Then MAT = 1: MATYP = 3
        If MAT = 1 Then Exit Do
     Loop Until MVMTMP > SowTmp And SUMRAIN = 0 And Nfixfind >= RfreeP
     Pdoy = DOY

ElseIf FixFind = 3 Then
    '-------- Sow in the 5th day of a 5-day rainfree period + temp < x oC
     T1 = 0:    T2 = 0:     T3 = 0:     T4 = 0:     T5 = 0
     R1 = 99:   R2 = 99:    R3 = 99:    R4 = 99:    R5 = 99:   RfreeP = 5:
     Nfixfind = 0:          SUMRAIN = 99:  cumfind = cumfind + 1
     Do
        Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
        DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
        SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
        TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
        TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
        RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)

        TMIN = TMIN '
        TMAX = TMAX '
        RAIN = RAIN '
        TMP = (TMAX + TMIN) / 2
        wthRow = wthRow + 1
    
        T5 = T4: R5 = R4:  T4 = T3: R4 = R3:  T3 = T2: R3 = R2:  T2 = T1: R2 = R1
        T1 = TMP: R1 = RAIN
        MVMTMP = (T1 + T2 + T3 + T4 + T5) / 5
        SUMRAIN = R1 + R2 + R3 + R4 + R5
        Nfixfind = Nfixfind + 1
        cumfind = cumfind + 1
    
        If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
        If nitrogen = 2 And water > 0 Then GoSub SoilN
        
        DAP = 0
        GoSub DailyPrintOut   ' to print out water and N balance before sowing
      
        
        If cumfind > SearchDur Then MAT = 1: MATYP = 3
        If MAT = 1 Then Exit Do
     Loop Until MVMTMP < SowTmp And SUMRAIN = 0 And Nfixfind >= RfreeP
     Pdoy = DOY

ElseIf FixFind = 4 Then
    '---------- Sow when top-layer FTSW1 => SowWat; soilWater should be 'ON'
     cumfind = 0
     
     Do
        Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
        DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
        SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
        TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
        TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
        RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)

        TMIN = TMIN '+ tchng
        TMAX = TMAX '+ tchng
        RAIN = RAIN '* pchng
        TMP = (TMAX + TMIN) / 2
        wthRow = wthRow + 1
        cumfind = cumfind + 1
    
        If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
        If nitrogen = 2 And water > 0 Then GoSub SoilN
        
        DAP = 0
        GoSub DailyPrintOut   ' to print out water and N balance before sowing
        
        If cumfind > SearchDur Then MAT = 1: MATYP = 3
        If MAT = 1 Then Exit Do
     Loop Until FTSW(1) >= SowWat
     Pdoy = DOY

ElseIf FixFind = 5 Then
    '---------- Sow when top-layer FTSW1 <= SowWat; soilWater should be 'ON'
     cumfind = 0
     
     Do
        Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
        DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
        SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
        TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
        TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
        RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)

        TMIN = TMIN '+ tchng
        TMAX = TMAX '+ tchng
        RAIN = RAIN '* pchng
        TMP = (TMAX + TMIN) / 2
        wthRow = wthRow + 1
        cumfind = cumfind + 1
    
        If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
        If nitrogen = 2 And water > 0 Then GoSub SoilN
        
        DAP = 0
        GoSub DailyPrintOut   ' to print out water and N balance before sowing
        
        If cumfind > SearchDur Then MAT = 1: MATYP = 3
        If MAT = 1 Then Exit Do
     Loop Until FTSW(1) <= SowWat
     Pdoy = DOY

ElseIf FixFind = 6 Then
    '-------- Sow when cum rain >= SowWat over a 5-day period
     R1 = 99:   R2 = 99:    R3 = 99:    R4 = 99:    R5 = 99:  RfreeP = 5:
     Nfixfind = 0:          SUMRAIN = 99:     cumfind = 0
     Do
        Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
        DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
        SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
        TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
        TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
        RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)
        TMIN = TMIN '+ tchng
        TMAX = TMAX '+ tchng
        RAIN = RAIN '* pchng
        TMP = (TMAX + TMIN) / 2
        wthRow = wthRow + 1
    
        R5 = R4:  R4 = R3:  R3 = R2:  R2 = R1
        SUMRAIN = R1 + R2 + R3 + R4 + R5
        Nfixfind = Nfixfind + 1
        cumfind = cumfind + 1
    
        If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
        If nitrogen = 2 And water > 0 Then GoSub SoilN
        
        DAP = 0
        GoSub DailyPrintOut   ' to print out water and N balance before sowing
        
        If cumfind > SearchDur Then MAT = 1: MATYP = 3
        If MAT = 1 Then Exit Do
     Loop Until SUMRAIN >= SowWat And Nfixfind >= RfreeP
     Pdoy = DOY

ElseIf FixFind = 7 Then
    '---------- Sow when soil profile ATSW > SowWat; soilWater should be 'ON'
     cumfind = 0
     Do
        Yr = wb.Worksheets("Data").Cells(wthRow, 1)              'wb.Worksheets("Sheet1").Cells(wthRow, 1)
        DOY = wb.Worksheets("Data").Cells(wthRow, 2)             'wb.Worksheets("Sheet1").Cells(wthRow, 2)
        SRAD = wb.Worksheets("Data").Cells(wthRow, 3)            'wb.Worksheets("Sheet1").Cells(wthRow, 3)
        TMAX = wb.Worksheets("Data").Cells(wthRow, 4)            'wb.Worksheets("Sheet1").Cells(wthRow, 4)
        TMIN = wb.Worksheets("Data").Cells(wthRow, 5)            'wb.Worksheets("Sheet1").Cells(wthRow, 5)
        RAIN = wb.Worksheets("Data").Cells(wthRow, 6)            'wb.Worksheets("Sheet1").Cells(wthRow, 6)
        TMIN = TMIN '+ tchng
        TMAX = TMAX '+ tchng
        RAIN = RAIN '* pchng
        TMP = (TMAX + TMIN) / 2
        wthRow = wthRow + 1
        cumfind = cumfind + 1
     
        CBD = 0
        If water = 1 Or water = 2 Or water = 3 Or water = 4 Then GoSub SoilWater
        If nitrogen = 2 And water > 0 Then GoSub SoilN
        
        DAP = 0
        GoSub DailyPrintOut   ' to print out water and N balance before sowing
        
        If cumfind > SearchDur Then MAT = 1: MATYP = 3
     Loop Until ATSWSL >= SowWat Or MAT = 1
     Pdoy = DOY
End If
        
If MAT = 1 Then
   Sheet10.Cells(scyrCntr + 1, 1) = scnName
   Sheet10.Cells(scyrCntr + 1, 2) = locName
   Sheet10.Cells(scyrCntr + 1, 3) = mngName
   Sheet10.Cells(scyrCntr + 1, 4) = solName
   Sheet10.Cells(scyrCntr + 1, 5) = crpName
   Sheet10.Cells(scyrCntr + 1, 6) = Pyear
   Pdoy = -1:
End If
Return



DailyPrintOut:  '////////////////////////////////// DAILY PRINTOUT


dayCntr = dayCntr + 1

If scnNo = 1 And yrno = 1 Then
   Sheet4.Cells(dayCntr + 1, 1) = scnName
   Sheet4.Cells(dayCntr + 1, 2) = locName
   Sheet4.Cells(dayCntr + 1, 3) = mngName
   Sheet4.Cells(dayCntr + 1, 4) = solName
   Sheet4.Cells(dayCntr + 1, 5) = crpName
   Sheet4.Cells(dayCntr + 1, 6) = Pyear
   Sheet4.Cells(dayCntr + 1, 7) = Yr
   Sheet4.Cells(dayCntr + 1, 8) = DOY
   Sheet4.Cells(dayCntr + 1, 9) = DAP
   Sheet4.Cells(dayCntr + 1, 10) = TMP
   Sheet4.Cells(dayCntr + 1, 11) = DTU
   Sheet4.Cells(dayCntr + 1, 12) = CBD
   If CBD > 0 Then
   Sheet4.Cells(dayCntr + 1, 13) = MSNN
   Sheet4.Cells(dayCntr + 1, 14) = GLAI
   Sheet4.Cells(dayCntr + 1, 15) = DLAI
   Sheet4.Cells(dayCntr + 1, 16) = LAI
   Sheet4.Cells(dayCntr + 1, 17) = TCFRUE
   Sheet4.Cells(dayCntr + 1, 18) = FINT
   Sheet4.Cells(dayCntr + 1, 19) = DDMP
   Sheet4.Cells(dayCntr + 1, 20) = GLF
   Sheet4.Cells(dayCntr + 1, 21) = GST
   Sheet4.Cells(dayCntr + 1, 22) = SGR
   Sheet4.Cells(dayCntr + 1, 23) = WLF
   Sheet4.Cells(dayCntr + 1, 24) = WST
   Sheet4.Cells(dayCntr + 1, 25) = WVEG
   Sheet4.Cells(dayCntr + 1, 26) = WGRN
   Sheet4.Cells(dayCntr + 1, 27) = WTOP
   Sheet4.Cells(dayCntr + 1, 28) = DEPORT
   End If
   Sheet4.Cells(dayCntr + 1, 29) = RAIN
   Sheet4.Cells(dayCntr + 1, 30) = IRGW
   Sheet4.Cells(dayCntr + 1, 31) = RUNOF
   Sheet4.Cells(dayCntr + 1, 32) = PET
   Sheet4.Cells(dayCntr + 1, 33) = SEVP
   Sheet4.Cells(dayCntr + 1, 34) = TR
   Sheet4.Cells(dayCntr + 1, 35) = DRAIN
   Sheet4.Cells(dayCntr + 1, 36) = ATSWRZ
   Sheet4.Cells(dayCntr + 1, 37) = FTSWRZ
   Sheet4.Cells(dayCntr + 1, 38) = CRAIN
   Sheet4.Cells(dayCntr + 1, 39) = CIRGW
   Sheet4.Cells(dayCntr + 1, 40) = IRGNO
   Sheet4.Cells(dayCntr + 1, 41) = CRUNOF
   Sheet4.Cells(dayCntr + 1, 42) = CE
   Sheet4.Cells(dayCntr + 1, 43) = CTR
   Sheet4.Cells(dayCntr + 1, 44) = CDRAIN
   Sheet4.Cells(dayCntr + 1, 45) = NFERT
   Sheet4.Cells(dayCntr + 1, 46) = CNFERT
   Sheet4.Cells(dayCntr + 1, 47) = NVOL
   Sheet4.Cells(dayCntr + 1, 48) = CNVOL
   Sheet4.Cells(dayCntr + 1, 49) = NLEACH
   Sheet4.Cells(dayCntr + 1, 50) = CNLEACH
   Sheet4.Cells(dayCntr + 1, 51) = SNMIN
   Sheet4.Cells(dayCntr + 1, 52) = CNMIN
   Sheet4.Cells(dayCntr + 1, 53) = SNDNIT
   Sheet4.Cells(dayCntr + 1, 54) = CNDNIT
   Sheet4.Cells(dayCntr + 1, 55) = SNAVL
   Sheet4.Cells(dayCntr + 1, 56) = CNSOL
   If CBD > 0 Then
   Sheet4.Cells(dayCntr + 1, 57) = NUP
   Sheet4.Cells(dayCntr + 1, 58) = NLF
   Sheet4.Cells(dayCntr + 1, 59) = NST
   Sheet4.Cells(dayCntr + 1, 60) = NVEG
   Sheet4.Cells(dayCntr + 1, 61) = NGRN
   Sheet4.Cells(dayCntr + 1, 62) = CNUP
   End If
   
   Sheet4.Cells(dayCntr + 1, 63) = CUMBNF
   For L = 1 To NLYER
    Sheet4.Cells(dayCntr + 1, L + 63) = WL(L)
   Next L
   Sheet4.Cells(dayCntr + 1, 75) = FTSW(1)

   
    'For L = 1 To NLYER
    '    Sheet11.Cells(dayCntr + 1, L + 12) = FTSW(L)
    'Next L
    
'   Sheet13.Cells(dayCntr + 1, 1) = DAP
'   Sheet13.Cells(dayCntr + 1, 2) = CBD
'   Sheet13.Cells(dayCntr + 1, 3) = BNF
'   Sheet13.Cells(dayCntr + 1, 4) = NUP - BNF
'   Sheet13.Cells(dayCntr + 1, 5) = ATSWRZ
'   Sheet13.Cells(dayCntr + 1, 6) = TTSWRZ
'   Sheet13.Cells(dayCntr + 1, 7) = FTSWRZ
End If


Return



SummaryPrintOut:    '////////////////////////////// SUMMARY PRINTOUT
'Single season and scenario
If scnNo = 1 And yrno = 1 Then
   Sheet1.[i6] = dtEMR
   Sheet1.[i7] = dtR0   'dtTIL
   Sheet1.[i8] = dtR1   'dtSEL
   Sheet1.[i9] = dtR2   'dtBOT
   Sheet1.[i10] = dtR3  'dtEAR
   Sheet1.[i11] = dtR4  'dtANT
   Sheet1.[i12] = dtR5  'dtBSG
   Sheet1.[i13] = dtR6  'dtTSG
   Sheet1.[i14] = dtR7  'dtPM
   Sheet1.[i15] = dtR8  'dtMAT
   
   Sheet1.[i18] = MSNN
   Sheet1.[i19] = R5ANTLAI
   Sheet1.[i20] = R5ANTDM
   
   Sheet1.[i23] = WTOP
   Sheet1.[i24] = WGRN
   Sheet1.[i25] = WGRN / (WTOP + 1E-07) * 100
   
   Sheet1.[i28] = ISOLWAT
   Sheet1.[i29] = CRAIN
   Sheet1.[i30] = CIRGW
   Sheet1.[i31] = IRGNO
   
   Sheet1.[i33] = ATSWSL
   Sheet1.[i34] = CRUNOF
   Sheet1.[i35] = CE
   Sheet1.[i36] = CTR
   Sheet1.[i37] = CDRAIN
   
   Sheet1.[i39] = CE + CTR
   Sheet1.[i40] = CE / (CE + CTR + 1E-06)
   
   Sheet1.[k6] = NLF
   Sheet1.[k7] = NST
   Sheet1.[k8] = (NLF + NST)
   Sheet1.[k9] = NGRN
   Sheet1.[k10] = CNUP
   
   Sheet1.[k13] = ISOLN
   Sheet1.[k14] = CNFERT
   Sheet1.[k15] = CNMIN
   
   Sheet1.[k17] = CNSOL
   Sheet1.[k18] = CNUP
   Sheet1.[k19] = CNVOL
   Sheet1.[k20] = CNLEACH
   Sheet1.[k21] = CNDNIT
End If

'Multiple season and/or scenarios

   Sheet10.Cells(scyrCntr + 1, 1) = scnName
   Sheet10.Cells(scyrCntr + 1, 2) = locName
   Sheet10.Cells(scyrCntr + 1, 3) = mngName
   Sheet10.Cells(scyrCntr + 1, 4) = solName
   Sheet10.Cells(scyrCntr + 1, 5) = crpName
   Sheet10.Cells(scyrCntr + 1, 6) = Pyear
   Sheet10.Cells(scyrCntr + 1, 7) = Pdoy
   Sheet10.Cells(scyrCntr + 1, 8) = dtEMR
   Sheet10.Cells(scyrCntr + 1, 90) = dtR0   'dtTIL
   Sheet10.Cells(scyrCntr + 1, 10) = dtR1   'dtSEL
   Sheet10.Cells(scyrCntr + 1, 11) = dtR2   'dtBOT
   Sheet10.Cells(scyrCntr + 1, 12) = dtR3   'dtEAR
   Sheet10.Cells(scyrCntr + 1, 13) = dtR4   'dtANT
   Sheet10.Cells(scyrCntr + 1, 14) = dtR5   'dtBSG
   Sheet10.Cells(scyrCntr + 1, 15) = dtR6   'dtTSG
   Sheet10.Cells(scyrCntr + 1, 16) = dtR7   'dtPM
   Sheet10.Cells(scyrCntr + 1, 17) = dtR8   'dtMAT
   Sheet10.Cells(scyrCntr + 1, 18) = MSNN
   Sheet10.Cells(scyrCntr + 1, 19) = R5ANTLAI
   Sheet10.Cells(scyrCntr + 1, 20) = R5ANTDM
   Sheet10.Cells(scyrCntr + 1, 21) = WTOP
   Sheet10.Cells(scyrCntr + 1, 22) = WGRN
   Sheet10.Cells(scyrCntr + 1, 23) = WGRN / (WTOP + 1E-05)   'hi
   Sheet10.Cells(scyrCntr + 1, 24) = ISOLWAT
   Sheet10.Cells(scyrCntr + 1, 25) = CRAIN
   Sheet10.Cells(scyrCntr + 1, 26) = CIRGW
   Sheet10.Cells(scyrCntr + 1, 27) = IRGNO
   Sheet10.Cells(scyrCntr + 1, 28) = ATSWSL
   Sheet10.Cells(scyrCntr + 1, 29) = CRUNOF
   Sheet10.Cells(scyrCntr + 1, 30) = CE
   Sheet10.Cells(scyrCntr + 1, 31) = CTR
   Sheet10.Cells(scyrCntr + 1, 32) = CDRAIN
   Sheet10.Cells(scyrCntr + 1, 33) = CE + CTR
   Sheet10.Cells(scyrCntr + 1, 34) = CE / (CE + CTR + 1E-05)
   Sheet10.Cells(scyrCntr + 1, 35) = NLF
   Sheet10.Cells(scyrCntr + 1, 36) = NST
   Sheet10.Cells(scyrCntr + 1, 37) = NVEG
   Sheet10.Cells(scyrCntr + 1, 38) = NGRN
   Sheet10.Cells(scyrCntr + 1, 39) = CNUP
   Sheet10.Cells(scyrCntr + 1, 40) = CUMBNF
   Sheet10.Cells(scyrCntr + 1, 41) = ISOLN
   Sheet10.Cells(scyrCntr + 1, 42) = CNFERT
   Sheet10.Cells(scyrCntr + 1, 43) = CNMIN
   Sheet10.Cells(scyrCntr + 1, 44) = CNSOL
   Sheet10.Cells(scyrCntr + 1, 45) = CNVOL
   Sheet10.Cells(scyrCntr + 1, 46) = CNLEACH
   Sheet10.Cells(scyrCntr + 1, 47) = CNDNIT
   Sheet10.Cells(scyrCntr + 1, 48) = MATYP
   Sheet10.Cells(scyrCntr + 1, 49) = SRAINT
   Sheet10.Cells(scyrCntr + 1, 50) = MTMINT
   Sheet10.Cells(scyrCntr + 1, 51) = MTMAXT
   Sheet10.Cells(scyrCntr + 1, 52) = SSRADT
   Sheet10.Cells(scyrCntr + 1, 53) = SUMETT
   Sheet10.Cells(scyrCntr + 1, 54) = SRAIN2
   Sheet10.Cells(scyrCntr + 1, 55) = MTMIN2
   Sheet10.Cells(scyrCntr + 1, 56) = MTMAX2
   Sheet10.Cells(scyrCntr + 1, 57) = SSRAD2
   Sheet10.Cells(scyrCntr + 1, 58) = SUMET2
   Sheet10.Cells(scyrCntr + 1, 59) = SRAIN3
   Sheet10.Cells(scyrCntr + 1, 60) = MTMIN3
   Sheet10.Cells(scyrCntr + 1, 61) = MTMAX3
   Sheet10.Cells(scyrCntr + 1, 62) = SSRAD3
   Sheet10.Cells(scyrCntr + 1, 63) = SUMET3

'**** LayerO
   Sheet11.Cells(scyrCntr + 1, 1) = scnName
   Sheet11.Cells(scyrCntr + 1, 2) = locName
   Sheet11.Cells(scyrCntr + 1, 3) = mngName
   Sheet11.Cells(scyrCntr + 1, 4) = solName
   Sheet11.Cells(scyrCntr + 1, 5) = crpName
   Sheet11.Cells(scyrCntr + 1, 6) = Pyear
   Sheet11.Cells(scyrCntr + 1, 7) = Pdoy
   
   For L = 1 To NLYER
       Sheet11.Cells(scyrCntr + 1, L + 8) = iATSW(L)
   Next L
   For L = 1 To NLYER
       Sheet11.Cells(scyrCntr + 1, L + 8 + 10) = iFTSW(L)
   Next L
   For L = 1 To NLYER
       Sheet11.Cells(scyrCntr + 1, L + 8 + 20) = INSOL(L)
   Next L
   For L = 1 To NLYER
       Sheet11.Cells(scyrCntr + 1, L + 8 + 30) = ATSW(L)
   Next L
   For L = 1 To NLYER
       Sheet11.Cells(scyrCntr + 1, L + 8 + 40) = FTSW(L)
   Next L
   For L = 1 To NLYER
       Sheet11.Cells(scyrCntr + 1, L + 8 + 50) = NSOL(L)
   Next L
   For L = 1 To NLYER
       Sheet11.Cells(scyrCntr + 1, L + 8 + 60) = MNORG(L)
   Next L
Return


End Sub '-------------------------------------------



Sub Headers()
'Headers:    '////////////////////////////////// HEADERS
'Titles for daily output; when the model is run for 1-scenario-1-year
'If scnNo = 1 And yrno = 1 Then

   Sheet4.Cells(1, 1) = "sName"
   Sheet4.Cells(1, 2) = "Location"
   Sheet4.Cells(1, 3) = "Manag"
   Sheet4.Cells(1, 4) = "Soil"
   Sheet4.Cells(1, 5) = "Crop"
   Sheet4.Cells(1, 6) = "Pyear"
   Sheet4.Cells(1, 7) = "Year"
   Sheet4.Cells(1, 8) = "DOY"
   Sheet4.Cells(1, 9) = "DAP"
   Sheet4.Cells(1, 10) = "TMP"
   Sheet4.Cells(1, 11) = "DTU"
   Sheet4.Cells(1, 12) = "CBD"
   Sheet4.Cells(1, 13) = "MSNN"
   Sheet4.Cells(1, 14) = "GLAI"
   Sheet4.Cells(1, 15) = "DLAI"
   Sheet4.Cells(1, 16) = "LAI"
   Sheet4.Cells(1, 17) = "TCFRUE"
   Sheet4.Cells(1, 18) = "FINT"
   Sheet4.Cells(1, 19) = "DDMP"
   Sheet4.Cells(1, 20) = "GLF"
   Sheet4.Cells(1, 21) = "GST"
   Sheet4.Cells(1, 22) = "SGR"
   Sheet4.Cells(1, 23) = "WLF"
   Sheet4.Cells(1, 24) = "WST"
   Sheet4.Cells(1, 25) = "WVEG"
   Sheet4.Cells(1, 26) = "WGRN"
   Sheet4.Cells(1, 27) = "WTOP"
   Sheet4.Cells(1, 28) = "DEPORT"
   Sheet4.Cells(1, 29) = "RAIN"
   Sheet4.Cells(1, 30) = "IRGW"
   Sheet4.Cells(1, 31) = "RUNOF"
   Sheet4.Cells(1, 32) = "PET"
   Sheet4.Cells(1, 33) = "SEVP"
   Sheet4.Cells(1, 34) = "TR"
   Sheet4.Cells(1, 35) = "DRAIN"
   Sheet4.Cells(1, 36) = "ATSWRZ"
   Sheet4.Cells(1, 37) = "FTSWRZ"
   Sheet4.Cells(1, 38) = "CRAIN"
   Sheet4.Cells(1, 39) = "CIRGW"
   Sheet4.Cells(1, 40) = "IRGNO"
   Sheet4.Cells(1, 41) = "CRUNOF"
   Sheet4.Cells(1, 42) = "CE"
   Sheet4.Cells(1, 43) = "CTR"
   Sheet4.Cells(1, 44) = "CDRAIN"
   Sheet4.Cells(1, 45) = "NFERT"
   Sheet4.Cells(1, 46) = "CNFERT"
   Sheet4.Cells(1, 47) = "NVOL"
   Sheet4.Cells(1, 48) = "CNVOL"
   Sheet4.Cells(1, 49) = "NLEACH"
   Sheet4.Cells(1, 50) = "CNLEACH"
   Sheet4.Cells(1, 51) = "NMIN"
   Sheet4.Cells(1, 52) = "CNMIN"
   Sheet4.Cells(1, 53) = "NDNIT"
   Sheet4.Cells(1, 54) = "CNDNIT"
   Sheet4.Cells(1, 55) = "SNAVL"
   Sheet4.Cells(1, 56) = "NSOL"
   Sheet4.Cells(1, 57) = "NUP"
   Sheet4.Cells(1, 58) = "NLF"
   Sheet4.Cells(1, 59) = "NST"
   Sheet4.Cells(1, 60) = "NVEG"
   Sheet4.Cells(1, 61) = "NGRN"
   Sheet4.Cells(1, 62) = "CNUP"
   Sheet4.Cells(1, 63) = "CUMBNF"
   For L = 1 To 10
    Sheet4.Cells(1, L + 63) = "WL(" & L & ")"
   Next L
   Sheet4.Cells(1, 63 + 11) = "SWL"
   
'ThisWorkbook.sheet11.Cells(1, 1) = "DAP"
'ThisWorkbook.sheet11.Cells(1, 2) = "ATSW by Layer"
'ThisWorkbook.sheet11.Cells(1, 13) = "FTSW by Layer"
'End If

'Titles for each season/scenario output
   Sheet10.Cells(1, 1) = "sName"
   Sheet10.Cells(1, 2) = "Location"
   Sheet10.Cells(1, 3) = "Manag"
   Sheet10.Cells(1, 4) = "Soil"
   Sheet10.Cells(1, 5) = "Crop"
   Sheet10.Cells(1, 6) = "Pyear"
   Sheet10.Cells(1, 7) = "Pdoy"
   Sheet10.Cells(1, 8) = "dtEMR"
   
   Sheet10.Cells(1, 9) = "R0.TIL.TSE"
   Sheet10.Cells(1, 10) = "R1.SEL.SIL"
   Sheet10.Cells(1, 11) = "R2.BOT"
   Sheet10.Cells(1, 12) = "R3.EAR"
   Sheet10.Cells(1, 13) = "R4.ANT"
   Sheet10.Cells(1, 14) = "R5.BSG"
   Sheet10.Cells(1, 15) = "R6.TSG"
   Sheet10.Cells(1, 16) = "R7.PM"
   Sheet10.Cells(1, 17) = "R8.MAT"
   
   Sheet10.Cells(1, 18) = "MSNN"
   Sheet10.Cells(1, 19) = "R5.ANTLAI"
   Sheet10.Cells(1, 20) = "R5.ANTDM"
   Sheet10.Cells(1, 21) = "WTOP"
   Sheet10.Cells(1, 22) = "WGRN"
   Sheet10.Cells(1, 23) = "HI"
   Sheet10.Cells(1, 24) = "IPASW"
   Sheet10.Cells(1, 25) = "CRAIN"
   Sheet10.Cells(1, 26) = "CIRGW"
   Sheet10.Cells(1, 27) = "IRGNO"
   Sheet10.Cells(1, 28) = "ATSWSL"
   Sheet10.Cells(1, 29) = "CRUNOF"
   Sheet10.Cells(1, 30) = "CE"
   Sheet10.Cells(1, 31) = "CTR"
   Sheet10.Cells(1, 32) = "CDRAIN"
   Sheet10.Cells(1, 33) = "ET"
   Sheet10.Cells(1, 34) = "E/ET"
   Sheet10.Cells(1, 35) = "NLF"
   Sheet10.Cells(1, 36) = "NST"
   Sheet10.Cells(1, 37) = "NVEG"
   Sheet10.Cells(1, 38) = "NGRN"
   Sheet10.Cells(1, 39) = "CNUP"
   Sheet10.Cells(1, 40) = "CUMBNF"
   Sheet10.Cells(1, 41) = "ISOLN"
   Sheet10.Cells(1, 42) = "CNFERT"
   Sheet10.Cells(1, 43) = "CNMIN"
   Sheet10.Cells(1, 44) = "CNSOL"
   Sheet10.Cells(1, 45) = "CNVOL"
   Sheet10.Cells(1, 46) = "CNLEACH"
   Sheet10.Cells(1, 47) = "CNDNIT"
   Sheet10.Cells(1, 48) = "MATYP"
   Sheet10.Cells(1, 49) = "SRAINT"
   Sheet10.Cells(1, 50) = "MTMINT"
   Sheet10.Cells(1, 51) = "MTMAXT"
   Sheet10.Cells(1, 52) = "SSRADT"
   Sheet10.Cells(1, 53) = "SUMETT"
   Sheet10.Cells(1, 54) = "SRAIN2"
   Sheet10.Cells(1, 55) = "MTMIN2"
   Sheet10.Cells(1, 56) = "MTMAX2"
   Sheet10.Cells(1, 57) = "SSRAD2"
   Sheet10.Cells(1, 58) = "SUMET2"
   Sheet10.Cells(1, 59) = "SRAIN3"
   Sheet10.Cells(1, 60) = "MTMIN3"
   Sheet10.Cells(1, 61) = "MTMAX3"
   Sheet10.Cells(1, 62) = "SSRAD3"
   Sheet10.Cells(1, 63) = "SUMET3"
   
'Print headers in "LayerO"
   Sheet11.Cells(1, 1) = "sName"
   Sheet11.Cells(1, 2) = "Location"
   Sheet11.Cells(1, 3) = "Manag"
   Sheet11.Cells(1, 4) = "Soil"
   Sheet11.Cells(1, 5) = "Crop"
   Sheet11.Cells(1, 6) = "Pyear"
   Sheet11.Cells(1, 7) = "Pdoy"
   Sheet11.Cells(1, 9) = "iATSW1"
   Sheet11.Cells(1, 10) = "iATSW2"
   Sheet11.Cells(1, 11) = "iATSW3"
   Sheet11.Cells(1, 12) = "iATSW4"
   Sheet11.Cells(1, 13) = "iATSW5"
   Sheet11.Cells(1, 14) = "iATSW6"
   Sheet11.Cells(1, 15) = "iATSW7"
   Sheet11.Cells(1, 16) = "iATSW8"
   Sheet11.Cells(1, 17) = "iATSW9"
   Sheet11.Cells(1, 18) = "iATSW10"
   Sheet11.Cells(1, 19) = "iFTSW1"
   Sheet11.Cells(1, 20) = "iFTSW2"
   Sheet11.Cells(1, 21) = "iFTSW3"
   Sheet11.Cells(1, 22) = "iFTSW4"
   Sheet11.Cells(1, 23) = "iFTSW5"
   Sheet11.Cells(1, 24) = "iFTSW6"
   Sheet11.Cells(1, 25) = "iFTSW7"
   Sheet11.Cells(1, 26) = "iFTSW8"
   Sheet11.Cells(1, 27) = "iFTSW9"
   Sheet11.Cells(1, 28) = "iFTSW10"
   Sheet11.Cells(1, 29) = "iNSOL1"
   Sheet11.Cells(1, 30) = "iNSOL2"
   Sheet11.Cells(1, 31) = "iNSOL3"
   Sheet11.Cells(1, 32) = "iNSOL4"
   Sheet11.Cells(1, 33) = "iNSOL5"
   Sheet11.Cells(1, 34) = "iNSOL6"
   Sheet11.Cells(1, 35) = "iNSOL7"
   Sheet11.Cells(1, 36) = "iNSOL8"
   Sheet11.Cells(1, 37) = "iNSOL9"
   Sheet11.Cells(1, 38) = "iNSOL10"
   Sheet11.Cells(1, 39) = "fATSW1"
   Sheet11.Cells(1, 40) = "fATSW2"
   Sheet11.Cells(1, 41) = "fATSW3"
   Sheet11.Cells(1, 42) = "fATSW4"
   Sheet11.Cells(1, 43) = "fATSW5"
   Sheet11.Cells(1, 44) = "fATSW6"
   Sheet11.Cells(1, 45) = "fATSW7"
   Sheet11.Cells(1, 46) = "fATSW8"
   Sheet11.Cells(1, 47) = "fATSW9"
   Sheet11.Cells(1, 48) = "fATSW10"
   Sheet11.Cells(1, 49) = "fFTSW1"
   Sheet11.Cells(1, 50) = "fFTSW2"
   Sheet11.Cells(1, 51) = "fFTSW3"
   Sheet11.Cells(1, 52) = "fFTSW4"
   Sheet11.Cells(1, 53) = "fFTSW5"
   Sheet11.Cells(1, 54) = "fFTSW6"
   Sheet11.Cells(1, 55) = "fFTSW7"
   Sheet11.Cells(1, 56) = "fFTSW8"
   Sheet11.Cells(1, 57) = "fFTSW9"
   Sheet11.Cells(1, 58) = "fFTSW10"
   Sheet11.Cells(1, 59) = "fNSOL1"
   Sheet11.Cells(1, 60) = "fNSOL2"
   Sheet11.Cells(1, 61) = "fNSOL3"
   Sheet11.Cells(1, 62) = "fNSOL4"
   Sheet11.Cells(1, 63) = "fNSOL5"
   Sheet11.Cells(1, 64) = "fNSOL6"
   Sheet11.Cells(1, 65) = "fNSOL7"
   Sheet11.Cells(1, 66) = "fNSOL8"
   Sheet11.Cells(1, 67) = "fNSOL9"
   Sheet11.Cells(1, 68) = "fNSOL10"
   Sheet11.Cells(1, 69) = "MNORG1"
   Sheet11.Cells(1, 70) = "MNORG2"
   Sheet11.Cells(1, 71) = "MNORG3"
   Sheet11.Cells(1, 72) = "MNORG4"
   Sheet11.Cells(1, 73) = "MNORG5"
   Sheet11.Cells(1, 74) = "MNORG6"
   Sheet11.Cells(1, 75) = "MNORG7"
   Sheet11.Cells(1, 76) = "MNORG8"
   Sheet11.Cells(1, 77) = "MNORG9"
   Sheet11.Cells(1, 78) = "MNORG10"
'Return
End Sub










