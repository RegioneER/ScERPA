# ScERPA
Si tratta di una applicazione Web - MVC e Pages realizzata in Microsoft .net core 8 che implementa una **WebUI** di consultazione di alcuni servizi ANPR.
E' semplicemente una interfaccia grafica che necessita per funzionare di **un api gateway verso ANPR che implementi il flusso di accesso agli EServices tramite PDND** che nel caso di Regione Emilia-Romagna consiste di un WSO2 (con flussi realizzati con Microintegrator).
Il presente progetto non include la parte dell'api gateway.

## Indice

L'installazione e le personalizzazioni sono a carico del soggetto che prende a riuso la soluzione.

- Come iniziare
- Contenuto del pacchetto
- Licenze software dei componenti di terze parti

### Come iniziare

#### Prerequisiti
L'applicazione necessita di:

Microsoft Visual Studio 2022 o versioni superiori o Visual Studio Code.\
Microsoft .NET core 8.x
Microsoft Sql-Server (altrimenti modificare il codice per usare altri db con la relativa library efcore)
**Un api gateway verso ANPR che implementi il flusso di accesso agli EServices tramite PDND** e che prevede l'accesso alle api esposte di ANPR tramite Oauth2 con clientid e clientsecret e il passaggio in header dei parametri di audit rest (userID, userLocation, LoA) e del purpouse id (purposeid) per i dettagli si veda (Services/AnprApiWSO2.cs).

#### Contenuto del pacchetto
La soluzione si compone di un solo progetto, può essere eseguito in modalità debug e ambiente develop in questo caso effettuerà le migrations del db e aggiungerà un utente SuperAdmin per provare l'applicativo.

#### Contenuto del pacchetto
Una volta istanziato un sql server (anche express) personalizzare i file 
appsettings.json
appsettings.Development.json
in tutti i punti in cui compaiono le graffe tipo  {...indicazione su cosa inserire...} ed impostare la modalità test (ambiente sul config uguale ad 1)
Eseguire il progetto in https in modalità debug e ambiente asp net Development, in questo caso verranno applicate le migration per generare il database, istanziata una utenza SuperAdmin e messa a disposizione una funzionalità di registrazione utenza per creare gli utenti applicativi.

#### Personalizzazioni

- modificare il config appsettings per ambienti di test e produzione
- modificare la parte di autenticazione (al momento usa una versione parzialemente modificata ottenuta con scaffolding parziale di Identity) adattandola al proprio sistema di autenticazione (il codice è sotto Areas/Identity)
- modificare i template delle views
  
### Licenze software dei componenti di terze parti
#### Componenti distribuiti e o dipendenze
Vengono di seguito elencati i componenti distribuiti o richiesti con MAppER che hanno una propria licenza diversa da CC0.

- [iText7 (9.3.0)](https://itextpdf.com/) di Apryse Licenza [AGPL-3.0-or-later](https://www.gnu.org/licenses/agpl-3.0.html) 
- [iText7.bouncy-castle-adapter(9.3.0)](https://itextpdf.com/) di Apryse Licenza [AGPL-3.0-or-later](https://www.gnu.org/licenses/agpl-3.0.html)
- [EPPlus (8.2.0)](https://epplussoftware.com/) di EPPlusSoftware Licenza [Poliform non Commercial](https://polyformproject.org/licenses/noncommercial/1.0.0)
- [HtmlSanitizer (9.0.886)](https://github.com/mganss/HtmlSanitizer) Licenza [MIT](https://mit-license.org/)
- [Micosoft .net core 8](https://microsoft.com/) di Microsoft Licenza [MIT](https://mit-license.org/)

#### Componenti utilizzati per la documentazione


#### Licenza
La licenza è GNU Affero General Public License (AGPL) versione 3 e successive (codice SPDX: AGPL-3.0-or-later) ed è visibile sul sito [GNU Affero General Public License](https://www.gnu.org/licenses/agpl-3.0.html) 
Vale solo per PA per riuso da parte di soggetti non PA prestare attenzione alla licenza poliform.
