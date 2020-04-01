#/bin/bash
echo 'running create_creds.sh'
echo '{ "Forge": { "clientId": "$FORGE_CLIENT_ID", "clientSecret": "$FORGE_CLIENT_SECRET" } }' > appsettings.Local.json        
echo 'creata_creds.sh finished'