# Embedded version of the application

The application can be run in embedded mode that allows it to be easily integrated into existing solutions. In this mode, most of the UI and interaction is removed and only essential funcionality remains. You can still interact with the model but cannot change its parameters directly using UI. Instead you are able to provide the target parameters as part of JSON object that is passed into the application as a get URL parameter. This funcionality extends the use cases of the original app and allows your solutions to control the show content programatically.

## Setup

After following the basic applications setup and initialization steps, enabling the embedded mode is simple, just add `"embedded" : true` to your appsettings.json (or appsettings.local.json)

## Usage

In order to get the embedded mode working you need to provide the application with a get parameter with an URL that points to a JSON object containing the model link and its parameters.
As an example we can use the wrench project (One of the applications default projects) and set its Wrench Size to Large:

```
{
	"Url": "https://sdra-default-projects.s3-us-west-2.amazonaws.com/Wrench_2021.zip",
	"Name": "TestWrench 30",
	"TopLevelAssembly": "Wrench.iam",
	"Config": {
		"WrenchSz": {
			"value": "\"Large\""
		},
		"JawOffset": {
			"value": "11 mm"
		},
		"PartMaterial": {
			"value": "\"Steel\""
		},
		"iTrigger0": {
			"value": "2 ul"
		}
	}
}
```

For purpose of this guide we have already uploaded the JSON file to S3: https://inventorio-dev-holecep.s3-us-west-2.amazonaws.com/Interaction/wrench.json

We can now start the application (make sure embedded mode is enabled) and go to the browser. Assuming that we run the application locally and with default settings we can now call the embedded application with our model like this:

https://localhost:5001/?url=https://inventorio-dev-holecep.s3-us-west-2.amazonaws.com/Interaction/wrench.json

After short processing, you will see your model loaded inside of the embedded window. 