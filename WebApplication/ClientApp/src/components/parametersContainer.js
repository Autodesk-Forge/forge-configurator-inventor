import React, { Component } from 'react';
import { connect } from 'react-redux';
import './parameters.css'
import Parameter from './parameter';
import { getActiveProject } from '../reducers/mainReducer';
import { fetchParameters } from '../actions/parametersActions'

export class ParametersContainer extends Component {

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.fetchParameters(this.props.activeProject.id);
    }

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

export default connect(function (store) {
    return {
        activeProject: getActiveProject(store)
    }
}, { fetchParameters })(ParametersContainer);
