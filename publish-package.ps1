param(
    [string] $apiKey
)

function checkExitCode([string] $message) {
    $exitCode = $LastExitCode
    if ($exitCode -ne 0) {
        Write-Host "Error: $message"
        exit $exitCode
    }
}

# build sln
dotnet build -c Release
checkExitCode "Build failed"

# test sln
dotnet test
checkExitCode "Test failed"

# create package without build
dotnet pack .\src\heitech.blazor.statelite\heitech.blazor.statelite.csproj -c Release

# publish package
dotnet nuget push .\src\heitech.blazor.statelite\bin\Release\heitech.blazor.statelite.1.0.0.nupkg --api-key $apiKey --source https://api.nuget.org/v3/index.json