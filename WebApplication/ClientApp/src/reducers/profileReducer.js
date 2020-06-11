import actions from '../actions/profileActions';

export const initialState = {
    name: "anonymous user",
    avatarUrl: null
};

export default function(state = initialState, action) {
    switch(action.type) {
        case actions.PROFILE_LOADED: {
            return {name: action.profile.name, avatarUrl: action.profile.avatarUrl};
        }
        default:
            return state;
    }
}