import React, { Component } from 'react';
import { connect } from 'react-redux';
import './parametersContainer.css';
import Parameter from './parameter';
import { getActiveProject, getParameters, getUpdateParameters, updateProgressShowing } from '../reducers/mainReducer';
import { fetchParameters, resetParameters, updateModelWithParameters } from '../actions/parametersActions';
import { showUpdateProgress } from '../actions/uiFlagsActions'
import Button from '@hig/button';
import Modal from '@hig/modal';

export class ParametersContainer extends Component {

    componentDidMount() {
        this.props.fetchParameters(this.props.activeProject.id);
    }

    updateClicked() {
        this.props.updateModelWithParameters(this.props.activeProject.id, this.props.projectUpdateParameters);
    }

    onCloseClick() {
        this.props.showUpdateProgress(false);
    }

    render() {
        const parameterList = this.props.activeProject ? this.props.projectUpdateParameters : [];
        const buttonsContainerClass = parameterList ? "buttonsContainer" : "buttonsContainer hidden";

        return (
            <div className="parametersContainer">
                <div className="pencilContainer">
                </div>
                <div className="parameters">
                {
                    parameterList ?
                        parameterList.map((parameter, index) => (<Parameter key={index} parameter={parameter}/>))
                        : "No parameters"
                }
                </div>
                <hr className="parametersSeparator"/>
                <div className={buttonsContainerClass}>
                    <Button style={{width: '125px'}}
                        size="standard"
                        title="Reset"
                        type="secondary"
                        width="grow"
                        onClick={() => {this.props.resetParameters(this.props.activeProject.id, this.props.projectSourceParameters);}}
                    />
                    <div style={{width: '14px'}}/>
                    <Button style={{width: '125px'}}
                        size="standard"
                        title="Update"
                        type="primary"
                        width="grow"
                        onClick={() => {this.updateClicked();}}
                    />
                    <Modal open={this.props.updateProgressShowing} title="Wait please" onCloseClick={() => this.onCloseClick()}>Model is now being updated based on the changed parameters.</Modal>
                </div>
            </div>
        );
    }
}

export default connect(function (store) {
    const activeProject = getActiveProject(store);

    return {
        activeProject: activeProject,
        projectSourceParameters: getParameters(activeProject.id, store),
        projectUpdateParameters: getUpdateParameters(activeProject.id, store),
        updateProgressShowing: updateProgressShowing(store)
    };
}, { fetchParameters, resetParameters, updateModelWithParameters, showUpdateProgress })(ParametersContainer);
