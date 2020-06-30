import React, { Component } from 'react';

import Modal from '@hig/modal';
import { CloseMUI, Error24 } from "@hig/icons";
import Typography from "@hig/typography";
import './modalProgressRfaFailed.css';
import merge from "lodash.merge";
import HyperLink from './hyperlink';
import Button from '@hig/button';
import IconButton from "@hig/icon-button";

export class ModalProgressRfaFailed extends Component {

    render() {
        const modalStyles = /* istanbul ignore next */ styles =>
            merge(styles, {
                modal: {
                    window: { // by design
                        width: "371px",
                        height: "263px",
                        borderLeftStyle: "solid",
                        borderLeftWidth: "thick",
                        borderLeftColor: "#ec4a41"
                    }
                }
            });
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
                            <IconButton style={{ width: "16px", height: "24px" }}
                                icon={<CloseMUI />}
                                onClick={this.props.onClose}
                                title=""
                            />
                        </div>
                    </header>
                }
            >
            <div className="modalProgressRFAFailedContent">
                    <div>
                        <Typography><span className="assemblyText">Assembly:</span> {this.props.label ? this.props.label : "Missing label."}</Typography>
                    </div>
                    <div className="logContainer">
                        <HyperLink link="Open log file" href={this.props.url} />
                    </div>
            </div>
            <div className="modalProgressRFAFailedButtonsContainer">
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

export default ModalProgressRfaFailed;