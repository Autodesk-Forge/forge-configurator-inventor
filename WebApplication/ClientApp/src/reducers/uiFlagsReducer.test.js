import {closeParametersEditedMessage, rejectParametersEditedMessage, showUploadPackage, editPackageFile, editPackageRoot, showDeleteProject} from '../actions/uiFlagsActions';
import uiFlagsReducer, * as uiFlags from './uiFlagsReducer';
import { editParameter, resetParameters } from '../actions/parametersActions';
import { stateParametersEditedMessageClosed, stateParametersEditedMessageNotRejected, stateParametersEditedMessageRejected } from './uiFlagsTestStates';


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

   it('Sets the show package dlg', () => {
      expect(uiFlagsReducer(uiFlags.initialState, showUploadPackage(true)).showUploadPackage).toEqual(true);
   });

   it('Sets the package file without overriding the root', () => {
      const mypackage = 'mypackage.zip';
      const packageroot = '.\\mypackage\\root.asm';
      const initialState = {
         showUpdateProgress: true,
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
         showUpdateProgress: true,
         package: {
            file: mypackage,
            root: ''
         }
      };

      const newState = uiFlagsReducer(initialState, editPackageRoot(packageroot));

      expect(newState.package.file).toEqual(mypackage);
      expect(newState.package.root).toEqual(packageroot);
   });

   it('Sets the show delete project dlg', () => {
      expect(uiFlagsReducer(uiFlags.initialState, showDeleteProject(true)).showDeleteProject).toEqual(true);
   });
});
