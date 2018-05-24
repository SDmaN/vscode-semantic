try {
    $runtime = 'win-x64'

    $compillerProjectFolder = './src/language-tools/src/SlangCompiller/'
    $compillerProject = $compillerProjectFolder + 'SlangCompiller.csproj'
    $compillerBuildFolder = $compillerProjectFolder + 'bin/Release/netcoreapp2.0/' + $runtime + '/publish/'

    $extensionFolder = './src/extensions/slang/'
    $compillerExtensionFolder = $extensionFolder + 'out/compiller/'

    dotnet clean ./src/language-tools/
    dotnet publish $compillerProject -c Release -r $runtime

    New-Item -Force -ItemType Directory -Path $compillerExtensionFolder
    Get-ChildItem -Path $compillerBuildFolder | Copy-Item -Destination $compillerExtensionFolder -Force -Recurse -Container

    try {
        Set-Location $extensionFolder
        'export const isDebug = false;' | Out-File ./src/enviroment.ts
        vsce package -o ../../../slang.vsix
    } 
    finally {
        'export const isDebug = true;' | Out-File ./src/enviroment.ts
        Set-Location ../../../
    }
} 
finally {
}