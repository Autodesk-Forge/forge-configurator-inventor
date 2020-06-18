	/* eslint-disable no-undef */
    const avatarAnonymous = '//button[@type="button"]//span[contains(@role, "img") and contains(@aria-label, "Avatar for Anonymous")]';
    const avatarDemoTool = '//button[@type="button"]//span[contains(@role, "img") and contains(@aria-label, "Avatar for Demo Tool")]';


    Before((I) => {
        I.amOnPage('/');
    });

    Feature('Authentication');

    Scenario('check Sign-in and Sign-out workflow', async (I) => {

        I.signIn();

        //wait for logged 'Demo Tool' user
        I.waitForElement(avatarDemoTool, 10);

        I.signOut();

        // wait for Anonymous user
        I.waitForElement(avatarAnonymous, 10);
    });
