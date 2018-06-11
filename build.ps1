using namespace System.IO

try {
    $runtime = 'win-x64'

    $compillerProjectFolder = './src/language-tools/src/SlangCompiller/'
    $compillerProject = [Path]::Combine($compillerProjectFolder, 'SlangCompiller.csproj')
    $compillerBuildFolder = [Path]::Combine($compillerProjectFolder, 'bin/Release/netcoreapp2.0/', $runtime, 'publish/')
    $mingwFolder = [Path]::Combine($compillerProjectFolder, 'mingw/')

    $extensionFolder = './src/extensions/slang/'
    $compillerExtensionFolder = [Path]::Combine($extensionFolder, 'out/compiller/')
    $mingwExtensionFolder = [Path]::Combine($compillerExtensionFolder, 'mingw/mingw32/')

    dotnet clean ./src/language-tools/
    dotnet publish $compillerProject -c Release -r $runtime

    New-Item -Force -ItemType Directory -Path $compillerExtensionFolder

    Get-ChildItem -Path $compillerBuildFolder | Copy-Item -Destination $compillerExtensionFolder -Force -Recurse -Container -Verbose

    if (-Not [Directory]::Exists($mingwExtensionFolder)) {
        Get-ChildItem -Path $mingwFolder | Copy-Item -Destination $mingwExtensionFolder -Force -Recurse -Container -Verbose   
    }

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