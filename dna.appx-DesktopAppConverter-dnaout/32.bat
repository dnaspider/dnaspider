cd c:\users\dziez\desktop\dnaout\32PackageFiles

del dna.appx

cd C:\Program Files (x86)\Windows Kits\10\bin\x64

makeappx.exe pack -d c:\users\dziez\desktop\dnaout\32PackageFiles -p c:\users\dziez\desktop\dnaout\32PackageFiles\dna.appx


SignTool sign /fd SHA256 /a /f C:\CERT\dnaspider.pfx C:\Users\dziez\Desktop\dnaout\32packagefiles\dna.appx


explorer C:\Users\dziez\Desktop\dnaout\32PackageFiles
