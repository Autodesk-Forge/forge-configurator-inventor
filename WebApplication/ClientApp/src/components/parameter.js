import React, { Component } from 'react';
import './parametersContainer.css'

import Input from '@hig/input';
import Checkbox from '@hig/checkbox';
import Dropdown from '@hig/dropdown';

export class Parameter extends Component {

    constructor(props) {
        super(props);
        this.onComboChange = this.onComboChange.bind(this);
        this.onCheckboxChange = this.onCheckboxChange.bind(this);
        this.onEditChange = this.onEditChange.bind(this);

        this.state = {
            value: null
        }
    }

    onComboChange(data) {
        console.log('onComboChange - ' + data);
        this.setState({ value: data });
    }

    onCheckboxChange(data) {
        this.setState({ value: data ? "True" : "False" });
    }

    onEditChange(data) {
        this.setState( {value: data.target.value } );
    }

    onFocus(data) {
        // ??
    }

    reset() {
        // state will reset back to original values when hit UPDATE
        this.setState({ value: this.prop.parameter.value });
    }

    render() {
        if (this.state.value == null)
            this.state.value = this.props.parameter.value;

        if (this.props.parameter.units === "Boolean")
            return (
                <div className="parameter checkbox">
                    <Checkbox
                        disabled={false}
                        indeterminate={false}
                        onBlur={null}
                        onChange={this.onCheckboxChange}
                        onFocus={this.onFocus}
                        onMouseDown={null}
                        checked={this.state.value === "True"}
                    />
                    <div className="parameter checkboxtext">
                        {this.props.parameter.name}
                    </div>
                </div>
            )
        else if (this.props.parameter.allowedValues != null && this.props.parameter.allowedValues.length > 0)
            return (
                <div className="parameter">
                    {this.props.parameter.name}
                    <Dropdown
                        variant="box"
                        disabled={false}
                        error={false}
                        required=""
                        multiple={false}
                        onBlur={null}
                        onChange={this.onComboChange}
                        onFocus={this.onFocus}
                        options={this.props.parameter.allowedValues}
                        value={this.state.value}
                    />
                </div>
            )
        else
            return (
            <div className="parameter">
                {this.props.parameter.name}
                <Input
                    disabled={false}
                    onBlur={null}
                    onChange={this.onEditChange}
                    onFocus={this.onFocus}
                    onMouseEnter={null}
                    onMouseLeave={null}
                    variant="box"
                    value={this.state.value}
                />

            </div>
            )
    }
}

export default Parameter;