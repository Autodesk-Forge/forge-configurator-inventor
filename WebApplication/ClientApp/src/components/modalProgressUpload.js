import React, { Component } from 'react';

import Modal from '@hig/modal';
import ProgressBar from '@hig/progress-bar';
import Typography from "@hig/typography";
import './modalProgress.css';
import merge from "lodash.merge";
import Button from '@hig/button';

export class ModalProgressUpload extends Component {

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

        const done = this.props.isDone();
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
              <div id="modalUpload">
                <div className="modalContent">
                    <div style={iconAsBackgroundImage}/>
                    <div className="modalAction" fontWeight="bold">
                        <Typography>
                            {this.props.label ? this.props.label : "Missing label."}
                        </Typography>
                        {!done && <ProgressBar className="modalProgress"/>}
                    </div>
                </div>
                {done &&
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
                }
              </div>
          </Modal>
        );
    }
}

export default ModalProgressUpload;