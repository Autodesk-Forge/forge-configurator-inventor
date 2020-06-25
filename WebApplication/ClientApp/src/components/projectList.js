import React, { Component } from 'react';
import { connect } from 'react-redux';
import 'react-base-table/styles.css';
import IconButton from '@hig/icon-button';
import { Upload24, Trash24 } from '@hig/icons';
import './projectList.css';
import { showUploadPackage, updateActiveTabIndex, showDeleteProject } from '../actions/uiFlagsActions';
import { setUploadProgressHidden } from '../actions/uploadPackageActions';
import { updateActiveProject } from '../actions/projectListActions';
import UploadPackage from './uploadPackage';
import DeleteProject from './deleteProject';

import ModalProgressUpload from './modalProgressUpload';
import { uploadProgressShowing, uploadProgressIsDone, uploadPackageData, checkedProjects } from '../reducers/mainReducer';
import CheckboxTable from './CheckboxTable';

export class ProjectList extends Component {

  isDone() {
    return this.props.uploadProgressIsDone;
  }

  onProgressCloseClick() {
    this.props.setUploadProgressHidden();
  }

  onProjectClick(projectId) {
    // set active project
    this.props.updateActiveProject(projectId);
    // switch to MODEL tab
    this.props.updateActiveTabIndex(1);
  }

  onProgressOpenClick() {

    this.props.setUploadProgressHidden();

    // switch to uploaded project
    const filename = this.props.uploadPackageData.file.name;
    // use file name without extension as ID of uploaded project
    const onlyName = filename.substring(0, filename.lastIndexOf('.')) || filename;
    this.props.updateActiveProject(onlyName);
    // switch to MODEL tab
    this.props.updateActiveTabIndex(1);
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
                    />}

        {/* use checkedProjects (array of project ids) for deletion */}
        <DeleteProject />
      </div>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  return {
    isLoggedIn: store.profile.isLoggedIn,
    checkedProjects: checkedProjects(store),
    uploadProgressShowing: uploadProgressShowing(store),
    uploadProgressIsDone: uploadProgressIsDone(store),
    uploadPackageData: uploadPackageData(store)
  };
}, { showUploadPackage, updateActiveProject, updateActiveTabIndex, setUploadProgressHidden, showDeleteProject })(ProjectList);
