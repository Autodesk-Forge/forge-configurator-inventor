while ($response.StatusCode -ne 200) {
  try { 
    $response = Invoke-WebRequest https://localhost:5001
  } catch {
  }
}
