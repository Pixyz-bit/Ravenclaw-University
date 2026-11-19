param(
    [string]$Server = 'localhost',
    [string]$Database = 'Ravenclaw University Db'
)

# Local development diagnostic: encrypted Windows authentication, matching the
# app's development certificate policy. Reads metadata only; never restarts SQL.
$ErrorActionPreference = 'Stop'
$sqlcmd = (Get-Command sqlcmd -ErrorAction Stop).Source
$query = @'
SET NOCOUNT ON;
SELECT COUNT(*) AS Tables,
       (SELECT COUNT(*) FROM sys.columns c JOIN sys.tables t ON t.object_id=c.object_id WHERE t.is_ms_shipped=0) AS Columns
FROM sys.tables WHERE is_ms_shipped=0;
SELECT physical_memory_in_use_kb, process_physical_memory_low, process_virtual_memory_low
FROM sys.dm_os_process_memory;
SELECT t.name AS TableName, c.name AS ColumnName, ty.name AS SqlType,
       c.max_length, c.precision, c.scale, c.is_nullable, c.is_identity
FROM sys.tables t JOIN sys.columns c ON c.object_id=t.object_id
JOIN sys.types ty ON ty.user_type_id=c.user_type_id
WHERE t.is_ms_shipped=0 ORDER BY t.name,c.column_id;
SELECT 'RAVENCLAW_METADATA_COMPLETE' AS DiagnosticResult;
'@
$startInfo = [Diagnostics.ProcessStartInfo]::new($sqlcmd)
$startInfo.UseShellExecute = $false
$startInfo.CreateNoWindow = $true
$startInfo.RedirectStandardOutput = $true
$startInfo.RedirectStandardError = $true
foreach ($argument in @('-S', $Server, '-d', $Database, '-E', '-N', '-C', '-b',
        '-l', '5', '-t', '10', '-W', '-s', '|', '-Q', $query)) {
    $startInfo.ArgumentList.Add($argument)
}
$process = [Diagnostics.Process]::Start($startInfo)
try {
    $output = $process.StandardOutput.ReadToEndAsync()
    $errors = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit(20000)) {
        $process.Kill()
        $process.WaitForExit()
        throw 'SQL diagnostic exceeded 20 seconds and was stopped. No database changes were made.'
    }
    $standardOutput = $output.GetAwaiter().GetResult()
    $standardError = $errors.GetAwaiter().GetResult()
    if ($process.ExitCode -ne 0) {
        throw "SQL diagnostic failed (exit $($process.ExitCode)). $standardError $standardOutput"
    }
    # sqlcmd can return zero after printing a timeout. Require evidence that the
    # final statement completed instead of trusting the process exit code alone.
    if ($standardOutput -notmatch '(?m)^RAVENCLAW_METADATA_COMPLETE\s*$' -or
        -not [string]::IsNullOrWhiteSpace($standardError) -or
        "$standardOutput $standardError" -match '(?i)timeout expired|sqlcmd: error:') {
        throw "SQL diagnostic did not complete successfully. $standardError $standardOutput"
    }
    Write-Output $standardOutput
    Write-Output 'Database metadata read passed. No database changes were made.'
}
finally {
    $process.Dispose()
}
