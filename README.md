# Query Unreal Engine Library

**Email:** [_alwyn.j.dippenaar@gmail.com_](mailto:_alwyn.j.dippenaar@gmail.com_)

---

## Introduction

This Windows tool allows querying Unreal Engine projects for asset information, including:

- Names
- Sizes
- Types
- Thumbnails from the [Unreal Engine Marketplace](https://www.unrealengine.com/marketplace/en-US/store) (No actual assets, only thumbnails and meta data)

It requires Cygwin, with JQ installed to function.

---

## Unreal Engine Project Catalog

The catalog contains the projects and URLs to the marketplace, where you can download/buy them. It lists the meta data of each asset in all marketplace projects.

Access the catalog here: [Google Spreadsheet](https://docs.google.com/spreadsheets/d/1xYUMvSJW7Z9hHA15pBjDcslm1z6FpUTEPxUDqikB0wg)

---

## Asset Catalog

The asset catalog is a ~13GB tar.gz zip file, containing JSON and 256x256 png thumbnails of all assets listed in the projects of the Google Spreadsheet catalog. Due to its size and filename lengths, it is hosted on Google Drive.

Get it here: [Google Drive](https://drive.google.com/file/d/1TSBblfevuoFxVz3LnhNvseGLVarkypuG)

---

## Usage

1. Download and extract the JSON+Thumbnail Catalog first into the `project folder/UELibrary/`. Use `tar -xzvf UELibrary.tar.gz -C UELibrary/` to extract the catalog.
2. Build the project with Dotnet 8 using `dotnet build QueryUELibrary.sln`.
3. Run the executable. This is a Windows Forms application.

---

## Extracting your own project's JSON/Thumbnails

Here are the steps to extract your own project's details:

1. Download and extract this [UE5.3 C++ Project](https://drive.google.com/file/d/1MHiWk5OJZ_mr4_JUtPSTqJgB8ohUYN3u).
2. Import your content into the Content/ folder, or grab the code, and amend the build file target changes.
3. Open the UE5.3 Project. Once in the Editor, select the content folder in the Content Browser.
   1. You can also select any subfolder, and it will only update the thumbnails for assets in that folder ( and subfolders ), but it will always extract all project asset's JSON.
4. Open the console with tilde "`", and enter "ExportAllAssetInfo", and press Enter.
   1. 1st run will take a while, depending on your PC and the project.
   2. **YOU HAVE TO RUN IT TWICE** to ensure all thumbnails get generated. The 2nd run will take only a few seconds. 
5. Once completed, thumbnails and JSON file will be in the root of the extracted `UE5.3` project folder.
6. If you just want the c++ code, look under: [ExtractJSONFromUE](https://github.com/alwynd/UnrealEngineLibrary/tree/main/ExtractJSONFromUE)

**Note:** There can be crashes if the blueprints and/or skeletons/animations and even maps have not been imported correctly. 

If that happens, fix them, or remove them, and run the process again. 
This has been used to extract over 500 Unreal Engine code and blueprint projects, 
so the process does work, but you have to understand Unreal Engine migration in order to use this tool effectively.

---

## Future Work

In the future, this tool aims to:

- Enable JQ search and display
- Allow easy copying of content from UE Library/Repo into an UE project using Azure azcopy
- Include custom copy facilities, such as disk/network.