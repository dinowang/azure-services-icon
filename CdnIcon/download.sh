#!/bin/bash

response=$(az account get-access-token)
token=$(echo $response | jq ".accessToken" -r)
# echo $token

# curl -X -O GET -H "Authorization: Bearer $token" -H "Content-Type: application/octet-stream" https://cds-icons.azurewebsites.net/downloads/All_Icons.zip

# az account get-access-token
wget --header="Authorization: Bearer $token" https://cds-icons.azurewebsites.net/downloads/All_Icons.zip -O All_Icons.zip 