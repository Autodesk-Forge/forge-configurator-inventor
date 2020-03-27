module.exports = {
  presets: [
    [
      './ClientApp/node_modules/@babel/preset-env',
      {
        targets: {
          node: 'current',
        },
      },
    ],
  ],
};