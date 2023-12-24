This allows querying Unreal Engine projects from the Marketplace, using JSON and JQ with Cygwin.
Only works on Windows, and requires Cygwin, with JQ installed to function.

The Includes most of the free marketplace asset meta data ( NO ACTUAL ASSETS, ONLY THUMBNAILS and META DATA )
Use this tool, so find assets, with a thumbnail to see what it looks like, by name/size/asset type

Since it uses JQ, there is no limit as to how the querying/grouping can be done.

You can also use JQ in the command line on the UELibrary/ included folder, BUT the tool includes the ability
to obtain and show the thumbnails along with its meta data on the screen.

TODO:
- The JQ search and display
- Copying the content from your UE Library/Repo into a UE project using Azure azcopy
- - Extend the tool to allow custom copy, such as disk/network.
- Include the code to extract the JSON/Thumbnails from any UE Project ( UE 5.2+ only )