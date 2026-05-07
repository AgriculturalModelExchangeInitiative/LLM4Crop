'------------------------------- Potential ET
TD = 0.6 * TMAX + 0.4 * TMIN
ALBEDO = CALB * (1 - Exp(-KET * ETLAI)) + SALB * Exp(-KET * ETLAI)
EEQ = SRAD * (0.004876 - 0.004374 * ALBEDO) * (TD + 29)
PET = EEQ * 1.1
If TMAX > 34 Then PET = EEQ * ((TMAX - 34) * 0.05 + 1.1)
If TMAX < 5 Then PET = EEQ * 0.01 * Exp(0.18 * (TMAX + 20))