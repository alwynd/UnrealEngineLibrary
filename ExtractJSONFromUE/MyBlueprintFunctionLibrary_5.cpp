// Fill out your copyright notice in the Description page of Project Settings.


#include "MyBlueprintFunctionLibrary_5.h"

#include "AssetToolsModule.h"
#include "FileHelpers.h"
#include "IAssetTools.h"


#if WITH_EDITOR

#include "ContentBrowserModule.h"
#include "IContentBrowserSingleton.h"
#include "Misc/FeedbackContext.h"
#include "IImageWrapper.h"
#include "IImageWrapperModule.h"
#include "ObjectTools.h"
#include "AssetRegistry/AssetRegistryModule.h"
#include "Misc/FileHelper.h"
#include "Dom/JsonObject.h"
#include "Serialization/JsonWriter.h"
#include "Serialization/JsonSerializer.h"


static void MyExportAllAssetInformation()
{
	// Call your BlueprintFunctionLibrary function here
	UMyBlueprintFunctionLibrary::ExportAllAssetInformation();

	// Log to the console to confirm that the function was called
	if (GEngine)
	{
		GEngine->AddOnScreenDebugMessage(-1, 30.f, FColor::Green, *FString::Printf(TEXT("ExportAllAssetInformation!, ProjectName: '%s', ProjectDir: '%s', Leaf: '%s'"), FApp::GetProjectName(), *FPaths::ProjectDir(), *FPaths::GetPathLeaf(FPaths::ProjectDir())));
	}
}

static FAutoConsoleCommand ExportAllAssetInformationCommand(
	TEXT("ExportAllAssetInformation"), 
	TEXT("ExportAllAssetInformation"),
	FConsoleCommandDelegate::CreateStatic(&MyExportAllAssetInformation)
);

static void MyMoveAllAssets(const TArray<FString>& Args)
{
	bool listOnly = false;
	if (Args.Num() > 0)
	{
		if (Args[0].Equals(TEXT("true"), ESearchCase::IgnoreCase)) listOnly = true;
	}
	
	// Call your BlueprintFunctionLibrary function here
	UMyBlueprintFunctionLibrary::MoveAllAssets(listOnly);

	// Log to the console to confirm that the function was called
	if (GEngine)
	{
		GEngine->AddOnScreenDebugMessage(-1, 30.f, FColor::Green, *FString::Printf(TEXT("MyMoveAllAssets!, ProjectName: '%s', ProjectDir: '%s', Leaf: '%s'"), FApp::GetProjectName(), *FPaths::ProjectDir(), *FPaths::GetPathLeaf(FPaths::ProjectDir())));
	}
}

static FAutoConsoleCommand MoveAllAssetsCommand(
	TEXT("MoveAllAssets"), 
	TEXT("MoveAllAssets"),
	FConsoleCommandWithArgsDelegate::CreateStatic(&MyMoveAllAssets)
);


void UMyBlueprintFunctionLibrary::ExportThumbnail(FAssetData& AssetData, const FString& OutputPath)
{
	UObject* MyObject = AssetData.GetAsset();
	if (MyObject)
	{
		FObjectThumbnail* ObjectThumbnail = ThumbnailTools::GenerateThumbnailForObjectToSaveToDisk(MyObject);
		if (ObjectThumbnail)
		{
			IImageWrapperModule& ImageWrapperModule = FModuleManager::Get().LoadModuleChecked<IImageWrapperModule>(TEXT("ImageWrapper"));
			TSharedPtr<IImageWrapper> ImageWrapper = ImageWrapperModule.CreateImageWrapper(EImageFormat::PNG);
			ImageWrapper->SetRaw(ObjectThumbnail->GetUncompressedImageData().GetData(), ObjectThumbnail->GetUncompressedImageData().Num(), ObjectThumbnail->GetImageWidth(), ObjectThumbnail->GetImageHeight(), ERGBFormat::BGRA, 8);
			if (ImageWrapper)
			{
				const TArray64<uint8>& CompressedByteArray = ImageWrapper->GetCompressed();
				FFileHelper::SaveArrayToFile(CompressedByteArray, *OutputPath);
			}
		}
	}
}

