import React, { Component } from 'react';

import Modal from '@hig/modal';
import Typography from "@hig/typography";
import './modalProgress.css';
import merge from "lodash.merge";

export class ModalUpdateFailed extends Component {

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

        return (
            <Modal
              open={this.props.open}
              title={this.props.title}
              onCloseClick={this.props.onClose}
              percentComplete={null}
              stylesheet={modalStyles}>
              <div className="modalContent">
                  <div className="modalAction" fontWeight="bold">
                      <Typography>
                        Assembly: {this.props.label ? this.props.label : "Missing label."}
                      </Typography>
                  </div>
                  {/* TODO add link to error report */}
              </div>
          </Modal>
        );
    }
}

export default ModalUpdateFailed;