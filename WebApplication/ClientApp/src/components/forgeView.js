import React, { Component } from 'react';
import Script from 'react-load-script';
import {connect} from 'react-redux';
import { getActiveProject } from '../reducers/mainReducer';
import './forgeView.css';
import Message from './message';
import repo from '../Repository';

let Autodesk = null;

export class ForgeView extends Component {

    constructor(props){
      super(props);

      this.viewerDiv = React.createRef();
      this.viewer = null;
    }

    handleScriptLoad() {

        const options = repo.hasAccessToken() ?
                            { accessToken: repo.getAccessToken() } :
                            { env: 'Local' };

        Autodesk = window.Autodesk;

        const container = this.viewerDiv.current;
        this.viewer = new Autodesk.Viewing.GuiViewer3D(container);

        // uncomment this for Viewer debugging
        //this.viewer.debugEvents(true);

        Autodesk.Viewing.Initializer(options, this.handleViewerInit.bind(this));
    }

    handleViewerInit() {
        const errorCode = this.viewer.start();
        if (errorCode)
            return;

        // orient camera in the same way as it's on the thumbnail
        // corresponding to ViewOrientationTypeEnum.kIsoTopRightViewOrientation
        const viewer = this.viewer;
        this.viewer.addEventListener(Autodesk.Viewing.EXTENSION_LOADED_EVENT, (event) => {

            const viewCubeExtensionId = "Autodesk.ViewCubeUi";

            // ER: it's not perfect, because the view transition is visible, so this is
            // a place to improve someday
            if (event.extensionId === viewCubeExtensionId) {

                const viewCubeUI = event.target.getExtension(viewCubeExtensionId);
                viewCubeUI.setViewCube("front top right");

                viewer.removeEventListener(Autodesk.Viewing.EXTENSION_LOADED_EVENT);
            }
        });

        // skip loading of svf when here is no active project svf
        if (!this.props.activeProject.svf)
            return;

        Autodesk.Viewing.Document.load(
            this.getSvfUrl(), this.onDocumentLoadSuccess.bind(this), () => {}
        );
    }

    componentDidUpdate(prevProps) {
        if (Autodesk && (this.props.activeProject.svf !== prevProps.activeProject.svf)) {
            Autodesk.Viewing.Document.load(
                this.getSvfUrl(), this.onDocumentLoadSuccess.bind(this), () => {}
            );
        }
    }

    getSvfUrl() {
        return this.props.activeProject.svf + `/bubble.json`;
    }

    onDocumentLoadSuccess(viewerDocument) {
        const defaultModel = viewerDocument.getRoot().getDefaultGeometry();
        this.viewer.loadDocumentNode(viewerDocument, defaultModel);
    }

    render() {
        return (
            <div className="modelContainer fullheight">
                <Message/>
                <div className="viewer" id="ForgeViewer">
                    <div ref={this.viewerDiv}></div>
                    <link rel="stylesheet" type="text/css" href={`https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/style.css`}/>
                    <Script url={`https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/viewer3D.js`}
                      onLoad={this.handleScriptLoad.bind(this)}
                    />
                </div>
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
      activeProject: getActiveProject(store)
    };
  })(ForgeView);