import React, { Component } from 'react';

/** Shared component for "Not Yet Implemented" pages */
export default class PageNYI extends Component {
    render() {
        return (<div style={{ textAlign: 'center', paddingTop: '30px', color: '#777' }}>
            <h3>The page is not yet implemented<br />Please switch to the Model tab</h3>
        </div>);
    }
}
