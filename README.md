# RepairDesk

Programma Windows per registrare clienti, smartphone e riparazioni, conservare lo storico e generare automaticamente una scheda PDF.

## Funzioni incluse

- Anagrafica cliente: nome, cognome, telefono ed email.
- Scelta smartphone a cascata: prima la marca, poi i relativi modelli.
- Marca e modello sempre scrivibili a mano; i nuovi valori vengono salvati nel catalogo.
- Catalogo iniziale con i marchi principali e centinaia di modelli.
- Descrizione libera della riparazione e spunte rapide per gli interventi.
- Accessori consegnati e stato del telefono alla consegna.
- Archivio ricercabile per cliente, telefono, email, IMEI o numero pratica.
- Appuntamento facoltativo con data e ora.
- Calendario delle riparazioni programmate e riprogrammazione al doppio clic.
- Salvataggio senza PDF, modifica, eliminazione e ristampa dall'archivio.
- PDF professionale con firma cliente e operatore.
- Dati del centro assistenza personalizzabili.
- Funzionamento completamente locale, senza account e senza connessione Internet.
- Modalità PC oppure portatile: archivio e PDF possono restare sulla chiavetta.
- Cartella dei PDF selezionabile manualmente dalle impostazioni.

## Come ottenere il programma da GitHub

1. Crea un nuovo repository vuoto su GitHub.
2. Carica **tutto il contenuto di questa cartella**, compresa la cartella `.github`.
3. Apri la scheda **Actions** del repository.
4. Apri **Crea RepairDesk per Windows**.
5. Premi **Run workflow**.
6. Al termine apri l'esecuzione e scarica l'artefatto **RepairDesk-Windows-x64**.
7. Estrai lo ZIP e avvia `RepairDesk.exe`.

Il workflow parte anche automaticamente dopo ogni modifica caricata sul ramo `main` o `master`.

## Dove vengono salvati i dati

- Database: `%LOCALAPPDATA%\RepairDesk\repairdesk.db`
- PDF: `Documenti\RepairDesk\Schede PDF`

Per fare un backup è sufficiente copiare il file `repairdesk.db` mentre il programma è chiuso.

In **modalità portatile** vengono invece create, accanto a `RepairDesk.exe`:

- `Dati\repairdesk.db`
- `PDF\Schede PDF`

Non rimuovere la chiavetta mentre RepairDesk è aperto.

## Primo utilizzo

Apri la scheda **Impostazioni** e inserisci nome, indirizzo, telefono, email e partita IVA del centro assistenza. Questi dati saranno riportati sui PDF.

## Requisiti per sviluppatori

- Windows 10 o 11
- Visual Studio 2022 con workload “Sviluppo desktop .NET”, oppure .NET SDK 8

Per avviare dal codice:

```powershell
dotnet restore RepairDesk.sln
dotnet run --project src/RepairDesk/RepairDesk.csproj
```

## Tecnologie

- C# / .NET 8
- WPF
- SQLite
- QuestPDF Community
