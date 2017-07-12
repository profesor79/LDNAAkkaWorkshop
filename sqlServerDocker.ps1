docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<YourStrong!Passw0rd>" -p 1400:1433  -d microsoft/mssql-server-linux
# -v sqlvolume:/var/opt/mssql