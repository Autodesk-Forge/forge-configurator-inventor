import modelActionTypes from '../actions/modelActions';


function toKey(projectId, hash) {
    return `${projectId}$$$${hash}`;
}

/** Check availability for the model */
export const modelAvailabilityState = function(state, id, hash) {

    const key = toKey(id, hash);

    return {
        projectId: id,
        hash: hash,
        available: !! state[key]
    };
};


export default function(state = {}, action) {

    switch(action.type) {

        case modelActionTypes.SET_MODEL_STATE_AS_AVAILABLE: {

            const stateUpdate = { ...state };

            const { projectId, hash } = action;
            const key = toKey(projectId, hash);
            stateUpdate[key] = true;

            return stateUpdate;
        }
        default:
            return state;
    }
}
