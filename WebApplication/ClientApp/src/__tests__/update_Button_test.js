/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Update Button');

//ensure that Stripe panel is not diabled!!!
Scenario('should check if Update button displays a message', (I) => {

    // click on Model tab
    I.wait(3); // allow the projects to load
    I.click({xpath: "//ul/li[2]/div"});

    // Click on Update button
    I.see("Update", '//div[2]/div/button[2]');
    I.click('//div[2]/div/button[2]');

    // TO DO
    // check an action after Click on Update button - not implemneted yet

});