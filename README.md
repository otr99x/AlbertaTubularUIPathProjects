# AlbertaTubularUIPathProjects

## Folder structure:
AlbertaTubularUIPathProjects - home directory
 - company.csv - Company list file. Required by project files.
 - ./GetAPInvoice - Get email attachments of AR invoices and call AlbertaTubularARInvoiceEntry to process.
 - ./ProcessAPInvoice - Loop through company folders, scrape invoice number, invoice date, GST total and invoice total.
 - ./projects - Additional supporting modules
 - ./projects/OITools - C# project to serialize invoice data into MIME multi part SOAP message.
 
Attachments - Email attachments saved by company.
 - ./Company1
 - ./Company2
 - ./...


## UiPath Project Notes:

General:
All messages logged using Log Message activity.
Only Ryan invoices implemented. Other companies can be implemented once Ryan is finalized.
All projects require a path to Attachments folder. Currently set relative to the UiPath project's current folder at the same level as the home folder.
All projects require a path to Company.CSV file. Currently set relative to the UiPath project's current folder in the parent directory.

GetAPInvoice:
 - All account information in Get Exchange Mail Messages activity has been blanked out for GitHub repository.

ProcessAPInvoice:
 - The screenscrape is based on a resolution of 1920x1080 and a maximized Acrobat. **TODO: Rebase resolution to 1280x1024**
 - GST and Invoice totals are not visible when the document of opened so PGDN key is pressed to scroll document to the bottom of the page.
 - There is a messagebox popup at the end of each invoice process. This is for debugging and should be deleted.
 - Invoice data is logged using Log Message activity. This will updated to call DLL functions once SOAP message spec is finialized.
