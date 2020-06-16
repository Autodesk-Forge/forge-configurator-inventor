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
import { uploadPackageDlgVisible, uploadPackageData } from '../reducers/mainReducer';
import { showUploadPackage, editPackageFile, editPackageRoot } from '../actions/uiFlagsActions';
import './uploadPackage.css';

export class UploadPackage extends Component {

    constructor(props) {
        super(props);
        this.onPackageFileChange = this.onPackageFileChange.bind(this);
        this.onPackageRootChange = this.onPackageRootChange.bind(this);
    }

    onPackageFileChange(data) {
        this.props.editPackageFile(data.target.value);
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

        return (
            <Modal
            open={this.props.uploadPackageDlgVisible}
            title="Upload package"
            onCloseClick={() => { this.props.showUploadPackage(false); }}
            stylesheet={modalStyles} >
                <div id="uploadPackageModal">
                    <Label
                        variant="top"
                        disabled={false} >
                        Package
                    </Label>
                    <div className="fileBrowseContainer">
                        <Input id="package_file"
                            variant="box"
                            onChange={this.onPackageFileChange}
                            value={this.props.package.file}
                        />
                        <label htmlFor="packageFileInput">
                            <div>
                            <IconButton
                                icon={<Folder24 />}
                                title="Browse package"
                                onClick={ () => { document.getElementById("packageFileInput").click(); }}
                            />
                            </div>
                        </label>
                        <input id="packageFileInput"
                            type="file"
                            onChange={ (e) => {
                                this.onPackageFileChange(e);
                            }}
                        />
                    </div>
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
                    <Spacer  spacing="m"/>
                    <div className="buttonsContainer">
                        <Button
                            size="standard"
                            title="Upload"
                            type="primary"
                            onClick={() => { console.log('UPLOAD PACKAGE CLICKED'); }}
                        />
                        <div style={{width: '14px'}}/>
                        <Button
                            size="standard"
                            title="Cancel"
                            type="secondary"
                            onClick={() => { this.props.showUploadPackage(false); } }
                        />
                    </div>
                </div>
            </Modal>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store) {
    return {
      uploadPackageDlgVisible: uploadPackageDlgVisible(store),
      package: uploadPackageData(store)
    };
}, { showUploadPackage, uploadPackageData, editPackageFile, editPackageRoot })(UploadPackage);