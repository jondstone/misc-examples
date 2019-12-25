<#
This is a PowerShell script I wrote to push files to a staging environment once they have been merged into the Master Git Branch.
This is done with a Team City build that will display a unqiue Exit Code if the build fails. 
#>

#Initializiations
$staging = "\\server\Path_To_Staging"
$ignoreFileList = New-Object Collections.Generic.List[String]
$replicatedFiles = New-Object Collections.Generic.List[string]

#Get list of files changed
$changedFileInfoPath = $args[0]

#Get the content of the file path
$changedFileData = Get-Content $changedFileInfoPath 

#List of files to ignore
$ignoreFileList.add("deploy_files.ps1")
$ignoreFileList.add("README.md")

#Iterate through the changed files then add to them to the replication list
foreach($line in $changedFileData)
{
	#Split the line and get file. 
	#<relative file path>:<change type>:<revision>
	$splitLine = $line -split ":"
	$replicatedFiles.add($splitLine[0])
}

#Push updated files to staging and a implement try/catch that will return unique error messages
try
{
    foreach ($replicatedFile in $replicatedFiles)
    {
        #Do not push ignored files
        if ($ignoreFileList -notcontains $replicatedFile)
        {
            Write-Output $replicatedFile
            $pushPath = Join-Path $staging $replicatedFile

            #If Replicated file "contains" a /, then we know it contains a folder (i.e. images/logo.jpg)
            #Else, it's a straight file copy
            if ($replicatedFile -like '*/*')
            {
                try
                {
                    #If folder in the path does not exist, it will create, and then copy the file
                    #Else if it exists, it will copy the file
                    if((Test-Path -Path $pushPath) -eq $false)
                    {
                        $folder = $replicatedFile.split('/')[0]
                        New-Item -Path $testThis -Name $folder -ItemType "directory"
                        $folderPath = Join-Path $staging $folder
                        Copy-Item $replicatedFile -Destination $folderPath
                    } 
                    else
                    {
			            Copy-Item $replicatedFile -Destination $pushPath
                    }
                }
                catch
                {
                    Write-Error "Exception: Error creating folder and/or copying filder to folder from Git to the Staging Environment."
                    Write-Error -Exception $_.Exception
                    Exit 3 
                }
            }
            else
            {
                try
                {
                    Copy-Item $replicatedFile -Destination $pushPath
                }
                catch
                {
                    Write-Error "Exception: Error copying the file from Git to the Staging Environment."
                    Write-Error -Exception $_.Exception
                    Exit 2 
                }
            }
        }
    }
}
catch
{
    Write-Error "An exception has occurred in the main code."
    Write-Error -Exception $_.Exception
    Exit 1
}