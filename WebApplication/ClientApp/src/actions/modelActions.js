import repo from '../Repository';
import { modelAvailabilityState } from '../reducers/mainReducer';

const actionTypes = {
    SET_MODEL_STATE_AS_AVAILABLE: 'SET_MODEL_STATE_AS_AVAILABLE',
};
export default actionTypes;

export const ensureModelState = () => async (dispatch, getState) => {

    const state = getState();
    console.log(`ensureModelState for ${JSON.stringify(state)}`);
    const { projectId, hash, available } = modelAvailabilityState(state);


    if (! available) {
        try {
            await repo.ensureModelData(projectId, hash);
        } catch (e) {
            alert("Failed to ensure model data"); // TODO: what is correct handling of it?
        }
    }

    dispatch(setModelStateAsAvailable(projectId, hash));
};

export const setModelStateAsAvailable = (projectId, hash) => {
    return {
        type: actionTypes.SET_MODEL_STATE_AS_AVAILABLE,
        projectId,
        hash
    };
};
