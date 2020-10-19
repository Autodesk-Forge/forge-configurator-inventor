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
