import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { UploadPackage } from './uploadPackage';

Enzyme.configure({ adapter: new Adapter() });

const filename = 'a:\\b\\c.zip';

const rootasm = 'root.asm';

const props = {
    uploadPackageDlgVisible: true,
    projectAlreadyExists: false,
    package: { file: filename, root: rootasm }
};

const showUploadPackageMockFn = jest.fn();
const uploadPackageMockFn = jest.fn();
const editPackageFileMockFn = jest.fn();
const editPackageRootMockFn = jest.fn();

describe('Upload package dialog', () => {

  beforeEach(() => {
    showUploadPackageMockFn.mockClear();
    uploadPackageMockFn.mockClear();
    editPackageFileMockFn.mockClear();
    editPackageRootMockFn.mockClear();
  });

  it('Shows the stored package and root path', () => {
    const wrapper = shallow(<UploadPackage { ...props } />);
    const fileinput = wrapper.find('#package_file');
    expect(fileinput.prop("value")).toEqual(filename);
    const rootinput = wrapper.find('#package_root');
    expect(rootinput.prop("value")).toEqual(rootasm);
  });

  it('Upload button calls the upload handler', () => {
    const wrapper = shallow(<UploadPackage { ...props } showUploadPackage={showUploadPackageMockFn} uploadPackage={uploadPackageMockFn} />);
    const button = wrapper.find("#upload_button");
    button.simulate('click');
    expect(uploadPackageMockFn).toHaveBeenCalled();
  });


  it('Cancel button closes the dialog', () => {
    const wrapper = shallow(<UploadPackage { ...props } showUploadPackage={showUploadPackageMockFn} showUploadProgress={uploadPackageMockFn} />);
    const button = wrapper.find("#cancel_button");
    button.simulate('click');
    expect(showUploadPackageMockFn).toHaveBeenCalledWith(false);
    expect(uploadPackageMockFn).toHaveBeenCalledTimes(0);
  });

  it('Calls appropriate reducer on file edit', () => {
    const wrapper = shallow(<UploadPackage { ...props } editPackageFile={editPackageFileMockFn} editPackageRoot={editPackageRootMockFn} />);
    const input = wrapper.find("#package_file");
    const newFileName = 'newFile';
    input.simulate('change', { target: { value: newFileName }} );
    expect(editPackageFileMockFn).toHaveBeenCalledWith(newFileName);
    expect(editPackageRootMockFn).toHaveBeenCalledTimes(0);
  });

  it('Calls appropriate reducer on file edit thru hidden file-type label', () => {
    const wrapper = shallow(<UploadPackage { ...props } editPackageFile={editPackageFileMockFn} editPackageRoot={editPackageRootMockFn} />);
    const input = wrapper.find("#packageFileInput");
    const newFileName = 'newFile';
    input.simulate('change', { target: { value: newFileName }} );
    expect(editPackageFileMockFn).toHaveBeenCalledWith(newFileName);
    expect(editPackageRootMockFn).toHaveBeenCalledTimes(0);
  });

  it('Calls appropriate reducer on root edit', () => {
    const wrapper = shallow(<UploadPackage { ...props } editPackageFile={editPackageFileMockFn} editPackageRoot={editPackageRootMockFn} />);
    const input = wrapper.find("#package_root");
    const newRootAsm = 'newRoot';
    input.simulate('change', { target: { value: newRootAsm }} );
    expect(editPackageFileMockFn).toHaveBeenCalledTimes(0);
    expect(editPackageRootMockFn).toHaveBeenCalledWith(newRootAsm);
  });
});
