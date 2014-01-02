Requirements
- Config
 * Port Name, Baud Rate, Stop Bit, …
 * Two time of the day to try dial
 * Dial number
 * Retry interval
 * Number of retries
- Random start time (around 15 minutes)
- Retry on error: (ring/no dial tone), OK is good
- Error log


To un/install service
installutil.exe MyNewService.exe
installutil.exe /u MyNewService.exe
