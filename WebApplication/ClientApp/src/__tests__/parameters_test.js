/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Parameters panel');

Scenario('should check if Parameter panel has Cancel and Update button', async (I) => {

    // click on Model tab
    I.wait(3); // allow the projects to load
    I.click({xpath: "//ul/li[2]/div"});

    // check that Model tab has correct content
    I.see("Cancel",{xpath: '//div[2]/div/button[1]'});
    I.see("Update",{xpath: '//div[2]/div/button[2]'});
});

//ensure that Stripe panel is not diabled!!!
Scenario('should check if Stripe panel is displayed and hidden', async (I) => {

    // click on Model tab
    I.wait(3); // allow the projects to load
    I.click({xpath: "//ul/li[2]/div"});

    // check that Model tab has a parameter
    I.waitForElement('//*[@id="model"]/div/div[1]/div[1]/div[1]/div/input', 2);

    // change the parameter
    I.clearField('//div[1]/div[1]/div/input');
    I.fillField('//div[1]/div[1]/div/input', "12500 mm");

    // check if the Stripe element is displayed
    I.seeElement('//*[@id="model"]/div/div[2]/div[1]');

    // change the parameter
    I.clearField('//div[1]/div[1]/div/input');
    I.fillField('//div[1]/div[1]/div/input', "12000 mm");

    // check if the Stripe element was hidden
    I.waitForInvisible('//*[@id="model"]/div/div[2]/div[1]');
});


