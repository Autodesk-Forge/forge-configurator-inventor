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
        <div style={{"font-weight": "bold"}}>Parameter has changed.</div>
        <Spacer  spacing="s"/>
        <div>The Inventor Server has updated the parameter. Your initial input is overridden.</div>
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
        // const changedOnUpdateClassName = this.props.parameter.changedOnUpdate == true ? "changedOnUpdate" : "";
        // const tooltipProps = this.props.parameter.changedOnUpdate == true ? {openOnHover: true} : {open: false};
        const changedOnUpdateClassName = "changedOnUpdate";
        const tooltipProps = {openOnHover: true};

        if (this.props.parameter.units === "Boolean")
            return (
                <div className="parameter checkbox">
                    <div>
                        <Tooltip {...tooltipProps} className="paramTooltip" anchorPoint="top-center" content={paramTooltipRenderer}>
                            <div className={changedOnUpdateClassName}>
                                <Checkbox
                                    disabled={false}
                                    indeterminate={false}
                                    onBlur={null}
                                    onChange={this.onCheckboxChange}
                                    onMouseDown={null}
                                    checked={this.props.parameter.value === "True"}
                                />
                            </div>
                            <div className="parameter checkboxtext">
                                {this.props.parameter.name}
                            </div>
                        </Tooltip>
                    </div>
                </div>
            );
        else if (this.props.parameter.allowedValues != null && this.props.parameter.allowedValues.length > 0)
            return (
                <div className="parameter">
                    {this.props.parameter.name}
                    <div>
                        <Tooltip {...tooltipProps} className="paramTooltip" anchorPoint="top-center" content={paramTooltipRenderer}>
                            <Dropdown className={changedOnUpdateClassName}
                                variant="box"
                                disabled={false}
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
                </div>
            );
        else
            return (
            <div className="parameter">
                {this.props.parameter.name}
                <div>
                    <Tooltip {...tooltipProps} className="paramTooltip" anchorPoint="top-center" content={paramTooltipRenderer}>
                        <Input className={changedOnUpdateClassName}
                            disabled={false}
                            onBlur={null}
                            onChange={this.onEditChange}
                            onMouseEnter={null}
                            onMouseLeave={null}
                            variant="box"
                            value={this.props.parameter.value}
                        />
                    </Tooltip>
                </div>
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
