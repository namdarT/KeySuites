
Function Write-Log {
    Param(
        $Message,
        $FgColor = "White",
        $Path = "C:\Users\nfaruqi\logs\FileUploadlog.txt"
    )

    Function TS {
                    Get-Date -Format 'hh:mm:ss'                
                }

    "[$(TS)]$Message" | Tee-Object -FilePath $Path -Append | Write-Verbose
     Write-Host "[$(TS)] $Message" -ForegroundColor $FgColor

}

Function Get-AssetList (){
        try
        {
            # This is a simple user/pass connection string.
            # Feel free to substitute "Integrated Security=True" for system logins.
            $connString = "Data Source=DESKTOP-CP5S6J8\MSSQLSERVER19;Database=simplicity;User ID=sa;Password=sa"

            #Create a SQL connection object
            $conn = New-Object System.Data.SqlClient.SqlConnection $connString;

            #Attempt to open the connection
            $conn.Open();
            if($conn.State -eq "Open")
            {
                # Notify of successful connection``````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````
                #Write-Host "Connection successful to Database:"$conn.Database -ForegroundColor Green
                Write-Log -Message "Connection successful to Database: $($conn.Database)" -FgColor "Green"
            }
            # We could not connect here
            # Notify connection was not in the "open" state
        }
        catch
        {
            # We could not connect here
                #Write-Host "Connection Failed" -ForegroundColor Red
                Write-Log -Message "Database Connection Failed to SENSORVM1;Database=simplicity" -FgColor "Red"
                Write-Log -Message "$($_.Exception.Message)" -FgColor "Red"
                $conn.Close();

         }
         # Set Stored procedure that returns Replication Docs for approved funds
        
            $sqlText = "Exec dbo.rpt_asset_device_list ";
            $sqlText = $sqlText + "0"; 
            # "{0:0}" -f $NoOfDays +",0";
            Write-Host "Sql String:"$sqlText -ForegroundColor Green
            Write-Log -Message $sqlText -FgColor "Yellow"

         $sqlCmd = new-object System.Data.SqlClient.SqlCommand($sqlText, $conn);
         $sqlCmd.CommandTimeout = 240;
         
         # Execute Stored Proc   
         $dataReader = $sqlCmd.ExecuteReader()

          #store Results in an Array
        $queryResults = @()
            while ($dataReader.Read())
                {
                    $dataRow = @{}
                        for ($i = 0; $i -lt $dataReader.FieldCount; $i++)
                            {
                                $dataRow[$dataReader.GetName($i)] = $dataReader.GetValue($i)
                             }
#                    $queryResults += New-Object PSObject -Property $dataRow 
                    $queryResults += [pscustomobject]$dataRow
                }
    
        #close connection
        $conn.Close();

        #return Data results Array
        Return $queryResults
}

Function Get-DeviceSplit()
{
    Param(
              $dataResponseJson
          )
    #split Json Start Asset
           $sqlTextSplit = "Exec json_Split ";
           $sqlTextSplit = $sqlTextSplit + "'" + $dataResponseJson + "'";
           Write-Host "Sql String:"$sqlTextSplit -ForegroundColor Green
            Write-Log -Message $sqlTextSplit -FgColor "Yellow"
            
            
             $sqlCmdSplit = new-object System.Data.SqlClient.SqlCommand($sqlTextSplit, $dataconn);
             $sqlCmdSplit.CommandTimeout = 240;
         
             # Execute Stored Proc   
             $dataReaderSplit = $sqlCmdSplit.ExecuteReader()
             #split Json End Asset
             return $dataReaderSplit
}
Function Get-AssetSplit()
{
Param(
              $dataResponseJson
          )

          # This is a simple user/pass connection string.
            # Feel free to substitute "Integrated Security=True" for system logins.
            $connStr = "Data Source=DESKTOP-CP5S6J8\MSSQLSERVER19;Database=simplicity;User ID=sa;Password=sa"

            #Create a SQL connection object
            $dataconn = New-Object System.Data.SqlClient.SqlConnection $connStr;

            #Attempt to open the connection
            $dataconn.Open();
            if($dataconn.State -eq "Open")
            {
                # Notify of successful connection``````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````
                #Write-Host "Connection successful to Database:"$conn.Database -ForegroundColor Green
                Write-Log -Message "Data upload Connection successful to Database: $($dataconn.Database)" -FgColor "Green"
            }
            # We could not connect here
            # Notify connection was not in the "open" state
        

    #split Json Start Asset
           $sqlTextSplit = "Exec json_Split ";
           $sqlTextSplit = $sqlTextSplit + "'" + $dataResponseJson + "'";
           Write-Host "Sql String:"$sqlTextSplit -ForegroundColor Green
            Write-Log -Message $sqlTextSplit -FgColor "Yellow"
            
            
             $sqlCmdSplit = new-object System.Data.SqlClient.SqlCommand($sqlTextSplit, $dataconn);
             $sqlCmdSplit.CommandTimeout = 240;
         
             # Execute Stored Proc   
             $dataReaderSplit = $sqlCmdSplit.ExecuteReader()
             #split Json End Asset

             $queryResults = @()
            while ($dataReaderSplit.Read())
                {
                    $dataRow = @{}
                        for ($i = 0; $i -lt $dataReaderSplit.FieldCount; $i++)
                            {
                                $dataRow[$dataReaderSplit.GetName($i)] = $dataReaderSplit.GetValue($i)
                             }
#                    $queryResults += New-Object PSObject -Property $dataRow 
                    $queryResults += [pscustomobject]$dataRow
                }

             $dataconn.Close();
             return $queryResults
}

