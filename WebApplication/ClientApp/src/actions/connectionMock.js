const connectionMock = {
    onHandlers: {},
    start: function() {},
    on: function(name, fn) {
        this.onHandlers[name] = fn;
    },
    invoke: function() {},
    stop: function() {},
    simulateComplete: function(data) {
        this.onHandlers['onComplete'](data);
    },
    simulateError: function(jobId, link) {
        this.onHandlers['onError'](jobId, link);
    }
};

export default connectionMock;