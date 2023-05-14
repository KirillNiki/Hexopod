@REM c:\windows\system32\OpenSSH\ssh kirill@192.168.0.49 "rm -R /home/kirill/Desktop/Hexopod && mkdir /home/kirill/Desktop/Hexopod"
@REM pscp -r -pw Ng3-dcc25 ./* kirill@192.168.0.49:/home/kirill/Desktop/Hexopod
@REM c:\windows\system32\OpenSSH\ssh kirill@192.168.0.49 "cd /home/kirill/Desktop/Hexopod/ && dotnet run"

@REM c:\windows\system32\OpenSSH\ssh kirill@192.168.0.49 "rm /home/kirill/Desktop/*.* && rm -R /home/kirill/Desktop/bin"
pscp  -pw Ng3-dcc25 ./* kirill@192.168.0.49:/home/kirill/Desktop/Hexopod
c:\windows\system32\OpenSSH\ssh -i C:\Users\kinik\.ssh\id_rsa kirill@192.168.0.49 "pkill gpioTests"
c:\windows\system32\OpenSSH\ssh -i C:\Users\kinik\.ssh\id_rsa kirill@192.168.0.49 "rm -R -f /home/kirill/Desktop/Hexopod/bin/ 2> /dev/null && cd /home/kirill/Desktop/Hexopod/ && dotnet run"