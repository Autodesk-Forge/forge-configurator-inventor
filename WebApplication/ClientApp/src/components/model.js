import React, { Component } from "react";
import { connect } from 'react-redux';

import { ensureModelState } from '../actions/modelActions';
import { modelAvailabilityState } from '../reducers/mainReducer';

import ForgeView from './forgeView';
import ParametersContainer from './parametersContainer';

export class Model extends Component {

    constructor(props) {
        super(props);

        this.shown = false;
    }

    componentDidMount() {
        if (! this.props.availabilityState.available) {
            this.props.ensureModelState();
        }
    }

    render() {

        // if component is not rendered yet - check if project has available state
        if (! this.shown &&
            ! this.props.availabilityState.available) return null;

        // OK, the Model component was rendered once - now it should not be hidden
        this.shown = true;

        return (
            <div className='inRow fullheight'>
                <ParametersContainer/>
                <ForgeView/>
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
        availabilityState: modelAvailabilityState(store)
    };
}, { ensureModelState } )(Model);
