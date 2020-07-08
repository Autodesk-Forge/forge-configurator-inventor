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

export class UploadPackage extends Component {

    constructor(props) {
        super(props);
        this.onPackageFileChange = this.onPackageFileChange.bind(this);
        this.onPackageRootChange = this.onPackageRootChange.bind(this);
    }

    onPackageFileChange(data) {
        if(data.target.files.length > 0) {
            this.props.editPackageFile(data.target.files[0]);
        }
    }

    onPackageRootChange(data) {
        this.props.editPackageRoot(data.target.value);
    }

    render() {
        const modalStyles = /* istanbul ignore next */ styles =>
        merge(styles, {
          modal: {
                window: { // by design
                    width: "370px",
                    height: "327px"
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
                                accept=".zip"
                                onChange={ (e) => {
                                    this.onPackageFileChange(e);
                                }}
                            />
                        </div>
                    </div>
                    <Spacer  spacing="xs"/>
                    <Label
                        variant="top"
                        disabled={false} >
                        Top Level Assembly
                    </Label>
                    <Input id="package_root"
                        variant="box"
                        onChange={this.onPackageRootChange}
                        value={this.props.package.root}
                    />
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