const actionTypes = {
    ADD_ERROR: 'ADD_ERROR',
    ADD_LOG: 'ADD_LOG'
};

export default actionTypes;

export const addError = error => {
    return {
        type: actionTypes.ADD_ERROR,
        info: error
    };
};

export const addLog = info => {
    return {
        type: actionTypes.ADD_LOG,
        info
    };
};