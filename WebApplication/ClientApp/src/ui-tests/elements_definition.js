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
     xpComboProjects : './/div[contains(@role,"button") and (.//p[contains(., "Conveyor")] or .//p[contains(., "Wrench")])]',
     xpProjectConveyor : '//li[contains(@role,"menuitem") and .//span[contains(., "Conveyor")]]',
     xpProjectWrench : '//li[contains(@role,"menuitem") and .//span[contains(., "Wrench")]]',
     xpProjectList : '//ul[contains(.//span, "Projects")]',
     xpButtonLog : '//button[contains(@title, "Log")]',
     xpStripeElement : '//div[contains(@role,"alert") and .//*[local-name()="svg"]]',
     PrametersList : '.parameters',
     ParametersContainer : '.parametersContainer',
     xpPopUpLog : '//div[contains(h3, "Navigation Action")]',
     xpMenuProjectsName : '//ul[contains(.//span, "Projects")]',
     xpViewerCanvas : '//*[@id="ForgeViewer"] //canvas',
     projectsTab : locate('li').find('p').withText('Projects'),
     modelTab : locate('li').find('p').withText('Model'),
     bomTab : locate('li').find('p').withText('BOM'),
     drawingTab : locate('li').find('p').withText('Drawing'),
     downloadsTab : locate('li').find('p').withText('Downloads'),
};