import React, { Component } from "react";
import {connect} from 'react-redux';

import ForgeView from './forgeView';
import ParametersContainer from './parametersContainer';

export class Model extends Component {

    render() {
        return (
            <div className='inRow fullheight'>
                <ParametersContainer/>
                <ForgeView/>
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (/*store*/){
    return {
        //activeProject: getActiveProject(store)
    };
}, {} )(Model);