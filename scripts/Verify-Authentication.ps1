param([string]$Server = 'localhost', [switch]$IncludeContent, [switch]$Preview)

$ErrorActionPreference = 'Stop'
$repoDir = Split-Path $PSScriptRoot -Parent
$projectFile = Join-Path $repoDir '241611JalopFinalQuiz2\241611JalopFinalQuiz2.csproj'
$databaseName = 'RavenclawAuthTest_' + [guid]::NewGuid().ToString('N')
$tempDir = Join-Path ([IO.Path]::GetTempPath()) $databaseName
New-Item -ItemType Directory -Path $tempDir | Out-Null
$oldConnection = $env:ConnectionStrings__UniversityDb
$oldEnvironment = $env:ASPNETCORE_ENVIRONMENT
$oldUploadRoot = $env:ImageUploads__RootPath
$script:imageBytes = [Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAusB9Wl6pT8AAAAASUVORK5CYII=')
$serverProcess = $null
$databaseCreated = $false
$clients = [Collections.Generic.List[IDisposable]]::new()
$script:passedChecks = 0

function Assert-Check([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "FAILED: $Message" }
    $script:passedChecks++
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
        if ($Fields.ContainsKey('__ImageBytes')) {
            $content = [Net.Http.MultipartFormDataContent]::new()
            foreach ($entry in $Fields.GetEnumerator()) {
                if ($entry.Key -notin @('__ImageBytes','__ImageName')) { $content.Add([Net.Http.StringContent]::new([string]$entry.Value), $entry.Key) }
            }
            $upload = [Net.Http.ByteArrayContent]::new([byte[]]$Fields.__ImageBytes)
            $content.Add($upload, 'ImageUpload', $(if ($Fields.ContainsKey('__ImageName')) { $Fields.__ImageName } else { 'verification.png' }))
        } else {
            $data = [Collections.Generic.Dictionary[string,string]]::new()
            foreach ($entry in $Fields.GetEnumerator()) { $data.Add($entry.Key, [string]$entry.Value) }
            $content = [Net.Http.FormUrlEncodedContent]::new($data)
        }
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
    for ($redirect=0; $redirect -lt 3 -and $page.Status -eq 302 -and $page.Location.StartsWith('/'); $redirect++) {
        $page = Send-Request $Client $page.Location
    }
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
    $testOutput = Join-Path $tempDir 'app'
    & dotnet build $projectFile --no-restore --output $testOutput
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
    $env:ImageUploads__RootPath = Join-Path $tempDir 'uploads'
    $testAssembly = Join-Path $testOutput '241611JalopFinalQuiz2.dll'
    $serverProcess = Start-Process -FilePath 'dotnet' -ArgumentList "`"$testAssembly`" --urls $script:baseUrl" -WorkingDirectory (Split-Path $projectFile -Parent) -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $tempDir 'server.log') -RedirectStandardError (Join-Path $tempDir 'server-error.log')
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
    if ($IncludeContent) {
        Assert-Check (-not (Send-Request $anonymous '/').Body.Contains('admin-sidebar')) 'Student university page keeps its own layout'
        Assert-Check ((Send-Request $anonymous '/AdminContent/Index?section=Programs').Location -like '*AccessDenied*') 'Student cannot access content editors'
        Assert-Check ((Send-Request $anonymous '/AcademicPrograms' @{SubjectName='Tampered'}).Location -like '*AccessDenied*') 'Student cannot POST content changes'
        Assert-Check ((Invoke-TestSql 'SELECT COUNT(*) FROM dbo.ProgramsTable;') -eq '0') 'Rejected student request changed no content'
    }
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
    if ($IncludeContent) {
        $adminDashboard = Send-Request $admin '/Admin'
        Assert-Check ($adminDashboard.Body.Contains('aria-label="Administration navigation"') -and -not $adminDashboard.Body.Contains('aria-label="Main navigation"')) 'Admin dashboard uses a separate sidebar layout'
        $programList = Send-Request $admin '/AcademicPrograms'
        $activeLinks = [regex]::Matches($programList.Body,'<a\b[^>]*aria-current="page"[^>]*>.*?</a>',[Text.RegularExpressions.RegexOptions]::Singleline)
        Assert-Check ($activeLinks.Count -eq 1 -and $activeLinks[0].Value.Contains('Academics')) 'Content sidebar marks only the current section active'
        $universityPage = Send-Request $admin '/UniversityInformation'
        Assert-Check ($universityPage.Body.Contains('name="AboutHeading"') -and $universityPage.Body.Contains('name="CoreValuesText"')) 'Hero and About includes the heading and combined core-value editor'
        $nav = [regex]::Match($universityPage.Body, '<nav aria-label="Administration navigation">(.*?)</nav>', [Text.RegularExpressions.RegexOptions]::Singleline).Groups[1].Value
        $navNames = [regex]::Matches($nav, '<a\b[^>]*>(.*?)</a>', [Text.RegularExpressions.RegexOptions]::Singleline) | ForEach-Object { [Net.WebUtility]::HtmlDecode($_.Groups[1].Value.Trim()) }
        Assert-Check (($navNames -join '|') -eq 'Dashboard|Hero and About|Academics|Founder|Contacts|Account Management') 'Sidebar shows Dashboard first, followed by the five content sections'
        Assert-Check ($universityPage.Status -eq 200 -and $universityPage.Body.Contains('name="ImageUpload"') -and $universityPage.Body.Contains('type="file"') -and $universityPage.Body.Contains('name="AcademyDescription"') -and $universityPage.Body.Contains('name="Mission"') -and $universityPage.Body.Contains('name="Vision"')) 'University page displays its image uploader and three text fields'
        Assert-Check (-not $adminDashboard.Body.Contains('admin-nav-number') -and -not $adminDashboard.Body.Contains('admin-nav-heading') -and -not $adminDashboard.Body.Contains('class="admin-topbar"')) 'Admin sidebar has no numbers/divider and the admin topbar is removed'
        Assert-Check ((Send-Request $admin '/AdminContent?section=University').Location -like '/UniversityInformation*') 'Existing University information link redirects to its dedicated editor'
        $sections = @(
            @{Name='University';Controller='UniversityInformation';Table='AcademyInfoTable';Key='AcademyInfoId';Fields=@{AboutHeading='Verification heading';AcademyDescription='Verification university description';Mission='Verification mission';Vision='Verification vision';HeroImg='/images/branding/ravenclaw-university-crest.png'}},
            @{Name='Contacts';Controller='ContactInformation';Table='ContactsTable';Key='ContactId';Fields=@{AcademyAddress='Verification campus';Email='office@university.example';Telephone='+63 912 345 6789';TimeIn='08:30';TimeOut='17:15'}},
            @{Name='Values';Controller='CoreValues';Table='CoreValuesTable';Key='CoreValueId';Fields=@{Title='Verification title'}},
            @{Name='Photos';Controller='AboutGallery';Table='AboutImagesTable';Key='AboutImageId';Fields=@{ImagePath='/images/branding/ravenclaw-university-crest.png';AltText='Verification image'}},
            @{Name='Programs';Controller='AcademicPrograms';Table='ProgramsTable';Key='SubjectId';Fields=@{SubjectName='Verification title';Description='Verification description';ImagePath='/images/branding/ravenclaw-university-crest.png'}},
            @{Name='Founders';Controller='FounderInformation';Table='FounderTable';Key='FounderId';Fields=@{FounderName='Verification title';Birthday='1900-01-01';Description='Verification description';FounderMessage='Verification title'}},
            @{Name='SocialLinks';Controller='SocialLinks';Table='SocialLinksTable';Key='SocialLinkId';Fields=@{PlatformName='Verification title';Url='https://university.example';IconClass='fa-solid fa-globe'}}
        )
        foreach ($contentSection in $sections) {
            $name=$contentSection.Name; $route='/'+$contentSection.Controller; $fields=$contentSection.Fields.Clone()
            if ($name -in @('University','Photos','Programs')) { $fields.__ImageBytes=$script:imageBytes }
            Assert-Check ((Send-Request $anonymous $route).Location -like '*Login*') "Anonymous cannot access $name editor"
            $studentClient = New-TestClient
            $studentLogin = Submit-Form $studentClient '/Account/Login' @{Username='profile.student';Password='Test-Only-Scholar-2026'}
            Assert-Check ((Send-Request $studentClient $route).Location -like '*AccessDenied*') "Student cannot access $name editor"
            Assert-Check ((Send-Request $studentClient $route @{}).Location -like '*AccessDenied*') "Student cannot POST $name editor"
            $created = Submit-Form $admin $route $fields
            $recordId = Invoke-TestSql "SELECT TOP(1) [$($contentSection.Key)] FROM dbo.[$($contentSection.Table)] ORDER BY [$($contentSection.Key)] DESC;"
            Assert-Check ($created.Status -eq 302 -and [int]$recordId -gt 0) "Admin can create $name content"
            if ($name -in @('University','Photos','Programs')) {
                $imageColumn = if ($name -eq 'University') { 'HeroImg' } else { 'ImagePath' }
                $imageBefore = Invoke-TestSql "SELECT [$imageColumn] FROM dbo.[$($contentSection.Table)] WHERE [$($contentSection.Key)]=$recordId;"
                Assert-Check ($imageBefore -match '^/images/uploads/[a-f0-9]{32}\.png$' -and (Send-Request $admin $imageBefore).Status -eq 200) "$name upload persists and can be displayed"
                $fields.Remove('__ImageBytes')
            }
            $fields.Id=$recordId
            if ($fields.ContainsKey('AcademyDescription')) { $fields.AcademyDescription='Updated verification description' }
            elseif ($fields.ContainsKey('Description')) { $fields.Description='Updated verification description' }
            elseif ($fields.ContainsKey('Title')) { $fields.Title='Updated verification title' }
            elseif ($fields.ContainsKey('AltText')) { $fields.AltText='Updated verification image' }
            elseif ($fields.ContainsKey('AcademyAddress')) { $fields.AcademyAddress='Updated verification campus' }
            $updated = Submit-Form $admin "${route}?id=$recordId" $fields
            Assert-Check ($updated.Status -eq 302 -and (Send-Request $admin $route).Status -eq 200) "Admin can edit and list $name content"
            if ($name -in @('University','Photos','Programs')) {
                Assert-Check ((Invoke-TestSql "SELECT [$imageColumn] FROM dbo.[$($contentSection.Table)] WHERE [$($contentSection.Key)]=$recordId;") -eq $imageBefore) "$name keeps the existing image when no replacement is selected"
            }
            Assert-Check ((Send-Request $admin $route @{}).Status -eq 400) "$name save requires an antiforgery token"
            $contentSection.RecordId=$recordId
        }
        $rendered = Send-Request $admin '/'
        $savedHero = Invoke-TestSql 'SELECT TOP(1) HeroImg FROM dbo.AcademyInfoTable;'
        Assert-Check ($rendered.Body.Contains('src="'+$savedHero+'"') -and $rendered.Body.Contains('Verification heading')) 'Public hero and About heading use the saved admin content'
        Assert-Check ($rendered.Body.Contains('8:30 AM') -and $rendered.Body.Contains('5:15 PM')) 'Public office hours use the saved time pair'
        Assert-Check ($rendered.Status -eq 200 -and $rendered.Body.Contains('Updated verification description') -and $rendered.Body.Contains('Updated verification title') -and $rendered.Body.Contains('Updated verification campus')) 'University page renders persisted content from the tables'
        $singleton = Submit-Form $admin '/UniversityInformation' @{HeroImg='';AcademyDescription='Updated verification description';Mission='Verification mission';Vision='Verification vision'}
        Assert-Check ($singleton.Status -eq 302 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.AcademyInfoTable;') -eq '1') 'University settings update the single existing record'
        $singleton = Submit-Form $admin '/ContactInformation' @{AcademyAddress='Updated verification campus'}
        Assert-Check ($singleton.Status -eq 302 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.ContactsTable;') -eq '1') 'Contact settings update the single existing record'
        $badHero = Submit-Form $admin '/UniversityInformation' @{__ImageBytes=[Text.Encoding]::UTF8.GetBytes('<script>invalid image</script>');__ImageName='fake.png'}
        Assert-Check ($badHero.Status -eq 200 -and $badHero.Body.Contains('do not match')) 'A fake PNG upload is rejected'
        $badLink = Submit-Form $admin '/SocialLinks' @{PlatformName='Bad link';Url='javascript:alert(1)'}
        Assert-Check ($badLink.Status -eq 200 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.SocialLinksTable;') -eq '1') 'Unsafe social URL is rejected'
        $badImage = Submit-Form $admin '/AboutGallery' @{__ImageBytes=$script:imageBytes;__ImageName='image.svg'}
        Assert-Check ($badImage.Status -eq 200 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.AboutImagesTable;') -eq '1') 'Unsupported image extensions are rejected'
        $oversize = Submit-Form $admin '/UniversityInformation' @{__ImageBytes=[byte[]]::new(5 * 1024 * 1024 + 1);__ImageName='large.png'}
        Assert-Check ($oversize.Status -eq 200 -and $oversize.Body.Contains('5 MB')) 'Image uploads over 5 MB are rejected'
        $emptyValue = Submit-Form $admin '/CoreValues' @{Title=' '}
        Assert-Check ($emptyValue.Status -eq 200 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.CoreValuesTable;') -eq '1') 'Blank required content title is rejected'
        $headingAndValues = @{AboutHeading='<script>Heading</script>';AcademyDescription='Updated verification description';Mission='Verification mission';Vision='Verification vision';CoreValuesText="Curiosity`r`n  Independent thought  `r`n`r`nRespect`r`nWisdom"}
        $combined = Submit-Form $admin '/UniversityInformation' $headingAndValues
        Assert-Check ($combined.Status -eq 302 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.CoreValuesTable;') -eq '4' -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.CoreValuesTable WHERE Title=N'Independent thought';") -eq '1') 'Combined editor stores trimmed values as individual ordered records'
        $combinedPublic = Send-Request $admin '/'
        Assert-Check (([regex]::Matches($combinedPublic.Body, '<ul class="about-values prose">').Count -eq 2) -and $combinedPublic.Body.Contains('<li>Curiosity</li>') -and $combinedPublic.Body.Contains('<li>Wisdom</li>')) 'Desktop and mobile render core values as semantic bullet lists'
        Assert-Check ($combinedPublic.Body.Contains('&lt;script&gt;Heading&lt;/script&gt;') -and -not $combinedPublic.Body.Contains('<script>Heading</script>')) 'Editable About heading is HTML encoded'
        $reloaded = Send-Request $admin '/UniversityInformation'
        Assert-Check ($reloaded.Body.Contains('Independent thought') -and $reloaded.Body.Contains('&lt;script&gt;Heading&lt;/script&gt;')) 'Combined values and heading reload into the admin editor'
        $tooLong = $headingAndValues.Clone(); $tooLong.CoreValuesText = 'x' * 101
        $rejected = Submit-Form $admin '/UniversityInformation' $tooLong
        Assert-Check ($rejected.Status -eq 200 -and $rejected.Body.Contains('100 characters') -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.CoreValuesTable;') -eq '4') 'An oversized value rejects the whole save without changing the list'
        $tooLong = $headingAndValues.Clone(); $tooLong.AboutHeading = 'x' * 151
        Assert-Check ((Submit-Form $admin '/UniversityInformation' $tooLong).Status -eq 200 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.CoreValuesTable;') -eq '4') 'About heading length is enforced before saving'
        $headingAndValues.CoreValuesText='Updated verification title'; $headingAndValues.AboutHeading=''
        Assert-Check ((Submit-Form $admin '/UniversityInformation' $headingAndValues).Status -eq 302 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.CoreValuesTable;') -eq '1' -and (Send-Request $admin '/').Body.Contains('A place for curious minds')) 'Removing value lines updates the list and blank heading uses the established fallback'
        $singleTime = Submit-Form $admin '/ContactInformation' @{TimeIn='08:30';TimeOut=''}
        Assert-Check ($singleTime.Status -eq 200 -and $singleTime.Body.Contains('Enter both Time In and Time Out')) 'A partial office-time pair is rejected'
        Assert-Check ((Submit-Form $admin '/ContactInformation' @{TimeIn='25:00';TimeOut='17:00'}).Status -eq 200) 'An invalid office time is rejected'
        Invoke-TestSql "UPDATE dbo.ContactsTable SET OfficeHours=N'Monday to Friday';" | Out-Null
        $overnight = Submit-Form $admin '/ContactInformation' @{TimeIn='22:00';TimeOut='06:00';OfficeHours='Forged replacement'}
        Assert-Check ($overnight.Status -eq 302 -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.ContactsTable WHERE TimeIn='22:00:00' AND TimeOut='06:00:00' AND OfficeHours=N'Monday to Friday';") -eq '1' -and (Send-Request $admin '/').Body.Contains('(next day)')) 'Office times persist, overnight hours are explicit, and existing hours text is protected'
        $contactEditor = Send-Request $admin '/ContactInformation'
        Assert-Check ($contactEditor.Body.Contains('type="time"') -and $contactEditor.Body.Contains('name="TimeIn"') -and $contactEditor.Body.Contains('name="TimeOut"') -and $contactEditor.Body.Contains('value="22:00"')) 'Contacts reloads saved times into native time inputs'
        Assert-Check ((Submit-Form $admin '/ContactInformation' @{TimeIn='';TimeOut=''}).Status -eq 302 -and (Send-Request $admin '/').Body.Contains('Monday to Friday')) 'Clearing both times restores the retained legacy office-hours text'
        Invoke-TestSql "UPDATE dbo.FounderTable SET Birthday='1900-01-01';" | Out-Null
        $founderId = Invoke-TestSql 'SELECT TOP(1) FounderId FROM dbo.FounderTable;'
        Assert-Check ((Submit-Form $admin '/FounderInformation' @{Id=$founderId;FounderName='Verification title';Description='Verification biography';FounderMessage='Verification message'}).Status -eq 302 -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.FounderTable WHERE Birthday='1900-01-01';") -eq '1' -and -not (Send-Request $admin '/FounderInformation').Body.Contains('name="Birthday"')) 'Founder shows the requested fields and preserves existing birthdays'
        Assert-Check ((Send-Request $admin '/AdminContent/Edit?section=999').Status -eq 404) 'Unknown content section is rejected'
        Assert-Check ((Send-Request $admin '/AcademicPrograms?id=99999').Status -eq 404) 'Missing content record returns 404'
        $xss = Submit-Form $admin '/AcademicPrograms' @{SubjectName='<script>alert(1)</script>';Description='Text only'}
        $encoded = Send-Request $admin '/'
        Assert-Check ($xss.Status -eq 302 -and $encoded.Body.Contains('&lt;script&gt;') -and -not $encoded.Body.Contains('<script>alert(1)</script>')) 'Stored content renders as escaped text'
        foreach ($contentSection in $sections | Where-Object {$_.Name -notin @('University','Contacts')}) {
            $name=$contentSection.Name; $recordId=$contentSection.RecordId; $route='/'+$contentSection.Controller
            $confirmation=Send-Request $admin "${route}?id=$recordId&confirmDelete=true"
            Assert-Check ($confirmation.Status -eq 200 -and $confirmation.Body.Contains('Delete this entry?') -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.[$($contentSection.Table)] WHERE [$($contentSection.Key)]=$recordId;") -eq '1') "GET delete only confirms $name"
            Assert-Check ((Send-Request $admin "$route/Delete" @{Id=$recordId}).Status -eq 400) "$name delete requires an antiforgery token"
            $pageToken=[regex]::Match($confirmation.Body, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value
            $removed=Send-Request $admin "$route/Delete" @{Id=$recordId;__RequestVerificationToken=[Net.WebUtility]::HtmlDecode($pageToken)}
            Assert-Check ($removed.Status -eq 302 -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.[$($contentSection.Table)] WHERE [$($contentSection.Key)]=$recordId;") -eq '0') "Admin can delete $name content"
        }
        $clearFields = @{AboutHeading='';CoreValuesText='One temporary value'}
        Assert-Check ((Submit-Form $admin '/UniversityInformation' $clearFields).Status -eq 302 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.CoreValuesTable;') -eq '1') 'Combined editor can add values to an empty list'
        $clearFields.CoreValuesText=''
        Assert-Check ((Submit-Form $admin '/UniversityInformation' $clearFields).Status -eq 302 -and (Invoke-TestSql 'SELECT COUNT(*) FROM dbo.CoreValuesTable;') -eq '0' -and (Send-Request $admin '/').Body.Contains('Core values have not been published yet.')) 'Blank combined value input clears the list and renders its empty state'
        Assert-Check ((Send-Request $admin '/AdminContent/Delete?section=University&id=1').Status -eq 404) 'Single university record cannot be deleted through content editor'
        Assert-Check ((Send-Request $anonymous '/AccountManagement').Location -like '*Login*') 'Anonymous cannot access Account management'
        Assert-Check ((Send-Request $studentClient '/AccountManagement').Location -like '*AccessDenied*') 'Students cannot access Account management'
        Assert-Check ((Send-Request $studentClient '/AccountManagement' @{}).Location -like '*AccessDenied*') 'Students cannot modify accounts'
        $accounts = Send-Request $admin '/AccountManagement'
        Assert-Check ($accounts.Status -eq 200 -and $accounts.Body.Contains('profile.student') -and -not $accounts.Body.Contains('PasswordHash')) 'Admin account list shows account summaries without password hashes'
        $profileId = Invoke-TestSql "SELECT UserId FROM dbo.UserTable WHERE Username=N'profile.student';"
        $adminId = Invoke-TestSql "SELECT UserId FROM dbo.UserTable WHERE Username=N'verification.student';"
        $details = Send-Request $admin "/AccountManagement/Details?id=$profileId"
        Assert-Check ($details.Status -eq 200 -and $details.Body.Contains('Account information') -and -not $details.Body.Contains('PasswordHash') -and -not $details.Body.Contains('Save account')) 'Account information is read-only and excludes password hashes'
        Assert-Check ((Send-Request $studentClient "/AccountManagement/Details?id=$profileId").Location -like '*AccessDenied*') 'Students cannot view another account information page'
        Assert-Check ((Send-Request $admin '/AccountManagement/SetActive' @{id=$profileId;isActive='false'}).Status -eq 400) 'Quick account status changes require antiforgery'
        Assert-Check ((Send-Request $studentClient '/AccountManagement/SetActive' @{id=$profileId;isActive='false'}).Location -like '*AccessDenied*') 'Students cannot use quick account status changes'
        $listToken = [Net.WebUtility]::HtmlDecode([regex]::Match($accounts.Body, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value)
        $toggleFields = @{id=$profileId;isActive='false';__RequestVerificationToken=$listToken;Username='forged.name';Role='Owner'}
        Assert-Check ((Send-Request $admin '/AccountManagement/SetActive' $toggleFields).Status -eq 302 -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.UserTable WHERE UserId=$profileId AND IsActive=0 AND Username=N'profile.student' AND Role=N'Student';") -eq '1') 'Quick deactivate only changes account access'
        $toggleFields.isActive='true'
        Assert-Check ((Send-Request $admin '/AccountManagement/SetActive' $toggleFields).Status -eq 302 -and (Invoke-TestSql "SELECT IsActive FROM dbo.UserTable WHERE UserId=$profileId;") -eq '1') 'Quick activate restores account access'
        $toggleFields.id=$adminId; $toggleFields.isActive='false'
        Assert-Check ((Send-Request $admin '/AccountManagement/SetActive' $toggleFields).Status -eq 302 -and (Invoke-TestSql "SELECT IsActive FROM dbo.UserTable WHERE UserId=$adminId;") -eq '1') 'Quick status action cannot deactivate the current admin'
        $totals = (Invoke-TestSql 'SELECT COUNT(*),SUM(CASE WHEN IsActive=1 THEN 1 ELSE 0 END),SUM(CASE WHEN IsActive=0 THEN 1 ELSE 0 END) FROM dbo.UserTable;') -split '\s+'
        $dashboard = Send-Request $admin '/Admin'
        Assert-Check ($dashboard.Status -eq 200 -and $dashboard.Body.Contains("<dt>Total Registered</dt><dd>$($totals[0])</dd>") -and $dashboard.Body.Contains("<dt>Active Users</dt><dd>$($totals[1])</dd>") -and $dashboard.Body.Contains("<dt>Deactivated Users</dt><dd>$($totals[2])</dd>")) 'Dashboard account analytics match database totals'
        $hashBefore = Invoke-TestSql "SELECT CHECKSUM(PasswordHash) FROM dbo.UserTable WHERE UserId=$profileId;"
        $accountFields = @{Id=$profileId;Username='profile.student';FirstName='Mira';LastName='Updated surname';Role='Student';IsActive='false';__ImageBytes=$script:imageBytes}
        $accountSaved = Submit-Form $admin "/AccountManagement?id=$profileId" $accountFields
        Assert-Check ($accountSaved.Status -eq 302 -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.UserTable WHERE UserId=$profileId AND LastName=N'Updated surname' AND IsActive=0;") -eq '1') 'Admin can update account details and deactivate an account'
        $profileImage = Invoke-TestSql "SELECT ProfileImg FROM dbo.UserTable WHERE UserId=$profileId;"
        Assert-Check ($profileImage -match '^/images/uploads/[a-f0-9]{32}\.png$' -and (Send-Request $admin $profileImage).Status -eq 200) 'Account profile image uploads and renders'
        Assert-Check ((Invoke-TestSql "SELECT CHECKSUM(PasswordHash) FROM dbo.UserTable WHERE UserId=$profileId;") -eq $hashBefore) 'Account edits preserve the existing password hash'
        $accountFields.Remove('__ImageBytes'); $accountFields.Role='Admin'; $accountFields.IsActive='true'
        $promoted = Submit-Form $admin "/AccountManagement?id=$profileId" $accountFields
        Assert-Check ($promoted.Status -eq 302 -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.UserTable WHERE UserId=$profileId AND Role=N'Admin' AND IsActive=1;") -eq '1') 'Admin can reactivate an account and change its role'
        Assert-Check ((Invoke-TestSql "SELECT ProfileImg FROM dbo.UserTable WHERE UserId=$profileId;") -eq $profileImage) 'Profile image is preserved without a replacement'
        $accountFields.Username='VERIFICATION.STUDENT'
        $duplicateAccount = Submit-Form $admin "/AccountManagement?id=$profileId" $accountFields
        Assert-Check ($duplicateAccount.Status -eq 200 -and $duplicateAccount.Body.Contains('already in use')) 'Account username uniqueness is checked case-insensitively'
        $accountFields.Username='profile.student'; $accountFields.Role='Owner'
        Assert-Check ((Submit-Form $admin "/AccountManagement?id=$profileId" $accountFields).Status -eq 200) 'Unsupported account roles are rejected'
        $selfEdit = Submit-Form $admin "/AccountManagement?id=$adminId" @{Id=$adminId;Username='verification.student';Role='Student';IsActive='false'}
        Assert-Check ($selfEdit.Status -eq 200 -and $selfEdit.Body.Contains('own account') -and (Invoke-TestSql "SELECT COUNT(*) FROM dbo.UserTable WHERE UserId=$adminId AND Role=N'Admin' AND IsActive=1;") -eq '1') 'Admin cannot deactivate or demote their own account'
        Assert-Check ((Send-Request $admin '/AccountManagement' @{Id=$profileId}).Status -eq 400) 'Account edits require an antiforgery token'
        $searchResult = Send-Request $admin '/AccountManagement?search=Updated%20surname'
        Assert-Check ($searchResult.Status -eq 200 -and $searchResult.Body.Contains('profile.student') -and -not $searchResult.Body.Contains('<h3>verification.student</h3>')) 'Account search filters the list'
    }
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
    Write-Output "Verification passed ($script:passedChecks checks). All mutations were confined to the disposable test database."
    if ($Preview) {
        # Explicit disposable-fixture preview only; never changes the university database.
        Invoke-TestSql "UPDATE dbo.UserTable SET IsActive=1, Role=N'Admin' WHERE Username=N'verification.student'; DELETE FROM dbo.ProgramsTable WHERE SubjectName=N'<script>alert(1)</script>';" | Out-Null
        if ($IncludeContent) {
            Invoke-TestSql @'
IF NOT EXISTS (SELECT 1 FROM dbo.ProgramsTable)
INSERT INTO dbo.ProgramsTable (SubjectName, Description, ImagePath) VALUES
(N'Preview: Astronomy', N'Disposable visual-preview content for testing the program cards and descriptions.', N'/images/hero/ravenclaw-common-room.png'),
(N'Preview: History', N'Disposable visual-preview content. This is not university production data.', N'/images/hero/portrait-stairwell.png'),
(N'Preview: Literature', N'Disposable visual-preview content for the horizontal program showcase.', N'/images/hero/great-hall.png'),
(N'Preview: Philosophy', N'Disposable visual-preview content for testing keyboard access and hover behavior.', N'/images/hero/ravenclaw-eagle-wall.png'),
(N'Preview: Natural sciences', N'Disposable visual-preview content for testing responsive card layout.', N'/images/hero/ravenclaw-common-room.png'),
(N'Preview: Mathematics', N'Disposable visual-preview content for testing the looping track.', N'/images/hero/portrait-stairwell.png');
'@ | Out-Null
        }
        Write-Output "PREVIEW_URL=$script:baseUrl"
        Write-Output 'Disposable preview login: verification.student / Test-Only-Scholar-2026'
        if ([Console]::IsInputRedirected) {
            $stopFile=Join-Path $tempDir 'stop-preview'
            Write-Output "PREVIEW_STOP_FILE=$stopFile"
            while (-not (Test-Path -LiteralPath $stopFile)) { Start-Sleep -Milliseconds 500 }
        } else {
            Read-Host 'Press Enter to stop the preview and remove the test database' | Out-Null
        }
    }
}
finally {
    foreach ($client in $clients) { $client.Dispose() }
    if ($serverProcess -and -not $serverProcess.HasExited) { & taskkill /PID $serverProcess.Id /T /F | Out-Null }
    $env:ConnectionStrings__UniversityDb = $oldConnection
    $env:ASPNETCORE_ENVIRONMENT = $oldEnvironment
    $env:ImageUploads__RootPath = $oldUploadRoot
    if ($databaseCreated) {
        # The target is this script's GUID-named, newly created test database only.
        & sqlcmd -S $Server -E -l 5 -b -d master -Q "ALTER DATABASE [$databaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$databaseName];"
        if ($LASTEXITCODE -ne 0) { Write-Warning "Could not remove disposable database $databaseName" }
    }
    $testUploads = [IO.Path]::GetFullPath((Join-Path $tempDir 'uploads'))
    if ([IO.Path]::GetDirectoryName($testUploads) -ne [IO.Path]::GetFullPath($tempDir) -or
        (Split-Path $tempDir -Leaf) -notmatch '^RavenclawAuthTest_[a-f0-9]{32}$') {
        throw 'Unexpected disposable upload cleanup path.'
    }
    if (Test-Path -LiteralPath $testUploads) { Remove-Item -LiteralPath $testUploads -Recurse -Force }
    Write-Output "Verification logs: $tempDir"
}
