param([string]$Server = 'localhost')

$ErrorActionPreference = 'Stop'
$repoDir = Split-Path $PSScriptRoot -Parent
$projectFile = Join-Path $repoDir '241611JalopFinalQuiz2\241611JalopFinalQuiz2.csproj'
$databaseName = 'RavenclawAuthTest_' + [guid]::NewGuid().ToString('N')
$tempDir = Join-Path ([IO.Path]::GetTempPath()) $databaseName
New-Item -ItemType Directory -Path $tempDir | Out-Null
$oldConnection = $env:ConnectionStrings__UniversityDb
$oldEnvironment = $env:ASPNETCORE_ENVIRONMENT
$serverProcess = $null
$databaseCreated = $false
$clients = [Collections.Generic.List[IDisposable]]::new()

function Assert-Check([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "FAILED: $Message" }
    Write-Output "PASS: $Message"
}
function Invoke-TestSql([string]$Query) {
    $result = & sqlcmd -S $Server -E -l 5 -b -d $databaseName -h -1 -W -Q "SET NOCOUNT ON; $Query"
    if ($LASTEXITCODE -ne 0) { throw 'Test database query failed.' }
    return ($result -join '').Trim()
}
function New-TestClient {
    $handler = [Net.Http.HttpClientHandler]::new()
    $handler.AllowAutoRedirect = $false
    $handler.CookieContainer = [Net.CookieContainer]::new()
    $client = [Net.Http.HttpClient]::new($handler)
    $client.Timeout = [TimeSpan]::FromSeconds(20)
    $clients.Add($client)
    return $client
}
function Send-Request($Client, [string]$Path, [hashtable]$Fields = $null) {
    if ($null -eq $Fields) { $response = $Client.GetAsync("$script:baseUrl$Path").GetAwaiter().GetResult() }
    else {
        $data = [Collections.Generic.Dictionary[string,string]]::new()
        foreach ($entry in $Fields.GetEnumerator()) { $data.Add($entry.Key, [string]$entry.Value) }
        $content = [Net.Http.FormUrlEncodedContent]::new($data)
        try { $response = $Client.PostAsync("$script:baseUrl$Path", $content).GetAwaiter().GetResult() }
        finally { $content.Dispose() }
    }
    try {
        return [pscustomobject]@{
            Status = [int]$response.StatusCode
            Body = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            Location = [string]$response.Headers.Location
        }
    } finally { $response.Dispose() }
}
function Submit-Form($Client, [string]$Path, [hashtable]$Fields) {
    $page = Send-Request $Client $Path
    $token = [regex]::Match($page.Body, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')
    if (-not $token.Success) { throw "Missing antiforgery token at $Path" }
    $Fields['__RequestVerificationToken'] = [Net.WebUtility]::HtmlDecode($token.Groups[1].Value)
    return Send-Request $Client $Path $Fields
}
function Student-Fields([string]$Username) {
    return @{ FirstName='Mira'; LastName='Velasco'; MiddleName=''; Birthday=''; Username=$Username;
        Password='Test-Only-Scholar-2026'; ConfirmPassword='Test-Only-Scholar-2026' }
}

try {
    & dotnet build $projectFile --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    $schema = Get-Content (Join-Path $repoDir '241611JalopFinalQuiz2\Database\Schema.sql') -Raw
    $schema.Replace('Ravenclaw University Db', $databaseName) | Set-Content (Join-Path $tempDir 'schema.sql')
    & sqlcmd -S $Server -E -l 5 -b -i (Join-Path $tempDir 'schema.sql')
    if ($LASTEXITCODE -ne 0) { throw 'Could not create the isolated test database.' }
    $databaseCreated = $true
    $listener = [Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback, 0)
    $listener.Start()
    $port = $listener.LocalEndpoint.Port
    $listener.Stop()
    $script:baseUrl = "http://127.0.0.1:$port"
    $env:ConnectionStrings__UniversityDb = "Server=$Server;Database=$databaseName;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    $serverProcess = Start-Process -FilePath 'dotnet' -ArgumentList "run --no-build --no-launch-profile --project `"$projectFile`" --urls $script:baseUrl" -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $tempDir 'server.log') -RedirectStandardError (Join-Path $tempDir 'server-error.log')
    $anonymous = New-TestClient
    $ready = $false
    for ($attempt=0; $attempt -lt 40; $attempt++) {
        try { $ready = (Send-Request $anonymous '/Account/Login').Status -eq 200 } catch { }
        if ($ready) { break }
        if ($serverProcess.HasExited) { throw 'Test server exited; inspect the temporary server logs.' }
        Start-Sleep -Milliseconds 500
    }
    Assert-Check $ready 'Login page is anonymously accessible'
    Assert-Check ((Send-Request $anonymous '/css/site.css').Status -eq 200) 'Login CSS is anonymously accessible'
    Assert-Check ((Send-Request $anonymous '/lib/jquery/dist/jquery.min.js').Status -eq 200) 'Login scripts are anonymously accessible'
    Assert-Check ((Send-Request $anonymous '/').Status -eq 302) 'Anonymous university access redirects to login'
    Assert-Check ((Send-Request $anonymous '/Admin').Status -eq 302) 'Anonymous admin access redirects to login'
    Assert-Check ((Send-Request $anonymous '/Account/Register' (Student-Fields 'csrf.test')).Status -eq 400) 'Registration rejects a missing antiforgery token'

    $future = Student-Fields 'future.test'; $future.Birthday = '2999-01-01'
    $result = Submit-Form $anonymous '/Account/Register' $future
    Assert-Check ($result.Status -eq 200 -and $result.Body.Contains('Birthday cannot be in the future')) 'Future birthday is rejected'
    $mismatch = Student-Fields 'mismatch.test'; $mismatch.ConfirmPassword = 'different-password'
    $result = Submit-Form $anonymous '/Account/Register' $mismatch
    Assert-Check ($result.Status -eq 200) 'Mismatched passwords are rejected'
    $missingName = Student-Fields 'missing.test'; $missingName.FirstName = ' '
    $result = Submit-Form $anonymous '/Account/Register' $missingName
    Assert-Check ($result.Status -eq 200) 'Blank required name is rejected'
    Assert-Check ((Invoke-TestSql 'SELECT COUNT(*) FROM dbo.UserTable;') -eq '0') 'Invalid submissions created no accounts'

    $fields = Student-Fields 'verification.student'; $fields.Role='Admin'; $fields.IsActive='false'
    $result = Submit-Form $anonymous '/Account/Register' $fields
    Assert-Check ($result.Status -eq 302 -and $result.Location -like '*Login*') 'Valid registration redirects to sign in'
    Assert-Check ((Invoke-TestSql "SELECT COUNT(*) FROM dbo.UserTable WHERE Username=N'verification.student' AND Role=N'Student' AND IsActive=1 AND MiddleName IS NULL AND Birthday IS NULL AND PasswordHash<>N'Test-Only-Scholar-2026';") -eq '1') 'Server forces Student/active, hashes the password, and accepts optional fields'
    $duplicate = Submit-Form $anonymous '/Account/Register' (Student-Fields 'VERIFICATION.STUDENT')
    Assert-Check ($duplicate.Status -eq 200 -and $duplicate.Body.Contains('already in use')) 'Case-insensitive duplicate username rejected'

    $profile = Student-Fields 'profile.student'; $profile.MiddleName='Isabel'; $profile.Birthday='2003-04-16'
    $profileResult = Submit-Form $anonymous '/Account/Register' $profile
    Assert-Check ($profileResult.Status -eq 302 -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.UserTable WHERE Username=N'profile.student' AND FirstName=N'Mira' AND LastName=N'Velasco' AND MiddleName=N'Isabel' AND Birthday=CONVERT(date,'20030416',112);") -eq '1') 'Names and a supplied birthday persist correctly'

    $wrong = Submit-Form $anonymous '/Account/Login' @{ Username='verification.student'; Password='wrong-password' }
    Assert-Check ($wrong.Status -eq 200 -and $wrong.Body.Contains('Unable to sign in')) 'Wrong password rejected'
    $login = Submit-Form $anonymous '/Account/Login' @{ Username='VERIFICATION.STUDENT'; Password='Test-Only-Scholar-2026'; ReturnUrl='https://example.invalid/escape' }
    Assert-Check ($login.Status -eq 302 -and $login.Location -notlike 'http*') 'Login succeeds case-insensitively and blocks external return URLs'
    Assert-Check ((Send-Request $anonymous '/').Status -eq 200) 'Student can access the university'
    $denied = Send-Request $anonymous '/Admin'
    Assert-Check ($denied.Status -eq 302 -and $denied.Location -like '*AccessDenied*') 'Student cannot access admin by URL'
    Assert-Check ((Send-Request $anonymous '/Account/AccessDenied').Status -eq 403) 'Access-denied page returns 403'
    Assert-Check ((Send-Request $anonymous '/Account/Logout' @{}).Status -eq 400) 'Logout rejects a missing antiforgery token'
    Assert-Check ((Send-Request $anonymous '/Account/Logout').Status -ne 200) 'Logout cannot be performed with GET'
    $homeResponse = Send-Request $anonymous '/'
    $logoutToken = [regex]::Match($homeResponse.Body, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value
    $logout = Send-Request $anonymous '/Account/Logout' @{ __RequestVerificationToken=$logoutToken }
    Assert-Check ($logout.Status -eq 302 -and (Send-Request $anonymous '/').Status -eq 302) 'Logout removes authenticated access'

    Invoke-TestSql "UPDATE dbo.UserTable SET IsActive=0 WHERE Username=N'verification.student';" | Out-Null
    $inactive = Submit-Form $anonymous '/Account/Login' @{ Username='verification.student'; Password='Test-Only-Scholar-2026' }
    Assert-Check ($inactive.Status -eq 200 -and $inactive.Body.Contains('Unable to sign in')) 'Inactive account cannot sign in'
    Invoke-TestSql "UPDATE dbo.UserTable SET IsActive=1, Role=N'Admin' WHERE Username=N'verification.student';" | Out-Null
    $admin = New-TestClient
    $adminLogin = Submit-Form $admin '/Account/Login' @{ Username='verification.student'; Password='Test-Only-Scholar-2026' }
    Assert-Check ($adminLogin.Status -eq 302 -and (Send-Request $admin '/Admin').Status -eq 200) 'Admin can access administration'
    Invoke-TestSql "UPDATE dbo.UserTable SET Role=N'Student' WHERE Username=N'verification.student';" | Out-Null
    Assert-Check ((Send-Request $admin '/Admin').Status -eq 302) 'Role change invalidates an existing admin session'
    Submit-Form $anonymous '/Account/Login' @{ Username='verification.student'; Password='Test-Only-Scholar-2026' } | Out-Null
    Invoke-TestSql "UPDATE dbo.UserTable SET IsActive=0 WHERE Username=N'verification.student';" | Out-Null
    Assert-Check ((Send-Request $anonymous '/').Status -eq 302) 'Deactivation invalidates an existing student session'
    $throttled = $false
    for ($attempt=0; $attempt -lt 35; $attempt++) {
        if ((Send-Request $anonymous '/Account/Login' @{}).Status -eq 429) { $throttled=$true; break }
    }
    Assert-Check $throttled 'Repeated account POST requests are rate limited'
    Assert-Check ((Send-Request $anonymous '/Account/Login').Status -eq 200) 'Login GET remains available after POST throttling'
    Write-Output 'Authentication verification passed. All mutations were confined to the disposable test database.'
}
finally {
    foreach ($client in $clients) { $client.Dispose() }
    if ($serverProcess -and -not $serverProcess.HasExited) { & taskkill /PID $serverProcess.Id /T /F | Out-Null }
    $env:ConnectionStrings__UniversityDb = $oldConnection
    $env:ASPNETCORE_ENVIRONMENT = $oldEnvironment
    if ($databaseCreated) {
        # The target is this script's GUID-named, newly created test database only.
        & sqlcmd -S $Server -E -l 5 -b -d master -Q "ALTER DATABASE [$databaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$databaseName];"
        if ($LASTEXITCODE -ne 0) { Write-Warning "Could not remove disposable database $databaseName" }
    }
    Write-Output "Verification logs: $tempDir"
}
