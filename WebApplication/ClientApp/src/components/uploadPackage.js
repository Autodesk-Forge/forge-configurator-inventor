import React, { Component } from 'react';
import { connect } from 'react-redux';
import Modal from '@hig/modal';
import { uploadPackageDlgVisible } from '../reducers/mainReducer';
import { dispatchShowUploadPackage } from '../actions/uiFlagsActions';

export class UploadPackage extends Component {
    render() {
        return (
            <Modal
                open={this.props.uploadPackageDlgVisible}
                title="Upload package"
                onCloseClick={() => { this.props.dispatchShowUploadPackage(false); }} />
        );
    }
}

/* istanbul ignore next */
export default connect(function (store) {
    return {
      uploadPackageDlgVisible: uploadPackageDlgVisible(store)
    };
}, { dispatchShowUploadPackage })(UploadPackage);