void UMyBlueprintFunctionLibrary::FixRedirectors(const FString& Path)
{
	UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets.FixRedirectors Path: %s"), *Path);	
	IAssetTools& AssetTools = FModuleManager::GetModuleChecked<FAssetToolsModule>("AssetTools").Get();
	FAssetRegistryModule& AssetRegistryModule = FModuleManager::LoadModuleChecked<FAssetRegistryModule>("AssetRegistry");
	
	TArray<FAssetData> Redirectors;
	AssetRegistryModule.Get().GetAssetsByPath(*Path, Redirectors, true /*bRecursive*/);

	TArray<UObjectRedirector*> RedirectorObjects;
	for (FAssetData& Redirector : Redirectors)
	{
		if (Redirector.AssetClassPath == UObjectRedirector::StaticClass()->GetClassPathName())
		{
			UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets.FixRedirectors Found a Redirector: '%s'"), *Redirector.GetObjectPathString());	
			UObjectRedirector* redirector = static_cast<UObjectRedirector*>(Redirector.GetAsset());
			RedirectorObjects.Add(redirector);
		}
	}

	// Fix up redirectors
	UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets.FixRedirectors Found %d Redirectors"), RedirectorObjects.Num());	
	if (RedirectorObjects.Num() > 0)
	{
		AssetTools.FixupReferencers(RedirectorObjects);
	}		
}

void UMyBlueprintFunctionLibrary::ExportAllAssetInformation()
{
	FString projectName = FApp::GetProjectName();
	const FText ReferenceUpdateSlowTask = FText::FromString(FString::Printf(TEXT("ExportAllAssetInformation.. projectName: '%s'"), *projectName));
	GWarn->BeginSlowTask(ReferenceUpdateSlowTask, true);

	// Get reference to the Asset Registry
	FAssetRegistryModule& AssetRegistryModule = FModuleManager::LoadModuleChecked<FAssetRegistryModule>("AssetRegistry");

	// Array of all assets
	TArray<FAssetData> AssetData;
	AssetRegistryModule.Get().GetAllAssets(AssetData);

	// Create a JSON object to hold all asset data
	TSharedPtr<FJsonObject> JsonObject = MakeShareable(new FJsonObject);
	int num = AssetData.Num();
	int idx = 0;
	
	IContentBrowserSingleton& ContentBrowserSingleton = FModuleManager::LoadModuleChecked<FContentBrowserModule>("ContentBrowser").Get();

	TArray<FString> SelectedPaths;
	ContentBrowserSingleton.GetSelectedPathViewFolders(SelectedPaths);

	// Now SelectedPaths contains the selected paths
	for (const FString& Path : SelectedPaths)
	{
		UE_LOG(LogTemp, Warning, TEXT("Selected Path: %s"), *Path);
	}
	
	for (auto& Asset : AssetData)
	{
		GWarn->StatusUpdate(idx, num,  ReferenceUpdateSlowTask);
		bool process = false;
		
		// Skip any assets that aren't in the Game folder
		if (!Asset.PackageName.ToString().StartsWith("/Game"))
		{
			continue;
		}
		for (const FString& Path : SelectedPaths)
		{
			if (Asset.PackageName.ToString().StartsWith(Path.RightChop(4)))
			{
				process = true;
				break; //for
			}
		}

		if (process)
		{
			FString thumbnailPath = FString::Printf(TEXT("%s/%s_thumbnails/%s.png"), *FPaths::ProjectDir(), *FPaths::GetPathLeaf(FPaths::ProjectDir()), *Asset.GetSoftObjectPath().ToString());
			if (GEngine)
			{
				GEngine->AddOnScreenDebugMessage(-1, 30.f, FColor::Green, *FString::Printf(TEXT("ExportAllAssetInformation Thumbnail: '%s'"), *thumbnailPath));
			}
			
			ExportThumbnail(Asset, thumbnailPath);
			//break;
		}
		
		// Create a JSON object for this asset
		TSharedPtr<FJsonObject> AssetObject = MakeShareable(new FJsonObject);

		
		// Populate the JSON object with asset data
		AssetObject->SetStringField("AssetPath", Asset.GetSoftObjectPath().ToString());
		AssetObject->SetNumberField("SizeOnDisk", Asset.GetPackage()->GetFileSize());
		AssetObject->SetStringField("AssetType", Asset.AssetClassPath.ToString());

		// Add this asset's JSON object to the main JSON object
		JsonObject->SetObjectField(Asset.GetSoftObjectPath().ToString(), AssetObject);
		idx++;
	}

	// Serialize JSON object to string
	FString OutputString;
	TSharedRef<TJsonWriter<>> Writer = TJsonWriterFactory<>::Create(&OutputString);
	FJsonSerializer::Serialize(JsonObject.ToSharedRef(), Writer);

	FFileHelper::SaveStringToFile(OutputString, *FString::Printf(TEXT("%s/%s_AssetCatalog.json"), *FPaths::ProjectDir(), *FPaths::GetPathLeaf(FPaths::ProjectDir())));
	GWarn->EndSlowTask();
}


