const proxy = require('http-proxy-middleware');

module.exports = function(app) {
    app.use(proxy('/signalr',
        { target: 'http://inventor-config-demo-dev.autodesk.io/', "changeOrigin": false, "secure": false, ws: true}
    ));

    app.use(proxy('/login',
        { target: 'http://inventor-config-demo-dev.autodesk.io/', "changeOrigin": true, "secure": false}
    ));

    app.use(proxy('/ClearSelf',
        { target: 'http://inventor-config-demo-dev.autodesk.io/', "changeOrigin": true, "secure": false }
    ));

    app.use(proxy('/favicon.ico',
        { target: 'http://inventor-config-demo-dev.autodesk.io/', "changeOrigin": true, "secure": false }
    ));
}