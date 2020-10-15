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

import Modal from '@hig/modal';
import ProgressBar from '@hig/progress-bar';
import Typography from "@hig/typography";
import Spacer from "@hig/spacer";
import './modalProgress.css';
import merge from "lodash.merge";
import CreditCost from './creditCost';
import Button from '@hig/button';
import { getStats } from '../reducers/mainReducer';

export class ModalProgress extends Component {

    render() {
        const iconAsBackgroundImage = {
            width: '48px',
            height: '48px',
            backgroundImage: 'url(' + this.props.icon + ')',
          };

        const warningIconAsBackgroundImage = {
            width: '24px',
            height: '24px',
            backgroundImage: 'url(alert-24.svg)',
        };

        const stats = this.props.statsKey == null ? this.props.stats : (this.props.stats ? this.props.stats[this.props.statsKey] : null);
        const done = stats != null;
        const withWarnings = this.props.warningMsg?.length > 0;
        const title = done && this.props.doneTitle ? this.props.doneTitle : this.props.title;

        const modalProps = (done && withWarnings) ? {
            window: { // by design
                width: "371px",
                height: "auto",
                borderLeftWidth: "3px",
                borderLeftStyle: "solid",
                borderLeftColor: "rgb(250, 162, 27)" // warningColor
            }
        } : {
            window: { // by design
                width: "371px",
                height: "auto"
            }
        };

        const modalStyles = /* istanbul ignore next */ styles =>
        merge(styles, {
          modal: modalProps
        });

        return (
            <Modal
            open={this.props.open}
            title={title}
            onCloseClick={this.props.onClose}
            percentComplete={null}
            stylesheet={modalStyles}
            headerChildren={
                <header id="customHeader">
                    <div className="customHeaderContent">
                        <div className="title">
                            {done && withWarnings && <div id='warningIcon' style={warningIconAsBackgroundImage}/>}
                            <Typography style={{
                                paddingLeft: "8px",
                                fontSize: "inherit",
                                fontWeight: "inherit",
                                lineHeight: "inherit"}}>{title}</Typography>
                        </div>
                    </div>
                </header>}
            >
                <div className="modalContent">
                    <div style={iconAsBackgroundImage}/>
                    <div className="modalAction" fontWeight="bold">
                        <Typography>
                        {this.props.label ? this.props.label : "Missing label."}
                        </Typography>
                        {(!done) &&
                            <ProgressBar className="modalProgress"/>
                        }
                    </div>
                </div>
                {(done) &&
                <React.Fragment>
                    {withWarnings && <div id='warningMsg'>
                        <Typography>{this.props.warningMsg}</Typography>
                        <Spacer spacing='m'/>
                    </div>}
                    <CreditCost statsKey={this.props.statsKey}/>
                    <div id="modalDone">
                        <Button className="button" style={
                            { width: '116px', height: '36px', borderRadius: '2px', marginLeft: '12px'}}
                            type="primary"
                            size="small"
                            title="Ok"
                            onClick={this.props.onClose}
                        />
                    </div>
                </React.Fragment>
              }
          </Modal>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
      stats: getStats(store)
    };
})(ModalProgress);