const { setHeadlessWhen } = require('@codeceptjs/configure');

// turn on headless mode when running with HEADLESS=true environment variable
// HEADLESS=true npx codecept run
setHeadlessWhen(process.env.HEADLESS);

//waitForNavigation: "networkidle0",
//waitForAction: 500,
//restart: false,

const chromiumArgs = [
  '--disable-web-security',
  '--ignore-certificate-errors',
  '--disable-infobars',
  '--allow-insecure-localhost',
  '--disable-device-discovery-notifications',
  '--window-size=1920,1080',
  '--window-posizition=200,0',
  '--no-sandbox'
];

exports.config = {
  tests: './src/ui-tests/*_test.js',
  output: './output',
  helpers: {
    Playwright: {
      url: 'http://localhost:80',
      show: true,
      browser: 'chromium',
      chromium: {args: chromiumArgs},
      waitForTimeout: 30000,
      MyHelper: {
        require: './src/ui-tests/helpers/playwright'
      }
    }
  },
  include: {
    I: './steps_file.js'
  },
  bootstrap: null,
  teardown: require("./teardown.js"),
  mocha: { bail: true },
  name: 'ClientApp',
  plugins: {
    retryFailedStep: {
      enabled: true
    },
    screenshotOnFail: {
      enabled: true
    }
  }
}