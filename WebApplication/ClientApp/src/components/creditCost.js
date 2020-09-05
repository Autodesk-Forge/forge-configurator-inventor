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
import Spacer from "@hig/spacer";
import Typography from "@hig/typography";
import { getStats } from '../reducers/mainReducer';
import { Cloud16 } from '@hig/icons';
import { significantDigits } from '../utils/conversion';

export class CreditCost extends Component {
    render() {

        return (
            <div>
                {this.props.stats?.processing ?
                    <div>
                        <Typography><b>Consumed resources:</b></Typography>
                        <Spacer spacing='s'/>
                        <Typography>Download: {significantDigits(this.props.stats?.download, 5)}s</Typography>
                        <Typography>Processing: {significantDigits(this.props.stats?.processing, 5)}s</Typography>
                        <Typography>Upload: {significantDigits(this.props.stats?.upload, 5)}s</Typography>
                        <Typography><b>Overall time: {significantDigits(this.props.stats?.total, 5)}s</b></Typography>
                        <Typography><b>Cloud Credits: {significantDigits(this.props.stats?.credits, 5)}</b> <Cloud16/></Typography>
                        <Spacer spacing='s'/>
                        <Typography>Queuing: {significantDigits(this.props.stats?.queueing, 5)}s</Typography>
                    </div>
                    :
                    <div>
                    <Typography><b>Used cache, Cloud Credits: 0</b> <Cloud16/></Typography>
                    <Spacer spacing='s'/>
                    <Typography>The last consumed Cloud Credits: {significantDigits(this.props.stats?.credits, 5)} <Cloud16/></Typography>
                    </div>
                }
                <Spacer spacing='m'/>
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
      stats: getStats(store)
    };
})(CreditCost);