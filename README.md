# GTA 3 Mod Manager

Un program simplu pentru instalarea si gestionarea modurilor pentru GTA 3.

## Functionalitati

- Detectare automata a locatiei jocului GTA 3
- Instalare usoara a modurilor din fisiere .zip
- Dezinstalare moduri
- Crearea copiilor de siguranta pentru fisierele jocului
- Interfata usoara de utilizat
- Lansarea jocului direct din aplicatie
- Backup si restaurare completa a jocului
- Gestionarea ordinii modurilor (prioritizare)
- Scanare automata pentru a gasi instalari ale jocului
- Optiuni configurabile (backup automat, minimizare la lansare)
- Salvarea setarilor intre sesiuni

## Cerinte

- Windows 7/8/10/11
- .NET Framework 4.7.2 sau mai nou
- GTA 3 instalat

## Utilizare

1. Descarcati si rulati programul
2. Programul va incerca sa gaseasca automat instalarea GTA 3
3. Daca nu o gaseste automat, puteti:
   - Apasati butonul "Browse..." pentru a selecta manual folderul de instalare GTA 3
   - Sau apasati butonul "Settings" si apoi "Scan for additional install locations" pentru a cauta pe toate discurile
4. Pentru a instala un mod:
   - Apasati butonul "Install New Mod"
   - Selectati fisierul .zip al modului
   - Asteptati finalizarea instalarii
5. Pentru a dezinstala un mod:
   - Selectati modul din lista
   - Apasati butonul "Uninstall Selected Mod"
6. Pentru a schimba ordinea modurilor:
   - Selectati modul din lista
   - Utilizați butoanele săgeți ↑↓ pentru a schimba prioritatea
7. Pentru a crea un backup complet:
   - Apasati butonul "Create Backup"
   - Selectati locatia unde doriti sa salvati backup-ul
8. Pentru a restaura un backup:
   - Apasati butonul "Restore Backup"
   - Selectati fisierul de backup
9. Pentru a lansa jocul:
   - Apasati butonul "Launch GTA 3"

## Setari

Pentru a configura programul, apasati butonul "Settings":
- Puteti activa/dezactiva backup-ul automat inainte de instalarea modurilor
- Puteti activa/dezactiva minimizarea automata a aplicatiei la lansarea jocului
- Puteti scana pentru a gasi instalari ale jocului

## Note importante

- Programul creeaza automat copii de siguranta pentru fisierele importante ale jocului
- Instalati modurile unul cate unul pentru a evita conflictele
- Este posibil sa fie necesare drepturi de administrator pentru anumite operatiuni
- Ordinea modurilor este importanta - modurile de la baza listei suprascriu pe cele de deasupra

## Compilare

Pentru a compila proiectul, aveti nevoie de Visual Studio cu suport pentru dezvoltare .NET Desktop. Deschideti fisierul solutie (.sln) si compilati proiectul. 