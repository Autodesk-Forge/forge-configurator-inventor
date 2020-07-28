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

import {parametersEditedMessageVisible} from './mainReducer';
import * as uiFlagsTestStates from './uiFlagsTestStates';
import * as uiFlags from './uiFlagsReducer';

const projectId = "1";

const originalParameters = {
   [projectId]: [
      {
         "name": "editedParameter",
         "value": null,
         "type": "xyz"
      },
      {
         "name": "unchangedParameter",
         "value": 123
      }
   ]
};

const editedParameter = {
   "name": "editedParameter",
   "value": "12000 mm",
   "type": "xyz"
};

const editedParameters = {
   [projectId]: [
      {
         "name": "unchangedParameter",
         "value": 123
      },
      editedParameter
   ]
};

const projectList = {
   activeProjectId: projectId,
   projects: [
      {id: projectId}
   ]
};

describe('main reducer', () => {
   it('Notification strip will not be shown if parameters are not changed', () => {
      const state = {
         uiFlags: uiFlags.initialState,
         projectList: projectList,
         parameters: originalParameters,
         updateParameters: originalParameters
      };
      expect(parametersEditedMessageVisible(state)).toEqual(false);
   },
   it('Will show parameters changed notification string when paramters are changed', () => {
      const state = {
         uiFlags: uiFlags.initialState,
         projectList: projectList,
         parameters: originalParameters,
         updateParameters: editedParameters
      };
      expect(parametersEditedMessageVisible(state)).toEqual(true);
   }),
   it('Will not show changed parameters because user closed them', () => {
      const state = {
         uiFlags: uiFlagsTestStates.stateParametersEditedMessageClosed,
         projectList: projectList,
         parameters: originalParameters,
         updateParameters: editedParameters
      };
      expect(parametersEditedMessageVisible(state)).toEqual(false);
   }),
   it('Will not show changed parameters because user closed them permanently', () => {
      const state = {
         uiFlags: uiFlagsTestStates.stateParametersEditedMessageRejected,
         projectList: projectList,
         parameters: originalParameters,
         updateParameters: editedParameters
      };
      expect(parametersEditedMessageVisible(state)).toEqual(false);
   }));
});
