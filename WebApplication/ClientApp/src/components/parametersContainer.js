import React, { Component } from 'react';
import { connect } from 'react-redux';
import './parametersContainer.css';
import Parameter from './parameter';
import { getActiveProject, getParameters, getUpdateParameters } from '../reducers/mainReducer';
import { fetchParameters, resetParameters } from '../actions/parametersActions';
import Button from '@hig/button';

export class ParametersContainer extends Component {

    componentDidMount() {
        this.props.fetchParameters(this.props.activeProject.id);
    }

    updateClicked() {
        alert("Update of model on server is not implemented yet. Parameter values will be returned back for now.");
        this.props.resetParameters(this.props.activeProject.id, this.props.projectSourceParameters);
    }

    render() {
        const parameterList = this.props.activeProject ? this.props.projectUpdateParameters : [];
        const buttonsContainerClass = parameterList ? "buttonsContainer" : "buttonsContainer hidden";

        return (
            <div className="parametersContainer">
                <div className="parametersTop">
                </div>
                <div className="parametersMiddle">
                    <div className="parameters">
                    {
                        parameterList ?
                            parameterList.map((parameter, index) => (<Parameter key={index} parameter={parameter}/>))
                            : "No parameters"
                    }
                    </div>
                </div>
                <hr className="parametersSeparator"/>
                <div className="parametersBottom">
                    <div className={buttonsContainerClass}>
                        <div className="buttons">
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
                        </div>
                    </div>
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
        projectUpdateParameters: getUpdateParameters(activeProject.id, store)
    };
}, { fetchParameters, resetParameters })(ParametersContainer);
