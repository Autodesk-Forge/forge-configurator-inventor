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

    it('should check the page is loaded', async () => {

        // check the page is loaded
        expect(page).not.toBeNull();
        expect(await page.title()).not.toBeNull();
    });

    it(`should project loading`, async () => {

        // check initial list content
        let initialContent = await page.evaluate(() => document.querySelector("#project-list").textContent);
        expect(initialContent).toBe("No projects loaded");

        // emulate click to trigger project loading
        await page.evaluate(() => document.querySelector("button span").click()); // TODO: this is pretty ineffective. Need to work with handles, I guess

        // wait until project list is refreshed
        await page.waitForSelector("#project-list ul");

        // check updated list content
        const updatedContent = await page.evaluate(() => document.querySelector("#project-list").textContent);
        expect(updatedContent).not.toBe("No projects loaded");
    });
});