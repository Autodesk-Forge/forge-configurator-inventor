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
