/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import React, { Component } from 'react';
import { connect } from 'react-redux';
import './parametersContainer.css';
import Parameter from './parameter';
import { getActiveProject, getParameters, getUpdateParameters, modalProgressShowing, updateFailedShowing, errorData, adoptWarningMessage } from '../reducers/mainReducer';
import { fetchParameters, resetParameters, updateModelWithParameters } from '../actions/parametersActions';
import { showModalProgress, showUpdateFailed, invalidateDrawing } from '../actions/uiFlagsActions';
import Button from '@hig/button';
import Tooltip from '@hig/tooltip';
import { Alert24 } from "@hig/icons";

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
        // mark drawing as not valid if any available
        this.props.invalidateDrawing();
    }

    onUpdateFailedCloseClick() {
        this.props.showUpdateFailed(false);
    }

    onModalProgressClose() {
        this.props.hideModalProgress();
    }

    render() {
        const parameterList = this.props.activeProject ? this.props.projectUpdateParameters : [];
        const buttonsContainerClass = parameterList ? "buttonsContainer" : "buttonsContainer hidden";

        // if model adopted with warning - then button should became white and have a tooltip with warning details
        const adoptWarnings = adoptWarningMessage(this.props.activeProject);
        const tooltipProps = adoptWarnings ? { openOnHover: true, content: () => <div className="warningButtonTooltip">{ adoptWarnings }</div>  } : { open: false };
        const buttonProps = adoptWarnings ? { type:"secondary", icon: <Alert24 style={ { color: "orange" }} /> } : { type: "primary" };

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
                    <Tooltip { ...tooltipProps } className="paramTooltip" anchorPoint="top-center">
                        <Button style={{width: '125px'}}
                            { ...buttonProps }
                            size="standard"
                            title= "Update"
                            width="grow"
                            onClick={() => this.updateClicked()}/>
                    </Tooltip>

                    {this.props.modalProgressShowing &&
                        <ModalProgress
                            open={this.props.modalProgressShowing}
                            title="Updating Project"
                            doneTitle="Update Finished"
                            label={this.props.activeProject.id}
                            icon="/Assembly_icon.svg"
                            onClose={() => this.onModalProgressClose()}
                        />
                    }
                    {this.props.updateFailedShowing &&
                        <ModalFail
                            open={this.props.updateFailedShowing}
                            title="Update Failed"
                            contentName="Project:"
                            label={this.props.activeProject.id}
                            onClose={() => this.onUpdateFailedCloseClick()}
                            errorData={this.props.errorData}/>
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
        errorData: errorData(store),
        projectSourceParameters: getParameters(activeProject.id, store),
        projectUpdateParameters: getUpdateParameters(activeProject.id, store)
    };
}, { fetchParameters, resetParameters, updateModelWithParameters, showModalProgress, showUpdateFailed, invalidateDrawing,
    hideModalProgress: () => async (dispatch) => { dispatch(showModalProgress(false)); } })(ParametersContainer);
