/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

const connectionMock = {
    onHandlers: {},
    start: function() {},
    on: function(name, fn) {
        this.onHandlers[name] = fn;
    },
    invoke: function() {},
    stop: function() {},
    simulateComplete: function(data, stats) {
        this.onHandlers['onComplete'](data, stats);
    },
    simulateErrorWithReport: function(jobId, link) {
        this.onHandlers['onError']({ errorType: 1, jobId, reportUrl: link });
    },
    simulateErrorWithMessage: function(jobId, message, title) {
        this.onHandlers['onError']({ errorType: 2, jobId, messages: [message], title });
    }
};

export default connectionMock;