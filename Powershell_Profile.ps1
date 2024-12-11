Get-Content .env | foreach {
    $name, $value = $_.split('=')
    if ([string]::IsNullOrWhiteSpace($name) || $name.Contains('#')) {
        return
    }
    $value = $value -replace '"', ''
    Set-Content env:\$name $value
}

function dc {
    [CmdletBinding()]
    param (
        [parameter(mandatory = $true, position = 0)][string]$cmd,
        [parameter(mandatory = $false, position = 1, ValueFromRemainingArguments = $true)]$args
    )

    function sanitizeTaskId {
        param (
            [parameter(mandatory = $true, position = 0)][string]$task_id
        )

        return $task_id -replace "#", ""
    }

    function test {

        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("Authorization", $Env:CLICKUP_API_KEY)

        $response = Invoke-RestMethod 'https://api.clickup.com/api/v2/task/868b9b8rf' -Method 'GET' -Headers $headers
        $response | ConvertTo-Json
	
        $branchname = $response.name -replace "[^a-zA-Z0-9]", "_"
        $branchname = $branchname.ToLower()	
		
        Write-Host $branchname
    }

    
    function getTaskData {
        param (
            [parameter(mandatory = $true)][string]$task_id
        )

        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("Authorization", $Env:CLICKUP_API_KEY)

        $response = Invoke-RestMethod "https://api.clickup.com/api/v2/task/${task_id}" -Method 'GET' -Headers $headers
        $response | ConvertTo-Json -WarningAction:SilentlyContinue
    }
    
    function setGithubLinkCustomField {
        param (
            [parameter(mandatory = $true, position = 0)][string]$task_id,
            [parameter(mandatory = $true, position = 1)][string]$branchname
        )

        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("Content-Type", "application/json")
        $headers.Add("Authorization", $Env:CLICKUP_API_KEY)

        $body = @"
        { 
            "value": "$Env:GITHUB_REPO_ROOT/tree/$branchname"
        }
"@

        Invoke-RestMethod "https://api.clickup.com/api/v2/task/$task_id/field/$ENV:CUSTOM_GITHUB_FIELD_ID" -Method 'POST' -Headers $headers -Body $body -Outfile nul
    }
    
    function setClickUpTaskStatus {
        param (
            [parameter(mandatory = $true, position = 0)][string]$task_id,
            [parameter(mandatory = $true, position = 1)][string]$status
        )

        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("Content-Type", "application/json")
        $headers.Add("Authorization", "$Env:CLICKUP_API_KEY")
        
        $body = @"
        {
            "status": "$status"
        }
"@

        Invoke-RestMethod "https://api.clickup.com/api/v2/task/${task_id}" -Method 'PUT' -Headers $headers -Body $body -Outfile nul
    }

    function _switch {
        param (
            [parameter(mandatory = $true, position = 0)][string]$task_id
        )

        $task_id = sanitizeTaskId($task_id)

        $task_data = getTaskData($task_id) | ConvertFrom-Json

        if (!$task_data) {
            Write-Error "No task found for id: $task_id"
        }

        $branchname = $task_data.name
        $branchname = $branchname -replace "[^a-zA-Z0-9]", "_"
        $branchname = $branchname.ToLower()

        git switch "$branchname"
    }
    function open { 
        param (
            [parameter(mandatory = $true, position = 0)][string]$task_id
        )

        $task_id = sanitizeTaskId($task_id)

        $task_data = getTaskData($task_id) | ConvertFrom-Json

        $branchname = $task_data.name
        $branchname = $branchname -replace "[^a-zA-Z0-9]", "_"
        $branchname = $branchname.ToLower()

        git switch -c "$branchname"
        git push origin "$branchname"

        setGithubLinkCustomField $task_id $branchname
        setClickUpTaskStatus $task_id "in progress"
    }

    Switch ($cmd) {
        "open" { open($args) }
        "switch" { _switch($args) }
        "test" { test }
        default { Write-Host "Invalid DC command $cmd" }
    }
}

function Prompt {
    # Get the current Git branch if in a Git repository
    $branchName = ""
    if (Test-Path .git) {
        $gitStatus = git symbolic-ref --short HEAD 2>$null
        if ($gitStatus) {
            $branchName = "($gitStatus)"
        }
    }

    # Build the prompt string
    $path = Get-Location
    Write-Host "$path" -NoNewline
    Write-Host " $branchName" -ForegroundColor Green -NoNewline
    return "> "
}

function pushAll {
    git add . &&
    git commit &&
    git push origin HEAD
}

