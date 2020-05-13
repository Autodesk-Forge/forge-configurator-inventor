const { setHeadlessWhen } = require('@codeceptjs/configure');

// turn on headless mode when running with HEADLESS=true environment variable
// HEADLESS=true npx codecept run
setHeadlessWhen(process.env.HEADLESS);

const chromiumArgs = [
  '--disable-web-security',
  '--ignore-certificate-errors',
  '--disable-infobars',
  '--allow-insecure-localhost',
  '--disable-device-discovery-notifications',
  '--window-size=1920,1080',
  '--window-posizition=200,0'
];

exports.config = {
  tests: './*_test.js',
  output: './output',
  helpers: {
    Playwright: {
      url: 'http://localhost:5001',
      show: true,
      browser: 'chromium',
      restart: false,
      waitForNavigation: "networkidle0",
      waitForAction: 500,
      chromium: {args: chromiumArgs}
    }
  },
  include: {
    I: './steps_file.js'
  },
  bootstrap: null,
  mocha: {},
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