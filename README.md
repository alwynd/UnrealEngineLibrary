# Query Unreal Engine Library

**Email:** [_alwyn.j.dippenaar@gmail.com_](mailto:_alwyn.j.dippenaar@gmail.com_)

---

## Screenshot:
The Tool UI:

![Screenshot](screenshot.png)

<br/>
It will also show the JSON results, for any matches found:

![Screenshot2](screenshot2.png)

## Introduction

This Windows tool allows querying Unreal Engine projects for asset information, including:

- Names
- Sizes
- Types
- Thumbnails from the [Unreal Engine Marketplace](https://www.unrealengine.com/marketplace/en-US/store) (No actual assets, only thumbnails and meta data)

It uses Powershell with a custom query string that is passed to a script template, for obtaining matching asset json, then displays their thumbnails in the UI.


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

### Query examples:
`$_.AssetPath -match 'spruce' -and $_.AssetType -match 'staticmesh'`<br/>
look for all assets, containing the text 'spruce', where the asset is a static mesh.<br/><br/>
`$_.SizeOnDisk -gt 104857600`<br/>
Look for all assets, where the size on disk, is greater than 100MB<br/><br/>
`$_.AssetPath -match 'slash.*sword' -and $_.AssetType -match 'anim'`<br/>
Look for all assets, where the name regex matches combat and sword, where the asset type is an animation.<br/><br/>
`$_.AssetPath -match 'sword|shield' -and $_.AssetType -match 'anim|sound'`<br/>
Look for all assets, where the name regex matches sword or shield, where the asset type is an animation, or a sound.<br/><br/>
`$_.AssetPath -match 'cave.*ambient|ambient.*cave' -and $_.AssetType -match 'sound'`<br/>
Look for all ambient cave sounds<br/><br/>
`($_.AssetPath -match 'armor' -and $_.AssetType -match 'static|skeletal') -or ($_.AssetPath -match 'combat' -and $_.AssetType -match 'anim')`<br/>
Look for more complex stuff.<br/><br/>

This will find all StaticMeshes in all marketplace projects, where the name matches "spruce" for example.
The query can be as simple, or as complex as you want it to be.

The only fields available to query are:

- **AssetPath**: _string_
  - This is relative to the Content/ directory, of the Unreal Engine Marketplace Project, as specified in the Unreal Engine Project Catalog, at the top of this document
  - It is NOT guaranteed to match the EXACT path of the marketplace project, BUT the name will always be the same
- **SizeOnDisk**: _number_
  - The actual number of bytes on disk, as obtained from Unreal Engine using c++.
- **AssetType**: _string_
  - Unreal Engine dictates these values, inspect the files to find out what they all are
  - Use -match in powershell to use "like" queries, since exact type matching will vary between engine version and who knows what Epic uses for everything. ( not all 500+ projects were extracted from a single engine version, this has been built up over time )


---

## JSON File Structure

Each and every marketplace project, listed in the main Project catalog above, will have 1 and only 1 JSON file, that contains an array of all of it's assets.
<br/>
It will also contain a folder, with each and every assets' 256x256 PNG thumbnail it was able to extract from Unreal, using c++ ( ExtractJSON source and project included ) 

```json
{
  "/Game/Characters/Mannequins/Meshes/SK_Mannequin.SK_Mannequin": {
    "AssetPath": "/Game/Characters/Mannequins/Meshes/SK_Mannequin.SK_Mannequin",
    "SizeOnDisk": 160872,
    "AssetType": "/Script/Engine.Skeleton"
  },
  "/Game/Characters/Mannequins/Animations/Manny/MM_Fall_Loop.MM_Fall_Loop": {
    "AssetPath": "/Game/Characters/Mannequins/Animations/Manny/MM_Fall_Loop.MM_Fall_Loop",
    "SizeOnDisk": 698209,
    "AssetType": "/Script/Engine.AnimSequence"
  }
}
``` 

The combination of the JSON and Thumbnails are used to search and display the results on screen.

---

## Extracting your own project's JSON/Thumbnails

Here are the steps to extract your own project's details:

1. Download and extract this [UE5.3 C++ Project](https://drive.google.com/file/d/1MHiWk5OJZ_mr4_JUtPSTqJgB8ohUYN3u).
2. Import your content into the Content/ folder, or grab the code, and amend the build file target changes.
3. Open the UE5.3 Project. Once in the Editor, select the content folder in the Content Browser.
   1. You can also select any subfolder, and it will only update the thumbnails for assets in that folder ( and subfolders ), but it will always extract all project asset's JSON.
4. Open the console with tilde "`", and enter "ExportAllAssetInformation", and press Enter.
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
- Page results, currently limits to 256 results.
- Allow easy copying of content from UE Library/Repo into an UE project using Azure azcopy
- Include custom copy facilities, such as disk/network.