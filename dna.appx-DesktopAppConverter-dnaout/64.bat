cd c:\users\dziez\desktop\dnaout\64PackageFiles

del dna.appx

cd C:\Program Files (x86)\Windows Kits\10\bin\x64

makeappx.exe pack -d c:\users\dziez\desktop\dnaout\64PackageFiles -p c:\users\dziez\desktop\dnaout\64PackageFiles\dna.appx


SignTool sign /fd SHA256 /a /f C:\CERT\dnaspider.pfx C:\Users\dziez\Desktop\dnaout\64PackageFiles\dna.appx


explorer C:\Users\dziez\Desktop\dnaout\64PackageFiles
