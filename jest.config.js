'use strict';

module.exports = {
  displayName: 'test',
  testPathIgnorePatterns: [
    '/node_modules/'
  ],
  verbose: true,
  transform: {"\\.js$": ['babel-jest', {rootMode: "upward"}]}
};