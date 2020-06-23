import React, { Component } from 'react';
import { connect } from 'react-redux';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';
import IconButton from '@hig/icon-button';
import { Upload24 } from '@hig/icons';
import './projectList.css';
import { showUploadPackage, updateActiveTabIndex } from '../actions/uiFlagsActions';
import { setUploadProgressHidden } from '../actions/uploadPackageActions';
import { updateActiveProject } from '../actions/projectListActions';
import UploadPackage from './uploadPackage';

import ModalProgressUpload from './modalProgressUpload';
import { uploadProgressShowing, uploadProgressIsDone, uploadPackageData } from '../reducers/mainReducer';

const Icon = ({ iconname }) => (
  <div>
    <img src={iconname} alt='' width='16px' height='18px' />
  </div>
);

const iconRenderer = ({ cellData: iconname }) => <Icon iconname={iconname} />;

export const projectListColumns = [
  {
      key: 'icon',
      title: '',
      dataKey: 'icon',
      cellRenderer: iconRenderer,
      align: Column.Alignment.RIGHT,
      width: 100,
  },
  {
      key: 'label',
      title: 'Package',
      dataKey: 'label',
      align: Column.Alignment.LEFT,
      width: 200,
  }
];

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
    let data = [];
    if(this.props.projectList.projects) {
      data = this.props.projectList.projects.map((project) => (
        {
          id: project.id,
          icon: 'Archive.svg',
          label: project.label,
        }
      ));
    }

    const visible = this.props.isLoggedIn;
    const uploadContainerClass = visible ? "uploadContainer" : "uploadContainer hidden";
    const showUploadProgress = this.props.uploadProgressShowing;

    return (
      <div className="fullheight">
        <div id="projectList_uploadButton" className={uploadContainerClass}>
          <IconButton
            icon={<Upload24 />}
            title="Upload package"
            className="uploadButton"
            onClick={ () => { this.props.showUploadPackage(true); }} />
        </div>
        <div className="fullheight">
          <AutoResizer>
            {({ width, height }) => {

                return <BaseTable
                    width={width}
                    height={height}
                    columns={projectListColumns}
                    data={data}
                    rowEventHandlers={{
                      onClick: ({ rowData }) => { this.onProjectClick(rowData.id); }
                  }}
                />;
            }}
          </AutoResizer>
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
      </div>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  return {
    projectList: store.projectList,
    isLoggedIn: store.profile.isLoggedIn,
    uploadProgressShowing: uploadProgressShowing(store),
    uploadProgressIsDone: uploadProgressIsDone(store),
    uploadPackageData: uploadPackageData(store)
  };
}, { showUploadPackage, updateActiveProject, updateActiveTabIndex, setUploadProgressHidden })(ProjectList);
