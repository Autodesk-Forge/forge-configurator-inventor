/* eslint-disable no-undef */
// in this file you can append custom step methods to 'I' object

const locators = require('./src/ui-tests/elements_definition.js');

module.exports = function() {
  const projectsCombo = '//div[@role="button"] //*[local-name()="svg"]';
  const projectList = locate('span').withText('Projects');
  const forgeViewerSpinner = '//div[@id="ForgeViewer"]//div[@class="spinner"]';

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
        this.waitForElement( projectsCombo, 10);
        this.click( projectsCombo );

        // wait until project list is displayed
        this.waitForElement(projectList, 10);

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
    }

  });
}
