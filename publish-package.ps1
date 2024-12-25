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
dotnet pack .\src\heitech-fluent-cli\heitech-fluent-cli.csproj -c Release

# publish package
dotnet nuget push .\src\heitech-fluent-cli\bin\Release\heitech-fluent-cli.2.2.0.nupkg --api-key $apiKey --source https://api.nuget.org/v3/index.json