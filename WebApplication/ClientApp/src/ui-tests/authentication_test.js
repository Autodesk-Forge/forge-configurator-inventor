	/* eslint-disable no-undef */
    const assert = require('assert');
    const avatarElement = '//button[@type="button"]//span[contains(@role, "img")]';
    const avatarName = 'Demo Tool';
    const avatarAnonymous = 'Anonymous';

    Before((I) => {
        I.amOnPage('/');
    });

    Feature('Authentication');

    Scenario('check Sign-in workflow', async (I) => {

        I.signIn();

        // check Avatar name
        I.waitUntil(() => document.readyState == "complete", 10);
        I.wait(10);
        const attr = await I.grabAttributeFrom(avatarElement, 'aria-label');
        assert.equal(attr.toString().includes(avatarName), true, 'Incorrect Sign-in process! Avatar "adsk.inv" is not Singed-in!');

    });

    Scenario('check Sign-out workflow', async (I) => {
        I.signIn();
        I.waitUntil(() => document.readyState == "complete", 10);
        I.signOut();

        // check Avatar name
        I.waitUntil(() => document.readyState == "complete", 10);
        const attr = await I.grabAttributeFrom(avatarElement, 'aria-label');
        assert.equal(attr.toString().includes(avatarAnonymous), true, 'Incorrect Sign-out process!');

    });
