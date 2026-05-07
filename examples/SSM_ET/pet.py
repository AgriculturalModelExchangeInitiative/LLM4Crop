"""
SSM model for Potential Evapotranspiration (PET).

A simplified Penman equation in combination with the exponential Beer–Bouguer–Lambert equation 
(to account for fraction uncovered soil) was used
to calculate potential evaporation from bare, wet soil surface. Another method that is commonly used is the Priestley and 
Taylor method (Priestley and Taylor, 1972) as modified and described by Ritchie (1998). 

Text from Sultani and Sinclair, 2012 p188
This method also needs daily maximum and minimum temperature and solar radiation. 
Potential evaporation (EOS) from bare, wet soil surface is obtained 
from potential evapotranspiration (PET, mm day-1) and the fraction of soil 
that is not covered by the crop. As mentioned before, the fraction of uncovered soil 
is calculated using ETLAI and KET based on exponential Beer–Bouguer–Lambert equation.
EOS = PET x EXP(-KET x ETLAI) (14.29)
Potential evapotranspiration (PET, mm day-1) is calculated as the equilibrium
evaporation (EEQ, mm day-1) multiplied by 1.1 to account for the effect of 
unsaturated air. The multiplier is increased above 1.1 to allow for advection 
(TMAX > 34) and is reduced to account for the influence of frozen soil on evaporation and
cold temperatures on stomatal closure (TMAX < 5) when necessary (Fig. 14.9).
PET = EEQ x 1.1 if 5 < TMAX < 34
PET = EEQ ( (TMAX - 34) x 0.05 + 1.1) if TMAX > 34
PET = EEQ x 0.01 x EXP (0.18 x (TMAX + 20) ) if TMAX < 5 (14.30)
EEQ is obtained from surface (crop plus soil) albedo (ALBEDO), average air temperature
during the day (TD, °C), and daily solar radiation (SRAD, MJ m-2 day-1):
EEQ = SRAD x (0.004876 - 0.004374 x ALBEDO) x (TD + 29) (14.31)
TD is computed using a higher weight for daily maximum temperature
(TMAX, °C) and a lower weight for daily minimum temperature (TMIN, °C):
TD = 0.6 x TMAX + 0.4 x TMIN (14.32)
Surface albedo (ALBEDO) depends on the proportion of the field surface that is
covered by crop or soil and the albedos of the crop (CALB) and the soil (SALB).
CALB is fairly constant at a value of 0.23.
"""
from math import exp

def potential_evapotranspiration(tmax, tmin, srad, albedo: float=1):
    """
    Calculate Potential Evapotranspiration (PET) using a simplified Penman equation.

    Parameters:
    tmax (float): Daily maximum temperature (°C).
    tmin (float): Daily minimum temperature (°C).
    srad (float): Daily solar radiation (MJ m-2 day-1).
    albedo (float): Surface Albedo.


    Returns:
    float: Potential Evapotranspiration (PET) in mm day-1.
    """
    # Calculate average daily temperature TD
    td = 0.6 * tmax + 0.4 * tmin

    # Calculate surface albedo based on crop and soil albedos
    #fraction_nrj_soil = exp(-ket*etlai)
    #albedo = calb * (1 - fraction_nrj_soil) + salb * fraction_nrj_soil

    # Calculate equilibrium evaporation EEQ
    eeq = srad * (0.004876 - 0.004374 * albedo) * (td + 29)


    # Calculate PET based on TMAX
    if 5 < tmax < 34:
        pet = eeq * 1.1
    elif tmax >= 34:
        pet = eeq * ((tmax - 34) * 0.05 + 1.1)
    else:  # tmax <= 5
        pet = eeq * 0.01 * exp(0.18 * (tmax + 20))

    return pet  




if __name__ == "__main__":
    SRAD = 122.8
    TMAX = 13.
    TMIN = 1.

    LAI = 2.
    ket=0.5
    calb=0.23
    salb=0.13

    fraction_nrj_soil = exp(-ket*LAI)
    albedo = calb * (1 - fraction_nrj_soil) + salb * fraction_nrj_soil
    #albedo = 0.20
    
    pet_value = potential_evapotranspiration(TMAX, TMIN, SRAD, albedo)
    print(f"Calculated PET: {pet_value:.2f} mm/day")