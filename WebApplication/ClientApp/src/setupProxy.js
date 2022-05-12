const proxy = require('http-proxy-middleware');

module.exports = function(app) {
    app.use(proxy('/signalr',
        { target: 'https://localhost:5001/', "changeOrigin": false, "secure": false, ws: true}
    ));

    app.use(proxy('/login',
        { target: 'https://localhost:5001/', "changeOrigin": true, "secure": false}
    ));

    app.use(proxy('/ClearSelf',
        { target: 'https://localhost:5001/', "changeOrigin": true, "secure": false }
    ));

    app.use(proxy('/favicon.ico',
        { target: 'https://localhost:5001/', "changeOrigin": true, "secure": false }
    ));
}