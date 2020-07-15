# turn off warning about self-signed certificate

if (-not("dummy" -as [type])) {
    add-type -TypeDefinition @"
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public static class Dummy {
    public static bool ReturnTrue(object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors) { return true; }

    public static RemoteCertificateValidationCallback GetDelegate() {
        return new RemoteCertificateValidationCallback(Dummy.ReturnTrue);
    }
}
"@
}

[System.Net.ServicePointManager]::ServerCertificateValidationCallback = [dummy]::GetDelegate()

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
while ($statusCode -ne " 200") {
  $statusCode = try {
    $responce = $(Invoke-WebRequest -Uri 'https://localhost:5001' -UseBasicParsing -ErrorAction Stop)
    $responce.StatusCode
  } catch {
    404
  }

  Write-Host "StatusCode: --${statusCode}--"
  sleep 10
}
