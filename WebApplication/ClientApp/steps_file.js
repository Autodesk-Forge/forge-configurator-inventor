/* eslint-disable no-undef */
// in this file you can append custom step methods to 'I' object

require('dotenv').config();
const locators = require('./src/ui-tests/elements_definition.js');

//Authentivation
const inputUserName = '#userName';
const inputPassword = '#password';
const buttonNext = '#verify_user_btn';
const buttonSubmit = '#btnSubmit';

module.exports = function() {

  const forgeViewerSpinner = '//div[@id="ForgeViewer"]//div[@class="spinner"]';
  const userButton = '//button[@type="button" and (//span) and (//img)]';
  const loggedDemoToolUser = '//button[contains(@type, "button")]//img[contains(@alt, "Avatar image of Demo Tool")]';
  const loggedAnonymousUser = '//button[contains(@type, "button")]//img[contains(@alt, "Avatar image of Anonymous")]';
  const authorizationButton = '.auth-button';
  const loginName = process.env.SDRA_USERNAME;
  const password = process.env.SDRA_PASSWORD;

  // returns Project name locator
  function getProjectLocator(name)
  {
    return locate('li').find('span').withAttr({role: 'button'}).withText(name);
  }

  return actor({

    // Define custom steps here, use 'this' to access default methods of I.
    // It is recommended to place a general 'login' function here.

    // select a project according the project Name
    selectProject(name){

        // wait until project combo is displayed
        this.waitForElement( locators.xpComboProjects, 10);
        this.click( locators.xpComboProjects );

        // wait until project list is displayed
        this.waitForElement(locators.xpProjectList, 10);

        // emulate click to trigger project loading
        this.click( getProjectLocator(name));
    },
    clickToModelTab() { // we create this method because we need to wait for viewer - https://jira.autodesk.com/browse/INVGEN-41877
      // click on Model tab
      this.click(locators.modelTab);

      // wait for spinner element to be visible
      this.waitForVisible(forgeViewerSpinner, 15);

      // wait for spinner to be hidden
      this.waitForInvisible(forgeViewerSpinner, 30);
    },
    clickToAuthorizationButton(currentUser){ 
      // wait for User button
      this.waitForVisible(userButton,10);
      this.click(userButton);

      // wait for Authotization popUp dialog
      this.waitForVisible(authorizationButton, 10);

      // check the user name
      this.see(currentUser, '.username');

      // click on Authorization Button
      this.click(authorizationButton);
    },
    signIn(){
     // we use Autodesk Account //https://accounts.autodesk.com/

      this.clickToAuthorizationButton('Anonymous');

      // check it is Sigh-In page
      this.seeTitleEquals('Sign in');
      this.waitForElement(inputUserName, 10);

      // specify Sign-in Email
      this.fillField(inputUserName, loginName);
      this.click(buttonNext);

      //specify Sign-in password
      this.waitForVisible(inputPassword, 10);
      this.fillField(inputPassword, password);
      this.click(buttonSubmit);

      this.waitForElement(loggedDemoToolUser, 10);
    },
    signOut(){
      this.clickToAuthorizationButton('Demo Tool');

      this.waitForElement(loggedAnonymousUser, 10);
    }

  });
}
