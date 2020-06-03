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

        const done = this.props.url !== null;
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
                  <Typography className="modalAction" fontWeight="bold">
                      {this.props.label ? this.props.label : "Missing label."}
                      {!done && <ProgressBar className="modalProgress"/>}
                  </Typography>
              </div>
              {done && <Typography><a href={this.props.url}>Click here</a> to download RFA model.</Typography>}
          </Modal>
        );
    }
}

export default ModalProgress;