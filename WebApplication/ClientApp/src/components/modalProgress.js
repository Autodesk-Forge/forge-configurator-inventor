import React, { Component } from 'react';

import Modal from '@hig/modal';
import ProgressBar from '@hig/progress-bar';
import Typography from "@hig/typography";
import './modalProgress.css';
import merge from "lodash.merge";

export class ModalProgress extends Component {

    render() {
        const modalStyles = styles =>
        merge(styles, {
          modal: {
                window: { // by design
                    width: "371px",
                    height: "263px"
                }
            }
        });

        return (
          <Modal
              open={this.props.open}
              title={this.props.title}
              onCloseClick={this.props.onClose}
              percentComplete={null}
              stylesheet={modalStyles}>
              <div className="modalContent">
                  <img className="modalIcon" src={this.props.icon}/>
                  <Typography className="modalAction" fontWeight="bold">
                      {this.props.label ? this.props.label : "Missing label."}
                      <ProgressBar className="modalProgress"/>
                  </Typography>
              </div>
          </Modal>
        );
    }
}

export default ModalProgress;