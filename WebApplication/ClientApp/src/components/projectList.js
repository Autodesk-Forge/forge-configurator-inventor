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

import React, { Component } from 'react';
import { connect } from 'react-redux';
import 'react-base-table/styles.css';
import IconButton from '@hig/icon-button';
import { Upload24, Trash24 } from '@hig/icons';
import './projectList.css';
import { showUploadPackage, updateActiveTabIndex, showDeleteProject, showModalProgress, invalidateDrawing } from '../actions/uiFlagsActions';
import { setUploadProgressHidden, hideUploadFailed } from '../actions/uploadPackageActions';
import { updateActiveProject } from '../actions/projectListActions';
import UploadPackage from './uploadPackage';
import DeleteProject from './deleteProject';

import ModalProgressUpload from './modalProgressUpload';
import ModalProgress from './modalProgress';
import ModalFail from './modalFail';
import { uploadProgressShowing, uploadProgressIsDone, uploadPackageData, uploadFailedShowing,
  checkedProjects, modalProgressShowing, errorData, getAdoptWarnings } from '../reducers/mainReducer';
import CheckboxTable from './checkboxTable';

export class ProjectList extends Component {

  isDone() {
    return this.props.uploadProgressIsDone;
  }

  onProgressCloseClick() {
    this.props.setUploadProgressHidden();
  }

  onDeleteCloseClick() {
    this.props.showModalProgress(false);
  }

  onProjectClick(projectId) {
    // set active project
    this.props.updateActiveProject(projectId);
    // mark drawing as not valid if any available
    this.props.invalidateDrawing();
    // switch to MODEL tab
    this.props.updateActiveTabIndex(1);
  }

  onProgressOpenClick() {
    this.props.setUploadProgressHidden();
    this.props.updateActiveProject(this.props.uploadProjectName);
    this.props.invalidateDrawing();
    // switch to MODEL tab
    this.props.updateActiveTabIndex(1);
  }

  onUploadFailedCloseClick() {
    this.props.hideUploadFailed();
  }

  render() {
    const uploadButtonVisible = this.props.isLoggedIn;
    const checkedProjects = this.props.checkedProjects && this.props.checkedProjects.length > 0;
    const deleteButtonVisible = this.props.isLoggedIn && checkedProjects;
    const uploadContainerClass =  uploadButtonVisible ? "" : "hidden";
    const deleteContainerClass = deleteButtonVisible ? "" : "hidden";
    const spacerClass = (uploadButtonVisible && deleteButtonVisible) ? "verticalSpacer" : "verticalSpacer hidden";
    const actionButtonContainerClass = uploadButtonVisible ? "actionButtonContainer" : "actionButtonContainer hidden";

    const showUploadProgress = this.props.uploadProgressShowing;

    return (
      <div className="tabContainer fullheight">
        <div className={actionButtonContainerClass}>
          <div id="projectList_uploadButton" className={uploadContainerClass}>
            <IconButton
              icon={<Upload24 />}
              title="Upload package"
              onClick={ () => { this.props.showUploadPackage(true); }} />
          </div>
          <div className={spacerClass}></div>
          <div id="projectList_deleteButton" className={deleteContainerClass}>
            <IconButton
              icon={<Trash24 />}
              title="Delete project(s)"
              onClick={ () => { this.props.showDeleteProject(true); }} />
          </div>
        </div>
        <div className="fullheight">
          <CheckboxTable
            onProjectClick={ (id) => { this.onProjectClick(id); }}
            selectable={this.props.isLoggedIn}
          />
        </div>

        <UploadPackage />
        {showUploadProgress && <ModalProgressUpload
                    open={true}
                    title={this.isDone() ? "Upload Finished" : "Uploading package"}
                    label={this.props.uploadPackageData.file.name}
                    icon='Archive.svg'
                    onClose={() => {this.onProgressCloseClick(); }}
                    onOpen={() => {this.onProgressOpenClick(); }}
                    url={null}
                    isDone={() => this.isDone() === true }
                    warningMsg={this.isDone() ? this.props.adoptWarning : null}
                    />}
        {this.props.uploadFailedShowing && <ModalFail
                    open={true}
                    title="Upload Failed"
                    contentName="Package:"
                    label={this.props.uploadPackageData.file.name}
                    onClose={() => this.onUploadFailedCloseClick()}
                    errorData={this.props.errorData}/>}

        <DeleteProject />
        {this.props.modalProgressShowing && <ModalProgress
                    open={this.props.modalProgressShowing}
                    title="Deleting Project(s)"
                    label="Deleting project(s) and cache"
                    icon="/Assembly_icon.svg"
                    onClose={() => this.onDeleteCloseClick()}/>
        }
      </div>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  const filename = uploadPackageData(store)?.file?.name;
  // use file name without extension as ID of uploaded project
  const uploadProjectName = filename ? (filename.substring(0, filename.lastIndexOf('.')) || filename) : null; // TODO: project name should be received from the server  
  const adoptWarning = getAdoptWarnings(uploadProjectName, store)?.join('\\n');
  return {
    isLoggedIn: store.profile.isLoggedIn,
    checkedProjects: checkedProjects(store),
    uploadProgressShowing: uploadProgressShowing(store),
    uploadProgressIsDone: uploadProgressIsDone(store),
    uploadPackageData: uploadPackageData(store),
    uploadProjectName: uploadProjectName,
    uploadFailedShowing: uploadFailedShowing(store),
    errorData: errorData(store),
    modalProgressShowing: modalProgressShowing(store),
    adoptWarning: adoptWarning
  };
}, { showUploadPackage, updateActiveProject, updateActiveTabIndex, setUploadProgressHidden, hideUploadFailed,
  showDeleteProject, showModalProgress, invalidateDrawing })(ProjectList);
