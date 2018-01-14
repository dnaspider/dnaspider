# dnaspider

![dnaspider logo](https://github.com/dnaspider/dnaspider/blob/v2.2.4.3/dna.appx-DesktopAppConverter-dnaout/64PackageFiles/Assets/dna.44x44.png "dnaspider")
This is a program for the keyboard and can output text, move mouse cursor, and simulate mouse click.  


## Example code

**Output text:**<br>
`«hw-»Hello World.`

**Notes:**<br>
Pressing **Tab** in the text box will print `«»`.<br>
`«hw-»` The minus sign will auto backspace twice.<br>
Press `Ctrl + S` to add `«hw-»Hello World.` code to the list.<br>
Press `Right Ctrl, release, then press H, W` in a text box outside of the program for output.<br>
Same result: `«hw»«bs*2»Hello World.`

**Simulate mouse:**<br>
`«hw-»Hello World.«xy:0-0»«right-click»`

**Notes:**<br>
`«x»` + **Tab** then move mouse cursor over x y location or press **Esc** over location and press **Ctrl + P** in text box to print.<br>
`«rc»` + **Tab** will print «right-click».

**Example extended:**<br>
`«hw-»Hello World.«shift»«left*12»«-shift»`<br>
[...](https://github.com/dnaspider/dnaspider/blob/master/dna.md "read more")

## Installing

[Release](https://github.com/dnaspider/dnaspider/releases "download")

If will be installing Windows 10 dna.appx install, first download and install [dnaspider.cer](https://github.com/dnaspider/dnaspider/releases/download/v2.2.5.1/dnaspider.cer) test certificate otherwise install dna.msi or run dna.exe portable.
See [install](https://github.com/dnaspider/dnaspider/wiki/Install) for more info.

## Source
Visual Studio 2017
