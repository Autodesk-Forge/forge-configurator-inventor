import React, { Component } from 'react';
import { connect } from 'react-redux';

import Modal from '@hig/modal';
import ProgressBar from '@hig/progress-bar';
import Typography from "@hig/typography";
import { getActiveProject, updateProgressShowing } from '../reducers/mainReducer';
import { showUpdateProgress } from '../actions/uiFlagsActions'
import './modalProgress.css';
import merge from "lodash.merge";

export class ModalProgress extends Component {

    onCloseClick() {
      // close is not supported now
      //this.props.showUpdateProgress(false);
    }

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
              open={this.props.updateProgressShowing}
              title="Updating Project"
              onCloseClick={() => {this.onCloseClick();}}
              percentComplete={null}
              stylesheet={modalStyles}>
              <div className="modalContent">
                  <img className="modalIcon" src="Assembly icon.svg"/>
                  <Typography className="modalAction" fontWeight="bold">
                      {this.props.activeProject.id ? this.props.activeProject.id : "Missing active project name."}
                      <ProgressBar className="modalProgress"/>
                  </Typography>
              </div>
          </Modal>
        );
    }
}

export default connect(function (store) {
    const activeProject = getActiveProject(store);

    return {
      activeProject: activeProject,
      updateProgressShowing: updateProgressShowing(store)
    };
}, { showUpdateProgress })(ModalProgress);