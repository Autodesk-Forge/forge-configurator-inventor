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

import repo from '../Repository';
import { checkedProjects } from '../reducers/mainReducer';
import { updateProjectList } from './projectListActions';
import { clearCheckedProjects, showModalProgress } from './uiFlagsActions';

export const deleteProject = () => async (dispatch, getState) => {

    dispatch(showModalProgress(true));

    try {
        await repo.deleteProjects(checkedProjects(getState()));
        const data = await repo.loadProjects();
        dispatch(updateProjectList(data));
        // the projects won't be there anymore (hopefully)
        dispatch(clearCheckedProjects());
        dispatch(showModalProgress(false));
    } catch (e) {
        dispatch(showModalProgress(false));
        alert("Failed to delete project(s)");
    }
};