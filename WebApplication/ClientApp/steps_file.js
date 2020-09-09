/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

/* eslint-disable prefer-const */
/* eslint-disable jest/no-standalone-expect */
/* eslint-disable no-console */
/* eslint-disable no-constant-condition */
/* eslint-disable no-undef */
// in this file you can append custom step methods to 'I' object

require('dotenv').config();
const locators = require('./src/ui-tests/elements_definition.js');

//Authentication
const inputUserName = '#userName';
const inputPassword = '#password';
const buttonNext = '#verify_user_btn';
const buttonSubmit = '#btnSubmit';

// Upload
const uploadPackageButton = '//button[@title="Upload package"]';
const uploadPackageRoot = '//div[@id="package_root"]';
const uploadFileElement = '//input[@id="packageFileInput"]';
const uploadButton = '//button[@id="upload_button"]';
const uploadConfirmationDialog = '//p[text()="Upload Finished"]';
const uploadFailedDialog = '//p[text()="Upload Failed"]';
const uploadFailLogLink = '//*[contains(@href,"report.txt")]';
const closeButton = '//button[@title="Close"]';

// Delete
const projectRow = '//div[div/div/text()="ProjectName"]';
const checkBox = '//input[@id="checkbox_row"]';
const deleteProjectButton = '//button[@title="Delete project(s)"]';
const confirmDelete = '//button[@id="delete_ok_button"]';

// Set param value
const paramsInput = '//div[@class="parameter"][text()="ParamName"]//input';

module.exports = function() {

  const forgeViewerSpinner = '//div[@id="ForgeViewer"]//div[@class="spinner"]';
  const userButton = '//button[@type="button" and (//span) and (//img)]';
  const loggedAnonymousUser = '//button[contains(@type, "button")]//img[contains(@alt, "Avatar image of Anonymous")]';
  const authorizationButton = '.auth-button';
  const loginName = process.env.SDRA_USERNAME;
  const password = process.env.SDRA_PASSWORD;
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
    clickToAuthorizationButton(){
      // wait for User button
      this.waitForVisible(userButton,10);
      this.click(userButton);

      // wait for Authorization popUp dialog
      this.waitForVisible(authorizationButton, 10);

      // click on Authorization Button
      this.click(authorizationButton);
    },
    async signIn(){
     // we use Autodesk Account credentials //https://accounts.autodesk.com/

      this.clickToAuthorizationButton();

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
      const currentUrl = await this.grabCurrentUrl();
      if (currentUrl.includes('auth.autodesk.com')) {
        // click on Allow Button
        this.waitForVisible(allowButton, 15);
        this.click(allowButton);
      }

      // check logged user
      this.waitForInvisible(loggedAnonymousUser, 30);

    },
    signOut(){
      this.clickToAuthorizationButton();

      // check if Anonymous user is signed
      this.waitForElement(loggedAnonymousUser, 10);
    },
    uploadProject(projectZipFile, projectAssemblyLocation) {
      // invoke upload UI
      this.waitForVisible(uploadPackageButton);
      this.click(uploadPackageButton);

      // select file to upload
      this.attachFile(uploadFileElement, projectZipFile);
      this.click(uploadPackageRoot); // click on combo
      this.fillField(uploadPackageRoot, projectAssemblyLocation); // filter to assembly
      this.pressKey('Enter'); // confirm selection

      // upload the zip to server
      this.click(uploadButton);

      // Wait for file to be uploaded
      this.waitForVisible(uploadConfirmationDialog, locators.FDAActionTimeout);
      this.click(closeButton);
    },
    uploadIPTFile(IPT_File) {
      // invoke upload UI
      this.waitForVisible(uploadPackageButton);
      this.click(uploadPackageButton);

      // select file to upload
      this.attachFile(uploadFileElement, IPT_File);

      // upload the zip to server
      this.click(uploadButton);

      // Wait for file to be uploaded
      this.waitForVisible(uploadConfirmationDialog, locators.FDAActionTimeout);
      this.click(closeButton);
    },
    uploadInvalidIPTFile(IPT_File) {
      // invoke upload UI
      this.waitForVisible(uploadPackageButton);
      this.click(uploadPackageButton);

      // select file to upload
      this.attachFile(uploadFileElement, IPT_File);

      // upload the zip to server
      this.click(uploadButton);

      // Wait for Upload Failed dialog
      this.waitForVisible(uploadFailedDialog, locators.FDAActionTimeout);
      // check log url link
      this.seeElement(uploadFailLogLink);

      this.click(closeButton);
    },
    deleteProject(projectName) {
      // hover above project
      const projectRowWithName = projectRow.replace('ProjectName', projectName);
      this.waitForVisible(projectRowWithName, 10);
      this.moveCursorTo(projectRowWithName);

      // click the checkbox to select project
      this.waitForVisible(checkBox);
      this.click(checkBox);

      // click the delete button
      this.waitForVisible(deleteProjectButton);
      this.click(deleteProjectButton);

      // confirm delete
      this.waitForVisible(confirmDelete);
      this.click(confirmDelete);

      // wait for project disappear from the list
      this.waitForInvisible(projectRowWithName, 60);
    },
    setParamValue(paramName, paramValue) {
      const paramsInputWithName = paramsInput.replace('ParamName', paramName);
      this.waitForVisible(paramsInputWithName, 10);
      this.clearField(paramsInputWithName);
      this.fillField(paramsInputWithName, paramValue);
    },
    updateProject() {
      // Click on Update button
      this.waitForVisible(locators.xpButtonUpdate, 10);
      this.click(locators.xpButtonUpdate);

      // wait for progress bar shows and disappears
      const updatingProjectProgress = '//p[text()="Updating Project"]';
      this.waitForVisible(updatingProjectProgress, 10);
      this.waitForVisible(locators.xpButtonDone, locators.FDAActionTimeout);
      this.click(locators.xpButtonDone);
      this.waitForInvisible(updatingProjectProgress, 10);
    },
    waitForForgeViewerToPreventItFromCrashing(timeout)
    {
      this.waitForElement('//div[@id="ForgeViewer"] //div[@class="viewcube"]', timeout);
    },
    goToDrawingTab()
    {
      this.waitForElement(locators.drawingTab, 5);
      this.click(locators.drawingTab);
    },
    goToDownloadsTab()
    {
      this.waitForElement(locators.downloadsTab, 5);
      this.click(locators.downloadsTab);
    }
  });
};
