import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { UploadPackage } from './uploadPackage';

Enzyme.configure({ adapter: new Adapter() });

const filename = 'a:\\b\\c.zip';

const rootasm = 'root.asm';

const props = {
    uploadPackageDlgVisible: true,
    projectAlreadyExists: false,
    package: { file: filename, root: rootasm },
    existsProject: false
};

const showUploadPackageMockFn = jest.fn();
const showUploadProgressMockFn = jest.fn();

describe('Upload package dialog', () => {

  beforeEach(() => {
    showUploadPackageMockFn.mockClear();
    showUploadProgressMockFn.mockClear();
  });

  it('Shows the stored package and root path', () => {
    const wrapper = shallow(<UploadPackage { ...props } />);
    const fileinput = wrapper.find('#package_file');
    expect(fileinput.prop("value")).toEqual(filename);
    const rootinput = wrapper.find('#package_root');
    expect(rootinput.prop("value")).toEqual(rootasm);
  });

  it('Upload button calls the upload handler', () => {
    const wrapper = shallow(<UploadPackage { ...props } showUploadPackage={showUploadPackageMockFn} showUploadProgress={showUploadProgressMockFn} />);
    const button = wrapper.find("#upload_button");
    button.simulate('click');
    expect(showUploadPackageMockFn).toHaveBeenCalledWith(false);
    expect(showUploadProgressMockFn).toHaveBeenCalledWith(filename);
  });


  it('Cancel button closes the dialog', () => {
    const wrapper = shallow(<UploadPackage { ...props } showUploadPackage={showUploadPackageMockFn} showUploadProgress={showUploadProgressMockFn} />);
    const button = wrapper.find("#cancel_button");
    button.simulate('click');
    expect(showUploadPackageMockFn).toHaveBeenCalledWith(false);
    expect(showUploadProgressMockFn).toHaveBeenCalledTimes(0);
  });

});
