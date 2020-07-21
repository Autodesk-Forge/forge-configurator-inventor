import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ProjectList } from './projectList';

Enzyme.configure({ adapter: new Adapter() });

const projectList = {
    activeProject: {
        id: '2'
    },
    projects: [
        {
            id: '1',
            label: 'One'
        },
        {
            id: '2',
            label: 'Two'
        },
        {
            id: '3',
            label: 'Three'
        }
    ]
};

const props = {
    projectList: projectList
};

describe('ProjectList components', () => {

  it('Project list has upload button for logged in user', () => {
    const propsWithProfile = { ...props, isLoggedIn: true };
    const wrapper = shallow(<ProjectList { ...propsWithProfile } />);
    const upload = wrapper.find('#projectList_uploadButton');
    expect(upload.prop("className")).not.toContain('hidden');
  });

  it('Project list has NO upload button for anonymous user', () => {
    const propsWithProfile = { ...props, isLoggedIn: false };
    const wrapper = shallow(<ProjectList { ...propsWithProfile } />);
    const upload = wrapper.find('#projectList_uploadButton');
    expect(upload.prop("className")).toContain('hidden');
  });

  it('Project list has NO delete button for anonymous user', () => {
    const propsWithProfile = { ...props, isLoggedIn: false, checkedProjects: [] };
    const wrapper = shallow(<ProjectList { ...propsWithProfile } />);
    const deleteBtn = wrapper.find('#projectList_deleteButton');
    expect(deleteBtn.prop("className")).toContain('hidden');
  });

  describe('click handlers', () => {
    const showUploadPackageMock = jest.fn();
    const showDeleteProjectMock = jest.fn();
    const updateActiveProjectMock = jest.fn();
    const updateActiveTabIndexMock = jest.fn();
    const setUploadProgressHiddenMock = jest.fn();
    const hideUploadFailedMock = jest.fn();
    const showModalProgressMock = jest.fn();

    const projectId = '3';
    const propsWithProfile = { ...props, isLoggedIn: true, uploadPackageData: { file: { name: projectId}} };
    // a bit of mess here, allowing to show all the dialogs at once, but it works, so...
    const wrapper = shallow(<ProjectList { ...propsWithProfile } uploadProgressShowing = {true}
      uploadFailedShowing = {true}
      modalProgressShowing = {true}
      showUploadPackage = {showUploadPackageMock}
      showDeleteProject = {showDeleteProjectMock}
      setUploadProgressHidden = {setUploadProgressHiddenMock}
      updateActiveProject = {updateActiveProjectMock}
      updateActiveTabIndex = {updateActiveTabIndexMock}
      hideUploadFailed = {hideUploadFailedMock}
      showModalProgress = {showModalProgressMock}
    />);

    beforeEach(() => {
      showUploadPackageMock.mockClear();
      showDeleteProjectMock.mockClear();
      updateActiveProjectMock.mockClear();
      updateActiveTabIndexMock.mockClear();
      setUploadProgressHiddenMock.mockClear();
      hideUploadFailedMock.mockClear();
      showModalProgressMock.mockClear();
    });

    it('handles click on upload button', () => {
      const uploadDiv = wrapper.find('#projectList_uploadButton');
      const button = uploadDiv.find('IconButton');
      button.simulate('click');
      expect(showUploadPackageMock).toHaveBeenCalledWith(true);
    });

    it('handles click on delete button', () => {
      const deleteDiv = wrapper.find('#projectList_deleteButton');
      const button = deleteDiv.find('IconButton');
      button.simulate('click');
      expect(showDeleteProjectMock).toHaveBeenCalledWith(true);
    });

    it('handles click on project row', () => {
      const table = wrapper.find('Connect(CheckboxTable)');
      table.simulate('ProjectClick', projectId);
      // project change
      expect(updateActiveProjectMock).toHaveBeenCalledWith(projectId);
      // model tab open
      expect(updateActiveTabIndexMock).toHaveBeenCalledWith(1);
    });

    it('handles click in progress dialog - close', () => {
      const modalProgressUpload = wrapper.find('ModalProgressUpload');
      modalProgressUpload.simulate('Close');
      expect(setUploadProgressHiddenMock).toHaveBeenCalledTimes(1);
      // NO project change
      expect(updateActiveProjectMock).toHaveBeenCalledTimes(0);
      // NO model tab open
      expect(updateActiveTabIndexMock).toHaveBeenCalledTimes(0);
    });

    it('handles click in progress dialog - open', () => {
      const modalProgressUpload = wrapper.find('ModalProgressUpload');
      modalProgressUpload.simulate('Open');
      expect(setUploadProgressHiddenMock).toHaveBeenCalledTimes(1);
      // project change
      expect(updateActiveProjectMock).toHaveBeenCalledWith(projectId);
      // model tab open
      expect(updateActiveTabIndexMock).toHaveBeenCalledWith(1);
    });

    it('handles click in fail dialog', () => {
      const modalFail = wrapper.find('ModalFail');
      modalFail.simulate('Close');
      expect(hideUploadFailedMock).toHaveBeenCalledTimes(1);
    });

    it('handles click in delete progress dialog', () => {
      const modalProgress = wrapper.find('ModalProgress');
      modalProgress.simulate('Close');
      expect(showModalProgressMock).toHaveBeenCalledTimes(1);
    });
  });
});
