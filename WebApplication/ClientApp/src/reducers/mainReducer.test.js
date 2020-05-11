import {showUpdateNotification} from './mainReducer';

describe('main reducer', () => {
   it('show and hide the notification strip', () => {
      const originalParameters = [
         {
            "name": "editedParameter",
            "value": null,
            "type": "xyz"
         },
         {
            "name": "unchangedParameter",
            "value": 123
         }
      ];

      const editedParameter = {
         "name": "editedParameter",
         "value": "12000 mm",
         "type": "xyz"
      };

      const editedParameters = [
         editedParameter,
         {
            "name": "unchangedParameter",
            "value": 123
         }
      ];

      const origProject = {
         id: "1",
         parameters: originalParameters,
         updateParameters : originalParameters
      };

      const editProject = {
         id: "2",
         parameters: originalParameters,
         updateParameters : editedParameters
      };

      const origProjectList = {
         activeProjectId: "1",
         projects: [
            origProject,
            editProject
         ]
      };

      const editProjectList = {
         activeProjectId: "2",
         projects: [
            origProject,
            editProject
         ]
      };

      const origState = {
         dismissUpdateMessage: false,
         projectList: origProjectList
      };

      const editState = {
         dismissUpdateMessage: false,
         projectList: editProjectList
      };

      expect(showUpdateNotification(editState)).toEqual(true);

      expect(showUpdateNotification(origState)).toEqual(false);

   });
});
