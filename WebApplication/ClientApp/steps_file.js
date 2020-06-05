/* eslint-disable no-undef */
// in this file you can append custom step methods to 'I' object

module.exports = function() {
  const projectsCombo = locate('div').withAttr({role: 'button'});
  const projectList = locate('span').withText('Projects');


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
    }

  });
}
