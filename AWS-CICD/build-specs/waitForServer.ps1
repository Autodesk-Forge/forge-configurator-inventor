[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
while ($statusCodeInt -ne 200) {
  $response = try {
    (Invoke-WebRequest -Uri 'https://localhost:5001' -UseBasicParsing -SslProtocol Tls12 -ErrorAction Stop)
  } catch [System.Net.WebException] {
    echo "An exception was caught: $($_.Exception.Message)"
    $_.Exception.Response
  }

  echo $response
  $statusCodeInt = [int]$response.BaseResponse.StatusCode
  # echo $statusCodeInt
  sleep 10
}
