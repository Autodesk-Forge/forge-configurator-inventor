const proxy = require('http-proxy-middleware');

module.exports = function(app) {
    app.use(proxy('/signalr',
        { target: 'http://localhost:5000/', "changeOrigin": false, "secure": false, ws: true}
    ));

    app.use(proxy('/login',
        { target: 'http://localhost:5000/', "changeOrigin": true, "secure": false}
    ));

    app.use(proxy('/ClearSelf',
        { target: 'http://localhost:5000/', "changeOrigin": true, "secure": false }
    ));

    app.use(proxy('/favicon.ico',
        { target: 'http://localhost:5000/', "changeOrigin": true, "secure": false }
    ));
}