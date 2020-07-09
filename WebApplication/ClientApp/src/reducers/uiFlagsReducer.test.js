import {closeParametersEditedMessage, rejectParametersEditedMessage, showUploadPackage, editPackageFile, editPackageRoot,
   showDeleteProject, setProjectAlreadyExists, setProjectChecked, clearCheckedProjects, setCheckedProjects} from '../actions/uiFlagsActions';
import uiFlagsReducer, * as uiFlags from './uiFlagsReducer';
import { editParameter, resetParameters } from '../actions/parametersActions';
import { stateParametersEditedMessageClosed, stateParametersEditedMessageNotRejected, stateParametersEditedMessageRejected } from './uiFlagsTestStates';
import { setUploadProgressVisible, setUploadProgressHidden, setUploadProgressDone, setUploadFailed, hideUploadFailed } from '../actions/uploadPackageActions';


describe('uiFlags reducer', () => {
   it('check dismiss', () => {
      expect(uiFlagsReducer(uiFlags.initialState, closeParametersEditedMessage()).parametersEditedMessageClosed).toEqual(true);
      expect(uiFlagsReducer(stateParametersEditedMessageClosed, editParameter("","")).parametersEditedMessageClosed).toEqual(false);
      expect(uiFlagsReducer(stateParametersEditedMessageClosed, resetParameters("")).parametersEditedMessageClosed).toEqual(false);
   });
   it('By default is parameter edit message shown (not rejected)', () => {
      expect(uiFlagsReducer(undefined, {}).parametersEditedMessageRejected).toEqual(false);
   }),
   it('Sets from not rejected to rejected', () => {
      expect(uiFlagsReducer(stateParametersEditedMessageNotRejected, rejectParametersEditedMessage(false)).parametersEditedMessageRejected).toEqual(false);
   }),
   it('Sets from rejected to not rejected', () => {
      expect(uiFlagsReducer(stateParametersEditedMessageRejected, rejectParametersEditedMessage(true)).parametersEditedMessageRejected).toEqual(true);
   });

   it('Sets the show delete project dlg', () => {
      expect(uiFlagsReducer(uiFlags.initialState, showDeleteProject(true)).showDeleteProject).toEqual(true);
   });

   describe('Upload package', () => {
      it('Sets the show package dlg', () => {
         expect(uiFlagsReducer(uiFlags.initialState, showUploadPackage(true)).showUploadPackage).toEqual(true);
         expect(uiFlagsReducer(uiFlags.initialState, showUploadPackage(false)).showUploadPackage).toEqual(false);
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
         const reportUrl = 'some url';
         const uploadFailedState = uiFlagsReducer(uiFlags.initialState, setUploadFailed(reportUrl));
         expect(uploadFailedState.uploadFailedShowing).toEqual(true);
         expect(uploadFailedState.reportUrl).toEqual(reportUrl);
      });

      it('Hides the upload failed', () => {
         expect(uiFlagsReducer(uiFlags.initialState, hideUploadFailed()).uploadFailedShowing).toEqual(false);
      });

      it('Sets the project exists flag', () => {
         expect(uiFlagsReducer(uiFlags.initialState, setProjectAlreadyExists(true)).projectAlreadyExists).toEqual(true);
         expect(uiFlagsReducer(uiFlags.initialState, setProjectAlreadyExists(false)).projectAlreadyExists).toEqual(false);
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

         const newState = uiFlagsReducer(initialState, editPackageFile(mypackage));

         expect(newState.package.file).toEqual(mypackage);
         expect(newState.package.root).toEqual(packageroot);
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

         const newState = uiFlagsReducer(initialState, editPackageRoot(packageroot));

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
         expect(uiFlagsReducer(haveSomeCheckedState, clearCheckedProjects()).checkedProjects).toEqual([]);
      });

      it('replaces checked projects with setCheckedProjects', () => {
         const newChecked = [ '3', '4', '5' ];
         expect(uiFlagsReducer(haveSomeCheckedState, setCheckedProjects(newChecked)).checkedProjects).toEqual(newChecked);
      });

      it('adds a new project to checked projects if not there yet', () => {
         const projectId = '1';
         expect(uiFlagsReducer(haveSomeCheckedState, setProjectChecked(projectId, true)).checkedProjects).toEqual(initialCheckedProjects.concat([projectId]));
      });

      it('does not add a new project to checked projects if already present', () => {
         const projectId = '2';
         expect(uiFlagsReducer(haveSomeCheckedState, setProjectChecked(projectId, true)).checkedProjects).toEqual(initialCheckedProjects);
      });

      it('removes a project from checked projects if present', () => {
         const projectId = '2';
         expect(uiFlagsReducer(haveSomeCheckedState, setProjectChecked(projectId, false)).checkedProjects).toEqual([ '4' ]);
      });

      it('does not alters checked projects if one unchecks a project that is not already there', () => {
         const projectId = '1';
         expect(uiFlagsReducer(haveSomeCheckedState, setProjectChecked(projectId, false)).checkedProjects).toEqual(initialCheckedProjects);
      });

   });
});
