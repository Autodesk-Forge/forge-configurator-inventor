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
import Modal from '@hig/modal';
import { CloseMUI, Complete24 } from "@hig/icons";
import ProgressBar from '@hig/progress-bar';
import Spacer from "@hig/spacer";
import Typography from "@hig/typography";
import './modalProgress.css';
import merge from "lodash.merge";
import Button from '@hig/button';
import IconButton from "@hig/icon-button";
import CreditCost from './creditCost';
import ReportUrl from './reportUrl';

export class ModalProgressUpload extends Component {

    render() {
        const done = this.props.isDone();
        const withWarnings = this.props.warningMsg?.length > 0;
        const doneColor = "rgb(135, 179, 64)";
        const warningColor = "rgb(250, 162, 27)";

        const modalStyles = /* istanbul ignore next */ styles =>
        merge(styles, {
          modal: {
                window: { // by design
                    width: "371px",
                    height: "auto",
                    borderLeftWidth: "3px",
                    borderLeftStyle: "solid",
                    borderLeftColor: done ? withWarnings ? warningColor : doneColor : "rgb(255, 255, 255)"
                }
            }
        });

        const iconAsBackgroundImage = {
            width: '48px',
            height: '48px',
            backgroundImage: 'url(' + this.props.icon + ')',
          };

        const warningIconAsBackgroundImage = {
            width: '33px',
            height: '33px',
            backgroundImage: 'url(alert-24.svg)',
            backgroundSize: '26px',
            backgroundRepeat: 'no-repeat',
            backgroundPosition: 'center'
        };

        return (
            <Modal
              open={this.props.open}
              title={this.props.title}
              onCloseClick={this.props.onClose}
              percentComplete={null}
              stylesheet={modalStyles}
              headerChildren={
                <header id="customHeader">
                    <div className="customHeaderContent">
                        <div className="title">
                            {done && !withWarnings && <Complete24 className="doneIcon"/>}
                            {done && withWarnings && <div id='warningIcon' style={warningIconAsBackgroundImage}/>}
                            <Typography style={{
                                paddingLeft: "8px",
                                fontSize: "inherit",
                                fontWeight: "inherit",
                                lineHeight: "inherit"}}>{this.props.title}</Typography>
                        </div>
                        <IconButton style={{ width: "24px", height: "24px", marginLeft: "auto", marginRight: "auto"}}
                            icon={<CloseMUI />}
                            onClick={this.props.onClose}
                            title=""
                        />
                    </div>
                </header>
              }
              >
              <div id="modalUpload">
                <div className="modalContent">
                    {!done && <div style={iconAsBackgroundImage}/>}
                    {done && <Typography className="package" fontWeight="bold">Package:</Typography>}
                    <div className="modalAction" fontWeight="bold">
                        <Typography>
                            {this.props.label ? this.props.label : "Missing label."}
                        </Typography>
                        {!done && <ProgressBar className="modalProgress"/>}
                    </div>
                </div>
                {done &&
                    <div>
                        {withWarnings && <div id='warningMsg'>
                            <Typography>{this.props.warningMsg}</Typography>
                            <Spacer spacing='m'/>
                        </div>}
                        <CreditCost />
                        <ReportUrl />
                        <div className="buttonsContainer">
                            <Button className="button" style={
                                { width: '102px', height: '36px', borderRadius: '2px', marginLeft: '12px'}}
                                type="primary"
                                size="small"
                                title="Open"
                                onClick={this.props.onOpen}
                            />
                            <Button className="button" style={
                                { width: '102px', height: '36px', borderRadius: '2px', marginLeft: '12px'}}
                                type="secondary"
                                size="small"
                                title="Close"
                                onClick={this.props.onClose}
                            />
                        </div>
                    </div>
                }
              </div>
          </Modal>
        );
    }
}

/* istanbul ignore next */
export default ModalProgressUpload;