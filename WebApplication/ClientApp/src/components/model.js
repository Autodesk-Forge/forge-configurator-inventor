import React, { Component } from "react";
import { connect } from 'react-redux';

import { ensureModelState } from '../actions/modelActions';
import { modelAvailabilityState } from '../reducers/mainReducer';

import ForgeView from './forgeView';
import ParametersContainer from './parametersContainer';

export class Model extends Component {

    componentDidMount() {
        if (! this.props.availabilityState.available) {
            this.props.ensureModelState();
        }
    }

    render() {

        if (! this.props.availabilityState.available) return null;

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
