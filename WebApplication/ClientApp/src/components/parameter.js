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
import {connect} from 'react-redux';
import { editParameter } from '../actions/parametersActions';
import { getActiveProject } from '../reducers/mainReducer';
import './parametersContainer.css';
import Input from '@hig/input';
import Checkbox from '@hig/checkbox';
import Dropdown from '@hig/dropdown';
import Tooltip from '@hig/tooltip';
import Spacer from "@hig/spacer";

const paramTooltipRenderer = () => {
    return (<div>
        <div style={{"fontWeight": "bold"}}>Parameter has changed.</div>
        <Spacer spacing="s"/>
        <div>Inventor Server updated the parameter. Your initial input was overridden.</div>
    </div>);
};

export class Parameter extends Component {

    constructor(props) {
        super(props);
        this.onComboChange = this.onComboChange.bind(this);
        this.onCheckboxChange = this.onCheckboxChange.bind(this);
        this.onEditChange = this.onEditChange.bind(this);
    }

    onComboChange(data) {
        this.props.editParameter(this.props.activeProject.id, {name: this.props.parameter.name, value: data});
    }

    onCheckboxChange(data) {
        this.props.editParameter(this.props.activeProject.id, {name: this.props.parameter.name, value: data ? "True" : "False"});
    }

    onEditChange(data) {
        this.props.editParameter(this.props.activeProject.id, {name: this.props.parameter.name, value: data.target.value});
    }

    render() {
        const changedOnUpdateClassName = this.props.parameter.changedOnUpdate == true ? "changedOnUpdate" : "";
        const tooltipProps = this.props.parameter.changedOnUpdate == true ? {openOnHover: true} : {open: false};
        // for debugging the tooltip, replace the above two lines with these:
        // const changedOnUpdateClassName = "changedOnUpdate";
        // const tooltipProps = {openOnHover: true};

        if (this.props.parameter.units === "Boolean")
            return (
                <div className="parameter checkbox">
                    <Tooltip {...tooltipProps} className="paramTooltip" anchorPoint="top-center" content={paramTooltipRenderer}>
                        <div style={{"display": "flex"}}>
                            <div className={changedOnUpdateClassName}>
                                <Checkbox
                                    disabled={this.props.parameter.readonly}
                                    indeterminate={false}
                                    onBlur={null}
                                    onChange={this.onCheckboxChange}
                                    onMouseDown={null}
                                    checked={this.props.parameter.value === "True"}
                                />
                            </div>
                            <div className="parameter checkboxtext">
                                {this.props.parameter.label}
                            </div>
                        </div>
                    </Tooltip>
                </div>
            );
        else if (this.props.parameter.allowedValues != null && this.props.parameter.allowedValues.length > 0)
            return (
                <div className="parameter">
                    {this.props.parameter.label}
                    <Tooltip {...tooltipProps} className="paramTooltip" anchorPoint="top-center" content={paramTooltipRenderer}>
                        <Dropdown className={changedOnUpdateClassName}
                            variant="box"
                            disabled={this.props.parameter.readonly}
                            error={false}
                            required=""
                            multiple={false}
                            onBlur={null}
                            onChange={this.onComboChange}
                            options={this.props.parameter.allowedValues}
                            value={this.props.parameter.value}
                        />
                    </Tooltip>
                </div>
            );
        else
            return (
            <div className="parameter">
                {this.props.parameter.label}
                <Tooltip {...tooltipProps} className="paramTooltip" anchorPoint="top-center" content={paramTooltipRenderer}>
                    <Input className={changedOnUpdateClassName}
                        disabled={this.props.parameter.readonly}
                        onBlur={null}
                        onChange={this.onEditChange}
                        onMouseEnter={null}
                        onMouseLeave={null}
                        variant="box"
                        value={this.props.parameter.value}
                    />
                </Tooltip>
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
        activeProject: getActiveProject(store)
    };
  }, { editParameter } )(Parameter);
