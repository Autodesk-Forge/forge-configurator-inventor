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
import ProgressBar from '@hig/progress-bar';
import Typography from "@hig/typography";
import './modalProgress.css';
import merge from "lodash.merge";
import HyperLink from './hyperlink';
import Button from '@hig/button';
import CreditCost from './creditCost';

export class ModalDownloadProgress extends Component {

    render() {
        const modalStyles = /* istanbul ignore next */ styles =>
        merge(styles, {
          modal: {
                window: { // by design
                    width: "371px",
                    height: "auto"
                }
            }
        });

        const done = this.props.url != null;
        const iconAsBackgroundImage = {
            width: '48px',
            height: '48px',
            backgroundImage: 'url(' + this.props.icon + ')',
          };

        return (
            <Modal
              open={this.props.open}
              title={this.props.title}
              onCloseClick={this.props.onClose}
              percentComplete={null}
              stylesheet={modalStyles}>
              <div className="modalContent">
                  <div style={iconAsBackgroundImage}/>
                  <div className="modalAction" fontWeight="bold">
                      <Typography>
                        {this.props.label ? this.props.label : "Missing label."}
                      </Typography>
                      {!done && <ProgressBar className="modalProgress"/>}
                  </div>
              </div>
              {(done) &&
                <React.Fragment>
                    <CreditCost/>
                    <div className="modalLink">
                        <HyperLink
                            onAutostart={(downloadHyperlink) => {
                                downloadHyperlink.click();
                            }}
                            /* onUrlClick={this.props.onClose} */ // onClose in onUrlClick colides with onAutostart, causing the dialog to close itself when download starts. But we dont want it to close itself.
                            prefix="Download should start automatically. If it doesn't, "
                            link="click here" href={this.props.url}
                            suffix=" to download it manually."
                            download={true}
                        />
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

export default ModalDownloadProgress;