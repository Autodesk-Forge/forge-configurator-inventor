# Forge Configurator Inventor
Demo application showcasing Configuration with Design Automation for Inventor

![thumbnail](/img/thumbnail.gif)

## Architecture

![thumbnail](/img/architecture.png)

## Prerequisites

### Web Application 
1. .NET Core 3.1
2. Node.js    
	**Important note:** In Node.js even releases are stable, and odd-numbered versions are unstable. That's said we won't support or guarantee the work of the application using the odd-numbered Node.js versions. Currently the latest version we support is the even version **v16.13.2**. Unless version **v18** will be released we are not supporting versions newer than **v16**.
3. (recommended) Visual Studio Code with extensions:
    * [Debugger for Chrome](https://marketplace.visualstudio.com/items?itemName=msjsdiag.debugger-for-chrome) (for debugging client side code)
    * [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint)
    * [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode)
    * [psioniq File Header](https://marketplace.visualstudio.com/items?itemName=psioniq.psi-header) (to insert copyright header)
### App Bundles
1. Autodesk Inventor 2021
1. Visual Studio 2019

## Run sample for the first time
1. Clone repository
1. Create a forge app at https://forge.autodesk.com/, and select `Design Automation API` and `Data Management API` in the **APIs** section
![thumbnail](/img/APIs.png)
1. Enter https://localhost:5001 as the callback URL
1. Note the `Client ID` and `Client Secret` generated
1. Specify **Forge credentials** using **one** of the following approaches:\
    **a)** Set environment variables `FORGE_CLIENT_ID` and `FORGE_CLIENT_SECRET`\
    **b)** Set environment variables `Forge__ClientId` and `Forge__ClientSecret`\
    **c)** Make a copy of `appsettings.Local.template.json` in the `webapplication` directory named `appsettings.Local.json` and replace the `<clientId>` and `<clientSecret>` in it\
    **d)** (not on dev machine) Modify `appsettings.json` (or `appsettings.<ENVIRONMENT>.json`) with the template mentioned in **c)**\
    **e)** Set environment variable 'set NODE_ENV=development'

