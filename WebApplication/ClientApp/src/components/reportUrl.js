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

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { getReportUrl } from '../reducers/mainReducer';
import HyperLink from './hyperlink';

export class ReportUrl extends Component {
    render() {
        const reportUrl =
            this.props.reportUrl !== undefined ? this.props.reportUrl : null;
        const showReportUrl = reportUrl !== null;

        // Return nothing if there is no report URL to render
        if (!showReportUrl) return (null);

        return (
            <React.Fragment>
                <div className="logContainer">
                    <HyperLink link="Open log file" href={reportUrl} />
                </div>
            </React.Fragment>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store) {
    return {
        reportUrl: getReportUrl(store),
    };
})(ReportUrl);
