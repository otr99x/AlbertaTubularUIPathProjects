# AlbertaTubularUIPathProjects

UiPath projects to connect to Alberta Tubular Products exchange mail server and download PDF invoice
attachments from select companies. These invoices are then read and the relevant invoice fields are
captured and passed to OpenInvoice through an XML API. The OpenInvoice API requires a user defined 
package, OIGenerator to be installed into UiPath.

## Folder structure:
AlbertaTubularUIPathProjects - home directory
 - company.csv - Company list file. Required by project files.
 - ./GetAPInvoice - Get email attachments of AP invoices and save into company folders.
 - ./ProcessAPInvoice - Loop through company folders, open pdf files, scrape invoice number, invoice date, GST total and invoice total.
 - ./projects - Additional supporting modules
 - ./projects/OITools - C# project to serialize invoice data into an XML message.
 
Attachments - Email attachments saved by company.
 - ./Company1
 - ./Company2
 - ./...


## UiPath Project Notes:

### General:
 - All messages logged using Log Message activity.
 - Ryan, BHD, Rhino and Cintas invoices implemented.
 - Using system variable CurrentDirectory (current UiPath project folder) to base relative paths from.
 - All projects require a path to Attachments folder. Currently set relative to the UiPath project's current folder at the same level as the home folder.
 - All projects require a path to Company.CSV file. Currently set relative to the UiPath project's current folder in the parent directory.

### GetAPInvoice:
 - All account information in Get Exchange Mail Messages activity has been blanked out for GitHub repository.

### ProcessAPInvoice:
 - The screenscrape is based on a resolution of 1920x1080 and a maximized Acrobat. **TODO: Rebase resolution to 1280x1024**
 - There is a bug with Acrobat Reader DC 2019 and UiPath where Get Visible Text does not work. The code attempts to read the text, but then failsover to OCR.
 - Cintas invoices place the GST and Invoice totals as line items so the entire PDF is parsed and the text fields are retreived using regular expressions.
 - Rhino invoices place GST total as a line item so the entire PDF is parsed and the text fields are retreived using regular expressions.
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
   **\*** Current UiPath version = app-19.10.1
 - To install into UiPath, select Manage Packages -> Local -> OIGenerator
 - OIGenerator requires the signed spectrux-private.pfx certificate to be installed in the Personal certificate store of Current User.
 
 ## Git Notes:
 
 - Git flags .json and .xaml files as binary and will not show differences between versions.
 - The workaround is to edit the Git attributes file in GitProject\\.git\\info\\
 - Edit \*.json binary, \*.xaml binary to \*.json diff, \*.xaml diff
 - Rescan project (F5)