import {showUpdateNotification} from './mainReducer';

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

function getProjectListWithProjectWithUnChangedParameters() {

   const origProject = {
      id: "1",
      parameters: originalParameters,
      updateParameters : originalParameters
   };

   return {
      activeProjectId: "1",
      projects: [
         origProject
      ]
   };
}

function getProjectListWithProjectWithChangedParameters() {
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

   const editProject = {
      id: "2",
      parameters: originalParameters,
      updateParameters : editedParameters
   };

   return {
      activeProjectId: "2",
      projects: [
         editProject
      ]
   };
}

describe('main reducer', () => {
   it('Notification strip will not be shown if parameters are not changed', () => {
      const state = {
         dismissUpdateMessage: false,
         projectList: getProjectListWithProjectWithUnChangedParameters()
      };
      expect(showUpdateNotification(state)).toEqual(false);

   },
   it('Will show parameters changed notification string when paramters are changed', () => {

      const state = {
         dismissUpdateMessage: false,
         projectList: getProjectListWithProjectWithChangedParameters()
      };
      expect(showUpdateNotification(state)).toEqual(true);
   }),
   it('Will not show changed parameters because user closed them', () => {
      const state = {
         dismissUpdateMessage: true,
         projectList: getProjectListWithProjectWithChangedParameters()
      };
      expect(showUpdateNotification(state)).toEqual(false);
   }),
   it('Will not show changed parameters because user closed them permanently', () => {
      const state = {
         dismissUpdateMessage: false,
         showChangedParameters: false,
         projectList: getProjectListWithProjectWithChangedParameters()
      };
      expect(showUpdateNotification(state)).toEqual(false);
   }));

});
