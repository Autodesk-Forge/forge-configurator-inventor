import React, { Component } from 'react';
import { connect } from 'react-redux';
import Modal from '@hig/modal';
import Label from '@hig/label';
import Input from '@hig/input';
import Spacer from '@hig/spacer';
import { uploadPackageDlgVisible, uploadPackageData } from '../reducers/mainReducer';
import { dispatchShowUploadPackage, editPackageFile, editPackageRoot } from '../actions/uiFlagsActions';

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
        return (
            <Modal
            open={this.props.uploadPackageDlgVisible}
            title="Upload package"
            onCloseClick={() => { this.props.dispatchShowUploadPackage(false); }} >
                <div>
                    <form id="a_form">
                        <Label
                        variant="top"
                        disabled={false} >
                        Package
                        </Label>
                        <Spacer spacing="s" />
                        <Input id="package_file"
                        variant="box"
                        onChange={this.onPackageFileChange}
                        value={this.props.package.file} />
                        <Spacer spacing="s" />
                        <Label
                        variant="top"
                        disabled={false} >
                        Top Level Assembly
                        </Label>
                        <Spacer spacing="s" />
                        <Input id="package_root"
                        variant="box"
                        onChange={this.onPackageRootChange}
                        value={this.props.package.root} />
                    </form>
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
}, { dispatchShowUploadPackage, uploadPackageData, editPackageFile, editPackageRoot })(UploadPackage);