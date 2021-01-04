# Embedded version of the application

The application can be run in embedded mode that allows it to be easily integrated into existing solutions. In this mode, the projects, parameters and user management UI is removed and only essential functionality remains. You can still upload any model and change its parameters, but instead of doing so using UI, you do so by passing the data through a JSON object that is passed into the application. This functionality extends the use cases of the original app and allows your solution to control the shown content programmatically.

## Features available in the emebbed mode

- Model viewer tab without model parameters
- BOM tab
- Drawing tab
- Downloads tab with all the appropriate downloads (Note: drawings and RFA are processed on the server upon first download. They are cached after the processing)

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

For purpose of this guide we have already uploaded the JSON file to S3: https://inventorio-dev-holecep.s3.us-west-2.amazonaws.com/Interaction/wrench_v2.json

We can now start the application (make sure embedded mode is enabled) and go to the browser. Assuming that we run the application locally and with default settings we can now call the embedded application with our model like this:

https://localhost:5001/?url=https://inventorio-dev-holecep.s3.us-west-2.amazonaws.com/Interaction/wrench_v2.json

After the processing, you will see your model loaded inside of the embedded window. 

## Consistency
The project contained in the JSON structure is uploaded only once and then it is cached alongside the parameters. Consequent calls with the same JSON parameters will result in cache hit and thus fast processing time. You can also modify the JSON data on the url and then the project will undergo update accordingly without the need to upload the project again.