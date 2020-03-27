const playwright = require('playwright');

// IMPORTANT: this is the page URL.  And it's expected that backend is running.
const pageUrl = "https://localhost:5001";

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

        // uncomment to watch the requested URLs
        /*
        await page.route('**', async (request) => {

            console.log(request.url());
            await request.continue();
        });
        */

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

        // wait until project list is refreshed.
        // This selector does not exist on page opening, and will be created after projects are loaded from the server. Wait for it.
        const listItems = await page.$$("#project-list ul li");
        expect(listItems.length).toBeGreaterThan(0); // expects that
    });

    it('checks Version endpoint', async () => {
        await page.goto(pageUrl + '/Version');
        expect(await page.$('pre')).not.toBeNull();
    })
});