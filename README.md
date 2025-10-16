# Exthand.FinanceExports
Exports bank transactions to standard formats like CAMT053, MT940, CSV, CODA, etc.

## Version 1.2.0

Includes a globalization record in CODA and HTML files.
That globalization record is not counted in the balances and is a resume of payments processed using a POS.

Includes new info in CAMT files

## Version 1.3.0

First line of CODA file, specifying the date of the creation file as been replaced by the date of the first transaction 
to avoid compatibility issues with accouting softwares like WinBooks.

## Version 1.4.0

Updated CAMT053 exports to have opening date same as closing date if same day.
