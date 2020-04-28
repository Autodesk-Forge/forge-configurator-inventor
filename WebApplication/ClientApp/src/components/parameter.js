import React, { Component } from 'react';
import './parameters.css'

class Parameter extends Component {

    render() {
        return (
            <div className="parameter">
                name={this.props.parameter.name},
                value={this.props.parameter.value},
                type={this.props.parameter.type},
                units={this.props.parameter.units}
            </div>
        )
    }
}

export default Parameter;