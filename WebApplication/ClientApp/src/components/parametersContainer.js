import React, { Component } from 'react';
import { connect } from 'react-redux';
import './parametersContainer.css';
import Parameter from './parameter';
import { getActiveProject, getParameters, getUpdateParameters, modalProgressShowing, updateFailedShowing, reportUrl } from '../reducers/mainReducer';
import { fetchParameters, resetParameters, updateModelWithParameters } from '../actions/parametersActions';
import { showModalProgress, showUpdateFailed } from '../actions/uiFlagsActions';
import Button from '@hig/button';
import ModalProgress from './modalProgress';
import ModalFail from './modalFail';

export class ParametersContainer extends Component {

    componentDidMount() {
        this.props.fetchParameters(this.props.activeProject.id);
    }

    componentDidUpdate(prevProps) {
        // fetch parameters when params UI was active before projects initialized
        if (this.props.activeProject.id !== prevProps.activeProject.id)
            this.props.fetchParameters(this.props.activeProject.id);
    }

    updateClicked() {
        this.props.updateModelWithParameters(this.props.activeProject.id, this.props.projectUpdateParameters);
    }

    onProgressCloseClick() {
        // close is not supported now
        //this.props.showModalProgress(false);
    }

    onUpdateFailedCloseClick() {
        this.props.showUpdateFailed(false);
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
                    {this.props.modalProgressShowing &&
                        <ModalProgress
                            open={this.props.modalProgressShowing}
                            title="Updating Project"
                            label={this.props.activeProject.id}
                            icon="/Assembly_icon.svg"
                            onClose={() => this.onProgressCloseClick()}/>
                    }
                    {this.props.updateFailedShowing &&
                        <ModalFail
                            open={this.props.updateFailedShowing}
                            title="Update Failed"
                            contentName="Assembly:"
                            label={this.props.activeProject.id}
                            onClose={() => this.onUpdateFailedCloseClick()}
                            url={this.props.reportUrl}/>
                    }
                </div>
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store) {
    const activeProject = getActiveProject(store);

    return {
        activeProject: activeProject,
        modalProgressShowing: modalProgressShowing(store),
        updateFailedShowing: updateFailedShowing(store),
        reportUrl: reportUrl(store),
        projectSourceParameters: getParameters(activeProject.id, store),
        projectUpdateParameters: getUpdateParameters(activeProject.id, store)
    };
}, { fetchParameters, resetParameters, updateModelWithParameters, showModalProgress, showUpdateFailed })(ParametersContainer);
