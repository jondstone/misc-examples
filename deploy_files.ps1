<#
This is a PowerShell script I wrote to push files to a staging environment once they have been merged into the Master Git Branch.
This is done with a Team City build that will display a unqiue Exit Code if the build fails. 
To do: Currently if you add a new folder "\images" and it does not already exist in the Staging Environment, then the build chokes.
Need to add the ability to create a folder if it does not exist, before trying to write a file to it.
#>

#Initializiations
$staging="\\server\Path_To_Staging"
$replicatedFiles = New-Object Collections.Generic.List[string]

#Get list of files changed
$changedFileInfoPath = $args[0]

#Get the content of the file path
$changedFileData = Get-Content $changedFileInfoPath 

#List of files to ignore
$ignoreFileList = New-Object Collections.Generic.List[String]
$ignoreFileList.add("deploy_files.ps1")

#Iterate through the changed files then add to them to the replication list
foreach($line in $changedFileData)
{
	#Split the line and get file. 
	#<relative file path>:<change type>:<revision>
	$splitLine = $line -split ":"
	$replicatedFiles.add($splitLine[0])
}

#then push these updated files to staging
#implement try/catch that will return error messages
try
{
    foreach ($replicatedFile in $replicatedFiles)
    {
        if ($ignoreFileList -notcontains $replicatedFile)
        {
            try
            {
                $pushPath = Join-Path $staging $replicatedFile
			    Write-Output $pushPath
			    Write-Output $replicatedFile
			    Copy-Item $replicatedFile -Destination $pushPath
            }
            catch
            {
                Write-Error -Exception $_.Exception
                Exit 1
            } 
        }
    }
}
catch
{
    Write-Error -Exception $_.Exception
    Exit 2
}