1. Make a copy of `InventorBinFolder.props.template` in the top `AppBundles` folder named `InventorBinFolder.props` and replace the `PATH_TO_YOUR_INVENTOR_BIN` string in it with your actual Inventor bin folder path, for example: `C:\Program Files\Autodesk\Inventor 2021\Bin`
1. (Optional) Choose network configuration for your application. By default polling is enabled as it offers an easier way to setup and run the application. This is OK for locally run applications and debugging. However
   in production using the new callback option is highly recommended to conserve resources. In order to enable the callback option modify the `Publisher` section of the appsettings.json file. 
   Change `"CompletionCheck"` value from `"Polling"` to `"Callback"` and set `"CallbackUrlBase"` url to your server URL or ngrok tunnel URL for a locally run application.
   To run and debug callbacks locally please refer to the [ngrok section](#Use-ngrok-for-localhost-callbacks)
1. *(Optional) Specify if access should be limited in `webapplication\appsettings.json`. Set `Enabled` to `true` or `false`, and populate the `Domains` and `Addresses` fields with comma delimited lists such as `["autodesk.com", "company.com"]` and `["person@company2.com", "person@company3.com"]`*
1. Open the `forge-configurator-inventor.sln` file with **Visual Studio 2019** and build the solution
![thumbnail](/img/BuildSolution.png)\
This will also generate the **zip** files of all the **app bundles** that the **web app** will need to upload to the **Design Automation** server\
![thumbnail](/img/AppBundleZips.png)\
When building the solution, make sure that all those **app bundles** got generated successfully
![thumbnail](/img/SuccessfulBuild.png)\
Now we have to initialize things both on the **Design Automation** server and locally. In the **terminal** (in **Visual Studio** or outside) navigate to the `webapplication` folder and run `dotnet run initialize=true`
![thumbnail](/img/DotnetRunInitialize.png)\
Once the output reaches the `Now listening on: https://localhost:5001` line just open that **URL** in your browser\
**Next time** you'll also be able to start the app from the **Visual Studio** debugger. 
In the **Debug** drop-down menu select `forge-configurator-inventor` and start debugging
![thumbnail](/img/DebugApp.png)
This time your browser should open up **automatically** showing the correct **URL** 

## Build

* Building the projects also installs required packages (this can take several minutes).

### Web Application and App Bundles
* Open the `forge-configurator-inventor.sln` file with **Visual Studio 2019** and build the solution

### Web Application Alone
* From a command prompt, go to the `webapplication` directory, and run `dotnet build`.

## (Optional) Update the npm packages
* If you are not running the Application for the first time, but rather getting an update, you may need to install npm packages that were added since your last successfull run:
1.  Using command line go to `webapplication/ClientApp` and run `npm install`. See [Adding npm package](#Add-npm-package-to-project) for more information.

## Run The Web Application Without Debugging
### Clear and load initial data during app launch time
 - Create initial data: from the `webapplication` directory, run `dotnet run initialize=true`
 - Clear data: from the `webapplication` directory, run `dotnet run clear=true`
 - Clear and then load initial data: from the `webapplication` directory, run `dotnet run initialize=true clear=true`
 - When the app finishes the initialization process it remains running and expects client calls. You can leave it running and follow by [opening the site](#open-site) or stop it and move to the the [Debugging section](#Debug-The-Web-Application-With-VS-Code)
### Run after initial data is created
 - From a command prompt, go to the `webapplication` directory, and run `dotnet run`
### Open site
 - Navigate to https://localhost:5001
     * You may need to refresh the browser after it launches if you see the error `This site can't be reached`
    * If you see the error `Your Connection is not private`, click `Advanced` and then `Proceed to localhost (unsafe)`. This is due a development certificate being used.

## Debug The Web Application With VS Code

1. Make sure that application is fully initialized, before you start debugging session. Please see the [Clear and load initial data](#Clear-and-load-initial-data-during-app-launch-time)
1. Open the repository root folder in VS Code
1. In the Run tab, select the `Server/Client` configuration and click the "Start Debugging" (arrow) button
    * Some browser errors are normal, see [open site](#open-site)
    * Disregard C# errors related to AppBundles in VS Code

## Run/Debug Tests
### Backend
* Note that running the tests clears initialization data, so you will either need to change forge credentials before running them, or run the initializer again afterward. See [Clear and load initial data...](#clear-and-load-initial-data-during-app-launch-time)
1. From Visual Studio 2019
    * Open Test Explorer and select tests to run or debug
1. From Visual Studio Code
    * Open a test file in the `webapplication.Tests` directory and click on either `Run Test` or `Debug Test` above one of the methods decorated with the `[Fact]` attribute. Or, above the class declaration click on either `Run All Tests` or `Debug All Tests`
1. From the command line, in either the root or `webapplication.Tests` directory run `dotnet test`
### Frontend
1. In Visual Studio Code, on the Run tab, select the `Debug Jest All` configuration and click the "Start Debugging" (arrow) button
    * Note that once you run the tests they will only run again if they changed since the last time
1. Alternatively, using the command line go to webapplication/ClientApp and execute `npm test`
### UI Tests
* For UI tests we are using `CodeCeptJs` framework. All tests are stored in `ClientApp/src/ui-tests/` and we filter all files end with `*_test.js`. 
* Set environment variables `SDRA_USERNAME` and `SDRA_PASSWORD` for `Sign-in` workflow. We are using Autodesk Account credentials for `Sign-in`.
* Set `"embedded" : true` In `appsettings.Local.json` file mentioned in `Run sample for the first time` 5c)
    * Also you can create a `.env` file in the `webapplication/ClientApp` directory to define the environment variables - for more details follow this link: https://www.npmjs.com/package/dotenv
* Note that the server needs to be running for these tests

1. From the `webapplication/ClientApp` directory:
    * For all UI tests Run this command: `npx codeceptjs run` or `npm run uitest`.
    * For particular file you can use this command: `npx codeceptjs run src/ui-tests/<test file name>`

## Backend
We are using the forge service on the backend https://forge.autodesk.com/

### .NET Core
We are using the latest version of .NET Core (3.1 at the time of writing) https://dotnet.microsoft.com/download/dotnet-core/3.1

The project was initally created using the command `dotnet new react`

### Forge Design Automation
https://forge.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/

C# SDK https://github.com/Autodesk-Forge/forge-api-dotnet-design.automation

We are using the Inventor and Revit engines.

### OSS
Used for storing your designs. For communication with Forge Design Automation

https://forge.autodesk.com/en/docs/data/v2/developers_guide/overview/

C# SDK https://github.com/Autodesk-Forge/forge-api-dotnet-client

## Client app

### React
https://reactjs.org/

### Redux
https://redux.js.org/

We are using redux-thunk for complex and asynchronous operations https://github.com/reduxjs/redux-thunk

#### Redux DevTool
In `index.js` replace
```js
const store = createStoreWithMiddleware(mainReducer);
```
with
```js
const store = createStoreWithMiddleware(mainReducer, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__());
```

### Autodesk HIG React

https://github.com/Autodesk/hig

Table is not part of todays React HIG implementation so we will use https://github.com/Autodesk/react-base-table

## How to

### Add npm package to project

We are using npm.

1. Using command line go to `webapplication/ClientApp` and run `npm install <package>`
    * Note that packages are normally installed as part of the build, but only if the `npm_modules` directory is not found. This means that when new packages are added, `webapplication/ClientApp/npm install` needs to be run again manually by other users (who did not add the new package).

### Manually run linter
* For JavaScript code: `npm run lint`
* For CSS: `npm run lint-css`

### Deploy
* For an advanced example of CI/CD on AWS, see [AWS-CICD](AWS-CICD/README.md)
* For a simple method of deploying to Azure, see [Publish a Web app to Azure App Service using Visual Studio](https://docs.microsoft.com/en-us/visualstudio/deployment/quickstart-deploy-to-azure?view=vs-2019)
    * First change `webapplication.Program.cs` by removing the `UseKestrel()` statement
    * You will need to change the callback in your forge app to match the URL you deploy to.
* Example of the real-life deploy of an **FDA** as an [App Service](https://github.com/Autodesk-Forge/forge-configurator-inventor/wiki/AppServiceDeploy)
	
### Project file zip encoding
* Uploading your project file can cause the error "Project name or assembly contains unsupported characters". This problem typically happens when you use non Latin characters in your project file names and the zip file does not use UTF-8 encoding for the project file names.
* This issue can be fixed by forcing your zip program of choice to zip the file with the UTF-8 encoding
* For 7-zip, you can follow these simple steps:
	* After selecting your files for zipping, right-click them and select 7-zip from the context menu
	* Select option "Add to archive..."
	* On the very bottom you can see a text field labelled "Parameters:"
	* Write "cu" without the quotes into that field
	* Press ok
	* Name your resulting zip file
	
* For more information on this issue please refer to the [FDA troubleshooting page](https://forge.autodesk.com/en/docs/design-automation/v3/developers_guide/troubleshooting/)

### Use ngrok for localhost callbacks
* If you choose webhook callback network configuration for the application, you will need a way for the callbacks to get from the FDA servers to your local machine.
* One of the tools that can assist you with this task is ngrok https://ngrok.com/ 
* These steps should help you to set up an ngrok tunnel to your localhost:
	* Create a free ngrok account
	* Download the ngrok executable from https://dashboard.ngrok.com/get-started/setup
	* Unzip the executable
	* (Optional) put the path to the ngrok executable in your system path to make it accessible from everywhere
	* Notice section 2. of the download page. This contains your credentials and full authenticate command for you to copy paste
	* Use the full authentication command from previous step - this only needs to be done once
	* Run ngroc with the format of `ngrok http port` and set the port number the web application uses on your local machine
	* With default settings the command would look like this: `ngrok http 5001` 
	* You are now ready to use and debug callbacks locally
	* If you experience issues running ngrok tunnel with the web application using https settings, the simple workaround is to switch the app to http mode (only for local use). 
	* In order to set the callback URL for local development it is recomended to create an appsettings.Local.json file in the webapplication directory (if you don't have it already) and then put following settings into it:
      ```json		  
      {
        "Publisher": {
            "CompletionCheck": "Callback",
            "CallbackUrlBase": "<YOUR NGROK URL>"
	    }
      }
      ```
### Embedded mode
You can read about embedded mode [here](EMBEDDED-README.md)
	
