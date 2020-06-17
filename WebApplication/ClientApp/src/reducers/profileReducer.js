import actions from '../actions/profileActions';

export const initialState = {
    isLoggedIn: false,
    name: "Anonymous",
    avatarUrl: "logo-xs-white-BG.svg"
};

export default function(state = initialState, action) {
    switch(action.type) {
        case actions.PROFILE_LOADED: {
            return { isLoggedIn: action.isLoggedIn, name: action.profile.name, avatarUrl: action.profile.avatarUrl};
        }
        default:
            return state;
    }
}
