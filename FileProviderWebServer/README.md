# Sample CPQ Application

## Build and run the application using installed .NET SDK
Application can be build and run from the command line executing the following commands in the folder `FileProviderWebServer`
```
dotnet build
dotnet run
```
Application can be shut down by pressing _Ctrl+C_ in the command line window.

## Build and run the application using Docker
Application is dockerized and can be easily build and run using the following way:
```
docker-compose build
docker-compose up
```
Run the following command to shut down the webapplication:
```
docker-compose down
```

## Use the web server
The application is available on this URL: [http://localhost:5080/fileprovider/index](http://localhost:5080/fileprovider/index)

### Change the config file
JSON config file can be updated by filling the text area and submitting the html form. The result is shown in the bottom frame.
