import React, { Component } from 'react';

import Modal from '@hig/modal';
import Typography from "@hig/typography";
import './modalUpdateFailed.css';
import merge from "lodash.merge";
import HyperLink from './hyperlink';

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
                <div>
                    <Typography><span className="assemblyText">Assembly:</span> {this.props.label ? this.props.label : "Missing label."}</Typography>
                </div>
                <div>
                    <HyperLink link="Open log file" href={this.props.url} />
                </div>
            </Modal>
        );
    }
}

export default ModalUpdateFailed;