void UMyBlueprintFunctionLibrary::MoveAllAssets(bool ListOnly)
{
	UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets:-- START, ListOnly: %s"), BOOL_TEXT(ListOnly));
	const FText ReferenceUpdateSlowTask = FText::FromString(TEXT("MoveAllAssets.. "));
	GWarn->BeginSlowTask(ReferenceUpdateSlowTask, true);
	
	
	// Get reference to the Asset Registry
	IContentBrowserSingleton& ContentBrowserSingleton = FModuleManager::LoadModuleChecked<FContentBrowserModule>("ContentBrowser").Get();
	FAssetRegistryModule& AssetRegistryModule = FModuleManager::LoadModuleChecked<FAssetRegistryModule>("AssetRegistry");
        
	TArray<FString> SelectedPaths;
	ContentBrowserSingleton.GetSelectedPathViewFolders(SelectedPaths);
	if (SelectedPaths.Num() < 2)
	{
		UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets NO PATHS SELECTED., Select path to move from 1st, then path to move to last."));
		if (GEngine)
		{
			GEngine->AddOnScreenDebugMessage(-1, 30.f, FColor::Green, *FString::Printf(TEXT("MoveAllAssets NO PATHS SELECTED.")));
		}
		return;
	}
	
	// Now SelectedPaths contains the selected paths
	FString DestinationPath = SelectedPaths[SelectedPaths.Num()-1];
	DestinationPath.RemoveFromStart("/All");
	if (!DestinationPath.EndsWith("/")) {
		DestinationPath += "/";
	}	
	
	UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets DestinationPath: '%s', ParentDestinationPath: '%s'"), *DestinationPath);
	for (int i=0; i<SelectedPaths.Num()-1; i++)
	{
		FString& Path = SelectedPaths[i];
		Path.RemoveFromStart("/All");
		if (!Path.EndsWith("/")) {
			Path += "/";
		}	

		FString destFolder = Path;
		destFolder.RemoveFromStart("/Game/");
		FString dest = FString::Printf(TEXT("%s%s"), *DestinationPath, *destFolder);
		
		// find and move all textures
		// find and move all material functions
		// find and move all materials
		// find and move all static meshes
		// find and move all skeletal meshes
		// find and move all animations
		// find and move all blueprints
		// find and move all remaining assets
		UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets ListOnly: %s, From: %s, To: '%s'"), BOOL_TEXT(ListOnly), *Path, *dest);
		
		MoveAndSaveAssets(Path, dest, UTexture::StaticClass(), ReferenceUpdateSlowTask, ListOnly);
		MoveAndSaveAssets(Path, dest, UMaterialFunctionInterface::StaticClass(), ReferenceUpdateSlowTask, ListOnly);
		MoveAndSaveAssets(Path, dest, UMaterialInterface::StaticClass(), ReferenceUpdateSlowTask, ListOnly);
		MoveAndSaveAssets(Path, dest, UStaticMesh::StaticClass(), ReferenceUpdateSlowTask, ListOnly);
		MoveAndSaveAssets(Path, dest, USkinnedAsset::StaticClass(), ReferenceUpdateSlowTask, ListOnly);
		MoveAndSaveAssets(Path, dest, UAnimationAsset::StaticClass(), ReferenceUpdateSlowTask, ListOnly);
		MoveAndSaveAssets(Path, dest, UBlueprintCore::StaticClass(), ReferenceUpdateSlowTask, ListOnly);
		MoveAndSaveAssets(Path, dest, UObject::StaticClass(), ReferenceUpdateSlowTask, ListOnly);
	}

	if (!ListOnly)
	{
		FARFilter Filter;
		Filter.PackagePaths.Add(FName(*DestinationPath));
		Filter.bRecursivePaths = true;

		TArray<FAssetData> AssetData;
		AssetRegistryModule.Get().GetAssets(Filter, AssetData);
		BatchSaveAssets(AssetData);

		for (int i=0; i<SelectedPaths.Num()-1; i++)
		{
			FString& Path = SelectedPaths[i];
			DeleteFolder(Path);
		}
	}
	
	GWarn->EndSlowTask();
}

void UMyBlueprintFunctionLibrary::MoveAndSaveAssets(const FString& From, const FString& To,	const UClass* Class, const FText& ReferenceUpdateSlowTask, bool ListOnly)
{
	// move
	MoveAssetsByType(From, To, Class, ReferenceUpdateSlowTask, ListOnly);

	// fix redirectors.
	FixRedirectors(From);
	FixRedirectors(To);

	FEditorFileUtils::SaveDirtyPackages(true, true, true);	
}

