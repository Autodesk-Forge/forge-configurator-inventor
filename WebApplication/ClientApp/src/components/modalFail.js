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
import { CloseMUI, Error24 } from "@hig/icons";
import Typography from "@hig/typography";
import './modalFail.css';
import merge from "lodash.merge";
import HyperLink from './hyperlink';
import Button from '@hig/button';
import IconButton from "@hig/icon-button";

const urlRegex = new RegExp("^(http|https)://", "i");

export class ModalFail extends Component {

    render() {
        const modalStyles = /* istanbul ignore next */ styles =>
            merge(styles, {
                modal: {
                    window: { // by design
                        width: "371px",
                        height: "263px",
                        borderLeftStyle: "solid",
                        borderLeftWidth: "thick",
                        borderLeftColor: "#ec4a41" // by design
                    }
                }
            });


        let reportUrlOrMessage;
        let isUrl;

        const errorData = this.props.errorData;
        if (typeof errorData === "string") { // obsolete way

            reportUrlOrMessage = errorData;
            isUrl = reportUrlOrMessage?.match(urlRegex);
        } else if (typeof errorData === "object" && errorData.errorType) {

            switch (errorData.errorType) {
                case 1:
                    isUrl = true;
                    reportUrlOrMessage = errorData.reportUrl;
                    break;
                case 2:
                    isUrl = false;
                    reportUrlOrMessage = errorData.messages.join(", ");
                    break;

                default:
                    console.error("Unsupported error type: " + JSON.stringify(errorData, null, 2));
                break;
            }

        } else {
            console.error("Unsupported error: " + JSON.stringify(errorData, null, 2));
        }



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
                                <Error24 className="errorIcon" />
                                <Typography style={{
                                    paddingLeft: "8px",
                                    fontSize: "inherit",
                                    fontWeight: "inherit",
                                    lineHeight: "inherit"
                                }}>{this.props.title}</Typography>
                            </div>
                            <IconButton style={{ width: "24px", height: "24px", marginLeft: "auto", marginRight: "auto" }}
                                icon={<CloseMUI />}
                                onClick={this.props.onClose}
                                title=""
                            />
                        </div>
                    </header>
                }
            >
                <div className="modalFailContent">
                    <div>
                        <Typography><span className="assemblyText">{this.props.contentName}</span> {this.props.label ? this.props.label : "Missing label."}</Typography>
                    </div>
                    {(reportUrlOrMessage && isUrl) &&
                        <div className="logContainer">
                            <HyperLink link="Open log file" href={ reportUrlOrMessage } />
                        </div>
                    }
                    {(reportUrlOrMessage && ! isUrl) &&
                        <div>
                            <Typography className="errorMessage">
                                { reportUrlOrMessage }
                            </Typography>
                        </div>
                    }
                </div>
                <div className="modalFailButtonsContainer">
                    <Button className="button" style={
                        { width: '102px', height: '36px', borderRadius: '2px', marginLeft: '12px' }}
                        type="primary"
                        size="small"
                        title="Ok"
                        onClick={this.props.onClose}
                    />
                </div>
            </Modal>
        );
    }
}

export default ModalFail;