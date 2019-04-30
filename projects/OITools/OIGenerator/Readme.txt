Generate the class from the XSD
- "xsd.exe /classes /language:cs /namespace:OIGenerator OpenImageInvoiceInput.xsd"

Compile the DLL
- "csc.exe /target:library XMLOIGenerator.cs OpenImageInvoiceInput.cs Invoice.cs"

Compile sample test code to test DLL
- "csc.exe /target:exe /reference:XMLOIGenerator.dll application.cs"

Package the nuget package
- "nuget.exe pack OIGenerator.nuspec"