void UMyBlueprintFunctionLibrary::DeleteFolder(const FString& FolderPath)
{
	// Get a reference to the file manager
	IFileManager& FileManager = IFileManager::Get();

	FString Path = FolderPath;
	Path.RemoveFromStart(TEXT("/All"));
	Path.RemoveFromStart(TEXT("/Game"));
	FString ContentDir = FPaths::ProjectContentDir(); // Get the "Content" directory path
	FString AbsoluteFolderPath = ContentDir + "/" + Path; // Combine the Content directory with the asset path
	AbsoluteFolderPath = FPaths::ConvertRelativePathToFull(AbsoluteFolderPath).Replace(TEXT("//"), TEXT("/"));

	bool exists = FileManager.DirectoryExists(*AbsoluteFolderPath); 
	UE_LOG(LogTemp, Warning, TEXT("DeleteFolder FolderPath: '%s', AbsoluteFolderPath: '%s', exists: %s"), *Path, *AbsoluteFolderPath, BOOL_TEXT(exists));

	// Check if the directory exists
	if (FileManager.DirectoryExists(*AbsoluteFolderPath))
	{
		// Delete the directory and its contents
		const bool bRequireExists = false;  // Do not require the folder to exist (avoids an error if the folder is already gone)
		const bool bTree = true;            // Remove the folder and all of its contents
		const bool bResult = FileManager.DeleteDirectory(*AbsoluteFolderPath, bRequireExists, bTree);

		if (!bResult)
		{
			// The folder could not be deleted; handle the error
			// For example, log an error message or show a notification to the user
			UE_LOG(LogTemp, Error, TEXT("Failed to delete directory at path: %s"), *AbsoluteFolderPath);
		}
	}
	else
	{
		// The directory doesn't exist; handle this case as needed
		UE_LOG(LogTemp, Warning, TEXT("Directory does not exist: %s"), *AbsoluteFolderPath);
	}
}


void UMyBlueprintFunctionLibrary::BatchSaveAssets(const TArray<FAssetData>& AssetsToSave)
{
	UE_LOG(LogTemp, Warning, TEXT("BatchSaveAssets, AssetsToSave: %d"), AssetsToSave.Num());
	TArray<UPackage*> PackagesToSave;

	for (const FAssetData& Asset : AssetsToSave)
	{
		FString PackageName = FPackageName::ObjectPathToPackageName(Asset.GetObjectPathString());
		UPackage* Package = FindPackage(nullptr, *PackageName);

		if (Package)
		{
			// Optional: Mark the package dirty if it's not already
			if (!Package->IsDirty())
			{
				Package->SetDirtyFlag(true);
			}
			PackagesToSave.Add(Package);
		}
	}

	if (PackagesToSave.Num() > 0)
	{
		// Save all the dirty packages to disk
		FEditorFileUtils::PromptForCheckoutAndSave(PackagesToSave, /*bCheckDirty*/ true, /*bPromptToSave*/ false);
	}
}


void UMyBlueprintFunctionLibrary::MoveAssetsByType(const FString& From, const FString& To, const UClass* Class, const FText& ReferenceUpdateSlowTask, bool ListOnly)
{
	FAssetRegistryModule& AssetRegistryModule = FModuleManager::LoadModuleChecked<FAssetRegistryModule>("AssetRegistry");
	UE_LOG(LogTemp, Warning, TEXT("MoveAssetsByType, Class: '%s', From: %s, To: '%s'"), *Class->GetName(), *From, *To);

	FARFilter Filter;
	Filter.PackagePaths.Add(FName(*From));
	Filter.ClassPaths.Add(Class->GetClassPathName());
	Filter.bRecursiveClasses = true;
	Filter.bRecursivePaths = true;

	TArray<FAssetRenameData> renames;
	TArray<FAssetData> AssetData;
	AssetRegistryModule.Get().GetAssets(Filter, AssetData);

	IAssetTools& AssetTools = FModuleManager::LoadModuleChecked<FAssetToolsModule>("AssetTools").Get();
	
	UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets ListOnly: %s, Found %d %s"), BOOL_TEXT(ListOnly), AssetData.Num(), *Class->GetName());
	int idx = 0;
	int num = AssetData.Num();
	for (const FAssetData& asset : AssetData)
	{
		GWarn->StatusUpdate(idx, num,  ReferenceUpdateSlowTask); idx++;

		// Extract the asset's existing path and name.
		FString assetPath = FPaths::GetPath(asset.GetObjectPathString());
		if (!assetPath.EndsWith("/")) {
			assetPath += "/";
		}	
		
		FString assetName = FPaths::GetBaseFilename(asset.GetFullName(), true);

		UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets '%s' assetPath: '%s', assetName: '%s'"), *Class->GetName(), *assetPath, *assetName);
		

		// Construct the new path by replacing the old base folder with the new one.
		assetPath.RemoveFromStart(From);
		assetPath.RemoveFromEnd(assetName);
		FString destPath = To + assetPath;		
		UE_LOG(LogTemp, Warning, TEXT("MoveAllAssets '%s' Asset From: '%s', To: '%s'"), *Class->GetName(), *asset.GetObjectPathString(), *destPath);

		FAssetRenameData renameData(asset.GetAsset(), destPath, assetName);
		renames.Add(renameData);
	}

	if (!ListOnly) AssetTools.RenameAssets(renames);		
	
}



#endif

