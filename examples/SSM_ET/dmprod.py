"""
SSM model for Potential DryMatter Production.

A summary of calculation method of daily dry matter production by
crop canopies.
From sowing to emergence:
    - No dry matter production!
From emergence to termination seed growth (TSG):
    - Fraction intercepted PAR is calculated from LAI and extinction coefficient.
    - Daily incident PAR is assumed to be half of daily total solar radiation.
    - Actual RUE is obtained by adjusting potential RUE for daily mean temperature.
    - Daily mass production is computed from intercepted PAR and RUE.
From TSG to crop harvest maturity:
    - No dry matter production!
"""
from math import exp

def dry_matter_prod(tmax: float, tmin: float, srad: float, lai: float, 
                    kpar: float=0.65, # parameter
                    RUE: float =2.2, # parameter for wheat
                    TBRUE: float=0., TP1RUE: float=15, TP2RUE: float=22, TCRUE: float=35,
                    ):
    """
    Calculate dry matter production at daily time step.

    Parameters:
    tmax (float): Daily maximum temperature (°C).
    tmin (float): Daily minimum temperature (°C).
    srad (float): Daily solar radiation (MJ m-2 day-1).
    lai (float): Leaf Area Index  (m2 m-2).
    kpar (float): Extinction coefficient (default is 0.65 for wheat).
    RUE (float): Radiation Use Efficiency (default is 2.2 g MJ-1 at optimal temperature for wheat).
    TBRUE (float): Base temperature for RUE adjustment (°C).
    TP1RUE (float): Lower optimal temperature for RUE adjustment (°C).
    TP2RUE (float): Upper optimal temperature for RUE adjustment (°C).
    TCRUE (float): Ceiling temperature for RUE adjustment (°C).

    Returns:
    float: rate of dry matter production (g m-2 day-1).
    """
    # Calculate average daily temperature TD
    tmp =  tmax + 0.4 * tmin

    coeff_RUE = 0.
    if tmp <= TBRUE or tmp >= TCRUE:
        coeff_RUE = 0.
    elif TBRUE < tmp < TP1RUE:
        coeff_RUE = (tmp - TBRUE) / (TP1RUE - TBRUE)
    elif TP2RUE <= tmp <= TCRUE:
        coeff_RUE = (TCRUE - tmp) / (TCRUE - TP2RUE)
    else:  # TP1RUE <= tmp < TP2RUE
        coeff_RUE = 1.

    actual_RUE = RUE * coeff_RUE  # g MJ-1

    # Calculate fraction of PAR intercepted by canopy
    fint = 1 - exp(-kpar * lai)

    # Daily dry matter production
    DDMP = srad * 0.48 * fint * actual_RUE  # g m-2 day-1

    return DDMP
    





if __name__ == "__main__":
    SRAD = 22.8
    TMAX = 13.
    TMIN = 1.

    LAI = 2.


    ddmp = dry_matter_prod(TMAX, TMIN, SRAD, LAI)
    print(f"Calculated Dry Matter Production: {ddmp:.2f} g/ m2 / day")