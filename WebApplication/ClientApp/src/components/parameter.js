import React, { Component } from 'react';
import './parameters.css'

export class Parameter extends Component {

    render() {
        return (
            <div className="parameter">
                {this.props.parameter.name},
                {this.props.parameter.value},
                {this.props.parameter.type},
                {this.props.parameter.units}
            </div>
        )
    }
}

export default Parameter;