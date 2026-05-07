C=======================================================================
C  YR_DOY, Subroutine, N.B. Pickering, 09/13/91
C  Converts YRDOY to YR and DOY.
C-----------------------------------------------------------------------
C  Input : YRDOY
C  Output: YR,DOY
C=======================================================================
!%%NotRequired%%
      SUBROUTINE YR_DOY(YRDOY,YR,DOY)
      ! test

      IMPLICIT NONE

      INTEGER, INTENT(IN) :: YRDOY
      INTEGER, INTENT(OUT):: DOY,YR

      !calcule

      YR  = INT(YRDOY / 1000)
      DOY = YRDOY - YR * 1000

      END SUBROUTINE YR_DOY