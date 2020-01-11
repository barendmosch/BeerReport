# Server Side Programming
## Software Requirements
-Postman / Insomnia / Fiddler
-Visual Studio 2017 >= 15.7 
-Azure Development workload
-AzureRM module for PowerShell
-Sql Server Management Studio
-Storage Explorer (storageexplorer.com)

## Task Requirements
	Doel: vraag een kaartje en voorspelde temperatuur op van een lokatie, waarbij gebaseerd op temperatuur geadviseerd wordt of je bier moet kopen of niet.
### Must
-Expose public accessible API for requesting bierrapport.
-Employ Queue.
-Employ Blob Storage.
-Employ Azure Maps API / or any other maps API for generating map image.
-Employ OpenWeatherMap API for fetching forecast.
-Expose public accessible API for fetching finished bierrapport.
-Use Azure Function for background task processing.
-Provide exported Postman Collection (or similar) as API documentation. 
-Deploy required resources using ARM template.
-Deploy code using Visual Studio + Publish Profile.

### Could
-Other language than C#
-Deploy code using script (Azure CLI).
-Use authentication on request API.
-Use Azure AD authentication on request API.
-Deploy code automatically from SCM (use VSTS).
-Employ multiple queues for each external API.
-Use SAS token instead of public accessible blob storage for fetching finished bierrapport directly from Blob.
-Slow down queue consuming and provide status API for fetching processing status and saving status in Table.

### Would
-Other useful features you can think of!

Wanneer alle must haves op de juiste manier zijn geïmplementeerd leidt dit tot het cijfer 6.0. Met de could haves en would haves kunnen extra punten verdiend worden. 

De opdracht dient te worden ingeleverd in de vorm van een git repository, bij voorkeur een die publiek toegankelijk is en kan worden gecloned. Indien dit geen optie is, resteert het opsturen van een zipfile. Het inleveren geschiedt door middel van het sturen van een email aan T.Bleijendaal@wearetriple.com met een cc aan erwin.devries@inholland.nl.De deadline voor de 1e kans is 16 oktober om 23:59. De deadline voor de herkansing ligt in periode 2 en zal via moodle nader bekend worden gemaakt.

