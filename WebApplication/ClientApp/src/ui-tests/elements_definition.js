/* eslint-disable no-undef */

//If the locator is an object, it should have a single element,
// with the key signifying the locator type (id, name, css, xpath, link, react, or class)
// and the value being the locator itself. This is called a "strict" locator.
// https://codecept.io/locators/#css-and-xpath

module.exports = {
     xpButtonReset : locate('button').find('span').withText('Reset'),
     xpButtonUpdate : locate('button').find('span').withText('Update'),
     ForgeViewer : '#ForgeViewer',
     xpLinkAdskForge : '//a[@href="https://forge.autodesk.com"]',
     xpComboProjects : '//div[@role="button"] //*[local-name()="svg"]',
     xpProjectConveyor : '//li[contains(@role,"menuitem") and .//span[contains(., "Conveyor")]]',
     xpProjectWrench : '//li[contains(@role,"menuitem") and .//span[contains(., "Wrench")]]',
     xpProjectList : '//ul//span[text()="Projects"]',
     xpButtonLog : '//button[contains(@title, "Log")]',
     xpStripeElement : '//p[contains(text(),"The assembly is out-of-date.")]',
     PrametersList : '.parameters',
     ParametersContainer : '.parametersContainer',
     xpPopUpLog : '//div[contains(h3, "Navigation Action")]',
     xpViewerCanvas : '//*[@id="ForgeViewer"] //canvas',
     projectsTab : locate('li').find('p').withText('Projects'),
     modelTab : locate('li').find('p').withText('Model'),
     bomTab : locate('li').find('p').withText('BOM'),
     drawingTab : locate('li').find('p').withText('Drawing'),
     downloadsTab : locate('li').find('p').withText('Downloads'),
};