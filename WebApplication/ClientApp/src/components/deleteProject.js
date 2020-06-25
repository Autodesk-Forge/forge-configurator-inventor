import React, { Component } from 'react';
import { connect } from 'react-redux';
import Modal from '@hig/modal';
import Button from '@hig/button';
import Spacer from '@hig/spacer';
import merge from "lodash.merge";
import { deleteProjectDlgVisible, checkedProjects } from '../reducers/mainReducer';
import { showDeleteProject } from '../actions/uiFlagsActions';
import { deleteProject } from '../actions/deleteProjectActions';
import './deleteProject.css';

export class DeleteProject extends Component {

    render() {
        const modalStyles = /* istanbul ignore next */ styles =>
        merge(styles, {
          modal: {
                window: { // by design
                    width: "370px"
                }
            }
        });

        return (
            <React.Fragment>
            <Modal
            open={this.props.deleteProjectDlgVisible}
            title="Delete project"
            onCloseClick={() => { this.props.showDeleteProject(false); }}
            stylesheet={modalStyles} >
                <div id="deleteProjectModal">
                    <div className="deleteProjectListContainer">
                        Are you sure you want to delete following {this.props.checkedProjects.length === 1 ? 'project' : 'projects'}?
                        <ul>
                            {this.props.checkedProjects.map(projectId => (<li key={projectId}>{projectId}</li>))}
                        </ul>
                    </div>
                    <Spacer  spacing="l"/>
                    <div className="buttonsContainer">
                        <Button
                            id="delete_ok_button"
                            size="standard"
                            title="Delete"
                            type="primary"
                            onClick={() => {
                                this.props.showDeleteProject(false);
                                this.props.deleteProject();
                            }}
                        />
                        <div style={{width: '14px'}}/>
                        <Button
                            id="cancel_button"
                            size="standard"
                            title="Cancel"
                            type="secondary"
                            onClick={() => { this.props.showDeleteProject(false); } }
                        />
                    </div>
                </div>
            </Modal>
            </React.Fragment>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store) {
    return {
      deleteProjectDlgVisible: deleteProjectDlgVisible(store),
      checkedProjects: checkedProjects(store)
    };
}, { showDeleteProject, deleteProject } )(DeleteProject);