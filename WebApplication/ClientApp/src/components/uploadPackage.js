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
import Modal from '@hig/modal';
import Label from '@hig/label';
import Input from '@hig/input';
import Button from '@hig/button';
import IconButton from '@hig/icon-button';
import Spacer from '@hig/spacer';
import { Folder24 } from '@hig/icons';
import merge from "lodash.merge";
import { uploadPackageDlgVisible, uploadPackageData, projectAlreadyExists } from '../reducers/mainReducer';
import { showUploadPackage, editPackageFile, editPackageRoot, setProjectAlreadyExists } from '../actions/uiFlagsActions';
import { uploadPackage } from '../actions/uploadPackageActions.js';
import './uploadPackage.css';
import { unzip, setOptions } from 'unzipit';
import Select from 'react-select';

export class UploadPackage extends Component {

    constructor(props) {
        super(props);
        this.onPackageFileChange = this.onPackageFileChange.bind(this);
        this.onPackageRootChange = this.onPackageRootChange.bind(this);
        this.getAssemblies = this.getAssemblies.bind(this);
    }

    async listZipAssemblies(file) {
        if (!file?.name.toLowerCase().endsWith('.zip'))
            return null;

        const assemblies = [];
        setOptions({ numWorkers: 0 });

        const {entries} = await unzip(file);
        Object.entries(entries).forEach(([name]) => {
            if (name.toLowerCase().endsWith('.iam')) {
                assemblies.push(name);
            }
        });

        return assemblies;
    }

    getAssemblies() {
        const hasAssemblies = this.props.package.assemblies && this.props.package.assemblies.length>0;
        let data = [];
        if (hasAssemblies)
            data = this.props.package.assemblies.map(item => { return { value: item, label: item }; });

        return data;
    }

    async onPackageFileChange(data) {
        if(data.target.files.length > 0) {
            const file = data.target.files[0];
            const assemblies = await this.listZipAssemblies(file);
            this.props.editPackageFile(file, assemblies);

            // preselect assembly if there is only one
            if (assemblies?.length === 1)
                this.props.editPackageRoot(assemblies[0]);
        }
    }

    onPackageRootChange(item, actionMeta) {
        if  (actionMeta.action === 'select-option') {
            this.props.editPackageRoot(item.value);
        }
    }

    shouldShowTopLevelAssembly() {
        return this.props.package.file?.name.endsWith('.zip');
    }

    render() {

        const height = this.shouldShowTopLevelAssembly() ? 327 : 280;
        const modalStyles = /* istanbul ignore next */ styles =>
        merge(styles, {
          modal: {
                window: { // by design
                    width: "370px",
                    height: `${height}px`
                },
                bodyContent: {
                    overflow: "hidden" // no scrollbar
                }
            }
        });
        const modalStylesConflict = /* istanbul ignore next */ styles =>
        merge(styles, {
          modal: {
                window: { // by design
                    width: "370px",
                    height: "180px"
                }
            }
        });

        const greyBorderColor = 'rgba(128,128,128,0.2)';
        const selectedBlueColor = '#0696d7';
        const selectControlHeight = '34px';
        const customSelectStyles = {
            control: (base) => ({ ...base,
                minHeight: {selectControlHeight}, // height
                height: {selectControlHeight}, // height
                boxShadow: null,
                fontSize: '14px'
            }),
            menu: styles => ({ ...styles, fontSize: '12px' }), // smaller menu fonts
            menuPortal: base => ({ ...base, zIndex: 10500 }), // allow menu to cross modal border
            valueContainer: (base) => ({
                ...base,
                minHeight: `${selectControlHeight}`, // height
                height: `${selectControlHeight}`, // height
                padding: '0 12px'
            }),
            indicatorSeparator: () => ({
                display: 'none',
            }),
            indicatorsContainer: (provided) => ({
                ...provided,
                height: `${selectControlHeight}`,
            }),
            singleValue: (provided) => ({
                ...provided,
                maxWidth: 'calc(100% - 20px)',
                direction: 'rtl', // make text ellipsis on the beginning of the text
                textAlign: 'left'
            })
        };

        const customSelectTheme = (theme) => ({
            ...theme,
            borderRadius: 0,
            fontFamily: 'ArtifaktElement, sans-serif',
            colors: {
              ...theme.colors,
              primary25: `${greyBorderColor}`,
              primary: `${selectedBlueColor}`,
            }
          });

        const data = this.getAssemblies();
        let selectedItem = null;
        if (this.props.package.root?.length > 0)
            selectedItem = data.find(element => element.value == this.props.package.root);// { value: this.props.package.root, label: this.props.package.root};

        return (
            <React.Fragment>
            <Modal
            open={this.props.uploadPackageDlgVisible}
            title="Upload package"
            onCloseClick={() => { this.props.showUploadPackage(false); }}
            stylesheet={modalStyles} >
                <div id="uploadPackageModal">
                    <div className="fileBrowseContainer">
                        <div className="stretch">
                            <Label
                                variant="top"
                                disabled={false} >
                                Package
                            </Label>
                            <Input id="package_file"
                                variant="box"
                                value={this.props.package.file?.name || ''}
                                disabled={true}
                            />
                        </div>
                        <div className="browseButton">
                            <label htmlFor="packageFileInput">
                                <IconButton
                                    icon={<Folder24 />}
                                    title="Browse package"
                                    onClick={ () => { document.getElementById("packageFileInput").click(); }}
                                />
                            </label>
                            <input id="packageFileInput"
                                type="file"
                                accept=".zip,.ipt"
                                onChange={ (e) => {
                                    this.onPackageFileChange(e);
                                }}
                            />
                        </div>
                    </div>
                    { this.shouldShowTopLevelAssembly() &&
                        <React.Fragment>
                            <Spacer spacing="xs"/>
                            <Label
                                variant="top"
                                disabled={false} >
                                Top Level Assembly
                            </Label>
                            <Select
                                id="package_root"
                                className="singleselect"
                                classNamePrefix="asm"
                                value={selectedItem}
                                isSearchable={true}
                                name="assemblies"
                                options={data}
                                styles={customSelectStyles}
                                theme={customSelectTheme}
                                maxMenuHeight={150}
                                menuPortalTarget={document.querySelector('body')}
                                onChange={ (item, actionMeta) => {this.onPackageRootChange(item, actionMeta);}}
                            />
                        </React.Fragment>
                    }
                    <Spacer  spacing="l"/>
                    <div className="buttonsContainer">
                        <Button
                            id="upload_button"
                            size="standard"
                            title="Upload"
                            type="primary"
                            onClick={() => {
                                this.props.uploadPackage();
                            }}
                            disabled={!this.props.package.root}
                        />
                        <div style={{width: '14px'}}/>
                        <Button
                            id="cancel_button"
                            size="standard"
                            title="Cancel"
                            type="secondary"
                            onClick={() => { this.props.showUploadPackage(false); } }
                        />
                    </div>
                </div>
            </Modal>
            {this.props.projectAlreadyExists && <Modal
            open={true}
            title="Warning"
            onCloseClick={() => { this.props.setProjectAlreadyExists(false); }}
            stylesheet={modalStylesConflict} >Project already exists, please choose different file to upload or rename it.</Modal>
            }
            </React.Fragment>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store) {
    return {
      uploadPackageDlgVisible: uploadPackageDlgVisible(store),
      projectAlreadyExists: projectAlreadyExists(store),
      package: uploadPackageData(store)
    };
}, { showUploadPackage, uploadPackageData, editPackageFile, editPackageRoot, uploadPackage, setProjectAlreadyExists })(UploadPackage);