/* eslint-disable no-console */
/* eslint-disable no-constant-condition */
/* eslint-disable no-undef */
// in this file you can append custom step methods to 'I' object

require('dotenv').config();
const locators = require('./src/ui-tests/elements_definition.js');

//Authentivation
const inputUserName = '#userName';
const inputPassword = '#password';
const buttonNext = '#verify_user_btn';
const buttonSubmit = '#btnSubmit';

// Upload
const uploadPackageButton = '//button[@title="Upload package"]';
const uploadPackageRoot = '//input[@id="package_root"]';
const uploadFileElement = 'input[id="packageFileInput"]';
const uploadButton = '//button[@id="upload_button"]';
const uploadConfirmationDialog = '//p[text()="Upload Finished"]';
const closeButton = '//button[@title="Close"]';

// Delete
const projectRow = '//div[div/div/text()="ProjectName"]';
const checkBox = '//input[@id="checkbox_row"]';
const deleteProjectButton = '//button[@title="Delete project(s)"]';
const confirmDelete = '//button[@id="delete_ok_button"]';

module.exports = function() {

  const forgeViewerSpinner = '//div[@id="ForgeViewer"]//div[@class="spinner"]';
  const userButton = '//button[@type="button" and (//span) and (//img)]';
  const loggedAnonymousUser = '//button[contains(@type, "button")]//img[contains(@alt, "Avatar image of Anonymous")]';
  const authorizationButton = '.auth-button';
  let loginName = process.env.SDRA_USERNAME;
  let password = process.env.SDRA_PASSWORD;
  const allowButton = '#allow_btn';

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

      // wait for Authorization popUp dialog
      this.waitForVisible(authorizationButton, 10);

      // check the user name
      this.see(currentUser, '.username');

      // click on Authorization Button
      this.click(authorizationButton);
    },
    async signIn(){
     // we use Autodesk Account credentials //https://accounts.autodesk.com/

      this.clickToAuthorizationButton('Anonymous');

      // check it is Sign-In page
      this.seeTitleEquals('Sign in');
      this.waitForElement(inputUserName, 10);

      // specify Sign-in Email
      this.fillField(inputUserName, loginName);
      this.click(buttonNext);

      // specify Sign-in password
      this.waitForVisible(inputPassword, 10);
      this.fillField(inputPassword, password);
      this.click(buttonSubmit);

      // look for the URL to determine if we are asked 
      // to agree to authorize our application
      this.waitForNavigation();
      const currentUrl = await this.grabCurrentUrl()
      console.log(currentUrl);
      if (currentUrl.includes('auth.autodesk.com')) {
        // click on Allow Button
        this.waitForVisible(allowButton, 15);
        this.click(allowButton);
      }

      // check logged user
      this.waitForElement(userButton, 10);
      this.dontSeeElement(loggedAnonymousUser);
    },
    signOut(){
      this.clickToAuthorizationButton('Demo Tool');

      // check if Anonymous user is signed
      this.waitForElement(loggedAnonymousUser, 10);
    },
    uploadProject(projectZipFile, projectAssemblyLocation) {
      // invoke upload UI
      this.waitForVisible(uploadPackageButton);
      this.click(uploadPackageButton);

      // select file to upload
      this.attachFile(uploadFileElement, projectZipFile);
      this.fillField(uploadPackageRoot, projectAssemblyLocation)

      // upload the zip to server
      this.click(uploadButton);

      // Wait for file to be uploaded
      this.waitForVisible(uploadConfirmationDialog, 120);
      this.click(closeButton);
    },
    deleteProject(projectName) {
      // hover above project
      let projectRowWithName = projectRow.replace('ProjectName', projectName);
      this.waitForVisible(projectRowWithName, 10);
      this.moveCursorTo(projectRowWithName);

      // click the checkbox to select projetc
      this.waitForVisible(checkBox);
      this.click(checkBox);

      // click the delete button
      this.waitForVisible(deleteProjectButton);
      this.click(deleteProjectButton);

      // confirm delete
      this.waitForVisible(confirmDelete);
      this.click(confirmDelete);

      // wait for project disapear from the list
      this.waitForInvisible(projectRowWithName, 60);
    }
  });
}