Function Get-AssetToken(){
    Param(
            $ProcessAsset
        )

    $tokenHeaders = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $tokenHeaders.Add("Content-Type", "application/x-www-form-urlencoded")

#    $tokenBody = "username=bbruce&password=Centeron02!&grant_type=password&client_id=Simplicity&client_secret=C815B7BC-1B37-4CC8-94A2-3A38F08CD090"
      $tokenBody = "username="+ $ProcessAsset.access_user_nm +"&password="+$ProcessAsset.access_pw +"&grant_type=" +$ProcessAsset.access_grant_type+"&client_id="+$ProcessAsset.access_client_id+"&client_secret="+$ProcessAsset.access_client_secret
      Write-Log -Message $tokenBody -FgColor "Yellow"

    #$tokenResponse = Invoke-RestMethod 'https://auth.centeron.net/connect/token' -Method 'POST' -Headers $tokenHeaders -Body $tokenBody
      $tokenResponse = Invoke-RestMethod $ProcessAsset.token_url -Method 'POST' -Headers $tokenHeaders -Body $tokenBody
      #$tokenResponse = Invoke-RestMethod -Method 'Post' -Uri $ProcessAsset.token_url -Headers $tokenHeaders -Body $tokenBody
      $tokenRespJson =$tokenResponse | ConvertTo-Json
      Write-Log -Message $tokenResponse.access_token -FgColor "Green"
      #Write-Host $tokenResponse.access_token -ForegroundColor Green
       Return $tokenResponse.access_token
       }

       $connStr = "Data Source=DESKTOP-CP5S6J8\MSSQLSERVER19;Database=simplicity;User ID=sa;Password=sa"

            #Create a SQL connection object
            $dataconn = New-Object System.Data.SqlClient.SqlConnection $connStr;

            #Attempt to open the connection
            $dataconn.Open();
            if($dataconn.State -eq "Open")
            {
                # Notify of successful connection``````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````
                #Write-Host "Connection successful to Database:"$conn.Database -ForegroundColor Green
                Write-Log -Message "Data upload Connection successful to Database: $($dataconn.Database)" -FgColor "Green"
            }

        $AssetList =  Get-AssetList;
    #Write-Log -Message "Data upload Connection successful to Database: " + $AssetList.Count -FgColor "Green"
    if($Assetlist.asset_name.Length -gt 0 )
        {
            #  Parse out doc_name_full Columns 
            
            $AssetListFull = $assetlist | select asset_id,asset_name,token_url,data_url,device_url,New_data_url,New_device_url,access_user_nm,access_pw,access_grant_type,access_client_id,access_client_secret

            Write-Log -Message "$($AssetListFull)" -FgColor green
            #Get token and data 
            #Get-AssetData -AssetListAll $AssetListFull
            


        Foreach ($Asset in $AssetListFull)
        {
        
        Write-Log -Message "If "  -FgColor "Green"
            $tokenResponse = Get-AssetToken -ProcessAsset $Asset
            $dataHeaders = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        #    $authToken = "bearer "+ $tokenResponse.access_token
             $authToken = "bearer "+ $tokenResponse

        # Write-Host $authToken -ForegroundColor Yellow
          Write-Log -Message $authToken -FgColor "Yellow"
            $dataHeaders.Add("Authorization", $authToken)
            
                  #   $dataResponse = Invoke-RestMethod 'https://api.centeron.net/centeronAPI/v1/asset/2100580' -Method 'GET' -Headers $dataHeaders
        try {
            $dataUrl = $Asset.New_data_url
            $dataUrl = $dataUrl  + '/Report/Asset'
            $deviceUrl = $Asset.New_data_url
            $deviceUrl = $deviceUrl  + '/Report/Device'
         
            $dataResponse = Invoke-RestMethod $dataUrl -Method 'GET' -Headers $dataHeaders
            $deviceResponse = Invoke-RestMethod $deviceUrl -Method 'GET' -Headers $dataHeaders
            

        }
        catch 
        {
            Write-Log -Message "Error invoking rest method fort " +  $Asset.New_data_url.tostring() + "/Report/Asset" -FgColor "Red"
            Write-Log -Message "Error invoking rest method for " + $Asset.New_device_url.tostring() + "/Report/Device" -FgColor "Red"
            continue;
        }
#             Write-Host $dataResponse | ConvertFrom-Json 
            $dataResponseJson = $dataResponse | ConvertTo-Json
            $deviceResponseJson = $deviceResponse | ConvertTo-Json
            Write-Host $dataResponseJson -ForegroundColor Green
            Write-Host $deviceResponseJson -ForegroundColor Green
           Write-Log -Message $dataResponse.createDate
           Write-Log -Message $deviceResponse.createDate
           #
           #
           #split Json Start Asset
           $sqlTextSplit = "Exec json_Split ";
           $sqlTextSplit = $sqlTextSplit + "'" + $dataResponseJson + "'";
           Write-Host "Sql String:"$sqlTextSplit -ForegroundColor Green
            Write-Log -Message $sqlTextSplit -FgColor "Yellow"
            
            
             $sqlCmdSplit = new-object System.Data.SqlClient.SqlCommand($sqlTextSplit, $dataconn);
             $sqlCmdSplit.CommandTimeout = 240;
         
             # Execute Stored Proc   
             $dataReaderSplit = Get-AssetSplit -dataResponseJson $dataResponseJson
             
             #split Json End Asset
             #
             #
             #split Json Start Device
           $sqlTextSplitD = "Exec json_Split ";
           $sqlTextSplitD = $sqlTextSplitD + "'" + $deviceResponseJson + "'";
           Write-Host "Sql String:"$sqlTextSplitD -ForegroundColor Green
            Write-Log -Message $sqlTextSplitD -FgColor "Yellow"
            

         $sqlCmdSplitD = new-object System.Data.SqlClient.SqlCommand($sqlTextSplitD, $dataconn);
         $sqlCmdSplitD.CommandTimeout = 240;
         
         # Execute Stored Proc   
         
         $dataReaderSplitD = Get-AssetSplit -dataResponseJson $deviceResponseJson
         #split Json End Device
         if($dataReaderSplitD.Count -gt 0 )
         {
             #while ($dataReaderSplitD.Read())
              #  {
                #$dataReaderSplit.Read();
                    #$dataRow = @{}
                        for ($i = 0; $i -lt $dataReaderSplitD.Count; $i++)
                            {
                                #$dataRow[$dataReaderSplitD.GetName($i)] = $dataReaderSplitD.GetValue($i)
                                $dataReaderSplitJson = $dataReaderSplit.GetValue($i) | ConvertTo-Json
                                $dataReaderSplitDJson = $dataReaderSplitD.GetValue($i) | ConvertTo-Json
                                $sqlText = "Exec dbo.ins_asset_device_json_data ";
                                $sqlText = $sqlText +  $Asset.asset_id.tostring() + ",'" + $dataReaderSplitJson + "','" + $dataReaderSplitDJson + "',0"; 
                                Write-Host "Sql String:"$sqlText -ForegroundColor Green
                                Write-Host -Message "Split:1 $i"  $dataReaderSplit.GetValue($i) -FgColor "Yellow"
                                Write-Host -Message "Split:2 $i"  $dataReaderSplitD.GetValue($i) -FgColor "Yellow"
                                

                                $sqlCmd = new-object System.Data.SqlClient.SqlCommand($sqlText, $dataconn);
                                $sqlCmd.CommandTimeout = 240;
         
                                # Execute Stored Proc   
                                $dataReader = $sqlCmd.ExecuteNonQuery() | Out-Null
                                #$dataReader.Close();
                                
                             }
#                    $queryResults += New-Object PSObject -Property $dataRow 
                    #$queryResults += [pscustomobject]$dataRow
                    
                #}

            #$dataReaderSplit.Close();
            #$dataReaderSplitD.Close();
            }
            #break
    }





        }
      else
        {
          #   Write-Host "No Assets to Process" -ForegroundColor Red
             Write-Log -Message "No Assets to Process" -FgColor Red
        }
$endTime = Get-Date
#Write-Host -f Magenta "Data Download Ended At: $($endTime.ToString('dddd MMMM dd,yyyy HH:mm:ss tt'))"
Write-Log -Message "Hurray Data Download Ended On: $($endTime.ToString('dddd MMMM dd,yyyy HH:mm:ss tt'))" -FgColor Green
Exit;
