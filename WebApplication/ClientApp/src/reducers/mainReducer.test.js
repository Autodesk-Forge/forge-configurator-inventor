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

import * as reducer from './mainReducer';
import { fullState as uiFlagsTestState } from './uiFlagsTestStates';
import * as uiFlagsReducer from './uiFlagsReducer';

const projectId = "1";

const originalParameters = {
   [projectId]: [
      {
         "name": "editedParameter",
         "value": "",
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
   it('Gets user profile correctly', () => {
      const profile = {isLoggedIn: true};
      const state = {
         profile: profile
      };
      expect(reducer.getProfile(state)).toEqual(profile);
   });

   describe('Parameters changed notification strip', () => {
      it('Strip will not be shown if parameters are not changed', () => {
         const state = {
            uiFlags: uiFlagsReducer.initialState,
            projectList: projectList,
            parameters: originalParameters,
            updateParameters: originalParameters,
            profile: {isLoggedIn: true}
         };
         expect(reducer.parametersEditedMessageVisible(state)).toEqual(false);
      }),
      it('Strip is shown when parameters are changed', () => {
         const state = {
            uiFlags: uiFlagsReducer.initialState,
            projectList: projectList,
            parameters: originalParameters,
            updateParameters: editedParameters,
            profile: {isLoggedIn: true}
         };
         expect(reducer.parametersEditedMessageVisible(state)).toEqual(true);
      });
      it('Strip is not shown when it was closed by the user', () => {
         const state = {
            uiFlags: { parametersEditedMessageClosed : true },
            profile: {isLoggedIn: true}
         };
         expect(reducer.parametersEditedMessageVisible(state)).toEqual(false);
      });
      it('Strip is not shown when it was denied by the user to show it', () => {
         const state = {
            uiFlags: { parametersEditedMessageClosed : false,
                       parametersEditedMessageRejected: true },
            profile: {isLoggedIn: true}
         };
         expect(reducer.parametersEditedMessageVisible(state)).toEqual(false);
      });
      it('Strip is not shown when there is no active project', () => {
         const state = {
            uiFlags: uiFlagsReducer.initialState,
            projectList: { activeProjectId: null },
            profile: {isLoggedIn: true}
         };
         expect(reducer.parametersEditedMessageVisible(state)).toEqual(false);
      });
      it('Strip is not shown when parameters to compare to are missing', () => {
         const state = {
            uiFlags: uiFlagsReducer.initialState,
            projectList: projectList,
            parameters: {[projectId]: null},
            updateParameters: originalParameters,
            profile: {isLoggedIn: true}
         };
         expect(reducer.parametersEditedMessageVisible(state)).toEqual(false);
      });
      it('Strip is not shown when edit parameters to compare to are missing', () => {
         const state = {
            uiFlags: uiFlagsReducer.initialState,
            projectList: projectList,
            parameters: originalParameters,
            updateParameters: {[projectId]: null},
            profile: {isLoggedIn: true}
         };
         expect(reducer.parametersEditedMessageVisible(state)).toEqual(false);
      });
      it('Will show parameters changed notification string when parameters are changed', () => {
         const state = {
            uiFlags: uiFlagsReducer.initialState,
            projectList: projectList,
            parameters: originalParameters,
            updateParameters: editedParameters,
            profile: {isLoggedIn: true}
         };
         expect(reducer.parametersEditedMessageVisible(state)).toEqual(true);
      });
   });

   describe('UI Flags getters', () => {
      it('gets modalProgressShowing', () => {
         expect(reducer.modalProgressShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.modalProgressShowing);
      }),
      it('gets updateFailedShowing', () => {
         expect(reducer.updateFailedShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.updateFailedShowing);
      }),
      it('gets loginFailedShowing', () => {
         expect(reducer.loginFailedShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.loginFailedShowing);
      }),
      it('gets downloadFailedShowing', () => {
         expect(reducer.downloadFailedShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.downloadFailedShowing);
      }),
      it('gets downloadDrawingFailedShowing', () => {
         expect(reducer.downloadDrawingFailedShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.downloadDrawingFailedShowing);
      }),
      it('gets errorData', () => {
         expect(reducer.errorData(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.errorData);
      }),
      it('gets downloadProgressShowing', () => {
         expect(reducer.downloadProgressShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.downloadProgressShowing);
      }),
      it('gets download progress title', () => {
         expect(reducer.downloadProgressTitle(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.downloadProgressTitle);
      }),
      it('gets downloadUrl', () => {
         expect(reducer.downloadUrl(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.downloadUrl);
      }),
      it('gets uploadPackageDlgVisible', () => {
         expect(reducer.uploadPackageDlgVisible(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.showUploadPackage); /* method and flag name differ */
      }),
      it('gets uploadProgressShowing', () => {
         expect(reducer.uploadProgressShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.uploadProgressShowing);
      }),
      it('gets uploadProgressIsDone', () => {
         expect(reducer.uploadProgressIsDone(uiFlagsTestState)).toEqual(true); /* uploadProgressStatus === "done" */ /* method and flag name differ */
      }),
      it('gets uploadPackageData', () => {
         expect(reducer.uploadPackageData(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.package); /* method and flag name differ */
      }),
      it('gets uploadFailedShowing', () => {
         expect(reducer.uploadFailedShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.uploadFailedShowing);
      }),
      it('gets activeTabIndex', () => {
         expect(reducer.activeTabIndex(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.activeTabIndex);
      }),
      it('gets projectAlreadyExists', () => {
         expect(reducer.projectAlreadyExists(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.projectAlreadyExists);
      }),
      it('gets deleteProjectDlgVisible', () => {
         expect(reducer.deleteProjectDlgVisible(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.showDeleteProject); /* method and flag name differ */
      }),
      it('gets checkedProjects', () => {
         expect(reducer.checkedProjects(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.checkedProjects);
      }),
      it('gets getDrawingPdfUrl', () => {
         expect(reducer.getDrawingPdfUrl(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.drawingUrls[uiFlagsTestState.uiFlags.activeDrawing]); /* method and flag name differ */
      }),
      it('gets drawingProgressShowing', () => {
         expect(reducer.drawingProgressShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.drawingProgressShowing);
      }),
      it('gets the processing stats', () => {
         expect(reducer.getStats(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.stats);
      }),
      it('gets the active drawing stats', () => {
         expect(reducer.getActiveDrawing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.activeDrawing);
      }),
      it('gets the drawings list', () => {
         expect(reducer.getDrawingsList(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.drawings); /* method and flag name differ */
      }),
      it('gets progress state for adoptWithParams', () => {
         expect(reducer.adoptWithParamsProgressShowing(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.adoptWithParamsProgressShowing);
      }),
      it('gets failure state for adoptWithParams', () => {
         expect(reducer.adoptWithParamsFailed(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.adoptWithParamsFailed);
      }),
      it('gets report url', () => {
         expect(reducer.getReportUrl(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.reportUrl); /* method and flag name differ */
      }),
      it('gets embedded mode enabled state', () => {
         expect(reducer.embeddedModeEnabled(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.embeddedModeUrl !== null); /* method and flag name differ */
      }),
      it('gets embedded mode url', () => {
         expect(reducer.embeddedModeUrl(uiFlagsTestState)).toEqual(uiFlagsTestState.uiFlags.embeddedModeUrl);
      });
   });

});
