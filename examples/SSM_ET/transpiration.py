"""
SSM model for Potential Transpiration .


"""
from math import exp

def potential_transpiration(tmax: float, tmin: float, ddmp:float, 
                            TEC: float= 5.8, # parameter
                            VPDF: float=0.75, # parameter
                            ):
    """
    Calculate dry matter production at daily time step.

    Parameters:
    tmax (float): Daily maximum temperature (°C).
    tmin (float): Daily minimum temperature (°C).
    ddmp (float): daily dry matter production (g m-2 day-1).
    TEC (float): Transpiration efficiency coefficient (default is 5.8 g mm-1).
    VPDF (float): Vapor pressure deficit factor (default is 0.75).

    Returns:
    float: potential transpiration rate (TR) in mm day-1.
    """
    # Calculate VPTMIN
    vptmin = 0.6108 * exp((17.27 * tmin) / (tmin + 237.3))
    # Calculate VPTMAX
    vptmax = 0.6108 * exp((17.27 * tmax) / (tmax + 237.3))
    # Calculate VPD
    VPD = VPDF * (vptmax - vptmin)

    TR = ddmp * VPD / TEC

    return TR
    





if __name__ == "__main__":
    TMAX = 25.
    TMIN = 1.
    DDMP = 20. # example dry matter production in g m-2 day-1


    TR = potential_transpiration(TMAX, TMIN, DDMP, TEC=5.8, VPDF=0.75)
    print(f"Calculated Potential Transpiration: {TR:.2f} mm/day")