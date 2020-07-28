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
    package: { file: { name: filename }, root: rootasm }
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

  it('File edit input is disabled', () => {
    const wrapper = shallow(<UploadPackage { ...props } editPackageFile={editPackageFileMockFn} editPackageRoot={editPackageRootMockFn} />);
    const input = wrapper.find("#package_file");
    expect(input.prop('disabled')).toBeTruthy();
    // we disabled it, we do not want it to call anything on change (reducers already called from the browse handler)
    const newFile = { name: 'newFile'};
    input.simulate('change', { target: { files: [ newFile ] }} );
    expect(editPackageFileMockFn).toHaveBeenCalledTimes(0);
    expect(editPackageRootMockFn).toHaveBeenCalledTimes(0);
  });

  it('Calls appropriate reducer on file edit thru hidden file-type label', () => {
    const wrapper = shallow(<UploadPackage { ...props } editPackageFile={editPackageFileMockFn} editPackageRoot={editPackageRootMockFn} />);
    const input = wrapper.find("#packageFileInput");
    const newFile = { name: 'newFile'};
    input.simulate('change', { target: { files: [ newFile ] }} );
    expect(editPackageFileMockFn).toHaveBeenCalledWith(newFile);
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

  it('Hides Top Level Assembly field when extension in not .zip', () => {
    const wrapper = shallow(<UploadPackage { ...{package: {file:{name:'part.ipt'}}} } />);
    expect(wrapper.find('#package_root').exists()).toBe(false);
  });

  it('Shows Top Level Assembly field when extension is .zip', () => {
    const wrapper = shallow(<UploadPackage { ...{package: {file:{name:'assembly.zip'}}} } />);
    expect(wrapper.find('#package_root').exists()).toBe(true);
  });

  it('Hides Top Level Assembly field when nothing is assigned', () => {
    const wrapper = shallow(<UploadPackage { ...{package: {file:null}} } />);
    expect(wrapper.find('#package_root').exists()).toBe(false);
  });
});
