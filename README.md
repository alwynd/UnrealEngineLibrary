This allows querying Unreal Engine projects from the Marketplace, using JSON and JQ with Cygwin.
Only works on Windows, and requires Cygwin, with JQ installed to function.

The Includes most of the free marketplace asset meta data ( NO ACTUAL ASSETS, ONLY THUMBNAILS and META DATA )
Use this tool, so find assets, with a thumbnail to see what it looks like, by name/size/asset type

Since it uses JQ, there is no limit as to how the querying/grouping can be done.

You can also use JQ in the command line on the UELibrary/ included folder, BUT the tool includes the ability
to obtain and show the thumbnails along with its meta data on the screen.

Unreal Engine Project Catalog: https://docs.google.com/spreadsheets/d/1xYUMvSJW7Z9hHA15pBjDcslm1z6FpUTEPxUDqikB0wg
- This contains the projects, and URL's to the marketplace, to download/buy the projects if you require them,
**as this project only lists meta data of each and every asset in all of the marketplace projects.**
 <br/>
 <br/>

Asset Catalog ( ~13GB tar.gz JSON and 256x256 png thumbnails of all assets listed in the projects from the main catalog ): https://drive.google.com/file/d/1TSBblfevuoFxVz3LnhNvseGLVarkypuG<br/>
_catalog tarball too big for git lfs, and filenames too long for github, so I keep them in google drive_
<br/>
<br/>
Usage: Download and extract the JSON+Thumbnail Catalog 1st into the **'project folder'/UELibrary/**
- **tar -xzvf UELibrary.tar.gz -C UELibrary/**
- Build the project with Dotnet 8
- - **dotnet build QueryUELibrary.sln**
- - Run the executable, it is a Windows Forms app.

<br/>

**How to extract your own project's JSON/Thumbnails**
Download and extract this UE5.3 C++ ( Project name says 5.1, but it was built for 5.3, it is a c++ project )
https://drive.google.com/file/d/1MHiWk5OJZ_mr4_JUtPSTqJgB8ohUYN3u/view?usp=drive_link

**Import your content into the Content/ folder, or grab the code, and the build file target changes etc.**

Open the UE51Test project, while in the EDITOR, open the CONSOLE with tilde "`" and enter
"ExportAllAssetInfo", and hit enter.
<br/>
<br/>
_Sometimes it can crash if the blueprints have not been imported correctly, if that is the case, fix them, or remove them, and run it again
you may need to run it twice, as the thumbnails dont always export the 1st time, not sure why and dont really care at this point
after doing it for over 500 projects I can live with it, 1st run can take long depending on your specs, 2nd run will take a few seconds._

<br/>
<br/>

**_TODO:_**
- The JQ search and display
- Copying the content from your UE Library/Repo into a UE project using Azure azcopy
- - Extend the tool to allow custom copy, such as disk/network.
