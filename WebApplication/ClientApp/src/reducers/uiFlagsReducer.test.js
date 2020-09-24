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

import * as uiFlagsActions from '../actions/uiFlagsActions';
import uiFlagsReducer, * as uiFlags from './uiFlagsReducer';
import { editParameter, resetParameters } from '../actions/parametersActions';
import { stateParametersEditedMessageClosed, stateParametersEditedMessageNotRejected, stateParametersEditedMessageRejected } from './uiFlagsTestStates';
import { setUploadProgressVisible, setUploadProgressHidden, setUploadProgressDone, setUploadFailed, hideUploadFailed } from '../actions/uploadPackageActions';


describe('uiFlags reducer', () => {
   it('check dismiss', () => {
      expect(uiFlagsReducer(uiFlags.initialState, uiFlagsActions.closeParametersEditedMessage()).parametersEditedMessageClosed).toEqual(true);
      expect(uiFlagsReducer(stateParametersEditedMessageClosed, editParameter("","")).parametersEditedMessageClosed).toEqual(false);
      expect(uiFlagsReducer(stateParametersEditedMessageClosed, resetParameters("")).parametersEditedMessageClosed).toEqual(false);
   });
   it('By default is parameter edit message shown (not rejected)', () => {
      expect(uiFlagsReducer(undefined, {}).parametersEditedMessageRejected).toEqual(false);
   }),
   it('Sets from not rejected to rejected', () => {
      expect(uiFlagsReducer(stateParametersEditedMessageNotRejected, uiFlagsActions.rejectParametersEditedMessage(false)).parametersEditedMessageRejected).toEqual(false);
   }),
   it('Sets from rejected to not rejected', () => {
      expect(uiFlagsReducer(stateParametersEditedMessageRejected, uiFlagsActions.rejectParametersEditedMessage(true)).parametersEditedMessageRejected).toEqual(true);
   });

   it('Sets the show delete project dlg', () => {
      expect(uiFlagsReducer(uiFlags.initialState, uiFlagsActions.showDeleteProject(true)).showDeleteProject).toEqual(true);
   });

   describe('Upload package', () => {
      it('Sets the show package dlg', () => {
         expect(uiFlagsReducer(uiFlags.initialState, uiFlagsActions.showUploadPackage(true)).showUploadPackage).toEqual(true);
         expect(uiFlagsReducer(uiFlags.initialState, uiFlagsActions.showUploadPackage(false)).showUploadPackage).toEqual(false);
      });

      it('Sets the show / hide upload progress', () => {
         expect(uiFlagsReducer(uiFlags.initialState, setUploadProgressVisible()).uploadProgressShowing).toEqual(true);
         const uploadProgressHiddenState = uiFlagsReducer(uiFlags.initialState, setUploadProgressHidden());
         expect(uploadProgressHiddenState.uploadProgressShowing).toEqual(false);
         expect(uploadProgressHiddenState.uploadProgressStatus).toEqual(null);
      });

      it('Sets the upload done', () => {
         expect(uiFlagsReducer(uiFlags.initialState, setUploadProgressDone(true)).uploadProgressStatus).toEqual('done');
      });

      it('Sets the upload failed', () => {
         const anyData = 'some url';
         const uploadFailedState = uiFlagsReducer(uiFlags.initialState, setUploadFailed(anyData));
         expect(uploadFailedState.uploadFailedShowing).toEqual(true);
         expect(uploadFailedState.errorData).toEqual(anyData);
      });

      it('Hides the upload failed', () => {
         expect(uiFlagsReducer(uiFlags.initialState, hideUploadFailed()).uploadFailedShowing).toEqual(false);
      });

      it('Sets the project exists flag', () => {
         expect(uiFlagsReducer(uiFlags.initialState, uiFlagsActions.setProjectAlreadyExists(true)).projectAlreadyExists).toEqual(true);
         expect(uiFlagsReducer(uiFlags.initialState, uiFlagsActions.setProjectAlreadyExists(false)).projectAlreadyExists).toEqual(false);
      });

      it('Sets the package file without overriding the root', () => {
         const mypackage = 'mypackage.zip';
         const packageroot = '.\\mypackage\\root.asm';
         const initialState = {
            showModalProgress: true,
            package: {
               file: '',
               root: packageroot
            }
         };

         const newState = uiFlagsReducer(initialState, uiFlagsActions.editPackageFile(mypackage));

         expect(newState.package.file).toEqual(mypackage);
         expect(newState.package.root).toEqual(''); // root is set only by PACKAGE_ROOT_EDITED
      });

      it('Sets the package root without overriding the file', () => {
         const mypackage = 'mypackage.zip';
         const packageroot = '.\\mypackage\\root.asm';
         const initialState = {
            showModalProgress: true,
            package: {
               file: mypackage,
               root: ''
            }
         };

         const newState = uiFlagsReducer(initialState, uiFlagsActions.editPackageRoot(packageroot));

         expect(newState.package.file).toEqual(mypackage);
         expect(newState.package.root).toEqual(packageroot);
      });
   });

   describe('Check / uncheck in project list', () => {
      const initialCheckedProjects = [ '2', '4' ];
      const haveSomeCheckedState = {
         checkedProjects: initialCheckedProjects
      };

      it('clears all checked projects', () => {
         expect(uiFlagsReducer(haveSomeCheckedState, uiFlagsActions.clearCheckedProjects()).checkedProjects).toEqual([]);
      });

      it('replaces checked projects with setCheckedProjects', () => {
         const newChecked = [ '3', '4', '5' ];
         expect(uiFlagsReducer(haveSomeCheckedState, uiFlagsActions.setCheckedProjects(newChecked)).checkedProjects).toEqual(newChecked);
      });

      it('adds a new project to checked projects if not there yet', () => {
         const projectId = '1';
         expect(uiFlagsReducer(haveSomeCheckedState, uiFlagsActions.setProjectChecked(projectId, true)).checkedProjects).toEqual(initialCheckedProjects.concat([projectId]));
      });

      it('does not add a new project to checked projects if already present', () => {
         const projectId = '2';
         expect(uiFlagsReducer(haveSomeCheckedState, uiFlagsActions.setProjectChecked(projectId, true)).checkedProjects).toEqual(initialCheckedProjects);
      });

      it('removes a project from checked projects if present', () => {
         const projectId = '2';
         expect(uiFlagsReducer(haveSomeCheckedState, uiFlagsActions.setProjectChecked(projectId, false)).checkedProjects).toEqual([ '4' ]);
      });

      it('does not alters checked projects if one unchecks a project that is not already there', () => {
         const projectId = '1';
         expect(uiFlagsReducer(haveSomeCheckedState, uiFlagsActions.setProjectChecked(projectId, false)).checkedProjects).toEqual(initialCheckedProjects);
      });
   });

   describe('Simple setters', () => {
      it('sets the modalProgressShowing', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.showModalProgress(true)).modalProgressShowing).toEqual(true);
      }),
      it('sets the updateFailedShowing', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.showUpdateFailed(true)).updateFailedShowing).toEqual(true);
      }),
      it('sets the loginFailedShowing', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.showLoginFailed(true)).loginFailedShowing).toEqual(true);
      }),
      it('sets the downloadFailedShowing', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.showDownloadFailed(true)).downloadFailedShowing).toEqual(true);
      }),
      it('sets the error data', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.setErrorData('a link')).errorData).toEqual('a link');
      }),
      it('sets the downloadProgressShowing', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.showDownloadProgress(true)).downloadProgressShowing).toEqual(true);
         expect(uiFlagsReducer({}, uiFlagsActions.showDownloadProgress(true)).downloadUrl).toEqual(null);
      }),
      it('sets the downloadUrl', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.setDownloadLink('rfa link')).downloadUrl).toEqual('rfa link');
      }),
      it('sets the activeTabIndex', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.updateActiveTabIndex(7)).activeTabIndex).toEqual(7);
      }),
      it('sets the drawingProgressShowing', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.showDrawingExportProgress(true)).drawingProgressShowing).toEqual(true);
      }),
      it('sets the drawingUrl', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.setDrawingPdfUrl('drawingName', 'pdf link')).drawingUrls['drawingName']).toEqual('pdf link');
      }),
      it('invalidates the drawingUrl', () => {
         expect(uiFlagsReducer({}, uiFlagsActions.invalidateDrawing()).drawingUrls).toEqual({});
      }),
      it('sets the processing stats', () => {
         const theStats = { credits: 7, processing: 13 };
         expect(uiFlagsReducer({}, uiFlagsActions.setStats(theStats)).stats).toEqual(theStats);
      }),
      it('sets the processing stats with key (for drawings)', () => {
         const theStats = { credits: 7, processing: 13 };
         const theKey = '2';
         expect(uiFlagsReducer({}, uiFlagsActions.setStats(theStats, theKey)).stats).toEqual({ '2': theStats }); // '2' == theKey
         expect(uiFlagsReducer({}, uiFlagsActions.setStats(theStats, theKey)).stats[theKey]).toEqual(theStats);
      }),
      it('sets the active drawing', () => {
         const activeDrawing = "drawing 2";
         expect(uiFlagsReducer({}, uiFlagsActions.updateActiveDrawing(activeDrawing)).activeDrawing).toEqual(activeDrawing);
      }),
      it('sets the drawing list', () => {
         const drawingList = [ "drawing 1", "drawing 2"];
         expect(uiFlagsReducer({}, uiFlagsActions.updateDrawingsList(drawingList)).drawings).toEqual(drawingList);
      });
   });
});
