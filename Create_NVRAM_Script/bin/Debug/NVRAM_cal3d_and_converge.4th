0x10000 userOpen
: put start: userWrite ;
put variable lastErr
put : cal3D
put calmode 1 set
put ." Calibration starting" cr
put calCommand cal_start set
put ." Press any key to take next point, ESC to finish" cr
put calCommand cal_start set drop
put 10 delay
put begin
put key 27 = 0= calNumPoints &di @ 12 < &&
put while
put calCommand cal_capture set drop
put 300 delay
put calNumPoints di.
put repeat
put 300 delay
put ." Starting error settling" cr
put calCommand cal_end_capture set drop
put magErr &di @ lastErr !
put begin
put 250 delay
put ?key 0= magErr &di @ lastErr @ f- fabs f0.001 f> &&
put while
put magErr &di @ dup lastErr ! f. cr
put repeat
put ." Calibration done!" cr
put calCommand cal_end set
put 300 delay
put calmode 0 set
put 300 delay
put calPointDistribution di.
put magFieldCalErr di.
put ;
userClose
