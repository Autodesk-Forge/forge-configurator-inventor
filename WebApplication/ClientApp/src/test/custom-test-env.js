const Environment = require('jest-environment-jsdom');

/**
 * A custom environment to set the TextDecoder that is required by unzipit.
 */
// eslint-disable-next-line no-undef
module.exports = class CustomTestEnvironment extends Environment {
    async setup() {
        await super.setup();
        if (typeof this.global.TextDecoder === 'undefined') {
            const { TextDecoder } = require('util');
            this.global.TextDecoder = TextDecoder;
		}
    }
};
