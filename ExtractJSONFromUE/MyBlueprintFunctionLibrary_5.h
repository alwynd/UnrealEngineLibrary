// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "MyBlueprintFunctionLibrary_5.generated.h"

#ifndef BOOL_TEXT
#define BOOL_TEXT(x) (x ? TEXT("true") : TEXT("false")) 
#endif

/**
 * 
 */
UCLASS()
class UE51TEST_API UMyBlueprintFunctionLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:
#if WITH_EDITOR	
	// Exports All Assets and catalogs them.
	UFUNCTION(exec, Category = "Common")
	static void ExportAllAssetInformation();

	UFUNCTION(BlueprintCallable, Category = "Common")
	static void ExportThumbnail(FAssetData& AssetData, const FString& OutputPath);

	UFUNCTION(Category = "Common")
	static void FixRedirectors(const FString& Path);
	
	// Moves all assets, to last selected folder.
	UFUNCTION(exec, Category = "Common")
	static void MoveAllAssets(bool ListOnly = false);

	UFUNCTION(Category = "Common")
	static void MoveAssetsByType(const FString& From, const FString& To, const UClass* Class, const FText& ReferenceUpdateSlowTask, bool ListOnly = false);

	UFUNCTION(Category = "Common")
	static void MoveAndSaveAssets(const FString& From, const FString& To, const UClass* Class, const FText& ReferenceUpdateSlowTask, bool ListOnly = false);

	UFUNCTION(Category = "Common")
	static void BatchSaveAssets(const TArray<FAssetData>& AssetsToSave);

	UFUNCTION(Category = "Common")
	static void DeleteFolder(const FString& FolderPath);
	
#endif
};



