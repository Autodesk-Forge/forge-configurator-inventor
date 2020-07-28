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

export class ModalProgress extends Component {

    render() {
        const modalStyles = /* istanbul ignore next */ styles =>
        merge(styles, {
          modal: {
                window: { // by design
                    width: "371px",
                    height: "263px"
                }
            }
        });

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
                      <ProgressBar className="modalProgress"/>
                  </div>
              </div>
          </Modal>
        );
    }
}

export default ModalProgress;