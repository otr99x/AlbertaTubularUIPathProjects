# AlbertaTubularUIPathProjects

## Accounts Payable Process
UiPath projects to connect to Alberta Tubular Products exchange mail server and download PDF invoice
attachments from select companies. These invoices are then read and the relevant invoice fields are
captured and passed to OpenInvoice through an XML API. The OpenInvoice API requires a user defined 
package, OIGenerator to be installed into UiPath.

## Git Folder structure:
AlbertaTubularUIPathProjects - home directory
 - APCompany.csv - Company list file. Required by project files.
 - ./GetAPInvoice - Get email attachments of AP invoices and save into company folders.
 - ./ProcessAPInvoice - Loop through company folders, open pdf files, scrape invoice number, invoice date, GST total and invoice total.
 - ./projects - Additional supporting modules
 - ./projects/OITools - C# project to serialize invoice data into an XML message.
 - ./APConfig.xlsx - Config file for all required variables.
 
## ATP Implemented Folder structure: 
\\\\ATP-CGY-VFS01\\Evraz\\UiPath Projects - home directory
 - ./bin/GetAPInvoice - Get email attachments of AP invoices and save into company folders.
 - ./bin/ProcessAPInvoice - Loop through company folders, open pdf files, scrape invoice number, invoice date, GST total and invoice total.
 - ./dev/OITools - Additional supporting modules
 - ./dev/OITools/Application - Exe to call OIGenerator dll to verify functionality without requiring UiPath process.
 - ./dev/OITools/OIGenerator - dll to serialize invoice data into an XML message.
 - ./lib/APConfig.xlsx - Configuration file for AP Processes.
 - ./lib/APCompany.csv - Company list file. Required by project files.
 - ./tmp/APAttachments - Email attachments saved by company.
 - ./tmp/APAttachments/Company1
 - ./tmp/APAttachments/Company2
 - ./tmp/APAttachments/...

## UiPath Project Notes:

### General:
 - Only the location of the config file is hardcoded in the Initialize Variables activity of each process. 
 - Variables used by the processes are stored in the config file.
 - For security, the config file values in GitHub are blanked out.
 - Ryan, BHD, Rhino and Cintas invoices implemented.
 - UiPath execution logs located at C:\\Users\\\<usename>\\AppData\\Local\\UiPath\\Logs
 
### ProcessAPInvoice:
 - The screenscrape is based on a resolution of 1920x1080 and a maximized Acrobat. **TODO: Rebase resolution to 1280x1024**
 - There is a bug with Acrobat Reader DC 2019 and UiPath where Get Visible Text does not work. The code attempts to read the text, but then failsover to OCR.
 - Cintas, Rhino and Ryans invoices read the entire PDF file and the text fields are retreived using regular expressions.
 - BHD invoices are scanned so have to be parsed using OCR.
 - Invoice data is displayed in Message Box requiring confirmation to upload into OpenInvoice.

## OIGenerator UiPath Dependency:
 - Source code located AlbertaTubularUIPathProjects\projects\OITools\OIGenerator
 - To generate the .cs class from the .xsd:  
   xsd.exe /classes /language:cs /namespace:OIGenerator OpenImageInvoiceInput.xsd
 - To compile the DLL:  
   csc.exe /target:library XMLOIGenerator.cs OpenImageInvoiceInput.cs Invoice.cs
 - To create the nuget package:  
   nuget.exe pack OIGenerator.nuspec  
   **\*** Nuget relies on updated OIGenerator.nuspec configuration.
 - Resulting OIGenerator.0.0.1.nupkg file must be placed in C:\\Users\\\<usename>\\AppData\\Local\\UiPath\\\<UiPath version>\\Packages  
   **\*** Current UiPath versions: Enterprise Edition = 19.10.1 Community Edition = app-19.10.4
 - To install into UiPath, select Manage Packages -> Local -> OIGenerator
 - OIGenerator requires the signed spectrux-private.pfx certificate to be installed in the Personal certificate store of Current User.
 
 ## Git Notes:
 - Git flags .json and .xaml files as binary and will not show differences between versions.
 - The workaround is to edit the Git attributes file in GitProject\\.git\\info\\
 - Edit \*.json binary, \*.xaml binary to \*.json diff, \*.xaml diff
 - Rescan project (F5)