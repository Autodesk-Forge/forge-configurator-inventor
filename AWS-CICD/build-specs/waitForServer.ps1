[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
while ($statusCode -ne 200) {
  $statusCode = try {
    (Invoke-WebRequest -Uri 'https://localhost:5001' -UseBasicParsing -ErrorAction Stop).BaseResponce.StatusCode
  } catch [System.Net.WebException] {
    echo "An exception was caught: $($_.Exception.Message)"
    if ($_.Exception.Message.IndexOf("The underlying connection was closed") -eq 0) {
      200
    } else {
      0
    }
  }

  echo "StatusCode:"
  echo $statusCode
  sleep 10
}
