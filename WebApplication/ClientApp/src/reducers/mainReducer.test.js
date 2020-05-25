import {parametersEditedMessageVisible} from './mainReducer';
import * as uiFlagsTestStates from './uiFlagsReducer.test';
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
         uiFlagsReducer: uiFlags.initialState,
         projectList: projectList,
         parameters: originalParameters,
         updateParameters: originalParameters
      };
      expect(parametersEditedMessageVisible(state)).toEqual(false);
   },
   it('Will show parameters changed notification string when paramters are changed', () => {
      const state = {
         uiFlagsReducer: uiFlags.initialState,
         projectList: projectList,
         parameters: originalParameters,
         updateParameters: editedParameters
      };
      expect(parametersEditedMessageVisible(state)).toEqual(true);
   }),
   it('Will not show changed parameters because user closed them', () => {
      const state = {
         uiFlagsReducer: uiFlagsTestStates.stateParametersEditedMessageClosed,
         projectList: projectList,
         parameters: originalParameters,
         updateParameters: editedParameters
      };
      expect(parametersEditedMessageVisible(state)).toEqual(false);
   }),
   it('Will not show changed parameters because user closed them permanently', () => {
      const state = {
         uiFlagsReducer: uiFlagsTestStates.stateParametersEditedMessageRejected,
         projectList: projectList,
         parameters: originalParameters,
         updateParameters: editedParameters
      };
      expect(parametersEditedMessageVisible(state)).toEqual(false);
   }));
});
