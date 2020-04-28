import React, { Component } from 'react';
import { connect } from 'react-redux';
import './parameters.css'
import Parameter from './parameter';
import { getActiveProject } from '../reducers/mainReducer';

class ParametersContainer extends Component {

    render() {
        const parameterList = this.props.activeProject.parameters;

        if (!parameterList) {
            return (<span>No parameters</span>)
        } else {
            return (
                <div className="parametersContainer">
                    {
                        parameterList.map((parameter, index) => (<Parameter key={index} parameter={parameter}/>))
                    }
                </div>
            )
        }
    }
}

ParametersContainer = connect(function (store) {
    return {
        activeProject: getActiveProject(store)
    }
})(ParametersContainer);

export default ParametersContainer;