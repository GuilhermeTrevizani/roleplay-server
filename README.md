# Roleplay Server

Text-based roleplay server developed for **alt:V** (https://altv.mp/).

## How to Run

- Change the database connection string with your settings in the option **dbConnectionString** in the file **ALTVSERVER/server.cfg**.

- Change the database connection string in the source code in the file below https://github.com/GuilhermeTrevizani/roleplay-server/blob/7340482e0ab93fc98cbff5fd1324cc9369b62f7b/Roleplay/Global.cs#L920

- Rebuild the project and use the EF Migrations command **update-database** using **Package Manager Console**.

- Run **ALTVSERVER//altv-server.exe**.

## Support

For support send a message in Discord **Treviza#5180**.
