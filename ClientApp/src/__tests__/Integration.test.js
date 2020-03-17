const playwright = require('playwright');

const pageUrl = "https://localhost:5001"

const config = { 
    // headless: false, // NOTE: uncomment to see what is actually going o the screen
    args: [
        // to avoid errors about self-signed SSL certificate: https://stackoverflow.com/q/55207690
        '--ignore-certificate-errors',
        '--ignore-certificate-errors-spki-list', 
    ]};


describe('Integration UI tests', () => {

    // give browser more time to start (https://stackoverflow.com/a/49864436)
    jest.setTimeout(30000);

    let browser = null;
    let page = null;

    beforeAll(async () => {

        // TODO: try different browser kinds
        browser = await playwright["chromium"].launch(config);
        page = await browser.newPage();

        if (!page) {
            throw new Error("Connection wasn't established");
        }

        // Open the page
        await page.goto(pageUrl, { waitUntil: "networkidle0" });
    });

    afterAll(async () => {
        await browser.close();
    });

    it(`should check click on button`, async () => {

        // check the page is loaded
        expect(page).not.toBeNull();
        expect(await page.title()).not.toBeNull();

        // check initial button title
        const buttonTitle = await page.evaluate(() => document.querySelector("button span").textContent);
        expect(buttonTitle).toBe("I am Autodesk HIG button and I am doing nothing");

        // emulate click
        await page.evaluate(() => document.querySelector("button span").click()); // TODO: this is pretty ineffective. Need to work with handles, I guess

        // verify that the button title is changed
        const updatedTitle = await page.evaluate(() => document.querySelector("button span").textContent);
        expect(updatedTitle).toBe("Oops");
    });
});